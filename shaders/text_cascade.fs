/*{
  "DESCRIPTION": "Lava Cascade — 3D raymarched volcanic waterfall. Molten lava pours down tiered rock steps. Warm HDR palette: gold, orange, cooled red, charcoal rock. Wide scenic shot with smoke haze.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"flowSpeed",  "LABEL":"Flow Speed",  "TYPE":"float","DEFAULT":0.5, "MIN":0.0,"MAX":2.0},
    {"NAME":"lavaThick",  "LABEL":"Lava Thick",  "TYPE":"float","DEFAULT":0.15,"MIN":0.05,"MAX":0.4},
    {"NAME":"hdrPeak",   "LABEL":"HDR Peak",    "TYPE":"float","DEFAULT":2.5, "MIN":1.0,"MAX":4.0},
    {"NAME":"audioReact","LABEL":"Audio React", "TYPE":"float","DEFAULT":0.6, "MIN":0.0,"MAX":2.0}
  ]
}*/

// ── Palette ──────────────────────────────────────────────────────────────────
const vec3 COL_LAVA_GOLD   = vec3(1.0, 0.8, 0.0);   // * hdrPeak  — hot core
const vec3 COL_LAVA_ORANGE = vec3(1.0, 0.4, 0.0);   // * 2.5
const vec3 COL_EMBER       = vec3(1.0, 0.2, 0.0);   // * 2.0
const vec3 COL_COOLED_RED  = vec3(0.5, 0.05, 0.0);
const vec3 COL_ROCK        = vec3(0.06, 0.04, 0.03);

// ── Hash / noise ─────────────────────────────────────────────────────────────
float hash12(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float noise2(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash12(i);
    float b = hash12(i + vec2(1.0, 0.0));
    float c = hash12(i + vec2(0.0, 1.0));
    float d = hash12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm2(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * noise2(p);
        p  = p * 2.1 + vec2(1.7, 9.2);
        a *= 0.5;
    }
    return v;
}

// ── SDF primitives ───────────────────────────────────────────────────────────
float sdBox(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}

// ── Ledge parameters (4 ledges) ──────────────────────────────────────────────
// Returns ledge Y height and Z protrusion offset
void ledgeParams(int i, out float ly, out float lz) {
    if (i == 0)      { ly = -1.0; lz =  0.6; }
    else if (i == 1) { ly = -0.3; lz =  0.1; }
    else if (i == 2) { ly =  0.4; lz = -0.3; }
    else             { ly =  1.1; lz = -0.7; }
}

// ── Combined scene SDF ────────────────────────────────────────────────────────
// Returns vec2(dist, matID)
// matID: 0 = rock ledge, 1 = lava slab, 2 = vertical lava stream
vec2 sceneSDF(vec3 p) {
    float dMin = 1e9;
    float mat  = 0.0;
    float t    = TIME;

    for (int i = 0; i < 4; i++) {
        float ly, lz;
        ledgeParams(i, ly, lz);

        // Rock ledge: wide flat box
        vec3 lp   = p - vec3(0.0, ly, lz);
        float lbx = sdBox(lp, vec3(3.5, 0.18, 0.55));
        if (lbx < dMin) { dMin = lbx; mat = 0.0; }

        // Lava slab at the front lip of each ledge
        // Animated drip ripple using FBM
        float drip = fbm2(vec2(p.x * 2.5 + 0.3 * float(i), t * flowSpeed * 0.8)) * 0.12;
        vec3 lavP  = p - vec3(0.0, ly + 0.17, lz - 0.35 - drip);
        float lav  = sdBox(lavP, vec3(3.2, lavaThick, 0.18 + drip));
        if (lav < dMin) { dMin = lav; mat = 1.0; }
    }

    // Vertical lava streams between ledges (3 streams)
    for (int j = 0; j < 3; j++) {
        float ly0, lz0, ly1, lz1;
        ledgeParams(j,     ly0, lz0);
        ledgeParams(j + 1, ly1, lz1);

        // Narrow box from ledge j to ledge j+1
        float midY = (ly0 + ly1) * 0.5;
        float midZ = (lz0 + lz1) * 0.5 - 0.3;
        float halfH = abs(ly1 - ly0) * 0.5 + 0.05;

        // Animated X wobble on the stream
        float wobble = sin(t * flowSpeed * 2.0 + float(j) * 2.1) * 0.08;
        vec3 sp = p - vec3(wobble, midY, midZ);
        float sv = sdBox(sp, vec3(0.22, halfH, 0.22));
        if (sv < dMin) { dMin = sv; mat = 2.0; }
    }

    return vec2(dMin, mat);
}

// ── Normal via finite differences ────────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy).x - sceneSDF(p - e.xyy).x,
        sceneSDF(p + e.yxy).x - sceneSDF(p - e.yxy).x,
        sceneSDF(p + e.yyx).x - sceneSDF(p - e.yyx).x
    ));
}

