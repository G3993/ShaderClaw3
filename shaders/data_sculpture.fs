/*{
  "DESCRIPTION": "Data Sculpture — thousands of cubes form a fluid 3D landscape from input imagery",
  "CATEGORIES": ["Radiant"],
  "INPUTS": [
    { "NAME": "baseColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "gridDensity", "LABEL": "Density", "TYPE": "float", "DEFAULT": 14.0, "MIN": 4.0, "MAX": 28.0 },
    { "NAME": "cubeScale", "LABEL": "Cube Size", "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.15, "MAX": 0.95 },
    { "NAME": "waveHeight", "LABEL": "Wave Height", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "camHeight", "LABEL": "Camera Height", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fogAmount", "LABEL": "Atmosphere", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

#define STEPS 36
#define HIT 0.002
#define FAR 14.0
#define PI 3.14159265
#define EXT 5.0

// ---- Fast hash ----
float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// ---- Smooth value noise 3D (much cheaper than simplex) ----
float vnoise(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    vec2 o = vec2(0.0, 1.0);
    float a = hash(i.xy + i.z * 37.0);
    float b = hash(i.xy + o.yx + i.z * 37.0);
    float c = hash(i.xy + o.xy + i.z * 37.0);
    float d = hash(i.xy + o.yy + i.z * 37.0);
    float e = hash(i.xy + (i.z + 1.0) * 37.0);
    float ff = hash(i.xy + o.yx + (i.z + 1.0) * 37.0);
    float g = hash(i.xy + o.xy + (i.z + 1.0) * 37.0);
    float h = hash(i.xy + o.yy + (i.z + 1.0) * 37.0);
    float z0 = mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
    float z1 = mix(mix(e, ff, f.x), mix(g, h, f.x), f.y);
    return mix(z0, z1, f.z) * 2.0 - 1.0;
}

// ---- SDF ----
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ---- Globals ----
vec2 g_cell;
float g_h;

// ---- Scene: single cell lookup (no neighbor loop) ----
float scene(vec3 p) {
    float sp = 1.0 / gridDensity;
    float ch = sp * 0.5 * cubeScale;

    // Bounds
    if (abs(p.x) > EXT + sp || abs(p.z) > EXT + sp)
        return max(abs(p.x) - EXT, abs(p.z) - EXT);
    float maxH = waveHeight + 1.0;
    if (p.y > maxH) return p.y - maxH + 0.1;
    if (p.y < -maxH) return -p.y - maxH + 0.1;

    // Current cell only — no neighbor loop (9x faster)
    vec2 id = floor(p.xz / sp);
    vec2 center = (id + 0.5) * sp;

    // Flow displacement
    float h1 = hash(id);
    float h2 = hash(id + 73.0);
    float phase = TIME * flowSpeed * 0.4 + h1 * 6.28;
    vec2 flow = vec2(sin(phase + h2 * 3.0), cos(phase * 0.7 + h1 * 5.0));
    vec2 disp = center + flow * sp * 0.25;

    // Height from cheap noise + texture
    float t = TIME;
    float fs = flowSpeed;
    float n = vnoise(vec3((disp + vec2(t * fs * 0.3, t * fs * 0.15)) * 1.5, t * fs));

    vec2 texUV = clamp((center + EXT) / (2.0 * EXT), 0.0, 1.0);
    vec4 tex = texture2D(inputTex, texUV);

    float ht;
    if (tex.a > 0.01) {
        float lum = dot(tex.rgb, vec3(0.299, 0.587, 0.114));
        ht = lum * 0.65 + n * 0.35;
    } else {
        ht = n;
    }

    // Edge fade
    float r = length(center) / EXT;
    ht *= smoothstep(1.0, 0.85, r);
    ht += audioBass * 0.12 * (1.0 - r);
    ht *= waveHeight;

    // Cube
    float vScale = 0.6 + h1 * 0.8;
    vec3 cubePos = vec3(disp.x, ht, disp.y);
    vec3 q = p - cubePos;

    float rot = h2 * 0.2;
    float cr = cos(rot), sr = sin(rot);
    q.xz = vec2(cr * q.x - sr * q.z, sr * q.x + cr * q.z);

    g_cell = center;
    g_h = ht;

    return sdBox(q, vec3(ch, ch * vScale, ch));
}

// ---- Normal (tetrahedron method) ----
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.003, -0.003);
    return normalize(
        e.xyy * scene(p + e.xyy) +
        e.yyx * scene(p + e.yyx) +
        e.yxy * scene(p + e.yxy) +
        e.xxx * scene(p + e.xxx)
    );
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Camera — spherical orbit, camHeight 0=side, 1=top-down
    float angle = TIME * 0.1;
    float elevAngle = camHeight * PI * 0.48;
    float camDist = 4.5;
    vec3 ro = vec3(
        sin(angle) * cos(elevAngle) * camDist,
        sin(elevAngle) * camDist + 0.5,
        cos(angle) * cos(elevAngle) * camDist
    );
    vec3 fwd = normalize(-ro);
    vec3 worldUp = abs(fwd.y) > 0.99 ? vec3(0.001, 0.0, 1.0) : vec3(0.0, 1.0, 0.0);
    vec3 right = normalize(cross(fwd, worldUp));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd * 1.2 + right * uv.x + up * uv.y);

    // Raymarch
    float t = 0.0;
    bool hit = false;
    vec3 p;
    float glow = 0.0;

    for (int i = 0; i < STEPS; i++) {
        p = ro + rd * t;
        float d = scene(p);
        glow += 0.003 / (0.15 + d * d);
        if (d < HIT) { hit = true; break; }
        if (t > FAR) break;
        t += d;
    }

    vec3 col = vec3(0.0);
    float alpha = 0.0;

    if (hit) {
        scene(p);
        vec3 n = calcNormal(p);
        vec3 v = normalize(ro - p);

        // Lighting
        vec3 L = normalize(vec3(1.5, 3.0, 2.0));
        vec3 H = normalize(L + v);
        float diff = max(dot(n, L), 0.0);
        float spec = pow(max(dot(n, H), 0.0), 48.0);
        float fres = pow(1.0 - max(dot(n, v), 0.0), 3.0);

        // Color
        vec2 texUV = clamp((g_cell + EXT) / (2.0 * EXT), 0.0, 1.0);
        vec4 texS = texture2D(inputTex, texUV);
        float hn = clamp(g_h / max(waveHeight, 0.01) * 0.5 + 0.5, 0.0, 1.0);

        vec3 albedo = texS.a > 0.01
            ? texS.rgb
            : mix(vec3(0.04, 0.03, 0.06), baseColor.rgb, hn * hn);

        col = albedo * (diff + 0.08);
        col += spec * mix(vec3(1.0), albedo, 0.3) * 0.5;
        col += fres * baseColor.rgb * 0.12;
        col += albedo * (hn * hn * 0.2 + audioBass * 0.08);

        // Fog
        float fog = 1.0 - exp(-t * fogAmount * 0.07);
        col = mix(col, vec3(0.01, 0.01, 0.015), fog);
        alpha = 1.0;
    }

    // Glow
    col += baseColor.rgb * glow * 0.015;

    // Tone map
    col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);

    if (!hit && transparentBg) {
        alpha = glow > 0.1 ? clamp(glow * 0.03, 0.0, 0.3) : 0.0;
    } else if (!hit) {
        col = max(col, vec3(0.01, 0.01, 0.02));
        alpha = 1.0;
    }

    gl_FragColor = vec4(col, alpha);
}
