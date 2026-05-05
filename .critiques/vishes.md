## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Neon Helix (3D double-helix DNA sculpture vs prior 2D cell-walker grid trails); completely different technique and aesthetic
**Critique:**
1. Reference fidelity: Cell-walker concept replaced with DNA helix — walker "beads" travel the helix strands in 3D space.
2. Compositional craft: Double helix is iconic and visually striking; traveling beads add motion; black void maximizes HDR neon contrast.
3. Technical execution: 48 capsule segments per helix, 16 traveling beads, fwidth() AA, Phong + rim, gold rungs.
4. Liveness: rotateY(TIME*0.15) full helix rotation; beads travel at beadSpeed; camera orbits; audio pulses bead size.
5. Differentiation: 3D spatial structure vs prior 2D grid canvas; rotating sculpture vs static trail accumulation.
**Changes:**
- Complete rewrite — single-pass 3D, no PASSES, no persistent canvas
- sdCapsule segments building double helix; gold rung capsules every 6 segments
- Traveling beads on both strands using fract(fi/N + TIME*speed)
- rotateY(TIME*0.15) full helix rotation before map()
- Palette: strand A magenta(2.5,0,2.0), strand B cyan(0,2.2,2.5), rungs gold(2.5,1.8,0)
- fwidth() AA on silhouette + per-bead edge
- Audio: bead radius *= 1.0 + audioLevel*audioPulse*0.2
- No state buffer, no persistent canvas, no walkers
**HDR peaks reached:** specular 3.0, strand surfaces 2.5, gold rungs 2.5
**Estimated rating:** 4.5★

## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 2D refine (HDR saturation + bloom boost)
**Critique:**
1. Reference fidelity: Cellular random walker trail system is a valid generative art concept.
2. Compositional craft: Single-pixel walker trails are too thin to accumulate visible color with default fadeRate.
3. Technical execution: Multi-pass state machine is correctly implemented.
4. Liveness: TIME-driven walker stepping; hueDrift creates color evolution.
5. Differentiation: Distinctive cell-stepping pattern; killed by desaturated colors (saturation param, no full-saturation HSV).
**Changes:**
- HSV saturation hardcoded to 1.0 (was user-settable, defaulted too low to read)
- hdrPeak input replaces brightness: default 2.5 — walkers paint at 2.5× HDR
- bloom default: 0.35 → 0.9 (makes trails visible across nearby cells)
- trailWidth parameter: walkers paint a 2-cell-wide swath (was 1 cell)
- backgroundColor default: black → deep navy [0,0,0.02]
- walkers default: 6 → 10
- stepRate default: 40 → 60 (faster trail accumulation)
- Audio: `audio = 1.0 + audioLevel * pulse + audioBass * pulse * 0.5`
- Black ink gap at low luminance edges via `inkEdge` smoothstep
- Bloom uses 5×5 kernel (unchanged, but larger radius relative to hdrPeak)
**HDR peaks reached:** walker cells at hdrPeak * audio = 2.5–3.5; bloom spreads to ~1.5 surrounding cells
**Estimated rating:** 3.8★
