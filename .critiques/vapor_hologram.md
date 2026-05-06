# vapor_hologram critique log

## Entry 1 — prior (Vaporwave Hologram — 2-pass 2D scene)
- **Technique**: 2-pass ISF: pass 0 renders full 2D vaporwave scene (banded sun, perspective grid, Y2K SDF shapes, katakana rain); pass 1 layers holographic glitch (scanlines, RGB shift, tear, EMI bursts, tint)
- **Lighting**: Flat 2D gradient + neon color tinting; no 3D shading
- **Composition**: Classic 80s/vaporwave aesthetic: banded sun above horizon, perspective floor grid, floating Y2K objects, hologram overlay
- **Color grading**: Hot pink sky gradient, cyan-turquoise horizon, hologram tint vec3(0.55, 1.0, 0.95); saturated but in the vaporwave palette
- **Reference**: Vaporwave / Y2K aesthetic (Floral Shoppe, Macintosh Plus)
- **Weaknesses**: 2D flat composition with no depth traversal; perspective grid is approximate; hologram glitch is a fixed post-process layer; lacks first-person immersion; audio modulates grid speed only

## Entry 2 — v17 (3D Neon City Night Drive)
- **Technique**: Single-pass SDF raymarcher (80 steps); domain-repeated SDF box buildings in Z; forward camera motion; fwidth-AA ground grid; neon strip SDF emissive geometry
- **Lighting**: HDR neon strips ×2.5 emissive; audio-modulated pulse per strip; neon shimmer reflected on dark pavement (Fresnel-like power function); building window glow via hash pattern
- **Composition**: First-person street-level city canyon; camera moves forward continuously, subtle head-bob and lateral weave; neon strips line both walls; buildings recede in distance with exponential fog
- **Color grading**: Full hue spectrum (slowly rotating per building via paletteShift + TIME); deep black void + warm orange horizon glow; neon haze columns in sky; no desaturation
- **Differentiation axes**: dimensionality (2D flat→full 3D raymarcher), movement (static camera→forward fly-through), geometry (flat grid→SDF building canyons), lighting (flat tint→HDR emissive neon), palette (fixed vaporwave pink/cyan→full rotatable spectrum)
