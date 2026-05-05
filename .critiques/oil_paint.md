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

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D ripple simulation — NEW ANGLE: overhead view of ink drops dispersing in water; v1 was 3D lava impasto, v2-v12 all 3D terrain/mountain/surface. This is 2D simulation — completely different physical metaphor and color palette.
**Critique:**
1. Reference fidelity: Ink-in-water physics (spreading drops, ripple rings, domain-warped organic edges) creates a genuinely painterly feel without 3D raymarching.
2. Compositional craft: Multiple drop centers create interference patterns; age-based decay prevents stale buildup.
3. Technical execution: Domain-warp via snoise gives organic non-circular ripples; age exponential decay keeps motion clean.
4. Liveness: TIME-driven radius expansion + ring oscillation; each drop has staggered birth time for perpetual motion.
5. Differentiation: Blue/violet ink palette is the opposite of all lava/fire/terrain approaches (v1-v12).
**Changes:**
- Full rewrite: 2D ripple simulation (vs 3D terrain raymarch)
- N animated ink drops (default 6) with staggered birth times
- Domain-warped ripple rings via sin(dist*freq - t) envelope
- Ink palette: deep cobalt->violet (HSV hue 0.55-0.80, sat=1.0)
- White-hot wave crests at 2.5 HDR
- Black trough ink darkening for contrast
- Audio modulates hdrPeak
**HDR peaks reached:** wave crests at 2.5 x audio ~= 3.0; white-hot crest secondary at 1.25+
**Estimated rating:** 4.4★
