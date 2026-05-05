/*{
    "DESCRIPTION": "Supernova Remnant — 3D raymarched shock-wave shell with turbulent ejecta filaments and star field",
    "CATEGORIES": ["Generator", "3D"],
    "CREDIT": "ShaderClaw / Supernova Remnant v1",
    "INPUTS": [
        { "NAME": "shellRadius",     "TYPE": "float", "DEFAULT": 0.70, "MIN": 0.20, "MAX": 1.20, "LABEL": "Shell Radius" },
        { "NAME": "shellThick",      "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.05, "MAX": 0.40, "LABEL": "Shell Thickness" },
        { "NAME": "rotSpeed",        "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.00, "MAX": 0.50, "LABEL": "Rotation" },
        { "NAME": "turbScale",       "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 6.0,  "LABEL": "Turbulence" },
        { "NAME": "hdrPeak",         "TYPE": "float", "DEFAULT": 3.5,  "MIN": 1.0,  "MAX": 6.0,  "LABEL": "HDR Peak" },
        { "NAME": "pulse",           "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Bass Pulse" },
        { "NAME": "audioReactivity", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio" }
    ]
}*/

float h31(vec3 p) {
    p = fract(p * vec3(443.89, 397.29, 491.18));
    p += dot(p, p.yzx + 19.27);
    return fract(p.x * p.y * p.z);
}

float vnoise3(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(mix(h31(i),             h31(i+vec3(1,0,0)), f.x),
            mix(h31(i+vec3(0,1,0)), h31(i+vec3(1,1,0)), f.x), f.y),
        mix(mix(h31(i+vec3(0,0,1)), h31(i+vec3(1,0,1)), f.x),
            mix(h31(i+vec3(0,1,1)), h31(i+vec3(1,1,1)), f.x), f.y),
        f.z);
}

float fbm3(vec3 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise3(p);
        p  = p * 2.07 + vec3(1.7, 9.2, 5.3);
        a *= 0.48;
    }
    return v;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = audioLevel + audioBass * pulse * audioReactivity;

    // Camera orbiting slowly around nebula
    float ang = TIME * rotSpeed;
    vec3 ro  = vec3(sin(ang) * 2.8, 0.18, cos(ang) * 2.8);
    vec3 fw  = normalize(-ro);
    vec3 rt  = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 upV = cross(rt, fw);
    vec3 rd  = normalize(fw + uv.x * rt * 0.55 + uv.y * upV * 0.55);

    // Star background — quantized celestial sphere grid
    vec3 col = vec3(0.0);
    {
        vec2 cellCoord = vec2(atan(rd.z, rd.x) * 8.0, rd.y * 14.0);
        vec2 ci = floor(cellCoord);
        vec2 cf = fract(cellCoord);
        float sh  = fract(sin(dot(ci, vec2(127.1, 311.7))) * 43758.5);
        float sh2 = fract(sin(dot(ci, vec2(269.5, 183.3))) * 43758.5);
        if (sh > 0.980) {
            vec2 starPos = vec2(sh2, fract(sh * 7.3));
            float starB  = max(0.0, 1.0 - length(cf - starPos) * 5.0);
            col += vec3(0.7, 0.82, 1.0) * pow(starB, 2.5) * 1.4;
        }
    }

    // Shell radius breathes with bass
    float sR = shellRadius + audioBass * pulse * 0.06;

    // Volumetric raymarch through the shell
    float t = 0.5;
    for (int i = 0; i < 72; i++) {
        vec3 p = ro + rd * t;
        float r = length(p);

        // Gaussian shell density centered on sR
        float dr   = r - sR;
        float dens = exp(-dr * dr / (shellThick * shellThick));

        if (dens > 0.002) {
            // Domain-warped FBM for filamentary ejecta structure
            vec3 wp = p * turbScale + TIME * 0.055;
            vec3 q  = vec3(fbm3(wp),
                           fbm3(wp + vec3(5.2, 1.3, 8.4)),
                           fbm3(wp + vec3(2.9, 7.1, 3.6)));
            float noise    = fbm3(wp + q * 0.9);
            float filament = dens * pow(max(noise, 0.0), 1.6);

            // Color ramp outer→inner: violet → crimson → orange → white-hot
            float normR = clamp((dr + shellThick) / (2.0 * shellThick), 0.0, 1.0);
            vec3 C0 = vec3(0.18, 0.04, 1.00) * hdrPeak * 0.60; // violet outer shock
            vec3 C1 = vec3(1.00, 0.10, 0.18) * hdrPeak * 0.85; // crimson mid
            vec3 C2 = vec3(1.00, 0.55, 0.08) * hdrPeak * 1.10; // orange inner
            vec3 C3 = vec3(1.00, 0.92, 0.70) * hdrPeak * 1.25; // white-hot core
            vec3 shellCol = mix(C0,
                             mix(C1,
                                 mix(C2, C3, smoothstep(0.60, 1.00, normR)),
                                 smoothstep(0.30, 0.60, normR)),
                             smoothstep(0.00, 0.30, normR));

            col += shellCol * filament * 0.09 * (1.0 + audio * 0.35);
        }

        // Neutron star remnant glow at center
        float core = exp(-r * r * 16.0);
        col += vec3(1.0, 0.92, 0.8) * core * hdrPeak * 0.45;

        t += 0.055;
        if (t > 6.0) break;
    }

    gl_FragColor = vec4(col, 1.0);
}
