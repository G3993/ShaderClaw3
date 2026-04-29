/*{
  "CATEGORIES": ["Effect", "Generator", "Audio Reactive"],
  "DESCRIPTION": "The image surrenders to gravity — pixels droop and smear downward in slow heavy waves like Dalí's melting clocks. Audio bursts widen the drips. Cocteau Twins chromatic separation.",
  "INPUTS": [
    {"NAME":"sagAmp","TYPE":"float","MIN":0.0,"MAX":0.4,"DEFAULT":0.12},
    {"NAME":"sagFreq","TYPE":"float","MIN":0.5,"MAX":8.0,"DEFAULT":2.5},
    {"NAME":"flow","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.3},
    {"NAME":"chroma","TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.012},
    {"NAME":"smear","TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.01},
    {"NAME":"reactive","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"dripBlur","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.4},
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

// Procedural Dali desert — used when no inputTex is supplied. Sky→ochre.
vec3 daliDesert(vec2 uv) {
    vec3 sky    = mix(vec3(0.95, 0.78, 0.55), vec3(0.55, 0.45, 0.6), 1.0 - uv.y);
    vec3 ground = mix(vec3(0.7, 0.55, 0.32), vec3(0.42, 0.30, 0.2), 1.0 - uv.y);
    float horizon = smoothstep(0.42, 0.46, uv.y);
    return mix(ground, sky, horizon);
}

vec3 sampleSrc(vec2 uv) {
    if (IMG_SIZE_inputTex.x > 0.0) {
        return texture(inputTex, clamp(uv, 0.0, 1.0)).rgb;
    }
    return daliDesert(uv);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Sag amount: horizontal sin curve drives vertical drop. Audio mid widens
    // the drip; flow speed scaled by audioLevel so loud moments melt faster.
    float sag = sin(uv.x * sagFreq + TIME * flow * (1.0 + audioLevel * 0.5))
              * sagAmp * (1.0 + audioMid * reactive);

    // Lateral smear — secondary horizontal jitter so it isn't pure vertical drop.
    vec2 base = vec2(uv.x + smear * sin(uv.y * 4.0 + TIME * flow), uv.y - sag);

    // Per-channel offset → wet chromatic aberration. R/B sample at +/- ch.
    float ch = chroma * (1.0 + audioHigh * reactive);
    float r = sampleSrc(base + vec2(0.0,  ch)).r;
    float g = sampleSrc(base                 ).g;
    float b = sampleSrc(base + vec2(0.0, -ch)).b;
    vec3 col = vec3(r, g, b);

    // Vertical drip blur — average several samples below the current point so
    // bright pixels appear to bleed downward (gravity-aligned blur).
    if (dripBlur > 0.001) {
        vec3 drip = vec3(0.0);
        const int N = 5;
        for (int i = 1; i <= N; i++) {
            float fi = float(i) / float(N);
            drip += sampleSrc(base - vec2(0.0, fi * 0.04 * dripBlur));
        }
        drip /= float(N);
        // Drip threshold widens with audioBass — transients release droplets.
        float thresh = 0.35 - audioBass * 0.15;
        float dripWeight = smoothstep(thresh, thresh + 0.1, max(max(col.r, col.g), col.b));
        col = mix(col, drip, dripBlur * 0.5 * dripWeight);
    }

    // Subtle S-curve tone map for that wet-paint look.
    col = smoothstep(vec3(-0.05), vec3(1.05), col);

    gl_FragColor = vec4(col, 1.0);
}
