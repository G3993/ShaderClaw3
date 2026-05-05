## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D refine — NEW ANGLE: Coral Depths bioluminescent underwater (aquatic vs prior outer-space starfield); warm bio-lights vs cold stars; different color temperature
**Critique:**
1. Reference fidelity: "Coral Depths" — bioluminescent cavern perspective. The zoom-by-distance rows become depth layers in the ocean.
2. Compositional craft: Depth-mapped color (near=cyan, far=magenta) matches scale gradient; plankton sparkles add micro-detail.
3. Technical execution: All font boilerplate + perspective math preserved; audioMod modulates glow pulse; _voiceGlitch intact.
4. Liveness: Bio-pulse per row: sin(TIME*speed*2+ri*0.9); plankton sparkle randomized per TIME floor.
5. Differentiation: Underwater palette (aquatic cyan/magenta/teal) vs prior space (white/cyan/violet nebula).
**Changes:**
- Depth-based color: near rows bio-cyan (0,2.5,2.5), far rows bio-magenta (2.5,0,1.5)
- Per-row bioluminescent pulse: 0.8 + 0.2*sin(TIME*speed*2 + ri*0.9)
- Plankton sparkles: hash per grid cell, sparkSeed>0.97, vec3(0.5,2.5,1.5) HDR
- Water glow fog: (0,0.04,0.06) per depth layer
- audioMod: rowColor *= 1.0 + audioLevel*audioMod*0.35
- Background: pure black ocean
- Removed transparentBg, bgColor, textColor
**HDR peaks reached:** bio-cyan near rows 2.5, sparkle 2.5, magenta far rows 2.5
**Estimated rating:** 3.8★

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
