# Autonomous Shader Improvement Loop

## Overview

A pipeline that audits the shader library, scores each shader on heuristic
metrics, picks the worst-scored ones, and generates targeted edits — then
verifies and commits improvements automatically.

## Components

### 1. Renderer — `audit_shaders.exe`

Source: `C:\Users\nofun\easel-ux-snapshot\src\audit_shaders.cpp`

Renders every `.fs` shader at four fixed `TIME` values (0.5, 2.0, 5.0, 9.0)
to 256×256 PNGs. Writes `<out>/index.json` listing all rendered frames.

**Run:**
```
audit_shaders.exe <shaders_dir> <out_dir> [resolution]
```

### 2. Heuristic critic — `tools/score_audit.py`

Source: `C:\Users\nofun\easel-ux-snapshot\tools\score_audit.py`

Reads the rendered frames; computes four metrics per shader:
- **density** (luminance variance + mean centering)
- **movement** (frame-to-frame pixel diff)
- **palette** (color-bucket entropy)
- **edges** (gradient magnitude average)

Each scaled to 0..10. Overall = mean. Writes `scores.json`.

**Run:**
```
python score_audit.py <audit_dir>
```

### 3. Scores baseline — `scores.json` (committed)

The latest baseline scores are committed to the repo so cloud agents
without local rendering can read them.

### 4. Patcher (next iteration)

A scheduled cloud agent that pulls latest, reads `scores.json`, picks
worst-scored shader, generates a targeted edit based on its weak axis,
commits + pushes.

## Heuristic axes → fix templates

| Weak axis | Programmatic fix |
|---|---|
| density < 4 | Bump count / N parameters by 1.5x; add secondary noise layer |
| movement < 4 | Add audio-reactive scale; add slow drift on existing center inputs |
| palette < 4 | Snap output to a known LUT (named-reference shaders); add posterize |
| edges < 4 | Add a contour-line layer; bump line-width parameters |

For named-reference shaders (art-movement series), a multimodal critic
should compare the rendered frame to a reference image and score the
"reference fidelity" axis specifically.

## Loop schedule

A nightly routine pulls origin/master, runs the patcher on the lowest
3 shaders, commits + pushes. Verified by re-running audit locally
(human verification).

## Manual override

To skip a shader from auto-patching, add `"skip_auto": true` to its
manifest entry (TODO).

## Initial baseline scores (2026-05-02)

Average: 3.1 / 10 across 111 shaders (severe undercount — ~10 shaders
with persistent buffers fail to render in audit warmup; needs longer
warmup window in audit_shaders.cpp).

Top 5: opart_riley_waves (8.9), fluid_pixels (8.8), lightning (8.7),
blobscillator (8.5), cascade_text (8.3).

Bottom 5 (excluding 0.0 warmup-failures): art_nouveau_mucha (3.8),
vaporwave_floral_shoppe (3.8), black_hole_sun (3.9), particles (3.9),
solar_flare_corona (3.9).
