
## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Double-helix tube raymarch vs prior 2D particle bounce system
**Critique:**
1. Reference fidelity: Original particle bounce concept stays but is discarded — the LED wall default killed all output. Helix tubes give a stronger focal element.
2. Compositional craft: Intertwined DNA-like helices with orbiting camera provide rich layered composition.
3. Technical execution: 80-step march on two sinusoidal helix SDFs. Volumetric glow pass gives tube aura even when ray misses.
4. Liveness: TIME-driven helix rotation, camera orbit, audio-reactive zoom. Two independent time axes.
5. Differentiation: Completely different primitive vocabulary (helix vs bounce), different lighting (neon glow vs LED wall), different reference (DNA/signal vs particle physics).
**Changes:**
- Full 3D rewrite as "Neon Helix" — double-helix SDF tubes
- Strand A: electric magenta; Strand B: cyan; specular: white-hot
- Volumetric glow pass (60-step nearest-miss glow) catches tube aura
- Orbiting camera (camAngle parameter)
- Black ink silhouette via fresnel grazing angle
- Audio: modulates camera distance (zoom in on beat)
- Background: deep violet-black with subtle neon radial wash
**HDR peaks reached:** tube surface 2.8×, specular 2.5+, volumetric glow 1.7+
**Estimated rating:** 4.2★
