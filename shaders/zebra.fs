/*{
  "DESCRIPTION": "Zebra — stripes drawn on a liquid sheet, with molten chrome. Glossy liquid-metal worms slither across the sheet, reflecting the live stripes and throwing shadows, while a vortex funnel twists the lines down into a tunnel. A heightfield made of flowing domain-warped waves, drifting bumps and beat-launched ripples is raymarched from a tilting camera, so the lines bend over real 3D folds, hide behind ridges, catch speculars and throw self-shadows. Six looks morph into each other: Satin (glossy black-and-white fabric), Op-Art (Riley-style gradient bands, orange to violet to cobalt to lime), Neon Spectrum (each line a full rainbow ribbon on black), Glitch Chroma (monochrome lines that split into RGB wherever the liquid is steep), Holo Chrome (thin silver lines over iridescent oil-slick colour) and Glow Lines (emissive line drawing floating on black). Stripes flow along the sheet, the camera breathes, and the RGB split follows the slope. Bass lifts the waves, beats fire ripples and a chroma kick, mids stir the flow, highs make the speculars and split flare.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "3D", "Op Art", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "lookSel",     "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Cycle", "Satin", "Op-Art", "Neon Spectrum", "Glitch Chroma", "Holo Chrome", "Glow Lines"], "GROUP": "Look" },
    { "NAME": "lookPeriod",  "LABEL": "Look Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 12.0, "GROUP": "Look" },
    { "NAME": "lineCount",   "LABEL": "Line Density",    "TYPE": "float", "MIN": 4.0, "MAX": 60.0, "DEFAULT": 20.0, "GROUP": "Lines" },
    { "NAME": "lineWidth",   "LABEL": "Line Width",      "TYPE": "float", "MIN": 0.1, "MAX": 0.9, "DEFAULT": 0.5, "GROUP": "Lines" },
    { "NAME": "lineAngle",   "LABEL": "Line Angle",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Lines" },
    { "NAME": "lineWarp",    "LABEL": "Op-Art Bend",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Lines" },
    { "NAME": "lineFlow",    "LABEL": "Line Flow",       "TYPE": "float", "MIN": -2.0, "MAX": 2.0, "DEFAULT": 0.3, "GROUP": "Lines" },
    { "NAME": "waveHeight",  "LABEL": "Wave Height",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Liquid" },
    { "NAME": "waveScale",   "LABEL": "Wave Scale",      "TYPE": "float", "MIN": 0.4, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Liquid" },
    { "NAME": "liquidSpeed", "LABEL": "Liquid Speed",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Liquid" },
    { "NAME": "turbulence",  "LABEL": "Turbulence",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Liquid" },
    { "NAME": "chromeAmt",   "LABEL": "Liquid Chrome",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Chrome" },
    { "NAME": "chromeCount", "LABEL": "Chrome Pieces",   "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 6.0, "DEFAULT": 4.0, "GROUP": "Chrome" },
    { "NAME": "chromeSize",  "LABEL": "Chrome Size",     "TYPE": "float", "MIN": 0.4, "MAX": 2.5, "DEFAULT": 1.6, "GROUP": "Chrome" },
    { "NAME": "chromeSpeed", "LABEL": "Chrome Slither",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Chrome" },
    { "NAME": "chromeTint",  "LABEL": "Chrome Tint",     "TYPE": "color", "DEFAULT": [0.86, 0.89, 0.96, 1.0], "GROUP": "Chrome" },
    { "NAME": "vortex",      "LABEL": "Vortex Funnel",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Chrome" },
    { "NAME": "tilt",        "LABEL": "Camera Tilt",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.42, "GROUP": "Camera" },
    { "NAME": "camDrift",    "LABEL": "Camera Drift",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Camera" },
    { "NAME": "zoom",        "LABEL": "Zoom",            "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Camera" },
    { "NAME": "chroma",      "LABEL": "Chromatic Split", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Light" },
    { "NAME": "shadows",     "LABEL": "Shadows",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Light" },
    { "NAME": "highlights",  "LABEL": "Highlights",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "lightAngle",  "LABEL": "Light Angle",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Light" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "colorCycle",  "LABEL": "Color Cycle",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.02, 0.06, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Zebra — single pass. Heightfield y = h(x,z,t) raymarched (relaxed step +
// bisection), finite-difference normal, marched soft self-shadow. Stripes
// are a coordinate on the sheet; every look is a function of that
// coordinate, so the RGB split is three cheap look-ups at s ± δ where δ
// grows with slope.
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

float gT, gAmp;
vec2 gRipC;
vec2 gVC;                 // vortex centre (world xz)
vec4 gS[30];              // chrome spheres: xyz centre, radius
vec4 gWB[6];              // per-worm bounding sphere
float gNS;                // active sphere count
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float sminZ(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }

// ── the liquid ───────────────────────────────────────────────────────────
float height(vec2 p) {
    vec2 vd = p - gVC;
    float funnel = -vortex * 2.6 * exp(-dot(vd, vd) * 0.55) - vortex * 0.9 * exp(-dot(vd, vd) * 0.15);
    p *= 0.75 / waveScale;
    float t = gT;
    float tu = turbulence;
    vec2 q = p;
    q += 0.55 * tu * vec2(sin(p.y * 1.3 + t * 0.9), cos(p.x * 1.1 - t * 0.7));
    q += 0.3 * tu * vec2(sin(q.y * 2.3 - t * 1.2 + 1.7), cos(q.x * 2.1 + t * 1.05));
    float h = 0.55 * sin(q.x * 1.25 + t * 0.4) * cos(q.y * 1.05 - t * 0.3);
    h += 0.25 * sin(q.x * 2.4 + q.y * 1.8 + t * 1.1);
    h += 0.12 * tu * sin(q.x * 4.7 - q.y * 3.9 - t * 1.7);
    // drifting domes (pulls / bulges)
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        vec2 c = vec2(1.6 * sin(t * (0.21 + 0.07 * fi) + fi * 2.1), 1.2 * cos(t * (0.17 + 0.05 * fi) + fi * 1.3));
        float s = (mod(fi, 2.0) < 0.5 ? 1.0 : -0.8);
        h += s * 0.7 * exp(-dot(p - c, p - c) * 1.6);
    }
    // beat ripple
    float rd = length(p - gRipC);
    float rr = (1.0 - audioBeatPulse) * 3.5;
    h += 0.35 * audioBeatPulse * audioReact * sin((rd - rr) * 9.0) * exp(-pow((rd - rr) * 2.2, 2.0));
    return (h + funnel) * gAmp;
}

vec3 normalAt(vec2 p) {
    float e = 0.01;
    float hx = height(p + vec2(e, 0.0)) - height(p - vec2(e, 0.0));
    float hz = height(p + vec2(0.0, e)) - height(p - vec2(0.0, e));
    return normalize(vec3(-hx, 2.0 * e, -hz));
}

// ── looks: colour of the stripe coordinate s at surface point ────────────
//  s : stripe coordinate (one unit = one line period)
//  P : world point, n : normal, diff/spec/sh : lighting terms
vec3 look(float k, float s, vec3 P, vec3 n, vec3 rd, float diff, float spec, float sh, float aa) {
    float f = fract(s);
    float w = lineWidth;
    float line = smoothstep(w + aa, w - aa, abs(f - 0.5) * 2.0);     // 1 on the line
    float fres = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 3.0);
    float cyc = gT * 0.05 * colorCycle;
    if (k < 0.5) {                                    // Satin
        vec3 base = mix(vec3(0.015), vec3(0.93), line);
        vec3 c = base * (0.35 + 0.75 * diff) * mix(1.0, sh, 0.85);
        c += vec3(1.0) * spec * (0.25 + 0.5 * line) * highlights * mix(0.3, 1.0, sh);
        return c;
    }
    if (k < 1.5) {                                    // Op-Art gradient bands
        float g = clamp(0.5 - P.z * 0.3 + 0.1 * sin(cyc * 6.0 + P.x * 0.3), 0.0, 1.0);
        vec3 grad = mix(vec3(0.55, 0.95, 0.35), vec3(0.15, 0.4, 0.9), smoothstep(0.0, 0.35, g));
        grad = mix(grad, vec3(0.3, 0.12, 0.45), smoothstep(0.35, 0.55, g));
        grad = mix(grad, vec3(0.95, 0.2, 0.12), smoothstep(0.55, 0.78, g));
        grad = mix(grad, vec3(1.0, 0.72, 0.1), smoothstep(0.78, 1.0, g));
        vec3 alt = mix(vec3(0.97, 0.95, 0.98), vec3(0.08, 0.04, 0.12), smoothstep(0.4, 0.9, g));
        // each band shades across its width (the folded-paper look)
        float across = smoothstep(0.0, 1.0, f);
        vec3 c = mix(alt, grad * (0.55 + 0.6 * across), line);
        c *= 0.6 + 0.5 * diff;
        c = mix(c, c * sh, 0.7);
        return c + vec3(1.0) * spec * 0.2 * highlights;
    }
    if (k < 2.5) {                                    // Neon spectrum ribbons
        float x = clamp((f - (0.5 - 0.5 * w - 0.2)) / (w + 0.4), 0.0, 1.0);
        vec3 c = rainbow(x * 0.85 + 0.6 + cyc);
        c = mix(vec3(0.0), c, smoothstep(0.0, 0.08, x) * smoothstep(1.0, 0.92, x));
        c *= 0.55 + 0.6 * diff;
        c *= mix(1.0, sh, 0.95);                      // hard black shadows like the reference
        return c * 1.15;
    }
    if (k < 3.5) {                                    // Glitch chroma (B/W; split happens outside)
        vec3 c = mix(vec3(0.03), vec3(0.95), line);
        c *= 0.6 + 0.45 * diff;
        c = mix(c, c * sh, 0.6);
        return c + spec * 0.3 * highlights;
    }
    if (k < 4.5) {                                    // Holo chrome
        float thin = smoothstep(0.12 * w + aa, 0.12 * w - aa, abs(f - 0.5));
        vec3 ir = rainbow(dot(n, -rd) * 1.6 + P.y * 0.6 + cyc * 3.0);
        vec3 c = mix(vec3(0.03, 0.02, 0.1), ir * 0.75, 0.35 + 0.65 * fres + 0.3 * diff);
        c *= mix(1.0, sh, 0.6);
        c += vec3(0.95, 0.97, 1.0) * thin * (0.6 + 0.6 * diff);
        c += vec3(1.0) * spec * 1.2 * highlights;
        return c;
    }
    // Glow lines
    float thin = smoothstep(0.18 * w + aa, 0.18 * w - aa, abs(f - 0.5));
    float glow = exp(-abs(f - 0.5) * 18.0);
    vec3 hc = mix(vec3(1.0, 0.45, 0.1), vec3(1.0, 0.15, 0.3), smoothstep(-0.3, 0.2, P.y));
    hc = mix(hc, vec3(0.35, 0.45, 1.0), smoothstep(0.1, 0.6, P.y + 0.15 * sin(cyc * 4.0 + P.x)));
    vec3 c = hc * (thin * 1.2 + glow * 0.45) * (0.5 + 0.7 * diff);
    c += vec3(1.0, 0.9, 0.95) * thin * spec * highlights;
    // soft island mask: the line field frays out at its border
    float m = smoothstep(2.6, 1.2, length(P.xz * vec2(0.8, 1.0)) + 0.5 * sin(P.x * 2.3 + gT) * sin(P.z * 1.9 - gT * 0.7));
    return c * m;
}

// ── liquid chrome: chains of melted spheres (precomputed per frame) ────────
void buildChrome() {
    gNS = floor(clamp(chromeCount, 1.0, 6.0) + 0.5) * 5.0;
    float ct = TIME * chromeSpeed * 0.6 + 0.3 * audioBassTime * audioReact * chromeSpeed;
    for (int w = 0; w < 6; w++) {
        float fw = float(w);
        vec2 c0 = vec2(2.3 * sin(ct * (0.13 + 0.03 * fw) + fw * 1.9), 1.7 * cos(ct * (0.11 + 0.025 * fw) + fw * 2.7));
        // keep worms out of the funnel's throat, they slide around its rim
        vec2 dv = c0 - gVC;
        c0 = gVC + normalize(dv + 1e-4) * max(length(dv), 1.8 * vortex + 0.4);
        float base = height(c0);
        float ang = ct * 0.25 + fw * 1.3;
        for (int k = 0; k < 5; k++) {
            float fk = float(k);
            float sk = fk - 2.0;
            float a = ang + 0.7 * sin(ct * 1.1 + fk * 0.9 + fw * 2.0) * sk * 0.5;
            vec2 off = vec2(cos(a), sin(a)) * sk * 0.25 * chromeSize;
            off += 0.12 * chromeSize * vec2(sin(ct * 1.7 + fk * 1.3 + fw), cos(ct * 1.3 + fk * 1.1 - fw)) * abs(sk);
            float r = chromeSize * (0.17 + 0.05 * sin(fk * 1.7 + fw * 3.1 + ct * 1.4)) * (1.0 - 0.22 * abs(sk) / 2.0)
                    * (1.0 + 0.16 * bassP() * audioReact);
            if (w == 2 && k > 2) r *= 0.0;                        // a few short drops, not all long worms
            vec2 cxz = c0 + off;
            gS[w * 5 + k] = vec4(cxz.x, base + r * 0.45 + 0.04, cxz.y, r);
        }
        gWB[w] = vec4(c0.x, base + 0.1, c0.y, chromeSize * 1.25 + 0.25);
    }
}

float chromeSDF(vec3 p) {
    float d = 1e5;
    for (int w = 0; w < 6; w++) {
        if (float(w) * 5.0 > gNS - 0.5) break;
        vec4 b = gWB[w];
        float db = length(p - b.xyz) - b.w;
        if (db > 0.3) { d = min(d, db); continue; }      // far from this worm: cheap bound
        for (int k = 0; k < 5; k++) {
            vec4 s = gS[w * 5 + k];
            if (s.w < 0.01) continue;
            d = sminZ(d, length(p - s.xyz) - s.w, 0.2 * chromeSize);
        }
    }
    return d;
}

vec3 chromeNormal(vec3 p) {
    vec2 e = vec2(0.002, -0.002);
    return normalize(e.xyy * chromeSDF(p + e.xyy) + e.yyx * chromeSDF(p + e.yyx) + e.yxy * chromeSDF(p + e.yxy) + e.xxx * chromeSDF(p + e.xxx));
}

// stripe coordinate on the sheet, with the vortex twisting the lines into its throat
float stripeCoord(vec3 P) {
    vec2 q = P.xz;
    vec2 d = q - gVC;
    q = gVC + rot2(vortex * (3.2 * exp(-length(d) * 0.6)) + vortex * 0.15 * sin(TIME * 0.3)) * d;
    float ang = lineAngle * PI;
    vec2 dir = vec2(cos(ang), sin(ang));
    vec2 pd = vec2(-dir.y, dir.x);
    float along = dot(q, pd);
    float bend = P.y * 0.45
               + turbulence * 0.28 * (sin(along * 1.7 + gT * 0.8) + 0.6 * sin(along * 3.3 - dot(q, dir) * 1.3 - gT * 1.2));
    float sLin = (dot(q, dir) + bend * lineWarp) * lineCount * 0.25 - TIME * lineFlow;
    // inside the funnel the lines close into rings that crowd toward the throat (an endless tunnel)
    float r = length(d);
    float sRing = -log(r + 0.04) * lineCount * 0.11 + TIME * (0.5 + lineFlow);
    float w = vortex * smoothstep(2.4, 0.5, r);
    return mix(sLin, sRing, w);
}

vec3 userBg(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    gT = TIME * liquidSpeed + 0.5 * audioBassTime * ar * liquidSpeed + 0.4 * midP() * ar;
    gAmp = 0.6 * waveHeight * (1.0 + 0.45 * bassP() * ar);
    gRipC = vec2(1.5 * (h11(floor(TIME * 0.5)) - 0.5), 1.0 * (h11(floor(TIME * 0.5) + 7.0) - 0.5));
    gVC = vec2(0.5 * sin(TIME * 0.07), 0.35 * cos(TIME * 0.05));
    bool useChrome = chromeAmt > 0.01;
    if (useChrome) buildChrome();

    // look selection
    float sel = float(lookSel);
    float la, lb, lf;
    if (sel < 0.5) {
        float ph = TIME / lookPeriod;
        la = mod(floor(ph), 6.0); lb = mod(la + 1.0, 6.0);
        lf = smoothstep(0.7, 1.0, fract(ph));
    } else { la = sel - 1.0; lb = la; lf = 0.0; }

    // camera
    float pitch = mix(1.45, 0.55, tilt) + camDrift * 0.06 * sin(TIME * 0.21);
    float yaw = camDrift * 0.25 * sin(TIME * 0.13);
    float dist = 5.5;
    vec3 ta = vec3(camDrift * 0.4 * sin(TIME * 0.09), 0.0, camDrift * 0.3 * cos(TIME * 0.11));
    vec3 ro = ta + dist * vec3(sin(yaw) * cos(pitch), sin(pitch), -cos(yaw) * cos(pitch));
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec2 sp = (fc - 0.5 * R) / R.y;
    float focal = 1.6 * zoom;
    vec3 rd = normalize(sp.x * uu + sp.y * vv + focal * ww);

    vec3 col = userBg(uv);
    float alpha = 0.0;

    // heightfield march
    float maxH = gAmp * (1.6 + 3.3 * vortex) + 0.05;
    float t = 0.0;
    // start where the ray enters the slab |y| < maxH
    if (rd.y < 0.0) t = max(0.0, (ro.y - maxH) / -rd.y);
    bool hit = false;
    float tPrev = t;
    for (int i = 0; i < 120; i++) {
        vec3 p = ro + rd * t;
        float dh = p.y - height(p.xz);
        if (dh < 0.0) { hit = true; break; }
        if (p.y < -maxH && rd.y < 0.0) break;
        tPrev = t;
        t += max(dh * mix(0.45, 0.3, vortex), 0.003 * t);
        if (t > 40.0) break;
    }
    if (hit) {
        // bisection refine
        float a = tPrev, b = t;
        for (int i = 0; i < 6; i++) {
            float m = 0.5 * (a + b);
            vec3 p = ro + rd * m;
            if (p.y - height(p.xz) < 0.0) b = m; else a = m;
        }
        t = 0.5 * (a + b);
        vec3 P = ro + rd * t;
        vec3 n = normalAt(P.xz);

        float la2 = lightAngle * TAU;
        vec3 L = normalize(vec3(cos(la2) * 0.8, 0.75, sin(la2) * 0.8 + 0.3));
        float diff = clamp(dot(n, L), 0.0, 1.0);
        vec3 H = normalize(L - rd);
        float spec = pow(clamp(dot(n, H), 0.0, 1.0), 48.0) * (1.0 + 1.5 * highP() * ar);
        // soft self-shadow
        float sh = 1.0;
        if (shadows > 0.001) {
            float st = 0.04;
            for (int i = 0; i < 14; i++) {
                vec3 q = P + L * st;
                float dd = q.y - height(q.xz);
                sh = min(sh, clamp(10.0 * dd / st, 0.0, 1.0));
                st += 0.06 + st * 0.25;
            }
            if (useChrome) {
                float cs = 0.03;
                for (int i = 0; i < 10; i++) {
                    float dc = chromeSDF(P + L * cs);
                    sh = min(sh, clamp(6.0 * dc / cs, 0.0, 1.0));
                    cs += max(dc, 0.05);
                    if (cs > 2.5) break;
                }
                // contact darkening right under the metal
                sh *= mix(1.0, smoothstep(0.0, 0.35, chromeSDF(P)), 0.8 * chromeAmt);
            }
            sh = mix(1.0, sh, shadows);
        }
        // stripe coordinate on the sheet (+ op-art lateral bend by height)
        float s = stripeCoord(P);
        float throat = vortex * smoothstep(-0.25, -1.8, P.y / max(gAmp, 0.05));
        float slope = length(n.xz) / max(n.y, 0.2);
        // pixel footprint in stripe units (for AA)
        float aa = clamp(lineCount * 0.25 * t / (R.y * focal * 0.5) / max(abs(dot(n, -rd)), 0.15), 0.002, 0.5);

        // chromatic split: grows with slope (everywhere in Glitch Chroma)
        float gl = (abs(la - 3.0) < 0.5 ? 1.0 - lf : 0.0) + (abs(lb - 3.0) < 0.5 ? lf : 0.0);
        float dlt = chroma * (0.04 + 0.22 * smoothstep(0.2, 1.6, slope) * (0.4 + gl) + 0.08 * audioBeatPulse * ar + 0.05 * highP() * ar);
        vec3 cA0 = look(la, s + dlt, P, n, rd, diff, spec, sh, aa);
        vec3 cA1 = look(la, s,       P, n, rd, diff, spec, sh, aa);
        vec3 cA2 = look(la, s - dlt, P, n, rd, diff, spec, sh, aa);
        vec3 c = vec3(cA0.r, cA1.g, cA2.b);
        if (lf > 0.001) {
            vec3 cB0 = look(lb, s + dlt, P, n, rd, diff, spec, sh, aa);
            vec3 cB1 = look(lb, s,       P, n, rd, diff, spec, sh, aa);
            vec3 cB2 = look(lb, s - dlt, P, n, rd, diff, spec, sh, aa);
            c = mix(c, vec3(cB0.r, cB1.g, cB2.b), lf);
        }
        c *= 1.0 - 0.8 * throat;
        // glow lines float on the background
        float gw = (abs(la - 5.0) < 0.5 ? 1.0 - lf : 0.0) + (abs(lb - 5.0) < 0.5 ? lf : 0.0);
        // distance fog into the background
        float fog = smoothstep(14.0, 30.0, t);
        vec3 surf = mix(c, col, fog);
        col = mix(surf, col + c, gw);
        alpha = mix(1.0 - fog, clamp(lum(c) * 2.0, 0.0, 1.0), gw);
    }

    // ── liquid chrome on top ──
    if (useChrome) {
        float tc = 0.0;
        float top = maxH + 0.9 * chromeSize;
        if (rd.y < 0.0) tc = max(0.0, (ro.y - top) / -rd.y);
        bool ch = false;
        for (int i = 0; i < 80; i++) {
            vec3 p = ro + rd * tc;
            float d = chromeSDF(p);
            if (d < 0.0015 * tc) { ch = true; break; }
            tc += d;
            if (tc > 40.0 || (p.y < -maxH && rd.y < 0.0)) break;
        }
        if (ch && (!hit || tc < t + 0.02)) {
            vec3 Pc = ro + rd * tc;
            vec3 nc = chromeNormal(Pc);
            vec3 rf = reflect(rd, nc);
            float la2c = lightAngle * TAU;
            vec3 Lc = normalize(vec3(cos(la2c) * 0.8, 0.75, sin(la2c) * 0.8 + 0.3));
            // environment: the stripe sheet below, a soft studio sky above
            vec3 env;
            vec3 up0 = vec3(0.0, 1.0, 0.0);
            if (rf.y >= -0.02) {
                // a striped room above: the same lines wrap the sky, so the metal stays high-contrast
                vec2 sky = rf.xz / (rf.y + 0.45) * 0.55;
                float ss = stripeCoord(vec3(sky.x, 0.0, sky.y)) * 0.5;
                env = look(la, ss, vec3(sky.x, 0.0, sky.y), up0, rf, 0.9, 0.0, 1.0, 0.16);
                if (lf > 0.001) env = mix(env, look(lb, ss, vec3(sky.x, 0.0, sky.y), up0, rf, 0.9, 0.0, 1.0, 0.16), lf);
                env = mix(env, vec3(1.0), 0.15 * smoothstep(0.2, 1.0, rf.y));
                env += vec3(1.6) * smoothstep(0.88, 0.97, dot(rf, normalize(vec3(-0.3, 1.0, -0.2))));   // softbox
                env += vec3(0.9) * smoothstep(0.93, 0.99, dot(rf, normalize(vec3(0.7, 0.6, 0.4))));    // kicker
            } else {
                vec3 Q = Pc + rf * ((Pc.y + 0.1) / -rf.y);
                float raa = clamp(0.06 + 0.08 * length(Q - Pc), 0.06, 0.45);
                float sq = stripeCoord(vec3(Q.x, 0.0, Q.z));
                float dl = chroma * 0.08;
                vec3 up = vec3(0.0, 1.0, 0.0);
                vec3 e0 = look(la, sq + dl, Q, up, rf, 0.8, 0.0, 1.0, raa);
                vec3 e1 = look(la, sq, Q, up, rf, 0.8, 0.0, 1.0, raa);
                vec3 e2 = look(la, sq - dl, Q, up, rf, 0.8, 0.0, 1.0, raa);
                env = vec3(e0.r, e1.g, e2.b);
                if (lf > 0.001) env = mix(env, look(lb, sq, Q, up, rf, 0.8, 0.0, 1.0, raa), lf);
            }
            float fr = 0.8 + 0.2 * pow(1.0 - clamp(dot(nc, -rd), 0.0, 1.0), 4.0);
            vec3 cc = env * chromeTint.rgb * fr;
            // occlusion where the metal meets the sheet
            float oc = clamp((Pc.y - height(Pc.xz)) / (0.25 * chromeSize), 0.0, 1.0);
            cc *= 0.35 + 0.65 * mix(1.0, oc, 0.7);
            cc += vec3(1.0) * pow(clamp(dot(rf, Lc), 0.0, 1.0), 90.0) * 2.2 * highlights * (1.0 + 1.5 * highP() * ar);
            cc += vec3(0.9, 0.95, 1.0) * pow(clamp(dot(rf, Lc), 0.0, 1.0), 12.0) * 0.18 * highlights;
            col = mix(col, cc, chromeAmt);
            alpha = mix(alpha, 1.0, chromeAmt);
        }
    }

    col *= 1.0 + 0.12 * audioLevel * ar;
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation);
    vec2 dc = uv - 0.5;
    col *= 1.0 - 0.1 * smoothstep(0.4, 1.0, length(dc * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.08;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
