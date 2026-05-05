/*{
    "DESCRIPTION": "Deep Sea Bioluminescence — 3D volumetric ocean trench. Layered FBM plankton glow dots in deep navy water with electric teal caustics. Cool night palette: contrasts prior warm magma flow.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Volumetric"],
    "INPUTS": [
        { "NAME": "depthScale",   "LABEL": "Depth Scale",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 6.0 },
        { "NAME": "planktonDens", "LABEL": "Plankton Dens", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.1, "MAX": 2.0 },
        { "NAME": "driftSpeed",   "LABEL": "Drift Speed",   "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "hdrBoost",     "LABEL": "HDR Boost",     "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 4.0 },
        { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

float hash31(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise3(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(mix(hash31(i),              hash31(i + vec3(1,0,0)), f.x),
            mix(hash31(i + vec3(0,1,0)), hash31(i + vec3(1,1,0)), f.x), f.y),
        mix(mix(hash31(i + vec3(0,0,1)), hash31(i + vec3(1,0,1)), f.x),
            mix(hash31(i + vec3(0,1,1)), hash31(i + vec3(1,1,1)), f.x), f.y),
        f.z
    );
}

float fbm3(vec3 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise3(p);
        p = p * 2.0 + vec3(1.73, 9.31, 4.17);
        a *= 0.5;
    }
    return v;
}

float planktonGlow(vec3 p, float t) {
    vec3 flow = p + vec3(sin(t * 0.13), cos(t * 0.09), t * driftSpeed);
    float dens = fbm3(flow * depthScale);
    float dot_ = smoothstep(0.55 + 0.1 * planktonDens, 0.75 + 0.1 * planktonDens, dens);
    return dot_ * dot_;
}

float oceanFloor(vec3 p, float t) {
    float h = fbm3(p * vec3(1.0, 0.0, 1.0) * 0.8 + vec3(0.0, t * 0.03, 0.0)) * 0.4;
    return p.y + 1.5 + h;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.35;

    vec3 ro = vec3(sin(t * 0.07) * 0.6, 0.3 + sin(t * 0.05) * 0.4, t * driftSpeed * 0.5);
    vec3 fwd   = normalize(vec3(sin(t * 0.04) * 0.1, -0.15, -1.0));
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.4 + uv.x * right + uv.y * up);

    // 4-color palette: abyssal navy, electric teal, bioluminescent cyan, white-hot
    vec3 navy    = vec3(0.01, 0.02, 0.09);
    vec3 teal    = vec3(0.00, 0.65, 0.55);
    vec3 bioBlue = vec3(0.00, 0.90, 1.00);
    vec3 whiteHot = vec3(0.80, 1.00, 1.00);

    vec3 col  = navy;
    float dt  = 0.0;
    float accum = 0.0;
    vec3  glowCol = vec3(0.0);
    bool  floorHit = false;
    float floorDist = 0.0;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dt;
        float floorD = oceanFloor(p, t);

        if (floorD < 0.05) { floorHit = true; floorDist = dt; break; }
        if (dt > 8.0) break;

        float g = planktonGlow(p, t) * audio * hdrBoost;
        if (g > 0.01) {
            float hue = fbm3(p * 1.5 + t * 0.07) * 0.4 + 0.45;
            vec3 pCol = mix(teal, bioBlue, clamp(hue * 2.0 - 0.9, 0.0, 1.0));
            pCol = mix(pCol, whiteHot, smoothstep(0.8, 1.0, g / hdrBoost));
            glowCol += pCol * g * 0.08 * (1.0 - accum);
            accum   += g * 0.05;
        }
        dt += 0.12;
    }

    col += glowCol;

    if (floorHit) {
        vec3 p = ro + rd * floorDist;
        float caustic = sin(p.x * 4.0 + t * 0.8) * sin(p.z * 4.0 + t * 0.6);
        caustic = smoothstep(0.3, 0.9, caustic * 0.5 + 0.5);
        vec3 floorCol = navy * 0.3 + teal * caustic * hdrBoost * 0.6;
        float fog = exp(-floorDist * 0.15);
        col += floorCol * fog;
    }

    float fog = exp(-dt * 0.06);
    col = mix(navy, col, fog);

    gl_FragColor = vec4(col, 1.0);
}
