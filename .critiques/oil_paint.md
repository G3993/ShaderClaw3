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

## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: Prior v2 (Lava Impasto) used warm diagonal lava landscape; new angle is a close-up portrait sphere — completely different camera framing and metaphor.
2. Compositional craft: Fixed close-up camera on FBM sphere creates intimate portrait framing; Rembrandt tri-corner lighting produces dramatic chiaroscuro against dark umber canvas.
3. Technical execution: FBM-displaced sphere (4-octave noise); light direction slowly rotates at TIME*0.04; subsurface scatter approximation warms shadow side; fwidth() AA on brushstroke texture.
4. Liveness: impasto FBM animates at TIME*0.06; lightAngle rotates; audio modulates displacement amplitude.
5. Differentiation: Portrait close-up with Rembrandt single-source lighting is fresh — distinct from landscape lava, overhead ink, all prior angles.
**Changes:**
- Full rewrite as "Rembrandt Sphere" — 3D portrait with single-source Rembrandt lighting
- FBM-displaced sphere (impasto parameter 0–0.4, 4 octaves)
- Warm amber lit side (1.0, 0.52, 0.08) vs deep crimson shadow (0.22, 0.02, 0.03)
- Subsurface scatter approximation (back-lit warmth offset)
- HDR gold specular: 3.5× on highlight peak
- fwidth() AA on FBM impasto brushstroke texture edges
- Fresnel darkens portrait edges (silhouette vignette)
- Dark umber canvas background
- audioMod modulates impasto amplitude
**HDR peaks reached:** gold specular 3.5, amber diffuse 1.0×base, subsurface 0.14
**Estimated rating:** 4.5★
