/*{
  "DESCRIPTION": "Marble 3D — sinusoidal wisp veins on a raymarched slab with bump-mapped calcite reflections and kintsugi gold flash",
  "CREDIT": "Based on SaturdayShader Week 30 by Joseph Fiola / bonniem",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Vein Texture",    "TYPE": "image" },
    { "NAME": "texMix",     "LABEL": "Texture Mix",     "TYPE": "float", "DEFAULT": 0,     "MIN": 0,    "MAX": 1 },
    { "NAME": "lines",      "LABEL": "Lines",           "TYPE": "float", "DEFAULT": 54,    "MIN": 1,    "MAX": 200 },
    { "NAME": "linesStartOffset","LABEL":"Start Offset","TYPE": "float", "DEFAULT": 0.785, "MIN": 0,    "MAX": 1 },
    { "NAME": "amp",        "LABEL": "Amplitude",       "TYPE": "float", "DEFAULT": 0.1551,"MIN": 0,    "MAX": 1 },
    { "NAME": "glowAmt",    "LABEL": "Glow",            "TYPE": "float", "DEFAULT": -8.2,  "MIN": -40,  "MAX": 0 },
    { "NAME": "mod1",       "LABEL": "Mod 1",           "TYPE": "float", "DEFAULT": 0.13,  "MIN": -0.5, "MAX": 0.5 },
    { "NAME": "mod2",       "LABEL": "Mod 2",           "TYPE": "float", "DEFAULT": -0.3,  "MIN": -1,   "MAX": 1 },
    { "NAME": "twisted",    "LABEL": "Twist",           "TYPE": "float", "DEFAULT": -0.5,  "MIN": -0.5, "MAX": 1.4095 },
    { "NAME": "zoomAmt",    "LABEL": "Zoom",            "TYPE": "float", "DEFAULT": 10,    "MIN": 0,    "MAX": 100 },
    { "NAME": "rotateCanvas","LABEL":"Rotate",          "TYPE": "float", "DEFAULT": 0,     "MIN": 0,    "MAX": 1 },
    { "NAME": "scroll",     "LABEL": "Scroll",          "TYPE": "float", "DEFAULT": 0,     "MIN": 0,    "MAX": 1 },
    { "NAME": "audioReact", "LABEL": "Audio React",     "TYPE": "float", "DEFAULT": 1.0,   "MIN": 0,    "MAX": 2 },
    { "NAME": "veinColor",  "LABEL": "Vein Color",      "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1] },
    { "NAME": "baseColor",  "LABEL": "Base Color",      "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1] },
    { "NAME": "transparentBg","LABEL":"Transparent BG", "TYPE": "bool",  "DEFAULT": false }
  ]
}*/

#define PI     3.14159265359
#define TWO_PI 6.28318530718

mat2 rotate2d(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Camera — orbiting above the slab
    float camA = TIME * 0.07;
    float camH = 2.2;
    vec3 ro = vec3(sin(camA) * 1.6, camH, cos(camA) * 1.6);
    vec3 fwd = normalize(-ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec2 ndc = (uv - 0.5) * vec2(aspect, 1.0);
    vec3 rd = normalize(fwd + ndc.x * right + ndc.y * up);

    // Dark background
    vec3 col = vec3(0.01, 0.01, 0.02);
    float alpha = transparentBg ? 0.0 : 1.0;

    // Ray-plane intersection at y = 0 (marble slab face)
    if (abs(rd.y) > 0.001) {
        float t = -ro.y / rd.y;
        if (t > 0.01) {
            vec3 hit = ro + t * rd;

            // Slab bounds: 1.5 × 1.5
            if (abs(hit.x) <= 1.5 && abs(hit.z) <= 1.5) {

                // Marble UV — same transforms as original 2D version
                vec2 rawUV = hit.xz * 0.5 + 0.5;  // [0,1] on slab face
                vec2 mu = hit.xz - vec2((mousePos - 0.5) * 2.0);
                mu *= zoomAmt * 0.1;
                mu = rotate2d(rotateCanvas * -TWO_PI) * mu;
                mu.y += scroll * 5.0;

                // Marble wisp accumulation
                float totalGlow = 0.0;
                float bass = 0.5 + 0.5 * audioBass * audioReact;
                for (float i = 0.0; i < 200.0; i++) {
                    mu = rotate2d(amp * (1.0 + audioBass * audioReact * 2.0) + twisted * -TWO_PI) * mu;
                    if (lines <= i) break;
                    mu.y += sin(sin(mu.x + i * mod1 + (scroll * TWO_PI)) * amp + (mod2 * PI) + TIME / 4.2);
                    if (lines * linesStartOffset - 1.0 <= i) {
                        float ww = abs(1.0 / (50.0 * mu.y * glowAmt * (1.0 + audioLevel * audioReact)));
                        totalGlow += ww;
                    }
                }

                // Bump-map normal from screen-space derivative of wisp field
                float dhdx = dFdx(totalGlow);
                float dhdz = dFdy(totalGlow);
                vec3 N = normalize(vec3(-dhdx * 0.4, 1.0, -dhdz * 0.4));

                // Blinn-Phong lighting
                vec3 L = normalize(vec3(0.5, 1.0, 0.3));
                vec3 V = normalize(-rd);
                vec3 H = normalize(L + V);
                float diff = max(dot(N, L), 0.0);
                float spec = pow(max(dot(N, H), 0.0), 48.0);

                // Marble material — HDR veins (no clamp on totalGlow)
                float intensity = totalGlow;
                vec3 marble = mix(baseColor.rgb, veinColor.rgb, clamp(intensity / 3.0, 0.0, 1.0));
                marble += veinColor.rgb * intensity * 0.5;  // additive glow

                // Optional texture on vein areas
                if (texMix > 0.001 && IMG_SIZE_inputImage.x > 0.0) {
                    vec3 tex = texture2D(inputImage, rawUV).rgb;
                    float veinMask = clamp(intensity / 1.5, 0.0, 1.0);
                    marble = mix(marble, tex * (0.5 + intensity * 0.3), texMix * veinMask);
                }

                // Lighting on marble surface — calcite specular (HDR peaks)
                col = marble * (0.1 + diff * 0.9);
                col += vec3(1.0, 0.98, 0.95) * spec * 2.5 * (0.7 + bass * 0.6);

                // Slab edge glow
                float edgeX = 1.5 - abs(hit.x);
                float edgeZ = 1.5 - abs(hit.z);
                float edgeDist = min(edgeX, edgeZ);
                col += veinColor.rgb * 0.3 * exp(-edgeDist * 8.0) * bass;

                alpha = 1.0;

                // Kintsugi gold vein — additive HDR flash every ~45s
                {
                    float _ph = fract(TIME / 45.0);
                    float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.20, 0.10, _ph);
                    float _y = 0.3 + 0.4 * fract(floor(TIME / 45.0) * 0.71);
                    float _vein = exp(-pow((rawUV.y - _y - 0.04 * sin(rawUV.x * 18.0 + TIME)) * 200.0, 2.0));
                    col += vec3(1.0, 0.82, 0.30) * _vein * _f * 2.0;
                }
            }
        }
    }

    // Subtle vignette
    col *= 1.0 - 0.4 * dot(uv - 0.5, uv - 0.5) * 3.0;

    gl_FragColor = vec4(col, alpha);
}
