## 2026-05-05
**Prior rating:** 0.8★
**Approach:** 3D REWRITE — raymarched vibrating plate with bump-mapped sand ridges (Chladni figures are a physical phenomenon, not a 2D painting reference)
**Lighting style:** studio — directional Blinn-Phong from above-right with warm specular

**Critique:**
- *Density*: original was a flat UV-space Chladni equation with softedged nodal lines and a halo — correct maths but zero spatial depth or material richness
- *Movement*: TIME drift + audio-mode evolution (bass→N, treble→M) was already solid; phantom resonance overtone at t≈30s a good surprise
- *Palette*: sandColor/plateColor inputs correct; bright sand on dark plate is the authentic look
- *Edges*: `smoothstep(lineSharpness, 0.0, abs(f))` gave soft nodal lines but no bump/height information; no 3D relief on sand ridges; no lighting gradient across grain
- *HDR/Bloom*: audio-reactive brightness via `audioLevel * 1.2` was capped by the flat 2D output; no specular peaks; `sandColor.rgb * bright` never exceeded 1.0; no bloom bait

**Changes made (full 3D rewrite):**
- 64-step SDF sphere march into thin `sdBox` plate (halfsize 1.0, 0.025, 1.0)
- Slow orbital camera (12°/s), Blinn-Phong with warm key from vec3(0.6, 1.0, 0.4)
- Sand ridge bump mapping: finite-diff `sandH` gradient on top face → perturbed normal; `sandAA = smoothstep(0.5 ± fwidth(sandRaw), sandRaw)` for sub-pixel AA
- Specular peaks: `spec * 2.5 * matSpec * (0.8 + bass * 1.2)` — reaches 1.5 HDR on plate surface
- Sand emissive: `sandAA * 1.2 * (0.5 + audioLevel * audioReact)` — dense ridges reach 1.8 HDR
- Grain jitter modulated by audioMid for visual grain response
- Preserved phantom resonance from original (30s cycle, higher harmonic interference)
- Plate edge glow for dimensional read of plate boundary
- Added `audioReact` float input; added "3D" to CATEGORIES
- Optional inputTex mapped onto plate face (mix with plateColor)
- Linear HDR output, no tonemap/gamma

**Estimated rating after:** 4★
