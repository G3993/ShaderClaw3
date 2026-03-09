/*{
    "DESCRIPTION": "3D image mosaic — feed images float as cards in a particle field with glow, depth of field, and audio reactivity",
    "CATEGORIES": ["Generator", "Data"],
    "INPUTS": [
        { "NAME": "feedImage0",  "TYPE": "image" },
        { "NAME": "feedImage1",  "TYPE": "image" },
        { "NAME": "feedImage2",  "TYPE": "image" },
        { "NAME": "feedImage3",  "TYPE": "image" },
        { "NAME": "feedImage4",  "TYPE": "image" },
        { "NAME": "feedImage5",  "TYPE": "image" },
        { "NAME": "feedImage6",  "TYPE": "image" },
        { "NAME": "feedImage7",  "TYPE": "image" },
        { "NAME": "feedImage8",  "TYPE": "image" },
        { "NAME": "feedImage9",  "TYPE": "image" },
        { "NAME": "feedImage10", "TYPE": "image" },
        { "NAME": "feedImage11", "TYPE": "image" },
        { "NAME": "feedCount",   "LABEL": "Feed Count",   "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 16.0 },
        { "NAME": "cardSize",    "LABEL": "Card Size",    "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.05, "MAX": 1.0 },
        { "NAME": "spread",      "LABEL": "Spread",       "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.5,  "MAX": 8.0 },
        { "NAME": "rotSpeed",    "LABEL": "Orbit Speed",  "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "camDist",     "LABEL": "Camera Dist",  "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0,  "MAX": 12.0 },
        { "NAME": "dofStrength", "LABEL": "DOF",          "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "glowAmt",     "LABEL": "Glow",         "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "bgColor",     "LABEL": "Background",   "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.05, 1.0] }
    ]
}*/

// Hash functions
float hash(float p) { return fract(sin(p * 127.1) * 43758.5453); }
vec3 hash3(float p) { return fract(sin(vec3(p, p+1.0, p+2.0) * vec3(127.1, 269.5, 419.2)) * 43758.5453); }

// Card position: deterministic orbit per card index
vec3 cardPos(float idx, float t) {
    vec3 h = hash3(idx * 17.3);
    float orbit = h.x * 6.2831 + t * rotSpeed * (0.5 + h.y);
    float height = (h.z - 0.5) * spread * 0.6;
    float radius = spread * (0.4 + h.x * 0.6);
    return vec3(cos(orbit) * radius, height + sin(t * 0.3 + idx) * 0.3, sin(orbit) * radius);
}

// Card rotation
mat3 cardRot(float idx, float t) {
    float a = t * 0.5 + hash(idx * 31.7) * 6.28;
    float b = hash(idx * 47.1) * 0.5 - 0.25;
    float ca = cos(a), sa = sin(a);
    float cb = cos(b), sb = sin(b);
    return mat3(ca, 0, sa, sa*sb, cb, -ca*sb, -sa*cb, sb, ca*cb);
}

