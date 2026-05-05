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
**Approach:** 3D raymarch — NEW ANGLE: Smooth metaball paint blobs vs prior displaced plane Lava Impasto
**Critique:**
1. Reference fidelity: Oil paint drops in suspension — painter's studio palette (cobalt, cadmium yellow, alizarin, viridian) vs prior lava/volcanic. Different conceptual reference (oil painting vs geology).
2. Compositional craft: Multiple floating blobs creating organic cluster vs single surface plane. Smooth merging via smin creates painterly transitions.
3. Technical execution: 14-blob smooth metaball SDF via smin(k=0.4). Nearest-blob color attribution. 64-step march. fwidth AA. Studio key+fill lighting.
4. Liveness: Per-blob independent sinusoidal drift orbits with hash-driven frequency. Audio modulates vertical amplitude (blob bounce).
5. Differentiation: Metaballs vs displaced plane; floating volume vs surface; cool primaries vs warm lava; multiple focal points vs single horizon; studio lighting vs ground reflections.
**Changes:**
- Full rewrite: 3D smooth metaball oil paint blobs
- 4+2 color palette: cobalt blue, cadmium yellow, alizarin crimson, viridian (fully saturated primaries)
- smin(k=0.4) for smooth metaball blending between blobs
- Per-blob hash-driven sinusoidal drift orbits
- Studio key (warm) + fill (cool) dual lighting
- fwidth() AA black ink silhouette
- Audio modulates blob vertical drift (audioMid)
**HDR peaks reached:** paintPal * 2.5 + spec 3.0 = ~3.2 at specular highlights
**Estimated rating:** 4.2★
