## 2026-05-05
**Prior rating:** 0★
**Approach:** 2D refine (abstract expressionist paint generator)
**Lighting style:** painterly (raking oblique impasto light, upper-left key)
**Critique:**
1. Reference fidelity 0/5 — prior shader was a Kuwahara image filter requiring `inputImage`; zero standalone visual, no TIME liveness, no audio reactivity, no palette.
2. Compositional craft 0/5 — no composition at all; filter-only pass with no procedural field.
3. Technical execution 0/5 — no ISF generator structure, no FBM, no HDR path, no fwidth AA, ISFVSN missing.
4. Liveness 0/5 — entirely static without input image.
5. Differentiation 0/5 — generic Kuwahara implementation indistinguishable from dozens of filter shaders.
**Changes made:**
- Complete rewrite as standalone ISF generator (ISFVSN "2") — no inputImage required.
- Procedural scene: two-level domain-warped FBM (`warpedFbm` uses `q → r → fbm(p + warpAmt*r)`) for layered wet-paint-on-paint topology, avoiding flat uniform noise hills.
- Five stroke layers at different scales and anisotropic biases (`strikeBias` compresses UV on one axis) to produce Kline-style wide horizontal sweeps vs. Twombly diagonal calligraphy.
- Four palette modes: Kline Black+Ivory (default), Twombly Ochre+Red, Cobalt+Cream, Night Palette — 5 named pigment constants each drawn from historical oil-paint formulations.
- TIME-driven churn: two-frequency drift (`sin(t*0.71) + cos(t*1.17)`) plus a slow canvas-rotation (`sin(t*0.19)*0.06`) keeps the composition alive at audio=0.
- Audio modulator: `(0.5 + 0.5 * audioBass * audioReact)` drives `strokeWidth` and gloss intensity; baseline 0.5 keeps strokes visible at silence.
- Impasto raking light: central-difference height gradient → pseudo-normal → Blinn-Phong diffuse + specular; specular lifted to 1.5–2.0 HDR range for ACES glisten.
- Wet-paint gloss: HDR additive ivory gloss on brightest primary strokes, peaks ~2.0.
- Twombly scribble trails: high-frequency `warpedFbm` TIME-animated at `3.9×` scale with fwidth-based AA.
- Canvas linen weave: biorthogonal sine product, visible only through thin paint.
- Vignette: darkens corners with `pow(vig, 0.5)` profile to frame the composition.
- Linear HDR output: no gamma, no ACES, no clamp — host pipeline applies ACES tonemapping.
**Estimated rating after:** 3.5★
