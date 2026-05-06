## 2026-05-06 (v3)
**Prior rating:** 0.0★
**Approach:** 2D refine — NEW ANGLE: lava cave background (dark basalt + glowing crimson-orange lava cracks) vs. prior neon brick wall; volcanic/geological vs. neon-urban; warm fractured rock vs. cool neon grid
**Critique:**
1. Reference fidelity: Text-bricks concept reinterpreted as text carved into a volcanic cave wall, glowing gold like cooling lava letters.
2. Compositional craft: High contrast: near-black basalt background vs. glowing gold text; lava cracks add depth and texture without competing with text.
3. Technical execution: FBM basalt texture + domain-warped crack network + fwidth() AA on crack edges.
4. Liveness: TIME-driven slow lava flow in cracks + audio modulates crack intensity.
5. Differentiation: Geological volcanic reference vs. neon-urban grid; warm crimson-orange-gold vs. cool cyan-violet neon; fractal cracks vs. regular brick grid.
**Changes:**
- Background replaced: neonBrickBg → lavaCaveBg (dark basalt + glowing lava cracks)
- textColor default: cyan [0,1,1] → electric gold [1.0, 0.9, 0.0]
- bgColor default: deep violet → deep basalt [0.02, 0.01, 0.005]
- Added crackIntensity parameter (default 1.5)
- Domain-warped FBM crack network (3-color: crimson→orange→gold)
- Animated lava flow within cracks (TIME-driven upward drift)
- fwidth() AA on crack edges
- Audio modulates crackIntensity
**HDR peaks reached:** lava crack centers 2.5+, gold text * hdrGlow 2.2, text glow halo ~2.0
**Estimated rating:** 4.0★
