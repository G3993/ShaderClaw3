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
**Approach:** 3D raymarch — NEW ANGLE: "Mountain Impasto" — Fauvist mountainscape with FBM terrain. Sunny daylight, warm/cool opposition. vs. prior v1 (Lava Impasto — volcanic molten rock, dark/volcanic).
**Critique:**
1. Reference fidelity: Kuwahara filter abandoned; now 3D terrain inspired by Matisse Fauvism — vivid non-naturalistic color opposition.
2. Compositional craft: FBM derivatives as brush-stroke normals give thick paint relief feel; elevation palette maps color zones cleanly.
3. Technical execution: 6-octave FBM with analytical derivatives; two-light rig (warm key + cool fill); specular on peaks; ink-black contour at grazing angles.
4. Liveness: Camera oscillates slowly; audio modulates wind/flow drift in FBM offset.
5. Differentiation: Sunny daylight vs volcanic darkness, mountain landscape vs lava field, Fauvist warm/cool opposition vs monochromatic heat.
**Changes:**
- Full rewrite: 3D terrain FBM with brush-stroke normals (analytical derivative FBM)
- 4-stop Fauvist palette: black → red-orange(2.1,0.4,0.1) → cyan-teal(0.1,1.8,2.0) → gold-white(2.5,2.2,0.5)
- Two-light rig: warm key(2.2,1.8,0.9) + cool fill(0.2,0.5,1.4)
- Ink-black contour: 1.0 - smoothstep(0.0, 0.25, dot(n, viewDir))
- Fauvist sky: warm-gold horizon → cool-cyan zenith
**HDR peaks reached:** peak specular 3.0+, gold peaks 2.5, teal mid 1.8
**Estimated rating:** 4.5★
