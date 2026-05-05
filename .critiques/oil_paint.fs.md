## 2026-05-05
**Prior rating:** 0.0 (unrated)
**Approach:** 2D refine
**Lighting style:** painterly
**Critique:**
- Reference fidelity: 3 — Kuwahara filter + relief lighting gives convincing oil-paint stroke quality; impasto bumps read well
- Compositional craft: 2 — entirely dependent on inputImage composition; no generative spatial layer
- Technical execution: 3 — two-pass Kuwahara is correct; specular uses fixed light direction; no TIME or audio use
- Liveness: 1 — completely static; same output every frame in silence; no temporal evolution
- Differentiation: 3 — Kuwahara + normal-map relief is a known technique but well executed for the format
**Changes made:**
- TIME-driven light-angle shimmer (+/-15 deg oscillation at shimmerSpeed) gives living candle-quality
- Micro-jitter on Kuwahara sample offsets (sin-driven, sub-pixel) for painterly breathing in silence
- Audio-reactive brush radius: brushRadius * (0.7 + 0.6 * amod) — strokes loosen on bass hits
- HDR impasto peaks: bright paint ridges (ridge mask > 0.5) pushed above 1.0 via impastoPeak for host bloom
- fwidth-based AA on the ridge transition zone in the relief pass for smoother specular boundary
- Specular range widened to 0-3.0 for HDR highlight bursts above 1.0
- Added shimmerSpeed, audioReact, impastoPeak inputs
- Output linear HDR — no clamp / ACES
**Estimated rating after:** 3★
**What to study next:** Anisotropic Kuwahara (structure-tensor eigenvectors) for stroke-direction fidelity; Sobel flow field for brush direction from image edges
