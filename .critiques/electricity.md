## 2026-05-05
**Prior rating:** 2.1★
**Approach:** 2D refine (plasma arc displacement field — the 2D canvas is the medium)

**Critique:**
- *Reference fidelity:* The simplex noise arc formula (displacement + pow4 accumulation) is a solid Jacob's ladder implementation. But `arcColor [0.95, 0.95, 0.95]` — near-white — destroys all saturation. Every render looks like white lightning regardless of `hueShift`.
- *Compositional craft:* 3 arcs at default is too sparse; the canvas often feels mostly black. Arc accumulation peaks at ~2.4 linear (white) which is HDR but colorless.
- *Technical execution:* `hueShift` default 0.0 means the cosine palette is never mixed in. Arc-center peak via `arcCol^4 * 3_arcs ≈ [2.43, 2.43, 2.43]` — HDR but still neutral white.
- *Liveness:* `audioReact` works well. Branching at 0.4 is visible but sparse.
- *Differentiation:* Looks like every other white lightning shader. The formula is unique; the color is not.

**Changes:**
- Changed `arcColor` default to electric purple `[0.7, 0.0, 1.0]` — saturated plasma/Jacob's ladder color
- Changed `hueShift` default 0.0→0.3 — cosine palette now blends in 30%, giving arc-to-arc hue variety
- Changed `arcCount` default 3→5 — denser arc field, more HDR stacking across overlapping arcs
- Changed `branching` default 0.40→0.60 — more visible fork bolts
- Added `hdrBoost` input (default 2.0, MAX 5.0) — scales `col += acc * hdrBoost`; peak blues at `5 * 1.0^4 * 2.0 = 10.0` linear on dense overlapping arcs

**HDR peaks reached:** ~4–10 linear (5 purple arcs × hdrBoost=2.0 × arcCol^4=1.0 for blue channel)
**Estimated rating:** 3★
<!-- auto-improve 2026-05-05 -->
