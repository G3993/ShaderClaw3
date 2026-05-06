## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 2D refine — NEW ANGLE: lava tunnel (underground warm) vs prior starfield (space cool)
**Critique:**
1. Reference fidelity: Underground geology reference — lava crack network has strong directional flow
2. Compositional craft: Crack lattice creates depth and texture; tunnel vignette focuses center
3. Technical execution: Three sin layers at different scales create non-repeating crack network
4. Liveness: t-driven crack animation gives slow cooling/heating pulse; audio reacts via audioMid
5. Differentiation: Infernal underground lava vs cosmic starfield — opposite spatial direction (down vs up)
**Changes:**
- lavaTunnelBg(): 3-layer sinusoidal crack network, crimson→orange→gold HDR lava colors
- Tunnel vignette (1 - d²) for depth focus
- transparentBg default: true→false
- textColor default: white→magma gold [1.0, 0.8, 0.0]
- Text 2.0× HDR + subtle lava reflection
**HDR peaks reached:** lava crack core 3.0, text 2.0, glow halo 1.5
**Estimated rating:** 3.8★
