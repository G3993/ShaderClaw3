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
**Approach:** 2D refine — NEW ANGLE: CRT Meltdown (2D dying monitor close-up) vs prior v1 3D RGB data planes with glitch geometry
**Critique:**
1. Reference fidelity: Prior v1 was a 3D raymarched scene of RGB data planes. v2 returns to 2D but as a completely different concept: the face of a dying CRT monitor — close-up composition vs open space.
2. Compositional craft: Hard horizontal glitch bars create strong ink-black separators. Phosphor burn-in oval anchors the center. RGB channel split creates visual tension. Three sweeping sync bars add motion.
3. Technical execution: All 2D, no input image; `crtSignal(uv)` generates 3-channel square-wave grid; 24 horizontal zones with 8fps quantized glitch selection; per-channel UV offset for RGB split; phosphor burn-in via exp falloff; fwidth() AA on sync bars and scanlines.
4. Liveness: TIME-driven sync bar sweep + glitch zone flipping at `glitchRate * 8fps`; audio modulates glitch frequency and RGB split amount.
5. Differentiation: 2D screen-space composition vs 3D spatial planes; close-up monitor face vs abstract space; hard ink separators (glitch bar boundaries) as strong compositional element; phosphor burn-in white HDR focal point.
**Changes:**
- Full rewrite: 3D RGB data plane march → 2D CRT monitor face
- 3-channel square-wave signal generator (crtSignal function)
- 24-zone horizontal glitch bars with 8fps quantized hash selection
- Per-channel (R/G/B) UV offset inside glitch zones
- Phosphor burn-in oval: exp(-dot(burnUV,burnUV)*0.25)*hdrPeak*2.5
- 3 sweeping sync-bar black stripes with fwidth() AA
- Hard ink separator borders at glitch bar boundaries
- fwidth() AA on scanlines and sync bars
**HDR peaks reached:** phosphor burn-in * hdrPeak * 2.5 = 5.0 (bloom catchpoint); RGB signals * 2.0; ink separators = 0.0 (black)
**Estimated rating:** 4.0★