// ── Temperature-based lava color ──────────────────────────────────────────────
vec3 lavaColor(float temp) {
    // temp: 1.0 = molten core (gold), 0.0 = cooled (cooled red)
    vec3 c = mix(COL_COOLED_RED, COL_EMBER,       smoothstep(0.0, 0.3, temp));
         c = mix(c,              COL_LAVA_ORANGE,  smoothstep(0.2, 0.6, temp));
         c = mix(c,              COL_LAVA_GOLD,    smoothstep(0.5, 1.0, temp));
    return c;
}

// ── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera: wide shot, looking slightly down at the cascade
    float camAngle = sin(TIME * 0.04) * 0.12;
    vec3 ro = vec3(sin(camAngle) * 0.5, 0.15, -4.5);
    vec3 ta = vec3(0.0, 0.05, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(uv.x * right + uv.y * up + fwd * 1.6);

    float audioBst = 1.0 + audioLevel * audioReact;

    // ── Raymarch ─────────────────────────────────────────────────────────────
    float t     = 0.05;
    float matID = -1.0;
    vec3  p     = ro;
    bool  hit   = false;

    for (int i = 0; i < 64; i++) {
        p = ro + rd * t;
        vec2 res = sceneSDF(p);
        float d  = res.x;
        if (d < 0.003) { hit = true; matID = res.y; break; }
        if (t > 20.0)  break;
        t += max(d * 0.7, 0.005);
    }

    // Background: black void with faint warm ember glow from below
    vec3 col = vec3(0.0);
    // Warm ambient sky glow from lava below
    float skyHeat = max(0.0, -uv.y) * 0.04;
    col += COL_LAVA_ORANGE * skyHeat;

    if (hit) {
        vec3 n = calcNormal(p);

        // ── Rock ledge ───────────────────────────────────────────────────
        if (matID < 0.5) {
            // Surface temperature based on proximity to lava
            // Find nearest lava slab (simplified: use Y distance to nearest ledge)
            float minLavaDist = 1e9;
            for (int li = 0; li < 4; li++) {
                float ly, lz;
                ledgeParams(li, ly, lz);
                minLavaDist = min(minLavaDist, abs(p.y - (ly + 0.17)));
            }
            float temp = 1.0 - smoothstep(0.0, 0.6, minLavaDist);

            // Rock base: charcoal with temperature tint
            vec3 rockCol = mix(COL_ROCK, COL_COOLED_RED * 0.6, temp * 0.5);
            col = rockCol;

            // Key light: warm upper-right
            vec3 keyDir  = normalize(vec3(0.6, 1.0, -0.5));
            float diff   = max(dot(n, keyDir), 0.0);
            // Fill light: cool left
            vec3 fillDir = normalize(vec3(-1.0, 0.2, 0.0));
            float fill   = max(dot(n, fillDir), 0.0) * 0.15;

            col = col * (0.04 + diff * 0.35 + fill);
            // Lava heat glow on rock surface
            col += lavaColor(temp) * temp * 0.4 * audioBst;
        }
        // ── Lava slab ────────────────────────────────────────────────────
        else if (matID < 1.5) {
            // Animated surface noise for temperature variation
            float noiseT = fbm2(vec2(p.x * 3.0 + TIME * flowSpeed, p.z * 2.0 - TIME * flowSpeed * 0.5));
            float temp   = 0.5 + noiseT * 0.6;
            temp         = clamp(temp, 0.0, 1.0);

            vec3 lavCol  = lavaColor(temp) * hdrPeak * audioBst;
            // fwidth AA on bright/dark boundary
            float fw     = fwidth(noiseT);
            float edge   = smoothstep(0.0 - fw, 0.0 + fw, noiseT - 0.3);
            col = lavCol * (0.5 + 0.5 * edge);

            // Emissive: self-illuminated, no diffuse needed
            col *= 1.0 + audioBass * audioReact * 0.5;
        }
        // ── Vertical lava stream ─────────────────────────────────────────
        else {
            float noiseT = fbm2(vec2(p.x * 2.0, p.y * 3.0 - TIME * flowSpeed * 1.5));
            float temp   = 0.6 + noiseT * 0.5;
            temp         = clamp(temp, 0.0, 1.0);
            col = lavaColor(temp) * hdrPeak * 0.85 * audioBst;
        }

        // Depth fog
        float fogT = 1.0 - exp(-t * 0.08);
        col = mix(col, vec3(0.0), fogT * 0.5);
    }

    // ── Additive smoke / heat-haze accumulation ───────────────────────────
    vec3 smoke = vec3(0.0);
    for (int k = 0; k < 4; k++) {
        float kt  = 0.8 + float(k) * 1.1;
        if (kt > t) break;
        vec3 sp   = ro + rd * kt;
        for (int li = 0; li < 4; li++) {
            float ly, lz;
            ledgeParams(li, ly, lz);
            float smokeD = abs(sp.y - (ly + 0.35));
            smoke += vec3(0.022, 0.010, 0.001) * exp(-smokeD * 4.5) * audioBst;
        }
    }
    col += smoke;

    gl_FragColor = vec4(col, 1.0);
}
