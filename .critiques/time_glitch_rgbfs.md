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
**Approach:** 3D raymarch — NEW ANGLE: Fractured Prism (octahedra cluster, magenta/lime/gold vs prior Signal Interference RGB planes); different 3D primitive vocabulary; different color grading
**Critique:**
1. Reference fidelity: VIDVOX buffer delay replaced; "Fractured Prism" captures the glitch/broken-signal spirit with shattered geometry.
2. Compositional craft: 6 floating octahedra in ring pattern, each spinning independently; black void maximizes HDR contrast.
3. Technical execution: sdOctahedron, per-shard rotX+rotY, Phong key+spec, silhouette darkening, fwidth() iso-edge AA.
4. Liveness: Each shard spins at different speed (hash11 speed variation); camera orbits; audio scales shard size.
5. Differentiation: Magenta/lime/gold palette vs prior red/green/blue; octahedra vs planes; spinning geometry vs sweeping camera.
**Changes:**
- sdOctahedron((p+y+z-s)*0.577) for each shard
- Per-shard rotX(rot*0.7)*rotY(rot) per TIME*spinSpeed*hash
- Material: id%3: 0→magenta(2.5,0,2.0), 1→lime(0.5,2.5,0), 2→gold(2.5,1.8,0)
- Silhouette darkening: 1-dot(n,-rd)^2 * 0.6
- fwidth() AA on silhouette iso-edge
- No inputImage, no PASSES, no buffer delay
**HDR peaks reached:** specular white 3.0, shard surfaces 2.5
**Estimated rating:** 4.0★

## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** 3D Wireframe Cyberspace — NEW ANGLE: 3D signal planes (v1) → 3D octahedra prisms (v2) → 3D first-person wireframe terrain flyover (v4)
**Critique:**
1. Reference fidelity: "Time glitch" digital aesthetic reinterpreted as classic 90s cyberspace wireframe — the original visual metaphor for "digital space" in computing culture.
2. Compositional craft: Undulating green terrain grid recedes to horizon; height-mapped color (dim→hot green) gives topographic readability; W_NODE intersection dots create visual rhythm.
3. Technical execution: Simple height-field march (pos.y < terrainH); fract-based X/Z wireframe cells; fog attenuation; horizon glow for depth.
4. Liveness: Camera flies forward (TIME*flySpeed); terrain waves animate (waveAmt*sin); audio modulates brightness.
5. Differentiation: 3D terrain flyover vs v2 orbiting octahedra; nostalgic 90s wireframe vs geometric prism; green monochrome vs magenta/lime/gold.
**Changes:**
- Full rewrite from 3D octahedra ring to 3D wireframe terrain
- terrainH() = 3-frequency sin sum for organic undulation
- Height-field march: advance until pos.y < terrain
- fract-based wireframe grid (cellSz=0.5)
- Height-mapped color: G_DIM (low) → G_HOT (high terrain)
- W_NODE bright dots at grid intersections
- Horizon atmospheric glow (G_DIM fade)
- Audio modulates hdrBoost
**HDR peaks reached:** G_HOT 2.8, W_NODE 3.0, G_MID 1.6 — at hdrBoost*audio
**Estimated rating:** 4.5★
