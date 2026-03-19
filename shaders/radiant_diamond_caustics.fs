/*{
  "CATEGORIES": [
    "Radiant",
    "Geometric",
    "Light"
  ],
  "DESCRIPTION": "Diamond light caustics with prismatic fire, brilliant-cut facets, chromatic dispersion, scintillation sparkles, and star bursts. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    {
      "NAME": "rotationSpeed",
      "LABEL": "Rotation Speed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 2,
      "DEFAULT": 0.5
    },
    {
      "NAME": "brilliance",
      "LABEL": "Brilliance",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [0.91, 0.25, 0.34, 1.0]
    },
    {
      "NAME": "texture",
      "LABEL": "Texture",
      "TYPE": "image"
    }
  ]
}*/

// Diamond Caustics - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359
#define TAU 6.28318530718

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float hash(vec2 p) {
    p = fract(p * vec2(443.897, 441.423));
    p += dot(p, p.yx + 19.19);
    return fract((p.x + p.y) * p.x);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

vec4 hexGrid(vec2 p) {
    vec2 s = vec2(1.0, 1.7320508);
    vec2 h = s * 0.5;
    vec2 a = mod(p, s) - h;
    vec2 b = mod(p - h, s) - h;
    vec2 ga = dot(a, a) < dot(b, b) ? a : b;
    vec2 cellId = p - ga;
    vec2 ab = abs(ga);
    float hexDist = max(dot(ab, normalize(vec2(1.0, 1.7320508))), ab.x);
    return vec4(ga, hexDist, hash(cellId));
}

float triGrid(vec2 p) {
    vec2 q = vec2(p.x + p.y * 0.57735, p.y * 1.1547);
    vec2 f = fract(q);
    float edge = min(min(f.x, f.y), abs(1.0 - f.x - f.y));
    return edge;
}

float brilliantFacets(vec2 uv, float time) {
    float r = length(uv);
    float a = atan(uv.y, uv.x);
    float table = smoothstep(0.12, 0.11, r);
    float starAngle = mod(a + PI / 8.0, PI / 4.0) - PI / 8.0;
    float starR = r - 0.18;
    float star = abs(starAngle) * 8.0 / PI;
    float starEdge = smoothstep(0.01, 0.0, abs(starR) - star * 0.08);
    float kiteAngle = mod(a, PI / 4.0) - PI / 8.0;
    float kiteEdge = smoothstep(0.01, 0.0, abs(kiteAngle) * r * 6.0 - 0.02);
    float kiteRing = smoothstep(0.01, 0.0, abs(r - 0.28) - 0.005);
    float ugAngle = mod(a, PI / 8.0) - PI / 16.0;
    float ugEdge = smoothstep(0.01, 0.0, abs(ugAngle) * r * 10.0 - 0.02);
    float ugRing = smoothstep(0.01, 0.0, abs(r - 0.38) - 0.005);
    float girdle = smoothstep(0.01, 0.0, abs(r - 0.45) - 0.008);
    float edges = max(max(starEdge, kiteEdge), max(ugEdge, girdle));
    edges = max(edges, kiteRing);
    edges = max(edges, ugRing);
    float radial16 = mod(a, PI / 8.0) - PI / 16.0;
    float radLine16 = smoothstep(0.008, 0.0, abs(radial16) * r * 4.0 - 0.004);
    radLine16 *= step(0.12, r) * step(r, 0.46);
    float radial8 = mod(a + PI / 8.0, PI / 4.0) - PI / 8.0;
    float radLine8 = smoothstep(0.006, 0.0, abs(radial8) * r * 5.0 - 0.003);
    radLine8 *= step(0.12, r) * step(r, 0.46);
    edges = max(edges, max(radLine16 * 0.5, radLine8));
    return edges;
}

vec3 spectral(float t) {
    t = fract(t);
    vec3 c = vec3(0.0);
    c.r = smoothstep(0.0, 0.15, t) - smoothstep(0.35, 0.5, t);
    c.r += smoothstep(0.8, 0.95, t);
    c.g = smoothstep(0.15, 0.35, t) - smoothstep(0.55, 0.75, t);
    c.b = smoothstep(0.4, 0.6, t) - smoothstep(0.75, 0.95, t);
    c = pow(c, vec3(0.6));
    return c * 3.0;
}

float diamondCaustic(vec2 uv, float time, float scale, float rotation) {
    vec2 p = rot(rotation) * uv * scale;
    vec4 hex = hexGrid(p);
    float cellRand = hex.w;
    float facetAngle = cellRand * TAU + time * 0.3;
    vec2 refract = vec2(cos(facetAngle), sin(facetAngle)) * 0.3;
    vec2 displaced = p + refract * (1.0 + 0.5 * sin(time * 0.7 + cellRand * 10.0));
    vec4 hex2 = hexGrid(displaced * 1.5);
    float edgeDist = hex.z;
    float caustic = 1.0 - smoothstep(0.0, 0.4, edgeDist);
    float fold = 1.0 - smoothstep(0.0, 0.25, hex2.z);
    fold = pow(fold, 2.0);
    float interference = sin(displaced.x * 8.0 + time * 0.5) * sin(displaced.y * 8.0 - time * 0.4);
    interference = pow(abs(interference), 1.5) * 0.5;
    return caustic * 0.3 + fold * 0.5 + interference * 0.2;
}

float scintillation(vec2 uv, float time) {
    float sparkle = 0.0;
    for (float i = 0.0; i < 3.0; i++) {
        float sc = 5.0 + i * 4.0;
        vec2 grid = floor(uv * sc);
        vec2 f = fract(uv * sc) - 0.5;
        float h = hash(grid + i * 100.0);
        float phase = h * TAU + time * (1.5 + h * 2.0);
        float flash = pow(max(sin(phase), 0.0), 48.0);
        float dist = length(f);
        float point = smoothstep(0.15, 0.0, dist);
        sparkle += flash * point * (1.0 - i * 0.25);
    }
    return sparkle;
}

float starBurst(vec2 uv, float time) {
    float r = length(uv);
    float a = atan(uv.y, uv.x);
    float star4 = pow(abs(cos(a * 2.0)), 64.0) / (r * 20.0 + 1.0);
    float star6 = pow(abs(cos(a * 3.0)), 64.0) / (r * 25.0 + 1.0);
    return (star4 + star6 * 0.5) * smoothstep(0.5, 0.0, r);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
    float t = TIME;
    float rotSpeed = rotationSpeed;

    // Audio: high frequencies make sparkles more brilliant
    float audioBrill = brilliance * (1.0 + audioHigh * 0.8);

    float globalRot = t * rotSpeed * 0.1;
    vec2 uvRot = rot(globalRot) * uv;

    float facetPattern = brilliantFacets(uvRot * 2.2, t);

    float tri1 = triGrid(rot(t * 0.05) * uv * 8.0);
    float tri2 = triGrid(rot(-t * 0.07 + 1.0) * uv * 12.0);
    float triEdges = smoothstep(0.04, 0.0, tri1) * 0.3 + smoothstep(0.03, 0.0, tri2) * 0.15;

    vec4 mainHex = hexGrid(rot(t * 0.06) * uv * 6.0);
    float hexEdges = smoothstep(0.44, 0.40, mainHex.z) - smoothstep(0.40, 0.36, mainHex.z);
    float hexOutline = smoothstep(0.45, 0.43, mainHex.z) - smoothstep(0.43, 0.41, mainHex.z);

    float c1 = diamondCaustic(uv, t, 3.0, t * rotSpeed * 0.15);
    float c2 = diamondCaustic(uv, t * 1.1 + 10.0, 5.0, -t * rotSpeed * 0.12 + PI * 0.3);
    float c3 = diamondCaustic(uv, t * 0.9 + 20.0, 8.0, t * rotSpeed * 0.08 + PI * 0.7);

    float caustics = c1 * 0.5 + c2 * 0.3 + c3 * 0.2;
    caustics = pow(caustics, 1.3) * 3.5;

    float dispersion = 0.018 * (1.0 + caustics * 0.3);
    float dispAngle = t * rotSpeed * 0.3 + atan(uv.y, uv.x) * 0.5;

    vec2 rOff = vec2(cos(dispAngle), sin(dispAngle)) * dispersion;
    float cR = diamondCaustic(uv + rOff, t, 3.0, t * rotSpeed * 0.15);
    cR += diamondCaustic(uv + rOff, t * 1.1 + 10.0, 5.0, -t * rotSpeed * 0.12 + PI * 0.3) * 0.6;

    vec2 gOff = vec2(cos(dispAngle + TAU / 3.0), sin(dispAngle + TAU / 3.0)) * dispersion;
    float cG = diamondCaustic(uv + gOff, t, 3.0, t * rotSpeed * 0.15);
    cG += diamondCaustic(uv + gOff, t * 1.1 + 10.0, 5.0, -t * rotSpeed * 0.12 + PI * 0.3) * 0.6;

    vec2 bOff = vec2(cos(dispAngle + TAU * 2.0 / 3.0), sin(dispAngle + TAU * 2.0 / 3.0)) * dispersion;
    float cB = diamondCaustic(uv + bOff, t, 3.0, t * rotSpeed * 0.15);
    cB += diamondCaustic(uv + bOff, t * 1.1 + 10.0, 5.0, -t * rotSpeed * 0.12 + PI * 0.3) * 0.6;

    vec3 chromatic = vec3(cR, cG, cB);
    float chrDiff = abs(cR - cG) + abs(cG - cB) + abs(cB - cR);

    float specPhase = atan(cR - cG, cG - cB) / TAU + 0.5;
    specPhase += t * 0.03 + length(uv) * 0.5;
    vec3 fireColor = spectral(specPhase);

    float specPhase2 = noise(uvRot * 3.0 + t * 0.2) + t * 0.05;
    vec3 fireColor2 = spectral(specPhase2);

    float sparkle = scintillation(uvRot, t * rotSpeed);

    float starTotal = 0.0;
    for (float i = 0.0; i < 4.0; i++) {
        float sc = 4.0 + i * 3.0;
        vec2 grid = floor(rot(i * 0.7 + t * 0.03) * uv * sc);
        vec2 center = (grid + 0.5) / sc;
        center = rot(-i * 0.7 - t * 0.03) * center;
        float h = hash(grid + i * 77.0);
        float phase = h * TAU + t * (1.0 + h);
        float flash = pow(max(sin(phase), 0.0), 24.0);
        float star = starBurst((uv - center) * sc * 0.8, t) * flash;
        starTotal += star * 0.35;
    }

    vec3 col = vec3(0.0);

    vec3 whiteLight = vec3(0.92, 0.95, 1.0);
    col += whiteLight * caustics * 1.2;
    col += chromatic * 0.5 * audioBrill;

    float fireMask = smoothstep(0.03, 0.25, chrDiff) * caustics;
    col += fireColor * fireMask * 2.5 * audioBrill;
    col += fireColor2 * caustics * 0.6 * audioBrill;

    vec3 facetColor = vec3(0.7, 0.75, 0.85);
    float structureMask = smoothstep(0.1, 0.5, caustics);
    col += facetColor * facetPattern * 0.04 * structureMask;
    col += facetColor * triEdges * hexEdges * 0.03 * (0.3 + structureMask * 0.7);
    col += vec3(0.8, 0.85, 0.95) * hexOutline * 0.025 * (0.2 + structureMask * 0.8);

    vec3 sparkleColor = vec3(1.0, 0.98, 0.95);
    col += sparkleColor * sparkle * 3.5 * audioBrill;

    float starSpec = fract(t * 0.15 + starTotal * 2.0);
    col += mix(vec3(1.0, 0.97, 0.92), spectral(starSpec), 0.5) * starTotal * 2.5 * audioBrill;

    // ShaderClaw accent: warm red tint in the caustics
    vec3 accent = vec3(0.91, 0.25, 0.34);
    col += accent * caustics * 0.04;

    float centerGlow = smoothstep(0.6, 0.0, length(uv));
    col *= 0.8 + centerGlow * 0.8;

    float flashPhase = sin(t * rotSpeed * 1.7 + uvRot.x * 4.0) *
                       cos(t * rotSpeed * 2.3 + uvRot.y * 3.0);
    float intenseFlash = pow(max(flashPhase, 0.0), 10.0) * 3.5;
    float flashSpec = fract(t * 0.2 + atan(uvRot.y, uvRot.x) / TAU);
    col += spectral(flashSpec) * intenseFlash * centerGlow * audioBrill;

    float sweep = sin(uv.x * 3.0 + uv.y * 2.0 + t * rotSpeed * 0.5);
    sweep = pow(max(sweep, 0.0), 4.0) * 0.35;
    col += spectral(t * 0.1 + uv.x * 0.3) * sweep * audioBrill;

    float vig = 1.0 - dot(uv, uv) * 0.35;
    vig = max(vig, 0.0);
    col *= vig;

    col *= 2.2;
    col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
    col = pow(col, vec3(0.97, 0.98, 1.03));
    col += vec3(0.008, 0.008, 0.012);

    col *= baseColor.rgb;
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
    vec4 texSample = IMG_NORM_PIXEL(texture, texUV);
    col = mix(col, col * texSample.rgb, texSample.a * 0.5);

    gl_FragColor = vec4(col, 1.0);
}
