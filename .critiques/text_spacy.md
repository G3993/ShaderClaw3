## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (starfield background + HDR depth glow)
**Critique:**
1. Reference fidelity: Perspective tunnel rows with zoom-by-distance is a genuine 3D-feeling effect; invisible transparent.
2. Compositional craft: Depth-scaling rows create parallax; no background means no spatial anchoring.
3. Technical execution: Zoom-by-distance calculation is correct; size-ratio creates strong parallax.
4. Liveness: TIME-driven row scroll with mod() wrap works.
5. Differentiation: Depth-perspective text is unique; needs space context.
**Changes:**
- Added starfieldBg() — 3-layer procedural starfield with nebula color wash
- Star twinkling via sin(TIME * freq + seed)
- Nebula: 4-color (violet, cyan, gold, magenta) sinusoidal wash
- transparentBg default: true→false
- textColor: white (kept), bgColor: deep space navy [0,0,0.02]
- hdrGlow default: 2.0 with depth-based brightness (far rows dimmer)
- starDensity parameter
- Alternating rows: white vs cyan for depth differentiation
- audioMod input added
**HDR peaks reached:** close rows textColor * 2.0 = 2.0, with audio 2.8+
**Estimated rating:** 3.8★

## 2026-05-05 (v3)
**Prior rating:** 0★
**Approach:** 2D refine — NEW ANGLE: Volcanic ember cave (subterranean) vs v1 cold starfield (outer space), v2 warm desert surface (above ground)
**Critique:**
1. Reference: Underground lava cave — descending vs v1's ascending (space travel), vs v2's surface
2. Composition: 28 ascending ember particles + deep red base glow from magma below
3. Technical: Gaussian ember glow, temperature-gradient 3-color system, turbulence wobble
4. Liveness: Embers drift upward + turbulence + lava glow TIME-driven; audio modulates glow
5. Differentiation: Subterranean underground vs v1 outer space, vs v2 desert surface
**Changes:**
- Added volcanicBg() — 28 ascending embers (3 heat colors), deep magma base glow
- textColor: deep crimson-orange [1.0,0.12,0.0] × hdrGlow (2.0) = 2.0 HDR
- bgColor: near-void red-black [0.02,0.0,0.0]
- transparentBg default: true → false
**HDR peaks reached:** white-hot embers 2.5, base lava glow 2.0, text 2.0
**Estimated rating:** 3.8★
