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
**Approach:** 3D raymarch — NEW ANGLE: neon aerial city grid (top-down orbiting camera over infinite skyscraper towers) vs prior "three stacked RGB signal planes"; different primitive (box towers vs flat planes), different lighting (window glow vs scanline glitch), different color (cyan/orange/white vs red/green/blue)
**Critique:**
1. Reference fidelity: Complete standalone generator; no inputImage dependency; aerial city is a radically different visual from RGB signal planes.
2. Compositional craft: Void black towers with neon cyan windows create strong HDR contrast; orange corner edge glow adds warm accent; city extends to infinity via fog.
3. Technical execution: 64-step march with 0.85 step factor; normal via central differences (4-tap); window mask uses hash-based random on/off per cell + slow blink; fog blends dist-exponentially.
4. Liveness: Camera orbits on y-axis (rotSpeed) with slow drift (sin t×0.07); building heights vary by hash; window blink at individual frequencies.
5. Differentiation: Prior approach = 3 horizontal planes (R/G/B layers) with displacement glitch — this is a full 3D urban environment with dozens of box tower SDFs; completely different scene vocabulary, lighting model, and color identity.
**Changes:**
- Full rewrite replacing VIDVOX 8-frame buffer delay architecture
- SDF: hash-based infinite tower grid (box SDFs), 25% ground-floor gaps for streets
- Camera: high aerial, tilted, rotating + drifting
- Window system: hash21-gated windows + random blink timing
- Palette: void black, cyan [0,1,1] windows × winBrightness (2.0), orange [1,0.4,0] edges × 1.2, white HDR spec
- Deep night cyan fog [0,0.12,0.18]
- Audio: audioBass scales tower height + window brightness
**HDR peaks reached:** window cyan × 2.0 + audio = 2.5+; orange edge × 1.2 = 1.2; white spec 2.0+
**Estimated rating:** 4.5★
