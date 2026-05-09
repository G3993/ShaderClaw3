/*{
    "DESCRIPTION": "3D Wormhole Tunnel — camera flies through a neon spiral tunnel with SDF torus ribs",
    "CREDIT": "ShaderClaw auto-improve 2026-05-09",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "INPUTS": [
        {
            "NAME": "tunnelSpeed",
            "TYPE": "float",
            "DEFAULT": 0.08,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "tunnelRadius",
            "TYPE": "float",
            "DEFAULT": 0.55,
            "MIN": 0.2,
            "MAX": 1.2
        },
        {
            "NAME": "spiralTwist",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "ribCount",
            "TYPE": "float",
            "DEFAULT": 8.0,
            "MIN": 3.0,
            "MAX": 20.0
        },
        {
            "NAME": "hdrPeak",
            "TYPE": "float",
            "DEFAULT": 2.5,
            "MIN": 1.0,
            "MAX": 4.0
        },
        {
            "NAME": "audioReact",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0
        }
    ]
}*/

// ---- ISF uniforms ----
// TIME, RENDERSIZE provided by ISF runtime
// audioBass, audioMid, audioHigh, audioLevel provided by ISF runtime

// ---- Constants ----
#define MAX_STEPS 64
#define MAX_DIST  20.0
#define SURF_DIST 0.003
#define PI        3.14159265358979323846
#define TAU       6.28318530717958647692

// ---- Math helpers ----
float sdTorus(vec3 p, vec2 t) {
    vec2 q = vec2(length(p.xz) - t.x, p.y);
    return length(q) - t.y;
}

float sdCylinder(vec3 p, float r) {
    return length(p.xy) - r;
}

// Hash for noise
float hash11(float n) {
    return fract(sin(n) * 43758.5453123);
}

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

// ---- Tunnel SDF ----
// Returns (dist, ribID): negative ribID = tunnel wall
// Tunnel is infinite cylinder along Z, period-repeated
// Ribs: torus segments placed at regular fract(z) intervals, twisted by spiralTwist

vec2 mapTunnel(vec3 p, float t, float audioBase) {
    // Audio-modulated radius (K=1.0)
    float aK = 1.0;
    float radMod = tunnelRadius * (1.0 + audioBass * aK * audioReact * 0.3);

    // Hollow cylinder wall (negative = inside tunnel)
    float wallDist = -(sdCylinder(p.xy, radMod));  // negative inside
    // Actually we want: tunnel interior, so dist = tunnelRadius - length(p.xy)
    float wallSDF = radMod - length(p.xy);
    // But for raymarching we want positive when outside, so:
    float tunnelSDF = -(wallSDF);  // positive when outside cylinder

    // Ribs: periodic in z
    float ribSpacing = 1.0 / max(1.0, ribCount * 0.5);
    vec3 rp = p;
    // Spiral twist: rotate XY based on Z
    float twistAngle = p.z * spiralTwist * TAU;
    float ct = cos(twistAngle);
    float st2 = sin(twistAngle);
    rp.xy = mat2(ct, -st2, st2, ct) * p.xy;

    // Fold Z for repeating ribs
    float zPeriod = ribSpacing;
    float zFrac = mod(rp.z, zPeriod) - zPeriod * 0.5;
    vec3 ribP = vec3(rp.xy, zFrac);

    // Torus rib: major radius matches tunnel, minor = rib thickness
    float ribThick = 0.025;
    float ribMajor = radMod * 0.85;
    float ribSDF = sdTorus(ribP, vec2(ribMajor, ribThick));

    // Audio brightens ribs (K=1.0)
    float ribBright = hdrPeak * (1.0 + audioHigh * 1.0 * audioReact);

    // Return combined scene: min distance
    // ribID: 1.0 = rib, 0.0 = tunnel wall/void
    if (ribSDF < tunnelSDF) {
        return vec2(ribSDF, 1.0);
    } else {
        return vec2(tunnelSDF, 0.0);
    }
}

// ---- Normal estimation ----
vec3 calcNormal(vec3 p, float t) {
    float eps = 0.001;
    vec2 e = vec2(eps, 0.0);
    float aB = audioBass;
    return normalize(vec3(
        mapTunnel(p + e.xyy, t, aB).x - mapTunnel(p - e.xyy, t, aB).x,
        mapTunnel(p + e.yxy, t, aB).x - mapTunnel(p - e.yxy, t, aB).x,
        mapTunnel(p + e.yyx, t, aB).x - mapTunnel(p - e.yyx, t, aB).x
    ));
}

