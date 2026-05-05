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

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D volumetric — NEW ANGLE: prior 2D text perspective rows with cool starfield → 3D volumetric warp-speed tunnel with warm-violet nebula (amber/magenta palette)
**Critique:**
1. Reference fidelity: Prior was 2D text rows zooming by in flat perspective; new is full 3D volumetric nebula + warp-star streaks — completely different modality.
2. Compositional craft: Straight-ahead camera into deep space, nebula clouds fill mid-field, warp stars radiate from center — strong radial composition.
3. Technical execution: 3D FBM nebula density march (48 steps), star streak via segment-closest-point, static star field via hash.
4. Liveness: TIME-driven nebula flow, star warp, camera slight oscillation; audio modulates warp star brightness.
5. Differentiation: 3D volumetric vs 2D flat; warm violet/amber vs cool blue/white; warp star streaks vs static dots; continuous nebula cloud vs discrete text glyphs.
**Changes:**
- Full rewrite as 3D volumetric nebula warp
- 48-step volumetric FBM nebula march
- Warp star streaks (sdSeg-based, fading length based on speed)
- 3D FBM nebula with domain-warp coloring
- 4-color palette: warm violet, hot amber, magenta, white-hot
- Static star field via hash21 threshold
- Edge vignette to focus composition center
**HDR peaks reached:** warp star cores 2.6+, nebula bright regions 1.5, static stars 0.8
**Estimated rating:** 4.0★
