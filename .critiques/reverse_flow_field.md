## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (HDR palette replacement)
**Critique:**
1. Reference fidelity: Flow field algorithm (cellular FBM backward trace) is well-executed and matches "wind-blown grass tips" reference.
2. Compositional craft: Grass gradient is desaturated (black→forest green→gray→white) — indistinct at small sizes.
3. Technical execution: Multi-pass ISF correctly implemented; Bezier weight curve is sophisticated.
4. Liveness: TIME-driven via flow offset, but temporal feels slow.
5. Differentiation: Interesting LIC-style approach; killed by the gray/white palette giving near-zero saturation score.
**Changes:**
- Replaced grass gradient with volcanic magma palette: black→deep crimson→orange→gold→white-hot HDR
- Seed dot colors changed from random→3 fire hues (deep ember, orange, gold)
- intensity default: 1.0→2.5 (HDR boost)
- dotDensity default: 0.1→0.12
- audioMod input added, modulates flow speed and direction field
- HDR peak: magma top ramp → 3.0× white-hot on high-intensity seeds
**HDR peaks reached:** white-hot seeds 3.0, gold 2.0, orange 1.3
**Estimated rating:** 3.5★

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D volumetric — NEW ANGLE: 2D volcanic magma palette swap → 3D volumetric aurora curtains
**Critique:**
1. Reference fidelity: Wind-blown flow field replaced with atmospheric aurora phenomenon — volumetric ribbon curtains bending in magnetic field.
2. Compositional craft: Wide upward camera + starfield background creates proper spatial scale; vertical ribbons give strong vertical hierarchy.
3. Technical execution: 64-step volumetric transmittance march; FBM warp drives curtain shape; exponential transmittance accumulation.
4. Liveness: Animated curtain warp + wave speed; drifting camera; twinkling stars; audio boosts density.
5. Differentiation: 3D volumetric vs 2D LIC-style flow; aurora palette (cyan/violet/gold) vs volcanic magma; wide sky composition vs grass-tip close-up.
**Changes:**
- Full rewrite from 2D flow field to 3D volumetric aurora
- Transmittance-based 64-step volume march
- FBM ribbon density function with height envelope
- Palette: electric cyan, violet, gold — cycling by horizontal phase
- Deep navy background + procedural stars
- Camera pointing up, slow drift
- Audio modulates density*brightness
**HDR peaks reached:** ribbon center density accumulation * 2.5 = 2.5+; with audio 3.0+
**Estimated rating:** 4.5★

## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** 2D Neon Silk Ribbons — NEW ANGLE: 2D volcanic palette (v1) → 3D volumetric aurora (v2) → 2D iso-contour ribbons on domain-warped field (v4)
**Critique:**
1. Reference fidelity: Flow field aesthetic preserved but rendered as glowing iso-contour ribbons rather than particle traces — still feels like flowing silk.
2. Compositional craft: 14 ribbons cycle through cyan/magenta/gold/violet; double domain-warp creates complex flowing curves; void black ground maximizes saturation contrast.
3. Technical execution: fwidth-based iso-contour width; black ink gap between adjacent ribbons (BG mix on ribbon centers); domain warp at two octaves.
4. Liveness: warpSpeed drives field evolution; warpAmt controls turbulence; audio modulates brightness.
5. Differentiation: 2D iso-contour vs v2 3D volumetric; ribbon/fabric aesthetic vs aurora curtain; tighter 4-color palette vs cycling hue gradient.
**Changes:**
- Full rewrite from 3D aurora to 2D iso-contour ribbon system
- Double domain warp: q = sin/cos layer, r = secondary warp on (p+q)
- field() = triple sinusoidal product for rich topology
- 24 potential ribbon iso-lines (default 14)
- fwidth-based adaptive ribbon width — no aliasing
- Black ink gap between ribbon edges
- Palette: cyan (0,2.5,2.3), magenta (2.5,0.05,1.8), gold (2.4,1.7,0), violet (1.5,0,2.5)
**HDR peaks reached:** all 4 colors at hdrBoost*audio = 2.5–3.5+
**Estimated rating:** 4.5★
