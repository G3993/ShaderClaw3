/*{
  "DESCRIPTION": "3D instanced billboard particles — rainbow HSL cloud orbiting in perspective",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "3D", "Particles"],
  "INPUTS": [
    { "NAME": "particleCount", "TYPE": "float", "DEFAULT": 200.0, "MIN": 50.0, "MAX": 500.0, "LABEL": "Particles" },
    { "NAME": "spread", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0, "LABEL": "Spread" },
    { "NAME": "particleSize", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.005, "MAX": 0.15, "LABEL": "Size" },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0, "LABEL": "Speed" },
    { "NAME": "rotateSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "LABEL": "Camera Spin" },
    { "NAME": "pulseAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse" },
    { "NAME": "hueShift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Hue Shift" },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 1.0, "LABEL": "Brightness" },
    { "NAME": "fov", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 5.0, "LABEL": "FOV" },
    { "NAME": "bgColor", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.04, 1.0], "LABEL": "Background" }
  ]
}*/

// Hash functions for deterministic particle positions
float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }
vec3 hash3(float n) {
    return fract(sin(vec3(n, n + 1.0, n + 2.0)) * vec3(43758.5453, 22578.1459, 19642.3490));
}

// HSL to RGB
vec3 hue2rgb(float h) {
    h = fract(h);
    float r = abs(h * 6.0 - 3.0) - 1.0;
    float g = 2.0 - abs(h * 6.0 - 2.0);
    float b = 2.0 - abs(h * 6.0 - 4.0);
    return clamp(vec3(r, g, b), 0.0, 1.0);
}

vec3 hsl2rgb(float h, float s, float l) {
    vec3 rgb = hue2rgb(h);
    float c = (1.0 - abs(2.0 * l - 1.0)) * s;
    return (rgb - 0.5) * c + l;
}

// Rotation matrix around Y axis
vec3 rotateY(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
}

// Rotation matrix around X axis
vec3 rotateX(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
}

// Soft circle billboard
float billboard(vec2 uv, vec2 center, float radius) {
    float d = length(uv - center);
    return smoothstep(radius, radius * 0.3, d);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    float t = TIME * speed;
    int N = int(particleCount);

    vec3 col = bgColor.rgb;

    // Camera orbits around origin
    float camAngleY = t * rotateSpeed * 0.5;
    float camAngleX = sin(t * rotateSpeed * 0.2) * 0.3;

    for (int i = 0; i < 500; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Deterministic 3D position in [-1, 1] cube
        vec3 pos = (hash3(fi * 3.7) * 2.0 - 1.0) * spread;

        // Sine-based pulsing (matching Three.js example)
        vec3 trTime = pos + t;
        float scale = sin(trTime.x * 2.1) + sin(trTime.y * 3.2) + sin(trTime.z * 4.3);
        float sizeScale = mix(1.0, (scale * 0.5 + 1.0), pulseAmount);

        // Rotate world by camera angle
        pos = rotateY(pos, camAngleY);
        pos = rotateX(pos, camAngleX);

        // Perspective projection
        float z = pos.z + fov;
        if (z < 0.1) continue;
        vec2 projected = pos.xy / z;

        // Screen-space size with perspective
        float sz = particleSize * sizeScale / z;

        // Draw billboard
        float d = billboard(p, projected, sz);

        if (d > 0.0) {
            // HSL coloring based on scale (like Three.js example)
            float hue = scale / 5.0 + hueShift;
            vec3 particleCol = hsl2rgb(hue, 1.0, brightness);

            // Depth fade — farther particles are dimmer
            float depthFade = smoothstep(fov + spread * 2.0, 0.5, z);

            col = mix(col, particleCol, d * depthFade);
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
