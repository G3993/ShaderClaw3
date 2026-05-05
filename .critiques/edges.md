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
**Approach:** 3D raymarch — NEW ANGLE: "Neon Grid City" — repeating SDF city blocks, street-level camera drift, 5-color neon edge glow, wet reflections. vs. prior v1 (2D particle bounce neon palette).
**Critique:**
1. Reference fidelity: 2D bouncing particles → abandoned entirely for 3D street-level urban scene.
2. Compositional craft: Strong perspective with buildings as solid black silhouettes, neon only on edges — high contrast ratio.
3. Technical execution: O(1) tiling city grid via floor/mod, 64-step march, 5-color HDR neon palette per building via hash, 10-sample volumetric air scatter for neon bleed.
4. Liveness: Camera drifts forward on Z with TIME + sinusoidal sway; audio modulates neon brightness.
5. Differentiation: 3D vs 2D, static architecture vs particles, city vs abstract — maximally different from v1.
**Changes:**
- Full rewrite: 3D raymarched city using sdBox buildings in tiled XZ grid
- 5 neon colors: cyan(0.1,2.5,2.2), magenta(2.5,0.1,1.8), lime(0.3,2.5,0.1), gold(2.5,1.8,0.0), violet(1.5,0.1,2.5)
- Wet street: reflected ray into scene + Fresnel wetness blend
- Star field in sky: hash-jittered sparse star grid
- Ink-black building bodies, neon only at edges via normal curvature detection
- CATEGORIES: ["Generator", "3D"]
**HDR peaks reached:** neon edge glow * neonBrightness 2.5, wet reflections 1.5, volumetric scatter 0.8
**Estimated rating:** 4.5★
