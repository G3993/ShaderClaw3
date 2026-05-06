# color_picker critiques

## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Neon Crystal Cave (prior orphaned commits planned Spectral Prism; this is a cave interior with gem-crystal formations — different composition: enclosed ambient vs open dispersion beams)
**Critique:**
1. Reference fidelity: Crystal cave with gem-light is a strong cinematic concept; completely standalone generator (no inputImage).
2. Compositional craft: Orbiting camera inside cave creates enclosed environment; ring of crystal spires + ceiling stalactites gives 360° depth.
3. Technical execution: 72-step march; sdCrystal (elongated octahedron); point-light accumulation loop on cave walls; fresnel ink edge.
4. Liveness: TIME-driven camera orbit + crystal scale audio-modulated; ring rotates slowly.
5. Differentiation: Cave interior (enclosed, ambient) vs Spectral Prism (open refraction beams); cool gem palette vs warm prism dispersion; different compositional axis (inside vs outside).
**Changes:**
- Full rewrite from inputImage color tinting to standalone 3D raymarched crystal cave
- sdCrystal (elongated octahedra) in ring + ceiling stalactite arrangement
- 4 user-controlled crystal colors, all fully saturated, driven to HDR by glowScale
- Point-light accumulation on cave walls from each crystal center
- Volumetric glow halo along eye ray (exp falloff)
- Fresnel ink edge on crystal silhouettes
- Audio modulates crystal scale and glow peak
- "3D" added to CATEGORIES
**HDR peaks reached:** crystal cores 2.5× (glowScale default), wall spill ~1.0, vol halos ~0.5 additive
**Estimated rating:** 4.0★
