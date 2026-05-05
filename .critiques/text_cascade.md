## 2026-05-05 (v5)
**Prior rating:** 0.0★
**Approach:** 2D background generator — NEW ANGLE: solar chromosphere granulation (vs. Ink Rain amber, Deep Sea Bioluminescence, Bioluminescent cave 3D — all ocean/biological)
**Critique:**
1. Reference fidelity: Solar granulation is a real astrophysical phenomenon — boiling convection cells on the sun's surface — visually striking and scientifically grounded.
2. Compositional craft: Voronoi granulation provides strong texture; bright centers against dark boundaries creates natural contrast.
3. Technical execution: 3×3 Voronoi neighborhood search; animated seed positions; HDR peaks on granule centers.
4. Liveness: Slow TIME drift of granule seeds simulates convection motion.
5. Differentiation: No ocean, no cave, no rain — stellar astrophysics is a completely new domain.
**Changes:**
- Added solarGranulation() Voronoi-based background
- Solar palette: gold centers (2.0 HDR), dark orange boundaries, white-hot plasma flares (1.5 HDR)
- transparentBg default: true→false
- textColor solar white (2.5 HDR)
- bgColor dark solar
**HDR peaks reached:** plasma flares 1.5+, text 2.5, granule centers 2.0
**Estimated rating:** 4.0★
