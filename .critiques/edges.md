## 2026-05-05 (v7)
**Prior rating:** 0.0★
**Approach:** 2D standalone generator — NEW ANGLE: neon rain noir city scene (vs. Molten Lava 2D, Neon Neural Network 3D, Neon Gyroscopes 3D, Neon Billiards 3D — all different)
**Critique:**
1. Reference fidelity: Film noir + neon rain aesthetic is visually cohesive and strongly differentiated from all prior approaches.
2. Compositional craft: Black building silhouettes provide strong focal silhouette; rain streaks create vertical rhythm; puddle reflections anchor the ground plane.
3. Technical execution: 128 rain capsule SDFs with distance-based AA; fwidth not applicable to discrete streaks but smoothstep width properly handles it.
4. Liveness: TIME-driven yPos cycling (fract wrap) gives continuous rain fall; shimmer in puddles oscillates.
5. Differentiation: No particles, no spheres, no plasma — city scene is a completely new visual domain.
**Changes:**
- Full standalone generator (no LED mode, no inputImage)
- 128 rain streaks with hash-determined positions and speeds
- 3-color neon palette: acid green, hot magenta, electric cyan
- Black building silhouette with dim amber window glow
- Wet-ground reflections with shimmer animation
- Audio modulates streak length and brightness
**HDR peaks reached:** streak core * neonGlow (2.5) = 2.5+; puddle reflections ~1.5
**Estimated rating:** 4.0★
