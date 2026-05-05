## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (starfield background + HDR depth glow)
**Critique:**
1. Reference fidelity: Perspective tunnel rows with zoom-by-distance is a genuine 3D-feeling effect; invisible transparent.
2. Compositional craft: Depth-scaling rows create parallax; no background means no spatial anchoring.
3. Technical execution: Zoom-by-distance calculation is correct; size-ratio creates strong parallax.
4. Liveness: TIME-driven row scroll with mod() wrap works.
5. Differentiation: Depth-perspective text is unique; needs space context.
**Changes:**
- Added starfieldBg() — 3-layer procedural starfield with nebula color wash
- Star twinkling via sin(TIME * freq + seed)
- Nebula: 4-color (violet, cyan, gold, magenta) sinusoidal wash
- transparentBg default: true→false
- textColor: white (kept), bgColor: deep space navy [0,0,0.02]
- hdrGlow default: 2.0 with depth-based brightness (far rows dimmer)
- starDensity parameter
- Alternating rows: white vs cyan for depth differentiation
- audioMod input added
**HDR peaks reached:** close rows textColor * 2.0 = 2.0, with audio 2.8+
**Estimated rating:** 3.8★

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D bioluminescent ocean background — NEW ANGLE: deep ocean with glowing bio creatures; v1 was cold starfield (space), v15 was geometric crystal lattice. Organic, warm teal/cyan, completely different reference world (ocean vs cosmos vs geometry).
**Critique:**
1. Reference fidelity: Glowing marine bioluminescence blobs create organic alien environment that pairs well with perspective text rows.
2. Compositional craft: 12 independent pulsing creatures fill the dark void; caustic shimmer adds surface texture.
3. Technical execution: Per-creature pulse phase offset (2.094 = 120° triplets) avoids synchronization; exp falloff creates soft glow.
4. Liveness: Independent pulse rates, slow drift orbits, caustic animation.
5. Differentiation: Warm teal/violet ocean is opposite of cold space (v1) and geometric crystal (v15); organic vs mechanical.
**Changes:**
- Added bioAbyssBg(): 12 bioluminescent blob creatures with pulsing glow
- HDR core: 2.5 per creature at peak pulse
- Caustic shimmer via layered noise
- textColor: white → bioluminescent cyan [0,1,0.85]
- bgColor: black → deep ocean [0,0.01,0.08]
- transparentBg default: true → false
- hdrGlow 2.0, audioMod 0.5
**HDR peaks reached:** creature cores 2.5 at pulse peak; text * 2.0 * audio ≈ 2.5
**Estimated rating:** 4.4★
