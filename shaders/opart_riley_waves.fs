/*{
  "CATEGORIES": ["Generator", "Geometric", "Audio Reactive"],
  "DESCRIPTION": "Dense black-and-white parallel waves bending and swimming until your eyes refuse to focus. Bridget Riley Op-Art that breathes with the music.",
  "INPUTS": [
    {"NAME":"freq","TYPE":"float","MIN":10.0,"MAX":160.0,"DEFAULT":60.0},
    {"NAME":"warpAmp","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.12},
    {"NAME":"xFreq","TYPE":"float","MIN":0.5,"MAX":12.0,"DEFAULT":3.0},
    {"NAME":"flow","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.4},
    {"NAME":"accentEvery","TYPE":"float","MIN":2.0,"MAX":20.0,"DEFAULT":7.0},
    {"NAME":"accentColor","TYPE":"color","DEFAULT":[0.95,0.15,0.25,1.0]},
    {"NAME":"texDisplace","TYPE":"float","MIN":0.0,"MAX":0.3,"DEFAULT":0.0},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Curl-noise-ish secondary turbulence so warp isn't pure sin; gives the
// "swimming" feel that Riley's plates evoke.
float audioCurl(vec2 uv, float t) {
    return sin(uv.x * 5.0 + t) * cos(uv.y * 7.0 - t) * 0.5;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Optional luminance guide from inputTex — bends pattern around silhouettes.
    float guide = 0.0;
    if (texDisplace > 0.001 && IMG_SIZE_inputTex.x > 0.0) {
        vec3 t = texture(inputTex, uv).rgb;
        guide = (t.r + t.g + t.b) / 3.0 - 0.5;
    }

    // Primary warp: low-frequency sin in x. Bass pushes it harder.
    float warp = sin(uv.x * xFreq + TIME * flow) * warpAmp * (1.0 + audioBass);
    warp += audioCurl(uv, TIME * flow) * audioMid * 0.08;
    warp += guide * texDisplace;

    float y = uv.y + warp;

    // Final stripe field — frequency modulated by audioHigh so highs tighten.
    float effectiveFreq = freq * (1.0 + audioHigh * 0.15);
    float stripe = sin(y * effectiveFreq);
    // Smoothstep edges to avoid aliasing on subpixel stripe widths.
    float bw = smoothstep(-0.04, 0.04, stripe);

    // Accent stripe substitution — every Nth black band becomes accent colour.
    float idx = floor(y * effectiveFreq / 3.14159);
    bool isAccent = mod(idx, max(2.0, accentEvery)) < 0.5;
    vec3 darkCol = isAccent ? accentColor.rgb : vec3(0.0);

    vec3 col = mix(darkCol, vec3(1.0), bw);

    // Audio peak invert flash — tasteful, only at very high level.
    float flash = smoothstep(0.85, 1.0, audioLevel);
    col = mix(col, vec3(1.0) - col, flash * 0.4);

    gl_FragColor = vec4(col, 1.0);
}
