# SCFONT/1 font tooling (Phase D — shared MSDF atlas)

Generates the checked-in MSDF font assets under `assets/fonts/`:

- `<id>.msdf.png` — MSDF atlas (msdf-atlas-gen, `-yorigin bottom`, pxrange 6)
- `<id>.json` — msdf-atlas-gen JSON verbatim + a top-level `sctext` object (SCFONT/1)
- `<id>.metrics.png` — RGBA8 2048x256 per-codepoint metrics texture
  (8 texels per code point; see `metrics-codec.mjs` for the exact layout)
- `fonts.json` — face index (family index -> face ids, weight resolution)
- `src/` — the OFL-licensed source TTFs + their `OFL-*.txt` license texts

Contract: `.claude/caption-pipeline/phase-D-design.md` §1.1–1.3. Consumers:
claw3 `js/media.js` (+ `js/msdf-metrics.js`, a copy of the codec — keep in
sync), easel `FontAtlas.cpp`, etherea `claw3-font-atlas.js`,
easel `tools/render_isf.py`.

## Regenerate

```
node tools/fonts/gen_msdf_atlas.mjs --force            # regenerate all faces
node tools/fonts/gen_msdf_atlas.mjs --fetch-tool       # (re)download msdf-atlas-gen
node tools/fonts/gen_msdf_atlas.mjs --outfit --force   # also build outfit-400/700
```

Tool resolution order: `MSDF_ATLAS_GEN` env var → `tools/fonts/.bin/**/msdf-atlas-gen.exe`
→ download `msdf-atlas-gen-1.3-win64.zip` from the Chlumsky/msdf-atlas-gen
GitHub releases into `tools/fonts/.bin/` (gitignored). If the tool cannot be
obtained it falls back to `npx msdf-bmfont-xml -t msdf -f json` plus a
BMFont→SCFONT converter; if neither works only `fonts.json` is written and
every consumer runs its synthesized-grid fallback.

Fonts: Inter Regular/Bold (rsms/inter release v4.1, `extras/ttf`),
Barlow Semi Condensed SemiBold (google/fonts `ofl/barlowsemicondensed`),
optionally Outfit variable (google/fonts `ofl/outfit`). All OFL 1.1.

Notes:
- Faces without a U+FFFD glyph get it aliased to `?` at generation time so
  `scGlyphResolve` always lands on a present glyph.
- The Arabic ranges in `charset.txt` are reserved; Inter/Barlow/Outfit ship no
  Arabic glyphs, so those code points stay absent (all-zero metrics texels).
- Verify with `node --test tests/font-assets.test.mjs`.
