// SCFONT/1 metrics codec — BROWSER COPY of tools/fonts/metrics-codec.mjs
// (encodeGlyphTexels / resolveFace only; the PNG codec is Node-only).
// KEEP IN SYNC with tools/fonts/metrics-codec.mjs — tests/font-assets.test.mjs
// compares the two implementations byte-for-byte.
// Classic script (index.html, after js/text-buffer.js); node can also import it
// as a side-effect module (it only assigns globalThis.SCMSDFMetrics).
(function () {
  const TEX_W = 2048, TEX_H = 256, TEXELS_PER_GLYPH = 8, FIXED_DENOM = 4096;

  // glyphsJson: SCFONT/1 face JSON ({ glyphs: [...] }) or a bare glyphs array.
  // opts.rowOrder: 'top-down' (default; PNG row order, page p at row 255-p) or
  // 'bottom-up' (raw GL upload with FLIP_Y=false, page p at buffer row p).
  function encodeGlyphTexels(glyphsJson, opts) {
    opts = opts || {};
    const glyphs = Array.isArray(glyphsJson) ? glyphsJson : ((glyphsJson && glyphsJson.glyphs) || []);
    const bottomUp = opts.rowOrder === 'bottom-up';
    const out = new Uint8Array(TEX_W * TEX_H * 4);
    const u16 = (v) => Math.max(0, Math.min(65535, Math.round(v)));
    const s16em = (v) => Math.max(0, Math.min(65535, Math.round(v * FIXED_DENOM) + 32768));
    for (const g of glyphs) {
      const cp = g.unicode;
      if (cp == null || cp < 0 || cp > 0xFFFF) continue;
      const page = cp >> 8;
      const lo = cp & 255;
      const row = bottomUp ? page : (TEX_H - 1 - page);
      const base = (row * TEX_W + lo * TEXELS_PER_GLYPH) * 4;
      const put = (k, r, gg, b, a) => {
        const o = base + k * 4;
        out[o] = r; out[o + 1] = gg; out[o + 2] = b; out[o + 3] = a;
      };
      const ab = g.atlasBounds || null;
      const pb = g.planeBounds || null;
      const aL = u16(ab ? ab.left : 0), aB = u16(ab ? ab.bottom : 0);
      const aR = u16(ab ? ab.right : 0), aT = u16(ab ? ab.top : 0);
      put(0, aL & 255, aL >> 8, aB & 255, aB >> 8);
      put(1, aR & 255, aR >> 8, aT & 255, aT >> 8);
      const pL = s16em(pb ? pb.left : 0), pB = s16em(pb ? pb.bottom : 0);
      const pR = s16em(pb ? pb.right : 0), pT = s16em(pb ? pb.top : 0);
      put(2, pL & 255, pL >> 8, pB & 255, pB >> 8);
      put(3, pR & 255, pR >> 8, pT & 255, pT >> 8);
      const adv = s16em(g.advance || 0);
      put(4, adv & 255, adv >> 8, 255, 255);
      // k = 5..7 stay zero (reserved)
    }
    return out;
  }

  // Face resolution (phase-D-design.md §1.2, identical in all hosts).
  function resolveFace(family, weight, fontsIndex, weightById) {
    const fams = (fontsIndex && fontsIndex.families) || [];
    if (!fams.length) return (fontsIndex && fontsIndex.default) || null;
    const idx = Math.round(Number(family) || 0);
    let fam = null;
    for (const f of fams) if (f.index === idx) { fam = f; break; }
    if (!fam) fam = fams[0];
    const w = (weight == null || isNaN(Number(weight))) ? 400 : Number(weight);
    let best = null, bestD = Infinity, bestW = Infinity;
    for (const id of (fam.faces || [])) {
      let fw = (weightById && weightById[id] != null) ? Number(weightById[id]) : null;
      if (fw == null) {
        const m = /-(\d+)$/.exec(id);
        fw = m ? Number(m[1]) : 400;
      }
      const d = Math.abs(fw - w);
      if (d < bestD || (d === bestD && fw < bestW)) { best = id; bestD = d; bestW = fw; }
    }
    return best;
  }

  globalThis.SCMSDFMetrics = { encodeGlyphTexels, resolveFace, TEX_W, TEX_H, TEXELS_PER_GLYPH, FIXED_DENOM };
})();
