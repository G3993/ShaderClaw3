## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: VIDVOX 8-frame buffer delay — requires inputImage; produces nothing standalone.
2. Compositional craft: Frame buffering is purely an effect; no content without source.
3. Technical execution: 9-pass persistent buffer architecture is complex and correct, but all passes output noise without input.
4. Liveness: TIME-driven via random delay shift, but input-dependent.
5. Differentiation: Interesting channel-split temporal effect; not a generator.
**Changes:**
- Full rewrite as "Signal Interference" — raymarched 3D RGB data planes with glitch geometry
- Three independently marched color planes (R/G/B) at Y-offsets (planeOffset parameter)
- Each plane: scanlines + column bars + glitch blocks as SDF geometry
- Per-channel glitch: horizontal displacement driven by hash(floor(y * 8 + t * rate))
- HDR: signal red, data green, electric blue — fully saturated
- White-hot specular peak on hit surfaces
- Camera slowly sweeps through the planes (sin(t * 0.13))
- hdrBoost parameter (default 2.0)
- audioMod modulates displacement and brightness
**HDR peaks reached:** per-channel hdrBoost * diffuse = 2.0; white spec adds ~2.5
**Estimated rating:** 4.0★

## 2026-05-05 (v8)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: industrial jet turbine (vs v7 top-down circuit board city)
**Critique:**
1. Reference fidelity: Jet turbine cross-section is a precise mechanical reference — spinning blade array, outer casing ring, central hub, white-hot core. Directly references aerospace turbomachinery.
2. Compositional craft: Strong radial composition — blades radiate from center hub; POV from directly down the Z-axis gives classic turbine cross-section view. Central heat bloom provides focal anchor.
3. Technical execution: Per-blade SDF via rotation in loop (up to 24 blades); hub/core/casing material IDs; 64-step march; heat bloom added as 2D additive overlay.
4. Liveness: Spin driven by TIME×spinSpeed; audio (audioBass) accelerates spin and brightens core glow.
5. Differentiation: Industrial mechanical vs all prior approaches (circuit board v7, crystal lattice v6, neon mandala v5, VHS v3, signal planes v1). 4-color palette: titanium steel, orange heat, white-hot HDR — no desaturation.
**Changes:**
- Full rewrite as 3D jet turbine cross-section
- sdBlade() function: N thin box SDFs rotated by spin angle
- sdCyl() for outer ring, hub, and white-hot core
- 4 material IDs: ring steel, blade steel, hub orange, core white-hot
- 2D volumetric heat bloom overlay (core color additive)
- Audio modulates both spin speed and core brightness
**HDR peaks reached:** white-hot core 2.5×hdrPeak×audio, heat bloom 0.9×hdrPeak, specular 1.8×
**Estimated rating:** 4.2★
