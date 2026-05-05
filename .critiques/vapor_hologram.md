## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** 2D scene (2-pass) — NEW ANGLE: cyberpunk rainy alley (vs. vaporwave sun+grid scene fix v2, Torii Gate at Dusk v3 — completely different visual domain)
**Critique:**
1. Reference fidelity: Cyberpunk rain alley is a classic noir-sci-fi visual (Blade Runner etc.) — dark, saturated neon, wet reflections — very different from Japanese aesthetics.
2. Compositional craft: Alley walls frame the vertical space; neon signs provide focal color patches; rain creates texture; reflections in wet ground complete the composition.
3. Technical execution: 96-particle rain streaks; sdRoundBox neon signs with pulse flicker; wet-ground shimmer distortion; hologram glitch pass preserved.
4. Liveness: Rain falls, signs flicker, puddle shimmer oscillates — multiple TIME-driven layers.
5. Differentiation: No sun, no grid floor, no torii gate — urban sci-fi aesthetic.
**Changes:**
- Pass 0 scene: dark alley with rain, neon signs, wet ground
- 96 rain streaks: magenta, cyan, amber (HDR 2.5)
- Neon sign outlines on alley walls (sdRoundBox SDF glow)
- Wet ground reflections with shimmer distortion
- Hologram glitch pass preserved unchanged
- skyTopColor/skyHorizonColor defaults changed to near-black
**HDR peaks reached:** rain streaks 2.5, neon signs 2.0, reflections 1.8
**Estimated rating:** 4.5★
