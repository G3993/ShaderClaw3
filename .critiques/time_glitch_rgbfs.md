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

## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D Plasma Storm (domain-warped clouds + fork lightning)
**Critique:**
1. Reference fidelity: Complete rewrite — no usable reference; original required inputImage and produced nothing standalone.
2. Compositional craft: Double domain-warp FBM creates turbulent cloud masses with natural variation; fork lightning with branches gives strong vertical focal elements.
3. Technical execution: Single-pass procedural generator; boltDist uses 16-segment piecewise-linear main bolt + 8-segment branch; plasma uses two-level domain warp with aspect correction.
4. Liveness: TIME-driven plasma drift (speed param); lightning flashes at 3 Hz with hash-gated visibility; audioBass swells turbulence, audioMid boosts lightning appearance rate.
5. Differentiation: Void-black / electric violet / hot cyan / gold / white-hot palette is distinctive; lightning fork geometry gives kinetic vertical structure against churning plasma.
**Changes:**
- Full rewrite as "Plasma Storm" — single-pass procedural generator
- Double domain-warp FBM: q = fbm(p), r = fbm(p + turb*q), plasma = fbm(p + turb*r)
- Plasma palette: void black → electric violet (2.5×) → hot cyan (2.75×) → gold (3.125×) → white-hot (3.625×)
- Fork lightning: 16-segment main bolt (top→bottom) + 8-segment branch, hash-seeded piecewise-linear path
- Lightning glow: white-hot core 4.5×, violet inner 3.5×, cyan outer 2.0×
- Black ink crush: smoothstep(0.04, 0.14, plasma) ensures void-black gaps
- Audio: audioBass modulates turbulence warp intensity; audioMid raises lightning appearance rate
- lightningRate param controls flash duty cycle; boltCount controls simultaneous bolts
**HDR peaks reached:** plasma white-hot 3.625, lightning core 4.5, lightning violet 3.5, lightning cyan 2.0
**Estimated rating:** 4.5★
