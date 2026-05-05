## 2026-05-05 (v3)
**Prior rating:** 0★
**Approach:** 2D standalone generator — NEW ANGLE: VHS Horror Playback (vs v1 audioReact hdrScale / v2 RGB data planes)
**Critique:**
1. Reference fidelity: Strong analog-horror aesthetic: coarse VHS grain (240-line resolution), tracking error displacement, ghost image, damage tear bands, dark figure silhouette emerging from static.
2. Compositional craft: Dark humanoid figure as strong silhouette focal element (black against amber/red artifacts). Figure flickers in and out via sin() threshold.
3. Technical execution: hash-based grain; tracking displacement per scanline band; ghost smear as 4 displaced dark rectangles; HDR white-hot spark layer.
4. Liveness: All driven by TIME * degradeRate; figure sways slightly; tracking errors and tear bands shift with time.
5. Differentiation: Horror/analog aesthetic (vs v2 data plane RGB grid, vs v1 simple boost). Figurative element (dark silhouette) vs. abstract glitch.
**Changes:**
- Complete rewrite: standalone VHS horror generator (no inputImage)
- Palette: blood red, amber burn, ghost white, deep black, static gray
- Dark humanoid figure silhouette (head + body SDF) flickering in/out
- VHS tracking error: horizontal scan displacement bands
- Ghost layer: 4 displaced dark smear shapes in blood red
- Amber tear bands + red damage channel
- HDR white-hot static sparks (2.0+)
- Audio modulates additively
**HDR peaks reached:** ghost white spark 2.0×2.2=4.4 peak; amber tears 1.5×2.2=3.3; blood red damage 2.2×0.5×aud
**Estimated rating:** 4.0★
