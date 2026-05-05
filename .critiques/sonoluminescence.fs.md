## 2026-05-05
**Prior rating:** 0.3★
**Approach:** 3D raymarch
**Lighting style:** cinematic
**Critique:**
1. Reference fidelity 2/5 — glass vessel + bubble concept is right but 2D flat rendering reads as diagram, not phenomenon
2. Compositional craft 2/5 — concentric rings, bubble glow, glass walls all present but lack depth/perspective
3. Technical execution 2/5 — sdRoundedRect used for 2D glass outline; no specular; contour AA missing
4. Liveness 3/5 — TIME-driven ripples and audio pulse work; bubble oscillation subtle
5. Differentiation 1/5 — looks like every other "glow circle in water" shader; no 3D form
**Changes made:**
- Complete 3D rewrite: orbiting camera raymarchs a cylinder vessel + analytic bubble sphere
- sdCylinder for glass beaker shell (outer - inner = glass wall SDF)
- Analytic sphere intersection for bubble (skip SDF overhead on smallest object)
- Water surface: ray-plane at waterY with numeric ripple displacement normal
- fwidth AA on concentric ripple iso-rings on water surface
- Bubble glow upwelling through water surface with exp() falloff
- Volumetric bubble halo integrated along camera ray
- Caustic shimmer via vnoise on underwater surface
- Fresnel on glass + water; Blinn-Phong spec 128-power HDR peaks
- HDR plasma core: glowCol × 2.2 + audioPulse × 2.5 (bloom-ready)
- No tonemapping/clamp — linear HDR output
**Estimated rating after:** 4★
