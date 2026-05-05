## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D domain warp — NEW ANGLE: pure 2D FBM turbulence; prior v1 planned 2D particles (not implemented), v2-v14 all went 3D (prism/crystal/geometry). This returns to 2D but as abstract expressionist noise art.
**Critique:**
1. Reference fidelity: Standalone generator — no inputImage needed. Domain-warped FBM creates organic cloud/ink forms.
2. Compositional craft: 3-layer nested warp produces complex, non-repeating turbulent structure filling the frame.
3. Technical execution: fwidth()-based iso-contour ink edges give clean AA black outlines; cosine palette ensures full saturation.
4. Liveness: TIME-driven warp layers at different speeds create slow, hypnotic motion.
5. Differentiation: Radically different from all 3D geometric/prism approaches (v2-v14); unique abstract expressionist angle.
**Changes:**
- Full rewrite: original was a color-tint effect requiring inputImage
- 3-layer nested domain warp FBM (Inigo Quilez-inspired)
- Neon cosine palette: magenta→orange→gold→lime→cyan→violet
- fwidth()-based iso-contour black ink edges
- Deep violet background (0.01,0,0.05)
- Audio modulates hdrPeak intensity
**HDR peaks reached:** neonPalette peaks at 2.5 × hdrPeak; audio pushes to ~3.0
**Estimated rating:** 4.2★
