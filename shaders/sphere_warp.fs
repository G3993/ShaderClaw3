/*{
  "DESCRIPTION": "Sphere Warp — maps video onto a sphere bulge, mouse controls center",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "bulge", "LABEL": "Bulge", "TYPE": "float", "DEFAULT": 0.5, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "radius", "LABEL": "Radius", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "refract", "LABEL": "Refraction", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "chromatic", "LABEL": "Chromatic", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float bass = smoothstep(0.0, 0.3, audioBass);
    float mid = smoothstep(0.0, 0.3, audioMid);
    float high = smoothstep(0.0, 0.3, audioHigh);

    vec2 center = mousePos;
    vec2 p = uv - center;
    p.x *= aspect;

    float r = length(p);
    float rad = radius * (1.0 + bass * 0.3 + audioBass * 0.2);
    float b = bulge * (1.0 + bass * 0.5 + audioBass * 0.8);

    vec2 warpUV = uv;
    float sphereMask = 0.0;

    if (r < rad) {
        // Inside sphere
        float nr = r / rad; // 0 at center, 1 at edge
        float theta = asin(clamp(nr, 0.0, 1.0));

        // Barrel/pincushion distortion
        float distortion = 1.0 + b * (1.0 - nr * nr);

        vec2 offset = p * (distortion - 1.0) / aspect;
        offset.x /= aspect;
        warpUV = uv + offset * vec2(1.0, 1.0);

        // Refraction shift — mid drives refraction
        float effectiveRefract = refract + mid * 0.4;
        if (effectiveRefract > 0.001) {
            vec2 normal2d = normalize(p);
            warpUV += normal2d * effectiveRefract * 0.05 * (1.0 - nr);
        }

        sphereMask = 1.0 - nr;
    }

    warpUV = clamp(warpUV, 0.0, 1.0);

    vec3 col;
    if (hasInput) {
        float effectiveChromatic = chromatic + high * 0.5;
        if (effectiveChromatic > 0.001 && r < rad) {
            float ca = effectiveChromatic * 0.01 * sphereMask;
            vec2 dir = normalize(p + 0.001);
            float rv = texture2D(inputTex, warpUV + dir * ca).r;
            float gv = texture2D(inputTex, warpUV).g;
            float bv = texture2D(inputTex, warpUV - dir * ca).b;
            col = vec3(rv, gv, bv);
        } else {
            col = texture2D(inputTex, warpUV).rgb;
        }
    } else {
        col = vec3(0.3 + 0.3 * sin(warpUV.x * 20.0), 0.3 + 0.3 * cos(warpUV.y * 15.0), 0.5);
    }

    // Specular highlight on sphere
    if (r < rad) {
        float spec = pow(max(1.0 - length(p / rad - vec2(-0.3, -0.3)), 0.0), 8.0) * 0.4 * abs(b);
        col += vec3(spec);
    }

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
