# GLITCH GOD

The ultimate glitch shader: **52 glitch modules in one ISF shader**, half digital, half
research-authentic analog degradation. One `God Seed` hashes into a *recipe* — which
modules fire and how hard — so every seed is a distinct, repeatable glitch personality.
2^52 possible configurations.

- **File:** `shaders/glitch_god.fs` · **Manifest id:** 1024 · **Title:** "Glitch God"
- **In Easel:** auto-loaded from `~/ShaderClaw3/shaders` (hot-reloads on save). Add it as
  a ShaderClaw layer. Bind an image to `Texture` to glitch a source, or leave it empty for
  the built-in procedural test signal.

## Controls

| Input | Range | What it does |
|-------|-------|--------------|
| **God Seed** | 0–1024 | Selects the recipe. Turn it → a totally different glitch. |
| **Chaos** | 0–1 | How many modules stack at once (0 = a few, 1 = everything). |
| **Intensity** | 0–1.5 | Master displacement/effect strength. |
| **Mutate** | 0–1 | >0 auto-advances the recipe over time — the shader cycles through configurations live. |
| **Speed** | 0–2 | Tempo of the per-frame glitch events. |
| **Audio React** | 0–2 | How hard `audioBass/Mid/High` drive the corruption. |
| **Texture** | image | Optional source. Empty → vivid procedural test signal. |

**How the recipe works:** each module is gated by `onMod(salt, rarity)` = a hash of the
seed. Common modules (chroma split, scanlines, bloom) have low rarity and fire often; wild
ones (kaleido, strobe, vertical-hold) are rarer. `Chaos` lowers every gate at once. Each
active module reads its own per-seed parameter, so seed 222 and seed 223 are unrelated
looks. Bass kicks macroblock/sync, mids drive warp/colorizer gain, treble drives
chroma/snow/sparkle.

## Module catalog

### Digital glitch (1–23)
chromatic aberration · pixel-sort drift · datamosh P-frame smear · 8×8 macroblock/DCT
corruption · CRT barrel + bezel · kaleido mirror-fold · sine warp · slab slice · sync tear ·
block-jump · frame jitter · mosaic pixelate · posterize · hue-rotate · channel swap/invert ·
solarize · scanlines · analog static · ordered dither/bit-crush · VHS magenta/cyan tint ·
ghost echo · sobel neon edges · strobe flash · HDR bloom feeders.

### Analog: VHS / magnetic tape
- **Color-Under Smear** — chroma downconverted to ~629 kHz lags/smears right of edges.
- **Tracking Tear** (head-switch band) — heads switch in VBI; bottom 6–12 lines tear + snow.
- **Dropout Streak** — oxide loss → white/black streaks or dropout-compensator line-repeat.
- **Tracking Lost** (roll) — head misalignment → noise band rolling up the frame.
- **TBC Off** (time-base) — per-line H-sync jitter + top-edge flagging.
- **3rd-Gen EP Dub** (generation loss) — soft luma, grown chroma noise, desat, level crush.
- **RF Snow** — bright sparse luma specks, heaviest in shadows.

### Analog: CRT / NTSC-PAL composite
- **Dot Crawl** — 3.58 MHz subcarrier crosstalk → crawling checkerboard on color edges.
- **Fine-Detail Rainbow** (cross-color) — fine luma misdecoded as chroma.
- **PAL Hanover** — per-line V-phase error → opposing-hue horizontal blinds.
- **Ghost Edge** (ringing) — bandwidth-limited overshoot halos + trailing echo.
- **Interlace Tear** (combing) — odd field lags → comb teeth on motion.
- **Phosphor Triad** — RGB shadow-mask + Gaussian scanline beam profile.
- **Burn-In Trail** (persistence) — long-persistence phosphor afterglow.
- **Vertical Hold Lost** — picture rolls with a black VBI retrace bar at the seam.

### Analog: RF / transmission / EMI
- **Weak Signal** (RF snow) — tilted noise-floor gradient swallowing the image.
- **Ghosting** — multipath reflections → faint delayed copies offset right.
- **Sync Slip / No Lock** — corrupted sync → per-line tear bursts + vertical roll.
- **Hum Bar Roll** — 50/60 Hz mains beat → soft brightness bands crawling up.
- **Herringbone** (co-channel) — two beating carriers → drifting diagonal weave.
- **Ignition Buzz** (sparkle) — EMI impulse noise → bright specks + horizontal dashes.

### Analog: video synthesis
- **Feedback Tunnel** — single-pass iterated zoom+rotate (camera-into-monitor recursion).
- **Rutt-Etra Terrain** — luminance deflects each scanline → glowing wireframe terrain.
- **Sandin Colorize** — quantize luma → voltage-controlled false-color palette banding.
- **Phase Shift Hue** (Paik-Abe) — chroma subcarrier phase rotation with nonlinear twist.
- **Wobbulator** — audio-rate sine modulation of the deflection yoke (raster bends).
- **Solar Fold** — Sabattier nonlinear transfer inverts highlights + Mackie-line rim.
- **Chroma Bloom** — overdriven colorizer gain clips into oversaturated voltage rails.

## Starting points

| Vibe | Seed | Chaos | Intensity |
|------|------|-------|-----------|
| Subtle CRT/VHS | 88 | 0.30 | 0.8 |
| Classic broadcast glitch | 222 | 0.50 | 1.0 |
| Heavy analog decay | 404 | 0.75 | 1.1 |
| Maximalist meltdown | 1000 | 0.95 | 1.3 |
| Living / self-evolving | any | 0.60 | 1.0 + **Mutate 0.4** |

Don't like a look? Nudge the **God Seed** by 1, or set **Mutate** > 0 and let it cycle.

## Credits

Digital vocabulary after Rosa Menkman, Takeshi Murata, JODI, Cory Arcangel. Analog modules
are research-authentic, modeled on real hardware behavior (VHS color-under & head-switching,
NTSC/PAL composite decode, RF multipath/EMI, and the Rutt–Etra, Sandin IP, and Paik–Abe
synthesizers). Built by a fleet of domain-expert agents, integrated and validated into a
single `glitch_god.fs`.
