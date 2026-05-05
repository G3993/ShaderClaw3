/*{
    "DESCRIPTION": "Electric arc — simplex noise plasma with glowing discharge line",
    "CREDIT": "Port of Humus Electro demo, simplex noise by Nikita Miropolskiy",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "midSize1", "LABEL": "Mid Size 1", "TYPE": "float", "DEFAULT": 0.60, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "midSize2", "LABEL": "Mid Size 2", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.4 },
        { "NAME": "burn", "LABEL": "Burn", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "wiggleAmp", "LABEL": "Wiggle Amp", "TYPE": "float", "DEFAULT": 45.0, "MIN": 0.0, "MAX": 100.0 },
        { "NAME": "freak", "LABEL": "Freak", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.5, "MAX": 10.0 },
        { "NAME": "freak2", "LABEL": "Freak 2", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "arcCount",   "LABEL": "Arc Count",    "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0, "MAX": 8.0 },
        { "NAME": "branching",  "LABEL": "Branch Forks", "TYPE": "float", "DEFAULT": 0.60, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "flicker",    "LABEL": "Flicker",      "TYPE": "float", "DEFAULT": 0.30, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "hueShift",   "LABEL": "Hue Shift",    "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "audioReact", "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",    "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5, "MAX": 5.0 },
        { "NAME": "arcColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.7, 0.0, 1.0, 1.0] }
    ]
}*/

vec3 random3(vec3 c) {
    float j = 4096.0 * sin(dot(c, vec3(17.0, 59.4, 15.0)));
    vec3 r;
    r.z = fract(512.0 * j);
    j *= 0.125;
    r.x = fract(512.0 * j);
    j *= 0.125;
    r.y = fract(512.0 * j);
    return r - freak;
}

float simplex3d(vec3 p) {
    float F3 = 0.3333333;
    float G3 = 0.1666667;
    vec3 s = floor(p + dot(p, vec3(F3)));
    vec3 x = p - s + dot(s, vec3(G3));
    vec3 e = step(vec3(0.0), x - x.yzx);
    vec3 i1 = e * (1.0 - e.zxy);
    vec3 i2 = 1.0 - e.zxy * (1.0 - e);
    vec3 x1 = x - i1 + G3;
    vec3 x2 = x - i2 + 2.0 * G3;
    vec3 x3 = x - 1.0 + 3.0 * G3;
    vec4 w, d;
    w.x = dot(x, x);
    w.y = dot(x1, x1);
    w.z = dot(x2, x2);
    w.w = dot(x3, x3);
    w = max(freak2 - w, 0.0);
    d.x = dot(random3(s), x);
    d.y = dot(random3(s + i1), x1);
    d.z = dot(random3(s + i2), x2);
    d.w = dot(random3(s + 1.0), x3);
    w *= w;
    w *= w;
    d *= w;
    return dot(d, vec4(wiggleAmp));
}

float fbmNoise(vec3 m) {
    return 0.5333333 * simplex3d(m)
         + 0.2666667 * simplex3d(2.0 * m)
         + 0.1333333 * simplex3d(4.0 * m)
         + 0.0666667 * simplex3d(8.0 * m);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 centered = uv * 2.0 - 1.0;

    // Guitar-string pluck: mouse crossing center amplifies wiggle
    float mouseDist = abs(mousePos.y - 0.5) * 2.0; // 0 at center, 1 at edges
    float pluck = 1.0 - smoothstep(0.0, 0.4, mouseDist); // strong near center, fades out
    float ampBoost = 1.0 + pluck * 3.0 + audioBass * 5.0; // mouse pluck + bass shakes the arc

    vec2 p = gl_FragCoord.xy / RENDERSIZE.x;

    // ── Multiple stacked arcs at varying y-offsets ───────────────────
    int AC = int(clamp(arcCount, 1.0, 8.0));
    vec3 col = vec3(0.0);
    for (int ai = 0; ai < 8; ai++) {
        if (ai >= AC) break;
        float fai = float(ai);
        // Each arc has its own y-offset, time-phase, and amplitude
        float yOff   = (fract(sin(fai * 7.13) * 43758.5453) - 0.5) * 0.7;
        float phase  = fai * 1.7;
        float ampJit = 0.6 + 0.6 * fract(sin(fai * 11.7) * 43758.5453);

        vec3 p3 = vec3(p, TIME * 0.4 + phase);
        float intensity = fbmNoise(p3 * 12.0 + 12.0);

        float tw = clamp(centered.x * -centered.x * midSize1 + midSize2, 0.0, 1.0);
        float yc = abs(intensity * -tw * ampBoost * ampJit + (centered.y - yOff));
        float g  = pow(yc, burn * (1.0 - audioLevel * audioReact * 0.4));

        // Hue shift per arc — purple/cyan/white gradient via cosine palette
        float hue = fract(hueShift + fai * 0.18 + TIME * 0.05);
        vec3 arcCol = mix(arcColor.rgb,
                          0.5 + 0.5 * cos(6.28318 * hue + vec3(0.0, 2.094, 4.188)),
                          hueShift);
        vec3 acc = arcCol;
        acc = acc * -g + acc;
        acc = acc * acc;
        acc = acc * acc;
        col += acc * hdrBoost;

        // Branch forks — sparse perpendicular bolts shooting off the arc
        if (branching > 0.001) {
            float branchPhase = fract(TIME * 1.5 + fai * 0.7);
            float branchTrig = step(0.94, fract(sin(floor(TIME * 6.0 + fai) * 17.3) * 43758.5453));
            float fork = smoothstep(0.05, 0.0, abs(centered.x - (branchPhase * 2.0 - 1.0)))
                       * smoothstep(0.5, 0.0, abs(centered.y - yOff));
            col += arcCol * fork * branching * branchTrig * 1.5 * hdrBoost;
        }
    }

    // Stochastic flicker
    float fl = 1.0 + (fract(sin(floor(TIME * 30.0) * 91.7) * 43758.5453) - 0.5) * flicker * 0.6;
    col *= fl;

    // Surprise: every ~11s a brief power-out — the entire field cuts to
    // black for ~80ms then resumes. Distant thunder behind the bolt.
    {
        float _ph = fract(TIME / 11.0);
        float _cut = step(_ph, 0.04);
        col = mix(col, vec3(0.0), _cut * 0.85);
    }

    gl_FragColor = vec4(col, 1.0);
}
