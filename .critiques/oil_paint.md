## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: Original Kuwahara filter requires inputImage — nothing to paint without source. Clever technique, wrong category.
2. Compositional craft: Zero standalone composition; effect pass without a generator pass.
3. Technical execution: Multi-pass Kuwahara is correctly implemented but useless as a standalone generator.
4. Liveness: No TIME-driven content; the painterly effect is static relative to input.
5. Differentiation: Kuwahara approach is elegant but requires input.
**Changes:**
- Full rewrite as "Lava Impasto" — standalone 3D molten rock surface
- Domain-warped FBM height field as raymarched displaced plane (64-step)
- Lava palette: black → deep crimson → orange → gold → white-hot (HDR)
- Time-driven flow using animated domain warp (flowSpeed parameter)
- Hot-spot pulse with TIME * 3.1 for liveness
- Charred crevice edge darkening via fwidth(rawH) AA
- Cinematic camera angled down onto surface, drifting slowly
- Audio modulates pulse intensity
**HDR peaks reached:** white-hot crack edges 3.0, gold flow 1.5–2.5, orange mid-tone 1.0
**Estimated rating:** 4.5★

## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Volumetric metaball paint cloud (prior 2026-05-05 was 3D displaced lava plane, never committed)
**Critique:**
1. Reference fidelity: Metaball cluster references Pollock / abstract expressionist paint throw — distinct from lava impasto.
2. Compositional craft: 8 blobs in golden-angle arrangement creates organic but balanced composition.
3. Technical execution: smin blending, per-blob color weighting, normal via central differences, fwidth() ink edges.
4. Liveness: Blobs slowly drift/pulse with TIME and audio bass.
5. Differentiation: Different 3D primitive (metaballs vs displaced FBM plane); different palette (Zorn vs lava); different lighting (painterly vs cinematic).
**Changes:**
- Full rewrite from Kuwahara filter to 3D metaball paint cloud
- 8 golden-angle blobs with smooth-min blending
- Palette: vermillion/cobalt/ochre — classic Zorn limited palette at HDR levels
- Painterly warm-key + cool-fill lighting + gloss specular
- Ink outline via fwidth() edge darkening
- Audio bass modulates blob scale
**HDR peaks reached:** blob surfaces at hdrPeak * diff = 2.5+; specular peaks ~1.5 additional
**Estimated rating:** 4.0★
