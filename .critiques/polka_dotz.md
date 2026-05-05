## 2026-05-05
**Prior rating:** 1.9★
**Approach:** 2D refine (effect shader requiring input image — polka-dot mosaic with flip animation)

**Critique:**
- *Reference fidelity:* Polka-dot mosaic flip is charming but the muted `bgColor [0.2, 0.3, 0.4]` gap color makes the effect look grimy and gray. LED-wall metaphor needs dark black/navy gaps with bright dots, not gray with gray.
- *Compositional craft:* Gap shading is subtle but applies toward a desaturated default; the dot-vs-gap contrast that makes LEDs read as LEDs is lost.
- *Technical execution:* `specAmt * spec` with default 0.5 barely lifts specular above base color. No center-of-dot brightness boost — dots look uniform when they should peak in the center.
- *Liveness:* `dotPulse` + `audioReact` exist but specular is too weak to see the pulse.
- *Differentiation:* Looks washed out at defaults; needs the black-gap / bright-dot contrast of a real LED wall.

**Changes:**
- Changed `bgColor` default to deep navy `[0.0, 0.02, 0.15]` — dark gaps create hard contrast with lit dots
- Changed `specAmt` default 0.5→1.2, MAX extended to 5.0
- Specular multiplied 3.0×: `specAmt * spec * 3.0` — peaks at 3.6 linear at default
- Added dot-center HDR bloom: center 40% of each dot gets `col * dotCenter * (0.8 + bass*react*0.6)` — creates the characteristic hot-center LED look with audio reactivity

**HDR peaks reached:** ~3.0–4.0 (specular edge), ~2.5× center bloom on audio peaks
**Estimated rating:** 3★
<!-- auto-improve 2026-05-05 -->
