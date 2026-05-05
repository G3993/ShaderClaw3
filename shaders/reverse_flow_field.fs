/*{
  "DESCRIPTION": "Aurora Curtain — 3D volumetric aurora borealis as a raymarched density field. Tall curtains of electric green/cyan/magenta on a void-black polar sky. NEW ANGLE: 3D volumetric vs prior 2D magma-palette flow field.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"curtainH",  "LABEL":"Curtain Height","TYPE":"float","MIN":0.5,"MAX":3.0, "DEFAULT":1.5},
    {"NAME":"rippleFreq","LABEL":"Ripple Freq",  "TYPE":"float","MIN":1.0,"MAX":8.0, "DEFAULT":3.5},
    {"NAME":"driftSpeed","LABEL":"Drift Speed",  "TYPE":"float","MIN":0.0,"MAX":1.0, "DEFAULT":0.22},
    {"NAME":"density",   "LABEL":"Density",      "TYPE":"float","MIN":0.5,"MAX":5.0, "DEFAULT":2.2},
    {"NAME":"hdrPeak",   "LABEL":"HDR Peak",     "TYPE":"float","MIN":1.0,"MAX":4.0, "DEFAULT":2.5},
    {"NAME":"camTilt",   "LABEL":"Camera Tilt",  "TYPE":"float","MIN":0.0,"MAX":0.5, "DEFAULT":0.18},
    {"NAME":"audioReact","LABEL":"Audio",        "TYPE":"float","MIN":0.0,"MAX":2.0, "DEFAULT":1.0}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i), hash(i+vec2(1,0)), u.x),
               mix(hash(i+vec2(0,1)), hash(i+vec2(1,1)), u.x), u.y);
}

float fbm2(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) { v += a * vnoise(p); p *= 2.1; a *= 0.5; }
    return v;
}

vec3 auroraPal(float t, float height) {
    vec3 green   = vec3(0.0, 2.5,  0.3);
    vec3 cyan    = vec3(0.0, 2.2,  2.5);
    vec3 magenta = vec3(2.5, 0.0,  2.2);
    if (t < 0.5) return mix(green, cyan, t * 2.0);
    return        mix(cyan, magenta, (t - 0.5) * 2.0);
}

float auroraDensity(vec3 p) {
    float t = TIME * driftSpeed;
    float ripple = sin(p.z * rippleFreq + t * 2.5) * 0.18
                 + sin(p.z * rippleFreq * 1.7 + t * 1.3) * 0.08;
    float curtainX = ripple + fbm2(vec2(p.z * 0.4 + t * 0.3, p.y * 0.3)) * 0.25;
    float xDist = p.x - curtainX;
    float xDens = exp(-xDist * xDist * 12.0);
    float yNorm = clamp(p.y / curtainH, 0.0, 1.0);
    float yEnv  = smoothstep(0.0, 0.15, yNorm) * smoothstep(1.0, 0.6, yNorm);
    float shimmer = fbm2(vec2(p.z * 2.0 + t * 0.8, p.y * 1.5)) * 0.5 + 0.5;
    return xDens * yEnv * shimmer * density;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    vec3 ro = vec3(0.0, 0.3, -3.0);
    vec3 ta = vec3(0.0, curtainH * 0.6, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0, 1, 0)));
    vec3 vv = cross(uu, ww);
    float tilt = camTilt + sin(TIME * 0.12) * 0.03;
    vec3 rd = normalize(uv.x * uu + (uv.y - tilt) * vv + 1.6 * ww);

    float audio = 1.0 + audioLevel * audioReact * 0.4
                      + audioBass  * audioReact * 0.3;

    vec3 col     = vec3(0.0, 0.0, 0.012);
    float alpha  = 0.0;
    float tMarch = 0.2;
    float dt     = 0.06;

    for (int i = 0; i < 60; i++) {
        if (alpha > 0.98) break;
        vec3 p = ro + rd * tMarch;
        if (tMarch > 8.0) break;
        float dens = auroraDensity(p) * dt;
        if (dens > 0.001) {
            float heightT = clamp(p.y / curtainH, 0.0, 1.0);
            vec3 aCol = auroraPal(heightT, p.y) * hdrPeak * audio;
            col   += aCol * dens * (1.0 - alpha);
            alpha += dens * 0.4;
        }
        tMarch += dt;
        dt *= 1.02;
    }

    float starField = hash(floor(uv * 180.0));
    float starBright = pow(max(starField - 0.985, 0.0) / 0.015, 2.0);
    col += vec3(0.6, 0.8, 1.0) * starBright * (1.0 - alpha);

    gl_FragColor = vec4(col, 1.0);
}
