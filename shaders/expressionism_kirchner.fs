/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Expressionism after Kirchner / Brücke / Soutine — DENSE saturated gestural brushstroke field (~80% canvas coverage, NO desaturation, NO white-mixing) with a LARGE central angular face/body silhouette in heavy black ink and raw red accents (gouged-mask Brücke woodcut aesthetic). Woodcut-bone black ink linework on stroke edges. HDR red gestural slashes peak 2.5+ linear for hard bloom. Mood enum: Soutine Twist (warped warm), Schiele Nervous Line (vertical sweep), Brücke Storm (full chromatic violence), War Charcoal (black + bone + red wound). Output LINEAR HDR, no internal tonemap.",
  "INPUTS": [
    { "NAME": "mood",          "LABEL": "Mood",          "TYPE": "long",  "DEFAULT": 2, "VALUES": [0,1,2,3], "LABELS": ["Soutine Twist","Schiele Nervous Line","Brücke Storm","War Charcoal"] },
    { "NAME": "brushDensity",  "LABEL": "Brush Density", "TYPE": "float", "MIN": 12.0, "MAX": 48.0, "DEFAULT": 32.0 },
    { "NAME": "gestureSpeed",  "LABEL": "Gesture Speed", "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "paletteWarmth", "LABEL": "Palette Warmth","TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "audioReact",    "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// Expressionism — DENSE saturated stroke field + central angular figure.
// Strokes cover ~80% of canvas, FULLY SATURATED pigment, no white-mixing.
// Woodcut-bone black ink edge linework on every stroke.
// One LARGE central angular face/body silhouette per mood (heavy black,
// raw red gouged accents). HDR red slashes 2.5+ linear. LINEAR HDR out.

// FULLY SATURATED — DO NOT desaturate, DO NOT mix toward white.
const vec3 P_BONECREAM = vec3(0.94, 0.88, 0.74);
const vec3 P_RAWUMBER  = vec3(0.30, 0.18, 0.08);   // 0.30 raw umber
const vec3 P_CHARCOAL  = vec3(0.02, 0.02, 0.03);
const vec3 P_INK       = vec3(0.005,0.005,0.01);   // woodcut black
const vec3 P_CADRED    = vec3(0.92, 0.10, 0.06);   // 0.92 cadmium red
const vec3 P_VIRIDIAN  = vec3(0.04, 0.55, 0.34);   // 0.55 viridian
const vec3 P_PRUSSIAN  = vec3(0.03, 0.10, 0.65);   // 0.65 prussian
const vec3 P_CADORANGE = vec3(0.98, 0.42, 0.06);
const vec3 P_FLESH     = vec3(0.90, 0.42, 0.30);
const vec3 P_BLOODHOT  = vec3(3.20, 0.18, 0.06);   // HDR red wound 2.5+ linear

float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float h21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = h21(ip), b = h21(ip + vec2(1,0));
    float c = h21(ip + vec2(0,1)), d = h21(ip + vec2(1,1));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.55;
    mat2 R = mat2(0.8, -0.6, 0.6, 0.8);
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = R * p * 2.07 + vec2(7.3, 1.9);
        a *= 0.55;
    }
    return v;
}

vec2 warp(vec2 p, float t, float k) {
    vec2 q = vec2(fbm(p + vec2(0.0, t * 0.3)),
                  fbm(p + vec2(5.2, -t * 0.27)));
    return p + (q - 0.5) * k;
}

vec3 audioBands(float t, float a) {
    float bass = 0.5 + 0.5 * sin(t * 0.31);
    float mid  = 0.5 + 0.5 * sin(t * 0.83 + 1.7);
    float trb  = 0.5 + 0.5 * sin(t * 1.91 + 3.1);
    float env  = 0.25 + 0.75 * a;
    return vec3(bass, mid, trb) * env;
}

// Oriented quad SDF for one brushstroke. Returns signed distance + local coord.
float strokeSDF(vec2 uv, vec2 c, float ang, float len, float wid, out vec2 local) {
    float ca = cos(ang), sa = sin(ang);
    vec2 d = uv - c;
    local = vec2(ca * d.x + sa * d.y, -sa * d.x + ca * d.y);
    vec2 q = abs(local) - vec2(len, wid);
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
}

