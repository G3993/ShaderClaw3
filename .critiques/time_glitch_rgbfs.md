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
**Approach:** 2D neon weave — NEW ANGLE: 3D RGB data planes geometry → 2D interwoven neon scanline strips
**Critique:**
1. Reference fidelity: 8-frame delay buffer replaced with interwoven neon strip tapestry — completely different visual.
2. Compositional craft: Horizontal + vertical strip weave creates textile-like pattern; over/under alternation adds woven depth illusion.
3. Technical execution: fwidth AA on all strip edges; per-strip hash glitch displacement; 6-color cycling; big glitch bursts.
4. Liveness: Color cycling (TIME*0.8); glitch displacement per-strip; big glitch step-function bursts; audio modulates brightness.
5. Differentiation: 2D textile weave vs prior 3D scanline geometry; 6-color neon vs R/G/B planes; flatness-as-feature vs spatial depth.
**Changes:**
- Full rewrite from 3D RGB data planes to 2D neon woven strip pattern
- Horizontal + vertical strip system with over/under weave
- 6-color neon palette: magenta, data green, electric blue, signal red, gold, cyan
- fwidth AA on all strip boundaries
- Per-strip hash glitch displacement + rare big glitch bursts
- Color cycling over time
- Audio modulates brightness
**HDR peaks reached:** strip center * 2.5 * centerBright = 2.5; with audio 3.5+
**Estimated rating:** 4.0★
