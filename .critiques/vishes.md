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

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: Bioluminescent metaball organism vs prior 2D cellular walker grid
**Critique:**
1. Reference fidelity: "Vishes" (fish? creatures?) now expressed as actual 3D bioluminescent organism. Cellular concept retained but in 3D metaball form vs discrete 2D grid cells.
2. Compositional craft: Single organism filling frame with orbiting camera. Strong focal point, depth. Lobes create rhythmic pulsing composition vs prior scattered 2D trails.
3. Technical execution: smin(k=0.35) smooth metaball: core sphere + N lobe spheres. Hash-driven lobe frequency/size/phase. 64-step march. Deep ocean particle background. fwidth AA.
4. Liveness: Lobe orbits pulse via sin(t*pulseRate + fi). Lobes breathe with audio. Orbiting camera. Background particle suspension twinkle.
5. Differentiation: 3D metaballs vs 2D grid cells; orbiting camera vs fixed view; bioluminescent blue/green vs rainbow HSV; pulsing organism vs walking trails; smooth merging vs discrete painting.
**Changes:**
- Full rewrite: 3D bioluminescent metaball organism
- 4-color deep ocean palette: electric blue, cyan-teal, phosphor green, violet
- Core body + N radial lobes via smin smooth merging
- Per-lobe hash-driven frequency, size, phase
- Subsurface scattering approximation (inner glow)
- Deep ocean void background with particle suspension
- Orbiting camera with vertical bob
- Audio modulates lobe pulse amplitude
**HDR peaks reached:** bioPal * 2.8 + spec 3.0 = ~3.2 at specular; sss adds ~0.3 inner glow
**Estimated rating:** 4.2★
