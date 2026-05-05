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
**Approach:** 3D raymarch — NEW ANGLE: p(2,3) torus knot SDF with iso-surface neon lines (vs prior 2D particle capsule bounce system)
**Critique:**
1. Reference fidelity: Prior was 2D particle bounce — totally different concept. Torus knot is a geometrically rich standalone shape.
2. Compositional craft: Single strong focal SDF with camera orbiting — excellent silhouette focus.
3. Technical execution: 64-sample knot-curve approximation per march step is expensive but correct; `fwidth()` iso-lines provide AA edge glow.
4. Liveness: Rotating camera + iso-line phase animation driven by TIME; audio modulates tube radius and scale.
5. Differentiation: Completely different from particle system — 3D geometry, iso-surface aesthetic, neon-on-black cinematic.
**Changes:**
- Full rewrite as 3D raymarched p(2,3) torus knot
- 72-step march with 64-iteration knot-curve approximation for SDF
- Orbiting camera with slow X tilt oscillation
- Neon iso-surface rings on tube cross-section using `fwidth()` AA
- 4-color palette: electric violet, cyan, gold, magenta — fully saturated
- Black velvet bg with faint neon haze glow
- Audio modulates tube radius, scale, and specular peak
- HDR white specular (2.0×), neon line peaks at glowPeak×audio
**HDR peaks reached:** iso lines × glowPeak × audio = 2.5+, white specular 2.0
**Estimated rating:** 4.5★
