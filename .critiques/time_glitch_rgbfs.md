## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** 3D SDF raymarch — NEW ANGLE: bismuth crystal cave (vs. 3D RGB data planes, VHS Horror 3D, Signal Weave 2D — no glitch aesthetic at all)
**Critique:**
1. Reference fidelity: Bismuth crystals are a real mineral with dramatic stair-step geometry and rainbow iridescence — completely orthogonal to any glitch/VHS/signal aesthetic.
2. Compositional craft: Cave setting creates strong dark silhouette framing; overhead point light creates dramatic shadows between crystal steps.
3. Technical execution: 64-step SDF march; stair-stepped box SDFs per crystal; normal-based iridescent hue mapping; HDR specular peaks.
4. Liveness: Orbital camera TIME-driven; audio modulates crystal scale and orbit.
5. Differentiation: No glitch, no screens, no data — pure geological mineral sculpture.
**Changes:**
- Complete rewrite as bismuth crystal cave 3D raymarcher
- SDF: stair-stepped sdBox crystals (3-5 steps per crystal) + inverse cave sphere
- Iridescent coloring: hsv2rgb with hue from normal direction
- Overhead point light + HDR specular (3.0 peak)
- Orbiting camera (TIME*rotSpeed)
- Audio modulates crystal scale
**HDR peaks reached:** specular 3.0, iridescent faces * hdrPeak (2.5) = 2.5
**Estimated rating:** 4.5★
