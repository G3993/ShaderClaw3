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
**Approach:** 2D standalone rewrite — NEW ANGLE: "Neon VHS Datamosh" — warm crimson/orange/gold palette, self-contained VHS band generator. vs. prior v1 (3D raymarched RGB data planes — Signal Interference).
**Critique:**
1. Reference fidelity: 9-pass 8-frame buffer delay abandoned entirely; now a self-contained VHS generator.
2. Compositional craft: Scrolling bands at random heights with glitch displacement; tear lines provide bright HDR accents against black void bands.
3. Technical execution: Band structure walks from y=1 down; each band gets color, glitch shift, phosphor burn gradient; scanline overlay; scattered static sparks.
4. Liveness: TIME-driven scroll; glitch seeds change on ~0.4Hz cycle; static at 12fps; audioMod modulates glitch and static intensity.
5. Differentiation: 2D vs 3D, warm vs RGB, generative vs effect pass, VHS aesthetic vs scan plane geometry.
**Changes:**
- Full rewrite: single-pass VHS band generator, no input required
- 4-color warm palette: black, crimson(2.2,0.15,0.1), orange(2.5,0.8,0.0), gold(2.0,1.8,0.0)
- Tear lines: HDR white-hot seams (3.5,3.0,2.5)
- Horizontal glitch displacement per band; micro-jitter overlay
- Phosphor burn gradient within each band; scanline overlay; static sparks
- CATEGORIES: ["Generator", "Glitch"]
**HDR peaks reached:** tear lines 3.5, neonBoost * band color 2.0–2.5, sparks 2.2
**Estimated rating:** 4.0★
