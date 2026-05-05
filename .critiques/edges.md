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
**Approach:** 3D raymarch — NEW ANGLE: SDF tube lattice vs prior 2D capsule particle system
**Critique:**
1. Reference fidelity: "Edges" of a geometric lattice — the concept of edges is now literally 3D tube edges of a cubic grid vs prior 2D bounce edges.
2. Compositional craft: Orbiting camera reveals the infinite 3D lattice from multiple angles. Depth and perspective create strong composition vs prior flat 2D view.
3. Technical execution: Modular tiling SDF (3 axis tubes + node sphere). 64-step march. Per-cell color via hash13(). fwidth() AA edge. Pulsing nodes via hash + sin(t).
4. Liveness: Camera orbits at camSpeed. Node radius pulses per-cell with TIME. Hue slowly drifts with t*0.04. Audio modulates node size.
5. Differentiation: 3D SDF lattice vs 2D particles; tube geometry vs capsule streaks; orbiting camera vs fixed view; cell-hash color vs per-particle random.
**Changes:**
- Full rewrite: 3D raymarched SDF tube lattice
- 4-color neon palette: cyan, magenta, gold, violet
- Modular tiling with 3-axis capsule tubes + node spheres
- Orbiting camera (3.2 units radius, height oscillation)
- Per-node hash drives hue + pulse amplitude
- fwidth() AA black ink silhouette edges
- Audio modulates node radius (audioBass + audioLevel)
**HDR peaks reached:** neonPal * 2.8 + spec 2.5 = ~3.2 at specular peaks on lit nodes
**Estimated rating:** 4.2★
