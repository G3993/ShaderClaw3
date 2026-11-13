/*{
  "DESCRIPTION": "Heatmap: an infrared thermal camera that runs straight off your camera. Load it and Easel's body-tracking camera feeds it automatically: the person silhouette from body tracking becomes real body heat (a hot core cooling toward the edges, faces and hands hottest), heat haze shimmers off you, the room cools to blue and black, and anything that moves leaves a warm trail that spreads out and fades, the way a hand-print lingers on a wall in a real thermal image. The camera auto-ranges like a FLIR (the hottest thing in frame sets the top of the scale) and a HUD shows the centre spot temperature, a box on the hottest point and a min/max colour scale. Seven palettes: Ironbow, Rainbow, White Hot, Black Hot, Arctic, Lava and Night Vision. Sensor Pixels drops the resolution to a chunky low-res sensor grid. With no camera bound, three warm figures walk through a cool room so the shader still runs. Bass pushes heat into the trails and beats flash the hotspot.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-11-07, original",
  "CATEGORIES": ["Generator", "Filter", "Live Input", "Camera", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "bodyCamera",  "LABEL": "Camera (auto)",     "TYPE": "image" },
    { "NAME": "bodyMask",    "LABEL": "Body Silhouette (auto)", "TYPE": "image" },
    { "NAME": "inputImage",  "LABEL": "Override Texture",  "TYPE": "image" },
    { "NAME": "palette",     "LABEL": "Palette", "TYPE": "long", "VALUES": [0,1,2,3,4,5,6],
      "LABELS": ["Ironbow","Rainbow","White Hot","Black Hot","Arctic","Lava","Night Vision"], "DEFAULT": 0, "GROUP": "Look" },
    { "NAME": "sensorRes",   "LABEL": "Sensor Pixels (0 = smooth)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 640.0, "DEFAULT": 0.0, "GROUP": "Look" },
    { "NAME": "softness",    "LABEL": "Thermal Blur",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "contrast",    "LABEL": "Contrast",       "TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.15, "GROUP": "Look" },
    { "NAME": "noise",       "LABEL": "Sensor Noise",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Heat Glow",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "haze",        "LABEL": "Heat Haze",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Look" },

    { "NAME": "bodyHeat",    "LABEL": "Body Heat",          "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Heat" },
    { "NAME": "skinHeat",    "LABEL": "Face / Skin Heat",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Heat" },
    { "NAME": "lumaHeat",    "LABEL": "Brightness = Heat",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Heat" },
    { "NAME": "invertLuma",  "LABEL": "Dark = Hot",         "TYPE": "bool",  "DEFAULT": false, "GROUP": "Heat" },
    { "NAME": "ambient",     "LABEL": "Room Temperature",   "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.18, "GROUP": "Heat" },

    { "NAME": "motionHeat",  "LABEL": "Movement Heat",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.4, "GROUP": "Movement" },
    { "NAME": "trail",       "LABEL": "Heat Trail (s)", "TYPE": "float", "AUDIO": false, "MIN": 0.05, "MAX": 8.0, "DEFAULT": 2.0, "GROUP": "Movement" },
    { "NAME": "spread",      "LABEL": "Heat Spread",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Movement" },
    { "NAME": "rise",        "LABEL": "Heat Rises",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Movement" },
    { "NAME": "lag",         "LABEL": "Sensor Lag",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.95, "DEFAULT": 0.35, "GROUP": "Movement" },

    { "NAME": "autoRange",   "LABEL": "Auto Range",     "TYPE": "bool",  "DEFAULT": true, "GROUP": "Range" },
    { "NAME": "rangeLo",     "LABEL": "Manual Min °C",  "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 40.0, "DEFAULT": 18.0, "GROUP": "Range" },
    { "NAME": "rangeHi",     "LABEL": "Manual Max °C",  "TYPE": "float", "AUDIO": false, "MIN": 10.0, "MAX": 60.0, "DEFAULT": 38.0, "GROUP": "Range" },
    { "NAME": "isotherm",    "LABEL": "Isotherm °C (0 = off)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 45.0, "DEFAULT": 0.0, "GROUP": "Range" },

    { "NAME": "hud",         "LABEL": "HUD",            "TYPE": "bool",  "DEFAULT": true, "GROUP": "Camera" },
    { "NAME": "mirror",      "LABEL": "Mirror Override Texture", "TYPE": "bool",  "DEFAULT": true, "GROUP": "Camera" },
    { "NAME": "zoom",        "LABEL": "Zoom",           "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Camera" },
    { "NAME": "audioReact",  "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Audio Reactivity" }
  ],
  "PASSES": [
    { "TARGET": "srcBuf",   "WIDTH": "floor($WIDTH/2)", "HEIGHT": "floor($HEIGHT/2)" },
    { "TARGET": "heatBuf",  "PERSISTENT": true, "WIDTH": "floor($WIDTH/2)", "HEIGHT": "floor($HEIGHT/2)" },
    { "TARGET": "rangeBuf", "PERSISTENT": true, "WIDTH": "1", "HEIGHT": "1" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Heatmap: infrared thermal camera
//   pass 0 srcBuf   : instant heat estimate of the scene (camera or the
//                     built-in walking figures). r = heat 0..1, g = luma
//   pass 1 heatBuf  : persistent. r = smoothed (lagged) heat,
//                     g = movement heat trail (diffuses, rises, cools),
//                     b = last frame's luma for frame differencing
//   pass 2 rangeBuf : 1x1 persistent auto-range. r = min, g = max,
//                     ba = hottest point position
//   pass 3 final    : sensor grid, palette, isotherm, noise, HUD
// heat 0..1 maps to 10..45 °C
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718

float g_ar()  { return clamp(audioReact, 0.0, 1.0); }
float bassP() { return pow(smoothstep(0.05, 0.85, audioBass), 1.4) * g_ar(); }
float beatP() { return clamp(audioBeatPulse, 0.0, 1.0) * g_ar(); }

bool useOverride() { return IMG_SIZE_inputImage.x > 0.5; }
bool hasBodyCam() { return IMG_SIZE_bodyCamera.x > 0.5; }
bool hasCam()     { return useOverride() || hasBodyCam(); }
// the silhouette is in the body camera's frame, so only trust it with that feed
bool hasMask()    { return !useOverride() && hasBodyCam() && IMG_SIZE_bodyMask.x > 0.5; }

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(i), hash12(i + vec2(1, 0)), f.x),
               mix(hash12(i + vec2(0, 1)), hash12(i + vec2(1, 1)), f.x), f.y);
}

float heatToC(float h) { return 10.0 + h * 35.0; }
float cToHeat(float c) { return (c - 10.0) / 35.0; }

// ── camera → heat ───────────────────────────────────────────────────────
vec2 camUV(vec2 uv) {
    vec2 res = RENDERSIZE;
    float outA = res.x / max(res.y, 1.0);
    vec2 isz = useOverride() ? IMG_SIZE_inputImage : IMG_SIZE_bodyCamera;
    float camA = isz.x / max(isz.y, 1.0);
    vec2 p = uv - 0.5;
    // the body camera is already mirrored by the tracker
    if (mirror && useOverride()) p.x = -p.x;
    // cover-fit the camera to the canvas
    if (outA > camA) p.y *= camA / outA; else p.x *= outA / camA;
    p /= zoom;
    return p + 0.5;
}

vec3 camTex(vec2 cu) {
    if (useOverride()) return IMG_NORM_PIXEL(inputImage, cu).rgb;
    return IMG_NORM_PIXEL(bodyCamera, cu).rgb;
}
float maskTex(vec2 cu) { return IMG_NORM_PIXEL(bodyMask, cu).r; }

vec3 camRGB(vec2 uv) {
    vec2 cu = camUV(uv);
    vec2 px = 1.5 / RENDERSIZE;
    // small 5-tap blur: thermal optics are soft and it kills camera noise
    vec3 c = camTex(cu) * 0.4;
    c += camTex(camUV(uv + vec2(px.x, 0))) * 0.15;
    c += camTex(camUV(uv - vec2(px.x, 0))) * 0.15;
    c += camTex(camUV(uv + vec2(0, px.y))) * 0.15;
    c += camTex(camUV(uv - vec2(0, px.y))) * 0.15;
    return c;
}

// soft skin detector in YCbCr: skin is the warmest thing in most rooms
float skinMask(vec3 c) {
    float y  = dot(c, vec3(0.299, 0.587, 0.114));
    float cb = 0.5 + dot(c, vec3(-0.168736, -0.331264, 0.5));
    float cr = 0.5 + dot(c, vec3(0.5, -0.418688, -0.081312));
    float m = smoothstep(0.035, 0.0, abs(cb - 0.41) - 0.09)
            * smoothstep(0.035, 0.0, abs(cr - 0.60) - 0.06);
    m *= smoothstep(0.08, 0.2, y) * smoothstep(0.98, 0.85, y);
    return m;
}

// ── built-in scene (no camera) ──────────────────────────────────────────
float sdCapsule(vec2 p, vec2 a, vec2 b, float r) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

float figure(vec2 p, vec2 base, float t, float s) {
    // a person ~0.55 tall, walking cycle t
    p = (p - base) / s;
    float sw = sin(t), sw2 = sin(t + 3.14159);
    float d = length(p - vec2(0.0, 0.48)) - 0.065;                         // head
    d = min(d, sdCapsule(p, vec2(0.0, 0.38), vec2(0.0, 0.18), 0.075));     // torso
    d = min(d, sdCapsule(p, vec2(0.0, 0.36), vec2(0.07 * sw, 0.2), 0.03)); // arms
    d = min(d, sdCapsule(p, vec2(0.0, 0.36), vec2(0.07 * sw2, 0.2), 0.03));
    d = min(d, sdCapsule(p, vec2(0.0, 0.18), vec2(0.06 * sw, 0.0), 0.04)); // legs
    d = min(d, sdCapsule(p, vec2(0.0, 0.18), vec2(0.06 * sw2, 0.0), 0.04));
    float body = smoothstep(0.02, -0.01, d);
    float headHot = smoothstep(0.1, 0.0, length(p - vec2(0.0, 0.48))) * 0.25;
    float core = smoothstep(0.0, -0.06, d) * 0.12;
    // clothes are cooler than skin; head and hands are hottest
    return body * (0.62 + core) + headHot * body;
}

vec2 sceneHeat(vec2 uv) {
    float asp = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 p = vec2(uv.x * asp, uv.y);
    float t = TIME;
    // room: cool floor, warmer ceiling, a cold window and a hot radiator
    float h = 0.16 + 0.06 * uv.y + 0.03 * vnoise(p * 3.0);
    float win = step(abs(p.x - asp * 0.22), 0.16) * step(abs(uv.y - 0.66), 0.17);
    h = mix(h, 0.04 + 0.02 * vnoise(p * 8.0 + t * 0.1), win);
    float rad = smoothstep(0.01, 0.0, max(abs(p.x - asp * 0.82) - 0.14, abs(uv.y - 0.22) - 0.07));
    rad *= 0.85 + 0.15 * sin(p.x * 120.0);
    h = max(h, rad * 0.86);
    h += smoothstep(0.35, 0.0, length(vec2(p.x - asp * 0.82, (uv.y - 0.32) * 0.6))) * 0.12; // warm air
    // a mug of coffee on a table
    float mug = smoothstep(0.005, 0.0, max(abs(p.x - asp * 0.5) - 0.025, abs(uv.y - 0.2) - 0.03));
    h = max(h, mug * 0.95);
    // three people walking back and forth
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float sp = 0.07 + 0.03 * fi;
        float ph = t * sp + fi * 2.1;
        float x = (0.5 + 0.42 * sin(ph)) * asp;
        float dir = cos(ph);
        float sc = 0.85 + 0.2 * fi;
        float walk = t * (3.0 + fi) * abs(dir) + fi;
        float bob = abs(sin(walk)) * 0.008;
        h = max(h, figure(p, vec2(x, 0.06 + bob + 0.02 * fi), walk, sc));
    }
    return vec2(h, h);
}

// ── palettes ────────────────────────────────────────────────────────────
vec3 ramp5(float t, vec3 a, vec3 b, vec3 c, vec3 d, vec3 e) {
    t = clamp(t, 0.0, 1.0) * 4.0;
    if (t < 1.0) return mix(a, b, t);
    if (t < 2.0) return mix(b, c, t - 1.0);
    if (t < 3.0) return mix(c, d, t - 2.0);
    return mix(d, e, t - 3.0);
}

// GLSL ES 1.0 has no bitwise operators
bool bitOn(int b, int k) { return mod(floor(float(b) / float(k)), 2.0) > 0.5; }

vec3 paletteColor(float t) {
    t = clamp(t, 0.0, 1.0);
    int pal = int(palette + 0.5);   // 'long' inputs arrive as float in WebGL1
    if (pal == 0) // Ironbow
        return ramp5(t, vec3(0.0, 0.0, 0.04), vec3(0.3, 0.0, 0.55), vec3(0.86, 0.1, 0.38),
                        vec3(1.0, 0.6, 0.0), vec3(1.0, 1.0, 0.88));
    if (pal == 1) // Rainbow HC
        return ramp5(t, vec3(0.0, 0.0, 0.3), vec3(0.0, 0.5, 1.0), vec3(0.0, 1.0, 0.3),
                        vec3(1.0, 0.9, 0.0), vec3(1.0, 0.05, 0.0));
    if (pal == 2) return vec3(pow(t, 0.9));            // White Hot
    if (pal == 3) return vec3(pow(1.0 - t, 0.9));      // Black Hot
    if (pal == 4) // Arctic
        return ramp5(t, vec3(0.0, 0.02, 0.1), vec3(0.0, 0.2, 0.55), vec3(0.15, 0.65, 0.95),
                        vec3(0.95, 0.75, 0.2), vec3(1.0, 0.98, 0.9));
    if (pal == 5) // Lava
        return ramp5(t, vec3(0.02, 0.0, 0.0), vec3(0.35, 0.02, 0.0), vec3(0.85, 0.15, 0.0),
                        vec3(1.0, 0.55, 0.05), vec3(1.0, 0.95, 0.6));
    // Night Vision
    return vec3(0.1, 1.0, 0.25) * pow(t, 1.1) + vec3(0.0, 0.03, 0.0);
}

// ── HUD digits (seven segment) ──────────────────────────────────────────
float segLine(vec2 p, vec2 a, vec2 b, float w) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return smoothstep(w, w * 0.4, length(pa - ba * h));
}
int segBits(int d) {
    if (d == 0) return 0x3F; if (d == 1) return 0x06; if (d == 2) return 0x5B;
    if (d == 3) return 0x4F; if (d == 4) return 0x66; if (d == 5) return 0x6D;
    if (d == 6) return 0x7D; if (d == 7) return 0x07; if (d == 8) return 0x7F;
    if (d == 9) return 0x6F; if (d == 10) return 0x39; /* C */
    return 0x40; /* minus */
}
// p in digit space: x 0..0.5, y 0..1
float digit(vec2 p, int d) {
    if (p.x < -0.15 || p.x > 0.65 || p.y < -0.15 || p.y > 1.15) return 0.0;
    int b = segBits(d);
    float w = 0.085, m = 0.0;
    if (bitOn(b, 1)) m = max(m, segLine(p, vec2(0.0, 1.0), vec2(0.5, 1.0), w));
    if (bitOn(b, 2)) m = max(m, segLine(p, vec2(0.5, 1.0), vec2(0.5, 0.5), w));
    if (bitOn(b, 4)) m = max(m, segLine(p, vec2(0.5, 0.5), vec2(0.5, 0.0), w));
    if (bitOn(b, 8)) m = max(m, segLine(p, vec2(0.0, 0.0), vec2(0.5, 0.0), w));
    if (bitOn(b, 16)) m = max(m, segLine(p, vec2(0.0, 0.0), vec2(0.0, 0.5), w));
    if (bitOn(b, 32)) m = max(m, segLine(p, vec2(0.0, 0.5), vec2(0.0, 1.0), w));
    if (bitOn(b, 64)) m = max(m, segLine(p, vec2(0.0, 0.5), vec2(0.5, 0.5), w));
    return m;
}
// prints "34.5°C" with its lower-left corner at org, digit height h (pixels)
float printTemp(vec2 frag, vec2 org, float h, float v) {
    vec2 p = (frag - org) / h;
    if (p.x < -0.3 || p.x > 4.4 || p.y < -0.3 || p.y > 1.3) return 0.0;
    v = clamp(v, -9.9, 99.9);
    float a = abs(v);
    int tens = int(floor(a / 10.0));
    int ones = int(floor(mod(a, 10.0)));
    int dec  = int(floor(mod(a * 10.0 + 0.5, 10.0)));
    float m = 0.0;
    float x = 0.0;
    if (v < 0.0) { m = max(m, digit(p - vec2(x, 0.0), 11)); x += 0.75; }
    else if (tens > 0) { m = max(m, digit(p - vec2(x, 0.0), tens)); x += 0.75; }
    m = max(m, digit(p - vec2(x, 0.0), ones)); x += 0.7;
    m = max(m, smoothstep(0.09, 0.05, length(p - vec2(x, 0.0))));    // dot
    x += 0.22;
    m = max(m, digit(p - vec2(x, 0.0), dec)); x += 0.75;
    m = max(m, smoothstep(0.03, 0.0, abs(length(p - vec2(x + 0.05, 0.9)) - 0.09))); // °
    x += 0.3;
    m = max(m, digit(p * vec2(1.0, 1.0) - vec2(x, 0.0), 10));       // C
    return m;
}

