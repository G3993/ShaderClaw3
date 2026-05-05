## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D refine — NEW ANGLE: volcanic lava field background (vs starfield in v1)
**Critique:**
1. Reference fidelity: Perspective tunnel rows are solid; volcanic ground plane gives physical context.
2. Compositional craft: Depth-based brightness (far rows dimmer) creates parallax on lava floor.
3. Technical execution: lavaFloorBg() uses 3-octave FBM noise mapped to crimson/orange/gold palette.
4. Liveness: Time-driven FBM flow + row scroll = two independent motion layers.
5. Differentiation: Lava floor vs v1 starfield; fiery orange text vs v1 white/cyan text.
**Changes:**
- transparentBg default: true → false
- textColor: white → fiery orange [1.0, 0.5, 0.0]; bgColor: → volcanic dark [0.03, 0.0, 0.0]
- Added hdrGlow (2.0); added lavaFloorBg() with FBM heat map
- Lava palette: black → crimson → orange → gold (4 tones)
- depthBright factor: far rows 0.6×, near rows 1.0× for depth cue
**HDR peaks reached:** text * 2.0 * 1.0 (near) = 2.0; lava floor ambient 0.35×
**Estimated rating:** 4.0★

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
