# text_spacy.fs — critique log

## v8 — 2026-05-05 — Infinite Crystal Forest (3D Raymarched fly-through)

**Approach:** 3D SDF raymarched fly-through of an infinite grid of glowing crystal columns. Prior attempts were ISF perspective text tunnel (font atlas, row scaling by perspective) producing 2D monochrome text in depth illusion with no real 3D geometry.

**Technique:** XZ grid repetition with jittered per-cell placement:
```glsl
vec2 cellId = floor(p.xz / cellSize + 0.5);
// Check 3×3 neighborhood for nearest column
vec3 ctr = vec3((cid.x + jitter.x) * cellSz, 0.0, (cid.y + jitter.y) * cellSz);
float stem = sdCap(p - ctr, vec3(0), vec3(0, h, 0), r);
float tip  = sdOct(p - (ctr + vec3(0, h + tipSz*0.8, 0)), tipSz);
```

**Geometry:** Each crystal = capsule stem + octahedral tip at apex. Height and radius vary per-cell via hash. Audio pulse modulates both stem radius and tip size.

**Camera:** Flies along Z with `zOff = TIME * flySpeed`, gentle horizontal weave via `sin(TIME*0.27)`. Looks slightly upward for depth impression.

**Palette (5 colors, warm forest tones):**
- Magenta `(1.0, 0.0, 0.6)`
- Gold `(1.0, 0.7, 0.0)`
- Cyan `(0.0, 0.9, 1.0)`
- Green `(0.0, 1.0, 0.35)`
- Violet `(0.6, 0.0, 1.0)`

**Floor:** Dark mossy ground with crystal-grid vein lines (fract-based grid at cell boundaries).

**HDR:** rim × 1.6 × glowStrength + spec × 1.2 × glowStrength + emission × 0.35. glowStrength default 2.2 → rim peaks ~3.52 linear.

**Fix vs prior:** Replaced ISF font-atlas perspective tunnel (2D text rows, no 3D, monochrome) with a real 3D infinite crystal geometry fly-through.
