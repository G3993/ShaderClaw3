# bars.fs — Critique (2026-05-08)
**Angle: Audio K Compliance + HDR Audit**

## 5-Axis Assessment

### 1. Composition / Layout
Strong. Mirrored-column layout with per-stripe phase offset creates pleasing wave relationships. The easing library is the real value here — 30 distinct easing modes all working correctly. The surprise burst bar (every ~17 s) fires with 22% ramp-in and 44% ramp-out of its 3-second active window, satisfying the ≥15% easing ramp rule.

### 2. Palette / Color
Functional grayscale by design (easing demo). Magenta tint during the burst adds the only chromatic moment. Palette is intentionally neutral to let the easing shape be the star.

### 3. Motion Discipline
**Violation found and fixed:** `1.0 + audioBass * 2.0` applied a K=2.0 speed multiplier to the phase driver, exceeding the K≤1.5 cap. Changed coefficient to 1.5. The base `speed` slider (DEFAULT 0.1, MAX 1.0) is within the animation-pulse-rate default range (0.5–1.5 is the target, but 0.1 is sensible for an easing demo that shouldn't auto-scroll aggressively).

### 4. Silhouette / Clarity
Clean stripe silhouettes. The easing functions produce well-defined bar profiles. No AA needed — bars are axis-aligned integer-pixel wide.

### 5. HDR Fidelity
The surprise burst already produces HDR output: `mask += _bandX * _f * 0.9` pushes mask above 1.0 (up to ~1.9), and the no-texture path outputs `mask` directly, so burst bars fire at ~1.9 HDR red. Downstream bloom will catch these peaks.

## Change Summary
- `(1.0 + audioBass * 2.0)` → `(1.0 + audioBass * 1.5)` (K cap enforcement, line 337)
