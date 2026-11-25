/*{
  "DESCRIPTION": "City Skin — Shades of New York. A body tiled out of the city: every cell of your silhouette is a New York sign. Emoji (the Statue, yellow cabs, a pizza slice, bagels, hot dogs, pigeons, rats, the bridge, the subway car, the sax), subway bullets in their line colours, words off the street (NYC, BK, BX, QNS, 212, 718, $1, DELI, TAXI, WALK, EXIT, 24H) and signs (ONE WAY, the walk hand, stars, hearts, arrows), packed with flat blocks into a pixel mosaic. Light falls warm on one side (taxi yellow, tan, magenta) and cool on the other (subway blue and green), with black holes eaten through. Tiles flip on their own clock and on the beat, some merge into big 2x2 and 4x4 signs. Body: with the camera on, your own silhouette and shading drive it; without, a built-in bust breathes on screen. Touch to burst the tiles under your finger into big signs.",
  "CREDIT": "Easel / ShaderClaw house shader — Shades of New York series, 2026-11-25 — original artwork. Emoji glyphs from Noto Emoji (SIL OFL 1.1).",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Shades of New York", "Interactive", "Audio Reactive", "Body Tracking"],
  "INPUTS": [
    { "NAME": "cells", "LABEL": "Tiles Tall", "TYPE": "float", "MIN": 16.0, "MAX": 120.0, "DEFAULT": 46.0, "GROUP": "Mosaic" },
    { "NAME": "bigTiles", "LABEL": "Big Signs", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Mosaic" },
    { "NAME": "blocksAmt", "LABEL": "Flat Blocks", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Mosaic" },
    { "NAME": "emojiAmt", "LABEL": "Emoji", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Mosaic" },
    { "NAME": "subwayAmt", "LABEL": "Subway Bullets", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Mosaic" },
    { "NAME": "wordsAmt", "LABEL": "Street Words", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Mosaic" },
    { "NAME": "signsAmt", "LABEL": "Signs", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Mosaic" },
    { "NAME": "holes", "LABEL": "Black Holes", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Mosaic" },
    { "NAME": "changeRate", "LABEL": "Flip Speed", "TYPE": "float", "MIN": 0.0, "MAX": 4.0, "DEFAULT": 0.7, "GROUP": "Mosaic" },
    { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "VALUES": [0, 1, 2, 3], "LABELS": ["Taxi Night", "Subway Lines", "Neon", "Newsprint"], "DEFAULT": 0, "GROUP": "Colour" },
    { "NAME": "bgGrid", "LABEL": "Background Grid", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Colour" },
    { "NAME": "bodyMode", "LABEL": "Body", "TYPE": "long", "VALUES": [0, 1, 2, 3], "LABELS": ["Auto", "Built-in Bust", "Camera Body", "Full Frame"], "DEFAULT": 0, "GROUP": "Body" },
    { "NAME": "bodyMirror", "LABEL": "Mirror (selfie view)", "TYPE": "bool", "DEFAULT": true, "GROUP": "Body" },
    { "NAME": "camTex", "LABEL": "Image / Camera", "TYPE": "image", "GROUP": "Body" },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Audio" }
  ],
  "IMPORTED": {
    "glyphs": { "PATH": "ny_city_skin_glyphs.png" }
  }
}*/

// glyphs: 8x8 tiles, R = glyph mask, G = colour hint * 20 (subway line). Rows from the top of the PNG.
//   0-27 emoji · 28-43 subway bullets · 44-57 street words · 58-63 signs

float h1(float n) { return fract(sin(n) * 43758.5453); }
float h2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float h3(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h2(i), h2(i + vec2(1, 0)), f.x), mix(h2(i + vec2(0, 1)), h2(i + vec2(1, 1)), f.x), f.y);
}

// ── Body (MediaPipe, ShaderClaw contract) ──
vec2 bodyCamUV(vec2 uv) { return vec2(bodyMirror ? 1.0 - uv.x : uv.x, uv.y); }
float bodySeg(vec2 uv) {
    vec2 s = vec2(bodyMirror ? 1.0 - uv.x : uv.x, 1.0 - uv.y);
    return smoothstep(0.35, 0.65, texture2D(mpSegMask, clamp(s, 0.0, 1.0)).r);
}
vec3 bodyJoint(float i) {
    vec4 lm = texture2D(mpPoseLandmarks, vec2((i + 0.5) / 33.0, 0.5));
    return vec3(bodyMirror ? 1.0 - lm.r : lm.r, lm.g, lm.a * step(0.002, lm.r + lm.g));   // unfed = no joint
}
float bodyPresence() {
    float v = bodyJoint(11.0).z + bodyJoint(12.0).z + bodyJoint(0.0).z;
    return smoothstep(1.0, 2.2, v);
}

