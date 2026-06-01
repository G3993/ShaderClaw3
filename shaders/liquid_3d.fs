/*{
  "DESCRIPTION": "3D Liquid — a self-contained, single-pass raymarched violet metaball fluid in the spirit of Wyatt's 3D SPH shader (shadertoy.com/view/mstfzS). The original is a multi-buffer particle simulation; this is a procedural approximation of the LOOK: an orbiting liquid blob raymarched with diffuse + procedural-environment reflection + fresnel rim, in the original's exact camera rig and palette. Knobs: BLOBS, VISCOSITY (merge smoothness), SPEED, SIZE, REFLECT, AUTO SPIN, and two liquid colors. All sliders are audio-bindable.",
  "CREDIT": "Easel · liquid_3d  (after Wyatt 'SPH 3D', shadertoy.com/view/mstfzS)",
  "CATEGORIES": ["Generator", "3D", "Fluid"],
  "INPUTS": [
    { "NAME": "blobs",     "LABEL": "Blobs",     "TYPE": "float", "MIN": 3.0, "MAX": 16.0, "DEFAULT": 11.0 },
    { "NAME": "viscosity", "LABEL": "Viscosity", "TYPE": "float", "MIN": 0.2, "MAX": 2.5,  "DEFAULT": 1.2 },
    { "NAME": "speed",     "LABEL": "Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 0.7 },
    { "NAME": "blobSize",  "LABEL": "Size",      "TYPE": "float", "MIN": 0.4, "MAX": 1.6,  "DEFAULT": 0.9 },
    { "NAME": "reflAmt",   "LABEL": "Reflect",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6 },
    { "NAME": "spin",      "LABEL": "Auto Spin", "TYPE": "float", "MIN": 0.0, "MAX": 1.5,  "DEFAULT": 0.25 },
    { "NAME": "colA", "LABEL": "Deep", "TYPE": "color", "DEFAULT": [0.220, 0.349, 1.000, 1.0] },
    { "NAME": "colB", "LABEL": "Glow", "TYPE": "color", "DEFAULT": [0.420, 0.302, 0.996, 1.0] }
  ]
}*/

#define FOV 2.5

// ── Camera rig — verbatim from the original (theta/phi rotation) ─────
mat3 getCamera(vec2 a){
    mat3 t = mat3(1.0, 0.0, 0.0,
                  0.0, cos(a.y), -sin(a.y),
                  0.0, sin(a.y),  cos(a.y));
    mat3 p = mat3(cos(a.x), sin(a.x), 0.0,
                 -sin(a.x), cos(a.x), 0.0,
                  0.0, 0.0, 1.0);
    return t * p;
}
vec3 getRay(vec2 a, vec2 pos){
    mat3 cam = getCamera(a);
    return normalize(transpose(cam) * vec3(FOV * pos.x, 1.0, FOV * pos.y));
}

float hash(float n){ return fract(sin(n) * 43758.5453); }

// Smooth-union of distance fields → the merging-droplet "liquid" look that
// stands in for SPH particle clustering.
float smin(float a, float b, float k){
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// Metaball fluid field. Up to 16 animated droplets orbiting/bobbing inside
// a ~unit sphere, smooth-unioned by VISCOSITY.
float map(vec3 p){
    float t = TIME * speed;
    float d = 1e9;
    int   n = int(blobs);
    float k = 0.35 * viscosity;
    for (int i = 0; i < 16; i++){
        if (i >= n) break;
        float fi = float(i);
        vec3 c = vec3(
            sin(t * (0.50 + 0.30 * hash(fi))       + fi * 1.7),
            sin(t * (0.40 + 0.35 * hash(fi + 9.0))  + fi * 2.3),
            cos(t * (0.45 + 0.25 * hash(fi + 3.0))  + fi * 1.1)
        ) * (1.10 + 0.30 * hash(fi + 5.0));
        float r = blobSize * (0.55 + 0.40 * hash(fi + 7.0));
        d = smin(d, length(p - c) - r, k);
    }
    return d;
}

vec3 calcNormal(vec3 p){
    vec2 e = vec2(0.012, 0.0);
    return normalize(vec3(
        map(p + e.xyy) - map(p - e.xyy),
        map(p + e.yxy) - map(p - e.yxy),
        map(p + e.yyx) - map(p - e.yyx)));
}

// Procedural environment — stands in for the original's iChannel3 cubemap.
vec3 env(vec3 d){
    float up  = d.y * 0.5 + 0.5;
    vec3  sky = mix(vec3(0.03, 0.04, 0.09), vec3(0.28, 0.42, 0.85), up);
    float s   = pow(max(dot(d, normalize(vec3(0.5, 0.8, 0.3))), 0.0), 18.0);
    sky += vec3(1.0, 0.95, 0.85) * s * 1.6;       // key-light hotspot
    return sky;
}

void main(){
    vec2 R  = RENDERSIZE;
    vec2 uv = (gl_FragCoord.xy - 0.5 * R) / max(R.x, R.y);

    // Auto-orbit camera (the original's no-mouse fallback path).
    vec2 angles = vec2(TIME * spin * 0.5, -0.5);
    vec3 rd  = getRay(angles, uv);
    vec3 crd = getRay(angles, vec2(0.0));
    vec3 ro  = -crd * 4.5;

    vec3 col = env(rd);

    // Sphere-march the metaball field.
    float t = 0.0; bool hit = false; vec3 p = ro;
    for (int i = 0; i < 90; i++){
        p = ro + rd * t;
        float d = map(p);
        if (d < 0.002){ hit = true; break; }
        t += d;
        if (t > 12.0) break;
    }

    if (hit){
        vec3  nrm  = calcNormal(p);
        vec3  L    = normalize(vec3(0.5, 0.8, 0.3));
        float diff = clamp(dot(nrm, L) * 0.5 + 0.5, 0.0, 1.0);   // half-Lambert
        vec3  refl = reflect(rd, nrm);
        vec3  envR = env(refl);
        float fres = pow(1.0 - max(dot(nrm, -rd), 0.0), 3.0);
        float grad = clamp(length(p) * 0.32, 0.0, 1.0);
        vec3  albedo = mix(colA.rgb, colB.rgb, grad);
        col  = albedo * (0.25 + 1.60 * diff);
        col  = mix(col, envR, reflAmt * (0.25 + 0.75 * fres));
        col += colB.rgb * fres * 0.40;                            // fresnel rim glow
    }

    // Tonemap + gamma.
    col = col / (1.0 + col);
    col = pow(max(col, 0.0), vec3(1.0 / 2.2));
    gl_FragColor = vec4(col, 1.0);
}
