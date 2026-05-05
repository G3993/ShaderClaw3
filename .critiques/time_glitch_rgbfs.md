## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: VIDVOX 8-frame buffer delay — requires inputImage; produces nothing standalone.
2. Compositional craft: Frame buffering is purely an effect; no content without source.
3. Technical execution: 9-pass persistent buffer architecture is complex and correct, but all passes output noise without input.
4. Liveness: TIME-driven via random delay shift, but input-dependent.
5. Differentiation: Interesting channel-split temporal effect; not a generator.
**Changes:**
- Full rewrite as "Signal Interference" — raymarched 3D RGB data planes with glitch geometry
- Three independently marched color planes; per-channel glitch displacement; HDR fully saturated
**HDR peaks reached:** per-channel hdrBoost * diffuse = 2.0; white spec adds ~2.5
**Estimated rating:** 4.0★

## 2026-05-05 (v5)
**Prior rating:** 0.0★
**Approach:** 2D SDF — NEW ANGLE: Neon Mandala Engine; sacred geometry rotary vs prior 3D RGB glitch planes (v1/v2)
**Critique:**
1. Reference fidelity: Original was RGB frame-delay glitch — this is a totally different 2D sacred-geometry concept.
2. Compositional craft: Concentric rings + 8-fold petal symmetry with dark center anchor — strong radial composition; outer vignette frames it cleanly.
3. Technical execution: 2D SDF petals (ellipse), rings, spokes all with fwidth() AA; multi-ring layering; independent counter-rotation on petal sets.
4. Liveness: TIME-driven dual-direction spin on petal groups; audio modulates brightness; halo glow around ring radii.
5. Differentiation: 2D mandala (saffron/crimson/gold/violet) vs 3D glitch planes (red/green/blue); Indian sacred geometry vs data glitch; warm vs signal-cold.
**Changes:**
- Full rewrite as "Neon Mandala Engine" — 2D SDF rings, petals, spokes
- 4 concentric ring iso-lines at R0–R3 with per-ring color
- Petal sets on each ring with independent spin speed and direction
- Radial spoke pattern at 2 angular frequencies
- fwidth() AA on all SDF layers
- 4-color palette: saffron/crimson/gold/violet — fully saturated
- Outer vignette + hard clip at R3+0.12 for clean composition
- Audio modulates brightness
**HDR peaks reached:** rings + petals * glowPeak * audio = 2.5
**Estimated rating:** 4.0★
