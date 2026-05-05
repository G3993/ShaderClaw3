## 2026-05-05 (v5)
**Prior rating:** 0.0★
**Approach:** 2D background generator — NEW ANGLE: volcanic pyroclastic lava (vs. Starfield, Desert Dust Storm, Arctic Ice Cave, Warp Singularity — all different environments)
**Critique:**
1. Reference fidelity: Pyroclastic volcanic flows have a strong visual language — deep black, crimson, flame orange, gold, white-hot — perfectly saturated.
2. Compositional craft: Domain-warped FBM lava gives flowing organic texture; ember particles add vertical motion.
3. Technical execution: Double FBM domain warp for lava; 32 rising ember particles; 5-zone color ramp.
4. Liveness: TIME-driven lava flow and ember rise creates continuous motion.
5. Differentiation: No space, no desert, no ice — geological fire is a totally new domain.
**Changes:**
- Added volcanicBg() domain-warp FBM lava background
- 5-color volcanic ramp: near-black → crimson → flame orange → gold → white-hot HDR
- Rising ember particles (32 count)
- transparentBg default: true→false
- textColor flame orange (2.5 HDR)
**HDR peaks reached:** white-hot lava zones 1.5+, text overlay 2.5, ember particles 2.0
**Estimated rating:** 4.0★
