/*{
  "DESCRIPTION": "Oil paint — Kuwahara filter with HDR impasto relief, TIME-driven shimmer, audio-reactive brush energy (Phase Q v4)",
  "CREDIT": "ShaderClaw (Kuwahara approach inspired by flockaroo) — auto-improve 2026-05-05",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage",   "LABEL": "Texture",       "TYPE": "image" },
    { "NAME": "brushRadius",  "LABEL": "Brush Size",    "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "paintSpec",    "LABEL": "Specular",      "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "vignetteAmt",  "LABEL": "Vignette",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "impastoPeak",  "LABEL": "HDR Peak",      "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "shimmerSpeed", "LABEL": "Shimmer Speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "transparentBg","LABEL": "Transparent",   "TYPE": "bool",  "DEFAULT": 0.0 }
  ],
  "PASSES": [
    { "TARGET": "paintBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI 3.1415927

vec2 fitUV(vec2 pos) {
    return (pos - 0.5 * RENDERSIZE)
         * min(IMG_SIZE_inputImage.y / RENDERSIZE.y, IMG_SIZE_inputImage.x / RENDERSIZE.x)
         / IMG_SIZE_inputImage + 0.5;
}

vec3 kuwahara(vec2 uv, float radius, float jitterAmt) {
    vec3  mean[4];
    vec3  var_acc[4];
    float count[4];

    for (int i = 0; i < 4; i++) {
        mean[i]    = vec3(0.0);
        var_acc[i] = vec3(0.0);
        count[i]   = 0.0;
    }

    for (int j = -6; j <= 6; j++) {
        for (int i = -6; i <= 6; i++) {
            if (abs(float(i)) > radius || abs(float(j)) > radius) continue;

            // Sub-pixel TIME-driven jitter for painterly breathing in silence
            float jit = sin(TIME * shimmerSpeed * PI + float(i + j) * 0.37) * jitterAmt;
            vec2 off  = vec2(float(i) + jit, float(j) + jit * 0.7);

            vec3 c = texture2D(inputImage, fitUV(uv * RENDERSIZE + off)).rgb;

            int qi = (i >= 0) ? 0 : 1;
            int qj = (j >= 0) ? 0 : 2;
            int q  = qi + qj;

            for (int k = 0; k < 4; k++) {
                if (k == q) {
                    mean[k]    += c;
                    var_acc[k] += c * c;
                    count[k]   += 1.0;
                }
            }
        }
    }

    float minVar = 1e8;
    vec3  result = vec3(0.0);

    for (int q = 0; q < 4; q++) {
        if (count[q] < 1.0) continue;
        vec3 m       = mean[q] / count[q];
        vec3 v       = var_acc[q] / count[q] - m * m;
        float totVar = v.r + v.g + v.b;
        if (totVar < minVar) { minVar = totVar; result = m; }
    }

    return result;
}

void main() {
    vec2  pos  = gl_FragCoord.xy;
    vec2  uv   = pos / RENDERSIZE;
    float amod = 0.5 + 0.5 * audioBass * audioReact;

    // PASS 0: Kuwahara paint filter — audio widens strokes on bass hits
    if (PASSINDEX == 0) {
        float r      = brushRadius * (0.7 + 0.6 * amod);
        float jitter = 0.35 * amod;
        gl_FragColor = vec4(kuwahara(uv, r, jitter), 1.0);
        return;
    }

    // PASS 1: Relief lighting + HDR impasto peaks
    vec2  texel = 1.0 / RENDERSIZE;
    float valC  = dot(texture2D(paintBuf, uv).rgb,                       vec3(0.333));
    float valR  = dot(texture2D(paintBuf, uv + vec2(texel.x, 0.0)).rgb,  vec3(0.333));
    float valL  = dot(texture2D(paintBuf, uv - vec2(texel.x, 0.0)).rgb,  vec3(0.333));
    float valU  = dot(texture2D(paintBuf, uv + vec2(0.0, texel.y)).rgb,  vec3(0.333));
    float valD  = dot(texture2D(paintBuf, uv - vec2(0.0, texel.y)).rgb,  vec3(0.333));

    // fwidth AA on ridge boundary for smoother specular edge
    float ridgeAA = fwidth(valC) * 6.0;

    vec3 norm = normalize(vec3(
        (valR - valL) / texel.x,
        (valU - valD) / texel.y,
        120.0
    ));

    // TIME-driven light shimmer: candle-like +/-15 deg oscillation
    float lShim = sin(TIME * shimmerSpeed) * 0.15;
    vec3  light = normalize(vec3(-1.0 + lShim, 1.0 + lShim * 0.4, 1.4));
    float diff  = clamp(dot(norm, light), 0.0, 1.0);

    vec3  viewDir = vec3(0.0, 0.0, 1.0);
    float spec    = pow(clamp(dot(reflect(-light, norm), viewDir), 0.0, 1.0), 12.0)
                  * paintSpec * amod;

    vec3 paintColor = texture2D(paintBuf, uv).rgb;
    vec3 lit = paintColor * mix(diff, 1.0, 0.85) + spec * vec3(0.95, 1.0, 1.1);

    // HDR impasto: bright ridge tips exceed 1.0 for host bloom pipeline
    float ridge = smoothstep(0.5 + ridgeAA, 0.9, valC);
    lit += paintColor * ridge * impastoPeak * amod * 0.9;

    // Vignette
    if (vignetteAmt > 0.0) {
        vec2  scc  = (pos - 0.5 * RENDERSIZE) / RENDERSIZE.x;
        float vign = 1.1 - vignetteAmt * dot(scc, scc);
        vign *= 1.0 - 0.7 * vignetteAmt * exp(-sin(pos.x / RENDERSIZE.x * PI) * 40.0);
        vign *= 1.0 - 0.7 * vignetteAmt * exp(-sin(pos.y / RENDERSIZE.y * PI) * 20.0);
        lit  *= vign;
    }

    // Output LINEAR HDR — host applies ACES / tone-map
    gl_FragColor = vec4(lit, 1.0);
}
