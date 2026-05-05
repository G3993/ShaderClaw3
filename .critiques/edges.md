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

## 2026-05-05 (v3)
**Prior rating:** 0★
**Approach:** 3D raymarch — NEW ANGLE: (2,3) torus knot neon sculpture vs v1 2D capsule bounce particles, v2 2D lava fluid
**Critique:**
1. Reference: Torus knot — mathematical focal sculpture, strong centered silhouette
2. Composition: Single knotted sculpture on void — total opposite of v2 environmental fill
3. Technical: 32-capsule torus knot SDF, 64-step march, fwidth ink silhouettes
4. Liveness: Orbiting camera + hue rotation TIME-driven; audio modulates tube radius
5. Differentiation: 3D geometric sculpture completely different from v1/v2 fluid/particle 2D
**Changes:**
- Full rewrite from 2D particles to raymarched (2,3) torus knot
- 4-color palette (cyan/magenta/gold/violet), position-based hue, zero white
- Ink-black silhouette at grazing angles via fwidth
- Audio modulates tube radius as modulator
**HDR peaks reached:** 2.5 (palette × glowPeak) + specular 0.4×2.5 = ~3.5 peaks
**Estimated rating:** 4.0★
