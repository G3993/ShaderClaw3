## 2026-05-06 (v4)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Volcanic Caldera (prior 2026-05-06 was Arctic Shard — COLD ice crystals, navy+cyan+white palette)
**Critique:**
1. Reference fidelity: Volcanic eruption is the direct palette-opposite of arctic ice — fire vs ice, the maximum axis contrast.
2. Compositional craft: Camera inside crater looking at ring of geysers creates dramatic enclosure; central geyser creates focal anchor.
3. Technical execution: sdCapsule geysers animated by sin(TIME + phase); floor lava cracks via sin products; 64-step SDF march.
4. Liveness: Geysers erupt at different phases (hash-seeded); camera slowly orbits crater; audio boosts geyser height.
5. Differentiation: COLD→HOT palette inversion; stretched octahedra→capsule geysers; crystal ring→eruption ring; exterior view→interior crater view.
**Changes:**
- Full rewrite from Arctic Shard (cold, ice) to Volcanic Caldera (hot, lava eruption)
- N animated sdCapsule geysers in a ring + 1 central large geyser
- Per-geyser tip-fraction color: crimson 1.6 → orange 2.5 → gold 3.0 → white-hot 4.0
- Floor lava cracks: sin product pattern with HDR orange emission
- Camera orbits inside crater at user-adjustable height
- Audio modulates all geyser heights simultaneously
**HDR peaks reached:** geyser tips 4.0; gold zone 3.0; orange zone 2.5; floor cracks ~2.0
**Estimated rating:** 4.5★
