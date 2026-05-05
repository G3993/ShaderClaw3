## 2026-05-05
**Prior rating:** 0.1★
**Approach:** 3D raymarch (existing — retained)
**Lighting style:** studio
**Critique:**
1. Reference fidelity 3/5 — liquid-metal metaballs concept solid; no real-world artist reference grounding
2. Compositional craft 3/5 — multi-blob smin composition works; camera fixed, no orbit
3. Technical execution 2/5 — compiled fine but applied ACES internally (wrong for linear HDR host pipeline); hard clamp at output killed all bloom
4. Liveness 3/5 — TIME-driven morphing good; audio inputs absent, blobs deaf to beats
5. Differentiation 3/5 — metaball liquid-metal is distinctive but specular peaks were capped at 1.0

**Changes made:**
- Added `audioReact`, `camDist`, `exposure` inputs
- Audio modulator `aB = 0.5 + 0.5*audioBass*audioReact` pulses blob size with beats
- Removed internal ACES tone-mapping (host handles it)
- Removed `clamp(col, 0.0, 1.0)` — output is now linear HDR
- Top specular highlight lifted from 2.0 to `2.5 + aB*1.5` (peaks ~4 at audio peak → bloom)
- Camera uses `camDist` parameter

**Estimated rating after:** 3★
