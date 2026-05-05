# text_spacy critique log

## Entry 1 — prior (starfield background)
- **Technique**: 2D perspective tunnel rows + procedural star field background
- **Lighting**: None — flat star points, no volumetric light
- **Composition**: Text fills full screen in scrolling rows; background is pure black with white dot stars
- **Color grading**: Monochrome — white text on black; no hue variation in background
- **Reference**: Classic Star Wars opening crawl style
- **Weaknesses**: Background is decorative filler with no energy; stars are static hash points with no animation beyond twinkle; palette is undersaturated; HDR headroom unused

## Entry 2 — v17 (solar corona background)
- **Technique**: 2D solar disk + corona halo + FBM prominence arcs + radial solar wind streaks; depth-based text brightness (outer rows brighter)
- **Lighting**: HDR chromosphere gradient (deep orange → white-hot 2.0× at core); corona exponential halo; prominence arcs emissive vec3(1.0,0.4,0.0)×hdrGlow×0.7
- **Composition**: Solar disk centered, text rows scroll over it; rows closer to screen edge are brighter (simulating depth)
- **Color grading**: Deep orange / crimson / white-hot; near-black red void; fully saturated warm palette
- **Reference**: SOHO/SDO solar corona imagery
- **Differentiation axes**: lighting model (flat→HDR emissive), background subject (stars→stellar corona), color temperature (cool white→deep orange/crimson), FBM usage (none→prominence arc warping)
