## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** Multi-pass (palette change) — NEW ANGLE: street art graffiti colors (vs. HDR saturation boost v2, Coral Bioluminescence SDF reef v3 — no biological/marine theme)
**Critique:**
1. Reference fidelity: Street art / graffiti has a distinct visual vocabulary: bold primary+secondary colors, high contrast against dark surfaces, spray-can bloom.
2. Compositional craft: 10 walkers with fixed signature colors create a vibrant color field; wider paint radius creates thick brush-stroke feel; ink edge darkening creates strong contrast.
3. Technical execution: Fixed 6-color palette per walker ID (no HSV drift); 3×3 neighborhood paint for thick strokes; luminance-based ink edge crushing.
4. Liveness: Walker motion unchanged (hash-based stepping); color is stable per walker; bloom spreads the HDR paint.
5. Differentiation: No biological colors, no drifting hue — fixed, bold, urban.
**Changes:**
- Fixed 6-color graffiti palette: fire red, orange, yellow, neon green, electric blue, violet
- All colors at 2.8× HDR
- 3×3 cell neighborhood painting (wider stroke)
- Luminance-based ink edge darkening
- walkers: 6→10, bloom: 0.35→0.6, brightness: 1.0→2.8
- Concrete grey background
**HDR peaks reached:** paint strokes * 2.8, bloom spreads to ~1.8 neighboring cells
**Estimated rating:** 4.0★
