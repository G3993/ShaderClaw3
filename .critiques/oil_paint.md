## 2026-05-05
**Prior rating:** N/A (first tracked version)
**Approach:** 3D FBM surface — domain-warped fractal Brownian motion "Lava Impasto" with hot red/orange/gold palette, Kuwahara-inspired painterly texture
**Critique:**
1. Reference fidelity: molten surface feel convincing; warm palette authentic
2. Compositional craft: domain warp creates organic flow; relief lighting adds depth
3. Technical execution: multi-pass Kuwahara filter, FBM domain warp, relief normal from luminance gradient
4. Liveness: TIME drives warp evolution; surface bubbles and shifts
5. Differentiation: clear painterly-impasto identity; warm palette distinctive
**Changes:** (initial version)
**HDR peaks reached:** ~1.5 (Kuwahara pass clamps)
**Estimated rating:** 2.5★

## 2026-05-06
**Prior rating:** 2.5★
**Approach:** 3D raymarch — NEW ANGLE: complete rewrite to Jade Torus Ring Sculpture; 5 nested SDFtoruses at independent rotation axes, jade green + glacier teal + ice white palette; cool/cold opposite of prior warm lava theme
**Critique:**
1. Reference fidelity: entirely opposite aesthetic — cool mineralic jade vs hot molten lava; torus geometry vs painted surface
2. Compositional craft: nested rings at different radii and axes create interlocking sculptural depth; slow camera orbit reveals ring intersections
3. Technical execution: sdTorus() with per-ring independent rotation axis (X/Y/Z alternating with phase offsets); finite-diff normals; key/fill/rim three-point lighting; specular ice-white highlight hits HDR peak
4. Liveness: audio pulses major radius as multiplier; each ring spins on its own phase; camera elevation sways gently
5. Differentiation: 3D torus SDF vs 2D Kuwahara image filter; cool jade/teal vs warm red/orange; no external image dependency; standalone generator
**Changes:**
- Removed all Kuwahara filter, multi-pass, inputImage, FBM, and domain-warp code
- Removed PASSES block (now single-pass)
- Added sdTorus() SDF
- Added scene() with loop over N tori, each with independent rotation axis and phase
- Added three-point lighting: key (upper-right-front), fill (lower-left), rim (teal)
- Added ice-white specular highlight at HDR 2.5+ peak
- Jade/glacier palette: vec3(0.1,0.8,0.5) + vec3(0.0,0.7,0.9); no grey/white in diffuse
- Black void background vec3(0.005, 0.01, 0.008)
- Black silhouette edge via smoothstep on dot(n,-rd)
- Audio as modulator: 1.0 + audioLevel * audioReact * 0.35
**HDR peaks reached:** specCol vec3(0.9,1.0,1.0) * hdrPeak * audio up to ~3.5; baseCol * hdrPeak up to 2.5; rimCol * hdrPeak * 0.5 up to 1.25
**Estimated rating:** 4.0★
