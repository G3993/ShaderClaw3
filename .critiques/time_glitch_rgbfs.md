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

## 2026-05-06
**Prior rating:** 0.0★
**Approach:** 3D wireframe (perspective projection)
**Critique:**
1. Reference fidelity: Prior v2 (Signal Interference) used 3D RGB data planes; new angle uses projected box-frame corridor — completely different geometry and visual metaphor.
2. Compositional craft: Infinite corridor of 14 nested wireframe frames creates strong depth illusion; depth-based cyan→violet color fade reinforces recession; pure black bg maximizes contrast.
3. Technical execution: Box frames perspective-projected from Z-corridor; fwidth() AA applied to all 4 edge distances (edgeH, edgeV) per frame; exponential depth brightness decay; receding grid overlay.
4. Liveness: Camera fly-through via mod(t*flySpeed, 1.8) continuous loop; audio widens wireWidth; frame spacing creates beat-sync potential.
5. Differentiation: Wireframe infinite room corridor is completely new — no prior version used projected box-frame geometry, wireframe aesthetic, or corridor infinite-loop.
**Changes:**
- Full rewrite as "Recursive Wireframe Room" — 14 perspective-projected box frames
- fwidth() AA on all 4 edge distances per frame (edgeH, edgeV)
- Color ramp: HDR cyan (0.0, 2.8, 3.2) near → HDR violet (2.2, 0.0, 2.8) far
- Exponential depth fade exp(-depth*2.2)
- Subtle receding background grid with fwidth() AA
- Pure black background for maximum wire contrast
- audioMod modulates wireWidth
**HDR peaks reached:** cyan near-frame 3.2, violet far-frame 2.8; grid accent 0.25
**Estimated rating:** 4.5★
