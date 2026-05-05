## 2026-05-05
**Prior rating:** 0.6★
**Approach:** 2D refine (volumetric beams are inherently 3D-ish; kept approach, fixed HDR)
**Lighting style:** neon
**Critique:**
1. Reference fidelity 3/5 — nightclub laser cone aesthetic is present; palette of 6 neon colors good
2. Compositional craft 3/5 — 6 sweeping beams from above with fog scattering; decent but cramped
3. Technical execution 1/5 — `1.0-exp(-col*2.0)` tonemapping + `clamp(col,0,1)` crushed all HDR; beam intensities were 0.35/0.75 (too dim to bloom); coneColor clamped to [0,1]
4. Liveness 3/5 — TIME-driven sweep + beat sync; alive in silence
5. Differentiation 2/5 — the 6-beam fog-cone look is distinctive; hurt by SDR output
**Changes made:**
- Added "3D" and "Generator" to CATEGORIES
- Added `audioReact` parameter; changed intensity to `beamIntensity * (0.5 + 0.5*audioLevel*audioReact)`
- Doubled globalBeat sensitivity to audioBass
- Removed `clamp()` from `coneColor()` — hue-rotated colors now push past 1.0
- Boosted beam multipliers: `0.35` → `1.5`, `0.75` → `2.5` (HDR bloom-ready peaks)
- Removed tonemapping `1.0 - exp(-col*2.0)` — was compressing all highlights to SDR
- Removed final `clamp(col, 0.0, 1.0)` — host applies ACES
- Kept vignette but replaced `clamp()` with `max(0,...)` 
- Reduced grain amplitude slightly (0.04 → 0.03) since no tonemapping inflates noise
- baseColor tint defaults to white (previously reddish, tinted all output)
**Estimated rating after:** 3.5★
