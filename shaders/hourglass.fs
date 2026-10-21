/*{
  "DESCRIPTION": "Hourglass — a living hourglass that keeps pinching thin and swelling thick. Its silhouette is a mirrored profile of two lobes meeting at a neck; the lobes breathe, change shape (round, teardrop, diamond, lamp-shade), wiggle and drift, while three looks flow into each other: Nested Rings (an infinite tunnel of tangent circles stacked into the neck, each a radial rainbow), Thin Film (glowing interference fringes around dark lobes holding a sun and a target), and Lathe (a turned 3D vessel whose rainbow sweeps around it as it spins). Over it all runs a film and glitch layer: travelling pixelation zones, block mosaics, RGB split, slice tears, gate weave, flicker and grain. Bass pinches and swells the neck, beats fire glitch bursts and pixel drops, mids feed the wiggle, highs push the chroma split.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "lookSel",     "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Cycle", "Nested Rings", "Thin Film", "Lathe"], "GROUP": "Look" },
    { "NAME": "lookPeriod",  "LABEL": "Look Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 5.0, "MAX": 90.0, "DEFAULT": 16.0, "GROUP": "Look" },
    { "NAME": "morphSpeed",  "LABEL": "Morph Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Shape" },
    { "NAME": "neck",        "LABEL": "Neck Width",      "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.06, "GROUP": "Shape" },
    { "NAME": "thickness",   "LABEL": "Thick ↔ Thin Swing","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Shape" },
    { "NAME": "wiggle",      "LABEL": "Wiggle",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Shape" },
    { "NAME": "size",        "LABEL": "Size",            "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.6, "DEFAULT": 0.85, "GROUP": "Shape" },
    { "NAME": "ringCount",   "LABEL": "Rings",           "TYPE": "float", "MIN": 1.0, "MAX": 8.0, "DEFAULT": 3.2, "GROUP": "Color" },
    { "NAME": "flowSpeed",   "LABEL": "Color Flow",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "pixelate",    "LABEL": "Pixelation",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Film & Glitch" },
    { "NAME": "pixelSize",   "LABEL": "Pixel Size",      "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 80.0, "DEFAULT": 28.0, "GROUP": "Film & Glitch" },
    { "NAME": "glitch",      "LABEL": "Glitch",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Film & Glitch" },
    { "NAME": "chroma",      "LABEL": "RGB Split",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Film & Glitch" },
    { "NAME": "film",        "LABEL": "Film (weave/flicker)", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Film & Glitch" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Film & Glitch" },
    { "NAME": "audioReact",  "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Look", "Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.89, 0.87, 0.84, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.84, 0.74, 0.66, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Hourglass — single pass, 2D. scene(p) returns colour+coverage for a point
// in shape space; the film/glitch layer decides WHERE to sample it
// (pixel blocks, torn slices, weave) and samples it three times for the
// RGB split.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}
vec3 rainbow(float t) { return 0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67))); }

// poster palette: hot pink, sun yellow, cobalt, leaf green, red, lavender
vec3 poster(float k) {
    k = mod(floor(k), 6.0);
    if (k < 0.5) return vec3(1.0, 0.18, 0.5);
    if (k < 1.5) return vec3(1.0, 0.86, 0.15);
    if (k < 2.5) return vec3(0.15, 0.3, 0.85);
    if (k < 3.5) return vec3(0.2, 0.6, 0.3);
    if (k < 4.5) return vec3(1.0, 0.3, 0.12);
    return vec3(0.7, 0.55, 1.0);
}
// thin-film interference ramp
vec3 film3(float x) {
    x = fract(x);
    vec3 c = vec3(0.02, 0.06, 0.03);
    c = mix(c, vec3(0.1, 0.75, 0.25), smoothstep(0.0, 0.18, x));
    c = mix(c, vec3(1.0, 0.42, 0.08), smoothstep(0.18, 0.4, x));
    c = mix(c, vec3(1.0, 0.85, 0.25), smoothstep(0.4, 0.55, x));
    c = mix(c, vec3(1.0, 0.35, 0.45), smoothstep(0.55, 0.72, x));
    c = mix(c, vec3(0.15, 0.4, 1.0), smoothstep(0.72, 0.88, x));
    c = mix(c, vec3(0.02, 0.06, 0.03), smoothstep(0.88, 1.0, x));
    return c;
}

float gW0, gW1, gW2;        // look weights
float gT;
float gNeck, gRt, gRb, gPt, gPb, gTear;

// half-width of the vessel at height y (y>0 top lobe, y<0 bottom lobe)
float profile(float y) {
    float R = y > 0.0 ? gRt : gRb;
    float P = y > 0.0 ? gPt : gPb;
    float u = abs(y) / R - 1.0;                 // −1 at neck … +1 at the far tip
    float base = pow(max(1.0 - pow(abs(u), P), 0.0), 1.0 / P);
    // teardrop: narrower toward the tip
    base *= mix(1.0, 1.0 - 0.45 * smoothstep(-0.2, 1.0, u), gTear);
    return R * base + gNeck * 0.5 * exp(-y * y * 120.0);
}

// returns rgb (already over its own look background) + coverage
vec4 scene(vec2 p, vec3 bgA, vec3 bgB, vec3 bgC) {
    // wiggle warp
    float wg = wiggle * (1.0 + 0.8 * midP() * audioReact);
    p.x += wg * 0.028 * sin(p.y * 8.0 + gT * 1.3) * smoothstep(0.0, 0.4, abs(p.y));
    p.y += wg * 0.015 * sin(p.x * 10.0 - gT * 1.1);
    float y = p.y, ax = abs(p.x);
    float r = profile(y);
    float d = ax - r;                           // ~signed distance (px units of shape space)
    float inside = smoothstep(0.004, -0.004, d);

    vec3 cA = bgA, cB = bgB, cC = bgC;
    float aA = 0.0, aB = 0.0, aC = 0.0;

    // ── look A: nested tangent circles (infinite tunnel into the neck) ──
    if (gW0 > 0.001) {
        float yy = abs(y) + 1e-4;
        float sx = max(r, 1e-3) / max(gRt, gRb);              // squeeze circles with the vessel
        float xx = p.x / mix(1.0, sx * 1.6, 0.5);
        float u = (xx * xx + yy * yy) / (2.0 * yy);          // radius of the tangent circle through p
        float lu = log2(u / 0.02) * ringCount * 0.5 - gT * flowSpeed * 0.6;
        float band = floor(lu);
        float fb = fract(lu);
        float top = step(0.0, y);
        // radial gradient inside each circle, dark rim at its edge
        vec3 c1 = poster(band + top * 2.0);
        vec3 c2 = poster(band + 1.0 + top * 2.0);
        vec3 c = mix(c2, c1, smoothstep(0.0, 1.0, fb));
        c = mix(c, c * 0.35, smoothstep(0.75, 1.0, fb) * 0.7);
        c = mix(c, vec3(1.0), 0.18 * (1.0 - top));            // bottom half: pastel
        float outer = smoothstep(0.004, -0.004, u - max(gRt, gRb) * 1.05);
        cA = mix(bgA, c, outer);
        aA = outer;
    }
    // ── look B: thin-film fringes on dark ──
    if (gW1 > 0.001) {
        float fr = exp(-max(d, 0.0) * 36.0) * smoothstep(-0.06, 0.0, d);
        vec3 edge = film3(d * 7.0 - gT * flowSpeed * 0.25 + 0.3);
        vec2 ct = vec2(0.0, gRt * 1.15), cb = vec2(0.0, -gRb * 1.0);
        float dt = length(p - ct) / gRt, db = length(p - cb) / gRb;
        vec3 inner = y > 0.0
            ? mix(vec3(1.0, 0.82, 0.2), vec3(0.3, 0.12, 0.1), smoothstep(0.25, 0.75, dt))
            : film3(db * 1.6 - gT * flowSpeed * 0.3);
        inner *= 0.35 + 0.65 * smoothstep(1.0, 0.3, y > 0.0 ? dt : db);
        vec3 c = mix(bgB, inner, inside);
        c += edge * fr * 1.3;
        // faint outer ripples
        c += film3(d * 16.0 - gT * 0.2) * 0.08 * exp(-max(d, 0.0) * 10.0) * (1.0 - inside);
        cB = c;
        aB = max(inside, fr);
    }
    // ── look C: turned 3D vessel with a rainbow that spins around it ──
    if (gW2 > 0.001) {
        float nx = clamp(p.x / max(r, 1e-3), -1.0, 1.0);
        float nz = sqrt(max(1.0 - nx * nx, 0.0));
        float dr = (profile(y + 0.01) - profile(y - 0.01)) / 0.02;
        vec3 n = normalize(vec3(nx, -dr * 0.5, nz));
        float phi = atan(nx, nz) + gT * 0.6;
        vec3 c = rainbow(phi / TAU + y * 0.25 + gT * flowSpeed * 0.05);
        c = mix(c, c * c, 0.3);
        float lam = clamp(dot(n, normalize(vec3(-0.4, 0.5, 0.75))), 0.0, 1.0);
        c *= 0.7 + 0.4 * lam;
        // soft rim falloff like the airbrushed reference
        c = mix(c, c * 0.8 + 0.08, smoothstep(0.7, 1.0, abs(nx)) * 0.5);
        cC = mix(bgC, c, inside);
        aC = inside;
    }
    vec3 col = cA * gW0 + cB * gW1 + cC * gW2;
    float a = aA * gW0 + aB * gW1 + aC * gW2;
    return vec4(col, a);
}

vec3 userBg(vec2 uv) {
    float m = float(bgMode);
    if (m < 1.5) return bgColor.rgb;
    if (m < 2.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    float ar = audioReact;
    float beat = audioBeatPulse * ar;

    // look weights
    float sel = float(lookSel);
    if (sel < 0.5) {
        float ph = TIME / lookPeriod;
        float k = mod(floor(ph), 3.0);
        float f = smoothstep(0.6, 1.0, fract(ph));
        float k2 = mod(k + 1.0, 3.0);
        gW0 = (k < 0.5 ? 1.0 - f : 0.0) + (k2 < 0.5 ? f : 0.0);
        gW1 = (abs(k - 1.0) < 0.5 ? 1.0 - f : 0.0) + (abs(k2 - 1.0) < 0.5 ? f : 0.0);
        gW2 = (k > 1.5 ? 1.0 - f : 0.0) + (k2 > 1.5 ? f : 0.0);
    } else {
        gW0 = abs(sel - 1.0) < 0.5 ? 1.0 : 0.0;
        gW1 = abs(sel - 2.0) < 0.5 ? 1.0 : 0.0;
        gW2 = abs(sel - 3.0) < 0.5 ? 1.0 : 0.0;
    }

    // shape morph
    gT = TIME * morphSpeed + 0.4 * audioBassTime * ar * morphSpeed;
    float swing = 0.5 + 0.5 * sin(gT * 0.7);
    gNeck = neck * (0.3 + 1.4 * swing * thickness + 0.8 * bassP() * ar);
    gRt = 0.215 * (1.0 + 0.18 * thickness * sin(gT * 0.9) + 0.06 * bassP() * ar);
    gRb = 0.225 * (1.0 + 0.18 * thickness * sin(gT * 0.9 + 2.0) + 0.06 * bassP() * ar);
    gPt = mix(1.15, 3.0, 0.5 + 0.5 * sin(gT * 0.37));
    gPb = mix(1.4, 2.6, 0.5 + 0.5 * sin(gT * 0.29 + 1.7));
    gTear = 0.5 + 0.5 * sin(gT * 0.23 + 0.6);
    // thinner ↔ thicker: squeeze the whole vessel horizontally
    float squeeze = mix(1.0, 0.55 + 0.9 * (0.5 + 0.5 * sin(gT * 0.53)), thickness);

    // ── film: gate weave + vertical roll on big beats ──
    vec2 weave = film * vec2(1.2 * sin(TIME * 13.0) * h11(floor(TIME * 8.0)), 0.8 * sin(TIME * 7.0)) ;
    vec2 pf = fc + weave;

    // ── glitch: torn horizontal slices ──
    float gl = glitch * (0.25 + 1.6 * beat);
    float row = floor(pf.y / (R.y / 48.0));
    float tear = step(1.0 - 0.12 * gl, h12(vec2(row, floor(TIME * 9.0))));
    pf.x += tear * (h12(vec2(row * 3.1, floor(TIME * 9.0))) - 0.5) * R.x * 0.12 * gl;

    // ── travelling pixelation zones ──
    float zone = smoothstep(0.05, 0.0, abs(fract(pf.y / R.y * 0.8 - TIME * 0.05) - 0.5) - 0.18);
    float pxAmt = clamp(pixelate * zone + beat * pixelate * 0.6, 0.0, 1.0);
    float ps = pixelSize * mix(0.5, 1.4, h11(floor(TIME * 0.5)));
    if (pxAmt > 0.15) {
        float q = mix(1.0, ps, pxAmt);
        pf = (floor(pf / q) + 0.5) * q;
    }

    vec2 uv = pf / R;
    vec2 p = (pf - 0.5 * R) / R.y / size;
    p.x /= squeeze;

    // backgrounds per look (user override if not "Follow Look")
    bool follow = float(bgMode) < 0.5;
    vec3 bgA = follow ? vec3(0.89, 0.87, 0.84) : userBg(uv);
    vec3 bgB = follow ? vec3(0.0, 0.07, 0.03) : userBg(uv);
    vec3 bgC = follow ? mix(vec3(0.89, 0.88, 0.85), vec3(0.86, 0.84, 0.8), uv.y) : userBg(uv);

    // RGB split: three samples along a slowly turning axis
    float ch = chroma * (0.003 + 0.012 * highP() * ar + 0.01 * beat);
    vec2 cd = ch * vec2(cos(TIME * 0.3), sin(TIME * 0.3));
    vec4 s0 = scene(p + cd, bgA, bgB, bgC);
    vec4 s1 = scene(p, bgA, bgB, bgC);
    vec4 s2 = scene(p - cd, bgA, bgB, bgC);
    vec3 col = vec3(s0.r, s1.g, s2.b);
    float alpha = max(s1.a, max(s0.a, s2.a));

    // mosaic block: a little checkerboard of poster colours dropped on beats
    float mb = step(0.8, h11(floor(TIME * 0.7) + 3.0)) * step(0.2, glitch) + step(0.6, beat * glitch);
    if (mb > 0.5) {
        vec2 org = vec2(0.5 + 0.25 * (h11(floor(TIME * 0.7)) - 0.5), 0.3 + 0.4 * h11(floor(TIME * 0.7) + 1.0)) * R;
        vec2 bp = (fc - org) / (ps * 0.7);
        if (bp.x > 0.0 && bp.y > 0.0 && bp.x < 4.0 && bp.y < 4.0) {
            vec2 bi = floor(bp);
            col = poster(h12(bi + floor(TIME * 3.0)) * 6.0);
            alpha = 1.0;
        }
    }

    // pixel blocks on light looks read as tinted squares (as in the reference)
    col *= 1.0 + 0.15 * audioLevel * ar;
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation);

    // film flicker, vignette, grain
    col *= 1.0 - film * 0.05 * h11(floor(TIME * 24.0));
    vec2 dc = fc / R - 0.5;
    col *= 1.0 - 0.12 * film * smoothstep(0.35, 0.9, length(dc * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.1;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
