/*{
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "DESCRIPTION": "Glitch / Datamosh after Rosa Menkman, Takeshi Murata's Monster Movie (2005), and JODI — persistent frame-feedback buffer pulled along motion vectors so I-frames seem to never refresh, plus per-row UV displacement, RGB channel split, 8×8 DCT-block quantize corruption, and burst rainbow garbage on bass kicks. The signal dies in interesting ways.",
  "INPUTS": [
    { "NAME": "moshDirection", "LABEL": "Mosh Direction", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 0.0 },
    { "NAME": "moshStrength", "LABEL": "Mosh Strength", "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "moshPersistence", "LABEL": "Mosh Persistence", "TYPE": "float", "MIN": 0.85, "MAX": 0.999, "DEFAULT": 0.94 },
    { "NAME": "tearAmp", "LABEL": "Tear Amount", "TYPE": "float", "MIN": 0.0, "MAX": 0.20, "DEFAULT": 0.06 },
    { "NAME": "rowDensity", "LABEL": "Row Density", "TYPE": "float", "MIN": 4.0, "MAX": 60.0, "DEFAULT": 24.0 },
    { "NAME": "chroma", "LABEL": "Chroma Split", "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.014 },
    { "NAME": "blockSize", "LABEL": "DCT Block Size", "TYPE": "float", "MIN": 4.0, "MAX": 32.0, "DEFAULT": 8.0 },
    { "NAME": "blockCorruption", "LABEL": "Block Corruption", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.40 },
    { "NAME": "burstProb", "LABEL": "Burst Probability", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.10 },
    { "NAME": "freezeChance", "LABEL": "Freeze Chance", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.05 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "resetField", "LABEL": "Reset", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "moshBuf", "PERSISTENT": true },
    {}
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec3 quantize(vec3 c, float steps) {
    return floor(c * steps) / steps;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // ============= PASS 0 — moshBuf accumulation =============
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            vec3 init = (IMG_SIZE_inputTex.x > 0.0)
                      ? texture(inputTex, uv).rgb : vec3(0.5);
            gl_FragColor = vec4(init, 1.0);
            return;
        }

        // I-frame stutter — every ~2 sec, briefly hard-reset the moshBuf
        // to a fresh frame. Without this the buffer eventually saturates
        // and the canvas freezes into mush.
        if (fract(TIME * 0.5) < 0.02) {
            vec3 fresh0;
            if (IMG_SIZE_inputTex.x > 0.0) {
                fresh0 = texture(inputTex, uv).rgb;
            } else {
                fresh0 = vec3(
                    step(0.5, fract(uv.x * 5.0 + TIME * 0.30)),
                    step(0.5, fract(uv.y * 7.0 - TIME * 0.20)),
                    step(0.7, hash21(floor(uv * 40.0) + floor(TIME * 2.0))));
            }
            gl_FragColor = vec4(fresh0, 1.0);
            return;
        }

        // Datamosh proper: fetch previous frame at uv shifted along the
        // mosh direction (the synthesized motion vector). Since the
        // previous frame already contains the mosh, this compounds.
        vec2 mDir = vec2(cos(moshDirection), sin(moshDirection))
                  * moshStrength
                  * (1.0 + audioBass * audioReact * 1.8);
        vec3 prev = texture(moshBuf, uv - mDir).rgb;

        // Decide whether to accept new frame or freeze on previous.
        // Most cells refresh; a fraction "freeze" — that's the I-frame
        // skip artefact.
        float fr = hash21(floor(uv * vec2(rowDensity * 2.0, rowDensity)) + floor(TIME * 8.0));
        // Always-on baseline freeze probability so catastrophic-failure
        // mode is visible without audio; audio amplifies on top.
        bool freeze = fr < freezeChance * (0.3 + audioMid * audioReact + 0.3);

        vec3 fresh;
        if (IMG_SIZE_inputTex.x > 0.0) {
            fresh = texture(inputTex, uv).rgb;
        } else {
            // Fallback: scrolling stripes + flickering 8x8 blocks +
            // moving ring — much more glitchy structure than a smooth
            // gradient, so the datamosh artefacts have something to bite.
            vec3 stripes = vec3(
                step(0.5, fract(uv.x * 5.0 + TIME * 0.30)),
                step(0.5, fract(uv.y * 7.0 - TIME * 0.20)),
                step(0.7, hash21(floor(uv * 40.0) + floor(TIME * 2.0))));
            float ang = atan(uv.y - 0.5, uv.x - 0.5);
            vec3 ring = vec3(0.5 + 0.5 * cos(ang * 3.0 + TIME * 1.7),
                             0.5 + 0.5 * sin(length(uv - 0.5) * 12.0 - TIME * 0.7),
                             0.5);
            fresh = mix(ring, stripes, 0.45);
        }

        vec3 outC = freeze
                  ? mix(prev, fresh, 0.05)
                  : mix(fresh, prev, moshPersistence);
        gl_FragColor = vec4(outC, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    // Per-row tearing — each horizontal row offset in x by hashed amount,
    // refreshed at 8 Hz. Bass amplifies tear.
    float rowH = 1.0 / max(rowDensity, 1.0);
    float rowId = floor(uv.y / rowH);
    float tBucket = floor(TIME * (4.0 + audioBass * audioReact * 12.0));
    float tear = (hash21(vec2(rowId, tBucket)) - 0.5)
               * tearAmp * (1.0 + audioMid * audioReact * 1.2);
    vec2 uvT = uv;
    uvT.x = fract(uvT.x + tear);

    // RGB channel split — chroma scales with audioHigh.
    float chr = chroma * (1.0 + audioHigh * audioReact * 2.0);
    float r = texture(moshBuf, uvT + vec2( chr, 0.0)).r;
    float g = texture(moshBuf, uvT).g;
    float b = texture(moshBuf, uvT - vec2( chr, 0.0)).b;
    vec3 col = vec3(r, g, b);

    // 8×8 DCT-block quantize corruption — random blocks get replaced
    // with their average plus aggressive quantization.
    float bs = max(blockSize, 1.0);
    vec2 blkPx = floor(gl_FragCoord.xy / bs) * bs;
    vec2 blkUV = blkPx / RENDERSIZE.xy;
    float blkRoll = hash21(blkPx + floor(TIME * 6.0));
    if (blkRoll < blockCorruption * (0.4 + audioLevel * audioReact)) {
        vec3 avg = texture(moshBuf, blkUV + vec2(bs, bs) / RENDERSIZE.xy * 0.5).rgb;
        col = quantize(avg, 4.0 + floor(blkRoll * 6.0));
    }

    // Burst — small random rectangles flash with hashed garbage values
    // on bass kicks. This is the catastrophic-failure mode.
    float burstRoll = hash21(blkPx * 1.3 + floor(TIME * 12.0));
    if (burstRoll < burstProb * (0.30 + audioBass * audioReact * 0.7)) {
        col = vec3(hash11(burstRoll * 1.7),
                   hash11(burstRoll * 3.7),
                   hash11(burstRoll * 7.3));
    }

    gl_FragColor = vec4(col, 1.0);
}
