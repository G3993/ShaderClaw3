/*{
  "DESCRIPTION": "Stitch City — Shades of New York. New York embroidered on black felt: blocky patches of felt and thread that drift and re-stitch themselves, each patch sewn with its own motif. Cross-stitch X's, plus-stitch lattices, french knots, hex rings, starbursts and satin stripes, and rows of New York in thread: the Statue, yellow cabs, pizza, bagels, pretzels, dumplings, hot dogs, pigeons, rats, the bridge, the sax, the subway car, cop cars, sneakers, subway bullets in their line colours, NYC / BK / BX / QNS / SOHO / DUMBO / LES / UWS / 212 / 718 / $1 / BODEGA / DELI / TAXI / WALK / HEY!, ONE WAY arrows and the walk hand. Felt paths snake between the patches like subway lines. Compositions: Manhattan (the island turned on its grid, Central Park sewn in green, the rivers in running stitch), your Body from the camera, a stitched Camera Portrait of you in the palette, or Full Frame. Audio: threads swell on the bass, patches re-stitch on the beat, highs glint on the thread. Touch: a white stitched ring follows your finger and turns the patches under it into big New York motifs.",
  "CREDIT": "Easel / ShaderClaw house shader — Shades of New York series, 2026-11-25 — original artwork. Emoji glyphs from Noto Emoji (SIL OFL 1.1).",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Shades of New York", "Interactive", "Audio Reactive", "Body Tracking"],
  "INPUTS": [
    { "NAME": "composition", "LABEL": "Composition", "TYPE": "long", "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto", "Manhattan", "Body (camera)", "Camera Portrait", "Full Frame"], "DEFAULT": 0, "GROUP": "Composition" },
    { "NAME": "cells", "LABEL": "Stitches Tall", "TYPE": "float", "MIN": 16.0, "MAX": 100.0, "DEFAULT": 44.0, "GROUP": "Stitch" },
    { "NAME": "patch", "LABEL": "Patch Size", "TYPE": "float", "MIN": 2.0, "MAX": 10.0, "DEFAULT": 4.5, "GROUP": "Stitch" },
    { "NAME": "nyAmt", "LABEL": "NY Symbols", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Stitch" },
    { "NAME": "stitchAmt", "LABEL": "Stitch Patterns", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Stitch" },
    { "NAME": "feltAmt", "LABEL": "Felt Blocks", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Stitch" },
    { "NAME": "pathAmt", "LABEL": "Felt Paths", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Stitch" },
    { "NAME": "drift", "LABEL": "Drift", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.35, "GROUP": "Motion" },
    { "NAME": "changeRate", "LABEL": "Re-stitch Speed", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.5, "GROUP": "Motion" },
    { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "VALUES": [0, 1, 2, 3], "LABELS": ["Felt Box", "Subway Lines", "Neon Bodega", "Brownstone"], "DEFAULT": 0, "GROUP": "Colour" },
    { "NAME": "grain", "LABEL": "Felt Texture", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Colour" },
    { "NAME": "river", "LABEL": "River Stitches", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Colour" },
    { "NAME": "bodyMirror", "LABEL": "Mirror (selfie view)", "TYPE": "bool", "DEFAULT": true, "GROUP": "Camera" },
    { "NAME": "camTex", "LABEL": "Image / Camera", "TYPE": "image", "GROUP": "Camera" },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Audio" }
  ],
  "IMPORTED": {
    "atlas": { "PATH": "ny_stitch_city_atlas.png" }
  }
}*/

// atlas: 8x12 tiles, R = glyph mask, G = colour hint * 20 (subway line), B = Manhattan over the whole sheet
//   0-27 emoji · 28-43 subway bullets · 44-57 words · 58-63 signs · 64-83 more emoji · 84-91 more words · 92-95 more signs
const float PARK_TREE = 74.0;

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
float joint(float i) {
    vec4 lm = texture2D(mpPoseLandmarks, vec2((i + 0.5) / 33.0, 0.5));
    return lm.a * step(0.002, lm.r + lm.g);                     // unfed = no joint
}
float bodyPresence() { return smoothstep(1.0, 2.2, joint(0.0) + joint(11.0) + joint(12.0)); }

