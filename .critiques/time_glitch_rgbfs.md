## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D kaleidoscopic RGB prism fractal — NEW ANGLE: 2D geometry (vs 3D signal planes in v1)
**Critique:**
1. Reference fidelity: Original was input-dependent. Standalone 2D chromatic prism is distinct and generative.
2. Compositional craft: Kaleidoscopic fold + iterative inversion creates complex symmetric geometry.
3. Technical execution: 3 channels offset in time + UV for chromatic split; 5-iter fractal loop; hash-based block dropout.
4. Liveness: TIME-driven rotation + zoom + per-channel phase offset = constant motion.
5. Differentiation: 2D kaleidoscope vs v1's 3D raymarched signal planes; prism geometry vs plane geometry.
**Changes:**
- FULL REWRITE: no more inputImage dependency
- 2D kaleidoscopic fold (kFold function, variable folds param)
- Iterative inversion fractal (5 iters) with per-iteration glitch displacement
- 3-channel chromatic split: R/G/B each offset in time + UV
- Block-dropout noise for glitch feel
- Palette: signal red, data green, electric blue + white-hot hotspots
- hdrBoost default 2.5; glitchAmt controls chromatic split + dropout intensity
**HDR peaks reached:** channel * 2.5 = 2.5; white-hot alignment peaks = 2.0 additional
**Estimated rating:** 4.5★

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
