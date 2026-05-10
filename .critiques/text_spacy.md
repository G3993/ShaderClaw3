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

## 2026-05-10
**Prior rating:** 0.0★
**Approach:** 2D refine — NEW ANGLE: Neon City Night background (prior 2026-05-05 was procedural starfield/nebula)
**Critique:**
1. Composition: neon-lit building silhouettes with glowing windows and signs vs. prior sinusoidal nebula wash + star twinkling. Urban street-level vs. outer-space.
2. Palette: cyan/magenta/gold windows ×2.0 HDR; neon signs ×2.5 HDR + soft aura; near-black sky vs. prior deep-space navy.
3. Motion: neon sign flicker at epoch 0.15 rate (≤0.2 §4 ✓); depth-perspective scroll unchanged; speed default 0.5.
4. Silhouette: building dark masses against lit sky create strong vertical rhythm — architectural blocking vs. prior diffuse nebula.
5. HDR: windows vec3(2.0) cycling palette; sign aura soft +0.3 halo bleed; text hdrGlow 2.0× depth.
**Changes:**
- Added `neonCityBg()` — 10 building silhouettes with cyan/magenta/gold windows (97% hash-gated), 5 neon signs ×2.5 HDR + aura
- `transparentBg` default: true → false
- `textColor` default: white → cyan [0.0, 1.0, 0.9]
- Added `hdrGlow` input (default 2.0) with depth-based brightness gradient
- Horizon glow band at void sky base
**Motion audit:** sign flicker epoch 0.15 (§4 ✓); no audio directly on epoch ✓; speed 0.5 default ✓
**HDR peaks reached:** neon sign ×2.5; window cycling ×2.0; text hdrGlow 2.0× close → dim far
**Estimated rating:** 4.0★
