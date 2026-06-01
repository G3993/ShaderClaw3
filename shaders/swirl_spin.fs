/*{
  "DESCRIPTION": "Swirl Liquid Metal — Lissajous swirl painted into a 3D liquid metal surface with CFD-style curl flow, bismuth reflections, and HDR specular highlights",
  "CREDIT": "Merged from TekF / flockaroo / ShaderClaw",
  "CATEGORIES": ["Generator", "Feedback"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Swirl Speed",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "spotSize",   "LABEL": "Spot Size",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.2,  "MAX": 4.0  },
    { "NAME": "shrink",     "LABEL": "Shrink",        "TYPE": "float", "DEFAULT": 0.985,"MIN": 0.90, "MAX": 1.00 },
    { "NAME": "drift",      "LABEL": "Drift",         "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0,  "MAX": 0.05 },
    { "NAME": "twist",      "LABEL": "Twist",         "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0,  "MAX": 0.20 },
    { "NAME": "gamma",      "LABEL": "Gamma",         "TYPE": "float", "DEFAULT": 2.2,  "MIN": 1.0,  "MAX": 3.0  },
    { "NAME": "bumpHeight", "LABEL": "Bump Height",   "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.001,"MAX": 0.12 },
    { "NAME": "envBright",  "LABEL": "Reflection",    "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "envShift",   "LABEL": "Env Hue",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "metalColor", "LABEL": "Metal Tint",    "TYPE": "color", "DEFAULT": [0.85,0.88,0.95,1.0] },
    { "NAME": "fluidSpeed", "LABEL": "Fluid Speed",   "TYPE": "float", "DEFAULT": 4.0,  "MIN": 0.5,  "MAX": 20.0 },
    { "NAME": "viscosity",  "LABEL": "Viscosity",     "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.0,  "MAX": 0.15 },
    { "NAME": "curlScale",  "LABEL": "Curl Scale",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "specPower",  "LABEL": "Specular Power","TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0  }
  ]
}*/

#define PI  3.14159265
#define PI2 6.28318530
#define ROT_N 5