// Sample feed image by index (unrolled for WebGL1 compatibility)
vec4 sampleFeed(int idx, vec2 uv) {
    if (idx == 0)  return texture2D(feedImage0, uv);
    if (idx == 1)  return texture2D(feedImage1, uv);
    if (idx == 2)  return texture2D(feedImage2, uv);
    if (idx == 3)  return texture2D(feedImage3, uv);
    if (idx == 4)  return texture2D(feedImage4, uv);
    if (idx == 5)  return texture2D(feedImage5, uv);
    if (idx == 6)  return texture2D(feedImage6, uv);
    if (idx == 7)  return texture2D(feedImage7, uv);
    if (idx == 8)  return texture2D(feedImage8, uv);
    if (idx == 9)  return texture2D(feedImage9, uv);
    if (idx == 10) return texture2D(feedImage10, uv);
    if (idx == 11) return texture2D(feedImage11, uv);
    return vec4(0.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 ndc = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME;

    // Orbiting camera
    float ca = t * rotSpeed * 0.5;
    vec3 ro = vec3(cos(ca) * camDist, 1.5, sin(ca) * camDist);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0, 1, 0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd * 2.0 + right * ndc.x + up * ndc.y);

    int count = int(max(1.0, feedCount));
    vec3 col = bgColor.rgb;
    float closestZ = 100.0;

    // Raymarch against card planes
    for (int i = 0; i < 12; i++) {
        if (i >= count) break;
        float fi = float(i);

        vec3 cpos = cardPos(fi, t);
        mat3 crot = cardRot(fi, t);

        // Card normal (facing camera roughly)
        vec3 normal = crot * vec3(0, 0, 1);

        // Ray-plane intersection
        float denom = dot(rd, normal);
        if (abs(denom) < 0.001) continue;
        float tHit = dot(cpos - ro, normal) / denom;
        if (tHit < 0.1 || tHit > 30.0) continue;

        vec3 hitPos = ro + rd * tHit;
        vec3 local = transpose(crot) * (hitPos - cpos);

        // Card bounds check
        float halfSize = cardSize * 0.5;
        float aspect = 0.75; // 4:3
        if (abs(local.x) < halfSize && abs(local.y) < halfSize * aspect) {
            // UV on card face
            vec2 cardUV = vec2(
                local.x / halfSize * 0.5 + 0.5,
                local.y / (halfSize * aspect) * 0.5 + 0.5
            );
            cardUV.y = 1.0 - cardUV.y; // flip Y

            vec4 texCol = sampleFeed(i, cardUV);

            // Depth-based DOF blur (simple blend toward bg)
            float depth = length(hitPos - ro);
            float focusDist = camDist * 0.7;
            float blur = abs(depth - focusDist) / camDist * dofStrength;
            blur = clamp(blur, 0.0, 0.8);
            texCol.rgb = mix(texCol.rgb, bgColor.rgb, blur * 0.6);

            // Edge glow
            vec2 edgeDist = abs(vec2(local.x / halfSize, local.y / (halfSize * aspect)));
            float edge = max(edgeDist.x, edgeDist.y);
            float edgeGlow = smoothstep(0.7, 1.0, edge) * glowAmt;
            vec3 glowCol = vec3(0.91, 0.25, 0.34); // ShaderClaw red
            texCol.rgb += glowCol * edgeGlow;

            // Depth sort — only draw if closer
            if (tHit < closestZ) {
                closestZ = tHit;
                // Slight transparency at edges
                float alpha = smoothstep(1.0, 0.85, edge);
                col = mix(col, texCol.rgb, alpha);
            }
        }
    }

    // Floating particle dust
    for (int p = 0; p < 30; p++) {
        vec3 pp = hash3(float(p) * 7.13) * spread * 2.0 - spread;
        pp.y += sin(t * 0.5 + float(p)) * 0.5;
        pp.xz += vec2(cos(t * 0.1 + float(p) * 0.7), sin(t * 0.1 + float(p) * 0.9)) * 0.3;
        vec3 toP = pp - ro;
        float projDist = dot(toP, rd);
        if (projDist < 0.0) continue;
        vec3 closest = ro + rd * projDist;
        float dist = length(closest - pp);
        float brightness = 0.003 / (dist * dist + 0.01);
        brightness *= smoothstep(15.0, 2.0, projDist);
        col += vec3(0.5, 0.6, 0.8) * brightness;
    }

    // Audio reactivity (if available)
    float bassGlow = audioBass * glowAmt * 0.3;
    col += vec3(0.91, 0.25, 0.34) * bassGlow;

    // Vignette
    float vig = 1.0 - dot((uv - 0.5) * 1.3, (uv - 0.5) * 1.3);
    col *= clamp(vig, 0.0, 1.0);

    // Tone map
    col = col / (col + 1.0);
    col = pow(col, vec3(0.9));

    gl_FragColor = vec4(col, 1.0);
}
