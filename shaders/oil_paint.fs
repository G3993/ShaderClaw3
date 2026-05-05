/*{
  "DESCRIPTION": "Mountain Impasto — Fauvist-style painted mountainscape raymarched as a thick impasto surface. FBM height field with brush-stroke normals, warm/cool opposition in the Matisse tradition. Sunny daytime palette.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "3D", "Art Movement"],
  "INPUTS": [
    { "NAME": "flowSpeed",    "LABEL": "Flow Speed",     "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "terrainScale", "LABEL": "Terrain Scale",  "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5,  "MAX": 6.0  },
    { "NAME": "roughness",    "LABEL": "Roughness",      "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1,  "MAX": 1.0  },
    { "NAME": "sunAngle",     "LABEL": "Sun Angle",      "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 6.283 },
    { "NAME": "audioMod",     "LABEL": "Audio Mod",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0  }
  ]
}*/

// ---- constants -------------------------------------------------------
#define MAX_STEPS 64
#define MAX_DIST  20.0
#define SURF_DIST 0.002

// ---- Fauvist 4-stop palette ------------------------------------------
// black | warm red-orange | vivid cyan-teal | gold sunlit peak
const vec3 COL_SHADOW = vec3(0.00, 0.00, 0.00);   // ink black
const vec3 COL_LOW    = vec3(2.10, 0.40, 0.10);   // saturated warm red-orange
const vec3 COL_MID    = vec3(0.10, 1.80, 2.00);   // vivid cyan-teal (Fauvist cool)
const vec3 COL_PEAK   = vec3(2.50, 2.20, 0.50);   // gold-white HDR sunlit peak
const vec3 COL_SPEC   = vec3(3.00, 2.80, 1.50);   // specular HDR

// ---- hash / value noise ----------------------------------------------
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

// FBM with analytical derivative accumulation for impasto normals
// Returns vec3(height, dh/dx, dh/dz)
vec3 fbmD(vec2 p, vec2 anim) {
    float val  = 0.0;
    float dhdx = 0.0;
    float dhdz = 0.0;
    float amp  = 0.5;
    float freq = 1.0;
    float e    = 0.001;

    for (int i = 0; i < 6; i++) {
        vec2 sp = p * freq + anim;
        float h  = vnoise(sp);
        float hx = (vnoise(sp + vec2(e, 0.0)) - vnoise(sp - vec2(e, 0.0))) / (2.0 * e);
        float hz = (vnoise(sp + vec2(0.0, e)) - vnoise(sp - vec2(0.0, e))) / (2.0 * e);
        val  += amp * h;
        dhdx += amp * hx * freq;
        dhdz += amp * hz * freq;
        amp  *= roughness;
        freq *= 2.07;
    }
    return vec3(val, dhdx, dhdz);
}

// ---- terrain SDF -----------------------------------------------------
float terrainSDF(vec3 pos, vec2 anim) {
    vec2 xz   = pos.xz * terrainScale;
    vec3 fd   = fbmD(xz, anim);
    float h   = fd.x * 1.6 - 0.35;
    return pos.y - h;
}

// Normal from FBM derivatives — gives thick brush stroke relief
vec3 terrainNormal(vec3 pos, vec2 anim) {
    vec2 xz = pos.xz * terrainScale;
    vec3 fd = fbmD(xz, anim);
    return normalize(vec3(-fd.y * terrainScale, 1.0, -fd.z * terrainScale));
}

// Height 0..1 for palette
float terrainHeight01(vec3 pos, vec2 anim) {
    vec2 xz = pos.xz * terrainScale;
    vec3 fd = fbmD(xz, anim);
    return clamp(fd.x, 0.0, 1.0);
}

// ---- 4-stop elevation palette ----------------------------------------
vec3 terrainPalette(float h01) {
    if (h01 < 0.15) return mix(COL_SHADOW, COL_LOW,  h01 / 0.15);
    if (h01 < 0.50) return mix(COL_LOW,  COL_MID,  (h01 - 0.15) / 0.35);
    if (h01 < 0.80) return mix(COL_MID,  COL_PEAK, (h01 - 0.50) / 0.30);
    return              mix(COL_PEAK, COL_SPEC, (h01 - 0.80) / 0.20);
}

