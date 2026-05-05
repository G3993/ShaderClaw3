## 2026-05-05
**Prior rating:** 2.1★
**Approach:** 2D refine (already a 3D SDF raymarch — remove the forbidden ACES tonemap and fix HDR)

**Critique:**
- *Reference fidelity:* Rotating rounded cubes in SDF space is a good concept. But `baseColor [1,1,1]` white means all cubes are colorless regardless of the procedural gradient inside.
- *Compositional craft:* Procedural gradient `0.5 + 0.5 * cos(...)` only spans [0,1] — the gradient is always full-SDR and gets squashed further by ACES.
- *Technical execution:* ACES tonemap (`col * (2.51*col+0.03) / (col*(2.43*col+0.59)+0.14)`) is explicitly forbidden — it compresses HDR highlights, destroys the electric specular peaks, and adds an orange-tint shift. Specular `spec1 * fresnel * 1.2` barely reaches 1.2 before getting ACESed to ~0.85.
- *Liveness:* Audio glow adds `vec3(0.05, 0.03, 0.02)` — invisible, warm-tinted, wrong direction for the crystal aesthetic.
- *Differentiation:* Looks like white plastic, not crystal, at default settings.

**Changes:**
- **Removed ACES tonemap entirely** — HDR values now pass through to output
- Changed `baseColor` default to electric blue `[0.2, 0.4, 1.0]` — crystals now have a vivid blue-indigo base
- Procedural gradient: `0.5 + 0.5 * cos(...)` → `1.0 + 1.5 * cos(...)` — HDR gradient peaks at 2.5 linear
- Changed reference tint in procCol from `vec3(0.85, 0.92, 1.0)` to `vec3(0.6, 0.8, 2.5)` — neon blue anchor
- Specular: `vec3(1.0) * spec1 * fresnel * 1.2` → `vec3(1.2, 1.5, 3.0) * spec1 * fresnel * 3.5` — electric blue-white hotspot, peaks ~3.5 linear
- Audio glow: `vec3(0.05, 0.03, 0.02)` → electric blue `vec3(0.1, 0.2, 0.6) * (1.0 + audioBass*0.8)`

**HDR peaks reached:** ~3.5 (specular), ~2.5 (gradient), ~4.0+ at audio peaks
**Estimated rating:** 3.5★
<!-- auto-improve 2026-05-05 -->
