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
**Approach:** 2D domain-warp FBM — NEW ANGLE: 2D fluid-swirl expressionist field vs prior 3D lava-impasto raymarched surface.
**Critique:**
1. Reference fidelity: Original was Kuwahara image-filter (required inputImage). This is a standalone abstract generator with paint aesthetic.
2. Compositional craft: Domain-warped FBM creates complex swirling forms with no dominant focal point — full-field expressionist style.
3. Technical execution: Double domain warp (2 rounds of FBM), ink edges at two scales via fwidth, time-animated colour cycling.
4. Liveness: TIME-driven flow + audioMid/audioBass modulating brightness.
5. Differentiation: 2D fluid swirl vs 3D terrain; expressionist paint colours (cadmium red/cobalt/viridian/yellow) vs lava reds; domain-warp vs ray displacement.
**Changes:**
- Full rewrite: standalone 2D abstract expressionist generator
- Double domain-warp FBM for chaotic turbulence
- 4-colour paint palette: cadmium red, cobalt blue, viridian, chrome yellow (all HDR)
- Ink edges at 2 scales (wide brushstroke + fine detail) via fwidth
- TIME-driven colour evolution (colIdx + TIME)
- Audio modulates brightness
**HDR peaks reached:** palette peaks 2.2× (cadmium/viridian), up to 2.5 with audio
**Estimated rating:** 4.0★
