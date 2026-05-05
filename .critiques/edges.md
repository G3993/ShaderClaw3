## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: full 3D replacement of 2D capsule-particle system → 6 neon spheres bouncing in 3D void with motion-blur capsule trails
**Critique:**
1. Reference fidelity: Edge-bounce concept now in 3D with deterministic triangle-wave physics; much more spatial than flat 2D.
2. Compositional craft: 6 saturated balls in black void → strong HDR impact; glow halos create depth layering.
3. Technical execution: 96-step march, bounce01() deterministic paths, Blinn-Phong + rim, fwidth() capsule edge AA.
4. Liveness: All motion TIME-driven; audio modulates ball scale via audioPulse.
5. Differentiation: 3D billiard aesthetic (spatial depth, multiple light colors) vs prior flat 2D streaks.
**Changes:**
- Complete 3D rewrite — SDF capsules for motion-blurred spheres in 3D box
- Palette: magenta(2.5,0,2.0), cyan(0,2.5,2.5), gold(2.5,2.0,0), orange(2.5,0.4,0), lime(0.4,2.5,0), violet(1.2,0,2.5)
- Deterministic bounce using bounce01(t*speed+phase)*2-1 per axis
- Capsule SDF between curr and prev positions for trail blur
- Glow halo: glowAmt * baseCol * exp(-minD * 12.0)
- fwidth() AA on capsule iso-edge
- Audio multiplicative scale only
- No LED mode (removed permanently)
**HDR peaks reached:** specular 3.0, sphere surfaces 2.5, glow accumulation 2.0+
**Estimated rating:** 4.5★

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
