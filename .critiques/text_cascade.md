# text_cascade.fs — critique log

## v8 — 2026-05-05 — Falling Crystal Prisms (3D Raymarched)

**Approach:** Full 3D SDF raymarch of octahedral crystal prisms drifting and falling through a dark void. Prior attempts were ISF text-cascade (font atlas + wave offset) producing dim white-text-on-black visuals with no depth or color saturation.

**Geometry:** N prisms (2–14) at golden-angle positions, each falling through Y via `mod`-based cyclic loop with per-prism speed variation. Each prism spins simultaneously on XZ plane and XY plane via per-frame trig rotation, creating dynamic tumbling.

**SDF:** Approximate elongated octahedron:
```glsl
float sdOctPrism(vec3 p, float r, float elongate) {
    p.y /= elongate;
    return (abs(p.x) + abs(p.y) + abs(p.z) - r) * 0.57735 * min(1.0, elongate);
}
```
Conservative (under-estimates), compensated with 0.75 step factor.

**Palette (5 colors, fully saturated crystalline):**
- Icy cyan `(0.0, 0.9, 1.0)`
- Violet `(0.65, 0.0, 1.0)`
- Gold `(1.0, 0.75, 0.0)`
- Magenta `(1.0, 0.0, 0.55)`
- Crystal green `(0.0, 1.0, 0.35)`

**HDR:** rim × 1.7 × glowStrength + spec × 1.3 × glowStrength + ambient × 0.25. glowStrength default 2.2 → rim peaks ~3.74 linear.

**Audio:** audioBass pulses each prism's scale via `pulse = 1.0 + audioBass * audioPulse * 0.12`.

**Additional:** 40-star background with sinusoidal twinkle; camera slowly orbits with slight vertical bob.

**Fix vs prior:** Replaced ISF font-atlas text cascade (zero visual depth, monochrome, no 3D) with fully saturated 3D crystal scene.
