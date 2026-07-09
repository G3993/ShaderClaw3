/*{
  "DESCRIPTION": "Text Cylinder — your message wraps around the circumference of a raymarched 3D cylinder, embossed into chrome and lit with neon rim light. The camera orbits slowly while the label itself drifts in a lazy independent spin, so the wrap-around reads continuously. Bass pops the letter relief, mids ripple the chrome, highs sparkle a sparse subset of glyphs, beats flash the neon — all eased around an always-alive idle rotation. An optional image ghosts into the chrome as a fake environment reflection.",
  "CREDIT": "ShaderClaw3",
  "CATEGORIES": [
    "Generator",
    "Text",
    "3D",
    "Audio Reactive"
  ],
  "INPUTS": [
    {
      "NAME": "embossAmount",
      "LABEL": "Letter Relief",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.6,
      "DEFAULT": 0.22,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "spinSpeed",
      "LABEL": "Label Spin Speed",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 1,
      "DEFAULT": 0.07,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "chromeAmount",
      "LABEL": "Chrome Amount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.65,
      "GROUP": "Color"
    },
    {
      "NAME": "neonColorA",
      "LABEL": "Neon Color A",
      "TYPE": "color",
      "DEFAULT": [
        0.15,
        0.95,
        1,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "neonColorB",
      "LABEL": "Neon Color B",
      "TYPE": "color",
      "DEFAULT": [
        0.95,
        0.15,
        0.85,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "hueShift",
      "LABEL": "Hue Shift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0,
      "GROUP": "Color"
    },
    {
      "NAME": "colorBoost",
      "LABEL": "Color Boost",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Color"
    },
    {
      "NAME": "orbitSpeed",
      "LABEL": "Camera Orbit Speed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1.5,
      "DEFAULT": 0.15,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "msg",
      "TYPE": "text",
      "DEFAULT": "ETHEREA",
      "MAX_LENGTH": 48,
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
        3
      ],
      "LABELS": [
        "Inter",
        "Times New Roman",
        "Libre Caslon",
        "Outfit"
      ],
      "DEFAULT": 0,
      "GROUP": "Text"
    },
    {
      "NAME": "textScale",
      "LABEL": "Label Size",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Text"
    },
    {
      "NAME": "kerning",
      "LABEL": "Kerning",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 1,
      "DEFAULT": 0.82,
      "GROUP": "Text"
    },
    {
      "NAME": "bgColor",
      "LABEL": "Background",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        1
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "transparentBg",
      "LABEL": "Transparent",
      "TYPE": "bool",
      "DEFAULT": false,
      "GROUP": "Background"
    },
    {
      "NAME": "audioReact",
      "LABEL": "Sound Reactivity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.85,
      "GROUP": "Audio Reactivity"
    },
    {
      "NAME": "texMix",
      "LABEL": "Image Reflection Mix",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "inputImage",
      "TYPE": "image",
      "LABEL": "Reflection Image"
    }
  ]
}*/

// ---- universal color block (defaults = no-op) ----
vec3 ucApply(vec3 uc) {
    float ucL = dot(uc, vec3(0.299, 0.587, 0.114));
    uc = mix(vec3(ucL), uc, colorBoost);                      // saturation
    if (hueShift > 0.0005) {                                  // cheap hue rotate (YIQ)
        float hA = hueShift * 6.2831853;
        float hC = cos(hA), hS = sin(hA);
        mat3 hM = mat3(0.299,0.587,0.114, 0.299,0.587,0.114, 0.299,0.587,0.114)
                + hC * mat3(0.701,-0.587,-0.114, -0.299,0.413,-0.114, -0.300,-0.588,0.886)
                + hS * mat3(0.168,0.330,-0.497, -0.328,0.035,0.292, 1.250,-1.050,-0.203);
        uc = clamp(hM * uc, 0.0, 1.0);
    }
    return uc;
}


// ============================================================================
// TEXT CYLINDER
// A single raymarched capped cylinder (cheap analytic SDF, no texture reads
// during the march). The message wraps around its circumference — one
// character per angular slot — and is only shaded onto the surface *after*
// the march, at the single hit point: angle -> column, height -> row, looked
// up in the house 5x7 font atlas (fontAtlasTex, A-Z=0-25 / space=26 / 0-9=
// 27-36 via msg_N floats + msg_len, ANGLE-safe if-chain, no dynamic array
// indexing). Letter relief is a fake bump normal built from finite
// differences of the glyph mask along the cylinder's own angular/vertical
// tangent frame (closed-form for a cylinder — no dFdx/fwidth needed).
// Chrome/neon shading: fresnel + two specular light dirs for the metal read,
// a two-hue neon palette for the emissive letterforms, and an optional
// equirectangular "reflection" sample of the user image gated by texMix.
// Black background, high contrast, camera orbits slowly while the label
// itself keeps a lazy independent spin so the wrap never looks frozen.
// ============================================================================