// ── Palettes: ten felts and threads each ──
vec3 sel10(float i, vec3 a, vec3 b, vec3 c, vec3 d, vec3 e, vec3 f, vec3 g, vec3 h, vec3 k, vec3 l) {
    if (i < 0.5) return a; if (i < 1.5) return b; if (i < 2.5) return c; if (i < 3.5) return d; if (i < 4.5) return e;
    if (i < 5.5) return f; if (i < 6.5) return g; if (i < 7.5) return h; if (i < 8.5) return k; return l;
}
vec3 palC(float i, float pal) {
    if (pal < 0.5) return sel10(i, vec3(0.96, 0.89, 0.23), vec3(0.91, 0.19, 0.16), vec3(0.12, 0.34, 0.88), vec3(0.94, 0.54, 0.18), vec3(0.92, 0.66, 0.71),
                                   vec3(0.73, 0.89, 0.71), vec3(0.50, 0.16, 0.55), vec3(0.18, 0.50, 0.18), vec3(0.95, 0.94, 0.90), vec3(0.31, 0.83, 0.88));
    if (pal < 1.5) return sel10(i, vec3(0.0, 0.22, 0.65), vec3(0.93, 0.21, 0.18), vec3(0.0, 0.58, 0.24), vec3(0.73, 0.2, 0.68), vec3(0.99, 0.8, 0.04),
                                   vec3(1.0, 0.39, 0.1), vec3(0.65, 0.66, 0.67), vec3(0.42, 0.75, 0.27), vec3(0.6, 0.4, 0.13), vec3(0.95, 0.95, 0.93));
    if (pal < 2.5) return sel10(i, vec3(1.0, 0.15, 0.6), vec3(0.0, 0.9, 1.0), vec3(0.7, 1.0, 0.15), vec3(1.0, 0.92, 0.2), vec3(1.0, 0.45, 0.1),
                                   vec3(0.5, 0.2, 1.0), vec3(1.0, 0.12, 0.2), vec3(1.0, 1.0, 0.95), vec3(0.15, 0.35, 1.0), vec3(1.0, 0.6, 0.85));
    return sel10(i, vec3(0.62, 0.25, 0.18), vec3(0.42, 0.26, 0.16), vec3(0.82, 0.68, 0.5), vec3(0.93, 0.88, 0.76), vec3(0.35, 0.4, 0.45),
                    vec3(0.2, 0.36, 0.28), vec3(0.72, 0.38, 0.16), vec3(0.85, 0.66, 0.25), vec3(0.55, 0.7, 0.8), vec3(0.95, 0.93, 0.88));
}
vec3 lineCol(float c) {
    if (c < 1.5) return vec3(0.0, 0.22, 0.65);
    if (c < 2.5) return vec3(0.93, 0.21, 0.18);
    if (c < 3.5) return vec3(0.0, 0.58, 0.24);
    if (c < 4.5) return vec3(0.73, 0.2, 0.68);
    if (c < 5.5) return vec3(0.99, 0.8, 0.04);
    if (c < 6.5) return vec3(1.0, 0.39, 0.1);
    if (c < 7.5) return vec3(0.65, 0.66, 0.67);
    return vec3(0.42, 0.75, 0.27);
}

vec4 glyphTex(float g, vec2 f) {
    vec2 tuv = (vec2(mod(g, 8.0), floor(g / 8.0)) + vec2(mix(0.04, 0.96, f.x), mix(0.96, 0.04, f.y))) / vec2(8.0, 12.0);
    return texture2D(atlas, tuv);
}
// Manhattan: 1 land, 0.5 Central Park, 0 water. p in screen-height units, centred.
float manhattan(vec2 p, float aspect) {
    if (aspect > 1.15) {                                   // landscape: lay the island across the screen
        float a = 0.62, c = cos(a), sn = sin(a);
        p = mat2(c, sn, -sn, c) * p / min(aspect * 0.62, 1.45);
    }
    vec2 m = p / vec2(512.0 / 768.0, 1.0) / 0.98 + 0.5;
    if (m.x < 0.0 || m.x > 1.0 || m.y < 0.0 || m.y > 1.0) return 0.0;
    return texture2D(atlas, vec2(m.x, 1.0 - m.y)).b;
}

