## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (aurora background + HDR glow)
**Critique:**
1. Reference fidelity: Cascading tiled rows with wave offsets is a legitimate effect; invisible in transparent mode.
2. Compositional craft: No background — transparent default means nothing visible standalone.
3. Technical execution: Wave offset logic is correct; multi-row cascade works.
4. Liveness: TIME-driven wave oscillation is present but hidden behind transparency.
5. Differentiation: Cascade row effect is distinctive; needs a background to show it.
**Changes:**
- Added auroraBg() — 5-layer sinusoidal aurora with 4-color saturated palette
- Aurora colors: violet, cyan, gold, magenta — all fully saturated
- transparentBg default: true→false
- textColor default: white → gold [1.0, 0.85, 0.0]
- bgColor default: black → deep purple [0.02, 0.0, 0.10]
- hdrGlow default: 2.2 (gold text glows HDR)
- Alternating row colors: gold vs magenta (row parity)
- audioMod input added
**HDR peaks reached:** gold text * 2.2 = 2.2 direct; with audio 3.0+
**Estimated rating:** 3.8★

## 2026-05-05 (v9)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: pyroclastic volcanic eruption (vs v8 cold crystal prisms 3D)
**Critique:**
1. Reference fidelity: Pyroclastic eruption column — rising ember fragments, lava vent mound, ash cloud — directly references volcanic geological phenomenon. Thermally opposite to cold crystal prisms (v8).
2. Compositional craft: Central vertical column from base vent to top; spiral ember trajectories create dynamic rising motion; dark ground with ash suggests environment.
3. Technical execution: Chain of 5 sphere SDFs for undulating column; cone SDF for vent mound; 60-particle 2D additive ember loop with age-based color fade; age-dependent size growth.
4. Liveness: Embers continuously rise with spiral paths; column sways with sin(TIME); audioBass drives speed and brightness.
5. Differentiation: Hot orange/white-hot volcanic vs cold cyan/blue crystal (v8). 4-color palette: void/lava-orange/ash-grey/white-hot HDR — all saturated. Industrial geological vs organic crystal.
**Changes:**
- Full rewrite as 3D pyroclastic eruption
- Chain of undulating sphere SDFs for central column
- Cone/sphere for lava vent base
- Age-graded embers: white-hot → lava orange → ash grey
- Spiral trajectory for rising particles
- Ash ground plane with radial heat glow
- Audio drives ember speed and column brightness
**HDR peaks reached:** white-hot mound 1.5×hdrPeak, embers white phase 1.5×hdrPeak, column 2.5×hdrPeak
**Estimated rating:** 4.2★
