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
- Added 3D category; Black background (0.0, 0.0, 0.01)
**HDR peaks reached:** particle cores + halo accumulation → 2.5+ per cluster
**Estimated rating:** 4.0★

## 2026-05-05 (v3)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Bioluminescent Reef; coral SDF colony vs prior neon particle bounce (v1/v2)
**Critique:**
1. Reference fidelity: Complete 3D replacement — bouncing-particles reference sacrificed for standalone visual quality.
2. Compositional craft: 4 coral trees at varied positions on dark ocean floor; orbiting camera provides continuous parallax; strong vertical + depth layering.
3. Technical execution: SDF capsule+sphere smooth-union coral trees, 72-step march, fwidth() AA edge darkening, screen-space polyp glow halos.
4. Liveness: TIME-driven coral polyp pulse, orbiting camera, audio modulates brightness; never static.
5. Differentiation: Completely new 3D vocabulary (coral vs particles), new palette (teal/cyan/violet/magenta), new environment (abyssal ocean vs canvas).
**Changes:**
- Full 3D rewrite as "Bioluminescent Reef" — 4 SDF coral trees + ocean floor ground plane
- sdCapsule+sdSphere smooth-union coral (trunk + 2 branches + pulsing polyp tips)
- 4-color HDR palette: teal/cyan/violet/magenta — fully saturated
- fwidth() AA edge darkening gives black-ink silhouette on all surfaces
- Screen-space additive halo glow around polyp tip positions
- Orbiting camera, TIME-driven polyp pulse, audio modulates glow
**HDR peaks reached:** coral surface * glowPeak * audio = 2.5–3.5 linear; screen-space halo ~2.0
**Estimated rating:** 4.0★
