## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: VIDVOX 8-frame buffer delay — requires inputImage; produces nothing standalone.
2. Compositional craft: Frame buffering is purely an effect; no content without source.
3. Technical execution: 9-pass persistent buffer architecture is complex and correct, but all passes output noise without input.
4. Liveness: TIME-driven via random delay shift, but input-dependent.
5. Differentiation: Interesting channel-split temporal effect; not a generator.
**Changes:**
- Full rewrite as "Signal Interference" — raymarched 3D RGB data planes with glitch geometry
- Three independently marched color planes (R/G/B) at Y-offsets (planeOffset parameter)
- Each plane: scanlines + column bars + glitch blocks as SDF geometry
- Per-channel glitch: horizontal displacement driven by hash(floor(y * 8 + t * rate))
- HDR: signal red, data green, electric blue — fully saturated
- White-hot specular peak on hit surfaces
- Camera slowly sweeps through the planes (sin(t * 0.13))
- hdrBoost parameter (default 2.0)
- audioMod modulates displacement and brightness
**HDR peaks reached:** per-channel hdrBoost * diffuse = 2.0; white spec adds ~2.5
**Estimated rating:** 4.0★

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D Voronoi glitch — NEW ANGLE: 2D spatial chromatic mosaic vs prior 3D RGB channel-split data planes.
**Critique:**
1. Reference fidelity: Original required inputImage (8-frame buffer). This is a standalone generative glitch mosaic.
2. Compositional craft: Voronoi cell boundaries create strong ink-black contrast; chromatic aberration at boundaries adds visual sharpness.
3. Technical execution: Per-cell Voronoi with hash22 jitter; glitch trigger per-cell (independent cycle); RGB channels sampled at ±chromaAmt offset positions.
4. Liveness: Cells drift slowly via sin(TIME + jitter); per-cell flash triggered independently; audioBass/audioHigh modulate chroma split.
5. Differentiation: 2D mosaic vs 3D planes; spatial (cells) vs temporal (frame delay); Voronoi vs voxel-column geometry.
**Changes:**
- Full rewrite: 2D Voronoi chromatic-aberration mosaic
- Per-cell palette phase + glitch flash trigger (independent per cell)
- RGB channels sampled at chromaAmt lateral offsets (colour fringing at boundaries)
- Ink-black cell boundary via fwidth + smoothstep
- White-hot edge glow via exp() falloff
- 4-colour palette: red/blue/green/gold (fully saturated, no white mixing)
- Audio: audioBass/audioHigh modulate chroma split width
**HDR peaks reached:** cell bodies 2.5, flash moments up to 3.75, edge glow 2.8
**Estimated rating:** 4.0★