// ---- sky colour (Fauvist warm-gold horizon / cool-cyan zenith) ------
vec3 skyColor(vec3 dir) {
    float t = clamp(dir.y * 0.5 + 0.5, 0.0, 1.0);
    vec3 horizon = vec3(2.0, 1.2, 0.1);
    vec3 zenith  = vec3(0.05, 0.9, 2.0);
    return mix(horizon, zenith, t * t);
}

// ---- ray march -------------------------------------------------------
float march(vec3 ro, vec3 rd, vec2 anim, out float tHit) {
    float t = 0.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3  p = ro + rd * t;
        float d = terrainSDF(p, anim);
        if (d < SURF_DIST) { tHit = t; return 1.0; }
        if (t > MAX_DIST)  break;
        t += max(d * 0.5, SURF_DIST * 2.0);
    }
    tHit = MAX_DIST;
    return 0.0;
}

// ---- main ------------------------------------------------------------
void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    // Audio modulator: baseline 1.0, never gates
    float audioLevel = texture(audioFFT, vec2(0.05, 0.5)).r;
    float audio      = 1.0 + audioMod * audioLevel * 2.0;

    // Wind animation offset — audio modulates drift speed
    vec2 anim = vec2(TIME * flowSpeed * audio * 0.15,
                     TIME * flowSpeed * audio * 0.09);

    // Camera: slightly above, looking across the mountainscape; slow oscillation
    vec3 ro = vec3(sin(TIME * 0.13) * 0.4, 1.8 + sin(TIME * 0.07) * 0.2, -3.5);
    vec3 target  = vec3(sin(TIME * 0.09) * 0.3, 0.5, 1.0);
    vec3 forward = normalize(target - ro);
    vec3 right   = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3 upV     = cross(right, forward);
    vec3 rd      = normalize(uv.x * right + uv.y * upV + 1.6 * forward);

    // Two-light rig
    vec3 keyLight  = normalize(vec3(cos(sunAngle) * 1.2, 1.0, sin(sunAngle) * 0.8));
    vec3 fillLight = normalize(vec3(-1.0, 0.4, 0.3));
    vec3 keyCol    = vec3(2.2, 1.8, 0.9);
    vec3 fillCol   = vec3(0.2, 0.5, 1.4);

    float tHit;
    float hit = march(ro, rd, anim, tHit);

    vec3 col;

    if (hit > 0.5) {
        vec3  p  = ro + rd * tHit;
        vec3  n  = terrainNormal(p, anim);
        float h  = terrainHeight01(p, anim);

        vec3  baseCol = terrainPalette(h);

        // Diffuse — key (warm) + fill (cool)
        float diffKey  = max(dot(n, keyLight),  0.0);
        float diffFill = max(dot(n, fillLight), 0.0) * 0.35;

        // Specular on peaks (Phong, HDR)
        vec3  vDir    = normalize(-rd);
        vec3  halfKey = normalize(keyLight + vDir);
        float spec    = pow(max(dot(n, halfKey), 0.0), 48.0);
        float specMask = smoothstep(0.6, 1.0, h);
        vec3  specCol  = COL_SPEC * spec * specMask * 1.5;

        // Shadow: slope facing away from key dims toward black
        float shadow = smoothstep(-0.1, 0.3, diffKey);

        col = baseCol * (diffKey * keyCol + diffFill * fillCol + 0.08) * shadow
            + specCol;

        // Impasto contour: ink-black edge where surface faces away from camera
        float edgeFac = 1.0 - smoothstep(0.0, 0.25, dot(n, vDir));
        col = mix(col, COL_SHADOW, edgeFac * 0.85);

        // Atmospheric fog toward sky at distance
        float fogT = clamp((tHit - 4.0) / 12.0, 0.0, 1.0);
        col = mix(col, skyColor(rd) * 0.6, fogT * fogT);
    } else {
        col = skyColor(rd);
    }

    // Linear HDR output — no gamma, no ACES, no clamp
    gl_FragColor = vec4(col, 1.0);
}
