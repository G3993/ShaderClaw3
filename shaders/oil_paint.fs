/*{
    "DESCRIPTION": "Impressionist Sea — raymarched choppy ocean surface with Monet-style sunset lighting. Standalone HDR generator.",
    "CREDIT": "auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {"NAME":"waveHeight","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":2.0,"LABEL":"Wave Height"},
        {"NAME":"waveSpeed","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":2.0,"LABEL":"Wave Speed"},
        {"NAME":"sunAngle","TYPE":"float","DEFAULT":0.3,"MIN":0.0,"MAX":1.0,"LABEL":"Sun Angle"},
        {"NAME":"foamPeak","TYPE":"float","DEFAULT":3.0,"MIN":1.0,"MAX":5.0,"LABEL":"HDR Peak"},
        {"NAME":"audioMod","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0,"LABEL":"Audio Mod"},
        {"NAME":"camTilt","TYPE":"float","DEFAULT":0.6,"MIN":0.1,"MAX":1.0,"LABEL":"Camera Tilt"}
    ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float noise(vec2 p) {
    vec2 i = floor(p); vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i), hash(i + vec2(1,0)), f.x),
               mix(hash(i + vec2(0,1)), hash(i + vec2(1,1)), f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0; float a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p = p * 2.1 + vec2(1.7, 9.2);
        a *= 0.5;
    }
    return v;
}

float oceanHeight(vec2 p, float t) {
    float h = fbm(p * 1.2 + vec2(t * 0.3, t * 0.17));
    h += fbm(p * 2.3 - vec2(t * 0.23, t * 0.11)) * 0.4;
    return h;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME * waveSpeed;
    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioMod;
    float wh = waveHeight * audio;

    vec3 ro = vec3(0.0, 5.0, 4.0);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up * camTilt);

    vec3 col = vec3(0.0, 0.02, 0.08);

    if (rd.y < -0.01) {
        float rayT = (0.0 - ro.y) / rd.y;
        vec3 p = ro + rd * rayT;

        float h = oceanHeight(p.xz, t) * wh;
        vec3 pos = vec3(p.x, h, p.z);

        float eps = 0.05;
        float hx = oceanHeight(p.xz + vec2(eps, 0.0), t) * wh;
        float hz = oceanHeight(p.xz + vec2(0.0, eps), t) * wh;
        vec3 normal = normalize(vec3(h - hx, eps, h - hz));

        float sa = sunAngle * 3.14159;
        vec3 sunDir = normalize(vec3(cos(sa), 0.6, sin(sa)));
        float diff = max(0.0, dot(normal, sunDir));
        vec3 refl = reflect(-sunDir, normal);
        float spec = pow(max(0.0, dot(refl, -rd)), 64.0);

        vec3 deepBlue = vec3(0.0, 0.1, 0.4);
        vec3 waveBlue = vec3(0.0, 0.4, 0.9);
        vec3 sunGold = vec3(2.5, 1.6, 0.2);
        vec3 foam = vec3(foamPeak, foamPeak, foamPeak * 0.9);

        float depth = clamp(h / (wh + 0.001), 0.0, 1.0);
        vec3 base = mix(deepBlue, waveBlue, depth);
        col = base * (0.2 + diff * 0.8) + sunGold * diff * 1.0 + foam * spec;

        float fogT = clamp(rayT / 30.0, 0.0, 1.0);
        col = mix(col, vec3(0.0, 0.02, 0.08), fogT);
    }

    gl_FragColor = vec4(col, 1.0);
}
