# vapor_hologram critique log

---

## v3 — 2026-05-05

**Prior rating:** 0.00  
**Prior angles:** v1 vapor/hologram aesthetic (scanlines, iridescent palette), v2 holographic grid with neon glow

### Approach — NEW ANGLE: Japanese Torii Gate at Dusk

A fully standalone 2D generator depicting a torii gate silhouette at twilight. Abandons the hologram/vapor wave aesthetic entirely in favour of a painterly Japanese scene: layered dusk sky gradient, animated paper lanterns, drifting cherry blossom petals, and a zenith star field. The torii gate is constructed from composite `sdBox()` SDFs with proper structural detail (kasagi top beam, nuki lower beam, shimaki cap, komainu base pedestals). SDF edges antialiased via `fwidth()`. No texture inputs — all geometry is procedural.

### 5-axis Critique

| Axis | Score | Notes |
|------|-------|-------|
| Density | 0.72 | Multiple overlapping layers (sky, horizon, ground, gate, lanterns, petals, stars) create rich depth |
| Movement | 0.68 | Lantern sway, petal drift, star flicker, slow sky shift all animate smoothly |
| Palette | 0.82 | Vermillion/gold/indigo triadic scheme is culturally cohesive; SAKURA_PINK adds warmth |
| Edges | 0.76 | fwidth() AA on gate SDF and all lantern/petal SDFs; horizon glow softened with smoothstep |
| Overall | 0.75 | Striking composition with clear silhouette hierarchy and HDR glow on lanterns |

### Changes from v2

- Full rewrite from hologram/grid generator to Japanese dusk scene
- `sdTorii()`: composite box SDF for accurate torii structure with 7 primitives
- `sdLantern()`: oval body + top/bottom caps
- 3-stop sky gradient with TIME-animated slow shift
- Cherry blossom petal drift using `fract()` periodic placement
- Audio modulator: `aud = 1.0 + (audioLevel + audioBass*0.5) * audioReact * 0.4`
- HDR: lantern glow peaks `GOLD_SKY * 1.5 * hdrBoost` (up to 3.0 with default hdrBoost=2.0)

### HDR peaks

- Lantern ambient glow: `GOLD_SKY * pulse * 0.8 * hdrBoost * aud` ≈ 1.5 at peak
- Horizon line: `GOLD_SKY * 1.5 * hdrBoost * aud` ≈ 3.0 at peak
- Star shimmer: `GOLD_SKY * 0.6 * hdrBoost` ≈ 1.2

### Estimated rating: 0.75
