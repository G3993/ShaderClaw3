## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Standalone HSV Prism Globe (first critique, replacing image-tint effect)
**Critique:**
1. Reference fidelity: Original was an image tint utility. New version becomes a standalone color gamut visualizer — HSV sphere is a natural "color picker" reference.
2. Compositional craft: Full-screen sphere with star background. Hue gradient along azimuth, value gradient from pole to pole. Strong focal point.
3. Technical execution: Analytic ray-sphere intersection (no march needed). Dual-axis rotation for globe tumble. fwidth() edge AA for black silhouette. Violet rim light creates HDR glow corona.
4. Liveness: Dual-axis rotation via TIME * rotSpeed. Audio modulates sphere radius (bass pulse).
5. Differentiation: Completely new concept: standalone 3D color sphere vs image effect. First proper generator in this slot.
**Changes:**
- Full rewrite: standalone HSV sphere generator (no inputImage)
- Analytic ray-sphere intersection (optimal for sphere)
- HSV mapped to sphere surface: longitude→hue, latitude→value, sat=1.0
- Dual-axis rotation animates the color globe
- Star background (hash-based point stars)
- Violet rim light (cool backlight glow) → HDR 2.0+
- fwidth() AA silhouette edge
- Audio modulates sphere radius
**HDR peaks reached:** surfCol * 2.2 (hdrBoost) + spec * 3.0 + rim * 2.0 = ~3.5 at specular + rim
**Estimated rating:** 4.0★
