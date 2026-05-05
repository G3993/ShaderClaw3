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
**Approach:** 3D raymarch — NEW ANGLE: 2D particle/bounce system → 3D nested rotating torus rings (gyroscopes)
**Critique:**
1. Reference fidelity: "edges" → particles bouncing at screen edges replaced by nested torus rings as a different orbital geometry concept.
2. Compositional craft: 4 concentrically nested rings at different major radii provide clear depth hierarchy; slow camera orbit adds liveness.
3. Technical execution: 64-step raymarch; finite-difference normals; studio key + cool fill; white-hot specular at HDR peak.
4. Liveness: Each ring spins at unique speed + direction; camera slow-orbits; audio boosts brightness.
5. Differentiation: Completely different from capsule-particle system — 3D SDF rings vs 2D screen-space particles; different palette (magenta/cyan/gold/violet vs white-based).
**Changes:**
- Full rewrite from 2D particle bounce to 3D torus ring gyroscopes
- 4 rings, each uniquely tilted (xy+yz rotations) + independent spin direction/speed
- Palette: hot magenta, electric cyan, gold, violet — all fully saturated
- Studio key light upper-left + dim cool fill + white-hot specular
- Slow camera orbit (sin/cos, 0.12 rad/s)
- Audio modulates hdrPeak brightness
**HDR peaks reached:** base color * 2.5 + white specular * 2.25 = ~3.0+ at highlights
**Estimated rating:** 4.0★

## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** 2D Expressionist Brushstorm — NEW ANGLE: 2D bounce particles (v1 patch) → 3D torus rings (v2) → 2D diagonal capsule brush strokes (v4 Kirchner Expressionism)
**Critique:**
1. Reference fidelity: "Edges" particle system replaced with Expressionist paint gesture — Ernst Ludwig Kirchner diagonal brushwork in hot Fauvist palette.
2. Compositional craft: 120 capsule strokes distributed across canvas; yellow/crimson/cobalt tricolor gives maximum chromatic tension; black ink accumulation creates depth.
3. Technical execution: Capsule SDF per stroke; fwidth AA on all edges; per-stroke color from 3-hue palette; black ink bleed via inkAcc accumulation.
4. Liveness: Stroke centroids drift sinusoidally; angle slowly wobbles; audio modulates placement amplitude.
5. Differentiation: 2D gestural brushwork vs v2 3D torus rings; Kirchner palette (yellow/crimson/cobalt) vs prior neon (magenta/cyan/gold); flat composition vs orbital 3D.
**Changes:**
- Full rewrite from 3D torus rings to 2D expressionist brush strokes
- 120 capsule-SDF strokes with hash-distributed parameters
- Palette: cadmium yellow (2.6,1.8,0), crimson (2.5,0.04,0.02), cobalt blue (0.05,0.2,2.6) — fully saturated
- Stroke centroids drift with sin/cos TIME animation
- Black ink accumulation for edge darkening (inkAcc * 0.65)
- fwidth AA on all stroke edges
- Audio modulates agitation and HDR boost
**HDR peaks reached:** stroke surfaces up to 2.6 (yellow), crimson 2.5, cobalt 2.6
**Estimated rating:** 4.2★
