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

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: 3D city skyline with neon-lit buildings vs prior three RGB data planes
**Critique:**
1. Reference fidelity: Cyber city interpretation of "time glitch" — data corruption expressed as visual urban decay. Different from RGB channel-split planes.
2. Compositional craft: Street-level camera looking up at neon-lit skyline. Strong vertical silhouette. Night atmosphere with wet reflections.
3. Technical execution: Tiled box SDF for buildings. Per-window hash for neon sign colors. 64-step march. Night sky gradient background. Audio pulses building heights via audioBass.
4. Liveness: Camera moves forward + lateral pan. Building heights pulse with audio. Window flicker driven by time hash. Neon signs flicker.
5. Differentiation: 3D box buildings vs flat XY planes; neon windows vs scanline bars; cityscape vs data visualization; street-level vs static camera.
**Changes:**
- Full rewrite: 3D SDF city grid with procedural neon windows
- 4-color neon palette: hot pink, electric cyan, toxic yellow-green, violet
- Per-building hash drives width, depth, height variation
- Per-window hash drives neon color + flicker state
- Wet asphalt ground with neon puddle reflection
- Street-level camera with lateral pan + forward motion
- Night sky gradient (deep indigo → black)
- Audio pulses building heights
**HDR peaks reached:** neonIntensity(3.0) on lit windows; building faces near-black (0.06)
**Estimated rating:** 4.0★
