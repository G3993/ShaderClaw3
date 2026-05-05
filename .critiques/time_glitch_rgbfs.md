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
