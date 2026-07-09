# text_ascii.fs — critique 2026-05-05

## Issues found
- **White default textColor**: `[1,1,1,1]` — monochrome white rain, no saturation, no per-column color identity. All columns look identical in hue.
- **SDR head glow**: `mix(charCol, vec3(1.0), headGlow * 0.7)` mixed toward SDR white (1.0) — the leading character only reached luminance 1.0, no bloom potential.
- **SDR trail**: `textColor.rgb * brightness` peaked at 1.0 — no signal above the bloom threshold for the host pipeline.
- **No per-column color variation**: Every column was the same color, losing the visual richness of the Matrix aesthetic.

## Changes made
1. `textColor` default: `[1,1,1,1]` → `[0.0, 1.0, 0.2, 1.0]` (vivid Matrix green baseline)
2. **Per-column rainbow**: each column gets a distinct hue from a full-spectrum HSV rainbow (column index * 0.08 drives hue), multiplied against textColor — gives distinct vivid color identity per stream while respecting user's color choice
3. **HDR trail**: `brightness * 2.5×` — peak trail luminance 2.5 linear for reliable host bloom
4. **HDR head burst**: `mix(colTint, vec3(1.0), 0.3) * 3.5` — leading character spikes to 3.5× for a crisp bright head marker, with headGlow blend weight 0.7→0.85
