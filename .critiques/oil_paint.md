## 2026-05-05
**Prior rating:** 0.0★
**Approach:** 3D raymarch
**Critique:**
1. Reference fidelity: Original Kuwahara filter requires inputImage — nothing to paint without source. Clever technique, wrong category.
2. Compositional craft: Zero standalone composition; effect pass without a generator pass.
3. Technical execution: Multi-pass Kuwahara is correctly implemented but useless as a standalone generator.
4. Liveness: No TIME-driven content; the painterly effect is static relative to input.
5. Differentiation: Kuwahara approach is elegant but requires input.
**Changes:**
- Full rewrite as "Lava Impasto" — standalone 3D molten rock surface
- Domain-warped FBM height field as raymarched displaced plane (64-step)
- Lava palette: black → deep crimson → orange → gold → white-hot (HDR)
- Time-driven flow using animated domain warp (flowSpeed parameter)
- Hot-spot pulse with TIME * 3.1 for liveness
- Charred crevice edge darkening via fwidth(rawH) AA
- Cinematic camera angled down onto surface, drifting slowly
- Audio modulates pulse intensity
**HDR peaks reached:** white-hot crack edges 3.0, gold flow 1.5–2.5, orange mid-tone 1.0
**Estimated rating:** 4.5★

## 2026-05-05 (v2)
**Prior rating:** 0.0★
**Approach:** 2D painterly — NEW ANGLE: 3D Lava Impasto (raymarch displaced plane) → 2D Fauve Expressionism (procedural brush strokes)
**Critique:**
1. Reference fidelity: Kuwahara oil-paint filter replaced with standalone 2D impasto generator — Fauvist thick stroke aesthetic.
2. Compositional craft: Direction field from double-domain-warp FBM creates organic brush paths; ridge profile creates tactile impasto depth.
3. Technical execution: Finite-difference normals from height field; warm key light; black ink troughs at stroke valleys.
4. Liveness: Animated domain warp (0.07 TIME) slowly evolves the stroke direction; audio modulates brightness.
5. Differentiation: 2D procedural brush vs prior 3D lava surface; Fauvist palette (cobalt/vermilion/cadmium/viridian) vs fire/lava palette; composition is colorist rather than volcanic.
**Changes:**
- Full rewrite from 3D lava raymarcher to 2D impasto brush system
- FBM double-warp direction field drives stroke angle
- Fauvist 4-hue palette: cobalt blue, viridian green, vermilion red, cadmium yellow
- Ridge profile = sin²(stripe) sharped impasto height
- Finite-difference normal → warm key light + white-hot specular ridge
- Black ink shadows in stroke troughs
- TIME-animated warp evolution
**HDR peaks reached:** specular ridge 2.2*0.8 = ~1.76; with audio boost to ~2.5
**Estimated rating:** 4.0★

## 2026-05-05 (v4)
**Prior rating:** 0.0★
**Approach:** 3D Metaball Paint Globs — NEW ANGLE: 3D lava plane (v1) → 2D Fauve brush strokes (v2) → 3D smooth-union metaballs (v4 impasto globs)
**Critique:**
1. Reference fidelity: Oil paint impasto — paint squeezed from tube in rounded blobs, capturing the physical material of oil paint.
2. Compositional craft: 8 smooth-unioned spheroid blobs merging and separating; warm umber background with jewel-colored globs in foreground.
3. Technical execution: smin(k=0.25) smooth union of all blobs; 64-step march; positional hash color assignment; Phong lighting with Fresnel-like edge darkening.
4. Liveness: Blob positions driven by independent sin/cos oscillators (unique freq per blob); audio modulates radius.
5. Differentiation: 3D spheroid metaballs vs v2 2D brush strokes; impasto blob palette (umber/cadmium/cobalt/yellow) vs v2 Fauvist stroke palette; organic merging vs directional brushwork.
**Changes:**
- Full rewrite from 2D Fauve strokes to 3D metaball smooth union
- smin(0.25) for all blob pairs — organic merging
- Palette: umber (1.9,0.55,0.10), crimson (2.4,0.08,0.04), cobalt (0.05,0.25,2.5), cadmium yellow (2.3,1.8,0.04)
- 12 potential blobs, count controlled by parameter
- Edge darkening via Fresnel (mix 1.0/0.55 at grazing angles)
- Audio modulates blob radius
**HDR peaks reached:** crimson 2.4, cobalt 2.5, cadmium yellow 2.3, specular 2.8+
**Estimated rating:** 4.5★
