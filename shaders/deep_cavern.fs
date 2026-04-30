/*{
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "DESCRIPTION": "Falling slowly into an infinite tunnel that breathes. Concentric rings recede forever; the camera tilts with the mouse. Eliasson + Höller + 2001 stargate.",
  "INPUTS": [
    {"NAME":"pullSpeed","TYPE":"float","MIN":0.0,"MAX":4.0,"DEFAULT":0.7},
    {"NAME":"ringDensity","TYPE":"float","MIN":4.0,"MAX":60.0,"DEFAULT":20.0},
    {"NAME":"fogDensity","TYPE":"float","MIN":0.5,"MAX":4.0,"DEFAULT":1.5},
    {"NAME":"breathe","TYPE":"float","MIN":0.0,"MAX":0.3,"DEFAULT":0.12},
    {"NAME":"mouseTilt","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.2},
    {"NAME":"edgeGlow","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.8},
    {"NAME":"texMix","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.4},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

float hash(float x) { return fract(sin(x * 127.1) * 43758.5453); }
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// 4-stop ring palette indexed by band number — analog film tones.
vec3 ringPalette(float band) {
    float t = fract(band * 0.157);
    vec3 a = vec3(0.85, 0.35, 0.65);
    vec3 b = vec3(0.25, 0.45, 0.95);
    vec3 c = vec3(0.95, 0.78, 0.35);
    vec3 d = vec3(0.5, 0.85, 0.65);
    if (t < 0.25) return mix(a, b, t / 0.25);
    if (t < 0.5)  return mix(b, c, (t - 0.25) / 0.25);
    if (t < 0.75) return mix(c, d, (t - 0.5) / 0.25);
    return mix(d, a, (t - 0.75) / 0.25);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p  = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    // Camera tilt — centre offset toward mouse.
    p -= (mousePos - 0.5) * mouseTilt;

    // Bass squeezes the tunnel rhythmically.
    float r = length(p) * (1.0 + audioBass * breathe);
    float th = atan(p.y, p.x);

    // Tunnel coordinate: v = 1/r → moves toward camera as TIME progresses,
    // creating the infinite-recession feel. audioLevel multiplies pull.
    float v = 1.0 / max(r, 1e-3) + TIME * pullSpeed * (1.0 + audioLevel * 0.5);
    float u = th / 6.2832 + 0.5;

    // Ring band index — used to pick a per-ring palette stop.
    float band = floor(v * ringDensity);
    vec3 ringCol = ringPalette(band + audioMid * 2.0);

    // Alternating fill — two-tone bands give the rings a hard edge.
    float fillStripe = step(0.5, fract(v * ringDensity));
    ringCol *= mix(0.6, 1.0, fillStripe);

    // Atmospheric depth fog — close pixels (large r) bright, distant rings dim.
    float fog = exp(-r * fogDensity);

    // Edge glow at ring boundaries — smoothstep on the |fract−0.5| distance.
    float edge = smoothstep(0.06, 0.0, abs(fract(v * ringDensity) - 0.5))
               * edgeGlow * (0.4 + audioHigh * 1.5);

    // Optional video lining the tunnel walls.
    vec3 col = ringCol;
    if (IMG_SIZE_inputTex.x > 0.0 && texMix > 0.001) {
        vec3 tex = texture(inputTex, vec2(u, fract(v * 0.5))).rgb;
        col = mix(ringCol, tex, texMix);
    }

    col = col * fog + edge * ringCol;

    // Soft centre vignette so the vanishing point reads as deep, not blown.
    col *= smoothstep(0.0, 0.15, r);

    // Surprise: every ~38s a soft afterimage of an eye stares back from
    // the vanishing point — for ~1.5s, then closes. Tarkovsky's Solaris.
    {
        float _ph = fract(TIME / 38.0);
        float _f  = smoothstep(0.0, 0.06, _ph) * smoothstep(0.30, 0.18, _ph);
        float _eye = smoothstep(0.10, 0.0, length(p)) - smoothstep(0.06, 0.0, length(p));
        // Lash that opens
        float _open = smoothstep(0.02, 0.05, abs(p.y));
        col += vec3(0.95, 0.85, 0.65) * _eye * _open * _f * 0.65;
    }

    gl_FragColor = vec4(col, 1.0);
}