// ── Built-in bust: head, neck, shoulders (p: screen-height units, centred) ──
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }
float sdRBox(vec2 p, vec2 b, float r) { vec2 q = abs(p) - b + r; return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r; }
float bust(vec2 p) {
    float t = TIME;
    p.x -= sin(t * 0.37) * 0.012;
    p.y -= sin(t * 0.9) * 0.004;                                   // breathing
    vec2 hp = p - vec2(sin(t * 0.23) * 0.015, 0.12);
    float ca = cos(sin(t * 0.31) * 0.06), sa = sin(sin(t * 0.31) * 0.06);
    hp = mat2(ca, -sa, sa, ca) * hp;
    float head = (length(hp / vec2(0.17, 0.215)) - 1.0) * 0.17;
    float jaw = (length((hp - vec2(0.0, -0.1)) / vec2(0.12, 0.12)) - 1.0) * 0.12;
    head = smin(head, jaw, 0.04);
    float neck = sdRBox(p - vec2(0.0, -0.17), vec2(0.085, 0.08), 0.03);
    vec2 q = p - vec2(0.0, -0.53); q.y += 0.3 * max(abs(q.x) - 0.1, 0.0);
    float sh = sdRBox(q, vec2(0.46, 0.3), 0.16);
    return smin(smin(head, neck, 0.05), sh, 0.07);
}

// ── Palettes: 8 colours ordered cool/dark → warm/bright ──
vec3 palCol(float i, float pal) {
    if (pal < 0.5) {            // Taxi Night (the reference: blue and green in shadow, yellow, tan and magenta in light)
        if (i < 0.5) return vec3(0.02, 0.03, 0.03);
        if (i < 1.5) return vec3(0.12, 0.36, 0.55);
        if (i < 2.5) return vec3(0.30, 0.76, 0.92);
        if (i < 3.5) return vec3(0.20, 0.52, 0.32);
        if (i < 4.5) return vec3(0.42, 0.93, 0.36);
        if (i < 5.5) return vec3(0.85, 0.72, 0.52);
        if (i < 6.5) return vec3(0.95, 0.28, 0.62);
        return vec3(0.97, 0.93, 0.18);
    } else if (pal < 1.5) {     // Subway Lines
        if (i < 0.5) return vec3(0.04);
        if (i < 1.5) return vec3(0.0, 0.22, 0.65);
        if (i < 2.5) return vec3(0.0, 0.58, 0.24);
        if (i < 3.5) return vec3(0.73, 0.2, 0.68);
        if (i < 4.5) return vec3(0.65, 0.66, 0.67);
        if (i < 5.5) return vec3(0.93, 0.21, 0.18);
        if (i < 6.5) return vec3(1.0, 0.39, 0.1);
        return vec3(0.99, 0.8, 0.04);
    } else if (pal < 2.5) {     // Neon
        if (i < 0.5) return vec3(0.02, 0.0, 0.05);
        if (i < 1.5) return vec3(0.25, 0.05, 0.6);
        if (i < 2.5) return vec3(0.1, 0.3, 1.0);
        if (i < 3.5) return vec3(0.0, 0.85, 0.95);
        if (i < 4.5) return vec3(1.0, 0.1, 0.55);
        if (i < 5.5) return vec3(1.0, 0.35, 0.15);
        if (i < 6.5) return vec3(0.75, 1.0, 0.2);
        return vec3(1.0, 0.95, 0.6);
    }
    // Newsprint
    float g = i / 7.0;
    return i > 5.5 && i < 6.5 ? vec3(0.85, 0.12, 0.1) : vec3(0.06 + 0.86 * g * g, 0.06 + 0.84 * g * g, 0.05 + 0.78 * g * g);
}
vec3 lineCol(float c) {         // MTA line colours for the bullets
    if (c < 1.5) return vec3(0.0, 0.22, 0.65);
    if (c < 2.5) return vec3(0.93, 0.21, 0.18);
    if (c < 3.5) return vec3(0.0, 0.58, 0.24);
    if (c < 4.5) return vec3(0.73, 0.2, 0.68);
    if (c < 5.5) return vec3(0.99, 0.8, 0.04);
    if (c < 6.5) return vec3(1.0, 0.39, 0.1);
    if (c < 7.5) return vec3(0.65, 0.66, 0.67);
    return vec3(0.42, 0.75, 0.27);
}
vec2 glyphAt(float g, vec2 f) {   // returns (mask, colour hint)
    vec2 tuv = (vec2(mod(g, 8.0), floor(g / 8.0)) + vec2(mix(0.03, 0.97, f.x), mix(0.97, 0.03, f.y))) / 8.0;
    vec4 s = texture2D(glyphs, tuv);
    return vec2(s.r, floor(s.g * 255.0 / 20.0 + 0.5));
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// body + light at a point in screen uv; returns (mask, light 0..1)
vec2 field(vec2 uv, float aspect, float mode, float pres) {
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);
    float d = bust(p);
    vec2 e = vec2(0.006, 0.0);
    vec2 gr = normalize(vec2(bust(p + e.xy) - d, bust(p + e.yx) - d) + 1e-5);
    float rim = 1.0 - smoothstep(0.0, 0.14, -d);
    float lit = 0.5 + 0.5 * dot(gr, normalize(vec2(1.0, 0.35))) * (0.35 + 0.65 * rim) + 0.15 * (p.x / 0.4);
    lit = max(lit, 0.14);                                          // the shadow side stays blue, not black
    // eyes and mouth: shadow pockets on the built-in face
    vec2 hp = p - vec2(sin(TIME * 0.23) * 0.015 + sin(TIME * 0.37) * 0.012, 0.12);
    float feat = smoothstep(0.035, 0.015, length((vec2(abs(hp.x), hp.y) - vec2(0.064, 0.02)) * vec2(1.0, 1.6)));
    feat = max(feat, smoothstep(0.03, 0.012, length((hp - vec2(0.0, -0.085)) * vec2(0.8, 2.6))));
    lit = mix(lit, 0.02, feat * 0.85);
    vec2 bustF = vec2(1.0 - smoothstep(-0.004, 0.004, d), clamp(lit, 0.0, 1.0));
    if (mode > 2.5) return vec2(1.0, clamp(0.35 + 0.5 * vnoise(uv * 3.0 + TIME * 0.05) + 0.2 * (uv.x - 0.5), 0.0, 1.0));
    vec2 camF = vec2(bodySeg(uv), clamp(lum(texture2D(camTex, bodyCamUV(uv)).rgb) * 1.4 - 0.05, 0.0, 1.0));
    if (mode > 1.5) return camF;
    if (mode > 0.5) return bustF;
    return mix(bustF, camF, pres);
}

