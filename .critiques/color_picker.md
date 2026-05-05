# color_picker.fs — critique log

## v9 — 2026-05-05 — Neon Mandala Kaleidoscope (2D generator)

**Approach:** 2D polar-coordinate kaleidoscope generator with HDR neon layers. Prior version was a trivial `inputImage * color * intensity` tint — no geometry, fully image-dependent, essentially zero creative output without an input.

**Technique:** K-fold angular mirror symmetry, then layered SDF-based drawing in reduced polar sector:
```glsl
float sector = TWO_PI / K;
theta = mod(theta, sector);
if (theta > sector * 0.5) theta = sector - theta;  // mirror
vec2 kp = vec2(r * cos(theta), r * sin(theta));
```

**5 layers (all additive HDR):**
1. **Concentric rings** — 5 rings at r=0.13..0.65, Gaussian glow, ring 1 (innermost) responds to audioBass
2. **Radial petal lines** — `abs(sin(theta * K)) - 0.92` → K petals fading toward center
3. **Inner starburst** — double-frequency petals at half-size, audioBass-reactive
4. **Diamond lattice** — `fract`-based grid on kp, only inside r<0.7
5. **Outer halo** — radial bloom at r>0.7

**Center spark:** Gaussian center flash `exp(-r² × 180)` in gold, audioBass-scaled.

**Breathing background:** slow `sin(TIME*0.8)` oscillation between near-black and deep-violet.

**Palette (5 colors, fully saturated neon):**
- Hot pink `(1.0, 0.0, 0.55)`
- Electric cyan `(0.0, 0.9, 1.0)`
- Gold `(1.0, 0.8, 0.0)`
- Vivid green `(0.0, 1.0, 0.4)`
- Violet `(0.65, 0.0, 1.0)`

**HDR:** All layers additive × hdrPeak (default 2.4). No inputImage dependency — pure generator.

**Fix vs prior:** Replaced trivial image-tint (not a generator, depends on inputImage) with a standalone HDR kaleidoscope mandala generator.
