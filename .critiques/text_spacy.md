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

## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D Aurora Tunnel (sinusoidal aurora curtains + perspective text rows)
**Critique:**
1. Reference fidelity: Perspective tunnel row mechanic preserved — size-by-distance creates depth illusion as rows scroll.
2. Compositional craft: Aurora curtains (sinusoidal bands with void-black gaps) provide strong contrast; text inherits aurora hue at its position for immersive color integration.
3. Technical execution: Three additive sinusoidal band layers at different frequencies; x-position warped per-y for curtain undulation; perspective scaling unchanged from original.
4. Liveness: Aurora curtains undulate and drift at 0.12×TIME; rows scroll at speed param; audioBass swells both aurora and text brightness.
5. Differentiation: Void-black gaps between curtains give ink contrast; text rows colored by aurora hue — not white text on aurora, but text-made-of-aurora.
**Changes:**
- Full background rewrite: `auroraBg()` — 3 sinusoidal band layers, x-position warped by y for curtain flutter
- Aurora palette: green [0,1,0.25] → teal [0,0.88,1] → violet [0.45,0,1] → pink [1,0.08,0.6]
- Text color: `auroraHue(rowPosition)` × hdrText (3.0×) — aurora-colored text matching local curtain
- `curtainFreq` param controls aurora curtain density (default 7.0)
- Audio: audioBass pulses aurora and text brightness
- Removed: transparentBg, textColor, bgColor, preset modes (replaced by procedural palette)
- Added: hdrText, hdrAurora, curtainFreq, pulse inputs
- Preserved: perspective zoom-by-distance, row scroll, voiceGlitch chromatic aberration
**HDR peaks reached:** aurora curtain peaks 2.2×, text 3.0×, text+audioBass 4.1×+
**Estimated rating:** 4.5★
