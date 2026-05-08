# black_hole_sun.fs — Critique (2026-05-08)
**Angle: Gamma Removal, Linear HDR Solar Output, Warm Palette**

## 5-Axis Assessment

### 1. Composition / Layout
Effective. The radially-centered fbm warp creates a convincing solar-corona structure. The `curvature` parameter lets users dial the apparent scale of the disk. The `warp` flag for the double-fbm distortion is a nice optional chaos layer.

### 2. Palette / Color
**Critical issue fixed.** Original palette was pure B&W, which technically satisfies "black" as the predominant color but misses the "fully saturated, 4-6 colors" intent. The "black hole sun" concept implies an intense stellar object — orange-gold solar corona against deep space black is the canonical read. Added warm tint: corona rays rendered in `(1.8, 0.9, 0.25)` (orange-gold) and the solar disk core in `(2.5, 2.0, 1.4)` (HDR gold-white). The black space (where fbm val is high → `1-val` near zero) remains deep black, giving strong silhouette contrast.

### 3. Motion Discipline
Clean. `float t = time * 0.025` — animation rate well within bounds. No audio reactivity exists (inputs are purely visual), so no K violations. The 0.025 base is the time-driver for the fbm walk, not a user-controlled speed, so it's internal.

### 4. Silhouette / Clarity
Strong radial structure with clear center/corona/space regions. The `ray_density` parameter controls how tight the corona rays are. The `curvature` and `brightness` interact cleanly to define the apparent size of the solar disk.

### 5. HDR Fidelity
**Critical issue fixed.** `gl_FragColor = sqrt(vec4(col, 1.0))` was baking gamma 0.5 into the output — a rules violation (no Reinhard/ACES/gamma). The `clamp()` on the initial col also hard-capped output at 1.0. Removed both. Solar core now outputs `vec3(2.5, 2.0, 1.4)` at center — bloom will catch these peaks as the stellar disc glows. Corona rays reach up to `(1.8, 0.9, 0.25)` where fbm dips to zero.

## Change Summary
- Removed `gl_FragColor = sqrt(vec4(...))` → `gl_FragColor = vec4(col, 1.0)` (linear output)
- Removed `clamp(1.0 - val, 0, 1)` → `max(1.0 - val, 0.0)` (no HDR cap on rays)
- Added warm solar color: `* vec3(1.8, 0.9, 0.25)` on corona rays
- Solar core: `mix(..., vec3(2.5, 2.0, 1.4), spotF)` with normalized spotF (no extrapolation overflow)
