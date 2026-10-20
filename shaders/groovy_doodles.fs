/*{
  "DESCRIPTION": "Groovy Doodles — Miró's playground in three dimensions: glossy painted objects floating in a cream studio — the blue eye-stone (a ring with a black pupil), yellow eggs, a green leaf, red beans, black ink squiggles that are real tubes winding through space, ink dots and dashes — each hovering on its own slow orbit above a soft shadow on the floor. Studio light, reflections on the enamel, occlusion in the gaps. Bass bounces the blobs, mids wave the squiggles, beats pop a colour, highs jitter the confetti.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "density",     "LABEL": "Doodle Density","TYPE": "float", "MIN": 0.4, "MAX": 1.0,  "DEFAULT": 0.85, "GROUP": "Shape / Geometry" },
    { "NAME": "doodleSize",  "LABEL": "Doodle Size",   "TYPE": "float", "MIN": 0.6, "MAX": 1.6,  "DEFAULT": 1.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "shadowAmt",   "LABEL": "Hover Shadow",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Shape / Geometry" },
    { "NAME": "wiggleSpeed", "LABEL": "Wiggle Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "driftAmt",    "LABEL": "Float Drift",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Motion / Animation" },
    { "NAME": "motionSpeed", "LABEL": "Motion Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "hueShift",    "LABEL": "Palette Spin",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "brightness",  "LABEL": "Brightness",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",    "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.05, "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",    "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Depth / Passes" },
    { "NAME": "aberration",  "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// GROOVY DOODLES — 3D (Lu 2026-10-14: "should be a 3D effect, not cheap
// scribbles"). A raymarched studio: 12 hashed objects on a 4x3 lattice
// in front of a cream wall, floating above a cream floor (4x4). Kinds by hash:
//   blob     — an enamel egg / bean (ellipsoid), some an eye-stone: a torus
//              ring with a black pupil sphere in the hole
//   squiggle — a black ink tube: a chain of capsules along a sine wave,
//              undulating in time (mids), turned to a hashed angle
//   dot      — a small glossy ink sphere
//   dash     — a short rounded ink bar at a hashed angle
// Each object has a slow personal orbit (driftAmt) and a hover height;
// the floor takes a soft shadow (march) + contact AO so the hover reads.
// Materials: enamel paint (Miró primaries) with a sharp highlight and
// fresnel, black ink with a wetter highlight. Studio key + fill + rim.
// ─────────────────────────────────────────────────────────────────────────
#define R RENDERSIZE.xy
#define TAU 6.2831853

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}
float hash11(float n) { return fract(sin(n) * 43758.5453123); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x), f.y);
}
vec3 hueRotate(vec3 c, float a) {
    float an = a * TAU;
    vec3 k = vec3(0.57735);
    return c * cos(an) + cross(k, c) * sin(an) + k * dot(k, c) * (1.0 - cos(an));
}
float sdEllipsoid(vec3 p, vec3 r) {
    float k0 = length(p / r);
    float k1 = length(p / (r * r));
    return k0 * (k0 - 1.0) / max(k1, 1e-5);
}
float sdTorus(vec3 p, vec2 t) { vec2 q = vec2(length(p.xz) - t.x, p.y); return length(q) - t.y; }
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// shared per-frame state
float gT, gAmt, gBassP, gMidP;

// Object i: placement (home + orbit + hover) and its hashed character.
// Returns the distance and writes the material id (1 blob colour index+,
// 9 ink) into `mat`.
float objectSDF(vec3 p, int i, out float mat) {
    float fi = float(i);
    vec2 id = vec2(mod(fi, 4.0), floor(fi / 4.0));   // 4 across x 4 down
    float h  = hash21(id * 3.17 + 5.0);
    float h2 = hash21(id * 7.91 + 2.0);
    float h3 = hash21(id * 1.37 + 9.0);
    mat = 0.0;
    if (h3 > density) return 1e5;
    // lattice home across the frame (x) and down it (y), slight depth stagger
    vec3 home = vec3((id.x - 1.5) * 0.60, (1.5 - id.y) * 0.40 - 0.08, (h - 0.5) * 0.45);
    home.xy += (vec2(h, h2) - 0.5) * 0.22;
    vec3 orbit = driftAmt * 0.06 * vec3(sin(gT * (0.25 + 0.3 * h) + h * TAU),
                                        cos(gT * (0.2 + 0.25 * h2) + h2 * TAU),
                                        sin(gT * 0.17 + h3 * TAU) * 0.5);
    orbit.y += gAmt * 0.05 * gBassP * sin(h * TAU + gT * 2.0);   // bass bounce
    vec3 q = p - home - orbit;
    float sz = doodleSize * (0.75 + 0.5 * h2);
    float kind = h * 4.0;
    // Bounding sphere first: a ray far from this object gets the (safe)
    // sphere distance and skips the rotations and the capsule chain. This is
    // what keeps 16 objects affordable per step.
    float bound = length(q) - 0.45 * sz;
    if (bound > 0.08) return bound;
    // every object turns slowly on its own
    q.xz = rot2(0.3 * sin(gT * 0.3 + h * 6.0)) * q.xz;
    q.xy = rot2(h2 * TAU + 0.15 * sin(gT * 0.2 + h * 5.0)) * q.xy;
    float d;
    if (kind < 1.6) {
        float rad = 0.17 * sz;
        if (h2 > 0.6) {
            // the eye-stone: a ring with a black pupil in the hole
            d = sdTorus(q.xzy, vec2(rad * 0.78, rad * 0.34));
            float pup = length(q) - rad * 0.30;
            if (pup < d) { d = pup; mat = 9.0; return d; }
        } else {
            // egg / bean: an ellipsoid, bent a little
            vec3 e = vec3(rad * 1.25, rad * 0.9, rad * 0.8);
            vec3 qb = q; qb.y += 0.35 * qb.x * qb.x / rad;
            d = sdEllipsoid(qb, e);
        }
        mat = 1.0 + floor(h2 * 4.999);
    } else if (kind < 2.7) {
        // ink squiggle: capsules along a sine wave that undulates
        float L = 0.28 * sz;
        float ph = gT * wiggleSpeed * (0.5 + h) + h * TAU + gAmt * 0.8 * gMidP;
        float freq = 9.0 + 6.0 * h;
        float ampl = 0.075 * sz;
        float r = 0.034 * sz;
        d = 1e5;
        vec3 prev = vec3(-L, ampl * sin(-L * freq + ph), 0.05 * sz * sin(-L * 5.0 + ph * 0.5));
        for (int s = 1; s <= 9; s++) {
            float x = -L + 2.0 * L * float(s) / 9.0;
            vec3 cur = vec3(x, ampl * sin(x * freq + ph), 0.05 * sz * sin(x * 5.0 + ph * 0.5));
            float rr = r * (0.75 + 0.4 * sin(x * 9.0 + h * 7.0));   // brush pressure
            d = min(d, sdCapsule(q, prev, cur, rr));
            prev = cur;
        }
        mat = 9.0;
    } else if (kind < 3.2) {
        d = length(q) - 0.045 * sz;
        mat = 9.0;
    } else {
        d = sdCapsule(q, vec3(-0.09 * sz, 0.0, 0.0), vec3(0.09 * sz, 0.0, 0.0), 0.024 * sz);
        mat = (h2 > 0.75) ? 2.0 : 9.0;     // some dashes are red
    }
    return d;
}
vec2 map(vec3 p) {
    float best = p.y + 0.95;           // the floor
    float bm = 10.0;
    for (int i = 0; i < 16; i++) {
        float m; float d = objectSDF(p, i, m);
        if (d < best) { best = d; bm = m; }
    }
    return vec2(best, bm);
}
vec3 calcN(vec3 p) {
    const vec2 e = vec2(0.0012, -0.0012);
    return normalize(e.xyy * map(p + e.xyy).x + e.yyx * map(p + e.yyx).x +
                     e.yxy * map(p + e.yxy).x + e.xxx * map(p + e.xxx).x);
}
float softShadow(vec3 ro, vec3 rd, float k) {
    float res = 1.0, t = 0.02;
    for (int i = 0; i < 12; i++) {
        float h = map(ro + rd * t).x;
        res = min(res, k * h / t);
        t += clamp(h, 0.03, 0.22);
        if (res < 0.02 || t > 2.0) break;
    }
    return clamp(res, 0.0, 1.0);
}
float calcAO(vec3 p, vec3 n) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 3; i++) {
        float h = 0.025 + 0.09 * float(i);
        occ += (h - map(p + n * h).x) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - 2.2 * occ, 0.0, 1.0);
}
vec3 paletteOf(float m) {
    vec3 pal0 = vec3(0.16, 0.32, 0.78);   // cobalt
    vec3 pal1 = vec3(0.88, 0.12, 0.10);   // red
    vec3 pal2 = vec3(0.99, 0.78, 0.10);   // chrome yellow
    vec3 pal3 = vec3(0.05, 0.55, 0.25);   // green
    vec3 pal4 = vec3(0.95, 0.45, 0.10);   // orange accent
    vec3 c = m < 1.5 ? pal0 : (m < 2.5 ? pal1 : (m < 3.5 ? pal2 : (m < 4.5 ? pal3 : pal4)));
    return hueRotate(c, hueShift);
}

vec3 renderScene() {
    vec2 fc = gl_FragCoord.xy;
    vec2 q = (fc - 0.5 * R) / R.y;
    gAmt   = audioReact;
    gBassP = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    gMidP  = pow(smoothstep(0.06, 0.85, audioMid), 1.2);
    float highP = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float beatP = clamp(audioBeatPulse, 0.0, 1.0);
    gT = TIME * motionSpeed;

    // camera: straight-ish on, a slow sway, so the lattice reads like the print
    float ca = 0.10 * sin(gT * 0.11);
    vec3 ro = vec3(sin(ca) * 2.8, 0.12 + 0.08 * sin(gT * 0.07), cos(ca) * 2.8);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 rt = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(rt, fw);
    vec3 rd = normalize(fw * 1.7 + rt * q.x + up * q.y);

    float tt = 0.0; vec2 hit = vec2(0.0); bool got = false;
    for (int i = 0; i < 56; i++) {
        vec3 p = ro + rd * tt;
        hit = map(p);
        if (hit.x < 0.0012 * tt + 0.0006) { got = true; break; }
        tt += hit.x * 0.95;
        if (tt > 7.0) break;
    }
    // cream wall with paper tooth
    vec3 cream = vec3(0.945, 0.92, 0.87);
    vec3 bg = cream * (1.0 - 0.12 * length(q)) + (vnoise(fc * 0.08) - 0.5) * 0.03;
    if (!got) return bg;

    vec3 p = ro + rd * tt;
    vec3 n = calcN(p);
    vec3 v = -rd;
    vec3 key  = normalize(vec3(0.5, 0.9, 0.55));
    vec3 fill = normalize(vec3(-0.7, 0.25, 0.5));
    vec3 rim  = normalize(vec3(-0.3, 0.5, -0.8));
    float sh = softShadow(p + n * 0.01, key, 10.0);
    float ao = calcAO(p, n);
    vec3 h = normalize(key + v);
    float fres = pow(1.0 - clamp(dot(n, v), 0.0, 1.0), 3.0);
    vec3 col;
    if (hit.y > 9.5) {
        // floor: cream, soft shadow scaled by the hover-shadow knob
        col = cream;
        col += (vnoise(p.xz * 40.0) - 0.5) * 0.02;
        float dif = clamp(dot(n, key), 0.0, 1.0);
        float shd = mix(1.0, sh, shadowAmt);
        col *= 0.74 + 0.26 * dif * shd;
        col *= mix(1.0, 0.55 + 0.45 * ao, shadowAmt);
        col = mix(col, bg, smoothstep(1.0, 3.5, length(p.xz)));
    } else if (hit.y > 8.5) {
        // black ink: wet enamel — dark, with a hard highlight and fresnel sheen
        vec3 ink = vec3(0.045, 0.04, 0.04);
        float dif = clamp(dot(n, key), 0.0, 1.0) * mix(0.3, 1.0, sh);
        col = ink * (0.5 + 0.8 * dif + 0.3 * clamp(dot(n, fill), 0.0, 1.0));
        col += vec3(1.0) * pow(clamp(dot(n, h), 0.0, 1.0), 90.0) * 0.9 * sh;
        col += vec3(0.6, 0.62, 0.66) * fres * 0.35;
        col += vec3(0.9) * pow(clamp(dot(n, rim), 0.0, 1.0), 4.0) * 0.12;
        col *= 0.5 + 0.5 * ao;
    } else {
        // enamel paint: Miró primary, soft diffuse wrap, crisp highlight
        vec3 pc = paletteOf(hit.y);
        pc *= 1.0 + gAmt * (0.18 * beatP + 0.14 * clamp(audioLevel, 0.0, 1.0));
        float dif = clamp(dot(n, key) * 0.6 + 0.4, 0.0, 1.0) * mix(0.35, 1.0, sh);
        col = pc * (0.30 + 0.85 * dif + 0.25 * clamp(dot(n, fill), 0.0, 1.0));
        col += vec3(1.0) * pow(clamp(dot(n, h), 0.0, 1.0), 48.0) * 0.55 * sh;
        col += mix(pc, vec3(1.0), 0.6) * fres * 0.30;
        col += vec3(1.0) * pow(clamp(dot(n, rim), 0.0, 1.0), 4.0) * 0.10;
        col *= 0.55 + 0.45 * ao;
    }
    // highs jitter the confetti: a faint sparkle on small ink objects
    col += vec3(1.0) * gAmt * 0.06 * highP * step(8.5, hit.y) * hash21(fc + floor(gT * 8.0));
    col += (hash21(fc + fract(gT) * vec2(17.0, 29.0)) - 0.5) * 0.012;
    return col;
}

void main() {
    vec2 uv = gl_FragCoord.xy / R;
    if (PASSINDEX == 0) {
        vec3 col = renderScene();
        vec3 prev = texture2D(abTrail, uv).rgb;
        float decay = 0.50 + 0.46 * trailAmt;
        col = max(col, prev * decay - 0.0045);
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
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.45);
        float lumHD = dot(col, vec3(0.299, 0.587, 0.114));
        col = clamp(mix(vec3(lumHD), col, 1.15), 0.0, 1.0);
        gl_FragColor = vec4(col, 1.0);
    }
}
