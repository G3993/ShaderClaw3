## 2026-05-05
**Prior rating:** 1.0★
**Approach:** 2D refine (Lichtenstein + Warhol + Rosenquist — named 2D painting exceptions; rewrite-to-3D exemption applied)
**Lighting style:** n/a — flat graphic print aesthetic

**Critique:**
- *Density*: three techniques (Ben-Day dots, 4-up silkscreen, halftone pop) well differentiated; speech bubble with drawWord glyphs is a nice touch
- *Movement*: `audioBass` scales speech bubble, `audioMid` modulates dot radius — already reactive; gold starburst surprise at 14s
- *Palette*: LIK constants (ink, white, red, yellow, blue) are Lichtenstein-accurate; TINT array for Warhol is solid
- *Edges*: Ben-Day dots used `step(length(gf), r)` — hard aliased circle edges visible at low density; Halftone Pop same issue
- *HDR/Bloom*: gold starburst used `mix(col, yellow, 0.85)` — max 1.0, no bloom signal; output always ≤1.0

**Changes made:**
- fwidth AA on Ben-Day dots (both Halftone Pop and Ben-Day technique): `smoothstep(r+fw, r-fw, dotD)` where `fw = max(fwidth(dotD), 0.5/dotDensity)` — sub-pixel smooth circle edges
- Gold starburst: `mix → additive` — `col += vec3(1.0, 0.85, 0.15) * _f * _star * 1.5` — starburst peaks 1.5 HDR for bloom

**Estimated rating after:** 2.5★
