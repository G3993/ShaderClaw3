## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: first-time improvement; effect pass needing inputImage → standalone cinematic 3D gem generator
**Critique:**
1. Reference fidelity: Original was a color-tint effect pass with zero standalone output — replaced with an iconic gemstone SDF.
2. Compositional craft: Rotating truncated octahedron with drifting camera creates strong centered focal element against black void.
3. Technical execution: 80-step raymarch, fwidth() AA on every edge, 3 studio lights (key/fill/rim), spectral facet hue from normal.
4. Liveness: TIME-driven spin + camera drift; audio modulates gem size.
5. Differentiation: Spectral facet coloring (hue = fract(dot(normal, vec3))), black ink edge contrast against white-hot HDR specular.
**Changes:**
- Full rewrite from effect pass to standalone generator
- Truncated octahedron SDF (sdOctahedron intersected with sdBox) as gem
- 3 studio lights (key 2.5× HDR, fill 0.4× HDR, rim)
- Spectral hue per facet based on normal orientation
- Black ink silhouette via fwidth AA on dSurf
- Orbiting drifting camera (sin/cos time offsets)
- 4-color palette: deep violet, electric cyan, gold, white-hot
**HDR peaks reached:** white-hot specular 2.5+, facet highlights 1.5–2.0, star field 0.5
**Estimated rating:** 4.0★
