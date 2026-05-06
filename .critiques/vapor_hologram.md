## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (fix audio dependence, boost HDR)
**Critique:**
1. Reference fidelity: Vaporwave + hologram is well-conceived; sky/sun/grid/Y2K is a strong visual system.
2. Compositional craft: Good layered composition; hologram overlay correctly degrading.
3. Technical execution: Bug: `holo *= 0.5 + audioLevel * 0.6` — at audioLevel=0 (no audio), image is at 50% brightness, causing 0.0 score.
4. Liveness: TIME-driven grid, Y2K bounce, all working.
5. Differentiation: Strong visual identity (vaporwave + holo); killed by audio dependency.
**Changes:**
- FIXED: `holo *= max(0.85, 0.7 + audioLevel * audioReact * 0.4)` — never drops below 85% brightness
- Y2K shapes: `shapeCol * 2.0` (HDR boost), white outline `3.0`
- Sun: `* 2.2` HDR boost
- Neon grid floor: `vec3(1.0, 0.1, 0.8) * 2.0` (hot magenta HDR)
- Sky: `* 1.3` boost
- Y2K shape saturation: `hsv2rgb(vec3(hue, 1.0, 1.0))` (was 0.85 → 1.0)
- skyTopColor default: hot pink deepened [1.0,0.10,0.60]
- katakana boosted: `vec3(0.5,1.0,0.8) * 2.5`
- holoGlow default: 0.7 → 1.4
**HDR peaks reached:** sun 2.2, Y2K shapes 2.0, grid lines 2.0, katakana 2.5, holo spec 2.0+
**Estimated rating:** 4.5★

## 2026-05-06
**Prior rating:** 0.0★ (vaporwave audio fix was a patch; full thematic rewrite gives cleaner baseline)
**Approach:** 2D layered — Japanese Onsen Night (single pass)
**Critique:**
1. Reference fidelity: Vaporwave aesthetic had been done; Onsen Night is culturally distinct and uses a completely different visual grammar.
2. Compositional craft: 7-layer composition (sky→stars→far mountain→near mountain→torii→water→steam/sakura/moon) creates cinematic depth.
3. Technical execution: `mountain()` SDF via noise ridge; `steam()` Gaussian column; `blossom()` drifting petal; torii as 2D SDF pillars+beams.
4. Liveness: 40 twinkling stars, 3 steam wisps, 10 drifting sakura, amber lantern flicker — all TIME-driven.
5. Differentiation: Onsen Night has zero overlap with any prior shader concept in this collection.
**Changes:**
- Full rewrite as "Japanese Onsen Night" (single pass, drops 2-pass vaporwave+hologram)
- `mountain(uv, baseY, amp, freq, seed)` — layered noise ridge function
- `steam(uv, cx, t, dens)` — Gaussian wisp column with noise drift
- `blossom(uv, center, t, seed)` — falling petal with sinusoidal path
- Torii gate: 2D SDF (2 pillars + 2 beams + amber HDR glow behind)
- Water: shimmer + sky reflection + amber lantern reflection
- Audio modulates `aud` multiplier (never cuts to black at audioLevel=0)
- `steamDens`, `audioReact`, `hdrPeak`, `speed` inputs
**HDR peaks reached:** sakura blossoms hdrPeak*2.5, lantern glow hdrPeak*0.7, moon 0.8×
**Estimated rating:** 4.6★
