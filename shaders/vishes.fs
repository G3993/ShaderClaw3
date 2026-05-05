/*{
    "DESCRIPTION": "Coral Reef — 3D raymarched bioluminescent coral forest with audio-reactive polyps",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "audioReact",    "TYPE": "float", "MIN": 0.5, "MAX": 3.0,  "DEFAULT": 1.5  },
        { "NAME": "cameraSpeed",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.3  },
        { "NAME": "coralDensity",  "TYPE": "float", "MIN": 1.0, "MAX": 8.0,  "DEFAULT": 5.0  },
        { "NAME": "glowIntensity", "TYPE": "float", "MIN": 0.5, "MAX": 3.0,  "DEFAULT": 1.5  }
    ]
}*/

precision highp float;

// ---------- palette ----------
const vec3 NAVY         = vec3(0.0,  0.02, 0.08);
const vec3 CORAL_WARM   = vec3(1.0,  0.2,  0.35);
const vec3 TEAL_GLOW    = vec3(0.0,  1.8,  1.2);
const vec3 MAGENTA_GLOW = vec3(1.5,  0.0,  1.8);
const vec3 IVORY_TIP    = vec3(2.2,  2.0,  1.6);

// ---------- hash / noise ----------
float hash(float n) { return fract(sin(n) * 43758.5453123); }
float hash2f(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123); }

// ---------- SDF primitives ----------
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a;
    float t = clamp(dot(p - a, ab) / dot(ab, ab), 0.0, 1.0);
    return length(p - a - t * ab) - r;
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdPlane(vec3 p) { return p.y + 0.2; }

// Smooth min for organic unions
float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// ---------- Scene SDF ----------
// Returns (dist, materialID)
//   materialID: 0=ground, 1=stalk, 2=polyp_tip
vec2 sceneSDF(vec3 p, float audio) {
    float d = sdPlane(p);
    float mat = 0.0;

    int nCoral = int(coralDensity);

    for (int i = 0; i < 8; i++) {
        if (i >= nCoral) break;
        float fi = float(i);

        // Grid position with hash displacement
        float gx = hash(fi * 3.1 + 0.5) * 2.0 - 1.0;
        float gz = hash(fi * 5.7 + 1.2) * 2.0 - 1.0;
        gx *= 2.5;
        gz *= 2.5;

        float height  = 0.5 + hash(fi * 7.3) * 1.0;
        float radius  = 0.04 + hash(fi * 2.9) * 0.04;
        // Slight sway with time
        float sway = sin(TIME * 0.7 + fi * 1.57) * 0.04 * audio;

        vec3 base = vec3(gx,        -0.2, gz);
        vec3 tip  = vec3(gx + sway,  -0.2 + height, gz);

        float stalkDist = sdCapsule(p, base, tip, radius);
        float polypDist = sdSphere(p - tip, 0.08 + 0.04 * audio);

        // Smooth blend stalk into ground
        float coralUnit = smin(stalkDist, polypDist, 0.06);

        if (coralUnit < d) {
            // Determine if we're closer to tip (polyp) vs stalk body
            mat = (polypDist < stalkDist + 0.05) ? (2.0 + fi * 0.01) : 1.0;
        }
        d = smin(d, coralUnit, 0.06);
    }

    return vec2(d, mat);
}

// ---------- Normal via central differences ----------
vec3 calcNormal(vec3 p, float audio) {
    float e = 0.002;
    return normalize(vec3(
        sceneSDF(p + vec3( e, 0, 0), audio).x - sceneSDF(p - vec3( e, 0, 0), audio).x,
        sceneSDF(p + vec3( 0, e, 0), audio).x - sceneSDF(p - vec3( 0, e, 0), audio).x,
        sceneSDF(p + vec3( 0, 0, e), audio).x - sceneSDF(p - vec3( 0, 0, e), audio).x
    ));
}

void main() {
    vec2 fragUV = isf_FragNormCoord;

    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    // ---- Camera ----
    float camAngle = TIME * cameraSpeed * 0.4;
    vec3 ro = vec3(cos(camAngle) * 4.0, 1.5 + sin(TIME * 0.2) * 0.5, sin(camAngle) * 4.0);
    vec3 target  = vec3(0.0, 0.5, 0.0);
    vec3 forward = normalize(target - ro);
    vec3 right   = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3 up      = cross(right, forward);

    vec2 screenUV = fragUV * 2.0 - 1.0;
    screenUV.x *= RENDERSIZE.x / RENDERSIZE.y;
    vec3 rd = normalize(forward + screenUV.x * right + screenUV.y * up);

    // ---- Raymarch ----
    float t   = 0.0;
    float mat = -1.0;
    bool  hit = false;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        vec2 res = sceneSDF(p, audio);
        float d = res.x;
        if (d < 0.005) {
            mat = res.y;
            hit = true;
            break;
        }
        t += max(d, 0.01);
        if (t > 20.0) break;
    }

    // ---- Shading ----
    vec3 col;

    if (hit) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p, audio);

        // Point light from above
        vec3 lightPos = vec3(0.0, 5.0, 0.0);
        vec3 ldir = normalize(lightPos - p);
        float diff = max(dot(n, ldir), 0.0);

        // Specular (Blinn-Phong)
        vec3 halfV = normalize(ldir - rd);
        float spec = pow(max(dot(n, halfV), 0.0), 32.0);

        if (mat < 0.5) {
            // Ground plane
            vec3 groundCol = NAVY + TEAL_GLOW * 0.03;
            col = groundCol * (0.2 + 0.8 * diff) + spec * 0.1;
        } else if (mat < 1.5) {
            // Coral stalk
            col = CORAL_WARM * (1.0 + audio * 0.5) * (0.3 + 0.7 * diff) + spec * 0.3;
        } else {
            // Polyp tips — cycle by coral index
            float ci = fract(mat - 2.0) * 100.0; // rough index
            float tipHash = hash(floor(mat * 100.0) * 0.01);
            vec3 tipCol;
            if (tipHash < 0.33) {
                tipCol = TEAL_GLOW;
            } else if (tipHash < 0.66) {
                tipCol = MAGENTA_GLOW;
            } else {
                tipCol = IVORY_TIP;
            }
            // Audio drives polyp brightness
            col = tipCol * glowIntensity * (0.6 + 0.8 * audio) * (0.4 + 0.6 * diff);
            col += spec * tipCol * 0.5;
        }

        // Depth fog toward camera distance
        float fogT = 1.0 - exp(-t * 0.08);
        col = mix(col, NAVY, fogT);

    } else {
        // ---- Background — deep navy with bioluminescent particle stars ----
        col = NAVY;

        // Deep-sea floating particles
        for (int i = 0; i < 12; i++) {
            float fi = float(i);
            vec3 starPos = vec3(
                hash(fi * 3.1) * 2.0 - 1.0,
                hash(fi * 5.7) * 2.0 - 1.0,
                hash(fi * 7.3) * 2.0 - 1.0
            );
            // Particle drifts slowly
            starPos.y += fract(TIME * 0.1 + fi * 0.13) * 2.0 - 1.0;
            vec3 toStar = normalize(starPos);
            float ang = dot(rd, toStar);
            float glow = exp((ang - 1.0) * 800.0);
            float ph = hash(fi * 11.3);
            vec3 particleCol = (ph < 0.33) ? TEAL_GLOW : (ph < 0.66 ? MAGENTA_GLOW : IVORY_TIP);
            col += particleCol * glow * (0.5 + 0.5 * audio);
        }
    }

    // ---- Output LINEAR HDR — no clamp, no tonemapping ----
    gl_FragColor = vec4(col, 1.0);
}
