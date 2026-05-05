## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch — NEW ANGLE: standalone plasma globe generator replacing useless image-filter
**Critique:**
1. Reference fidelity: Original was a pure image tint filter (needed inputImage); rewritten as standalone generator with strong visual identity.
2. Compositional craft: Orbiting camera reveals sphere from all angles; dual-band FBM arc ridges create dramatic lightning-strike patterns across the surface.
3. Technical execution: Analytical sphere intersection (no march needed); dual-frequency FBM arcs (a1 + a2*0.5); primary + secondary ridge layers; fwidth() AA on both arc edge distances; violet rim glow.
4. Liveness: TIME-driven camera orbit + arc animation; audio modulates arc density (bass) and ridge brightness (level).
5. Differentiation: Plasma globe concept unique in catalog; 5-hue neon cycle (magenta→cyan→gold→violet→orange) fully saturated with no white dilution.
**Changes:**
- Removed inputImage dependency; full standalone generator
- 3D analytical sphere intersection instead of UVs
- Dual-band FBM arc ridges with primary + secondary frequency layers
- 5-hue neon palette cycling with arcVal + TIME offset
- fwidth() AA on both edge distances (primary glowWidth, secondary 0.35×)
- Violet rim glow pow(1-NdotV, 2) × 1.5
- Soft halo + tangent spark ring for rays that miss sphere
**HDR peaks reached:** ridge * hdrPeak * audioBoost = 2.5–3.5 at arc cores; rim glow 1.5; secondary ridges 1.25; spark ring hdrPeak
**Estimated rating:** 4.0★
