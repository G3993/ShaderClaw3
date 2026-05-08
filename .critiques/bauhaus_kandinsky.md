# bauhaus_kandinsky.fs — Critique (2026-05-08)
**Angle: Positional K Cap, Rotation Base, HDR Primaries**

## 5-Axis Assessment

### 1. Composition / Layout
Excellent. Kandinsky's strict yellow=triangle, red=square, blue=circle pairing is faithfully implemented. Lissajous orbits with slowly drifting home positions prevent the layout from repeating. The checkerboard interior on every 4th shape is an authentic Composition VIII detail. Support lines and pattern bands enrich the negative space.

### 2. Palette / Color
Strong. Three fully-saturated Bauhaus primaries. The per-painting backgrounds (black for Several Circles/Composition X, warm white for Yellow-Red-Blue, near-white for Composition VIII) are accurate. No white-mixing on the primaries themselves.

### 3. Motion Discipline
**Two violations found and fixed:**
1. **Positional K violation:** `ctr = home + orbit + fromCtr * springReact * audioBass * audioReact`. At springReact MAX=0.4 and audioReact MAX=2.0, positional K_pos = 0.4 × 2.0 = 0.8 > 0.6 cap. Fixed by capping `springReact` MAX at 0.3 (worst-case K = 0.3 × 2.0 = 0.6 = cap exactly).
2. **Rotation speed / no-audio base:** `TIME * audioMid * audioReact * 0.4` drives rotation only when audio is active — shapes are frozen without audio input. Added base rate 0.15 rad/s (within the 0.10–0.30 default range) with audio adding up to K=1.0 at audioReact MAX: `TIME * 0.15 * (1.0 + audioMid * audioReact * 0.5)`. At audioReact default=1.0 max audio rate = 0.225, stays within range.

### 4. Silhouette / Clarity
Sharp SDF shapes with 0.0025 feather. Black support lines read clearly on both light and dark backgrounds. The halo gradient (Several Circles) is soft enough to not crowd the primaries.

### 5. HDR Fidelity
Zero HDR before this pass — all shape fills capped at 1.0. Added additive core boost: `col += bestCol * fillMask * 1.0`. Result: yellow cores reach (1.96, 1.70, 0.20), red cores (1.78, 0.24, 0.28), blue cores (0.20, 0.36, 1.30). Downstream bloom will pick up the peaks.

## Change Summary
- `springReact` MAX: 0.4 → 0.3 (K_pos cap enforcement)
- Rotation: `TIME * audioMid * audioReact * 0.4` → `TIME * 0.15 * (1.0 + audioMid * audioReact * 0.5)` (base rate + K cap)
- Added `col += bestCol * fillMask * 1.0` (HDR additive core for bloom)
