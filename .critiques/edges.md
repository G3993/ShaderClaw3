## 2026-05-05 (v3)
**Prior rating:** 0★
**Approach:** 3D raymarch — NEW ANGLE: Neon Neural Network (vs v1 capsule-particle fix / v2 Molten Lava FBM)
**Critique:**
1. Reference fidelity: Neural network graph is a completely distinct visual concept — nodes as glowing spheres, edges as magenta tubes.
2. Compositional craft: Ring topology + skip-one connections create visual complexity; orbiting camera reveals 3D depth.
3. Technical execution: 64-step raymarch; capsule SDF for tubes; fwidth not needed here (no iso-edges, SDF smoothstep handles AA).
4. Liveness: Camera orbits, pulse wave travels along edges via sin(t*pulseRate - position), nodes activate with TIME.
5. Differentiation: Cold cyan/magenta palette (vs lava = hot) and graph topology (vs FBM organic = no topology).
**Changes:**
- Full rewrite: 3D raymarched neural net with sphere nodes + capsule-SDF edge tubes
- Palette: CYAN_NODE, MAGENTA_EDGE, GOLD_BURST, VIOLET_RIM — all fully saturated HDR
- 64-step march, orbit camera (sin/cos TIME * cameraSpeed)
- Pulse wave: sin(t*pulseRate - dot(p, dir)) travels along tubes
- Node activation glow: unoccluded halo via ray-node projection
- Audio modulates hdrBoost multiplicatively
**HDR peaks reached:** node specular white 2.0+, node core 2.5, edge pulse 2.5, glow halo soft ~1.5
**Estimated rating:** 4.5★
