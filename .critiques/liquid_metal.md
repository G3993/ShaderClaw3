## 2026-05-05
**Prior rating:** 1.9★
**Approach:** 2D refine (sophisticated CFD fluid simulation — 2-pass rotational curl, not easily 3D-ified)

**Critique:**
- *Reference fidelity:* The CFD simulation itself is excellent — multi-scale rotational curl with self-advection is genuinely sophisticated. But the near-white `metalColor [0.85, 0.88, 0.95]` tint strips all saturation from the output; every render looks silver-gray regardless of the environment.
- *Compositional craft:* Default `envBright 1.41` is below a useful HDR threshold; the environment map doesn't punch. The cyan horizon band is a good compositional anchor but is too dim (V=0.9 → horizon*1.3 = 1.17).
- *Technical execution:* Specular `spec * 1.5` barely reaches 1.5 linear — invisible against a near-white surface. Audio bass only adds `metalColor * 0.1` — inaudible in practice.
- *Liveness:* Fluid motion is great; audio responsiveness near-zero at default settings.
- *Differentiation:* Looks like brushed aluminum with bad lighting. The bismuth/oil-slick potential isn't realized.

**Changes:**
- Changed `metalColor` default to teal `[0.0, 0.8, 1.0]` — saturated cyan tint unlocks the oil-slick/bismuth look
- Changed `envBright` default 1.41→2.5 (MAX extended to 5.0); sky/horizon now reach 2.5-4.0 linear
- Boosted sky: `skyHigh` to V=1.8, `skyLow` to V=1.2, both with higher saturation (0.7/0.8)
- Changed horizon to HDR cyan `hsv(0.52, 0.9, 1.6)` — punchy teal-white horizon band
- Specular: replaced `spec * 1.5 * metalColor` with white-hot `spec * 4.0` mixed 50/50 with metalColor tint — peak ~4 linear
- Audio bass pulse: 0.1→0.5

**HDR peaks reached:** ~3.5–5.0 linear (envBright 2.5 × sky V=1.8, specular peaks ~4.0)
**Estimated rating:** 3.5★
<!-- auto-improve 2026-05-05 -->
