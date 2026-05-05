# text_digifade.fs — critique log

## v8 — 2026-05-05 — Holographic Data Sphere (3D Raymarched)

**Approach:** Full 3D SDF raymarch of a holographic data sphere with three orbiting torus rings. Prior attempts were ISF text glitch-dissolve (font atlas, scan lines, text animations) producing desaturated text-on-black visuals.

**Geometry:**
- Central sphere with audioBass-pulsed radius
- 3 tori, each orbiting at 120° phase offset and 60° tilt increments
- Per-torus orbit rotation around Y axis + tilt around Z axis
- Torus thickness responds to audioHigh

**Holographic shading:** Sphere uses azimuthal + poloidal plasma color cycling:
```glsl
float plasma = sin(az*3.0 + TIME*1.1)*0.4 + sin(po*4.0 + TIME*0.8)*0.4
             + sin(az*5.0 - po*2.0 + TIME*0.5)*0.2;
```
Cycles through cyan → magenta → gold → green → violet.

**Scanline modulation:** `scan = 0.85 + 0.15*sin(p.y*18.0 + TIME*4.0)` adds holographic flicker.

**Background:** Sparse grid haze in cyan suggesting holographic projector floor.

**Palette (5 colors, per ring + sphere iridescence):**
- Cyan `(0.0, 0.9, 1.0)` — ring 1
- Magenta `(1.0, 0.0, 0.55)` — ring 2
- Gold `(1.0, 0.75, 0.0)` — ring 3
- Sphere: cycles all 5 via plasma function

**HDR:** rim × 1.6 × hdrPeak + spec × hdrPeak + emission × 0.3. hdrPeak default 2.5 → rim peaks ~4.0 linear.

**Camera:** Slow orbit `camA = TIME * 0.09`, slight vertical bob.

**Fix vs prior:** Replaced ISF font-atlas glitch dissolve (no 3D, text-only, monochrome) with a fully saturated 3D holographic sphere.
