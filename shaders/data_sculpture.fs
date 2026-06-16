/*{
  "DESCRIPTION": "Data Sculpture — thousands of cubes form a fluid 3D landscape from input imagery",
  "CATEGORIES": ["Radiant"],
  "INPUTS": [
    { "NAME": "baseColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "gridDensity", "LABEL": "Density", "TYPE": "float", "DEFAULT": 14.0, "MIN": 4.0, "MAX": 28.0 },
    { "NAME": "shapeType", "LABEL": "Shape", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Cube", "Sphere"] },
    { "NAME": "cubeScale", "LABEL": "Size", "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.15, "MAX": 0.95 },
    { "NAME": "waveHeight", "LABEL": "Wave Height", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "camHeight", "LABEL": "Camera Height", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fogAmount", "LABEL": "Atmosphere", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "baseGrid", "LABEL": "Base Grid", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "baseGap", "LABEL": "Base Gap", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.3, "MAX": 3.0 },
    { "NAME": "baseGlow", "LABEL": "Base Glow", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "basePulse", "LABEL": "Base Pulse", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

#define STEPS 48
#define HIT 0.003
#define FAR 20.0
#define PI 3.14159265
#define EXT 7.0

// ---- Fast hash ----
float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// ---- Cheap animated height from 2D hashes (replaces expensive vnoise) ----
float cheapHeight(vec2 id, vec2 disp, float t, float fs) {
    // Animated height using hash + sin combinations (no 3D noise)
    float h1 = hash(id);
    float h2 = hash(id + 73.0);
    float h3 = hash(id * 0.7 + 31.0);
    float anim = sin(t * fs * 0.6 + h1 * 6.28) * 0.4
               + sin(t * fs * 0.3 + h2 * 6.28 + disp.x * 2.0) * 0.35
               + cos(t * fs * 0.45 + h3 * 6.28 + disp.y * 1.5) * 0.25;
    return anim;
}

// ---- SDF ----
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ---- Globals ----
vec2 g_cell;
float g_h;
float g_isBase;

// Compute cell data: returns height, writes disp to out param
float cellHeight(vec2 id, float sp, out vec2 disp) {
    vec2 center = (id + 0.5) * sp;
    float h1 = hash(id);
    float h2 = hash(id + 73.0);
    float phase = TIME * flowSpeed * 0.4 + h1 * 6.28;
    vec2 flow = vec2(sin(phase + h2 * 3.0), cos(phase * 0.7 + h1 * 5.0));
    disp = center + flow * sp * 0.25;

    float n = cheapHeight(id, disp, TIME, flowSpeed);

    vec2 texUV = clamp((center + EXT) / (2.0 * EXT), 0.0, 1.0);
    vec4 tex = texture2D(inputTex, texUV);

    float ht;
    if (tex.a > 0.01) {
        float lum = dot(tex.rgb, vec3(0.299, 0.587, 0.114));
        ht = lum * 0.65 + n * 0.35;
    } else {
        ht = n;
    }

    float r = length(center) / EXT;
    ht *= smoothstep(1.0, 0.85, r);
    ht += audioBass * 0.12 * (1.0 - r);
    ht *= waveHeight;
    return ht;
}

// ---- Scene: 2x2 neighbor for raised, single cell for base ----
float scene(vec3 p) {
    float sp = 1.0 / gridDensity;
    float ch = sp * 0.5 * cubeScale;

    // Bounds
    if (abs(p.x) > EXT + sp * 2.0 || abs(p.z) > EXT + sp * 2.0)
        return max(abs(p.x) - EXT, abs(p.z) - EXT);
    float maxH = waveHeight + 1.5;
    if (p.y > maxH) return p.y - maxH + 0.1;
    float minH = baseGrid ? baseGap + 1.5 : maxH;
    if (p.y < -minH) return -p.y - minH + 0.1;

    vec2 cellPos = p.xz / sp;
    vec2 baseId = floor(cellPos);
    // 2x2 quadrant: pick the neighbor direction based on fractional position
    vec2 frac = fract(cellPos);
    vec2 offset = step(0.5, frac) * 2.0 - 1.0; // -1 or +1

    float best = 999.0;
    vec2 bestCenter = vec2(0.0);
    float bestH = 0.0;
    float bestIsBase = 0.0;
    bool isSphere = shapeType > 0.5;

    // Check 2x2 raised cells
    for (int i = 0; i < 4; i++) {
        vec2 id = baseId;
        if (i == 1) id.x += offset.x;
        else if (i == 2) id.y += offset.y;
        else if (i == 3) id += offset;

        vec2 center = (id + 0.5) * sp;
        // Quick reject: if xz distance to center is too far, skip
        vec2 xzDiff = p.xz - center;
        if (abs(xzDiff.x) > sp * 1.5 || abs(xzDiff.y) > sp * 1.5) continue;
        if (length(center) > EXT + sp) continue;

        float h1 = hash(id);
        float h2 = hash(id + 73.0);

        vec2 disp;
        float ht = cellHeight(id, sp, disp);

        float vScale = 0.6 + h1 * 0.8;
        vec3 cubePos = vec3(disp.x, ht, disp.y);
        vec3 q = p - cubePos;

        float rot = h2 * 0.2;
        float cr = cos(rot), sr = sin(rot);
        q.xz = vec2(cr * q.x - sr * q.z, sr * q.x + cr * q.z);

        float d;
        if (isSphere) {
            d = length(q) - ch * vScale;
        } else {
            d = sdBox(q, vec3(ch, ch * vScale, ch));
        }

        if (d < best) {
            best = d;
            bestCenter = center;
            bestH = ht;
            bestIsBase = 0.0;
        }
    }

    // Base layer: single cell (no displacement = no neighbor issues)
    if (baseGrid) {
        vec2 id = baseId;
        vec2 center = (id + 0.5) * sp;
        if (length(center) <= EXT + sp) {
            float h1 = hash(id);
            float h2 = hash(id + 73.0);
            float pulse = sin(TIME * 1.5 + h1 * 6.28 + h2 * 3.14) * basePulse * 0.15;
            float baseY = -baseGap + pulse;
            float baseScale = 0.35 + h1 * 0.15;

            vec3 basePos = vec3(center.x, baseY, center.y);
            vec3 qb = p - basePos;

            float d;
            if (isSphere) {
                d = length(qb) - ch * baseScale;
            } else {
                d = sdBox(qb, vec3(ch, ch * baseScale, ch));
            }

            if (d < best) {
                best = d;
                bestCenter = center;
                bestIsBase = 1.0;
                // Store raised height at this cell for base glow effect
                vec2 dummy;
                bestH = cellHeight(id, sp, dummy);
            }
        }
    }

    g_cell = bestCenter;
    g_h = bestH;
    g_isBase = bestIsBase;

    return best;
}

// ---- Normal (tetrahedron method, wider epsilon for speed) ----
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.004, -0.004);
    return normalize(
        e.xyy * scene(p + e.xyy) +
        e.yyx * scene(p + e.yyx) +
        e.yxy * scene(p + e.yxy) +
        e.xxx * scene(p + e.xxx)
    );
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Camera
    float angle = TIME * 0.1;
    float elevAngle = camHeight * PI * 0.48;
    float camDist = 6.0;
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

        if (g_isBase > 0.5) {
            // ---- Base layer effects ----
            float h1 = hash(floor(g_cell * gridDensity));

            vec3 baseAlbedo = albedo * 0.25;

            // Ripple pulse
            float ripple = sin(length(g_cell) * 4.0 - TIME * 2.0 + h1 * 6.28);
            ripple = ripple * 0.5 + 0.5;
            baseAlbedo = mix(baseAlbedo, baseColor.rgb * 0.3, ripple * basePulse);

            // Height-reactive glow
            float heightGlow = smoothstep(0.0, waveHeight * 0.8, abs(g_h)) * baseGlow;
            baseAlbedo += baseColor.rgb * heightGlow * 0.4;

            // Audio shimmer
            baseAlbedo += baseColor.rgb * audioBass * 0.15;

            col = baseAlbedo * (diff * 0.7 + 0.15);
            col += spec * baseColor.rgb * 0.15;
            col += fres * baseColor.rgb * 0.08;
            col += baseColor.rgb * pow(fres, 2.0) * baseGlow * 0.5;

        } else {
            // ---- Raised layer ----
            // Two-light setup for real volumetric depth.
            vec3 ld1 = normalize(vec3(-0.45, 0.85, 0.30));   // primary key
            vec3 ld2 = normalize(vec3( 0.55, 0.40, -0.65));  // fill
            float diff1 = clamp(dot(n, ld1), 0.0, 1.0);
            float diff2 = clamp(dot(n, ld2), 0.0, 1.0) * 0.4;

            // Ambient occlusion proxy — taller (closer to top) cubes get
            // more light, lower cubes are AO-darkened.
            float ao = clamp(0.4 + hn * 0.7, 0.3, 1.0);

            // Cast-shadow approximation: short march toward primary light.
            // If we hit something within 0.6, this cube is shadowed.
            float sh = 1.0;
            {
                float st = 0.05;
                for (int s = 0; s < 6; s++) {
                    vec3 sp = p + ld1 * st;
                    float sd = scene(sp);
                    if (sd < 0.005) { sh = 0.35; break; }
                    st += sd;
                    if (st > 0.6) break;
                }
            }

            col = albedo * (diff1 * sh + diff2 + 0.06) * ao;
            col += spec * mix(vec3(1.0), albedo, 0.3) * 0.7 * sh;
            col += fres * baseColor.rgb * 0.18;
            col += albedo * (hn * hn * 0.18 + audioBass * 0.08);
        }

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

    // Surprise: every ~32s a single horizontal scan-line cuts across,
    // marking the moment a measurement is taken — the data is observed.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 32.0);
        float _y  = (_ph - 0.04) / 0.30;
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.34, 0.18, _ph);
        float _line = exp(-pow((_suv.y - _y) * 200.0, 2.0));
        col += vec3(0.4, 1.0, 0.8) * _line * _f;
    }

    gl_FragColor = vec4(col, alpha);
}
