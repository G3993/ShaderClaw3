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
**Approach:** 2D Voronoi — NEW ANGLE: Voronoi cell edge distance field glow. v1 was particle bounce system, v13 was 3D neon city. This is 2D with entirely different geometry (Voronoi distance).
**Critique:**
1. Reference fidelity: Animated Voronoi cells create organic territorial boundaries; neon edge halos simulate plasma lightning discharge.
2. Compositional craft: Black void interiors ensure maximum contrast; variable cell shapes create strong silhouettes.
3. Technical execution: fwidth() on edge distance gives sub-pixel AA; per-cell hue from hash gives fully saturated neon variety.
4. Liveness: Animated cell centers via sin(time*seed) creates shifting, morphing territorial boundaries.
5. Differentiation: Completely different from particle bounce (v1) and 3D geometry (v2-v13); Voronoi edge field is a unique visual language.
**Changes:**
- Replaced particle system with animated 2D Voronoi cell edge glow
- 5×5 neighbor search for nearest-2 cell centers
- Animated centers: seed + 0.45*sin(t * rand * 2π)
- Per-cell saturated HSV neon colors via hash
- Black void (0.005,0,0.01) with neon edge accumulation
- White-hot specular core at exact edge position
- Audio modulates glow amplitude
**HDR peaks reached:** edgeGlow * hdrBoost = 2.5 * 2.0 = 5.0 at core; glow falloff reaches 2.5 near edge
**Estimated rating:** 4.3★
