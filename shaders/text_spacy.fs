/*{
  "DESCRIPTION": "Neon Gyroscope — 3 raymarched precessing torus rings with rim glow",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Spin Speed",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 3.0  },
    { "NAME": "ringSize",   "LABEL": "Ring Size",   "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.2, "MAX": 0.9  },
    { "NAME": "tubeRadius", "LABEL": "Tube Width",  "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.01,"MAX": 0.12 },
    { "NAME": "hdrPeak",    "LABEL": "Brightness",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 6.0  },
    { "NAME": "camDist",    "LABEL": "Camera Dist", "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.5, "MAX": 5.0  }
  ]
}*/

const float PI = 3.14159265;

#define COBALT  vec3(0.08, 0.22, 0.88)
#define CRIMSON vec3(0.78, 0.03, 0.09)
#define GOLD    vec3(1.0,  0.82, 0.0)
#define WHTHT   vec3(1.5,  1.3,  1.1)
#define VOID    vec3(0.0,  0.0,  0.012)

mat3 rotX(float a) { float c=cos(a),s=sin(a); return mat3(1,0,0, 0,c,-s, 0,s,c); }
mat3 rotY(float a) { float c=cos(a),s=sin(a); return mat3(c,0,s, 0,1,0, -s,0,c); }
mat3 rotZ(float a) { float c=cos(a),s=sin(a); return mat3(c,-s,0, s,c,0, 0,0,1); }

float sdTorus(vec3 p, float R, float r) {
    return length(vec2(length(p.xz) - R, p.y)) - r;
}
float sdSphere(vec3 p, float r) { return length(p) - r; }

// Returns (dist, matID): 1=cobalt ring, 2=crimson ring, 3=gold ring, 4=hub sphere
vec2 scene(vec3 p, float t) {
    float sp = speed;
    float R  = ringSize;
    float r  = tubeRadius;

    // Cobalt ring — XZ plane, primary rotation
    float d1 = sdTorus(rotY(t * sp * 1.0) * p, R, r);
    // Crimson ring — XY plane (rotated 90° in X), secondary rotation
    float d2 = sdTorus(rotX(PI * 0.5) * rotZ(t * sp * 0.7) * p, R, r);
    // Gold ring — diagonal (45° tilt), tertiary rotation
    float d3 = sdTorus(rotX(PI * 0.25) * rotY(-t * sp * 0.55) * p, R, r);
    // Central hub sphere
    float ds = sdSphere(p, 0.09);

    vec2 res = vec2(d1, 1.0);
    if (d2 < res.x) res = vec2(d2, 2.0);
    if (d3 < res.x) res = vec2(d3, 3.0);
    if (ds < res.x) res = vec2(ds, 4.0);
    return res;
}

vec3 calcNormal(vec3 p, float t) {
    float e = 0.0005;
    return normalize(vec3(
        scene(p + vec3(e,0,0), t).x - scene(p - vec3(e,0,0), t).x,
        scene(p + vec3(0,e,0), t).x - scene(p - vec3(0,e,0), t).x,
        scene(p + vec3(0,0,e), t).x - scene(p - vec3(0,0,e), t).x
    ));
}

vec4 renderGyro(vec2 uv) {
    float t = TIME;

    // Slowly orbiting camera
    float camA = t * speed * 0.18;
    vec3 ro  = vec3(sin(camA), 0.3 + sin(t*speed*0.11)*0.2, cos(camA)) * camDist;
    vec3 fwd = normalize(-ro);
    vec3 rgt = normalize(cross(fwd, vec3(0,1,0)));
    vec3 up  = cross(rgt, fwd);
    vec3 rd  = normalize(uv.x * rgt + uv.y * up + 1.7 * fwd);

    vec3 col = VOID;
    float dist = 0.0;
    vec2  hit  = vec2(-1.0, 0.0);

    for (int i = 0; i < 96; i++) {
        vec3  p = ro + rd * dist;
        vec2  s = scene(p, t);
        if (s.x < 0.0005) { hit = vec2(dist, s.y); break; }
        if (dist > 14.0)   break;
        dist += s.x;
    }

    if (hit.x > 0.0) {
        vec3 p = ro + rd * hit.x;
        vec3 n = calcNormal(p, t);
        vec3 ldir  = normalize(vec3(1.5, 2.0, 1.0));
        float diff = clamp(dot(n, ldir), 0.0, 1.0);
        float spec = pow(clamp(dot(reflect(-ldir, n), -rd), 0.0, 1.0), 32.0);
        float rim  = pow(1.0 - abs(dot(-rd, n)), 3.0);

        int matID = int(hit.y + 0.5);
        if (matID == 4) {
            // White-hot hub — audio-reactive
            col = WHTHT * hdrPeak * 3.0 * (1.0 + audioBass * 0.5);
        } else {
            vec3 baseCol = (matID == 1) ? COBALT : (matID == 2) ? CRIMSON : GOLD;
            col  = baseCol * (diff * 0.7 + 0.3) * hdrPeak * 1.8;
            col += WHTHT  * spec * hdrPeak * 0.8;
            col += baseCol * rim * hdrPeak * 1.2;
        }
    }

    // Additive hub glow bloom
    col += GOLD * hdrPeak * 0.14 * exp(-dot(uv,uv) * 8.0);

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);
    uv /= (1.0 + audioLevel * 0.15);
    vec4 col = renderGyro(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band       = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise  = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec4 cR = renderGyro(uv + vec2(shift + chromaAmt, 0.0));
        vec4 cG = renderGyro(uv + vec2(shift, chromaAmt * 0.5));
        vec4 cB = renderGyro(uv + vec2(shift - chromaAmt, 0.0));
        vec4 glitched = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        float scanline   = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        float blockX     = floor(uv.x * 6.0);
        float blockY     = floor(uv.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(t * 8.0)) * 43758.5);
        float dropout    = step(1.0 - g * 0.15, blockNoise);
        glitched.rgb *= scanline;
        glitched.rgb *= 1.0 - dropout;
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = col;
}
