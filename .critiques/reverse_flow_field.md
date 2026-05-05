# reverse_flow_field.fs — critique log

## v8 — 2026-05-05 — Aurora Borealis Curtains (2D layered)

**Approach:** 2D layered sinusoidal light curtains simulating aurora borealis in a polar night sky. Prior attempts were 2D flow fields with particle systems producing dim, desaturated trails.

**Technique:** N sinusoidal aurora curtains (2–10), each defined by a horizontally oscillating band:
```
wavey = yBase + sin(uv.x * freq * π + t*spd + ph) * 0.075
              + sin(uv.x * freq * 1.7 + t*spd*0.65 + ph*1.3) * 0.035
band = exp(-dy² / bw²) * 1.6
```
Layered additively with HDR glow. AudioBass modulates band brightness via `audioPulse`.

**Additional elements:**
- 60 twinkling stars (smoothstep point lights, sinusoidal twinkle)
- Treeline silhouette at bottom 13% using 3-octave sin noise
- Faint moon halo at (0.82, 0.78)

**Palette (5 colors, fully saturated cool tones):**
- Vivid green `(0.0, 1.0, 0.4)`
- Electric cyan `(0.0, 0.8, 1.0)`
- Deep violet `(0.6, 0.0, 1.0)`
- Magenta `(1.0, 0.0, 0.65)`
- Teal-mint `(0.1, 1.0, 0.8)`

**HDR:** bands × hdrPeak (default 2.3), no clamping. Peaks ~3.7 linear with audio.

**Fix vs prior:** Replaced dim 2D reverse flow field (low contrast, grey-brown palette) with high-contrast polar aurora scene — dark sky + HDR oversaturated light curtains for maximum visual punch.
