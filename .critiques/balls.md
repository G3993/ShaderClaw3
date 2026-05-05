# balls.fs — critique 2026-05-05

## Issues found
- **Desaturated palette**: `accentColor` default was `[1,1,1,1]` (white). Because the per-sphere hue rotation matrix has no effect on achromatic inputs, all spheres rendered as grey/white regardless of rotation angle.
- **Narrow hue spread**: `hueShift * 0.3` gave only ±54° spread; spheres clustered in a narrow tonal band.
- **SDR specular**: spec peak `0.8 + 0.3 = 1.1` — barely reaching white, no HDR punch for the host bloom pipeline.
- **SDR fresnel rim**: `fresnel * 0.3` produced a dim edge highlight, invisible against lit surfaces.
- **Audio bloom underpowered**: `audioLevel * audioDrive * 0.5` — bloom spheres barely brightened on beat.

## Changes made
1. `accentColor` default: `[1,1,1,1]` → `[0.2, 0.5, 1.0, 1.0]` (vivid electric blue — gives the hue rotation something to work with)
2. `hueShift` spread: `* 0.3` → `* 1.0` (full ±180° hue coverage, guaranteed distinct per-sphere colors)
3. Specular: `spec * 0.8 + spec2 * 0.3` → `spec * 3.0 + spec2 * 1.0`; dielectric boost `vec3(1.0)` → `vec3(2.5)`
4. Fresnel: `0.3` → `2.0`; fill term `vec3(0.5)` → `vec3(1.5)` — vivid HDR rim at grazing angles
5. Audio bloom drive: `0.5` → `2.0` — blooming spheres now spike to HDR on beat hits
