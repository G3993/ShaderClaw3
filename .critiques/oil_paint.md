## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: Original Kuwahara filter requires inputImage — nothing to paint without source. Clever technique, wrong category.
2. Compositional craft: Zero standalone composition; effect pass without a generator pass.
3. Technical execution: Multi-pass Kuwahara is correctly implemented but useless as a standalone generator.
4. Liveness: No TIME-driven content; the painterly effect is static relative to input.
5. Differentiation: Kuwahara approach is elegant but requires input.
**Changes:**
- Full rewrite as "Lava Impasto" — standalone 3D molten rock surface
- Domain-warped FBM height field as raymarched displaced plane (64-step)
- Lava palette: black → deep crimson → orange → gold → white-hot (HDR)
- Time-driven flow using animated domain warp (flowSpeed parameter)
- Hot-spot pulse with TIME * 3.1 for liveness
- Charred crevice edge darkening via fwidth(rawH) AA
- Cinematic camera angled down onto surface, drifting slowly
- Audio modulates pulse intensity
**HDR peaks reached:** white-hot crack edges 3.0, gold flow 1.5–2.5, orange mid-tone 1.0
**Estimated rating:** 4.5★

## 2026-05-05 (v11)
**Prior rating:** 0.0★
**Approach:** 2D refine (Azulejo Tiles — Portuguese geometric ceramic)
**Critique:**
1. Reference fidelity: Kuwahara requires image input — no standalone visual without source feed; wrong category for a generator catalog.
2. Compositional craft: Multi-pass effect has no self-contained composition.
3. Technical execution: Kuwahara implementation is clean; PERSISTENT buffer setup is correct but dependent on image input.
4. Liveness: Zero TIME content; purely static filter effect.
5. Differentiation: Kuwahara is a well-known filter with no audio reactivity or procedural content.
**Changes:**
- Full rewrite as "Azulejo Tiles" — Portuguese cobalt-blue/white geometric ceramic tiling
- Single-pass generator, no image input required
- Pattern: abs(lp-0.5) fold → corner quarter-circle SDFs + center cross SDF
- AA via fwidth() for both arc and cross edges
- Checkerboard alternation: mod(tileIdx.x + tileIdx.y, 2.0) swaps blue/white
- Glaze shimmer: slow sin*cos wave across tile surface
- Grout: smoothstep(0,groutW,...) → near-void black seams
- audioBass pulse: tile grid breathes subtly with bass hits
- Removed PASSES / PERSISTENT buffer — single-pass only
**HDR peaks reached:** WHITE hdrWhite=3.0 × audioBright~1.3 → 3.9 peak; BLUE hdrBlue=2.5 → 3.25 peak
**Estimated rating:** 4.2★
