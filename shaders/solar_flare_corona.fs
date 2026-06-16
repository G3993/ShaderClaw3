/*{
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "DESCRIPTION": "Looking at the sun. Convective surface roils, magnetic ropes arc off the limb in fiery loops, the corona flares with the music. SDO/SOHO + Eliasson Weather Project.",
  "INPUTS": [
    {"NAME":"discRadius","TYPE":"float","MIN":0.2,"MAX":0.6,"DEFAULT":0.35},
    {"NAME":"surfaceScale","TYPE":"float","MIN":2.0,"MAX":12.0,"DEFAULT":5.0},
    {"NAME":"flow","TYPE":"float","MIN":0.0,"MAX":1.5,"DEFAULT":0.3},
    {"NAME":"loopCount","TYPE":"float","MIN":0.0,"MAX":12.0,"DEFAULT":5.0},
    {"NAME":"coronaReach","TYPE":"float","MIN":0.1,"MAX":1.2,"DEFAULT":0.45},
    {"NAME":"flareIntensity","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"texMix","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.0},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash(i), b = hash(i + vec2(1, 0));
    float c = hash(i + vec2(0, 1)), d = hash(i + vec2(1, 1));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) { v += a * vnoise(p); p *= 2.07; a *= 0.5; }
    return v;
}

// 5-stop fire palette: black → deep red → orange → yellow → white. Mapping
// via stop-by-stop mix avoids the plasticky look of naive 2-stop gradients.
vec3 firePalette(float t) {
    t = clamp(t, 0.0, 1.0);
    vec3 c0 = vec3(0.02, 0.0, 0.0);
    vec3 c1 = vec3(0.4, 0.05, 0.02);
    vec3 c2 = vec3(0.95, 0.32, 0.05);
    vec3 c3 = vec3(1.0, 0.85, 0.3);
    vec3 c4 = vec3(1.0, 1.0, 0.95);
    if (t < 0.25) return mix(c0, c1, t / 0.25);
    if (t < 0.5)  return mix(c1, c2, (t - 0.25) / 0.25);
    if (t < 0.75) return mix(c2, c3, (t - 0.5) / 0.25);
    return mix(c3, c4, (t - 0.75) / 0.25);
}

// Curl-noise approximation: take perpendicular of the noise gradient. Sells
// the swirling photospheric convection without doing real fluid sim.
vec2 curl(vec2 p, float t) {
    float e = 0.01;
    float n1 = fbm(p + vec2(0, e) + t);
    float n2 = fbm(p - vec2(0, e) + t);
    float n3 = fbm(p + vec2(e, 0) + t);
    float n4 = fbm(p - vec2(e, 0) + t);
    return vec2((n1 - n2), -(n3 - n4)) / (2.0 * e);
}

// Magnetic loop: half-ellipse arc between two points on the limb. Render
// as smoothed thickness, additive glow. Footpoint pairs are hash-driven.
float magneticLoop(vec2 p, int idx, float radius, float t) {
    float fi = float(idx);
    float a1 = hash(vec2(fi, 1.7)) * 6.2832;
    float a2 = a1 + 0.4 + hash(vec2(fi, 9.3)) * 0.6;
    vec2 fp1 = vec2(cos(a1), sin(a1)) * radius;
    vec2 fp2 = vec2(cos(a2), sin(a2)) * radius;
    vec2 mid = (fp1 + fp2) * 0.5;
    vec2 outward = normalize(mid);
    float h = 0.04 + 0.06 * (0.5 + 0.5 * sin(t * 2.0 + fi));
    vec2 apex = mid + outward * h;
    // Quadratic Bezier-ish distance
    float minD = 1e9;
    for (int i = 0; i < 12; i++) {
        float u = float(i) / 11.0;
        vec2 bp = mix(mix(fp1, apex, u), mix(apex, fp2, u), u);
        minD = min(minD, length(p - bp));
    }
    return smoothstep(0.012, 0.0, minD);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p  = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float r = length(p);

    // Surface fbm in domain warp by curl — gives convection-cell appearance.
    vec2 cw = curl(p * surfaceScale, TIME * flow * 0.3) * 0.05;
    float surf = fbm(p * surfaceScale + cw + vec2(TIME * flow, 0.0));
    vec3 disc = firePalette(surf * 0.7 + 0.3 + audioBass * 0.25)
              * step(r, discRadius);

    // Limb glow — bright rim where the disc edge is, scaled by audioBass for flares.
    float rim = pow(max(0.0, 1.0 - r / discRadius), 2.0) * step(r, discRadius);
    disc += vec3(1.0, 0.7, 0.3) * rim * (1.0 + audioBass * flareIntensity);

    // Corona — outside the disc, falling off over coronaReach.
    float coronaT = smoothstep(discRadius + coronaReach, discRadius, r);
    float coronaNoise = fbm(p * (4.0 + audioHigh * 8.0) + TIME * 0.5);
    vec3 corona = firePalette(coronaNoise) * coronaT * (0.4 + audioHigh * 1.2);

    // Magnetic loops — additive, count modulated by audioMid.
    int LC = int(clamp(loopCount + audioMid * 4.0, 0.0, 12.0));
    vec3 loops = vec3(0.0);
    for (int i = 0; i < 12; i++) {
        if (i >= LC) break;
        float L = magneticLoop(p, i, discRadius, TIME);
        loops += firePalette(0.7 + hash(vec2(float(i))) * 0.3) * L;
    }

    vec3 col = disc + corona + loops * (0.6 + audioMid);

    // Optional inputTex tint warms photosphere with external content.
    if (IMG_SIZE_inputTex.x > 0.0 && texMix > 0.001) {
        vec3 t = texture(inputTex, uv).rgb;
        col = mix(col, t * col * 1.5, texMix);
    }

    // Global brightness with audioLevel
    col *= 0.7 + audioLevel * 0.5;

    // Surprise: every ~60s a coronal mass ejection — for ~3s a single
    // bright plasma plume erupts asymmetrically from the limb and trails
    // outward, then dissipates.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 60.0);
        float _f  = smoothstep(0.0, 0.06, _ph) * smoothstep(0.30, 0.20, _ph);
        float _angle = floor(TIME / 60.0) * 2.39 + 1.7;
        vec2 _dir = vec2(cos(_angle), sin(_angle));
        float _proj = dot(_suv - 0.5, _dir);
        float _para = abs(dot(_suv - 0.5, vec2(-_dir.y, _dir.x)));
        float _plume = smoothstep(0.45, 0.10, _proj) * smoothstep(0.04, 0.0, _para)
                     * step(0.05, _proj);
        col += vec3(1.0, 0.55, 0.20) * _plume * _f * 1.5;
    }

    gl_FragColor = vec4(col, 1.0);
}
