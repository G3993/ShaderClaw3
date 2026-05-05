## 2026-05-05 (v3)
**Prior rating:** 0★
**Approach:** 2D refine — NEW ANGLE: Arctic Ice Cave (vs v1 audio-react HDR boost / v2 Desert Dust Storm)
**Critique:**
1. Reference fidelity: Clear ice cave aesthetic: abyss azure background, stalactite/stalagmite silhouettes, ice sparkle highlights, slow water drip animation.
2. Compositional craft: Depth-faded perspective text (close rows bright, far rows dim) reinforces the cave depth illusion; cold palette creates strong cold identity.
3. Technical execution: arcticCaveBg() generates stalactites via sin(), sparkle via hash grid, drip via fract wrapping; depth-fade applied to text rows.
4. Liveness: Ice sparkle changes per TIME floor(); drips flow downward via fract(uv.y - t*0.08); background cave static but sparkle/drip animate.
5. Differentiation: Cold icy cave (vs v2 hot dusty desert, vs v1 void space). Geological/natural vs. cosmic aesthetic.
**Changes:**
- Added arcticCaveBg(): azure cave gradient + stalactites/stalagmites + sparkle + drip
- textColor default: white→ice azure [0.5,0.95,1.0]
- bgColor default: black→cave [0,0.02,0.08]
- transparentBg default: true→false
- hdrGlow param added (default 2.2); near rows hdrGlow×aud, far rows dimmed to 25%
- audioReact param added
- White-hot specular tint on close text (glacier reflection)
**HDR peaks reached:** close-row text 2.2×aud = 2.2–3.0; ice sparkle 1.5; drip glow 0.6 ambient
**Estimated rating:** 4.0★
