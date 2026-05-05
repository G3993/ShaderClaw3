## 2026-05-05 (v8)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: standalone crystal prism generator vs prior image-dependent color tint filter
**Critique:**
1. Reference fidelity: Original was a pure image filter (color tint) — zero standalone output, scores 0 on every axis.
2. Compositional craft: No composition possible without inputImage; complete generator replacement needed.
3. Technical execution: Original had no generative math; new version requires full SDF + refraction system.
4. Liveness: Static tint with no TIME dependency; rewrite adds orbiting camera + gyroid faceting oscillation.
5. Differentiation: Color picker concept salvaged as chromatic dispersion — light splitting through crystal is the new visual metaphor.
**Changes:**
- Complete rewrite: "Prismatic Refractions" — gyroid-modulated sphere SDF, 64-step raymarch
- 5-sample chromatic dispersion loop (R/G/B/cyan/violet channels at different IOR)
- Studio lighting: warm key + cool fill
- Background light shaft fan keyed to `facets` parameter
- HDR white specular via Fresnel: peak `hdrPeak * audio` ≈ 2.5–4.0
- Spectral palette: ruby, gold, cyan, violet — fully saturated
- Edge ink via fresnel darkening at glancing silhouette
- fwidth() AA on crystal iso surface
- Audio modulates brightness + IOR oscillation
- Orbiting camera with slow pitch bob
**HDR peaks reached:** white specular 2.5+, spectral beams 1.6, shaft fan 1.25
**Estimated rating:** 4.0★
