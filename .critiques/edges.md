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

## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 3D rewrite (neon ring gyroscope — 3 interlocked counter-rotating SDF tori, volumetric glow)
**Critique:**
1. Reference fidelity: Complete 3D rewrite; gyroscope metaphor is strong and distinct from particle bounce.
2. Compositional craft: 3 interlocked rings on orthogonal axes (XZ/XY/YZ) with independent rotation speeds create gyroscope precession. Black ink backdrop, cyan/magenta/gold palette.
3. Technical execution: SDF raymarcher (64 steps). Volumetric glow accumulates exp(-surface_dist × gStr) per step. Fresnel shading at surface. Numerical normal via 6 SDF evaluations.
4. Liveness: Global tumble + independent ring spins (0.8×/1.1×/0.65× gyroSpeed). audioReact pulses brightness + tube size via audioBoost.
5. Differentiation: 3D SDF approach is completely different from v1's 2D particle capsule streaks; gyroscope identity is visually unique.
**Changes:**
- Complete rewrite: 3 SDF tori on XZ/XY/YZ planes with individual rotY/rotZ/rotX spins
- Global YX tumble for gyroscope precession
- Volumetric neon glow via exp(-d_surface × 10/glowRadius) × 0.055 per step × 3 rings
- Fresnel-edge surface shading: bright rim, dark face-on
- HDR: CYAN/MAGENTA/GOLD all at hdrPeak (default 2.5)
- audioReact: modulates audioBoost = 1.0 + audioLevel×0.8 + audioBass×0.4
- Inputs: gyroSpeed, tubeRadius, ringScale, glowRadius, hdrPeak, audioReact, bg
**HDR peaks reached:** ring surfaces at hdrPeak×audioBoost (2.5–4.5); glow halos 0.5–1.5 depending on glowRadius
**Estimated rating:** 4.5★
