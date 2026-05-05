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
**Approach:** 3D raymarch — NEW ANGLE: prior lava impasto (warm/cinematic) → Van Gogh impasto terrain (cool blue/yellow, painterly style)
**Critique:**
1. Reference fidelity: Prior used lava + cinematic lighting; this uses domain-warped FBM terrain with painterly swirling brushwork referencing Starry Night.
2. Compositional craft: Moving camera drifts over terrain showing wide environmental perspective with visible brush-ridge silhouettes.
3. Technical execution: Domain-warped FBM (3 levels of warp), 64-step march on displaced plane, fwidth() on terrain edges.
4. Liveness: TIME-driven domain warp flow, camera drift; audio modulates terrain height.
5. Differentiation: Cool blue/yellow palette (Prussian blue, cadmium yellow, viridian) vs prior warm orange/gold; painterly sun lighting vs cinematic; wide terrain vs close-up surface.
**Changes:**
- Domain-warped FBM (q → r → final) for Van Gogh swirl effect
- 4-color palette: Prussian blue, cadmium yellow, viridian, white-hot
- Height-based color blend: blue lowlands → viridian midslopes → yellow ridges → white peaks
- Painterly sun lighting (upper-right) with subsurface scatter simulation
- HDR ridge gilding: yellow * diff * diff * hdrBoost
- Sky gradient: deep Prussian blue
- Moving camera drifting over terrain
- Black ink crevice via fwidth AA
**HDR peaks reached:** sunlit ridges white-hot 2.3+, yellow HDR gilding 2.0, sky ambient 0.1
**Estimated rating:** 4.0★
