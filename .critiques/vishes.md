## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Deep-ocean bioluminescent jellyfish swarm (complete rewrite from cellular walker)
**Critique:**
1. Reference fidelity: Complete standalone 3D generator; multi-pass walker required feedback state and produced near-zero output without tuning.
2. Compositional craft: 5 independently drifting jellyfish at golden-angle offsets, volumetric mote particles, orbiting camera give strong spatial depth.
3. Technical execution: SDF: bell cap (squashed sphere + undercut), rim torus, 8 capsule tentacles; 64-step march; normal via 6-sample finite differences.
4. Liveness: TIME-driven drift (3-axis sinusoids per creature), tentacle wag, orbiting camera; audio-reactive bell scale via audioBass.
5. Differentiation: Bioluminescent ink-silhouette approach in deep ocean context is genuinely alien — nothing like the prior cellular walker.
**Changes:**
- Complete 3D raymarch rewrite (sdBell + sdRim + sdCap tentacles × 8)
- 5-color fully saturated palette: electric cyan, hot magenta, chartreuse, violet, orange
- Ink-edge darkening via silhouette face dot product
- Volumetric micro-organism motes (18 particles in view space)
- Audio: bell scale pulsed by audioBass; tentacle wag speed by audioMid
- HDR: rim × glowAmt × 1.3 = 2.86+, spec × glowAmt = 2.2, diff × glowAmt
**HDR peaks reached:** rim 2.86, specular 2.2, mote accumulation ~1.5
**Estimated rating:** 4.0★