// patches: blocky Voronoi on the stitch grid (half Manhattan, half Chebyshev distance), seeds drift
vec2 regionAt(vec2 c, float G, float t) {
    vec2 g = floor(c / G), id = g; float best = 1e9;
    for (int j = -1; j <= 1; j++) for (int i = -1; i <= 1; i++) {
        vec2 q = g + vec2(float(i), float(j));
        float hh = h2(q);
        vec2 s = (q + 0.5 + 0.4 * vec2(sin(t + hh * 6.283), cos(t * 0.8 + hh * 11.0))) * G;
        vec2 d = abs(c + 0.5 - s);
        float m = (d.x + d.y) * 0.5 + max(d.x, d.y) * 0.5 + hh * 0.01;
        if (m < best) { best = m; id = q; }
    }
    return id;
}

// stitch shapes: x = distance to the thread's centreline, y = half width (cell units)
float sdHex(vec2 p, float r) { p = abs(p); return max(dot(p, vec2(0.866, 0.5)), p.y) - r; }
vec2 stitch(float k, vec2 f, float thick) {
    vec2 q = f - 0.5; float box = max(abs(q.x), abs(q.y));
    if (k < 0.5) return vec2(box < 0.4 ? min(abs(q.x - q.y), abs(q.x + q.y)) * 0.7071 : 1.0, 0.075 * thick);    // cross-stitch X
    if (k < 1.5) return vec2(min(abs(q.y) < 0.38 ? abs(q.x) : 1.0, abs(q.x) < 0.38 ? abs(q.y) : 1.0), 0.07 * thick); // plus
    if (k < 2.5) return vec2(length(q), 0.21 * thick);                                                            // french knot
    if (k < 3.5) return vec2(abs(sdHex(q, 0.27)), 0.055 * thick);                                                  // hex ring
    if (k < 4.5) { float r = length(q), a = atan(q.y, q.x);                                                       // starburst
                   return vec2(r < 0.44 ? abs(sin(a * 4.0)) * r : 1.0, (0.06 - r * 0.1) * thick + 0.012); }
    if (k < 5.5) return vec2(abs(q.y) < 0.42 ? abs(fract(f.x * 3.0) - 0.5) / 3.0 : 1.0, 0.1 * thick);              // satin stripes
    return vec2(min(abs(q.x), abs(q.y)), 0.045 * thick);                                                          // running lattice
}
// a thread: colour with a rounded, twisted, lit body; a = coverage
vec4 thread(vec2 sd, vec3 tc, vec2 f, float aa, float glint) {
    float cov = 1.0 - smoothstep(sd.y - aa, sd.y + aa * 0.5, sd.x);
    float sh = sqrt(max(0.0, 1.0 - sd.x * sd.x / (sd.y * sd.y + 1e-6)));
    float twist = 0.84 + 0.16 * sin((f.x + f.y) * 62.0);
    vec3 c = tc * (0.4 + 0.8 * sh) * twist + pow(sh, 6.0) * (0.18 + glint) * vec3(1.0);
    return vec4(c, cov);
}

