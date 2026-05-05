# random_freeze.fs — critique log

## v7 — 2026-05-05 — Bioluminescent Coral (3D Raymarched)

**Approach:** Full 3D SDF raymarch of a bioluminescent coral reef in dark water. Prior attempts were 2D freeze/noise effects producing static, desaturated visuals with no depth or motion.

**Geometry:** N coral branches (2–10) at golden-angle positions on the sea floor. Each branch: main capsule stem + 3 evenly-spaced sub-branches (smooth-unioned via `smin`) + polyp spheres at all tips. Branches sway via `sin(TIME * sway + id * 2.1)`. Sea floor at y=-0.08.

**SDFs used:**
- `sdCap` (capsule) — stem and sub-branches
- `sdSphere` — polyp tips at branch ends
- `smin` with k=0.04–0.06 for organic blending

**Palette (5 colors, fully saturated):**
- Magenta `(1.0, 0.05, 0.75)` — hot primary
- Cyan `(0.0, 1.0, 0.85)` — electric secondary
- Green `(0.2, 1.0, 0.2)` — vivid mid
- Orange `(1.0, 0.55, 0.0)` — warm accent
- Violet `(0.6, 0.0, 1.0)` — cool deep

**HDR:** bioluminescent emissive `basecol * 0.45 * glowStrength` + rim `* 1.5 * glowStrength` + spec. glowStrength default 2.3 → rim peaks ~3.45 linear.

**Audio:** audioBass pulses polyp sphere radii; audioMid modulates colored window bleeding; 20 floating bioluminescent particles react to `particleAmt`.

**Camera:** Slow orbit `camA = TIME * 0.1`, radius 3.6, height 1.0, looking at (0, 0.6, 0).

**Fix vs prior:** Replaced flat 2D freeze/noise glitch effect (no geometry, desaturated) with fully saturated 3D organic coral scene.
