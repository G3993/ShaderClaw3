/*{
  "CATEGORIES": ["Effect", "Generator", "Audio Reactive"],
  "DESCRIPTION": "A signal trying to hold itself together — scanlines, RGB shift, vertical tear, EMI bursts. Image transmits rather than displays. Blade Runner 2049 + Nam June Paik.",
  "INPUTS": [
    {"NAME":"chroma","TYPE":"float","MIN":0.0,"MAX":0.04,"DEFAULT":0.008},
    {"NAME":"scanFreq","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.0},
    {"NAME":"tearProbability","TYPE":"float","MIN":0.0,"MAX":0.3,"DEFAULT":0.06},
    {"NAME":"breakAmount","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.3},
    {"NAME":"glow","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.7},
    {"NAME":"hologramTint","TYPE":"color","DEFAULT":[0.4,1.0,0.95,1.0]},
    {"NAME":"audioReact","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Procedural test pattern when no input bound — looks like a holographic
// circuit grid so the shader is still meaningful as a generator.
vec3 testPattern(vec2 uv) {
    vec2 g = fract(uv * 16.0);
    float lines = step(0.95, max(g.x, g.y));
    float diag = sin((uv.x + uv.y) * 60.0) * 0.5 + 0.5;
    float ring = smoothstep(0.05, 0.0, abs(length(uv - 0.5) - 0.3));
    return vec3(0.2 + lines * 0.6 + diag * 0.2 + ring * 0.8) * vec3(0.4, 1.0, 0.95);
}

vec3 sampleSrc(vec2 uv) {
    if (IMG_SIZE_inputTex.x > 0.0) return texture(inputTex, clamp(uv, 0.0, 1.0)).rgb;
    return testPattern(uv);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Vertical tear: divide screen into bands, hash some bands per time-slice
    // and shift them horizontally. Probability scales with audioBass.
    float bandH = 0.04;
    float bandY = floor(uv.y / bandH) * bandH;
    float tearTrig = step(1.0 - tearProbability * (1.0 + audioBass * audioReact),
                          hash(vec2(bandY, floor(TIME * 8.0))));
    uv.x += tearTrig * (hash(vec2(bandY, TIME)) - 0.5) * 0.15;

    // RGB chromatic shift — width modulated by audioHigh.
    float ch = chroma * (1.0 + audioHigh * audioReact);
    float r = sampleSrc(uv + vec2( ch, 0.0)).r;
    float g = sampleSrc(uv                ).g;
    float b = sampleSrc(uv - vec2( ch, 0.0)).b;
    vec3 col = vec3(r, g, b) * hologramTint.rgb;

    // Scanlines — pin frequency to gl_FragCoord.y so it scales with resolution.
    col *= 0.85 + 0.15 * sin(gl_FragCoord.y * scanFreq * 0.5);

    // Signal break: every K seconds, replace fragments with hash noise.
    float breakTrig = step(0.9, hash(vec2(floor(TIME * 4.0), 0.0)));
    col = mix(col, vec3(hash(uv * TIME)),
              breakAmount * audioBass * audioReact * 0.4 * breakTrig);

    // Mid-band flicker — audioMid drives a subtle brightness wobble.
    float flicker = 0.92 + 0.08 * sin(TIME * 60.0 + hash(vec2(floor(TIME * 30.0))) * 6.28);
    col *= mix(1.0, flicker, audioMid * audioReact * 0.5);

    // Edge bloom — high-luminance pixels glow beyond their actual position.
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col += hologramTint.rgb * pow(lum, 1.4) * glow * 0.3;

    // Transmission strength: low audio dims the hologram (signal is weak).
    col *= 0.5 + audioLevel * 0.6;

    gl_FragColor = vec4(col, 1.0);
}
