/*{
    "DESCRIPTION": "HSV Prism Globe — standalone 3D raymarched sphere displaying the full HSV color gamut. Hue from azimuth, value from elevation. Cinematic key + rim lighting. Orbiting rotation.",
    "CATEGORIES": ["Generator", "Color", "3D"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "rotSpeed",  "LABEL": "Rotation Speed", "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 2.0  },
        { "NAME": "hdrBoost",  "LABEL": "HDR Boost",      "TYPE": "float", "DEFAULT": 2.2,  "MIN": 1.0, "MAX": 4.0  },
        { "NAME": "rimLight",  "LABEL": "Rim Intensity",  "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.0, "MAX": 5.0  },
        { "NAME": "audioReact","LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0  }
    ]
}*/

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.5 + audioBass * audioReact * 0.3;

    // Analytic ray-sphere
    vec3 ro = vec3(0.0, 0.0, 2.5);
    vec3 rd = normalize(vec3(uv, -2.0));
    float radius = 0.88 * audio;

    // Deep violet-black star background
    float starSeed = fract(sin(dot(uv * 100.0, vec2(127.1, 311.7))) * 43758.5);
    vec3 col = vec3(0.008, 0.0, 0.018) + step(0.985, starSeed) * vec3(0.6, 0.7, 1.0) * 0.8;

    float b = dot(ro, rd);
    float c2 = dot(ro, ro) - radius * radius;
    float disc = b * b - c2;

    if (disc >= 0.0) {
        float tHit = -b - sqrt(disc);
        vec3 hit = ro + rd * tHit;
        vec3 N = normalize(hit);

        // Dual-axis rotation for globe spin
        float ca = cos(t * rotSpeed);
        float sa = sin(t * rotSpeed);
        float cy = cos(t * rotSpeed * 0.41);
        float sy = sin(t * rotSpeed * 0.41);
        // Rotate Y then X
        vec3 Nr = vec3(ca*N.x + sa*N.z, N.y, -sa*N.x + ca*N.z);
        Nr = vec3(Nr.x, cy*Nr.y - sy*Nr.z, sy*Nr.y + cy*Nr.z);

        // HSV: hue = longitude, value = latitude elevation
        float hue = atan(Nr.x, Nr.z) / 6.28318 + 0.5;
        float elevation = Nr.y * 0.5 + 0.5; // 0=south, 1=north
        float val = 0.15 + 0.85 * pow(elevation, 0.35);
        vec3 surfCol = hsv2rgb(vec3(hue, 1.0, val));

        // Key light (warm white, top-right)
        vec3 keyDir = normalize(vec3(0.7, 0.6, 1.0));
        float diff = max(dot(N, keyDir), 0.08);
        // Specular (white highlight)
        float spec = pow(max(dot(reflect(-keyDir, N), -rd), 0.0), 28.0);

        // Cool violet rim light (backlit glow)
        float rim = pow(1.0 - max(dot(N, -rd), 0.0), 3.5);

        // Black ink silhouette using fwidth
        float dotNV = dot(N, -rd);
        float edgeW = fwidth(dotNV);
        float edge = 1.0 - smoothstep(-edgeW, 0.12 + edgeW, dotNV);

        col = surfCol * diff * hdrBoost
            + vec3(1.0, 1.0, 0.95) * spec * 3.0
            + vec3(0.3, 0.1, 1.0) * rim * rimLight;
        col *= 1.0 - edge * 0.92;
    }

    gl_FragColor = vec4(col, 1.0);
}
