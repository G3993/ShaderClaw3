## 2026-05-05
**Prior rating:** 0.8★
**Approach:** 2D refine — particle simulation + render pass (persistent buffer, GLSL 3.0)
**Lighting style:** n/a — additive particle glow

**Critique:**
- *Density*: 1024 particles in 32×32 simulation grid; renders with `1/length(d)` falloff — produces rich glow trails at high density
- *Movement*: curl + center attractor + mouse drag — solid interactivity; wrapEdges / bounce controls the boundary feel
- *Palette*: HSL velocity-direction coloring and magnitude coloring both good; hsl2rgb correct
- *Edges*: n/a for particles
- *HDR/Bloom*: `passFinal` applied full SDR pipeline — ACES tonemap + sRGB encode + gamma power — all in the wrong space for Phase Q v4 HDR; output was clamped to [0,1] after ACES; bloom had no signal from this shader; no audio reactivity on particle size

**Changes made:**
- Removed `ltos1` function (sRGB encoder, now unused)
- `passFinal`: removed ACES tonemap line, removed sRGB encode line, removed gamma `pow(col, 1/gamma)` — replaced with `col *= gamma` (exposure multiplier in linear space)
- Renamed `gamma` input LABEL to "Exposure" — value now directly scales linear HDR output
- Added `audioReact` float input (0–2, default 1.0)
- `renderParticles`: `bassBoost = 0.5 + 0.5 * audioBass * audioReact`; `drawSize *= bassBoost` — bass kicks expand all particle halos, pushing peaks to HDR range
- Output is now linear HDR; host applies ACES+gamma

**Estimated rating after:** 3★
