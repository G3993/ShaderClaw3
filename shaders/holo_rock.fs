/*{
  "DESCRIPTION": "Holo Rock — a real stone in a white studio: a raymarched granite monolith, knobbly and speckled, tumbling slowly in the air above its soft contact shadow. Holographic paint ribbons wrap its faces and shift hue with the angle of view as it turns — iridescent bands with a chrome sheen. Studio key, fill and rim lights, ambient occlusion in the crevices. Bass makes the rock breathe, mids tumble it, beats flash the strokes, highs flare the iridescence.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "rockSize",    "LABEL": "Rock Size",     "TYPE": "float", "MIN": 0.5, "MAX": 1.6,  "DEFAULT": 1.2,  "GROUP": "Shape / Geometry" },
    { "NAME": "strokeAmt",   "LABEL": "Holo Strokes",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.75, "GROUP": "Shape / Geometry" },
    { "NAME": "grit",        "LABEL": "Stone Grit",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Shape / Geometry" },
    { "NAME": "tumbleSpeed", "LABEL": "Tumble Speed",  "TYPE": "float", "MIN": -2.0, "MAX": 2.0, "DEFAULT": 0.3,  "GROUP": "Motion / Animation" },
    { "NAME": "crawlSpeed",  "LABEL": "Stroke Crawl",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.4,  "GROUP": "Motion / Animation" },
    { "NAME": "motionSpeed", "LABEL": "Motion Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "hueShift",    "LABEL": "Holo Shift",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "brightness",  "LABEL": "Brightness",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",    "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.05, "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",    "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Depth / Passes" },
    { "NAME": "aberration",  "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "hrScene" },
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// HOLO ROCK — 3D (Lu 2026-10-14: "should be 3D too, not the cheap 2D").
//   A raymarched stone: an ellipsoid displaced by two octaves of 3D noise
//   (the knobbly silhouette) plus fine grit on the normal, tumbling about
//   two axes. It floats over a white paper floor that takes a soft marched
//   shadow and a contact-darkening. Holographic strokes are ribbon fields
//   in OBJECT space (they stay glued to the stone as it turns): distance to
//   wavy curves on the surface, coloured by an iridescence keyed on the
//   view angle, so the hue sweeps as the rock rotates. Studio lighting:
//   key + fill + rim, specular, 5-tap AO, fresnel on the holo paint.
// ─────────────────────────────────────────────────────────────────────────
#define R RENDERSIZE.xy
#define TAU 6.2831853

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}
float hash11(float n) { return fract(sin(n) * 43758.5453123); }
float hash31(vec3 p) {
    p = fract(p * vec3(0.1031, 0.1030, 0.0973));
    p += dot(p, p.yxz + 33.33);
    return fract((p.x + p.y) * p.z);
}
float n3d(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n000 = hash31(i), n100 = hash31(i + vec3(1.0, 0.0, 0.0));
    float n010 = hash31(i + vec3(0.0, 1.0, 0.0)), n110 = hash31(i + vec3(1.0, 1.0, 0.0));
    float n001 = hash31(i + vec3(0.0, 0.0, 1.0)), n101 = hash31(i + vec3(1.0, 0.0, 1.0));
    float n011 = hash31(i + vec3(0.0, 1.0, 1.0)), n111 = hash31(i + vec3(1.0, 1.0, 1.0));
    return mix(mix(mix(n000, n100, f.x), mix(n010, n110, f.x), f.y),
               mix(mix(n001, n101, f.x), mix(n011, n111, f.x), f.y), f.z);
}
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x), f.y);
}
vec3 iri(float k) {
    return vec3(0.55 + 0.45 * cos(TAU * (k + 0.00)),
                0.55 + 0.45 * cos(TAU * (k + 0.30)),
                0.55 + 0.45 * cos(TAU * (k + 0.62)));
}
mat3 rotX(float a) { float c = cos(a), s = sin(a); return mat3(1.0, 0.0, 0.0, 0.0, c, -s, 0.0, s, c); }
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, s, 0.0, 1.0, 0.0, -s, 0.0, c); }

// shared per-frame state
float gBreath;          // bass breathing of the stone
mat3  gRot;             // world -> object (tumble)
float gLift;            // hover height

// The stone in OBJECT space: an ellipsoid with a knobbly noise skin.
float rockSDF(vec3 p) {
    vec3 e = vec3(0.30, 0.36, 0.27) * gBreath;
    float k = length(p / e);
    float d = (k - 1.0) * min(e.x, min(e.y, e.z));
    // knobs (two octaves, in object space so they turn with the stone)
    d -= (n3d(p * 4.2 + 1.7) - 0.5) * 0.085;
    d -= (n3d(p * 9.5 + 4.1) - 0.5) * 0.032;
    return d;
}
// world-space scene: the stone (hovering, tumbling) and the paper floor
vec2 map(vec3 p) {
    vec3 po = gRot * (p - vec3(0.0, gLift, 0.0));
    float dr = rockSDF(po);
    float df = p.y + 0.46;
    return (dr < df) ? vec2(dr, 1.0) : vec2(df, 2.0);
}
vec3 calcN(vec3 p) {
    vec2 e = vec2(0.0015, 0.0);
    return normalize(vec3(map(p + e.xyy).x - map(p - e.xyy).x,
                          map(p + e.yxy).x - map(p - e.yxy).x,
                          map(p + e.yyx).x - map(p - e.yyx).x));
}
float softShadow(vec3 ro, vec3 rd, float k) {
    float res = 1.0, t = 0.02;
    for (int i = 0; i < 24; i++) {
        float h = map(ro + rd * t).x;
        res = min(res, k * h / t);
        t += clamp(h, 0.01, 0.12);
        if (res < 0.005 || t > 2.5) break;
    }
    return clamp(res, 0.0, 1.0);
}
float calcAO(vec3 p, vec3 n) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = 0.012 + 0.06 * float(i);
        float d = map(p + n * h).x;
        occ += (h - d) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - 2.2 * occ, 0.0, 1.0);
}

vec3 renderScene() {
    vec2 fc = gl_FragCoord.xy;
    vec2 q = (fc - 0.5 * R) / R.y;
    float amt   = audioReact;
    float bassP = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    float midP  = pow(smoothstep(0.06, 0.85, audioMid), 1.2);
    float highP = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float beatP = clamp(audioBeatPulse, 0.0, 1.0);
    float levP  = clamp(audioLevel, 0.0, 1.0);
    float t = TIME * motionSpeed;

    gBreath = 1.0 + amt * 0.05 * bassP;
    float tum = t * tumbleSpeed * 0.25 + amt * 0.3 * midP;
    gRot  = rotX(tum * 0.55 + 0.4) * rotY(tum);
    gLift = 0.02 + 0.03 * sin(t * 0.5);

    // camera: a slow drift around the studio, framing by rockSize
    float ca = 0.18 * sin(t * 0.09);
    vec3 ro = vec3(sin(ca) * 2.3, 0.28 + 0.05 * sin(t * 0.13), cos(ca) * 2.3) / rockSize * 1.0;
    ro *= mix(1.6, 1.0, clamp((rockSize - 0.5) / 1.1, 0.0, 1.0));
    vec3 ta = vec3(0.0, -0.05, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 rt = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(rt, fw);
    vec3 rd = normalize(fw * 1.9 + rt * q.x + up * q.y);

    // march
    float tt = 0.0; vec2 hit = vec2(0.0); bool got = false;
    for (int i = 0; i < 96; i++) {
        vec3 p = ro + rd * tt;
        hit = map(p);
        if (hit.x < 0.0008 * tt + 0.0004) { got = true; break; }
        tt += hit.x * 0.9;
        if (tt > 8.0) break;
    }
    // studio backdrop (seen past the floor edge): soft white with a warm fall-off
    vec3 bg = vec3(0.965, 0.960, 0.950) * (1.0 - 0.10 * length(q));
    if (!got) return bg + (vnoise(fc * 0.06) - 0.5) * 0.012;

    vec3 p = ro + rd * tt;
    vec3 n = calcN(p);
    vec3 key  = normalize(vec3(0.55, 0.85, 0.45));
    vec3 fill = normalize(vec3(-0.7, 0.3, 0.4));
    vec3 rim  = normalize(vec3(-0.2, 0.4, -0.9));
    vec3 v = -rd;
    float ao = calcAO(p, n);
    float sh = softShadow(p + n * 0.004, key, 14.0);
    vec3 col;

    if (hit.y > 1.5) {
        // ── the paper floor: white, soft shadow, contact darkening ──────
        col = vec3(0.965, 0.960, 0.950);
        col += (vnoise(p.xz * 60.0) - 0.5) * 0.012;
        float dif = clamp(dot(n, key), 0.0, 1.0);
        col *= 0.72 + 0.28 * dif * mix(0.35, 1.0, sh);
        col *= 0.6 + 0.4 * ao;
        // distance fade into the backdrop
        col = mix(col, bg, smoothstep(1.2, 3.2, length(p.xz)));
    } else {
        // ── the stone ───────────────────────────────────────────────────
        vec3 po = gRot * (p - vec3(0.0, gLift, 0.0));      // object space
        // grit: fine salt & pepper in object space + a normal roughen
        vec3 ng = n;
        ng += (vec3(n3d(po * 60.0), n3d(po * 60.0 + 3.0), n3d(po * 60.0 + 7.0)) - 0.5) * 0.45 * grit;
        ng = normalize(ng);
        float g1 = n3d(po * 110.0 + 1.0), g2 = n3d(po * 170.0 + 9.0);   // fine granite speckle
        vec3 stone = mix(vec3(0.22, 0.215, 0.21), vec3(0.55, 0.54, 0.53), n3d(po * 5.0 + 2.0));
        stone = mix(stone, vec3(0.10, 0.095, 0.09), smoothstep(0.74, 0.80, g1) * grit * 0.9);  // pepper
        stone = mix(stone, vec3(0.72, 0.71, 0.70), smoothstep(0.84, 0.90, g2) * grit * 0.7);   // salt
        float dif  = clamp(dot(ng, key), 0.0, 1.0) * mix(0.25, 1.0, sh);
        float difF = clamp(dot(ng, fill), 0.0, 1.0);
        float rimL = pow(clamp(1.0 - dot(n, v), 0.0, 1.0), 3.0) * clamp(dot(n, rim), 0.0, 1.0);
        vec3 lit = stone * (0.22 + 0.95 * dif + 0.30 * difF + 0.35 * rimL);
        vec3 h = normalize(key + v);
        lit += vec3(1.0) * pow(clamp(dot(ng, h), 0.0, 1.0), 22.0) * 0.16 * sh;
        lit *= 0.45 + 0.55 * ao;
        col = lit;
        // ── holographic ribbons, glued to the stone in object space ──────
        // each ribbon: a wavy curve on a hashed great circle; distance from
        // the surface point's direction to the curve selects the paint
        vec3 dirO = normalize(po);
        float fres = pow(1.0 - clamp(dot(n, v), 0.0, 1.0), 2.0);
        for (int i = 0; i < 9; i++) {
            float fi = float(i);
            float h1 = hash11(fi * 5.13 + 1.0);
            float h2 = hash11(fi * 8.77 + 4.0);
            if (h2 > strokeAmt + 0.25) continue;
            // ribbon frame: a random axis; the ribbon runs around it like a belt
            vec3 ax = normalize(vec3(h1 - 0.5, hash11(fi * 2.3 + 7.0) - 0.5, h2 - 0.5) + 0.05);
            vec3 bx = normalize(cross(ax, vec3(0.3, 1.0, 0.2)));
            vec3 by = cross(ax, bx);
            float lat = dot(dirO, ax);                               // -1..1 across the belt
            float lon = atan(dot(dirO, by), dot(dirO, bx));          // around it
            float crawl = t * crawlSpeed * (0.25 + 0.35 * h1) + h1 * TAU;
            float wave = 0.10 * sin(lon * (2.0 + 3.0 * h2) + crawl);
            float centre = (h2 - 0.5) * 0.9;
            float across = lat - centre - wave;
            float width = 0.075 * (0.6 + 0.6 * h1) * (0.75 + 0.25 * sin(lon * 3.0 + crawl * 0.7));
            // belts are partial: a hashed arc, soft ends
            float arc = smoothstep(0.0, 0.4, 0.5 + 0.5 * sin(lon + h1 * TAU + crawl * 0.3) - (0.1 + 0.6 * h2));
            float sm = smoothstep(width, width * 0.55, abs(across)) * arc;
            if (sm > 0.001) {
                float keyH = hueShift + h1 + fres * 1.4 + dot(n, key) * 0.35 + lon * 0.08
                           + amt * 0.15 * highP;
                vec3 holo = iri(keyH);
                // chrome sheen: tight specular + fresnel lift
                holo += vec3(1.0) * pow(clamp(dot(ng, h), 0.0, 1.0), 60.0) * 0.8 * sh;
                holo *= 0.35 + 0.65 * (dif * 0.8 + 0.3) * (0.6 + 0.4 * ao);
                holo += holo * fres * 0.5;
                holo *= 1.0 + amt * 0.5 * beatP * step(0.6, h1);
                col = mix(col, holo, sm * 0.95);
            }
        }
        col *= 1.0 + amt * 0.12 * levP;
    }
    col += (hash21(fc + fract(t) * vec2(17.0, 29.0)) - 0.5) * 0.010;
    return col;
}

void main() {
    vec2 uv = gl_FragCoord.xy / R;
    if (PASSINDEX == 0) {
        gl_FragColor = vec4(clamp(renderScene(), 0.0, 1.0), 1.0);
    } else if (PASSINDEX == 1) {
        vec3 col = texture2D(hrScene, uv).rgb;
        vec3 prev = texture2D(abTrail, uv).rgb;
        float decay = 0.50 + 0.46 * trailAmt;
        col = max(col, prev * decay - 0.0045);
        col += (hash21(gl_FragCoord.xy + fract(TIME) * 61.0) - 0.5) * 0.006;
        gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
    } else {
        vec2 dir = uv - 0.5;
        float ab = aberration * 0.0045;
        vec3 base;
        base.r = texture2D(abTrail, uv + dir * ab).r;
        base.g = texture2D(abTrail, uv).g;
        base.b = texture2D(abTrail, uv - dir * ab).b;
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float an = float(i) * 0.7853982;
            vec2 o = vec2(cos(an), sin(an)) * (3.5 / R.y);
            bl += texture2D(abTrail, uv + o).rgb;
            bl += texture2D(abTrail, uv + o * 2.6).rgb * 0.6;
        }
        bl /= 12.8;
        bl = max(bl - 0.52, 0.0);
        vec3 col = base + bl * bl * bloomAmt * 1.8;
        float lvl = clamp(audioLevel, 0.0, 1.0);
        col *= brightness * mix(1.0, 0.64 + 0.36 * lvl, audioReact)
             * (1.0 + audioReact * 0.06 * clamp(audioBeatPulse, 0.0, 1.0));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.35);
        gl_FragColor = vec4(col, 1.0);
    }
}
