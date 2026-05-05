/*{
  "DESCRIPTION": "Lichtenberg Figure — 2D branching Lichtenberg/fractal-lightning tree grown from a bright HDR trunk. NEW ANGLE: 2D branching fractal tree vs prior 3D arctic ice-crystal ring.",
  "CATEGORIES": ["Generator", "Abstract"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"branchDepth","LABEL":"Branch Depth","TYPE":"float","MIN":2.0,"MAX":8.0,"DEFAULT":5.5},
    {"NAME":"branchAng",  "LABEL":"Branch Angle","TYPE":"float","MIN":0.1,"MAX":1.2,"DEFAULT":0.45},
    {"NAME":"branchLen",  "LABEL":"Branch Length","TYPE":"float","MIN":0.3,"MAX":0.9,"DEFAULT":0.62},
    {"NAME":"glowWidth",  "LABEL":"Glow Width",  "TYPE":"float","MIN":0.003,"MAX":0.05,"DEFAULT":0.012},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",    "TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.8},
    {"NAME":"growSpeed",  "LABEL":"Grow Speed",  "TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.18},
    {"NAME":"audioReact", "LABEL":"Audio",       "TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

vec3 lightningPal(float depth) {
    vec3 core = vec3(1.4, 0.0,  2.5);
    vec3 mid  = vec3(0.0, 0.8,  2.5);
    vec3 tip  = vec3(2.5, 2.5,  2.8);
    if (depth < 0.5) return mix(core, mid, depth * 2.0);
    return mix(mid, tip, (depth - 0.5) * 2.0);
}

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1,311.7))) * 43758.5453); }

float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    return length(pa - ba * h);
}

void addBranch(vec2 uv, vec2 a, vec2 b, float depthNorm,
               float audio, inout vec3 col) {
    float d = sdSegment(uv, a, b);
    float w = glowWidth * (1.0 - depthNorm * 0.5);
    float edge = fwidth(d);
    float core   = smoothstep(w * 0.5 + edge, 0.0, d);
    float corona = exp(-d / (w * 4.0));
    vec3 c = lightningPal(depthNorm) * hdrPeak * audio;
    col += c * (core + corona * 0.35);
    col *= 1.0 - smoothstep(0.0, edge, abs(d - w * 0.5) - edge) * core * 0.4;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioReact * 0.4
                      + audioBass  * audioReact * 0.3;

    float growPhase = fract(TIME * growSpeed * 0.25);
    float totalGrow = smoothstep(0.0, 0.6, growPhase)
                    * smoothstep(1.0, 0.7, growPhase);

    vec3 col = vec3(0.0, 0.0, 0.008);

    int maxDepth = int(clamp(branchDepth, 2.0, 7.0));

    float tAngle0 = 1.5707963;
    float tLen0   = 0.35 * totalGrow;
    vec2  tStart  = vec2(0.0, -0.75);
    vec2  tEnd    = tStart + tLen0 * vec2(cos(tAngle0), sin(tAngle0));
    addBranch(uv, tStart, tEnd, 0.0, audio, col);

    float curLen   = tLen0 * branchLen;
    float baseAngle= tAngle0;
    float jitter0  = (hash11(TIME * 0.1) - 0.5) * 0.15;

    for (int i1 = 0; i1 < 2; i1++) {
        if (maxDepth < 1) break;
        float a1 = baseAngle + (float(i1) * 2.0 - 1.0) * branchAng + jitter0;
        float len1 = curLen * totalGrow;
        vec2 s1 = tEnd;
        vec2 e1 = s1 + len1 * vec2(cos(a1), sin(a1));
        addBranch(uv, s1, e1, 0.15, audio, col);

        float len2 = len1 * branchLen;
        for (int i2 = 0; i2 < 2; i2++) {
            if (maxDepth < 2) break;
            float j2 = hash11(float(i1) * 7.3 + float(i2) * 3.1 + floor(TIME * growSpeed)) * 0.2 - 0.1;
            float a2 = a1 + (float(i2) * 2.0 - 1.0) * branchAng + j2;
            float l2 = len2 * totalGrow;
            vec2 s2 = e1;
            vec2 e2 = s2 + l2 * vec2(cos(a2), sin(a2));
            addBranch(uv, s2, e2, 0.33, audio, col);

            float len3 = len2 * branchLen;
            for (int i3 = 0; i3 < 2; i3++) {
                if (maxDepth < 3) break;
                float j3 = hash11(float(i1)*13.1 + float(i2)*7.7 + float(i3)*2.9 + floor(TIME * growSpeed)) * 0.2 - 0.1;
                float a3 = a2 + (float(i3) * 2.0 - 1.0) * branchAng + j3;
                float l3 = len3 * totalGrow;
                vec2 s3 = e2;
                vec2 e3 = s3 + l3 * vec2(cos(a3), sin(a3));
                addBranch(uv, s3, e3, 0.55, audio, col);

                float len4 = len3 * branchLen;
                for (int i4 = 0; i4 < 2; i4++) {
                    if (maxDepth < 4) break;
                    float j4 = hash11(float(i1)*17.3 + float(i2)*11.1 + float(i3)*5.7 + float(i4)*2.3 + floor(TIME * growSpeed)) * 0.25 - 0.125;
                    float a4 = a3 + (float(i4) * 2.0 - 1.0) * branchAng + j4;
                    float l4 = len4 * totalGrow;
                    vec2 s4 = e3;
                    vec2 e4 = s4 + l4 * vec2(cos(a4), sin(a4));
                    addBranch(uv, s4, e4, 0.75, audio, col);

                    float len5 = len4 * branchLen;
                    for (int i5 = 0; i5 < 2; i5++) {
                        if (maxDepth < 5) break;
                        float j5 = hash21(vec2(float(i1)*19.0+float(i2)*13.0, float(i3)*7.0+float(i4)*5.0+float(i5)*2.0) + floor(TIME * growSpeed)) * 0.3 - 0.15;
                        float a5 = a4 + (float(i5) * 2.0 - 1.0) * branchAng + j5;
                        float l5 = len5 * totalGrow;
                        vec2 s5 = e4;
                        vec2 e5 = s5 + l5 * vec2(cos(a5), sin(a5));
                        addBranch(uv, s5, e5, 0.90, audio, col);
                    }
                }
            }
        }
    }

    float rootD = length(uv - tStart) - 0.025;
    col += vec3(3.0, 2.5, 2.8) * exp(-max(rootD, 0.0) * 50.0) * audio;

    gl_FragColor = vec4(col, 1.0);
}
