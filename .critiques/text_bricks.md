# text_bricks.fs — critique log

## v8 — 2026-05-05 — Gothic Cathedral Interior (3D Raymarched walk-through)

**Approach:** 3D SDF raymarched walk-through of a gothic cathedral nave with stained glass light beams. Prior attempts were 2D brick/text renderers producing flat, pixelated results.

**Geometry:**
- Tiled nave sections (mod-based Z repetition, `span=2.5`)
- Gothic arch SDF: two overlapping circle intersection + box undercut
- Side walls (box SDFs) with arched stained-glass window openings subtracted via CSG
- Vaulted ceiling following arch profile

**Gothic arch SDF:**
```glsl
float r = sqrt((w*0.5)² + h²) * 0.5 + w*0.15;
float interior = max(d1, d2);  // inside both circles
float box = min(|p.x| - w*0.5, -p.y);
return max(interior, -box);
```

**Camera:** Walk-through with `ro.z += TIME * walkSpeed * 0.5`, slight side-to-side sway via `sin(TIME*0.12)*0.4`.

**Lighting:**
- Main directional light (stone diffuse + spec)
- 5 colored window light sources (crimson/royal-blue/emerald/amber/violet)
- Volumetric light shaft accumulation (5-source inner loop, Gaussian falloff)
- Colored window light bleeding onto stone surfaces

**Palette (5 colors, fully saturated stained glass):**
- Crimson `(1.0, 0.0, 0.15)`
- Royal blue `(0.0, 0.4, 1.0)`
- Emerald `(0.0, 0.85, 0.2)`
- Amber `(1.0, 0.65, 0.0)`
- Violet `(0.55, 0.0, 1.0)`

**HDR:** Shaft accumulation × hdrPeak × 0.08 + colored bleeding × hdrPeak × 0.35. hdrPeak default 2.5.

**Audio:** audioBass modulates shaft brightness; audioMid modulates window color bleeding.

**Fix vs prior:** Replaced flat 2D brick/text rendering with a fully 3D atmospheric gothic interior walkthrough.