float boxOutline(vec2 frag, vec2 c, vec2 half_, float w) {
    vec2 d = abs(frag - c) - half_;
    float outer = max(d.x, d.y);
    return smoothstep(w, 0.0, abs(outer));
}

// ── main ────────────────────────────────────────────────────────────────
void main() {
    vec2 frag = gl_FragCoord.xy;
    vec2 uv = frag / RENDERSIZE;

    // ── pass 0: instant heat ────────────────────────────────────────────
    if (PASSINDEX == 0) {
        vec2 hl;
        float body = 0.0;
        if (hasCam()) {
            vec3 c = camRGB(uv);
            float y = dot(c, vec3(0.299, 0.587, 0.114));
            // brightness only feeds heat as a broad, blurred term: thermal
            // images carry almost none of the visible texture
            vec3 cw = vec3(0.0);
            float mWide = 0.0;
            bool hm = hasMask();
            for (int j = -1; j <= 1; j++)
            for (int i = -1; i <= 1; i++) {
                vec2 cu = camUV(uv + vec2(float(i), float(j)) * 12.0 / RENDERSIZE);
                cw += camTex(cu);
                if (hm) mWide += maskTex(cu);
            }
            cw /= 9.0; mWide /= 9.0;
            float yw = dot(cw, vec3(0.299, 0.587, 0.114));
            float yl = invertLuma ? 1.0 - yw : yw;
            float skin = skinMask(c);
            float veg = smoothstep(0.02, 0.12, cw.g - max(cw.r, cw.b));   // leaves, plants read cool
            float h = ambient + lumaHeat * 0.3 * yl - veg * 0.08;
            if (hm) {
                vec2 cu = camUV(uv);
                float m = smoothstep(0.25, 0.75, maskTex(cu));
                // a body is hottest in its core and cools toward the edges
                // (surfaces turn away from the sensor); clothes and hair
                // show as small dips, bare skin as peaks
                float core = smoothstep(0.3, 1.0, mWide);
                // clothes ~29-31 C, bare skin ~33-36 C, darker fabric a touch warmer
                float cloth = (y - yw) * 0.06 + (0.5 - yw) * 0.03;
                float skinB = max(skin, skinMask(cw) * 0.8);
                float bh = 0.5 + 0.06 * core + cloth + skinHeat * skinB * 0.2;
                bh *= bodyHeat;
                h = mix(h, max(h, bh), m);
                // warm air halo hugging the body
                h += smoothstep(0.0, 0.6, mWide) * (1.0 - m) * 0.06 * bodyHeat;
                body = m;
            } else {
                h += bodyHeat * skinHeat * (skin * 0.3 + skinMask(cw) * 0.35);
            }
            // very bright sources (lamps, screens) read hot
            h += smoothstep(0.88, 1.0, yw) * lumaHeat * 0.35 * (invertLuma ? 0.0 : 1.0);
            hl = vec2(max(h, 0.0), y);
        } else {
            hl = sceneHeat(uv);
            hl.x = mix(ambient, 1.0, hl.x) * mix(0.8, 1.0, bodyHeat / 1.5);
        }
        gl_FragColor = vec4(hl, body, 1.0);
        return;
    }

    // ── pass 1: smoothed heat + movement trail ──────────────────────────
    if (PASSINDEX == 1) {
        vec2 px = 1.0 / RENDERSIZE;
        float dt = clamp(TIMEDELTA, 0.001, 0.1);
        vec4 prev = texture2D(heatBuf, uv);
        bool first = FRAMEINDEX < 3;

        vec4 cur = texture2D(srcBuf, uv);
        // frame difference, lightly blurred so it reads as body motion,
        // not pixel shimmer
        float lumNow = 0.0, lumThen = 0.0;
        for (int j = -1; j <= 1; j++)
        for (int i = -1; i <= 1; i++) {
            vec2 o = vec2(float(i), float(j)) * px * 2.0;
            lumNow  += texture2D(srcBuf, uv + o).g;
            lumThen += texture2D(heatBuf, uv + o).b;
        }
        float motion = abs(lumNow - lumThen) / 9.0;
        motion = smoothstep(0.035, 0.2, motion);
        // silhouette motion: where the body just was / just arrived
        float mNow = 0.0, mThen = 0.0;
        for (int i = -1; i <= 1; i++) {
            vec2 o = vec2(float(i), float(i * i) - 0.5) * px * 3.0;
            mNow  += texture2D(srcBuf, uv + o).b;
            mThen += texture2D(heatBuf, uv + o).a;
        }
        if (hasMask()) motion = max(motion * 0.5, smoothstep(0.05, 0.5, abs(mNow - mThen) / 3.0));
        if (first) motion = 0.0;

        // heat trail: diffuse to neighbours, drift upward, cool over time
        float r = spread * 3.0;
        vec2 up = vec2(0.0, -rise * 2.0) * px;   // sample from below = heat rises
        float nb = texture2D(heatBuf, uv + up + vec2(r, 0) * px).g
                 + texture2D(heatBuf, uv + up - vec2(r, 0) * px).g
                 + texture2D(heatBuf, uv + up + vec2(0, r) * px).g
                 + texture2D(heatBuf, uv + up - vec2(0, r) * px).g;
        float tr = mix(texture2D(heatBuf, uv + up).g, nb * 0.25, clamp(spread, 0.0, 1.0) * 0.6);
        tr *= exp(-dt * 3.0 / max(trail, 0.05));
        float add = motion * (0.6 + 0.8 * bassP()) * dt * 4.0;
        tr = clamp(tr + add * (1.0 - tr), 0.0, 1.0);

        // sensor lag (thermal sensors respond slowly)
        float k = first ? 1.0 : 1.0 - pow(clamp(lag, 0.0, 0.95), dt * 30.0);
        float hs = mix(prev.r, cur.r, k);

        gl_FragColor = vec4(hs, first ? 0.0 : tr, cur.g, cur.b);
        return;
    }

    // ── pass 2: auto range (1x1) ────────────────────────────────────────
    if (PASSINDEX == 2) {
        float lo = 1e3, hi = -1e3;
        vec2 hiPos = vec2(0.5);
        for (int j = 0; j < 14; j++)
        for (int i = 0; i < 14; i++) {
            vec2 q = (vec2(float(i), float(j)) + 0.5) / 14.0;
            vec4 h = texture2D(heatBuf, q);
            float T = h.r + h.g * motionHeat * 0.22;
            lo = min(lo, T);
            if (T > hi) { hi = T; hiPos = q; }
        }
        vec4 prev = texture2D(rangeBuf, vec2(0.5));
        float dt = clamp(TIMEDELTA, 0.001, 0.1);
        float k = (FRAMEINDEX < 4 || prev.g <= prev.r) ? 1.0 : 1.0 - exp(-dt * 2.0);
        vec2 rg = mix(prev.rg, vec2(lo, hi), k);
        vec2 pos = mix(prev.ba, hiPos, (FRAMEINDEX < 4) ? 1.0 : 1.0 - exp(-dt * 6.0));
        gl_FragColor = vec4(rg, pos);
        return;
    }

    // ── pass 3: display ─────────────────────────────────────────────────
    vec2 suv = uv;
    // heat haze: air above hot things shimmers
    if (haze > 0.0) {
        vec4 below = texture2D(heatBuf, uv - vec2(0.0, 0.05));
        float hotB = smoothstep(0.35, 0.7, below.r + below.g * 0.3);
        vec2 hn = vec2(vnoise(uv * vec2(22.0, 10.0) - vec2(0.0, TIME * 3.0)),
                       vnoise(uv * vec2(22.0, 10.0) + vec2(7.3, -TIME * 2.4))) - 0.5;
        suv += hn * hotB * haze * 0.007;
    }
    if (sensorRes >= 8.0) {
        vec2 grid = vec2(sensorRes, floor(sensorRes * RENDERSIZE.y / RENDERSIZE.x));
        suv = (floor(uv * grid) + 0.5) / grid;
    }
    vec2 px = 1.0 / RENDERSIZE;
    float bl = softness * 3.0;
    vec4 hc = texture2D(heatBuf, suv) * 0.36;
    hc += texture2D(heatBuf, suv + vec2( bl,  bl) * px) * 0.16;
    hc += texture2D(heatBuf, suv + vec2(-bl,  bl) * px) * 0.16;
    hc += texture2D(heatBuf, suv + vec2( bl, -bl) * px) * 0.16;
    hc += texture2D(heatBuf, suv + vec2(-bl, -bl) * px) * 0.16;
    float T = hc.r + hc.g * motionHeat * 0.22;

    // sensor noise + fixed-pattern column noise
    float n = (hash12(floor(frag / 2.0) + floor(TIME * 30.0) * 17.0) - 0.5) * 0.02
            + (hash12(vec2(floor(suv.x * RENDERSIZE.x / 2.0), 3.0)) - 0.5) * 0.008;
    T += n * noise;

    vec4 rng = texture2D(rangeBuf, vec2(0.5));
    float lo, hi;
    if (autoRange) {
        lo = rng.r; hi = max(rng.g, rng.r + 0.12);
        lo -= 0.02; hi += 0.02;
    } else {
        lo = cToHeat(rangeLo); hi = max(cToHeat(rangeHi), lo + 0.02);
    }
    float t = (T - lo) / (hi - lo);
    t = clamp((t - 0.5) * contrast + 0.5, 0.0, 1.0);
    vec3 col = paletteColor(t);

    // glow: hot areas bloom into the cold around them
    if (glow > 0.0) {
        float gsum = 0.0;
        for (int i = 0; i < 8; i++) {
            float a = float(i) * TAU / 8.0 + 0.4;
            vec2 o = vec2(cos(a), sin(a));
            vec4 g1 = texture2D(heatBuf, suv + o * 14.0 * px);
            vec4 g2 = texture2D(heatBuf, suv + o * 34.0 * px);
            gsum += (g1.r + g1.g * motionHeat * 0.22) * 0.6 + (g2.r + g2.g * motionHeat * 0.22) * 0.4;
        }
        float gt = clamp((gsum / 8.0 - lo) / (hi - lo), 0.0, 1.0);
        col += paletteColor(gt) * smoothstep(0.45, 1.0, gt) * glow * (0.3 + 0.25 * beatP());
    }

    // isotherm: everything above the threshold is flagged
    if (isotherm > 0.5) {
        float iso = smoothstep(-0.004, 0.004, T - cToHeat(isotherm));
        float stripes = 0.75 + 0.25 * step(0.5, fract((frag.x + frag.y) / 10.0 - TIME * 1.5));
        col = mix(col, vec3(0.1, 1.0, 0.3) * stripes * (0.5 + 0.5 * t), iso * 0.85);
    }

    // optics: vignette + faint scanlines
    vec2 vc = uv - 0.5;
    col *= 1.0 - dot(vc, vc) * 0.55;
    col *= 0.97 + 0.03 * sin(frag.y * 3.14159);

    if (hud) {
        float sc = RENDERSIZE.y / 720.0;
        vec3 hudCol = vec3(1.0);
        float m = 0.0;
        // centre crosshair + spot temp
        vec2 cc = RENDERSIZE * 0.5;
        vec2 d = abs(frag - cc);
        float ch = max(step(d.x, 1.2 * sc) * step(8.0 * sc, d.y) * step(d.y, 26.0 * sc),
                       step(d.y, 1.2 * sc) * step(8.0 * sc, d.x) * step(d.x, 26.0 * sc));
        m = max(m, ch);
        vec4 ctr = texture2D(heatBuf, vec2(0.5));
        float cT = heatToC(ctr.r + ctr.g * motionHeat * 0.22);
        m = max(m, printTemp(frag, cc + vec2(34.0, 10.0) * sc, 26.0 * sc, cT));

        // hottest point box
        vec2 hp = rng.ba * RENDERSIZE;
        float bx = boxOutline(frag, hp, vec2(16.0 + 4.0 * beatP()) * sc, 1.6 * sc);
        col = mix(col, vec3(1.0, 0.15 + 0.6 * beatP(), 0.1), bx);
        m = max(m, printTemp(frag, hp + vec2(24.0, 4.0) * sc, 18.0 * sc, heatToC(rng.g)) * 0.95);

        // scale bar on the right
        float bw = 14.0 * sc, bh = 300.0 * sc;
        vec2 bo = vec2(RENDERSIZE.x - 48.0 * sc, RENDERSIZE.y * 0.5 - bh * 0.5);
        vec2 bp = frag - bo;
        if (bp.x >= 0.0 && bp.x <= bw && bp.y >= 0.0 && bp.y <= bh)
            col = paletteColor(bp.y / bh);
        m = max(m, boxOutline(frag, bo + vec2(bw, bh) * 0.5, vec2(bw, bh) * 0.5 + 1.5 * sc, 1.2 * sc));
        m = max(m, printTemp(frag, bo + vec2(-82.0 * sc, bh - 16.0 * sc), 16.0 * sc, heatToC(hi)));
        m = max(m, printTemp(frag, bo + vec2(-82.0 * sc, 0.0), 16.0 * sc, heatToC(lo)));

        // corner brackets
        vec2 f = min(frag, RENDERSIZE - frag) - 20.0 * sc;
        float br = max(step(abs(f.x), 1.2 * sc) * step(0.0, f.y) * step(f.y, 26.0 * sc),
                       step(abs(f.y), 1.2 * sc) * step(0.0, f.x) * step(f.x, 26.0 * sc));
        m = max(m, br);

        col = mix(col, hudCol, clamp(m, 0.0, 1.0));
    }

    gl_FragColor = vec4(col, 1.0);
}
