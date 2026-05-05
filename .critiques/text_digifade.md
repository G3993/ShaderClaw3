## 2026-05-05 (v7)
**Prior rating:** 0.0★
**Approach:** 3D particle/painterly — NEW ANGLE: ember scatter particle system (warm/painterly) vs prior 2D CRT terminal phosphor (cool/technical)
**Critique:**
1. Reference fidelity: v1 CRT approach was technically cool but the "digifade" concept could also be a physical dissolution.
2. Compositional craft: 2D scanlines → 3D depth illusion: embers grow in apparent size as they drift "forward."
3. Technical execution: Dense grid sampling within character glyph bbox; per-ember depth-drift via scale factor.
4. Liveness: dissolve/reform cycle; embers emerge from TEXT pixels only (glyph-masked spawn).
5. Differentiation: Warm amber/gold palette (opposite to prior CRT green phosphor); physical scatter vs electronic glitch.
**Changes:**
- Full rewrite: "Ember Scatter" — text pixels spawn HDR embers with depth-illusion drift
- Glyph-masked particle spawn: only pixels with glyph coverage > 0.3 emit embers
- 3D depth illusion: embers scale up as they drift (z-drift factor)
- Ember palette: white-hot core → amber → dark edge (coreColor, emberColor)
- dissolve/reform phase cycle (every ~4s): solid → scatter → solid
- Black void background with warm coal glow center during scatter phase
- Audio modulates particle size + brightness
**HDR peaks reached:** white-hot ember core hdrPeak * audio ≈ 2.8, ember body 1.5
**Estimated rating:** 4.0★

## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (CRT background + HDR phosphor glow)
**Critique:**
1. Reference fidelity: Glitch dissolve effect is creatively distinct; invisible in transparent mode.
2. Compositional craft: Sweep/dissolve sweep creates movement, but no background canvas.
3. Technical execution: Slice-based glitch displacement works correctly.
4. Liveness: Sweep wave and glitch noise are TIME-driven.
5. Differentiation: Digifade sweep is unique; needs a visible surface.
**Changes:**
- Added crtBg() — CRT terminal background: scanlines + slow data bar noise + vignette
- Terminal color palette: phosphor green [0,1,0.5] text on void black bg
- transparentBg default: true→false
- textColor default: white → phosphor green [0, 1.0, 0.5]
- bgColor default: black → void green-black [0, 0.02, 0]
- hdrGlow default: 2.5 — phosphor text glows brightly
- scanlineInt parameter controls CRT scanline depth
- audioMod input added
- Soft phosphor bleed halo around text row
**HDR peaks reached:** textColor * 2.5 = 2.5 direct; glow halo adds ~0.3 soft bleed
**Estimated rating:** 3.8★