const float PI      = 3.14159265;
const float TWO_PI   = 6.28318530;
#define MAX_STEPS 80
#define MAX_DIST  14.0
#define EPS       0.0016

// ─── audio conditioning (playbook standard) ────────────────────────────────
float knee(float x, float lo, float hi){ return smoothstep(lo, hi, x); }

// ─── house font atlas idiom (A-Z=0-25, space=26, 0-9=27-36) ────────────────
float charPixel(int ch, float col, float row) {
    if (ch < 0 || ch > 36) return 0.0;
    vec2 uv = vec2(col / 5.0, row / 7.0);
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return smoothstep(0.20, 0.42, texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r);
}

int getChar(int slot) {
    if (slot == 0)  return int(msg_0);  if (slot == 1)  return int(msg_1);
    if (slot == 2)  return int(msg_2);  if (slot == 3)  return int(msg_3);
    if (slot == 4)  return int(msg_4);  if (slot == 5)  return int(msg_5);
    if (slot == 6)  return int(msg_6);  if (slot == 7)  return int(msg_7);
    if (slot == 8)  return int(msg_8);  if (slot == 9)  return int(msg_9);
    if (slot == 10) return int(msg_10); if (slot == 11) return int(msg_11);
    if (slot == 12) return int(msg_12); if (slot == 13) return int(msg_13);
    if (slot == 14) return int(msg_14); if (slot == 15) return int(msg_15);
    if (slot == 16) return int(msg_16); if (slot == 17) return int(msg_17);
    if (slot == 18) return int(msg_18); if (slot == 19) return int(msg_19);
    if (slot == 20) return int(msg_20); if (slot == 21) return int(msg_21);
    if (slot == 22) return int(msg_22); if (slot == 23) return int(msg_23);
    if (slot == 24) return int(msg_24); if (slot == 25) return int(msg_25);
    if (slot == 26) return int(msg_26); if (slot == 27) return int(msg_27);
    if (slot == 28) return int(msg_28); if (slot == 29) return int(msg_29);
    if (slot == 30) return int(msg_30); if (slot == 31) return int(msg_31);
    if (slot == 32) return int(msg_32); if (slot == 33) return int(msg_33);
    if (slot == 34) return int(msg_34); if (slot == 35) return int(msg_35);
    if (slot == 36) return int(msg_36); if (slot == 37) return int(msg_37);
    if (slot == 38) return int(msg_38); if (slot == 39) return int(msg_39);
    if (slot == 40) return int(msg_40); if (slot == 41) return int(msg_41);
    if (slot == 42) return int(msg_42); if (slot == 43) return int(msg_43);
    if (slot == 44) return int(msg_44); if (slot == 45) return int(msg_45);
    if (slot == 46) return int(msg_46); return int(msg_47);
}

int charCount() {
    int n = int(msg_len + 0.5);
    if (n < 1) n = 1;
    if (n > 48) n = 48;
    return n;
}

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// ─── scene: plain capped cylinder (cheap SDF, no texture in the march) ─────
float gRadius    = 1.0;
float gHalfHeight = 1.0;

float mapScene(vec3 p) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(gRadius, gHalfHeight);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(EPS, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy) - mapScene(p - e.xyy),
        mapScene(p + e.yxy) - mapScene(p - e.yxy),
        mapScene(p + e.yyx) - mapScene(p - e.yyx)
    ));
}

bool marchCylinder(vec3 ro, vec3 rd, out vec3 hitP) {
    float d = 0.0;
    bool hit = false;
    vec3 p = ro;
    for (int i = 0; i < MAX_STEPS; i++) {
        p = ro + rd * d;
        float ds = mapScene(p);
        if (ds < EPS) { hit = true; break; }
        d += ds * 0.9;
        if (d > MAX_DIST) break;
    }
    hitP = p;
    return hit;
}

