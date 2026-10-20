/*{
  "DESCRIPTION": "Veins — a living network of chrome roots and veins in 3D. Three families of wavy vessels thread through a slowly writhing space and meet at real junctions, so the tangle branches like roots: thick trunks, medium veins and fine capillaries, all in mirror chrome reflecting a dark studio with magenta, cyan and amber lights. Pulses travel along every vessel as peristaltic bulges, glowing faintly from inside as they pass, and the camera glides endlessly forward through a clear channel in the tangle while the whole network breathes and drifts. Chrome inter-reflections, Fresnel, three-colour glints, ambient occlusion, depth fog, bloom and film grain. Bass swells the pulses, mids speed the flow, highs twinkle the glints, beats fire a bright pulse down the vessels.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-15 — original. CIDKID pack",
  "CATEGORIES": ["Generator", "3D", "Organic", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",             "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,   "DEFAULT": 1.0,   "GROUP": "Speed" },
    { "NAME": "flow",        "LABEL": "Pulse Flow",        "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,   "DEFAULT": 1.0,   "GROUP": "Speed" },
    { "NAME": "camSpeed",    "LABEL": "Camera Glide",      "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.5,   "GROUP": "Speed" },
    { "NAME": "drift",       "LABEL": "Writhe",            "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.7,   "GROUP": "Speed" },
    { "NAME": "density",     "LABEL": "Density",           "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0,   "DEFAULT": 0.8,   "GROUP": "Veins" },
    { "NAME": "thickness",   "LABEL": "Thickness",         "TYPE": "float", "MIN": 0.04,  "MAX": 0.4,   "DEFAULT": 0.17,  "GROUP": "Veins" },
    { "NAME": "veinMix",     "LABEL": "Roots vs Veins",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.55,  "GROUP": "Veins" },
    { "NAME": "capillaries", "LABEL": "Capillaries",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.7,   "GROUP": "Veins" },
    { "NAME": "pulseAmt",    "LABEL": "Pulse Swell",       "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.4,   "GROUP": "Veins" },
    { "NAME": "tunnel",      "LABEL": "Clear Channel",     "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 1.5,   "DEFAULT": 0.9,   "GROUP": "Veins" },
    { "NAME": "breath",      "LABEL": "Breath",            "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.4,   "GROUP": "Veins" },
    { "NAME": "chromeTint",  "LABEL": "Chrome Tint",       "TYPE": "color", "DEFAULT": [0.92, 0.93, 0.96, 1.0], "GROUP": "Color" },
    { "NAME": "light1",      "LABEL": "Light 1",           "TYPE": "color", "DEFAULT": [1.0, 0.15, 0.55, 1.0],  "GROUP": "Color" },
    { "NAME": "light2",      "LABEL": "Light 2",           "TYPE": "color", "DEFAULT": [0.15, 0.75, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "light3",      "LABEL": "Light 3",           "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.15, 1.0],   "GROUP": "Color" },
    { "NAME": "glowColor",   "LABEL": "Pulse Glow",        "TYPE": "color", "DEFAULT": [1.0, 0.12, 0.08, 1.0],  "GROUP": "Color" },
    { "NAME": "glow",        "LABEL": "Glow Amount",       "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.7,   "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.0,   "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,   "MAX": 1.5,   "DEFAULT": 1.0,   "GROUP": "Color" },
    { "NAME": "backColor",   "LABEL": "Background Color",  "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",     "LABEL": "Background Amount", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 1.0,   "GROUP": "Color" },
    { "NAME": "keyLight",    "LABEL": "Key Light",         "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "envLight",    "LABEL": "Studio Lights",     "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "reflections", "LABEL": "Inter-Reflections", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "fog",         "LABEL": "Depth Fog",         "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.8,   "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.25,  "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.2,   "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0,   "DEFAULT": 0.75,  "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.6,   "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "vnScene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Veins — CIDKID pack
//   pass 0  vnScene — raymarched vessel network: three families of curves
//                     {n_i = 0} ∩ {n_j = 0} of warped sine fields (they share
//                     level sheets, so they meet at true junctions) + a finer
//                     capillary family; peristaltic pulses along each family's
//                     run direction; chrome with one bounce; a clear channel
//                     along the camera path.             rgb = HDR, a = hit
//   pass 1  final   — bloom, ACES, fringe, grain, vignette, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define FAR  17.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU; float s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev, g_flowT, g_breath;
vec3  g_ro, g_uu, g_vv, g_ww, g_back, g_key, g_L1, g_L2, g_L3, g_glowC;
float g_keyI;
// last evaluation: family id (0 trunk, 1 vein, 2 vein, 3 capillary) + pulse
float m_fam, m_pulse, g_hitT;

// camera path through the tangle (the network is carved clear around it)
vec2 pathXY(float z) { return vec2(0.55 * sin(z * 0.19) + 0.25 * sin(z * 0.47 + 1.3), 0.4 * sin(z * 0.23 + 0.7) + 0.2 * sin(z * 0.41)); }

// organic drifting domain warp shared by every field
vec3 warp(vec3 p) {
    float t = g_t * 0.1 * drift;
    p += 0.5 * sin(p.yzx * 0.8 + vec3(0.0, 1.7, 3.1) + t);
    p += 0.22 * sin(p.zxy * 1.9 + vec3(2.3, 0.4, 5.0) - t * 0.7);
    return p;
}

// three warped sine sheets (zero level sets) with different spacings
void fields(vec3 q, out float n1, out float n2, out float n3, out float k1, out float k2, out float k3) {
    k1 = 1.05 * density; k2 = 1.3 * density; k3 = 1.6 * density;
    n1 = sin(q.x * k1 + 0.55 * sin(q.y * k1 * 0.7 + q.z * 0.5));
    n2 = sin(q.y * k2 + 0.55 * sin(q.z * k2 * 0.7 + q.x * 0.5 + 1.0));
    n3 = sin(q.z * k3 + 0.55 * sin(q.x * k3 * 0.7 + q.y * 0.5 + 2.0));
}

// peristaltic pulse along a run coordinate
float pulseAt(float along, float seed) {
    float ph = along * 1.6 - g_flowT * 1.8 + seed;
    float b = 0.5 + 0.5 * sin(ph);
    b = b * b; b = b * b * b;
    // a second, rarer, bigger pulse
    float ph2 = along * 0.55 - g_flowT * 1.1 + seed * 2.0;
    float b2 = 0.5 + 0.5 * sin(ph2); b2 = pow(b2, 14.0);
    return b + 1.6 * b2;
}

float map(vec3 p) {
    vec3 q = warp(p);
    float n1, n2, n3, k1, k2, k3;
    fields(q, n1, n2, n3, k1, k2, k3);
    // vessels thin out and end before the camera channel (no sliced caps)
    float taper = smoothstep(tunnel * 0.45, tunnel * 1.5, length(p.xy - pathXY(p.z)));
    float thick = thickness * g_breath * taper;
    float rT = thick * (0.8 + 0.3 * sin(q.z * 0.9 + q.x * 0.6));                       // trunks (along z)
    float rV = thick * mix(0.75, 0.45, veinMix) * (0.85 + 0.25 * sin(q.x * 1.1 + q.y * 0.8));
    float rW = thick * mix(0.7, 0.4, veinMix) * (0.85 + 0.25 * sin(q.y * 1.2 + q.z * 0.7));
    float pA = pulseAt(q.z, 0.0), pB = pulseAt(q.x, 2.1), pC = pulseAt(q.y, 4.2);
    float swell = pulseAmt * (1.0 + 1.2 * g_ar * g_bass + 1.5 * g_ar * g_beat);
    rT *= 1.0 + swell * pA; rV *= 1.0 + swell * pB; rW *= 1.0 + swell * pC;
    float dA = length(vec2(n1 / k1, n2 / k2)) - rT;        // trunks: n1 = n2 = 0, run along z
    float dB = length(vec2(n2 / k2, n3 / k3)) - rV;        // veins: run along x
    float dC = length(vec2(n1 / k1, n3 / k3)) - rW;        // veins: run along y
    float d = dA; m_fam = 0.0; m_pulse = pA;
    if (dB < d) { d = dB; m_fam = 1.0; m_pulse = pB; }
    if (dC < d) { d = dC; m_fam = 2.0; m_pulse = pC; }
    // capillaries: a finer family wrapped between the sheets
    if (capillaries > 0.001) {
        float kc = 2.3 * density;
        float c1 = sin(q.x * kc + 1.1 + 0.5 * sin(q.z * kc * 0.6 + q.y * 0.4));
        float c2 = sin(q.y * kc + 0.3 + 0.5 * sin(q.x * kc * 0.6 + q.z * 0.4 + 1.0));
        float rc = thick * 0.22 * capillaries * (0.8 + 0.3 * sin(q.z * 1.7)) * (1.0 + 0.6 * swell * pulseAt(q.z * 1.3, 1.3));
        float dD = length(vec2(c1 / kc, c2 / kc)) - rc;
        if (dD < d) { d = dD; m_fam = 3.0; m_pulse = pulseAt(q.z * 1.3, 1.3); }
    }
    // hard floor of the channel so the camera is never inside a vessel
    d = max(d, tunnel * 0.35 - length(p.xy - pathXY(p.z)));
    return d * 0.45;
}

vec3 normalAt(vec3 p, float t) {
    float e = 0.0012 + 0.0006 * t;
    vec2 k = vec2(1.0, -1.0);
    return normalize(k.xyy * map(p + k.xyy * e) + k.yyx * map(p + k.yyx * e) + k.yxy * map(p + k.yxy * e) + k.xxx * map(p + k.xxx * e));
}

float march(vec3 ro, vec3 rd, float maxT, int steps) {
    float t = 0.03;
    for (int i = 0; i < 120; i++) {
        if (i >= steps) break;
        float d = map(ro + rd * t);
        if (d < 0.0014 * (1.0 + t)) return t;
        t += d;
        if (t > maxT) break;
    }
    return -1.0;
}

float ambientOcc(vec3 p, vec3 n) {
    float occ = 0.0, sca = 1.0;
    for (int i = 1; i <= 3; i++) {
        float h = 0.02 + 0.1 * float(i);
        occ += (h - map(p + n * h)) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - 2.2 * occ, 0.0, 1.0);
}

// ── studio environment: black, one long softbox overhead, three coloured lamps ──
vec3 L1dir() { return normalize(vec3(-0.8, 0.1, -0.5)); }
vec3 L2dir() { return normalize(vec3(0.85, 0.25, -0.3)); }
vec3 L3dir() { return normalize(vec3(0.1, -0.5, 0.85)); }
vec3 envCol(vec3 rd) {
    vec3 c = g_back * 0.6;
    float strip = smoothstep(0.55, 0.9, rd.y) * smoothstep(0.9, 0.2, abs(rd.x)) ;
    c += vec3(1.0, 0.98, 0.95) * 1.9 * strip * envLight;
    c += vec3(0.12, 0.13, 0.16) * envLight * smoothstep(-0.2, 0.9, rd.y);
    c += g_L1 * envLight * 1.1 * pow(max(dot(rd, L1dir()), 0.0), 6.0);
    c += g_L2 * envLight * 1.1 * pow(max(dot(rd, L2dir()), 0.0), 6.0);
    c += g_L3 * envLight * 0.9 * pow(max(dot(rd, L3dir()), 0.0), 7.0);
    // faint floor bounce
    c += vec3(0.08, 0.06, 0.07) * envLight * smoothstep(-0.1, -0.8, rd.y);
    float kd = max(dot(rd, g_key), 0.0);
    c += vec3(1.0) * g_keyI * (pow(kd, 220.0) * 1.5 + pow(kd, 14.0) * 0.06);
    return c;
}

vec3 famF0(float fam) {
    float dark = (fam < 0.5) ? 0.9 : ((fam > 2.5) ? 0.84 : 1.0);
    return chromeTint.rgb * dark;
}
// one-bounce shade: environment + glints + pulse glow, no further bounce
vec3 shadeBounce(vec3 p, vec3 rd, vec3 n, float fam, float pulse) {
    vec3 v = -rd;
    float ndv = max(dot(n, v), 0.0);
    vec3 F0 = famF0(fam);
    vec3 F = F0 + (1.0 - F0) * pow(1.0 - ndv, 5.0);
    vec3 col = F * envCol(reflect(rd, n));
    vec3 h = normalize(g_key + v);
    col += vec3(1.0) * g_keyI * pow(max(dot(n, h), 0.0), 160.0) * 3.0 * F;
    vec3 h1 = normalize(L1dir() + v), h2 = normalize(L2dir() + v), h3 = normalize(L3dir() + v);
    col += (g_L1 * pow(max(dot(n, h1), 0.0), 90.0) + g_L2 * pow(max(dot(n, h2), 0.0), 90.0) + g_L3 * pow(max(dot(n, h3), 0.0), 90.0)) * envLight * 1.4 * F;
    float pg = clamp(pulse, 0.0, 2.6);
    col += g_glowC * pg * pg * glow * 0.35 * (0.35 + 0.65 * ndv);
    return col;
}

vec3 shadeChrome(vec3 p, vec3 rd, vec3 n, float fam, float pulse) {
    vec3 v = -rd;
    float ndv = max(dot(n, v), 0.0);
    vec3 F0 = famF0(fam);
    vec3 F = F0 + (1.0 - F0) * pow(1.0 - ndv, 5.0);
    vec3 rr = reflect(rd, n);
    vec3 refl = envCol(rr);
    float ao = ambientOcc(p, n);
    if (reflections > 0.001 && g_hitT < 9.0) {
        float t2 = march(p + n * 0.03, rr, 4.5, 26);
        if (t2 > 0.0) {
            vec3 p2 = p + n * 0.03 + rr * t2;
            vec3 n2 = normalAt(p2, t2);
            float fam2 = m_fam, pulse2 = m_pulse;
            vec3 c2 = shadeBounce(p2, rr, n2, fam2, pulse2);
            refl = mix(refl, c2 * exp(-t2 * 0.3 * fog), reflections * 0.9);
        }
    }
    vec3 col = F * refl * (0.3 + 0.7 * ao);
    // key light + three coloured lamps as sharp chrome glints
    vec3 h = normalize(g_key + v);
    float sN = pow(max(dot(n, h), 0.0), 160.0);
    float sW = pow(max(dot(n, h), 0.0), 14.0);
    col += vec3(1.0) * g_keyI * (sN * 3.5 + sW * 0.25) * F;
    vec3 h1 = normalize(L1dir() + v), h2 = normalize(L2dir() + v), h3 = normalize(L3dir() + v);
    float tw = 0.7 + 0.3 * sin(g_t * 7.0 + p.x * 9.0 + p.z * 5.0);
    float hi = (1.0 + 1.5 * g_ar * g_high * tw) * (1.0 + 0.6 * g_ar * g_bass);
    col += g_L1 * envLight * pow(max(dot(n, h1), 0.0), 90.0) * 1.6 * F * hi;
    col += g_L2 * envLight * pow(max(dot(n, h2), 0.0), 90.0) * 1.6 * F * hi;
    col += g_L3 * envLight * pow(max(dot(n, h3), 0.0), 90.0) * 1.2 * F * hi;
    // the pulse glows through the chrome as it passes (thin-walled vessel)
    float pg = clamp(pulse, 0.0, 2.6);
    col += g_glowC * pg * pg * glow * 0.35 * (0.35 + 0.65 * ndv) * (1.0 + 3.5 * g_ar * g_beat) * (1.0 - F * 0.5);
    // beats also flush the whole vessel faintly, bass warms the lamps
    col += g_glowC * glow * 0.12 * g_ar * g_beat * (0.3 + 0.7 * ndv);
    return col;
}

void setupCam() {
    float z = -g_t * camSpeed * 1.4;
    vec2 xy = pathXY(z);
    g_ro = vec3(xy, z);
    vec2 xy2 = pathXY(z - 1.6);
    vec3 ta = vec3(xy2 + vec2(0.08 * sin(g_t * 0.31), 0.06 * sin(g_t * 0.27 + 1.0)), z - 1.6);
    g_ww = normalize(ta - g_ro);
    float roll = 0.08 * sin(g_t * 0.13);
    vec3 up = vec3(sin(roll), cos(roll), 0.0);
    g_uu = normalize(cross(up, g_ww));
    g_vv = cross(g_ww, g_uu);
}
vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * 1.15); }

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME * speed;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_flowT = TIME * flow * (1.0 + 0.5 * g_ar * g_mid) * max(speed, 0.001);
    g_breath = 1.0 + breath * 0.12 * sin(g_t * 0.6) + 0.22 * g_ar * g_bass;
    g_back = backColor.rgb * backAmt;
    g_key = normalize(vec3(0.35, 0.9, -0.25));
    g_keyI = keyLight * (1.0 + 0.3 * g_ar * g_lev);
    g_L1 = hueRot(light1.rgb, hueShift); g_L2 = hueRot(light2.rgb, hueShift); g_L3 = hueRot(light3.rgb, hueShift);
    g_glowC = hueRot(glowColor.rgb, hueShift);
    setupCam();

    if (PASSINDEX == 0) {
        vec3 rd = camRay(uv);
        float t = march(g_ro, rd, FAR, 96);
        vec3 col; float hit;
        if (t > 0.0) {
            vec3 p = g_ro + rd * t;
            vec3 n = normalAt(p, t);
            float fam = m_fam, pulse = m_pulse;
            g_hitT = t;
            col = shadeChrome(p, rd, n, fam, pulse);
            float ff = 1.0 - exp(-t * 0.13 * fog);
            col = mix(col, g_back + vec3(0.01, 0.008, 0.012) * fog, ff);
            hit = 1.0;
        } else {
            col = g_back;
            hit = 0.0;
        }
        gl_FragColor = vec4(max(col, 0.0), hit);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec4 c0 = texture2D(vnScene, st);
        vec3 col = c0.rgb;
        float ignR = fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y)) * 6.28;
        float ab = aberration * 0.0035;
        vec2 dir = (st - 0.5) * 2.0;
        vec3 fr = vec3(texture2D(vnScene, st + dir * ab).r, c0.g, texture2D(vnScene, st - dir * ab).b);
        col = mix(col, fr, 0.7 * smoothstep(0.2, 0.9, c0.a) + 0.2);
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR;
            float rr = sqrt((fi + 0.5) / 16.0) * 0.045;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(vnScene, st + of).rgb;
            bl += max(s - 1.0, 0.0) * (1.0 - rr / 0.045);
        }
        bl /= 16.0;
        vec3 bl2 = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR + 1.0;
            float rr = sqrt((fi + 0.5) / 8.0) * 0.012;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            bl2 += max(texture2D(vnScene, st + of).rgb - 0.9, 0.0);
        }
        bl2 /= 8.0;
        col += (bl * 1.3 + bl2 * 0.5) * bloom;
        col *= 1.0 + 0.08 * g_ar * g_lev;
        col = aces(col * 1.05);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, saturation), 0.0, 1.0);
        float g1 = (hash12(fragCoord + fract(TIME) * 61.0) - 0.5) * 0.05;
        col += g1 * grain * (0.5 + 0.5 * (1.0 - abs(l0 - 0.5) * 2.0));
        col *= 1.0 - 0.2 * smoothstep(0.45, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.5);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? smoothstep(0.1, 0.9, c0.a) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
