/*{
  "DESCRIPTION": "RGB Fault Lines — 2D Voronoi fault map with per-cell chromatic time offsets. Each cell replays a different moment. NEW ANGLE: 2D spatial mosaic vs prior 3D RGB channel-split data planes.",
  "CATEGORIES": ["Generator", "Glitch", "Abstract"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"cellScale",  "LABEL":"Cell Scale",   "TYPE":"float","MIN":2.0,"MAX":20.0,"DEFAULT":7.0},
    {"NAME":"timeShift",  "LABEL":"Time Shift",   "TYPE":"float","MIN":0.0,"MAX":3.0, "DEFAULT":1.2},
    {"NAME":"chromaAmt",  "LABEL":"RGB Split",    "TYPE":"float","MIN":0.0,"MAX":0.08,"DEFAULT":0.025},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",     "TYPE":"float","MIN":1.0,"MAX":4.0, "DEFAULT":2.5},
    {"NAME":"glitchRate", "LABEL":"Glitch Rate",  "TYPE":"float","MIN":0.0,"MAX":4.0, "DEFAULT":1.8},
    {"NAME":"edgeGlow",   "LABEL":"Edge Glow",    "TYPE":"float","MIN":0.0,"MAX":3.0, "DEFAULT":1.4},
    {"NAME":"audioReact", "LABEL":"Audio",        "TYPE":"float","MIN":0.0,"MAX":2.0, "DEFAULT":1.0}
  ]
}*/

vec3 rgbFaultPal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(2.5, 0.0, 0.0),  vec3(0.0, 0.0, 2.5),  t * 4.0);
    if (t < 0.50) return mix(vec3(0.0, 0.0, 2.5),  vec3(0.0, 2.5, 0.0),  (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(0.0, 2.5, 0.0),  vec3(2.5, 2.0, 0.0),  (t-0.50)*4.0);
    return         mix(vec3(2.5, 2.0, 0.0),         vec3(2.5, 0.0, 0.0),  (t-0.75)*4.0);
}

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash22(vec2 p)  {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)),
                          dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}

vec2 voronoi(vec2 p) {
    vec2 ip = floor(p);
    float minD = 1e9;
    float minId = 0.0;
    for (int iy = -1; iy <= 1; iy++) {
        for (int ix = -1; ix <= 1; ix++) {
            vec2 neighbor = vec2(float(ix), float(iy));
            vec2 cell     = ip + neighbor;
            vec2 jitter   = hash22(cell);
            jitter = 0.5 + 0.5 * sin(TIME * 0.15 * glitchRate + jitter * 6.28318);
            vec2 r  = neighbor + jitter - fract(p);
            float d = dot(r, r);
            if (d < minD) {
                minD  = d;
                minId = dot(cell, vec2(1.0, 37.0));
            }
        }
    }
    return vec2(sqrt(minD), minId);
}

vec3 cellColour(float cellId) {
    float t = TIME * glitchRate;
    float tOff = hash11(cellId) * timeShift;

    float glitchCycle = floor(t * 0.4 + hash11(cellId * 3.7));
    float glitchPhase = fract(t * 0.4 + hash11(cellId * 3.7));
    float flickerActive = step(0.82, hash11(cellId + glitchCycle * 31.7));
    float flicker = flickerActive * smoothstep(0.0, 0.12, glitchPhase)
                                  * smoothstep(1.0, 0.45, glitchPhase);

    float ci   = hash11(cellId * 0.017) + tOff * 0.08 + t * 0.02;
    vec3  base = rgbFaultPal(ci) * hdrPeak;
    vec3  flash = rgbFaultPal(ci + 0.5) * hdrPeak * 1.5;

    return mix(base, flash, flicker);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioReact * 0.4
                      + audioBass  * audioReact * 0.3;

    vec2 p = vec2(uv.x * aspect, uv.y) * cellScale;

    float ca = chromaAmt * (1.0 + audioHigh * audioReact * 0.5);
    vec2 pR = p + vec2( ca * cellScale,  0.0);
    vec2 pG = p;
    vec2 pB = p + vec2(-ca * cellScale,  0.0);

    vec2 vorR = voronoi(pR);
    vec2 vorG = voronoi(pG);
    vec2 vorB = voronoi(pB);

    vec3 cR = cellColour(vorR.y);
    vec3 cG = cellColour(vorG.y);
    vec3 cB = cellColour(vorB.y);

    vec3 col = vec3(cR.r, cG.g, cB.b) * audio;

    vec2 vorMain = voronoi(p);
    float edgeDist = vorMain.x;
    float edgeAA = fwidth(edgeDist);
    float glowEdge = exp(-edgeDist * cellScale * 2.5) * edgeGlow;
    col += vec3(2.8, 2.8, 2.8) * glowEdge;
    float inkMask = smoothstep(edgeAA * 3.0, 0.0, edgeDist - 0.04 / cellScale);
    col *= 1.0 - inkMask * 0.80;

    gl_FragColor = vec4(col, 1.0);
}
