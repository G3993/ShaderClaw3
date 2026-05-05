## 2026-05-05
**Prior rating:** 0.9★
**Approach:** 3D REWRITE — ray-plane intersection onto marble slab, same wisp algorithm applied to 3D surface UV; Blinn-Phong with dFdx/dFdy bump map
**Lighting style:** cinematic — warm overhead key with calcite specular (HDR peaks)

**Critique:**
- *Density*: sinusoidal wisp accumulation (200-iter loop) already produces dense layered veins; good complexity
- *Movement*: scroll + twist + TIME-driven drift working; audioBass modulates rotation rate
- *Palette*: veinColor/baseColor user-configurable; kintsugi gold vein surprise every 45s
- *Edges*: no AA on wisps (they're anti-aliased by the `1/y` formula naturally); no fwidth needed here
- *HDR/Bloom*: `intensity = clamp(totalGlow, 0.0, 3.0)` — clamped and used as a [0,1] ratio, never outputting HDR; `marble += veinColor * intensity * 0.4` at most 0.4 above base; kintsugi gold was `mix(..., 0.85)` capped at 1.0; no surface lighting, no specular

**Changes made (full 3D rewrite with analytical ray-plane hit):**
- Slow orbital camera (7°/s) above marble slab (±1.5 units); ray-plane intersection at y=0
- Same wisp loop applied at hit.xz marble UV (mousePos, zoomAmt, rotateCanvas preserved)
- dFdx/dFdy bump normal from wisp gradient field — no extra loop evaluations; calcite surface looks 3D
- Blinn-Phong: diffuse `0.1 + diff * 0.9`, specular `pow(H·N, 48) * 2.5 * bass` → peaks ~1.8 HDR
- Removed `clamp(totalGlow, 0.0, 3.0)` — raw intensity used; `marble += veinColor * intensity * 0.5` — additive, HDR-capable
- Kintsugi gold: `mix → col += gold * 2.0` — additive, peaks 2.0 HDR for bloom
- Added `audioReact` input; vein slab edge glow for dimensional boundary read
- Added "3D" to CATEGORIES; transparentBg preserved

**Estimated rating after:** 4★
