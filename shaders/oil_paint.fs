/*{
  "DESCRIPTION": "Impressionist Fields — domain-warped FBM color patches evoking Monet/Matisse. Standalone generator, no input required.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "Abstract", "Painterly"],
  "INPUTS": [
    { "NAME": "flowSpeed",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0,  "LABEL": "Flow Speed"    },
    { "NAME": "brushScale", "TYPE": "float", "DEFAULT": 2.8,  "MIN": 0.5, "MAX": 8.0,  "LABEL": "Brush Scale"   },
    { "NAME": "warpAmt",    "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.5,  "LABEL": "Warp Amount"   },
    { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.4,  "MIN": 1.0, "MAX": 4.0,  "LABEL": "HDR Peak"      },
    { "NAME": "contrast",   "TYPE": "float", "DEFAULT": 1.6,  "MIN": 0.5, "MAX": 4.0,  "LABEL": "Contrast"      },
    { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio React"   }
  ]
}*/

// 5-color fully saturated Impressionist palette (no white mixing in base stops)
//   cadmium red / viridian / cobalt blue / aureolin gold / lilac
vec3 paletteImpressionist(float t) {
    t = fract(t);
    const vec3 c0 = vec3(0.90, 0.05, 0.05);  // cadmium red
    const vec3 c1 = vec3(0.0,  0.75, 0.25);  // viridian green
    const vec3 c2 = vec3(0.05, 0.20, 0.95);  // cobalt blue
    const vec3 c3 = vec3(1.0,  0.82, 0.0);   // aureolin gold
    const vec3 c4 = vec3(0.55, 0.0,  0.90);  // deep lilac
    float s = t * 5.0;
    int i = int(s); float f = fract(s);
    if (i == 0) return mix(c0, c1, f);
    if (i == 1) return mix(c1, c2, f);
    if (i == 2) return mix(c2, c3, f);
    if (i == 3) return mix(c3, c4, f);
    return mix(c4, c0, f);
}

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5); }

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(hash(i), hash(i+vec2(1,0)), f.x),
        mix(hash(i+vec2(0,1)), hash(i+vec2(1,1)), f.x), f.y);
}

// 4-octave FBM
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 rot = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));
    for (int i = 0; i < 4; i++) {
        v += a * noise(p);
        p  = rot * p * 2.1;
        a *= 0.5;
    }
    return v;
}

// Domain-warped FBM (iq's technique): warp UV by two FBMs before sampling
float warpedFbm(vec2 p, float warp, float t) {
    vec2 q = vec2(fbm(p + vec2(0.0, 0.0) + t * 0.11),
                  fbm(p + vec2(5.2, 1.3) + t * 0.09));
    vec2 r = vec2(fbm(p + warp * q + vec2(1.7, 9.2) + t * 0.13),
                  fbm(p + warp * q + vec2(8.3, 2.8) + t * 0.07));
    return fbm(p + warp * r);
}

void main() {
    vec2 uv  = (gl_FragCoord.xy / RENDERSIZE.xy);
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.5) * audioReact;
    float t     = TIME * flowSpeed;

    vec2 p = uv * brushScale;

    // Primary warp-field value
    float v = warpedFbm(p, warpAmt, t);

    // Edge field: derivative of v gives brush-stroke directionality
    float vdx = warpedFbm(p + vec2(0.01, 0.0), warpAmt, t) - v;
    float vdy = warpedFbm(p + vec2(0.0,  0.01), warpAmt, t) - v;
    float edgeMag = length(vec2(vdx, vdy)) * 80.0;

    // Map v → color in the 5-stop palette
    vec3 col = paletteImpressionist(v);

    // Contrast boost: push mid-tones toward saturated extremes
    col = pow(col, vec3(1.0 / contrast));

    // HDR peak: brightest regions in the warp field pop above 1.0
    float brightness = smoothstep(0.3, 0.85, v);
    col *= mix(0.5, hdrPeak, brightness) * audio;

    // Black ink edges: high gradient magnitude = dark brush dividing lines
    float aa = fwidth(v);
    float ink = smoothstep(aa, 0.0, aa - edgeMag * aa * 0.6);
    col = mix(vec3(0.0), col, ink + 0.15);

    // Audio modulates saturation pop (brighter, more saturated on beat)
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(lum), col, 0.7 + audio * 0.3);

    gl_FragColor = vec4(col, 1.0);
}
