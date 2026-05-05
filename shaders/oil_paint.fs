/*{
    "DESCRIPTION": "Impasto Terrain — raymarched FBM-displaced plane rendered as thick Van Gogh brushwork. Prussian blue / cadmium yellow palette, strong directional painterly light. New angle: cool blue-yellow contrast vs prior warm lava.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Painterly"],
    "INPUTS": [
        { "NAME": "terrainScale", "LABEL": "Terrain Scale", "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5,  "MAX": 6.0 },
        { "NAME": "brushHeight",  "LABEL": "Brush Height",  "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.05, "MAX": 1.0 },
        { "NAME": "flowSpeed",    "LABEL": "Flow Speed",    "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "hdrBoost",     "LABEL": "HDR Boost",     "TYPE": "float", "DEFAULT": 2.3,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

// Value noise
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Domain-warped FBM → thick brushwork
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p  = p * 2.1 + vec2(1.7, 9.3);
        a *= 0.5;
    }
    return v;
}

float terrain(vec2 p, float t) {
    // Domain warp for swirling brush strokes
    vec2 q = vec2(fbm(p + vec2(0.0, 0.0) + t * flowSpeed),
                  fbm(p + vec2(5.2, 1.3) + t * flowSpeed * 0.7));
    vec2 r = vec2(fbm(p + q * 4.0 + vec2(1.7, 9.2) + t * flowSpeed * 0.5),
                  fbm(p + q * 4.0 + vec2(8.3, 2.8) + t * flowSpeed * 0.3));
    return fbm(p + r * 3.5);
}

float scene(vec3 p, float t) {
    float h = terrain(p.xz * terrainScale, t) * brushHeight;
    return p.y - h;
}

vec3 calcNormal(vec3 p, float t) {
    const vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t) - scene(p - e.xyy, t),
        scene(p + e.yxy, t) - scene(p - e.yxy, t),
        scene(p + e.yyx, t) - scene(p - e.yyx, t)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.3;

    // Camera: looking down at the terrain at an angle
    vec3 ro = vec3(sin(t * 0.06) * 0.5, 1.2 + audio * 0.1, 0.8 + t * flowSpeed * 0.5);
    vec3 target = ro + vec3(sin(t * 0.04) * 0.2, -0.7, -1.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.4 + uv.x * right + uv.y * up);

    // March
    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dt;
        dSurf = scene(p, t);
        if (dSurf < 0.002) { hit = true; break; }
        if (dt > 10.0) break;
        dt += max(abs(dSurf) * 0.5, 0.008);
    }

    // Sky: deep Prussian blue gradient
    float skyT = clamp(uv.y * 0.5 + 0.5, 0.0, 1.0);
    vec3 col = mix(vec3(0.04, 0.08, 0.22), vec3(0.01, 0.03, 0.14), skyT);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);
        float h  = terrain(p.xz * terrainScale, t);

        // Van Gogh palette: 4 colors
        // Deep Prussian blue, cadmium yellow, viridian, white-hot ridge
        vec3 blue    = vec3(0.05, 0.12, 0.50);
        vec3 yellow  = vec3(1.00, 0.80, 0.05);
        vec3 virid   = vec3(0.05, 0.45, 0.22);
        vec3 whiteHot = vec3(1.00, 0.95, 0.80);

        // Height-based color blending
        vec3 terrCol = mix(blue,   virid,  smoothstep(0.1, 0.4, h));
        terrCol      = mix(terrCol, yellow, smoothstep(0.45, 0.75, h));
        terrCol      = mix(terrCol, whiteHot, smoothstep(0.80, 1.0, h));

        // Painterly directional light (upper-right, warm)
        vec3 sunDir = normalize(vec3(1.0, 1.8, 0.4));
        float diff  = max(0.0, dot(nor, sunDir));
        float spec  = pow(max(0.0, dot(reflect(-sunDir, nor), -rd)), 12.0);
        float sss   = max(0.0, dot(-nor, sunDir)) * 0.25; // subsurface scatter on back faces

        col  = terrCol * (diff * 0.8 + sss + 0.06);
        col += whiteHot * spec * hdrBoost;
        col += yellow   * diff * diff * hdrBoost * 0.4;    // HDR ridge gilding
        col *= hdrBoost * 0.85;

        // Black ink crevice via fwidth AA
        float ew   = fwidth(dSurf) * 3.0;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
