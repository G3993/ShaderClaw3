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

## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 2D refine (lava lamp background + HDR amber glow)
**Critique:**
1. Reference fidelity: Perspective depth rows fully retained; lava lamp fluid background reframes the depth illusion as warm organic layers rising through the screen.
2. Compositional craft: 6 rising amber/orange blobs in deep crimson fluid fill the frame organically; white-hot text floats visibly above the warm field.
3. Technical execution: Metaball-style blobs via Gaussian heat sum; height looping via fract(phase*0.3) for continuous rise; fwidth() AA on blob boundary contour ring; heatPeak drives amber→orange→gold→white-hot ramp.
4. Liveness: Each blob advances at different rate; audio modulates blob radius; fluid base shifts color with depth.
5. Differentiation: Lava lamp oil diffusion is completely new — warm organic blobs vs prior cold starfield, crystal lattice, bioluminescent ocean.
**Changes:**
- Added lavaLampBg() — 6 rising metaball blobs (Gaussian heat sum) in deep crimson base
- Blobs loop continuously via fract(phase * 0.3) per blob index
- Heat ramp: crimson → amber (2.0×) → orange-gold (3.0×) → white-hot (3.5×)
- fwidth() AA on blob boundary contour ring (dark edge at surface boundary)
- textColor default: white → warm white-hot [1.5, 1.2, 0.8] (pre-boosted)
- bgColor default: black → deep crimson [0.18, 0.01, 0.0]
- transparentBg default: true→false (confirmed)
- hdrGlow default: 2.5
- audioMod modulates blob radius
**HDR peaks reached:** blob white-hot core 3.5, orange 3.0, amber 2.0; text 2.5
**Estimated rating:** 4.2★
