# vishes

## v1 — 2026-05-06
**Original angle:** Cellular random walkers leaving hue-drifting color trails on a slow-fading grid.
**Issues:** Random walkers produce unstructured noise; no focal element or recognizable shape; hue drift is cosmetic but the underlying motion is directionless; `gridSize` cell-snapping produces aliased step artifacts; no strong visual identity; audio only pulses brightness rather than shaping the behavior.

## v2 — 2026-05-06
**New angle:** Clifford Strange Attractor Nebula — strange attractor rendered as an additive HDR point cloud.
**Changes (full rewrite):**
- **Attractor**: Clifford map `x' = sin(a·y) + c·cos(a·x)`, `y' = sin(b·x) + d·cos(b·y)` with default params `a=-1.7, b=1.8, c=-1.9, d=-0.4` (classic "feather/bird" shape)
- **Per-pixel accumulation (pass 0)**: Each pixel fires 4 independent trajectories (seeded by `TIME`), each 150 steps (skip first 25 warmup). For each orbit point, map to screen coords and add Gaussian splash `exp(-d²/(2·σ²))` with `σ=2.5px` to `contrib`
- **Color**: each seed gets a distinct hue `fract(fs*0.25 + TIME*0.025)` + small angular variation `atan(p.y,p.x)*0.04` — produces 4 colored arms rotating slowly through hue
- **Slow morph**: `a,b,c,d` drift sinusoidally with `morphSpeed` parameter — attractor gradually shape-shifts without breaking (stays bounded)
- **Persistent canvas (HDR float)**: fade by `1 - fadeRate` each frame + add new contribution; dense attractor hotspots accumulate canvas values of 50-100, sparse regions ≈ 0.1-1.0
- **Display pass 1**: `sqrt(canvas) * brightness` tone curve compresses 2-decade HDR range — dense cores glow white-hot HDR (7+ linear), colored arms at 1.4 linear, faint wisps visible above deep indigo background `[0.003,0.001,0.012]`
- **Audio**: `audioBass*0.45` pulses contribution scale (density surge on beats); `audioLevel*0.2` global pulse
- **Strong silhouette**: near-black void between attractor arms vs HDR white-hot core — full dynamic range
- **2-pass** (no state buffer): canvas accumulates over time without needing walker state; random seed restarts each frame give ergodic attractor coverage