// ─── glyph lookup on the cylinder surface: angle -> column, height -> row ──
// angleNorm in [0,1) around the circumference, height in world units.
float glyphMaskAt(float angleNorm, float height, float totalSlots, int numChars,
                  float glyphWidthFrac, float bandHalf, out int outCi) {
    float rawIdx  = fract(angleNorm) * totalSlots;
    float slotIdx = floor(rawIdx);
    float localU  = fract(rawIdx);
    outCi = int(mod(slotIdx, max(float(numChars), 1.0)));
    if (localU > glyphWidthFrac) return 0.0;
    float rowV = (bandHalf - height) / (2.0 * bandHalf);
    if (rowV < 0.0 || rowV > 1.0) return 0.0;
    int ch  = getChar(outCi);
    float col = (localU / glyphWidthFrac) * 5.0;
    float row = rowV * 7.0;
    return charPixel(ch, col, row);
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 ndc = (gl_FragCoord.xy - 0.5 * res) / res.y;
    float t = TIME;

    // --- audio conditioning (playbook standard) ----------------------------
    float bassP = pow(knee(audioBass * audioReact, 0.05, 0.85), 1.6);
    float midP  = knee(audioMid  * audioReact, 0.05, 0.9);
    float highP = pow(knee(audioHigh * audioReact, 0.10, 0.90), 1.2);
    float drive = 0.25 + 0.75 * knee(audioEnergy * audioReact, 0.05, 0.9);
    float beatPulse = clamp(audioBeatPulse * audioReact, 0.0, 1.5);
    float punch = clamp(audioPunch * audioReact, 0.0, 1.5);

    // --- message geometry: circumference sized to fit the label neatly -----
    int numChars = charCount();
    float nf = max(float(numChars), 1.0);
    float bandHalf   = 0.30 * textScale;
    gHalfHeight      = bandHalf + 0.42 * textScale;
    gRadius          = clamp(0.34 * textScale * sqrt(nf) + 0.35 * textScale, 0.55, 3.0);
    float totalSlots = nf;
    float glyphWidthFrac = clamp(kerning, 0.5, 1.0);

    // --- camera: slow orbit, idle-alive bob + gentle bass lift --------------
    // Constant orbit speed (silent look unchanged: 0.5+0.6*0.25 = 0.65) plus a
    // BOUNDED audio angle offset — never TIME * (audio speed), which makes the
    // camera jump proportionally to elapsed time on every energy swell.
    // (no beatPulse term here: an instant-attack envelope on the camera angle
    // rotated the whole scene ~9 deg in one frame on every beat = the chop)
    float camAng  = t * orbitSpeed * 0.65 + 0.45 * (drive - 0.25);
    float camDist = gRadius * 1.55 + 0.95;
    float camY = 0.16 * gHalfHeight + 0.10 * gHalfHeight * sin(t * 0.23)
               + 0.05 * gHalfHeight * bassP * sin(t * 2.7);
    vec3 ro = vec3(sin(camAng) * camDist, camY, cos(camAng) * camDist);
    vec3 ta = vec3(0.0, camY * 0.35, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 upv = cross(fwd, rgt);
    // zoom rides the SMOOTHED bass follower; only a whisper of instant punch
    // (a 7% one-frame fov snap moved every edge pixel = chop)
    float fov = 1.05 - 0.05 * bassP - 0.02 * punch;
    vec3 rd = normalize(fwd + (ndc.x * rgt + ndc.y * upv) * fov);

    // --- label spin: always-on idle drift, independent from the camera -----
    float spinPhase = t * spinSpeed * 0.5 + 0.06 * bassP * sin(t * 0.7);

    vec3 hitP;
    bool hit = marchCylinder(ro, rd, hitP);

    vec3 outCol = transparentBg ? vec3(0.0) : bgColor.rgb;
    float outAlpha = transparentBg ? 0.0 : 1.0;

    if (hit) {
        vec3 n = calcNormal(hitP);
        float theta = atan(hitP.z, hitP.x);
        float angleNorm = fract(theta / TWO_PI + 0.5 + spinPhase);
        float height = hitP.y;

        int ci = 0;
        float mask = glyphMaskAt(angleNorm, height, totalSlots, numChars, glyphWidthFrac, bandHalf, ci);

        // finite-difference bump: cylinder tangent frame is closed-form —
        // angular tangent and vertical tangent — so no derivatives needed.
        float dA = 0.4 / max(totalSlots * 5.0, 8.0);
        float dH = 0.4 * (2.0 * bandHalf) / 7.0;
        int ciDum;
        float maskA = glyphMaskAt(angleNorm + dA, height, totalSlots, numChars, glyphWidthFrac, bandHalf, ciDum);
        float maskH = glyphMaskAt(angleNorm, height + dH, totalSlots, numChars, glyphWidthFrac, bandHalf, ciDum);
        float gradA = maskA - mask;
        float gradH = maskH - mask;

        vec3 tanA = normalize(vec3(-sin(theta), 0.0, cos(theta)));
        vec3 tanH = vec3(0.0, 1.0, 0.0);
        float reliefDepth = embossAmount * (0.82 + 0.30 * bassP);
        vec3 bumpN = normalize(n - reliefDepth * 14.0 * (gradA * tanA + gradH * tanH));

        // --- chrome material: fresnel + two specular light directions ------
        vec3 viewDir = normalize(ro - hitP);
        float fres = pow(1.0 - clamp(dot(bumpN, viewDir), 0.0, 1.0), 1.7);
        vec3 lightA = normalize(vec3(0.55, 0.85, 0.35));
        vec3 lightB = normalize(vec3(-0.55, -0.25, 0.65));
        vec3 refl = reflect(-viewDir, bumpN);
        float specA = pow(max(dot(refl, lightA), 0.0), 20.0);
        float specB = pow(max(dot(refl, lightB), 0.0), 34.0);
        float ripple = 0.12 * midP * sin(theta * 9.0 + height * 6.0 + t * 0.8);
        // brushed-chrome streak detail — cheap, high-frequency, gives the
        // metal surface fine tooling marks instead of a flat gradient
        float streak = 0.13 * sin(height * 95.0 + theta * 2.0) + 0.11 * sin(theta * 60.0 - height * 3.0);
        float chromeLuma = 0.22 + 0.62 * fres + 0.75 * specA + 1.05 * specB + ripple + streak;
        vec3 chromeCol = vec3(clamp(chromeLuma, 0.0, 1.6));

        // optional equirectangular "reflection" of the user image, gated ----
        if (texMix > 0.001) {
            float eu = atan(refl.z, refl.x) / TWO_PI + 0.5;
            float ev = acos(clamp(refl.y, -1.0, 1.0)) / PI;
            vec3 envCol = texture2D(inputImage, vec2(eu, ev)).rgb;
            chromeCol = mix(chromeCol, chromeCol * 0.45 + envCol * 1.15, texMix * (0.35 + 0.65 * chromeAmount));
        }

        // cohesive two-hue tint on the chrome itself (not just the letters)
        vec3 chromeTint = mix(neonColorA.rgb, neonColorB.rgb, 0.5 + 0.5 * sin(dot(bumpN, vec3(1.3, 2.1, 1.7))));
        chromeCol = mix(chromeCol, chromeCol * chromeTint * 1.25, 0.16 * chromeAmount);

        // --- neon emissive letterforms --------------------------------------
        float ciHash = hash(float(ci) * 12.9898 + 4.0);
        float idleShimmer = 0.85 + 0.15 * sin(t * 1.1 + ciHash * 18.0);
        float sparkGate = smoothstep(0.965 - 0.30 * highP, 0.985 - 0.30 * highP, ciHash);
        float sparkle = sparkGate * highP * (0.6 + 0.6 * sin(t * 6.0 + ciHash * 40.0));

        vec3 neonMix = mix(neonColorA.rgb, neonColorB.rgb, ciHash);
        // Continuous band-following glow (reads on beatless material) + capped
        // beat/punch flashes that decay with the host envelopes.
        vec3 emissive = neonMix * mask * idleShimmer
                      * (0.95 + 0.45 * bassP + 0.25 * midP
                         + 0.35 * pow(beatPulse, 1.4) + 0.22 * pow(punch, 1.5));
        emissive += vec3(1.0) * mask * sparkle;

        vec3 col = mix(chromeCol, emissive, clamp(mask * 1.05, 0.0, 1.0));
        // neon rim bleed on the silhouette, independent of letterforms —
        // this is what keeps the cylinder's edge reading crisp against black
        col += neonColorB.rgb * fres * 0.22 * (0.55 + 0.5 * bassP);
        // beat flash — a brief, eased whole-surface brightening (never a
        // per-frame strobe: audioBeatPulse arrives pre-enveloped by the host),
        // plus a smooth bass/mid breathing lift so ambient swells stay visible
        col *= (1.0 + 0.16 * pow(beatPulse, 1.4) + 0.16 * bassP + 0.09 * midP);
        // faint hologram-style scanlines — cheap fine-frequency detail that
        // reads as a chrome/CRT surface rather than a flat gradient
        col *= (1.0 - 0.16 * (0.5 + 0.5 * sin(gl_FragCoord.y * 1.5)));

        outCol = col;
        outAlpha = 1.0;
    } else if (transparentBg) {
        outAlpha = 0.0;
    } else {
        // very faint neon atmosphere near screen center — background stays
        // overwhelmingly black/near-black per house taste
        float glow = exp(-6.0 * dot(ndc, ndc)) * 0.05 * (0.4 + 0.6 * drive);
        vec3 atmo = mix(neonColorA.rgb, neonColorB.rgb, 0.5 + 0.5 * sin(t * 0.15)) * glow;
        outCol = bgColor.rgb + atmo;
        outAlpha = transparentBg ? 0.0 : 1.0;
    }

    gl_FragColor = vec4(ucApply(clamp(outCol, 0.0, 4.0)), clamp(outAlpha, 0.0, 1.0));
}
