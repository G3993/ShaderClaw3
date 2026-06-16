/*{
  "CATEGORIES": ["Generator", "Ambient"],
  "DESCRIPTION": "A breathing volume of pure colour-light — almost no motion, just slow chromatic drift. Reads as architectural light, not screen content. Turrell Aten Reign / Skyspace.",
  "INPUTS": [
    {"NAME":"colorA","TYPE":"color","DEFAULT":[0.92,0.35,0.55,1.0]},
    {"NAME":"colorB","TYPE":"color","DEFAULT":[0.25,0.35,0.85,1.0]},
    {"NAME":"cyclePeriod","TYPE":"float","MIN":5.0,"MAX":300.0,"DEFAULT":60.0},
    {"NAME":"vignette","TYPE":"float","MIN":0.0,"MAX":4.0,"DEFAULT":1.4},
    {"NAME":"grain","TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.015},
    {"NAME":"audioInfluence","TYPE":"float","MIN":0.0,"MAX":0.3,"DEFAULT":0.05},
    {"NAME":"texInfluence","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.0},
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

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 c  = uv - 0.5;
    float r = length(c) * 1.4;

    // Ultra-slow temporal lerp between A and B. Period in seconds; sin avoids
    // sudden A↔B flips that would break the meditative pace.
    float t = 0.5 + 0.5 * sin(TIME * 6.2832 / max(cyclePeriod, 1.0));
    vec3 base = mix(colorA.rgb, colorB.rgb, t);

    // Subtle 2D low-frequency drift — almost-but-not-quite uniform field.
    float drift = vnoise(uv * 0.6 + vec2(TIME * 0.005, TIME * 0.003));
    // Tiny hue rotation per region keeps OLED from looking flat-banded.
    base = mix(base, hsv2rgb(vec3(drift, 0.4, 1.0)) * length(base), 0.08);

    // Optional: live video colour bleeds into the room. Sample average of
    // 3 widely-spaced pixels so it's a tone, not an image.
    if (IMG_SIZE_inputTex.x > 0.0 && texInfluence > 0.001) {
        vec3 avg = (texture(inputTex, vec2(0.2, 0.2)).rgb +
                    texture(inputTex, vec2(0.5, 0.5)).rgb +
                    texture(inputTex, vec2(0.8, 0.8)).rgb) / 3.0;
        base = mix(base, avg, texInfluence);
    }

    // Soft radial vignette.
    base *= 1.0 - pow(clamp(r, 0.0, 1.0), max(vignette, 0.01));

    // Tiny audio breath — capped at audioInfluence. Default extremely subtle.
    base += audioLevel * audioInfluence;

    // Film grain — kills OLED banding on projection.
    base += (hash(uv * RENDERSIZE.xy + TIME) - 0.5) * grain;

    gl_FragColor = vec4(base, 1.0);
}
