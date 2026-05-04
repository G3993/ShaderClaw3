# color_picker — critique log

## 2026-05-04
**Prior rating:** 0.0 (overall, scores.json 0–10 scale)
**Approach:** 3D raymarch
**Lighting style:** studio (3-point: warm key + cool fill + neutral rim)

**Reference sources:**
- HSV cylindrical color space: https://www.researchgate.net/figure/HSV-color-space-in-cylindrical-coordinates_fig1_331257472
- Johannes Itten 12-part color circle (Bauhaus, 1921): https://edouardfouche.com/The-Extended-Color-Wheel-by-Itten/
- Raymarching SDF tutorial: https://varun.ca/ray-march-sdf/

**Critique:**
- Reference fidelity: 3 — The 12 hue spheres match Itten's primary/secondary/tertiary divisions; the `color` input highlights the nearest sphere, making the "picker" concept tangible in 3D. Central ivory sphere echoes Itten's neutral hub. Could push harder on the Bauhaus aesthetic (flat ground plane, stark contrasts).
- Compositional craft: 4 — Chrome torus frames the ring cleanly; dark marble checker platform anchors depth; camera orbits give reveal moments. Depth fog unifies. The warm-key / cool-fill split creates pleasing color temperature contrast on the spheres.
- Technical execution: 4 — Compiles clean, fwidth AA on checker iso-surface, soft shadows (16-step), 64-step march with early exit. No banding. `audioReact` modulates sphere size as `(0.5 + 0.5*audio)`, never gates. TIME-driven at all audio levels.
- Liveness: 4 — Ring rotates continuously (0.22 rad/s), camera orbits (0.07 rad/s) with vertical drift, individual spheres bob at unique phases. Selected-hue highlight pulses via exponential falloff. Stays alive with zero audio.
- Differentiation: 4 — The Itten color-circle is a specific, named reference. The highlight mechanic (pick a hue, the matching sphere enlarges and brightens) directly enacts the "color picker" concept in 3D space rather than a generic particle/noise scene.

**Changes made:**
- Removed `inputImage` filter dependency — rewritten as standalone generator.
- Added `audioReact` float input (modulates sphere size, never gates).
- Built SDF scene: 12 hue spheres (Itten wheel), central ivory sphere, chrome torus ring, dark checker platform slab.
- Studio 3-point lighting with 16-step soft shadows.
- fwidth-AA checkerboard on platform SDF iso-surface.
- Slow ring rotation + individual sphere bob (TIME-driven, silence stays alive).
- HSV → RGB palette grounded in Itten's fully-saturated color circle.
- Depth fog (exp decay).
- CATEGORIES updated: `["Generator", "3D", "Color"]`.
- Output is linear HDR; ACES applied host-side.

**Estimated rating after:** 4.0★ (0–10 → ~4.0 overall)
**What to study next:** Add a Bauhaus-style ground plane with De Stijl grid lines. Try volumetric glow on the highlighted sphere (accumulate density along ray within sphere radius). The central ivory sphere could show a preview swatch of the picked color rather than plain ivory.
