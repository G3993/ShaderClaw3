# vapor_hologram

## v1 — 2026-05-06
**Original angle:** Vaporwave Hologram — warm Y2K vaporwave (pink sky, orange sun, magenta grid) transmitted through holographic glitch.
**Issues:** Warm pink/orange/magenta palette covered well by other shaders; `holo *= 0.5 + audioLevel * 0.6` bug dimmed the hologram to 50% brightness when no audio was connected; Y2K shape hue was unbounded (all colors), diluting the palette focus.

## v2 — 2026-05-06
**New angle:** Cryogenic Hologram — cold deep-space ice scene with holographic glitch.
**Changes (targeted palette swap + audio fix):**
- `skyTopColor` default: `[1.0,0.42,0.71]` (pink) → `[0.04,0.06,0.32]` (deep indigo night sky)
- `skyHorizonColor` default: `[0.36,0.85,0.76]` (cyan-teal) → `[0.42,0.82,0.98]` (ice horizon blue)
- `holoTint` default: `[0.55,1.0,0.95]` (warm cyan-green) → `[0.38,0.90,1.0]` (cold deep cyan)
- Sun color: `mix(orange, hot-pink, ty)` → `mix(vec3(0.55,0.82,1.0), vec3(0.90,0.97,1.0), ty)` — arctic ice star: cold blue to white-hot HDR
- Grid floor base: warm purple-dark `mix([0.10,0.05,0.18],[0.55,0.10,0.45])` → deep ocean blue `mix([0.01,0.03,0.18],[0.02,0.12,0.42])`
- Grid lines: magenta `[1.0,0.42,0.85]` → electric cyan `[0.0,0.88,1.0]`
- Y2K shape hue: free-roaming rainbow `fract(h2 + TIME*0.05)` → ice-locked `fract(0.50 + h2*0.32 + TIME*0.03)` — shapes cycle only through blue/cyan/violet (hue 0.50–0.82)
- Katakana: warm green-white `[0.7,1.0,0.85]` → ice blue `[0.55,0.92,1.0]`
- **Audio bug fix**: `holo *= 0.5 + audioLevel * 0.6` → `holo *= 0.9 + audioLevel * audioReact * 0.15` — hologram no longer dims to 50% when silent; audio now cleanly boosts brightness without gating
