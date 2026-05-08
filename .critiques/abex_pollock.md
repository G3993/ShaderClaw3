## 2026-05-08
**Prior rating:** unrated
**Approach:** 2D persistent-paint refine — NEW ANGLE: wet-paint HDR + fwidth AA (restore anti-aliased stroke edges stripped by linter, push specular peaks and gold flash into linear HDR, fix motion audit violations)
**Critique:**
- *Composition*: all-over drip skein reads as genuine Pollock; segment-based deposit gives continuous skeins not bead-chains — strong.
- *Palette*: five per-work palettes are correct; black/ochre/red/silver contrast is solid. No white-mixing.
- *Motion*: two epoch violations found: pooling snap at rate=0.3 (3.3s cycle < 5s minimum) and wHash at rate=1.2 (0.83s). Audio K exceeded 1.5 cap at MAX audioReact.
- *Silhouette*: stroke thickness variation good; linter had removed AA so edges were aliased. Restored fwidth-based smoothstep.
- *HDR fidelity*: linter had stripped HDR core peaks from strokes and reduced specular/glisten to sub-1.0; gold flash was SDR (1.0). All restored.
**Changes:**
- Pooling epoch: `floor(TIME * 0.3)` → `floor(TIME * 0.15)` — 6.67s cycle at audio=0
- Removed dead `wHash` variable (`floor(TIME * 1.2)`)
- Audio K fixed: wander `(0.5 + mid * react * 1.5)` → `(1.0 + mid * react * 0.75)`, stroke width K 0.8 → 0.75, splatter K 1.3/1.5 → 0.75 — all ≤ 1.5 at MAX audioReact
- Restored `fwidth()`-based AA on stroke edges and splatter dots
- Restored HDR core peak on bright/metallic strokes: `c + c * corePeak * 1.2` (peaks ~2.1 linear on white/silver)
- Specular ridge peak: `specBoost * ridge * 2.2` (was 0.20, no ridge mask)
- Wet-paint glisten: `1.1` (was 0.15)
- Gold flash: `vec3(1.80, 1.48, 0.55)` (was `vec3(1.00, 0.85, 0.35)`) — HDR peak ~1.8 linear
**Motion audit:** pooling epoch 0.15 ≤ 0.2 ✓; audio K ≤ 1.5 at all audioReact values ✓; no camera motion. Gold flash 14s cycle = 0.071 ≤ 0.2 ✓.
**HDR peaks reached:** ~2.1 linear on metallic/white stroke cores; ~2.2 on specular ridges; ~1.1 on wet glisten; ~1.8 on gold flash
**Estimated rating:** 3★
