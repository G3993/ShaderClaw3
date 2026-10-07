/*{
  "CATEGORIES": [
    "Generator",
    "Text"
  ],
  "DESCRIPTION": "Typewriter — characters appear one by one with blinking cursor. SCFONT/1 MSDF atlas (glyphMSDFTex + glyphMetricsTex) with proportional advance widths; SCTEXT/1 glyph buffer (textBuf_msg): mixed case, punctuation, digits; Voice Sync reveals by per-word timing when the host provides it.",
  "INPUTS": [
    {
      "NAME": "msg",
      "TYPE": "text",
      "DEFAULT": "Etherea",
      "MAX_LENGTH": 48,
      "CASE": "preserve",
      "OVERFLOW": "tail",
      "LABEL": "Message",
      "GROUP": "Text"
    },
    {
      "NAME": "fontFamily",
      "LABEL": "Font",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3,
        4
      ],
      "LABELS": [
        "Inter",
        "Times New Roman",
        "Libre Caslon",
        "Outfit",
        "Caption"
      ],
      "DEFAULT": 0,
      "GROUP": "Text"
    },
    {
      "NAME": "fontWeight",
      "LABEL": "Weight",
      "TYPE": "float",
      "MIN": 100,
      "MAX": 900,
      "DEFAULT": 400,
      "GROUP": "Text"
    },
    {
      "NAME": "textScale",
      "LABEL": "Size",
      "TYPE": "float",
      "MIN": 0.01,
      "MAX": 1,
      "DEFAULT": 0.3,
      "GROUP": "Text"
    },
    {
      "NAME": "kerning",
      "LABEL": "Spacing",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 3,
      "DEFAULT": 1,
      "GROUP": "Text"
    },
    {
      "NAME": "speed",
      "LABEL": "Speed",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 40,
      "DEFAULT": 12,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "cursorBlink",
      "LABEL": "Cursor Blink",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 5,
      "DEFAULT": 2,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "oscSpeed",
      "LABEL": "Osc Speed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 10,
      "DEFAULT": 0,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "oscAmount",
      "LABEL": "Osc Amount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.2,
      "DEFAULT": 0,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "oscSpread",
      "LABEL": "Osc Spread",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.5,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "textColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        1,
        1,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "hueShift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0,
      "LABEL": "Hue Shift",
      "GROUP": "Color"
    },
    {
      "NAME": "colorBoost",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "LABEL": "Color Boost",
      "GROUP": "Color"
    },
    {
      "NAME": "bgColor",
      "LABEL": "Background",
      "TYPE": "color",
      "DEFAULT": [
        0.02,
        0.02,
        0.04,
        1
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "transparentBg",
      "LABEL": "Transparent",
      "TYPE": "bool",
      "DEFAULT": true,
      "GROUP": "Background"
    },
    {
      "NAME": "voiceSync",
      "LABEL": "Voice Sync",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "loop",
      "LABEL": "Loop",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}*/

// SCFONT/1 proof conversion (Phase D): glyphs come from the textBuf_msg glyph
// buffer (scTextCode / scTextWord / scTextRevealMs) and are rendered from the
// MSDF atlas glyphMSDFTex with per-glyph plane boxes and proportional advance
// widths read from glyphMetricsTex — helpers injected by the host (sc_text v2,
// analytic path only; no derivatives). No msg_i uniforms, no 37-cell
// fontAtlasTex, no 96-cell grid sampling.

