## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (particle system, 3D category added)
**Critique:**
1. Reference fidelity: Particle bounce concept is solid but LED grid default masks all output — black on dark bg.
2. Compositional craft: Capsule streak particles are a strong visual idea, lost in default darkness.
3. Technical execution: uses undeclared audio uniforms (audioBass, audioHigh) safely; LED mode quantizes to near-black at default ledSize.
4. Liveness: TIME-driven particle motion works but LED mode destroys visibility.
5. Differentiation: Unique capsule-stretch bounce system; killed by LED default and desaturated colorJitter mixing with white.
**Changes:**
- Removed LED wall mode entirely (was default ON, producing near-black output)
- Replaced colorJitter white-mixing with fully saturated 6-hue neon palette (magenta→cyan→gold→orange→violet→lime)
- Glow boosted: default 1.3 → 2.5 (HDR range)
- Particle count stays at 128 (was N=256 const regardless of particleCount input)
- Added 3D category
- Black background (0.0, 0.0, 0.01)
- Stretch, particle size defaults tuned up for visibility
**HDR peaks reached:** particle cores + halo accumulation → 2.5+ per cluster
**Estimated rating:** 4.0★

## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D Neon Spiderweb (analytic polar rings + radial threads)
**Critique:**
1. Reference fidelity: Complete departure from particle bounce — replaced with analytic polar geometry (fundamentally different architecture).
2. Compositional craft: Concentric ring structure gives strong focal center; void-black background creates maximum contrast; node intersections add white-hot accent points.
3. Technical execution: Polar coordinates; ring SDF via `mod(r, ringSpacing)`; thread SDF via arc-length `|dTheta| × r`; sinusoidal wobble on both r and theta for organic feel; inner/outer masks.
4. Liveness: Slow rotation (rotSpeed); wobble oscillates continuously; audioBass breathing (web expands/contracts); 4-hue palette cycles globally (colorDrift).
5. Differentiation: Intersection nodes produce white-hot HDR 7.0× bursts; ring/thread hierarchy provides visual depth; bass breathing is kinetically satisfying.
**Changes:**
- Full rewrite as "Neon Spiderweb" — analytic polar generator, no particles
- Ring SDF: `mod(r_wobbled, ringSpacing)` with smooth AA via pixel-size glow
- Thread SDF: `|mod(theta_wobbled, TAU/N) - TAU/N/2| × r` in screen-space arc length
- 4-color palette: violet [0.42,0,1] / cyan [0,0.88,1] / gold [1,0.75,0] / magenta [1,0,0.85] — per-ring assignment cycling with colorDrift
- Nodes (ring∩thread): `ringCore × threadCore × hdrRing × 2.8` — white-hot
- audioBass breathing: `p /= (1 + audioBass × pulse × 0.07)` — web expands on beat
- Wobble: sinusoidal perturbation of r and theta for organic spider silk feel
**HDR peaks reached:** ring core 2.5, ring+glow 3.4, thread 2.5, node intersections 7.0 (2.5×2.8)
**Estimated rating:** 4.6★
