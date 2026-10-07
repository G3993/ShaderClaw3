// SCFONT/1 metrics-texture codec (Phase D, "Words on the Wall").
// Shared by tools/fonts/gen_msdf_atlas.mjs and tests/font-assets.test.mjs.
// A browser copy of encodeGlyphTexels/resolveFace lives in js/msdf-metrics.js —
// KEEP THE TWO IN SYNC (tests/font-assets.test.mjs compares their output).
//
// Texture layout (contract: .claude/caption-pipeline/phase-D-design.md §1.3):
// RGBA8, 2048 x 256, 8 texels per code point, U+0000..U+FFFF.
//   texel x = (cp mod 256) * 8 + k, page = cp / 256.
//   Texture-space rule: page p occupies GL row p counting from v = 0 (bottom).
//   Every host uploads the PNG "upright" (PNG bottom row at v = 0), so the
//   PNG stores page p at row 255 - p from the top (default rowOrder).
//   k=0: (atlasL lo, hi, atlasB lo, hi)  u16 atlas px, rounded
//   k=1: (atlasR lo, hi, atlasT lo, hi)
//   k=2: (planeL, planeB)  s16 fixed round(em*4096)+32768
//   k=3: (planeR, planeT)
//   k=4: (advance lo, hi, present 255|0, 255)
//   k=5..7: zero (reserved). Absent code points: all-zero texels.

import zlib from 'node:zlib';

export const TEX_W = 2048;
export const TEX_H = 256;
export const TEXELS_PER_GLYPH = 8;
export const FIXED_DENOM = 4096;

// glyphsJson: the SCFONT/1 face JSON ({ glyphs: [...] }) or a bare glyphs array.
// opts.rowOrder: 'top-down' (default; PNG row order, page p at row 255-p) or
// 'bottom-up' (raw GL upload with FLIP_Y=false, page p at buffer row p).
export function encodeGlyphTexels(glyphsJson, opts = {}) {
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

// Face resolution (§1.2, identical in all hosts): family index -> families row
// (|| families[0]); among its faces pick the weight nearest `weight`
// (default 400; ties -> lower). weightById maps face id -> weight; when a face
// is missing from the map the trailing "-NNN" of the id is used.
export function resolveFace(family, weight, fontsIndex, weightById) {
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

// ---- minimal PNG codec (RGBA8, filter-0 rows) ----------------------------

let _crcTable = null;
function crc32(buf) {
  if (!_crcTable) {
    _crcTable = new Int32Array(256);
    for (let n = 0; n < 256; n++) {
      let c = n;
      for (let k = 0; k < 8; k++) c = (c & 1) ? (0xEDB88320 ^ (c >>> 1)) : (c >>> 1);
      _crcTable[n] = c;
    }
  }
  let c = 0xFFFFFFFF;
  for (let i = 0; i < buf.length; i++) c = _crcTable[(c ^ buf[i]) & 0xFF] ^ (c >>> 8);
  return (c ^ 0xFFFFFFFF) >>> 0;
}

function pngChunk(type, data) {
  const out = Buffer.alloc(8 + data.length + 4);
  out.writeUInt32BE(data.length, 0);
  out.write(type, 4, 'latin1');
  data.copy(out, 8);
  out.writeUInt32BE(crc32(out.subarray(4, 8 + data.length)), 8 + data.length);
  return out;
}

// rgba: Uint8Array(width*height*4) in top-down row order.
export function encodePng(rgba, width, height) {
  const stride = width * 4;
  const raw = Buffer.alloc((stride + 1) * height);
  for (let y = 0; y < height; y++) {
    raw[y * (stride + 1)] = 0; // filter type 0 (None)
    Buffer.from(rgba.buffer, rgba.byteOffset + y * stride, stride).copy(raw, y * (stride + 1) + 1);
  }
  const ihdr = Buffer.alloc(13);
  ihdr.writeUInt32BE(width, 0);
  ihdr.writeUInt32BE(height, 4);
  ihdr[8] = 8;  // bit depth
  ihdr[9] = 6;  // color type RGBA
  ihdr[10] = 0; ihdr[11] = 0; ihdr[12] = 0;
  return Buffer.concat([
    Buffer.from([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]),
    pngChunk('IHDR', ihdr),
    pngChunk('IDAT', zlib.deflateSync(raw, { level: 9 })),
    pngChunk('IEND', Buffer.alloc(0)),
  ]);
}

// Decodes an RGBA8 PNG whose rows all use filter 0 (what encodePng writes).
// Returns { width, height, data: Uint8Array(top-down RGBA) }.
export function decodePng(buffer) {
  const buf = Buffer.isBuffer(buffer) ? buffer : Buffer.from(buffer);
  const sig = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
  for (let i = 0; i < 8; i++) if (buf[i] !== sig[i]) throw new Error('not a PNG');
  let pos = 8, width = 0, height = 0;
  const idats = [];
  while (pos + 8 <= buf.length) {
    const len = buf.readUInt32BE(pos);
    const type = buf.toString('latin1', pos + 4, pos + 8);
    const data = buf.subarray(pos + 8, pos + 8 + len);
    if (type === 'IHDR') {
      width = data.readUInt32BE(0);
      height = data.readUInt32BE(4);
      if (data[8] !== 8 || data[9] !== 6) throw new Error('decodePng: expected 8-bit RGBA, got depth ' + data[8] + ' colorType ' + data[9]);
      if (data[12] !== 0) throw new Error('decodePng: interlaced PNGs unsupported');
    } else if (type === 'IDAT') {
      idats.push(data);
    } else if (type === 'IEND') {
      break;
    }
    pos += 12 + len;
  }
  const raw = zlib.inflateSync(Buffer.concat(idats));
  const stride = width * 4;
  if (raw.length !== (stride + 1) * height) throw new Error('decodePng: unexpected data length');
  const out = new Uint8Array(stride * height);
  for (let y = 0; y < height; y++) {
    const f = raw[y * (stride + 1)];
    if (f !== 0) throw new Error('decodePng: only filter-0 rows supported (row ' + y + ' uses ' + f + ')');
    out.set(raw.subarray(y * (stride + 1) + 1, (y + 1) * (stride + 1)), y * stride);
  }
  return { width, height, data: out };
}
