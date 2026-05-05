## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (HDR palette replacement)
**Critique:**
1. Reference fidelity: Flow field algorithm (cellular FBM backward trace) is well-executed and matches "wind-blown grass tips" reference.
2. Compositional craft: Grass gradient is desaturated (black→forest green→gray→white) — indistinct at small sizes.
3. Technical execution: Multi-pass ISF correctly implemented; Bezier weight curve is sophisticated.
4. Liveness: TIME-driven via flow offset, but temporal feels slow.
5. Differentiation: Interesting LIC-style approach; killed by the gray/white palette giving near-zero saturation score.
**Changes:**
- Replaced grass gradient with volcanic magma palette: black→deep crimson→orange→gold→white-hot HDR
- Seed dot colors changed from random→3 fire hues (deep ember, orange, gold)
- intensity default: 1.0→2.5 (HDR boost)
- dotDensity default: 0.1→0.12
- audioMod input added, modulates flow speed and direction field
- HDR peak: magma top ramp → 3.0× white-hot on high-intensity seeds
**HDR peaks reached:** white-hot seeds 3.0, gold 2.0, orange 1.3
**Estimated rating:** 3.5★

## 2026-05-05 (v12)
**Prior rating:** 0.0★
**Approach:** 2D refine (Starling Murmuration — analytic flock particles)
**Critique:**
1. Reference fidelity: Multi-pass LIC backward trace with desaturated grass gradient is visually interesting but scores near-zero on palette saturation.
2. Compositional craft: Monochrome grass palette with near-white endpoint gives indistinct output at display brightness.
3. Technical execution: 3-pass ISF (directions/positions/image) is correctly implemented; Bezier weight curve is sophisticated.
4. Liveness: TIME-driven flow scroll works but feels slow and lacks dynamic events.
5. Differentiation: Unique LIC approach; ruined by the gray/white palette.
**Changes:**
- Full rewrite as "Starling Murmuration" — analytic 2D flock silhouette
- 400 birds as oriented capsule streaks (smoothstep SDF), directionally aligned to velocity
- Flock center drifts via sin/cos oscillation; cohesion oscillates for compression/expansion
- Each bird: double-bounce-oscillator position per bird seed, offset from flock center by cohesion radius
- Dusk sky background: amber horizon HDR → purple mid-sky → deep violet zenith
- Sun glow: Gaussian disc + wide atmospheric halo at horizon
- Black bird silhouettes mixed over sky via min(accumulated darkness, 1.0)
- audioBass pulse breathes cohesion → flock compresses on beat
- Removed 3-pass ISF structure; single-pass generator
**HDR peaks reached:** sun disc vec3(1,0.9,0.55)×3.0×0.7 = 2.1 peak; skyAmber×3.0×0.9 = 2.7 peak; birds are 0.0 (black silhouettes)
**Estimated rating:** 4.2★
