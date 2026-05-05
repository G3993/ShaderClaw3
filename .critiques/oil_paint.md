
## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D painterly procedural — NEW ANGLE: Impressionist domain-warp vs prior 3D lava impasto
**Critique:**
1. Reference fidelity: Kuwahara required inputImage. Prior fix: 3D lava surface. This: 2D Monet/Matisse color-field approach — completely different reference and dimension.
2. Compositional craft: Domain-warped color patches mimic oil paint gesture strokes. Brush-scale dividers create natural composition blocks.
3. Technical execution: Double-warp FBM (iq technique), 4-octave noise, per-pixel fwidth edge detection for ink separators.
4. Liveness: Two independent warp-time axes (q-warp and r-warp at different speeds), audio-reactive saturation.
5. Differentiation: 2D vs 3D, Impressionist palette vs lava, color-field painting vs physical simulation.
**Changes:**
- Full rewrite as "Impressionist Fields" — standalone 2D domain-warp generator
- 5-color palette: cadmium red, viridian, cobalt blue, aureolin gold, deep lilac — all fully saturated
- Double-domain-warp FBM (Inigo Quilez technique): warp → warp → sample
- Black ink dividers via fwidth(v) edge magnitude
- HDR: smoothstep brightness map pushes bright regions to 2.4×
- Contrast parameter for push-to-saturated-extremes
- Audio: modulates saturation mix (beat = more saturated)
- No inputImage dependency
**HDR peaks reached:** bright field regions 2.4× hdrPeak × audio, ink edges 0.0 (black)
**Estimated rating:** 4.3★
