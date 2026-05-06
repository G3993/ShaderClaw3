# vishes critique log

## Entry 1 — prior (cellular random walkers)
- **Technique**: 3-pass multi-pass ISF (16×1 stateBuf + persistent canvas + display); up to 16 walkers navigate a grid cell-by-cell, leaving hue-drifting colour trails on a fading canvas buffer
- **Lighting**: None — flat 2D colour trails, no volumetric depth
- **Composition**: Top-down 2D grid; walkers explore cells randomly; trails accumulate and fade; bloom via simple blur in display pass
- **Color grading**: HSV-rotated trails; saturation/brightness controls; background colour param; fully 2D
- **Reference**: Conway-like cellular automata, Physarum (slime mould) trail simulations
- **Weaknesses**: 3-pass persistent-buffer architecture is fragile; 2D only; no 3D depth; camera-less (fixed top-down); no HDR emissive; audio modulates speed only (gate-like)

## Entry 2 — v17 (3D Bioluminescent Tendrils)
- **Technique**: Single-pass SDF raymarcher (64 steps); 6 organic 3-segment capsule-chain tendrils smooth-union'd (smin k=0.10); volumetric glow accumulated during march; fwidth iso-rings on tendril cross-section
- **Lighting**: HDR emissive body ×hdrGlow×0.7; white-hot tip specular ×1.5; fresnel edge glow; AO (5-sample); volumetric glow halo for missed rays; floating spore sparkles in background
- **Composition**: Camera orbits the central tendril cluster with gentle elevation oscillation; tips animated with sin-wave per-tendril phase (breathing effect)
- **Color grading**: Cyan/Gold, Violet/Cyan, Green/White palettes; deep near-black ocean void; coreCol→midCol→tipCol gradient along tendril length; fully saturated
- **Differentiation axes**: dimensionality (2D grid→3D SDF volume), technique (cellular walker→raymarched capsules), lighting (flat trail colour→HDR emissive + volumetric aura), camera (none→orbital), audio (speed gate→emissive brightness modulation)