// Brush body coverage with bristles, ragged ends, FBM erosion.
// Also emits raw signed distance for woodcut edge linework.
float brushCoverage(vec2 uv, vec2 c, float ang, float len, float wid,
                    float seed, float warpK, out float sdOut) {
    vec2 wuv = warp(uv * 2.0, seed, warpK) * 0.5;
    vec2 lp;
    float sd = strokeSDF(wuv, c, ang, len, wid, lp);
    sdOut = sd;
    float body = smoothstep(0.0, -0.004, sd);
    if (body <= 0.0) return 0.0;
    float endTaper = smoothstep(len, len * 0.55, abs(lp.x));
    float bristles = sin((lp.y / max(wid, 1e-4)) * 7.0 + lp.x * 22.0 + seed * 6.28);
    bristles = 0.5 + 0.5 * bristles;
    bristles = mix(0.78, 1.0, bristles);    // less erosion → denser body
    float grain = fbm(lp * vec2(38.0, 70.0) + seed * 13.0);
    float erode = smoothstep(0.10, 0.55, grain); // wider plateau → denser body
    return body * endTaper * bristles * erode;
}

// One stroke layer: per-index angle/length/color, woodcut-bone ink edge.
vec3 paintStroke(vec3 under, vec2 uv, float idx, float t, int moodI,
                 float density, float warmth, vec3 bands, float audio) {
    float s  = h11(idx * 17.13 + 1.7);
    float s2 = h11(idx * 31.7  + 3.3);
    float s3 = h11(idx * 9.91  + 7.1);

    vec2 c = vec2(s, s2) * 2.0 - 1.0;
    c.x *= 1.6;
    c += 0.10 * vec2(sin(t * 0.7 + idx), cos(t * 0.61 + idx * 1.3));

    float baseAng = mix(-1.4, 1.4, s3);
    if (moodI == 1) baseAng *= 1.15;
    if (moodI == 0) baseAng += 0.3 * sin(t * 0.4 + idx);
    if (moodI == 3) baseAng = mix(baseAng, 0.0, 0.4);

    // Bigger / fatter strokes for ~80% coverage.
    float len = 0.45 + 0.45 * h11(idx * 5.7);
    float wid = 0.045 + 0.075 * h11(idx * 11.3);
    if (moodI == 1) { len *= 0.85; wid *= 0.70; }
    if (moodI == 0) { len *= 1.15; wid *= 1.20; }
    if (moodI == 2) { wid *= 1.20; }
    if (moodI == 3) { wid *= 0.95; }

    len *= 0.85 + 0.5 * bands.x * audio;
    wid *= 0.9  + 0.4 * bands.y * audio;

    float warpK = (moodI == 0) ? 0.55 : (moodI == 1) ? 0.20 : 0.34;
    float sd;
    float cov = brushCoverage(uv, c, baseAng, len, wid, idx, warpK, sd);
    if (cov <= 0.0 && sd > 0.012) return under;

    // SATURATED stroke pigment. NO mixing toward white.
    vec3 col;
    float pickW = h11(idx * 2.71);
    if (moodI == 0) {
        if      (pickW < 0.32) col = P_CADRED;
        else if (pickW < 0.56) col = P_FLESH;
        else if (pickW < 0.78) col = P_RAWUMBER;
        else                   col = P_VIRIDIAN;
    } else if (moodI == 1) {
        if      (pickW < 0.30) col = P_RAWUMBER;
        else if (pickW < 0.55) col = P_CADRED;
        else if (pickW < 0.78) col = P_PRUSSIAN;
        else                   col = P_CHARCOAL;
    } else if (moodI == 2) {
        if      (pickW < 0.28) col = P_CADRED;
        else if (pickW < 0.50) col = P_VIRIDIAN;
        else if (pickW < 0.70) col = P_PRUSSIAN;
        else if (pickW < 0.86) col = P_CADORANGE;
        else                   col = P_CHARCOAL;
    } else {
        if      (pickW < 0.55) col = P_CHARCOAL;
        else if (pickW < 0.80) col = P_RAWUMBER;
        else if (pickW < 0.95) col = P_INK;
        else                   col = P_CADRED;
    }

    // Warmth bias — push warmer, never desaturate.
    col = mix(col, col * vec3(1.10, 0.92, 0.78), warmth * 0.5);

    // Wet-into-wet drag — keep more pigment, less under bleed.
    vec3 dragged = mix(col, under, 0.12 + 0.12 * h11(idx * 4.1));
    float edge = pow(1.0 - smoothstep(0.0, 0.7, cov), 1.5);
    vec3 pigment = mix(col, dragged, edge * 0.5); // keep saturation in core

    vec3 outc = mix(under, pigment, cov);

    // WOODCUT-BONE BLACK INK LINEWORK on stroke edges (Brücke woodcut).
    float inkBand = smoothstep(0.014, 0.000, abs(sd));
    inkBand *= smoothstep(-0.020, -0.002, sd); // only just inside/at edge
    outc = mix(outc, P_INK, inkBand * 0.85);

    return outc;
}