void main() {
    vec2 R = RENDERSIZE.xy;
    float aspect = R.x / R.y;
    float bass = clamp(audioBass, 0.0, 1.0) * audioReact;
    float high = clamp(audioHigh, 0.0, 1.0) * audioReact;
    float beat = clamp(audioBeatPulse, 0.0, 1.0) * audioReact;
    float pal = float(palette);
    float mode = float(composition);
    if (mode < 0.5) mode = bodyPresence() > 0.5 ? 2.0 : 1.0;         // Auto: your body when the camera sees you, else Manhattan

    float N = floor(cells);
    float cellPx = R.y / N, aa = 1.2 / cellPx;
    vec2 px = gl_FragCoord.xy / R.y * N;
    vec2 cid = floor(px), f = fract(px);
    vec2 cuv = (cid + 0.5) / N; cuv.x /= aspect;
    vec2 cp = (cuv - 0.5) * vec2(aspect, 1.0);

    // felt grain, shared by every surface
    float gn = vnoise(gl_FragCoord.xy * 0.55) * 0.55 + vnoise(gl_FragCoord.xy * 1.9) * 0.45;
    float felt = 1.0 + (gn - 0.5) * 0.45 * grain;

    // where the embroidery goes
    float mask = 1.0, park = 0.0;
    vec3 cam = texture2D(camTex, bodyCamUV(cuv)).rgb;
    if (mode < 1.5) { float mb = manhattan(cp, aspect); mask = step(0.3, mb); park = mask * step(mb, 0.75); }
    else if (mode < 2.5) mask = step(0.5, bodySeg(cuv));

    // touch: patches under the finger become big NY motifs, a white stitched ring follows it
    vec2 mp = (mousePos - 0.5) * vec2(aspect, 1.0);
    float tdist = length(cp - mp);
    float touch = mouseDown > 0.5 ? smoothstep(0.24, 0.0, tdist) : 0.0;

    vec3 col = vec3(0.028, 0.028, 0.03) * (0.75 + 0.5 * gn);          // black felt
    // rivers: running stitch around the island
    if (mode < 1.5 && mask < 0.5) {
        float row = mod(cid.y, 3.0);
        float dash = step(0.45, fract(px.x * 0.5 - TIME * 0.15 + h1(cid.y) * 3.0));
        vec4 th = thread(vec2(abs(f.y - 0.5), 0.06), vec3(0.18, 0.36, 0.7), f, aa, 0.0);
        col = mix(col, th.rgb, th.a * dash * step(row, 0.5) * river * 0.6);
    }

    if (mask > 0.5) {
        float t = TIME * drift * 0.4;
        vec2 rid = regionAt(cid, patch, t);
        float rh = h2(rid * 1.17 + 3.0);
        float ep = floor(TIME * changeRate * (0.25 + rh * 0.5) + rh * 7.0);
        if (h1(rh * 77.0 + floor(TIME * 2.0)) < beat * 0.3) ep += 1.0;
        float sd = h3(vec3(rid, ep));
        float sd2 = h3(vec3(rid + 9.1, ep));

        // style: felt block · felt path · stitch pattern · NY motif
        float wF = feltAmt, wP = pathAmt, wS = stitchAmt, wN = nyAmt + touch * 4.0;
        float tot = wF + wP + wS + wN + 1e-4;
        float r = sd * tot;
        float style = r < wF ? 0.0 : r < wF + wP ? 1.0 : r < wF + wP + wS ? 2.0 : 3.0;
        if (park > 0.5) style = sd < 0.6 ? 3.0 : 0.0;

        float ci = floor(h1(sd * 13.1) * 10.0);
        float ti = mod(ci + 2.0 + floor(h1(sd * 29.3) * 7.0), 10.0);
        vec3 base = palC(ci, pal), tcol = palC(ti, pal);
        if (mode > 2.5 && mode < 3.5) {                                    // Camera Portrait: felts picked from your colours
            float bd = 1e9;
            for (int i = 0; i < 10; i++) { vec3 pc = palC(float(i), pal); float d = dot(pc - cam, pc - cam); if (d < bd) { bd = d; base = pc; ci = float(i); } }
            tcol = palC(mod(ci + 5.0, 10.0), pal);
            if (style == 1.0) style = 0.0;
        }
        if (park > 0.5) { base = vec3(0.16, 0.48, 0.2); tcol = vec3(0.55, 0.85, 0.35); }
        bool onFelt = h1(sd * 51.7) < 0.5 || style == 0.0 || (mode > 2.5 && mode < 3.5) || park > 0.5;
        vec3 under = onFelt ? base * felt : col;
        float thick = 1.0 + bass * 0.45;
        float glint = high * step(0.8, h2(cid + floor(TIME * 10.0)));

        if (style < 0.5) {
            col = base * felt;
        } else if (style < 1.5) {
            // felt path: only the patch's rim is felt, so paths snake between patches
            float edge = 0.0;
            if (regionAt(cid + vec2(1, 0), patch, t) != rid) edge = 1.0;
            if (regionAt(cid - vec2(1, 0), patch, t) != rid) edge = 1.0;
            if (regionAt(cid + vec2(0, 1), patch, t) != rid) edge = 1.0;
            if (regionAt(cid - vec2(0, 1), patch, t) != rid) edge = 1.0;
            col = edge > 0.5 ? base * felt : col;
            if (edge < 0.5) {                                              // inside the path: scattered knots
                vec4 th = thread(stitch(2.0, f, 0.55), tcol, f, aa, glint);
                col = mix(col, th.rgb, th.a * step(0.55, h2(cid + rid)));
            }
        } else if (style < 2.5) {
            float k = floor(sd2 * 7.0);
            vec2 so = stitch(k, f + vec2(0.05, 0.06), thick);              // shadow under the thread
            col = mix(under, under * 0.35, (1.0 - smoothstep(so.y, so.y + 0.06, so.x)) * 0.7);
            vec4 th = thread(stitch(k, f, thick), tcol, f, aa, glint);
            col = mix(col, th.rgb, th.a);
        } else {
            // NY motif: one sign repeated across the patch, sometimes at double size
            float g = floor(sd2 * 96.0);
            if (park > 0.5) g = PARK_TREE;
            float big = step(0.35, h1(sd * 7.7)) + touch;
            vec2 ff = big > 0.5 ? fract((cid + f) * 0.5) : f;
            vec4 gt = glyphTex(g, ff);
            float m = smoothstep(0.3, 0.65, gt.r);
            float msh = smoothstep(0.3, 0.65, glyphTex(g, ff - vec2(-0.03, 0.04) / (big > 0.5 ? 2.0 : 1.0)).r);
            float e = glyphTex(g, ff + vec2(0.02, 0.02)).r - gt.r;          // emboss across the thread
            col = mix(under, under * 0.35, msh * (1.0 - m) * 0.8);
            float hint = floor(gt.g * 255.0 / 20.0 + 0.5);
            if (hint > 0.5) {                                              // subway bullet: line colour disc, white letter
                float disc = 1.0 - smoothstep(0.42, 0.45, length(ff - 0.5));
                vec3 dc = lineCol(hint) * (0.85 + 0.15 * sin((ff.x - ff.y) * 70.0));
                col = mix(col, mix(dc, vec3(0.97), m), disc);
            } else {
                float satin = 0.8 + 0.2 * sin((ff.x - ff.y) * (big > 0.5 ? 90.0 : 55.0));
                vec3 tc = tcol * satin * (0.85 - e * 2.5) + glint * 0.3;
                col = mix(col, tc, m);
            }
        }
        // a seam where patches meet
        col *= 0.9 + 0.1 * step(0.04, min(min(f.x, f.y), min(1.0 - f.x, 1.0 - f.y)));
    }

    // the touch ring, stitched in white
    if (mouseDown > 0.5) {
        float rr = 0.11 + 0.02 * sin(TIME * 5.0);
        vec2 q = (cp - mp) * N;                                            // in stitch units
        float ring = abs(length(q) - rr * N);
        float dash = step(0.35, fract(atan(q.y, q.x) / 6.2831853 * 28.0 - TIME * 0.8));
        vec4 th = thread(vec2(ring, 0.16), vec3(1.0), vec2(atan(q.y, q.x), length(q)) * 0.1, aa, 0.2);
        col = mix(col, th.rgb, th.a * dash);
    }

    col *= 1.0 + beat * 0.15;
    float vig = 1.0 - 0.3 * dot((gl_FragCoord.xy / R - 0.5) * 1.2, (gl_FragCoord.xy / R - 0.5) * 1.2);
    gl_FragColor = vec4(col * vig, 1.0);
}
