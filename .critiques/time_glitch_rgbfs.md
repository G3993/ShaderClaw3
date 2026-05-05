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
**Approach:** 3D raymarch — NEW ANGLE: prior 3D RGB scan planes (flat planes sweeping through) → 3D grid of glowing cubes (volumetric voxel field, per-cube per-channel pulse)
**Critique:**
1. Reference fidelity: Prior used flat plane geometry with RGB offset; new uses 3D repeated cube SDFs — different primitive vocabulary entirely.
2. Compositional craft: Grid of N×N×N cubes seen from diagonal isometric-ish orbit — creates structured geometric composition with black gap contrast.
3. Technical execution: sdBox with mod() repetition, cellId from floor(), per-cell R/G/B channel assignment by hash dominance, 100-step march.
4. Liveness: TIME-driven per-cube RGB pulse (independent frequencies); audio modulates pulse amplitude.
5. Differentiation: Cube primitive vs plane SDF; per-cell color assignment vs RGB channel split; isometric orbit vs straight-through camera; spatial 3D field vs flat RGB overlay.
**Changes:**
- Full rewrite as 3D voxel grid
- sdBox with mod() rep and cellId
- Per-cube dominant channel (R/G/B) from hash
- Independent pulse frequency per cube (sin(t * pulseSpeed * (0.6 + h) + phase))
- Audio boosts pulse amplitude directly
- Isometric orbit camera (camA = t * 0.09)
- Pure black background for maximum contrast
- Black gap ink via fwidth on cube edges
**HDR peaks reached:** electric cube emission 2.6, white spec 2.2, combined cluster 3.0+
**Estimated rating:** 4.2★