// HDR red slashes — bloom 2.5+ linear.
vec3 redSlash(vec3 under, vec2 uv, float t, int moodI, float audio) {
    int n = (moodI == 3) ? 2 : (moodI == 1) ? 3 : 4;
    vec3 col = under;
    for (int i = 0; i < 5; i++) {
        if (i >= n) break;
        float fi = float(i);
        vec2 c = vec2(0.7 * sin(t * 0.23 + fi * 2.1),
                      0.6 * cos(t * 0.31 + fi * 1.7));
        float ang = mix(-1.2, 1.2, h11(fi * 7.3 + 0.13)) + 0.2 * sin(t * 0.5 + fi);
        float len = 0.55 + 0.18 * sin(t * 0.7 + fi * 2.0);
        float wid = 0.012 + 0.012 * h11(fi * 3.7);
        float sd;
        float cov = brushCoverage(uv, c, ang, len, wid, fi * 11.0 + 91.0, 0.25, sd);
        vec3 hot = P_BLOODHOT * (1.0 + 0.6 * audio);
        col = mix(col, hot, cov);
    }
    return col;
}

// LARGE central angular figure silhouette (Brücke gouged-mask aesthetic).
// Returns ink-filled face/body region with raw red accent gouges.
vec3 centralFigure(vec3 under, vec2 uv, float t, int moodI, float audio) {
    // Slow head sway.
    vec2 p = uv;
    p.x -= 0.04 * sin(t * 0.27);
    p.y -= 0.02 * cos(t * 0.21);

    // Angular face SDF: stretched diamond head + jaw + neck/shoulders.
    // Head: y above 0, jaw flares; shoulders: y below -0.35.
    vec2 hp = p - vec2(0.0, 0.18);
    // skew per mood for character
    if (moodI == 1) hp.x += 0.18 * hp.y;          // Schiele tilt
    if (moodI == 0) hp.x += 0.10 * sin(hp.y*5.0); // Soutine warp
    // Angular head: |x| + 0.6|y - flatten| < r, with FBM erosion.
    float headR = 0.55 + 0.05 * sin(t * 0.5);
    float headD = abs(hp.x) * 1.05 + abs(hp.y) * 0.85;
    // Jaw stretch downward
    if (hp.y < 0.0) headD = abs(hp.x) * 1.25 + abs(hp.y * 1.4) * 0.95;
    float head = smoothstep(headR + 0.02, headR - 0.02, headD);
    // Shoulders/body trapezoid below
    vec2 bp = p - vec2(0.0, -0.62);
    float bodyD = max(abs(bp.x) - (0.55 + 0.35 * smoothstep(0.0, -0.6, bp.y)),
                      abs(bp.y) - 0.40);
    float body = smoothstep(0.02, -0.02, bodyD);
    // Neck connector
    vec2 np = p - vec2(0.0, -0.30);
    float neckD = max(abs(np.x) - 0.18, abs(np.y) - 0.14);
    float neck = smoothstep(0.02, -0.02, neckD);

    float figMask = max(head, max(body, neck));
    if (figMask <= 0.0) return under;

    // Edge erosion: woodcut chip noise on silhouette boundary
    float edgeNoise = fbm(p * 8.0 + vec2(t * 0.1, 0.0));
    float chip = smoothstep(0.30, 0.70, edgeNoise);
    figMask *= mix(0.85, 1.0, chip);

    // Heavy black ink fill.
    vec3 figCol = P_INK;

    // Gouged-mask interior carving: parallel angular gouge lines.
    vec2 gp = p;
    float gAng = (moodI == 1) ? 1.2 : (moodI == 3) ? 1.5 : 0.9;
    float gca = cos(gAng), gsa = sin(gAng);
    float gu = gca * gp.x + gsa * gp.y;
    float gv = -gsa * gp.x + gca * gp.y;
    // gouge stripes
    float stripe = sin(gv * 28.0 + fbm(gp * 4.0) * 6.0);
    float gouge = smoothstep(0.55, 0.85, stripe);
    // Eye sockets — two dark gouge holes around y=0.25
    float eyeY = 0.30;
    vec2 e1 = p - vec2(-0.18, eyeY);
    vec2 e2 = p - vec2( 0.18, eyeY);
    float eye = smoothstep(0.10, 0.06, length(e1 * vec2(1.0, 1.6)))
              + smoothstep(0.10, 0.06, length(e2 * vec2(1.0, 1.6)));
    eye = clamp(eye, 0.0, 1.0);
    // Mouth gash — red cad accent slash
    vec2 mp = p - vec2(0.02 * sin(t * 0.4), -0.02);
    float mouthD = max(abs(mp.x) - 0.22, abs(mp.y) - 0.025);
    float mouth = smoothstep(0.005, -0.005, mouthD);

    // Raw red gouge accents inside figure (cad red 0.92).
    vec3 gougeCol = P_CADRED * 1.4; // push into HDR
    vec3 figureFill = mix(figCol, gougeCol, gouge * 0.45);
    // Mouth gash: HDR red slash
    figureFill = mix(figureFill, P_BLOODHOT * (0.7 + 0.5 * audio), mouth * 0.95);
    // Eyes: deepest ink black
    figureFill = mix(figureFill, P_INK * 0.0, eye * 0.85);

    // Strong silhouette: composite at full opacity within mask.
    return mix(under, figureFill, figMask);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME * (0.4 + gestureSpeed);
    int moodI = clamp(int(mood), 0, 3);
    vec3 bands = audioBands(t, audioReact);

    // 1) Underpainting — coarse cream/bone with raw-umber stains.
    vec3 under = mix(P_BONECREAM, P_BONECREAM * 0.78, fbm(uv * 1.4));
    float stain = fbm(uv * 2.7 + 13.0);
    under = mix(under, P_RAWUMBER, smoothstep(0.55, 0.85, stain) * 0.55);
    if (moodI == 3) under = mix(under, P_CHARCOAL, 0.70 + 0.20 * fbm(uv * 1.9));
    if (moodI == 1) under *= 1.04;

    // 2) DENSE stroke field — ~80% coverage.
    int N = int(clamp(brushDensity, 12.0, 48.0));
    vec3 col = under;
    for (int i = 0; i < 48; i++) {
        if (i >= N) break;
        col = paintStroke(col, uv, float(i) + 1.0, t, moodI,
                          float(N), paletteWarmth, bands, audioReact);
    }

    // 3) HDR red slashes (bloom 2.5+ linear).
    col = redSlash(col, uv, t, moodI, audioReact);

    // 4) LARGE CENTRAL ANGULAR FIGURE — gouged-mask Brücke silhouette.
    col = centralFigure(col, uv, t, moodI, audioReact);

    // 5) Canvas tooth highlights.
    float tooth = fbm(uv * 90.0);
    float hi = smoothstep(0.78, 0.95, tooth);
    col += vec3(0.30, 0.26, 0.18) * hi * 0.7;

    // 6) Slight raw-umber vignette wash.
    float r = length(uv * vec2(0.6, 0.8));
    col = mix(col, col * 0.78 + P_RAWUMBER * 0.05, smoothstep(0.95, 1.55, r));

    gl_FragColor = vec4(col, 1.0);
}
