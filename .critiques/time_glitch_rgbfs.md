# time_glitch_rgbfs critique log

## Entry 1 — prior (VIDVOX 8-frame buffer delay filter)
- **Technique**: 10-pass persistent-buffer frame delay with randomized horizontal/vertical glitch blocks; requires inputImage
- **Lighting**: None — pure post-process colour remap on captured frames
- **Composition**: Depends on input; glitch blocks are uniform random tiles across screen
- **Color grading**: Inherits input colours with per-channel temporal offset (RGB delay differential)
- **Reference**: VIDVOX ISF standard effects library
- **Weaknesses**: inputImage dependency means zero standalone value; no generative content; no audio reactivity; HDR headroom completely unused; palette fully determined by input

## Entry 2 — v17 (BCC Diamond Crystal Lattice)
- **Technique**: Single-pass raymarcher (72 steps); BCC lattice via domain-repeat mod(); 8-bond sdCapsule per A-site; fwidth-based neon iso-rings on atom surfaces
- **Lighting**: HDR white specular ×2.2 (peak >2.0 linear); fresnel rim; emissive core glow modulated by audio; AO (5-sample)
- **Composition**: Camera spirals inside the infinite lattice, slow sin-wave vertical drift; near-field atoms fill frame as large glowing spheres
- **Color grading**: Blue Crystal / Amethyst / Ice White palettes; deep navy/violet void; fully saturated emissive cores
- **Reference**: Diamond-cubic BCC crystal structure, scanning-electron-microscopy crystal imagery
- **Differentiation axes**: technique (post-process filter→SDF raymarcher), subject (temporal glitch→crystal physics), dimensionality (2D/filter→3D generative), palette (input-dependent→cool blue/violet HDR), audio (unused→emissive core modulation)
