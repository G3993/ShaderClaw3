# memphis_primitives.fs — critique 2026-05-05

## Issues found
- **SDR palette**: All fills ≤ 1.0 — red at 0.9, yellow at 0.96, blue at 1.0. No HDR peak to drive the host bloom pipeline; the shapes look flat and washed-out.
- **No ink outlines**: SDF boundaries used bare `smoothstep(0.0, 0.02, d)` AA, producing a soft muddy edge. Memphis Group design language demands crisp black ink borders for contrast and graphic impact.
- **Weak audio coupling**: `audioLevel * 0.05` gave imperceptible beat response; visual energy didn't change with music.
- **SDR surprise squiggle**: Lipstick pink `vec3(1.0, 0.40, 0.65)` was entirely within SDR — no bloom from the host pipeline.

## Changes made
1. **HDR palette**: red `0.9→2.4`, yellow `0.96→2.8`, blue `1.0→3.2` — each above 2.0 linear for reliable host bloom
2. **Ink outline helper** `inkFill()`: uses `fwidth(d)` to compute screen-space antialiasing + a `pw*6.0` wide black ink band at every SDF boundary, replicating authentic Memphis printing aesthetics
3. Updated all 7 primitive branches (`circle`, `square`, `triangle`, `checker-in-circle`, `stripes-in-square`, `squiggle`, `polka-dots-in-square`) to use `inkFill()` or equivalent ink border logic
4. **Audio pulse**: `audioLevel * 0.05` → `audioLevel * audioBass * 0.7` multiplicative — shapes spike on bass transients
5. **Surprise squiggle**: SDR pink `[1.0, 0.40, 0.65]` → HDR `[3.5, 0.4, 1.8]` — now blooms through the host pipeline
