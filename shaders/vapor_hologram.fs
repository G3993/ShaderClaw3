/*{
  "DESCRIPTION": "Miami Vice Synthwave — 3D raymarched scene: palm tree silhouettes, neon grid ocean, gradient synthwave sky, sun sphere. Hot pink, cyan, magenta.",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "camSpeed",   "LABEL": "Cam Speed",   "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.4 },
    { "NAME": "gridSpeed",  "LABEL": "Grid Speed",  "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "sunSize",    "LABEL": "Sun Size",    "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define MAX_DIST  60.0
#define SURF_DIST 0.01

float hash21(vec2 p) { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float hash11(float n) { return fract(sin(n*12.9898)*43758.5453); }

// Smooth noise for palm frond FBM
float smoothNoise(vec2 p) {
    vec2 i = floor(p), f = fract(p); f = f*f*(3.0-2.0*f);
    return mix(mix(hash21(i),hash21(i+vec2(1,0)),f.x),
               mix(hash21(i+vec2(0,1)),hash21(i+vec2(1,1)),f.x),f.y);
}

// Palm tree SDF (cylinder trunk + hemisphere frond cluster)
float sdPalmTrunk(vec3 p, vec3 base, float height, float radius) {
    vec3 q = p - base;
    // Slightly curved trunk (lean outward)
    float lean = q.y * 0.12;
    q.x -= lean;
    vec2 d = vec2(length(q.xz) - radius, abs(q.y - height*0.5) - height*0.5);
    return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

float sdPalmFrond(vec3 p, vec3 top, float r) {
    return length(p - top) - r;
}

// SDF for two palm trees (left and right silhouettes)
float sdPalms(vec3 p) {
    float h = 4.5;
    float d = MAX_DIST;
    // Left palm at x=-5, right at x=+5
    for (int i = 0; i < 2; i++) {
        float sign = (i == 0) ? -1.0 : 1.0;
        vec3 base = vec3(sign * 5.2, -1.2, 8.0);
        d = min(d, sdPalmTrunk(p, base, h, 0.18 + sign*0.02));
        vec3 top = base + vec3(sign*h*0.12, h, 0.0);
        d = min(d, sdPalmFrond(p, top, 1.2));
        d = min(d, sdPalmFrond(p, top + vec3(sign*0.8, 0.2, 0.0), 0.7));
        d = min(d, sdPalmFrond(p, top + vec3(0.0, 0.5, 0.8), 0.6));
        d = min(d, sdPalmFrond(p, top + vec3(sign*0.3, -0.3, -0.8), 0.5));
    }
    return d;
}

// Ocean plane: y = -1.2 with ripple displacement
float sdOcean(vec3 p) {
    float t = TIME * gridSpeed;
    float wave = sin(p.x * 0.8 + t) * 0.05 + sin(p.z * 0.6 + t * 0.7) * 0.04;
    return p.y + 1.2 - wave;
}

// Sun sphere
float sdSun(vec3 p) {
    return length(p - vec3(0.0, 2.5, 30.0)) - sunSize * 6.0;
}

float map(vec3 p) {
    float d = MAX_DIST;
    d = min(d, sdPalms(p));
    d = min(d, sdOcean(p));
    d = min(d, sdSun(p));
    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.005, 0.0);
    return normalize(vec3(
        map(p+e.xyy)-map(p-e.xyy),
        map(p+e.yxy)-map(p-e.yxy),
        map(p+e.yyx)-map(p-e.yyx)));
}

// Synthwave sky gradient
vec3 skyColor(vec3 rd) {
    float t = clamp(rd.y * 2.0 + 0.5, 0.0, 1.0);
    vec3 skyBot = vec3(0.8, 0.0, 0.5);  // hot pink at horizon
    vec3 skyTop = vec3(0.05, 0.0, 0.25); // deep violet at zenith
    vec3 sky = mix(skyBot, skyTop, t);
    // Horizontal stripe bands (synthwave effect)
    float bands = step(0.5, fract(clamp(rd.y * 8.0 + 0.5, 0.0, 4.0)));
    sky = mix(sky, sky * 0.6, bands * 0.3 * (1.0 - t));
    return sky;
}

// Neon grid on ocean surface
vec3 gridColor(vec2 oceanXZ, float t) {
    float gt = t * gridSpeed;
    vec2 g = vec2(oceanXZ.x * 0.5, oceanXZ.y * 0.5 - gt);
    vec2 gf = fract(g);
    float lineX = smoothstep(0.48, 0.47, abs(gf.x - 0.5));
    float lineZ = smoothstep(0.48, 0.47, abs(gf.y - 0.5));
    float lineMask = max(lineX, lineZ);
    // Perspective fade toward horizon
    float horizFade = clamp(1.0 - oceanXZ.y / 30.0, 0.0, 1.0);
    vec3 gridNeon = vec3(0.0, 0.9, 1.0) * lineMask * horizFade; // cyan grid
    gridNeon += vec3(0.9, 0.0, 0.9) * lineX * lineZ * horizFade; // magenta at intersections
    return gridNeon;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.3 + audioBass*audioReact*0.2;

    // Fixed eye-level camera (slight slow sway)
    float camSway = sin(TIME * camSpeed) * 0.3;
    vec3 ro = vec3(camSway, 1.0, -2.0);
    vec3 ta = vec3(0.0, 1.0, 20.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 1.8*ww);

    // March
    float dist = 0.0;
    float hitT = MAX_DIST;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd*dist;
        float d = map(p);
        if (d < SURF_DIST) { hitT = dist; break; }
        dist += d * 0.8;
        if (dist > MAX_DIST) break;
    }

    // Sky background
    vec3 col = skyColor(rd) * hdrPeak * 0.6 * audio;

    // Sun glow in sky
    float sunD = length(rd - normalize(vec3(0.0, 2.5, 30.0) - ro));
    col += vec3(1.0, 0.3, 0.8) * exp(-sunD*4.0) * hdrPeak * 0.8 * audio;

    if (hitT < MAX_DIST) {
        vec3 p = ro + rd*hitT;
        vec3 n = calcNormal(p);

        bool isPalm  = (sdPalms(p) < sdOcean(p)+0.1) && (sdPalms(p) < sdSun(p)+0.1);
        bool isOcean = (!isPalm) && (sdOcean(p) < sdSun(p)+0.1);
        bool isSun   = (!isPalm) && (!isOcean);

        if (isPalm) {
            // Silhouette: near-black with magenta rim light from sun
            vec3 sunDir = normalize(vec3(0.0, 2.5, 30.0) - p);
            float rimL = pow(1.0 - max(dot(n,-rd),0.0), 3.0);
            col = vec3(0.02, 0.01, 0.04);  // near-black silhouette
            col += vec3(1.0, 0.1, 0.7) * rimL * hdrPeak * audio; // hot pink rim

            // fwidth AA on palm edge
            float palmD = sdPalms(p);
            col *= 0.02 + smoothstep(0.0, fwidth(palmD), abs(palmD));
        } else if (isOcean) {
            // Ocean: neon grid + reflection of sky and sun
            vec3 oceanN = calcNormal(p);
            vec3 reflDir = reflect(rd, oceanN);
            vec3 reflCol = skyColor(reflDir) * 0.4;
            // Sun reflection
            float sunReflD = length(reflDir - normalize(vec3(0,2.5,30.0)-p));
            reflCol += vec3(1.0,0.2,0.8)*exp(-sunReflD*8.0)*hdrPeak*0.5*audio;
            // Neon grid overlay
            vec3 gridNeon = gridColor(p.xz, TIME);
            col = reflCol + gridNeon * hdrPeak * audio;
            col += vec3(0.02, 0.05, 0.1); // dark water base
        } else {
            // Sun sphere: hot pink/white HDR emitter with horizontal bars
            float barY = fract(p.y * 5.0);
            float barMask = smoothstep(0.5, 0.45, barY);
            vec3 sunCol = mix(vec3(1.0, 0.15, 0.65), vec3(1.0, 0.7, 0.3), barMask);
            col = sunCol * hdrPeak * audio;
            col += vec3(1.1, 1.0, 0.9) * pow(1.0-barMask,3.0) * hdrPeak * audio; // HDR bands
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
