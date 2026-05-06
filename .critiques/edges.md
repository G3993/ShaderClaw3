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
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: Prior v2 (LED particle fix) was 2D capsule particles; new angle is fully standalone 3D SDF with no input image dependency.
2. Compositional craft: Concentric rings around a central sphere create strong circular symmetry; orbiting camera adds cinematic depth and parallax.
3. Technical execution: 8 independent tori each with rotX(ph+t)*rotY(ph*1.618+t*r) golden-ratio phase separation. 64-step march. fwidth() AA on longitude-line surface decoration.
4. Liveness: Rings spin at different rates per index; camera orbits in two axes; audio expands ring thickness.
5. Differentiation: Armillary sphere with gyroscope rings is a completely new metaphor — no prior version used concentric orbital ring geometry.
**Changes:**
- Full rewrite as "Armillary Sphere" — 3D concentric gyroscope rings SDF
- N tori (2–8 count) each tilted at golden-ratio-spaced angles, independently rotating
- Gold rings vs deep navy inner sphere (material split by length(p))
- HDR electric cyan rim glow: 3.2× at grazing angle
- Longitude-line surface decoration with fwidth() AA
- Deep navy starfield background
- 64-step raymarcher, orbiting camera (two-axis)
- audioMod modulates ring thickness
**HDR peaks reached:** cyan rim 3.2, gold specular 3.0, ring diffuse 2.4
**Estimated rating:** 4.5★