// ---- Palette ----
// void black, electric blue 3.0, violet 2.5, magenta 2.0, cyan accent 2.5
vec3 ribColor(float ribID, vec3 pos, float t) {
    // Cycle through hues based on Z position
    float zPhase = pos.z * 0.3 + t * 0.1;
    float hueIdx = mod(floor(zPhase), 5.0);

    vec3 col;
    if (hueIdx < 1.0) {
        col = vec3(0.0, 0.5, 1.0) * 3.0;  // electric blue
    } else if (hueIdx < 2.0) {
        col = vec3(0.5, 0.0, 1.0) * 2.5;  // violet
    } else if (hueIdx < 3.0) {
        col = vec3(1.0, 0.0, 0.7) * 2.0;  // magenta
    } else if (hueIdx < 4.0) {
        col = vec3(0.0, 1.0, 0.9) * 2.5;  // cyan accent
    } else {
        col = vec3(0.3, 0.0, 1.0) * 2.5;  // deep violet
    }
    return col;
}

// ---- Main ----
void main() {
    vec2 uv = (isf_FragNormCoord.xy * 2.0 - 1.0);
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float t = TIME;

    // Camera flies forward
    float camZ = t * tunnelSpeed;

    // Gentle camera sway (no step functions, smooth)
    float swayX = sin(t * 0.13) * 0.04;
    float swayY = cos(t * 0.17) * 0.03;

    vec3 ro = vec3(swayX, swayY, camZ);
    vec3 rd = normalize(vec3(uv, 1.6));

    // Slightly rotate ray for mild look-around
    float lookAngle = sin(t * 0.07) * 0.05;
    float lc = cos(lookAngle);
    float ls = sin(lookAngle);
    rd.xz = mat2(lc, -ls, ls, lc) * rd.xz;

    // Raymarching
    float dist = 0.0;
    float hitID = -1.0;
    vec3 hitPos = ro;
    bool hit = false;

    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * dist;
        vec2 res = mapTunnel(p, t, audioBass);
        float d = res.x;

        if (abs(d) < SURF_DIST) {
            hit = true;
            hitID = res.y;
            hitPos = p;
            break;
        }

        // Step forward — clamp step to avoid overstepping
        dist += max(abs(d) * 0.7, SURF_DIST * 2.0);

        if (dist > MAX_DIST) break;
    }

    vec3 finalColor = vec3(0.0);

    if (hit) {
        vec3 nor = calcNormal(hitPos, t);

        if (hitID > 0.5) {
            // Rib hit — neon color
            vec3 baseCol = ribColor(hitID, hitPos, t);

            // fwidth-based edge AA on rib: sharp core with soft halo
            // Use surface proximity to edge (how close to rib surface)
            float edgeFactor = 1.0;  // fully hit

            // Rim lighting — backlit silhouette
            vec3 lightDir = normalize(vec3(0.0, 0.0, -1.0));  // from behind camera
            float rim = pow(1.0 - abs(dot(nor, -rd)), 3.0);
            float diff = max(0.0, dot(nor, -lightDir));

            // Combine: rib emits HDR glow
            float glowStrength = hdrPeak * (1.0 + audioHigh * 1.0 * audioReact);
            vec3 emissive = baseCol * (0.5 + rim * 0.5);
            vec3 highlight = baseCol * rim * 1.5;

            finalColor = emissive + highlight;
            finalColor *= glowStrength / hdrPeak;  // normalize to hdrPeak scale

        } else {
            // Tunnel wall — deep void with faint edge glow
            // Wall only visible when camera clips edge
            float rim = pow(1.0 - abs(dot(nor, -rd)), 4.0);
            vec3 wallGlow = vec3(0.0, 0.1, 0.3) * rim * 0.5;
            finalColor = wallGlow;
        }
    } else {
        // No hit = void center of tunnel (looking straight ahead)
        // Deep void — slight vignette flicker from audio
        float vigPulse = audioBass * audioReact * 0.05;
        finalColor = vec3(0.0, 0.0, vigPulse);
    }

    // Additive glow pass — accumulate halo along ray near ribs
    // Simple approximation: soft glow volume near rib surfaces
    {
        float glowAcc = 0.0;
        vec3 glowCol = vec3(0.0);
        float gStep = MAX_DIST / 32.0;
        float gDist = 0.1;

        for (int gi = 0; gi < 32; gi++) {
            vec3 gp = ro + rd * gDist;
            vec2 gres = mapTunnel(gp, t, audioBass);

            if (gres.y > 0.5) {
                // Near a rib
                float proximity = exp(-abs(gres.x) * 30.0);
                vec3 haloCol = ribColor(gres.y, gp, t);
                float audioBoost = 1.0 + audioMid * 0.8 * audioReact;
                glowAcc += proximity * 0.04 * audioBoost;
                glowCol += haloCol * proximity * 0.04 * audioBoost;
            }

            gDist += gStep;
            if (gDist > MAX_DIST) break;
        }

        glowCol = clamp(glowCol, 0.0, 3.0);
        finalColor += glowCol;
    }

    // Output linear HDR — no tonemapping, no ACES, no clamp
    gl_FragColor = vec4(finalColor, 1.0);
}