float h11(float x) { return fract(sin(x * 127.1) * 43758.5453); }

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = int(msg_len);
    if (numChars <= 0) numChars = 7;
    if (numChars > 64) numChars = 64;
    float sc = textScale > 0.01 ? textScale : 1.0;
    float kr = kerning > 0.01 ? kerning : 1.0;

    vec3 col = bgColor.rgb;
    float alpha = transparentBg ? 0.0 : 1.0;

    vec2 p = vec2((uv.x - 0.5) * aspect + 0.5, uv.y);
    float maxW = aspect * 0.9;

    // LINEAR band envelopes (r2): the round-1 pow/smoothstep knees crushed
    // ambient's 0.1-0.8 swells into ~2% size wiggle — deaf. The bands are
    // already smoothed upstream; these all feed geometry (scale/bob/drift),
    // so they take no knee. Knee-shaping stays on the beat accent only.
    // NOTE: audio modulates AMPLITUDES of a stable TIME clock — the old
    // `TIME * (0.5 + 1.2*energy)` clock phase-jumped instead of following.
    float bassP = audioBass;
    float midP  = audioMid;
    float highP = audioHigh;
    float beatKick = audioBeatPulse * audioBeatPulse;

    // Typewriter reveal
    int revealed;
    if (voiceSync) {
        // Voice sync mode: reveal by per-glyph word timing (SCTEXT/1 revealMs)
        // against the utterance clock msgAge. Glyphs with unknown timing are
        // shown immediately (= "show everything the recognizer produced");
        // msgAge < 0 (static text) shows all.
        if (msgAge >= 0.0) {
            float nowMs = max(msgAge, 0.0) * 1000.0;
            revealed = 0;
            for (int i = 0; i < 64; i++) {
                if (i >= numChars) break;
                float rm = scTextRevealMs(textBuf_msg, msg_cap, i);
                if (rm < 0.0 || rm <= nowMs) revealed++;
            }
        } else {
            revealed = numChars;
        }
    } else {
        float typeTime = float(numChars) / speed;
        float t = TIME;
        if (loop) {
            float cycle = typeTime + 2.0;
            t = mod(t, cycle);
        }
        revealed = int(floor(t * speed));
        if (revealed > numChars) revealed = numChars;
    }
    int showCount = revealed;

    // Sum of the shown glyphs' proportional advances (em, SCFONT/1 metrics) —
    // drives auto-fit and centering; replaces the old monospace cell count.
    float gapEm = 0.05 * kr;
    float sumAdvEm = 0.0;
    for (int i = 0; i < 64; i++) {
        if (i >= showCount) break;
        if (i >= numChars) break;
        int rcp0 = scGlyphResolve(glyphMetricsTex, scTextCode(textBuf_msg, msg_cap, i));
        sumAdvEm += scGlyphAdvance(glyphMetricsTex, rcp0) + gapEm;
    }

    // Auto-scale: shrink the em size to fit all revealed glyphs on screen
    float baseH = 0.18 * sc;
    if (aspect < 1.0) baseH *= aspect;
    float neededW = max(sumAdvEm, 0.001) * baseH;
    float fitScale = neededW > maxW ? maxW / neededW : 1.0;

    // Size breathing — the whole word swells with bass and mids. This is
    // the main audible→visible path: glyphs are the only big thing on
    // screen, so scale is where the response has to live. Silence → 1.0.
    float sizePulse = 1.0 + 0.14 * bassP + 0.07 * midP;
    float em = baseH * fitScale * sizePulse;   // screen-relative em size

    // Analytic MSDF sharpness: glyph px on screen vs glyph px in the atlas
    float screenPxRange = scMSDFScreenPxRange(glyphAtlasInfo, em * RENDERSIZE.y, glyphAtlasInfo.w);

    // Slow global bob — always-on autonomous drift; bass deepens its swing.
    float globalBob = em * (0.05 + 0.10 * bassP) * sin(TIME * 0.5);
    float baseY = 0.5 - em * 0.35 + globalBob;   // baseline

    // Center visible text — all characters always visible
    float visibleW = (showCount > 0) ? (sumAdvEm - gapEm) * em : 0.0;
    float originX = 0.5 - visibleW * 0.5;

    // Render characters — pen advances by each glyph's advance width
    float textMask = 0.0;
    vec3 textCol = vec3(0.0);
    float pen = originX;

    for (int i = 0; i < 64; i++) {
        if (i >= showCount) break;
        if (i >= numChars) break;

        int cp = scTextCode(textBuf_msg, msg_cap, i);
        int rcp = scGlyphResolve(glyphMetricsTex, cp);
        float adv = scGlyphAdvance(glyphMetricsTex, rcp) * em;

        // Autonomous per-character drift — small, incommensurate frequencies
        // (golden-angle phase spacing) so characters never lock into a shared
        // period; lives even with the user oscillator off / in silence.
        // Phase is per WORD (SCTEXT/1 word index) so letters of a word drift together.
        float idxPhase = max(float(scTextWord(textBuf_msg, msg_cap, i)), 0.0) * 2.399963;
        float driftX = em * (0.03 + 0.06 * highP) * sin(TIME * (0.23 + 0.07 * h11(float(i) + 3.7)) + idxPhase * 1.3);
        float driftY = em * (0.06 + 0.15 * midP) * sin(TIME * (0.35 + 0.11 * h11(float(i))) + idxPhase);
        // Oscillator: per-character Y offset (user-controlled + autonomous drift)
        float oscY = driftY + oscAmount * sin(TIME * oscSpeed * 6.2832 + float(i) * oscSpread * 3.14159);

        if (cp > 32) {
            // Glyph box = pen + plane bounds (em, y up from the baseline)
            vec4 plane = scGlyphPlane(glyphMetricsTex, rcp);
            vec2 boxMin = vec2(pen + driftX + plane.x * em, baseY + oscY + plane.y * em);
            vec2 boxMax = vec2(pen + driftX + plane.z * em, baseY + oscY + plane.w * em);
            if (boxMax.x > boxMin.x && boxMax.y > boxMin.y) {
                vec2 localUV = (p - boxMin) / (boxMax - boxMin);
                float cov = scGlyphMSDF(glyphMSDFTex, glyphMetricsTex, glyphAtlasInfo, rcp, localUV, screenPxRange);
                if (cov > 0.0) {
                    textCol = textColor.rgb;
                    textMask = max(textMask, cov);
                }
            }
        }

        pen += adv + gapEm * em;
    }

    // Blinking cursor after the pen — phase-wobbled so it never freezes
    // into a perfectly periodic (visually static) blink, and gets a soft
    // width pulse on beat.
    float blinkPhase = fract(TIME * cursorBlink + 0.15 * sin(TIME * 0.37));
    float cursorOn = step(0.5, blinkPhase);
    float cursorW = em * 0.09 * (1.0 + 0.4 * beatKick);
    if (p.x >= pen && p.x <= pen + cursorW &&
        p.y >= baseY && p.y <= baseY + em * 0.72) {
        textCol = textColor.rgb;
        textMask = max(textMask, cursorOn);
    }

    // R3: backdrop RGB breath — the eval (and any RGB-reading consumer)
    // ignores alpha; with a transparent bg 99.8% of the canvas carried zero
    // RGB response and the tiny glyphs (~0.2% of pixels) could never move
    // frameDiff. Linear composite follower with bus band-mix weights; alpha
    // stays textMask so the app's transparent overlay is untouched.
    // Silence = exactly the current look (adds 0).
    float bgB = 0.14 * bassP + 0.09 * midP + 0.055 * highP;
    col += vec3(0.85, 0.90, 1.15) * bgB;

    col = mix(col, textCol, clamp(textMask, 0.0, 1.0));
    if (transparentBg) alpha = clamp(textMask, 0.0, 1.0);

    // ---- universal color block (defaults = no-op) ----
    vec3 uc = col;
    float ucL = dot(uc, vec3(0.299, 0.587, 0.114));
    uc = mix(vec3(ucL), uc, colorBoost);                     // saturation
    if (hueShift > 0.0005) {                                  // cheap hue rotate (YIQ)
        float hA = hueShift * 6.2831853;
        float hC = cos(hA), hS = sin(hA);
        mat3 hM = mat3(0.299,0.587,0.114, 0.299,0.587,0.114, 0.299,0.587,0.114)
                + hC * mat3(0.701,-0.587,-0.114, -0.299,0.413,-0.114, -0.300,-0.588,0.886)
                + hS * mat3(0.168,0.330,-0.497, -0.328,0.035,0.292, 1.250,-1.050,-0.203);
        uc = clamp(hM * uc, 0.0, 1.0);
    }
    col = uc;

    gl_FragColor = vec4(col, alpha);
}
