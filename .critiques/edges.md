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

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: 2D particle bounce (prior) → 3D nested torus-ring Fabergé lattice (Painterly lighting)
**Critique:**
1. Reference fidelity: Prior neon particle bounce is 2D; new version is a genuine 3D object with orbital structure and volume.
2. Compositional craft: N nested tori at orthogonal/diagonal angles create a sphere-like Fabergé lattice — strong centered silhouette.
3. Technical execution: 80-step march, each ring is an sdTorus with rotX/rotY/rotZ transforms, fwidth() AA on all edges.
4. Liveness: TIME-driven ring rotation + orbiting camera; audio modulates ring radius.
5. Differentiation: Different primitive vocabulary (tori vs capsule-particle), different lighting (painterly diffuse/rim vs neon accumulation), different palette (gold/magenta/emerald cycle vs fixed neon).
**Changes:**
- Full rewrite from 2D particle system to 3D torus lattice
- Up to 7 tori at orthogonal and diagonal orientations (ringCount param)
- Each ring spins at different speed offset (spinSpeed * various multipliers)
- Hue from 3D position + TIME for animated palette cycling
- Painterly: diffuse + key specular + rim light
- Black ink edge via fwidth
- Orbiting camera (camA = t * 0.11)
- 4-color palette: gold, magenta, emerald, white-hot (cycling via HSV hue)
**HDR peaks reached:** tube core + rim contribution 2.4+, white spec 2.4
**Estimated rating:** 4.2★
