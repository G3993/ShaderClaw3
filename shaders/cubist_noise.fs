/*{
  "DESCRIPTION": "Cubist Noise — a solid built from thousands of short glossy tubes, like steel quills or combed hair, each one aligned to a swirling 3D noise flow that wraps around the surface in vortices and eddies. The whole lattice morphs between a Cube, a Sphere and a Triangle (tetrahedron), spinning slowly while the flow rolls across it. Erosion opens holes that reveal deeper layers of tubes. Bass swells the tubes and the shape, beats make the quills bristle outward, mids churn the flow faster, highs glint off the tips. Show Dynamics: at Rest the solid shrinks back into a sparse, eroded, slowly combed lattice of thin pale tubes; during a Build it contracts and spins up while the flow coils tighter, the camera pushes in, glints shimmer faster and the colour streams saturate; at the Drop every quill bristles straight out in a white flash as the shape punches outward.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "shapeSel",    "LABEL": "Shape", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Cube", "Sphere", "Triangle"], "GROUP": "Shape" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 40.0, "DEFAULT": 9.0, "GROUP": "Shape" },
    { "NAME": "shapeSize",   "LABEL": "Size",            "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Shape" },
    { "NAME": "density",     "LABEL": "Tube Density",    "TYPE": "float", "AUDIO": false, "MIN": 10.0, "MAX": 44.0, "DEFAULT": 30.0, "GROUP": "Shape" },
    { "NAME": "layers",      "LABEL": "Depth Layers",    "TYPE": "float", "MIN": 1.0, "MAX": 6.0, "DEFAULT": 3.5, "GROUP": "Shape" },
    { "NAME": "erosion",     "LABEL": "Erosion",         "TYPE": "float", "MIN": 0.0, "MAX": 0.8, "DEFAULT": 0.28, "GROUP": "Shape" },
    { "NAME": "tubeLen",     "LABEL": "Tube Length",     "TYPE": "float", "MIN": 0.3, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Tubes" },
    { "NAME": "tubeWidth",   "LABEL": "Tube Width",      "TYPE": "float", "MIN": 0.05, "MAX": 0.35, "DEFAULT": 0.16, "GROUP": "Tubes" },
    { "NAME": "swirl",       "LABEL": "Swirl",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Tubes" },
    { "NAME": "bristle",     "LABEL": "Bristle Out",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Tubes" },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Motion" },
    { "NAME": "spin",        "LABEL": "Spin",            "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Motion" },
    { "NAME": "tilt",        "LABEL": "Tilt",            "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Motion" },
    { "NAME": "tubeColor",   "LABEL": "Tube Colour",     "TYPE": "color", "DEFAULT": [0.92, 0.92, 0.94, 1.0], "GROUP": "Look" },
    { "NAME": "accentAmt",   "LABEL": "Colour Streams",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Look" },
    { "NAME": "gloss",       "LABEL": "Gloss",           "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "fogAmt",      "LABEL": "Depth Fade",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Look" },
    { "NAME": "inputImage",  "LABEL": "Image Texture",   "TYPE": "image", "GROUP": "Image" },
    { "NAME": "imageAmt",    "LABEL": "Image Amount",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Image" },
    { "NAME": "imageMap",    "LABEL": "Image Mapping", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Wrap Around Shape", "Front Projection", "Fixed to Screen"], "GROUP": "Image" },
    { "NAME": "imageScale",  "LABEL": "Image Scale",     "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 4.0, "DEFAULT": 1.0, "GROUP": "Image" },
    { "NAME": "imageScroll", "LABEL": "Image Drift",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Image" },
    { "NAME": "imagePerTube","LABEL": "Per-Tube Mosaic", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Image" },
    { "NAME": "imageBg",     "LABEL": "Image Behind Shape", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Image" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",  "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",      "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",     "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.06, 0.06, 0.07, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG", "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// Domain-repeated lattice: one capsule per cell, oriented by a curl-ish flow
// tangent to the morphing shape. Steps are clamped to the cell exit so each
// cell's capsule is exact.

#define PI  3.14159265359
#define TAU 6.28318530718

float h12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float h13(vec3 p) { p = fract(p * 0.1031); p += dot(p, p.zyx + 31.32); return fract((p.x + p.y) * p.z); }
vec3  h33(vec3 p) { return vec3(h13(p), h13(p + 17.1), h13(p + 41.7)); }
float vn3(vec3 x) {
    vec3 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(h13(i), h13(i + vec3(1, 0, 0)), f.x), mix(h13(i + vec3(0, 1, 0)), h13(i + vec3(1, 1, 0)), f.x), f.y),
               mix(mix(h13(i + vec3(0, 0, 1)), h13(i + vec3(1, 0, 1)), f.x), mix(h13(i + vec3(0, 1, 1)), h13(i + vec3(1, 1, 1)), f.x), f.y), f.z);
}
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (palAmount <= 0.0) return c;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    vec3 o;
    if (m < 0.5) {
        // Colorful: saturation pushed hard toward neon / primary / poppy colour.
        vec3 hueC = pow((c - mn) / max(mx - mn, 1e-4), vec3(1.4));   // pure hue, nudged toward primaries
        float s2 = 1.0 - pow(1.0 - sat, 4.0);                         // strong but curved saturation boost
        float v = clamp(pow(mx, 0.85) * 1.12, 0.0, 1.0);
        o = mix(vec3(1.0), hueC, s2) * v;
        // flat mid-greys pick up a drifting neon spectrum keyed to brightness
        float hx = L * 1.7 + 0.04 * TIME;
        vec3 neon = pow(clamp(abs(fract(hx + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0), vec3(1.4));
        float wn = (1.0 - smoothstep(0.02, 0.1, sat)) * smoothstep(0.06, 0.25, L) * (1.0 - smoothstep(0.55, 0.85, L));
        o = mix(o, neon * v, wn);
        o = mix(o, vec3(1.0), smoothstep(0.85, 1.0, L) * 0.5);       // white-hot cores
    } else if (m > 2.5) {
        o = vec3(smoothstep(0.02, 0.95, pow(L, 0.9)));             // Black & White
    } else {
        vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
        float t = pow(L, 0.8);
        vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
        vec3 mono = vec3(smoothstep(0.0, 1.0, L));
        o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    }
    return mix(c, o, palAmount);
}
vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

float gT;           // flow time
vec3  gW;           // shape weights (cube, sphere, tetra)
float gScale;       // shape scale
float gCS;          // cell size
float gBeat;

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {                                   // manual override for the VJ
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }                                      // Rest
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; } // Build: 8 s riser, loops
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); } // Peak: hits every 4 s + beats
    }
    float k = dynamics;
    gBuild = b * k;                                                  // tension 0..1
    gDrop  = d * k;                                                  // impact impulse 0..1
    gPeak  = smoothstep(0.5, 0.9, e) * k;                            // sustained high section
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;       // quiet / breakdown
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);  // 0 rest … 1 peak; 0.5 when dynamics = 0
}

float sdBox(vec3 p, float b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }
float sdTetra(vec3 p, float s) {
    // regular tetrahedron with one vertex pointing up, base below
    p.y += s * 0.12;
    vec3 v = vec3(0.57735);
    vec3 u1 = vec3(0.70711, -0.70711, 0.0);
    vec3 u2 = cross(v, u1);
    vec3 r = p.x * u1 - p.y * v + p.z * u2;   // world up → the (-1,-1,-1) vertex
    float t = max(max(-r.x - r.y + r.z, r.x - r.y - r.z), max(-r.x + r.y - r.z, r.x + r.y + r.z));
    return (t - s) * 0.57735;
}
float shapeSDF(vec3 p) {
    float s = gScale;
    float d = 0.0;
    if (gW.x > 0.001) d += gW.x * sdBox(p, 0.78 * s);
    if (gW.y > 0.001) d += gW.y * (length(p) - 1.0 * s);
    if (gW.z > 0.001) d += gW.z * sdTetra(p, 0.78 * s);
    return d;
}
vec3 shapeN(vec3 p) {
    vec2 e = vec2(0.02, 0.0);
    return normalize(vec3(shapeSDF(p + e.xyy) - shapeSDF(p - e.xyy),
                          shapeSDF(p + e.yxy) - shapeSDF(p - e.yxy),
                          shapeSDF(p + e.yyx) - shapeSDF(p - e.yyx)));
}

float pot(vec3 x) {
    return vn3(x * 1.25 + vec3(0.0, gT * 0.35, gT * 0.2)) + 0.45 * vn3(x * 2.5 + vec3(gT * 0.3, 3.1, -gT * 0.25));
}

// the cell's tube: returns (active, half length) and fills a/b endpoints
bool cellTube(vec3 id, out vec3 A, out vec3 B, out float rad, out vec3 info) {
    vec3 c = (id + 0.5) * gCS;
    float sd = shapeSDF(c);
    float shell = layers * mix(0.6, 1.0, smoothstep(0.15, 0.8, gLift)) * gCS;
    A = c; B = c; rad = 0.0; info = vec3(0.0);
    if (sd > 0.0 || sd < -shell) return false;
    // erosion: drifting holes that reveal deeper layers (never erode the deepest layer fully)
    float er = vn3(c * 2.2 + vec3(0.0, gT * 0.15, gT * 0.1));
    float ero = clamp(erosion + 0.22 * gRest - 0.12 * gPeak, 0.0, 0.85);
    if (er < ero * smoothstep(-shell, -shell * 0.4, sd) * 1.2 && sd > -shell * 0.7) return false;
    vec3 n = shapeN(c);
    // swirling flow: noise vector crossed with the normal → tangential vortices
    // tubes follow the iso-lines of a drifting noise potential → eddies and vortices
    float e = 0.09;
    float p0 = pot(c);
    vec3 gr = vec3(pot(c + vec3(e, 0.0, 0.0)), pot(c + vec3(0.0, e, 0.0)), pot(c + vec3(0.0, 0.0, e))) - p0;
    vec3 tng = cross(n, gr / e);
    vec3 drift = cross(n, normalize(vec3(0.3, 1.0, 0.2)));          // calm combed base direction
    tng = drift * mix(0.6, 0.3, gLift) + tng * swirl * (1.6 * mix(0.55, 1.0, gLift) + 1.4 * gBuild);
    float bo = clamp(bristle * (1.0 - 0.8 * gBuild) + 0.6 * gBeat + 0.95 * gDrop, 0.0, 1.0);
    vec3 dir = normalize(mix(tng, n, bo) + 1e-4);
    vec3 j = (h33(id) - 0.5) * 0.08 * gCS;
    float rr = tubeWidth * gCS * (0.8 + 0.4 * h13(id + 3.0)) * (1.0 + 0.35 * bassP() * audioReact) * mix(0.7, 1.12, gLift);
    float hl = 0.5 * gCS * tubeLen * mix(0.7, 1.0, gLift) * (0.75 + 0.25 * h13(id + 9.0)) - rr;
    hl = max(hl, 0.0);
    // keep the capsule inside the cell
    vec3 lim = 0.5 * gCS - rr - abs(j);
    float maxH = min(lim.x / max(abs(dir.x), 1e-3), min(lim.y / max(abs(dir.y), 1e-3), lim.z / max(abs(dir.z), 1e-3)));
    hl = min(hl, max(maxH, 0.0));
    A = c + j - dir * hl; B = c + j + dir * hl; rad = rr;
    info = vec3(sd / -shell, h13(id + 21.0), dot(dir, n));
    return true;
}

float sdCap(vec3 p, vec3 a, vec3 b, float r, out float hh) {
    vec3 pa = p - a, ba = b - a;
    hh = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    return length(pa - ba * hh) - r;
}

// ── user image texture ────────────────────────────────────────────────────
// IMG_SIZE_inputImage is (0,0) until an image is loaded, so the shader looks unchanged without one.
float hasImage() { return step(1.5, IMG_SIZE_inputImage.x) * step(1.5, IMG_SIZE_inputImage.y); }
vec3 imgRaw(vec2 uv) {
    uv += vec2(imageScroll * 0.03 * TIME, 0.0);
    uv = abs(mod(uv + 1.0, 2.0) - 1.0);                       // mirror-repeat: no seams
    return texture2D(inputImage, uv).rgb;
}
vec3 imgSample(vec2 uv) {
    // keep the image's aspect on the shape; mirror-tile beyond its edges
    float ia = IMG_SIZE_inputImage.x / max(IMG_SIZE_inputImage.y, 1.0);
    return imgRaw((uv - 0.5) * vec2(1.0 / ia, 1.0) / imageScale + 0.5);
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    showDynamics();
    // additive clocks only (no TIME * changing-speed → no phase jumps)
    gT = TIME * flowSpeed + 0.6 * audioMidTime * ar * flowSpeed + 0.5 * audioBassTime * ar * flowSpeed + 1.5 * gBuild * gBuild * flowSpeed;
    gBeat = audioBeatPulse * ar;
    gCS = 2.4 / floor(density);
    gScale = shapeSize * (1.0 + 0.06 * bassP() * ar) * (mix(0.86, 1.04, gLift) - 0.1 * gBuild + 0.1 * gDrop);

    // shape morph: cube → sphere → triangle → …
    float ss = float(shapeSel);
    if (ss > 0.5) {
        gW = vec3(ss < 1.5 ? 1.0 : 0.0, (ss > 1.5 && ss < 2.5) ? 1.0 : 0.0, ss > 2.5 ? 1.0 : 0.0);
    } else {
        float ph = TIME / morphPeriod;
        float k = mod(floor(ph), 3.0);
        float f = smoothstep(0.55, 1.0, fract(ph));
        vec3 a = vec3(k < 0.5 ? 1.0 : 0.0, (k > 0.5 && k < 1.5) ? 1.0 : 0.0, k > 1.5 ? 1.0 : 0.0);
        vec3 b = a.zxy;
        gW = mix(a, b, f);
    }

    // camera & object rotation
    vec2 sp = (fc - 0.5 * R) / R.y;
    float trem = smoothstep(0.7, 1.0, gBuild) * 0.006;
    sp += trem * vec2(sin(TIME * 47.0), sin(TIME * 53.0 + 1.0));
    vec3 ro = vec3(0.0, 0.0, 4.6 + 0.35 * gRest - 0.75 * gBuild);     // camera pushes in on the build
    vec3 rd = normalize(vec3(sp, -1.55));
    float ay = TIME * spin * 0.6 + 0.25 * sin(TIME * 0.21) + 0.3 * audioMidTime * spin * ar + 2.2 * gBuild * gBuild * sign(spin + 1e-4);
    float ax = tilt * 0.7 + 0.12 * sin(TIME * 0.17);
    mat2 RY = rot(ay), RX = rot(ax);
    ro.yz = RX * ro.yz; rd.yz = RX * rd.yz;
    ro.xz = RY * ro.xz; rd.xz = RY * rd.xz;

    float hImg = hasImage();
    vec3 col = backdrop(uv);
    if (hImg * imageBg > 0.0) {
        // cover-fit: the image fills the frame without repeating
        float ia = IMG_SIZE_inputImage.x / max(IMG_SIZE_inputImage.y, 1.0);
        float sa = RENDERSIZE.x / RENDERSIZE.y;
        vec2 cv = vec2(sa / ia, 1.0) / max(sa / ia, 1.0);
        col = mix(col, imgRaw((uv - 0.5) * cv / imageScale + 0.5) * (0.55 + 0.25 * gLift), imageBg);
    }
    float alpha = 0.0;

    // bounding box
    float B = 1.65 * gScale + gCS;
    vec3 inv = 1.0 / rd;
    vec3 t0 = (-B - ro) * inv, t1 = (B - ro) * inv;
    vec3 tmn = min(t0, t1), tmx = max(t0, t1);
    float tn = max(max(tmn.x, tmn.y), tmn.z), tf = min(min(tmx.x, tmx.y), tmx.z);
    if (tf > max(tn, 0.0)) {
        float t = max(tn, 0.0) + 1e-3;
        bool hit = false;
        vec3 hA, hB, hInfo; float hR = 0.0;
        vec3 lastId = vec3(1e5);
        vec3 A, Bp, info; float rad = 0.0; bool act = false;
        for (int i = 0; i < 320; i++) {
            if (t > tf) break;
            vec3 p = ro + rd * t;
            vec3 id = floor(p / gCS);
            if (any(notEqual(id, lastId))) { act = cellTube(id, A, Bp, rad, info); lastId = id; }
            // distance to the cell exit along the ray
            vec3 cb = (id + step(0.0, rd)) * gCS;
            vec3 te = (cb - p) * inv;
            float tExit = min(te.x, min(te.y, te.z));
            float d = 1e5, hh;
            if (act) d = sdCap(p, A, Bp, rad, hh);
            if (d < 0.0004) { hit = true; hA = A; hB = Bp; hR = rad; hInfo = info; break; }
            t += min(d, tExit + gCS * 0.002);
        }
        if (hit) {
            vec3 p = ro + rd * t;
            float hh;
            vec3 ba = hB - hA;
            hh = clamp(dot(p - hA, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
            vec3 nrm = normalize(p - (hA + ba * hh));
            vec3 L1 = normalize(vec3(-0.5, 0.75, 0.6));
            L1.yz = RX * L1.yz; L1.xz = RY * L1.xz;          // light follows the camera
            vec3 L2 = normalize(vec3(0.7, -0.2, 0.4));
            L2.yz = RX * L2.yz; L2.xz = RY * L2.xz;
            float dif = clamp(dot(nrm, L1), 0.0, 1.0);
            float dif2 = clamp(dot(nrm, L2), 0.0, 1.0);
            vec3 hv = normalize(L1 - rd);
            float spec = pow(clamp(dot(nrm, hv), 0.0, 1.0), 48.0) * gloss;
            spec *= 1.0 + gBuild * 1.8 * step(0.5, fract(hInfo.y * 7.0 + 18.0 * gBuild * gBuild));   // shimmer, rate climbs
            float fres = pow(1.0 - clamp(dot(nrm, -rd), 0.0, 1.0), 4.0);
            float depth = hInfo.x;                                   // 0 surface … 1 deep
            float ao = mix(1.0, 0.25, smoothstep(0.0, 1.0, depth));
            ao *= 0.6 + 0.4 * smoothstep(0.0, 0.5, min(hh, 1.0 - hh) + 0.25);
            vec3 base = tubeColor.rgb;
            // colour streams: bands of hue running through the flow
            float band = smoothstep(0.55, 0.85, vn3((hA + hB) * 0.9 + vec3(gT * 0.25, 0.0, -gT * 0.2)));
            vec3 hue = 0.5 + 0.5 * cos(TAU * (hInfo.y * 0.15 + gT * 0.03 + vec3(0.0, 0.33, 0.67)));
            if (hImg * imageAmt > 0.0) {
                // where on the image this pixel reads from: per-tube centre (mosaic) or the exact hit point
                vec3 q = mix(p, (hA + hB) * 0.5, imagePerTube);
                float m = float(imageMap);
                vec2 iuv;
                if (m < 0.5) {
                    vec3 qn = normalize(q + 1e-5);
                    iuv = vec2(atan(qn.z, qn.x) / TAU + 0.5, asin(clamp(qn.y, -1.0, 1.0)) / PI + 0.5);
                    iuv.x *= 2.0;                                            // keep the wrap from stretching
                } else if (m < 1.5) {
                    iuv = q.xy / (2.2 * gScale) + 0.5;
                } else {
                    iuv = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0) + 0.5;
                }
                vec3 ic = imgSample(iuv);
                base = mix(base, ic * 1.15, imageAmt);
            }
            base = mix(base, hue * 1.1, band * clamp(accentAmt * mix(0.4, 1.6, gLift) + 0.3 * gBuild, 0.0, 1.0) * (1.0 - 0.7 * hImg * imageAmt));
            vec3 c = base * (0.16 + 1.05 * dif + 0.3 * dif2) * ao * (0.7 + 0.4 * gLift);
            c += vec3(1.0) * spec * (1.0 + 1.5 * highP() * ar) * ao;
            c += base * fres * 0.25;
            // tip glints on highs
            float tip = smoothstep(0.85, 1.0, abs(hh * 2.0 - 1.0));
            c += vec3(1.0) * tip * highP() * ar * 0.4 * step(0.7, hInfo.y);
            c += vec3(1.0, 0.97, 0.92) * gDrop * (0.35 + 0.9 * tip) * (1.0 - 0.6 * depth);   // drop flash on the bristling quills
            // depth fade toward the background
            float fz = smoothstep(3.0, 6.2, t);
            c = mix(c, backdrop(uv), fz * fogAmt);
            col = c;
            alpha = 1.0;
        }
    }

    col *= 1.0 + 0.12 * audioLevel * ar;
    col += vec3(0.9, 0.95, 1.0) * gDrop * gDrop * 0.12 * (1.0 - smoothstep(0.0, 0.9, length(sp)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
