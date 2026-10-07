// SCFONT/1 asset tests — run: node --test tests/font-assets.test.mjs
// Verifies the checked-in assets/fonts/* against the Phase D contract
// (.claude/caption-pipeline/phase-D-design.md §1.2-1.3) and that the browser
// codec copy (js/msdf-metrics.js) matches tools/fonts/metrics-codec.mjs.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { encodeGlyphTexels, decodePng, resolveFace, TEX_W, TEX_H, TEXELS_PER_GLYPH, FIXED_DENOM } from '../tools/fonts/metrics-codec.mjs';
import '../js/msdf-metrics.js'; // side effect: globalThis.SCMSDFMetrics

const ROOT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const FONTS = path.join(ROOT, 'assets', 'fonts');
const index = JSON.parse(fs.readFileSync(path.join(FONTS, 'fonts.json'), 'utf8'));

// texel accessor for the TOP-DOWN (PNG order) buffer: page p at row 255 - p
function texel(data, cp, k) {
  const page = cp >> 8, lo = cp & 255;
  const o = ((TEX_H - 1 - page) * TEX_W + lo * TEXELS_PER_GLYPH + k) * 4;
  return [data[o], data[o + 1], data[o + 2], data[o + 3]];
}
const u16 = (t) => t[0] + 256 * t[1];
const s16em = (t) => (u16(t) - 32768) / FIXED_DENOM;

test('fonts.json shape: version, default/caption faces, family indices 0..4', () => {
  assert.equal(index.version, 1);
  assert.ok(index.faces.includes(index.default), 'default face listed');
  assert.ok(index.faces.includes(index.caption), 'caption face listed');
  assert.equal(index.caption, 'barlow-semicondensed-600');
  const idxs = index.families.map((f) => f.index);
  assert.deepEqual(idxs, [0, 1, 2, 3, 4]);
  for (const fam of index.families) {
    for (const id of fam.faces) {
      assert.ok(index.faces.includes(id), `families[${fam.index}] face ${id} is in faces[]`);
      assert.ok(fs.existsSync(path.join(FONTS, id + '.json')), id + '.json exists');
    }
  }
});

test('face resolution: nearest weight, ties lower; caption family', () => {
  assert.equal(resolveFace(0, 400, index, null), 'inter-400');
  assert.equal(resolveFace(0, 700, index, null), 'inter-700');
  assert.equal(resolveFace(4, 400, index, null), 'barlow-semicondensed-600');
  assert.equal(resolveFace(99, 400, index, null), 'inter-400'); // unknown family -> families[0]
  // browser copy agrees
  const B = globalThis.SCMSDFMetrics;
  assert.equal(B.resolveFace(0, 700, index, null), 'inter-700');
  assert.equal(B.resolveFace(4, 400, index, null), 'barlow-semicondensed-600');
});

for (const id of index.faces) {
  const json = JSON.parse(fs.readFileSync(path.join(FONTS, id + '.json'), 'utf8'));

  test(`${id}: SCFONT/1 json shape`, () => {
    assert.equal(json.sctext.version, 1);
    assert.equal(json.sctext.id, id);
    assert.equal(json.atlas.yOrigin, 'bottom');
    assert.ok(json.atlas.width <= 2048 && json.atlas.height <= 2048,
      `atlas ${json.atlas.width}x${json.atlas.height} <= 2048`);
    assert.deepEqual(json.sctext.metricsLayout,
      { width: TEX_W, height: TEX_H, texelsPerGlyph: TEXELS_PER_GLYPH, fixedPointDenominator: FIXED_DENOM });
    assert.ok(fs.existsSync(path.join(FONTS, json.sctext.atlas)), 'atlas png exists');
    assert.ok(fs.existsSync(path.join(FONTS, json.sctext.metricsTexture)), 'metrics png exists');
  });

  test(`${id}: every atlasBounds inside the atlas`, () => {
    for (const g of json.glyphs) {
      const b = g.atlasBounds;
      if (!b) continue;
      assert.ok(b.left >= 0 && b.bottom >= 0 && b.right <= json.atlas.width && b.top <= json.atlas.height &&
        b.left < b.right && b.bottom < b.top,
        `U+${g.unicode.toString(16)} bounds ${JSON.stringify(b)} inside ${json.atlas.width}x${json.atlas.height}`);
    }
  });

  test(`${id}: metrics png decodes to exactly encodeGlyphTexels(json)`, () => {
    const png = decodePng(fs.readFileSync(path.join(FONTS, json.sctext.metricsTexture)));
    assert.equal(png.width, TEX_W);
    assert.equal(png.height, TEX_H);
    const expect = encodeGlyphTexels(json);
    assert.equal(png.data.length, expect.length);
    assert.ok(Buffer.from(png.data).equals(Buffer.from(expect)), 'byte-for-byte equal');
  });

  test(`${id}: spot checks — 'A' page 0, U+FFFD present, 'a' advance sane`, () => {
    const data = encodeGlyphTexels(json);
    // 'A' present on page 0 (row 255 from the top)
    assert.equal(texel(data, 65, 4)[2], 255, "'A' present flag");
    assert.ok(u16(texel(data, 65, 1)) > u16(texel(data, 65, 0)), "'A' atlasR > atlasL");
    // U+FFFD present (page 255 -> top PNG row)
    assert.equal(texel(data, 0xFFFD, 4)[2], 255, 'U+FFFD present flag');
    // 'a' advance within 0.2..1.0 em
    const adv = s16em(texel(data, 97, 4));
    assert.ok(adv > 0.2 && adv < 1.0, `'a' advance ${adv} in 0.2..1.0 em`);
    // space: present, advance > 0, zero-area atlas rect
    assert.equal(texel(data, 32, 4)[2], 255, 'space present');
    assert.ok(s16em(texel(data, 32, 4)) > 0, 'space advance > 0');
    // absent code point (Arabic block, no glyphs in these faces): all-zero texels
    for (let k = 0; k < 8; k++) assert.deepEqual(texel(data, 0x0601, k), [0, 0, 0, 0]);
  });

  test(`${id}: browser codec copy (js/msdf-metrics.js) matches, both row orders`, () => {
    const B = globalThis.SCMSDFMetrics;
    const a = encodeGlyphTexels(json);
    const b = B.encodeGlyphTexels(json);
    assert.ok(Buffer.from(a).equals(Buffer.from(b)), 'top-down equal');
    const a2 = encodeGlyphTexels(json, { rowOrder: 'bottom-up' });
    const b2 = B.encodeGlyphTexels(json, { rowOrder: 'bottom-up' });
    assert.ok(Buffer.from(a2).equals(Buffer.from(b2)), 'bottom-up equal');
    // bottom-up really is the row-reversed top-down buffer
    const stride = TEX_W * 4;
    const rowFlipped = Buffer.alloc(a.length);
    for (let y = 0; y < TEX_H; y++) {
      Buffer.from(a.buffer, a.byteOffset + y * stride, stride).copy(rowFlipped, (TEX_H - 1 - y) * stride);
    }
    assert.ok(rowFlipped.equals(Buffer.from(a2)), 'bottom-up == flipud(top-down)');
  });
}