void main() {
    vec2 R = RENDERSIZE.xy;
    float aspect = R.x / R.y;
    float bass = clamp(audioBass, 0.0, 1.0) * audioReact;
    float high = clamp(audioHigh, 0.0, 1.0) * audioReact;
    float beat = clamp(audioBeatPulse, 0.0, 1.0) * audioReact;
    float mode = float(bodyMode), pal = float(palette);
    float pres = mode < 0.5 ? bodyPresence() : 0.0;

    float N = floor(cells);
    vec2 px = gl_FragCoord.xy / R.y * N;
    vec2 cid = floor(px), fr = fract(px);
    vec2 cuv = (cid + 0.5) / N; cuv.x /= aspect;                    // cell centre in screen uv
    vec2 F = field(cuv, aspect, mode, pres);

    // touch: cells under the finger burst into big signs
    vec2 m = mousePos * vec2(aspect, 1.0);
    float touch = mouseDown > 0.5 ? smoothstep(0.22, 0.0, length(cuv * vec2(aspect, 1.0) - m)) : 0.0;

    // merge cells into 4x4 and 2x2 big tiles
    float size = 1.0; vec2 bid = cid;
    vec2 b4 = floor(cid / 4.0), b2 = floor(cid / 2.0);
    float ep4 = floor(TIME * changeRate * 0.15 + h2(b4) * 5.0);
    if (h3(vec3(b4, ep4)) < bigTiles * 0.22 + touch * 0.6) { size = 4.0; bid = b4; }
    else if (h3(vec3(b2, 7.0 + floor(TIME * changeRate * 0.25 + h2(b2) * 5.0))) < bigTiles * 0.45 + touch) { size = 2.0; bid = b2; }
    vec2 f = (cid - bid * size + fr) / size;
    vec2 buv = (bid * size + size * 0.5) / N; buv.x /= aspect;
    vec2 FB = size > 1.0 ? field(buv, aspect, mode, pres) : F;

    // the tile's own clock, plus a beat reshuffle
    vec3 key = vec3(bid, size);
    float kh = h3(key);
    float ep = floor(TIME * changeRate * (0.35 + kh) + kh * 9.0);
    float seed = h3(key + ep * 1.37);
    if (h1(kh * 91.0 + floor(TIME * 2.0)) < beat * 0.35) seed = fract(seed + 0.37);

    // colour: light picks the band, slow noise patches it (neighbours share colours)
    float nz = vnoise(bid * size * 0.13 + vec2(TIME * 0.05, 0.0));
    float v = 0.1 + FB.y * 0.9 + (nz - 0.5) * 0.55 + (seed - 0.5) * 0.08 + bass * 0.12;
    float ci = floor(clamp(v, 0.0, 0.999) * 7.0) + 1.0;              // 1..7, black is reserved for holes
    vec3 bg = palCol(ci, pal);
    vec3 alt = palCol(mod(ci + 2.0 + floor(seed * 4.0), 7.0) + 1.0, pal);

    // holes: dark patches eaten through, shrinking on the bass
    float hn = vnoise(cid * 0.19 + vec2(3.0, TIME * 0.07)) * 0.7 + vnoise(cid * 0.47 - TIME * 0.05) * 0.3;
    float hole = step(1.0 - holes * 0.55 + bass * 0.12, hn) + step(FB.y, 0.06);

    // what goes in the tile
    float wB = blocksAmt, wE = emojiAmt, wS = subwayAmt, wW = wordsAmt, wG = signsAmt;
    if (size > 1.0) wB *= 0.15;                                      // big tiles are always signs
    float tot = wB + wE + wS + wW + wG + 1e-4;
    float r = h1(seed * 17.3) * tot;
    float sel = h1(seed * 41.9);
    vec3 col = bg;
    if (r < wB) {
        // flat block with a street pattern: solid, stripes, half, checks, dot
        float k = floor(sel * 5.0);
        float pat = 0.0;
        if (k < 0.5) pat = 0.0;
        else if (k < 1.5) pat = step(0.55, fract(f.x * 4.0)) * step(0.15, f.y) * step(f.y, 0.85);
        else if (k < 2.5) pat = step(f.x, f.y);
        else if (k < 3.5) pat = mod(floor(f.x * 2.0) + floor(f.y * 2.0), 2.0);
        else pat = step(length(f - 0.5), 0.22);
        col = mix(bg, alt * 0.75, pat);
    } else {
        float g;
        r -= wB;
        if (r < wE) g = floor(sel * 28.0);
        else if (r < wE + wS) g = 28.0 + floor(sel * 16.0);
        else if (r < wE + wS + wW) g = 44.0 + floor(sel * 14.0);
        else g = 58.0 + floor(sel * 6.0);
        vec2 gl = glyphAt(g, f);
        vec3 ink = lum(bg) > 0.45 ? mix(vec3(0.02), alt * 0.35, step(0.5, seed)) : mix(alt, vec3(1.0), 0.25);
        if (gl.y > 0.5) {                      // subway bullet: line colour disc, white letter
            float disc = step(length(f - 0.5), 0.44);
            col = mix(bg * 0.25, mix(vec3(1.0), lineCol(gl.y), gl.x), disc);
            if (pal > 2.5) col = mix(bg * 0.3, mix(vec3(0.95), vec3(0.05), gl.x), disc);
        } else {
            col = mix(bg, ink, gl.x);
        }
    }
    // sparkle on the highs: a few tiles flash inverted
    if (h1(kh * 13.0 + floor(TIME * 12.0)) < high * 0.06) col = 1.0 - col;
    col *= 1.0 + beat * 0.25;
    col = mix(col, vec3(0.012, 0.014, 0.01), clamp(hole, 0.0, 1.0) * (1.0 - touch));

    // thin seams between tiles
    vec2 sf = fract(px);
    col *= 0.86 + 0.14 * step(0.06, min(sf.x, sf.y));

    // background: dark board with a dot grid and ghost glyphs
    vec3 back = vec3(0.025, 0.03, 0.018);
    float dots = smoothstep(0.16, 0.08, length(fract(px * 2.0) - 0.5));
    back += dots * 0.045 * bgGrid;
    float gg = floor(h2(cid + 0.5) * 64.0);
    back += glyphAt(gg, fr).x * 0.035 * bgGrid * step(0.7, h2(cid * 1.3 + floor(TIME * 0.2)));
    col = mix(back, col, F.x);

    gl_FragColor = vec4(col, 1.0);
}
