# oil_paint.fs — critique log

## v8 — 2026-05-05 — Alien Mushroom Garden (3D Raymarched)

**Approach:** Full 3D SDF raymarch of a bioluminescent alien mushroom field. Prior attempts were oil-paint 2D stylization which produced muddy, desaturated results with no focal depth.

**Geometry:** N mushrooms (2–12) at golden-angle positions, each a smooth-union of a capsule stem and a squashed-sphere cap via `smin`. Caps pulse gently with audioBass. Per-mushroom sway via `sin(TIME * swaySpeed + ph)`. Sandy floor with subtle grid lines.

**Palette (5 colors, fully saturated):**
- Cyan `(0.0, 1.0, 0.85)` — primary glow
- Magenta `(1.0, 0.05, 0.75)` — accent caps
- Chartreuse `(0.35, 1.0, 0.0)` — mid-ground
- Violet `(0.55, 0.0, 1.0)` — back mushrooms
- Orange `(1.0, 0.6, 0.0)` — warmth accent

**HDR:** bioluminescent emissive `basecol * 0.55 * glowStrength` + rim `* 1.4 * glowStrength` + spec. glowStrength default 2.2 → rim peaks ~3.1 linear.

**Audio:** audioBass pulses cap radius; floating spores (0–40) light up with cyan glow.

**Ink silhouette:** `face = smoothstep(0.0, 0.22, dot(n, -rd))` darkens rim grazing angles.

**Camera:** Slow orbit `camA = TIME * 0.09`, height 1.1, looking at origin y=0.5.

**Fix vs prior:** Replaced flat 2D oil-paint texture effect (washed desaturated) with fully saturated 3D bioluminescent scene.