// ---- Math helpers ----
vec2 rot2(vec2 v, float a) {
    float c = cos(a), s = sin(a);
    return vec2(c*v.x - s*v.y, s*v.x + c*v.y);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// ---- Curl field from stored buffer ----
// We store the accumulated swirl color in a simulated "buffer" by warping
// history via texture sampling of the previous frame's gl_FragColor.
// Since we have no multipass, we synthesize the curl analytically from
// TIME + position, then blend it with the swirl color field.

// Procedural curl velocity — replaces the CFD buffer lookup
vec2 curlVelocity(vec2 uv, float t) {
    vec2 v = vec2(0.0);
    float ang = PI2 / float(ROT_N);
    vec2 b = vec2(0.04, 0.0);
    for (int l = 0; l < 6; l++) {
        vec2 p = b;
        for (int i = 0; i < ROT_N; i++) {
            float phase = t * 0.15 + dot(uv, p) * 8.0;
            float curl = sin(phase) * cos(phase * 1.3 + float(l) * 0.7);
            v += p.yx * vec2(1.0, -1.0) * curl / max(dot(b,b), 0.0001);
            p = rot2(p, ang);
        }
        b *= 2.0;
        if (dot(b,b) > 0.25) break;
    }
    return v / float(ROT_N) * curlScale;
}

// ---- Swirl spot color ----
vec3 swirlSpot(vec2 fragCoord, float t) {
    vec2 res = RENDERSIZE;
    vec2 nfrag = fragCoord / res;
    float audioScale = 1.0 + audioLevel * 1.5;
    vec2 spotDrift  = vec2(cos(t * 0.5), sin(t * 0.7)) * 50.0 * audioScale;
    vec2 spotCenter = sin(vec2(11.0, 13.0) * t) * 60.0 * audioScale + spotDrift + res * 0.5;
    float idx = smoothstep(9.0 * spotSize, 30.0 * spotSize, length(fragCoord - spotCenter));
    vec3 hotCol = 0.5 + 0.5 * sin(t * vec3(13.0, 11.0, 17.0) + nfrag.xyx * 3.0);
    vec3 bgHue  = 0.5 + 0.5 * sin(t * 0.3 + nfrag.x * 4.0 + nfrag.y * 2.5
                                   + vec3(0.0, 2.094, 4.189));
    return mix(hotCol, bgHue, idx);
}

// ---- Surface normal from procedural height field ----
vec3 liquidNormal(vec2 uv, float t, vec2 vel) {
    float delta = 1.5 / RENDERSIZE.x;

    // Height = amplitude of the curl velocity + time-animated ripples
    // We layer multiple sine waves warped by the velocity field
    vec2 uvL = uv + vec2(-delta, 0.0);
    vec2 uvR = uv + vec2( delta, 0.0);
    vec2 uvU = uv + vec2(0.0,  delta);
    vec2 uvD = uv + vec2(0.0, -delta);

    // Warp sample coords by velocity for 3D liquid look
    float warp = 0.06;
    vec2 velL = curlVelocity(uvL, t);
    vec2 velR = curlVelocity(uvR, t);
    vec2 velU = curlVelocity(uvU, t);
    vec2 velD = curlVelocity(uvD, t);

    float hC = length(vel);
    float hL = length(velL);
    float hR = length(velR);
    float hU = length(velU);
    float hD = length(velD);

    // Additional wave ripple layer
    float ripple = sin(uv.x * 18.0 + t * 1.2 + vel.x * 12.0)
                 * sin(uv.y * 14.0 + t * 0.9 + vel.y * 10.0) * 0.5;
    hC += ripple * 0.4;

    vec3 n = normalize(vec3(
        -(hR - hL) * bumpHeight * RENDERSIZE.x,
        -(hU - hD) * bumpHeight * RENDERSIZE.y,
        1.0
    ));
    return n;
}

// ---- Procedural environment map ----
vec3 envMap(vec3 R, float t) {
    float envY    = R.y * 0.5 + 0.5;
    float envAngle = atan(R.z, R.x) / PI2 + 0.5;

    vec3 skyHigh  = hsv2rgb(vec3(0.60 + envShift, 0.30, 1.20));
    vec3 skyLow   = hsv2rgb(vec3(0.55 + envShift, 0.50, 0.80));
    vec3 ground   = hsv2rgb(vec3(0.08 + envShift, 0.60, 0.15));
    vec3 horizon  = hsv2rgb(vec3(0.10 + envShift, 0.30, 0.90));

    // Animated color bands (liquid iridescence)
    vec3 iridA = hsv2rgb(vec3(fract(envAngle * 3.0 + envShift + t * 0.05), 0.8, 1.1));
    vec3 iridB = hsv2rgb(vec3(fract(envAngle * 5.0 + envShift + t * 0.07 + 0.3), 0.7, 0.9));

    float aaW       = max(fwidth(envY), 0.001);
    float skyMask   = smoothstep(0.52 - aaW, 0.52 + aaW, envY);
    float groundMask= 1.0 - smoothstep(0.48 - aaW, 0.48 + aaW, envY);
    float horizMask = 1.0 - skyMask - groundMask;

    float tSky = smoothstep(0.52, 1.0, envY);
    vec3 skyCol = mix(horizon, skyHigh, tSky);
    float cloud = sin(envAngle * 12.0 + R.y * 8.0) * 0.5 + 0.5;
    skyCol = mix(skyCol, skyLow, cloud * 0.2);
    // Iridescence in sky
    skyCol = mix(skyCol, iridA, 0.25);

    vec3 horizCol  = horizon * 2.4 + iridB * 0.4;
    float tGround  = smoothstep(0.48, 0.0, envY);
    vec3 groundCol = mix(horizon * 0.5, ground, tGround);

    return skyCol * skyMask + horizCol * horizMask + groundCol * groundMask;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 res       = RENDERSIZE;
    vec2 uv        = isf_FragNormCoord;

    float t = TIME * speed * (1.0 + audioLevel * 1.5);

    // ---- 1. Compute procedural curl velocity ----
    vec2 vel = curlVelocity(uv, TIME * fluidSpeed * 0.05
                               * (1.0 + audioBass * 0.8));

    // ---- 2. Self-advection feedback — warp UV to simulate persistence ----
    float speedScale = fluidSpeed * sqrt(res.x / 600.0) * 0.0004;
    vec2 advUV = uv - vel * speedScale;

    // ---- 3. Swirl spot color at advected position ----
    // Use advected + twist feedback to mimic the original swirl buffer
    vec2 c2      = fragCoord - res * 0.5;
    vec2 sampleP = (fragCoord * shrink
                   + res * drift
                   + c2.yx * vec2(-twist, twist)) / res;

    // Blend current spot with a slightly time-lagged version for fake persistence
    vec3 swirlCol    = swirlSpot(fragCoord, t);
    vec3 swirlPrev   = swirlSpot(sampleP * res, t - TIMEDELTA * speed);
    vec3 swirlBlended= mix(swirlCol, swirlPrev, 0.72);

    // ---- 4. Compute 3D surface normal ----
    vec3 N = liquidNormal(uv, TIME * fluidSpeed * 0.05, vel);

    // ---- 5. Screen-space ray & reflection ----
    vec2 sc      = (fragCoord - res * 0.5) / res.x;
    vec3 viewDir = normalize(vec3(sc, -1.0));
    vec3 R       = reflect(viewDir, N);

    // ---- 6. Environment lookup ----
    vec3 envCol = envMap(R, TIME);
    vec3 refl   = envCol * envBright;

    // ---- 7. Fluid/swirl color contribution (oil-slick / bismuth) ----
    vec3 fluidCol = swirlBlended;
    // Modulate by normal for 3D shading
    float ndotv = max(dot(N, -viewDir), 0.0);
    float fresnel = pow(1.0 - ndotv, 3.0);
    // Mix diffuse swirl with mirror reflection via fresnel
    vec3 diffuse = fluidCol * (1.0 - fresnel * 0.85);
    vec3 surface = mix(diffuse, refl, fresnel * 0.9 + 0.25);

    // ---- 8. Metal tint ----
    surface *= metalColor.rgb;

    // ---- 9. HDR specular highlight ----
    vec3 lightDir = normalize(vec3(0.5, 0.8, 1.0));
    vec3 halfVec  = normalize(lightDir - viewDir);
    float ndoth   = max(dot(N, halfVec), 0.0);
    float specBase = pow(ndoth, 64.0);
    float specCore = pow(ndoth, 256.0);
    vec3  specHDR  = (vec3(specBase) * 1.5 + vec3(specCore) * 2.5) * specPower;
    surface += specHDR * metalColor.rgb;

    // ---- 10. Audio reactive pulse ----
    surface += metalColor.rgb * audioBass * 0.12;
    surface += vec3(0.6, 0.8, 1.0) * audioHigh * 0.06;

    // ---- 11. Gamma correction + edge shimmer ----
    // Approximate edge glow using normal divergence
    float edgeGlow = pow(1.0 - abs(ndotv), 4.0) * 0.4;
    surface += hsv2rgb(vec3(fract(envShift + TIME * 0.04), 0.9, 1.0)) * edgeGlow;

    surface = pow(max(surface, vec3(0.0)), vec3(1.0 / gamma));

    // ---- 12. Subtle posterise for premium feel ----
    surface = mix(surface, floor(surface * 8.0 + 0.5) / 8.0, 0.12);

    gl_FragColor = vec4(surface, 1.0);
}