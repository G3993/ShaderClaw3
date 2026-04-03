/*{
  "DESCRIPTION": "Oil paint effect — Kuwahara filter with relief lighting for painterly brush strokes",
  "CREDIT": "ShaderClaw (Kuwahara approach inspired by flockaroo)",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "brushRadius", "LABEL": "Brush Size", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "paintSpec", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "vignetteAmt", "LABEL": "Vignette", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ],
  "PASSES": [
    { "TARGET": "paintBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI 3.1415927

// Aspect-correct UV
vec2 fitUV(vec2 pos) {
    return (pos - 0.5 * RENDERSIZE) * min(IMG_SIZE_inputImage.y / RENDERSIZE.y, IMG_SIZE_inputImage.x / RENDERSIZE.x) / IMG_SIZE_inputImage + 0.5;
}

// Kuwahara filter: find the quadrant with lowest variance and use its mean color
// This creates the flat-color brush stroke look of oil paintings
vec3 kuwahara(vec2 uv, float radius) {
    vec2 texel = 1.0 / RENDERSIZE;
    int r = int(radius);

    vec3 mean[4];
    vec3 var_acc[4];
    float count[4];

    // Initialize accumulators
    for (int i = 0; i < 4; i++) {
        mean[i] = vec3(0.0);
        var_acc[i] = vec3(0.0);
        count[i] = 0.0;
    }

    // Sample the 4 quadrants around the pixel
    for (int j = -6; j <= 6; j++) {
        for (int i = -6; i <= 6; i++) {
            if (abs(i) > r || abs(j) > r) continue;

            vec2 offset = vec2(float(i), float(j)) * texel;
            vec3 c = texture2D(inputImage, fitUV((uv * RENDERSIZE) + vec2(float(i), float(j)))).rgb;

            // Determine which quadrant(s) this sample belongs to
            // Quadrant 0: top-right, 1: top-left, 2: bottom-left, 3: bottom-right
            int qi = (i >= 0) ? 0 : 1;
            int qj = (j >= 0) ? 0 : 2;
            int q = qi + qj;

            mean[q] += c;
            var_acc[q] += c * c;
            count[q] += 1.0;
        }
    }

    // Find the quadrant with minimum variance
    float minVar = 1e8;
    vec3 result = vec3(0.0);

    for (int q = 0; q < 4; q++) {
        if (count[q] < 1.0) continue;
        vec3 m = mean[q] / count[q];
        vec3 v = var_acc[q] / count[q] - m * m;
        float totalVar = v.r + v.g + v.b;
        if (totalVar < minVar) {
            minVar = totalVar;
            result = m;
        }
    }

    return result;
}

void main() {
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = pos / RENDERSIZE;

    // ==== PASS 0: Kuwahara paint filter ====
    if (PASSINDEX == 0) {
        gl_FragColor = vec4(kuwahara(uv, brushRadius), 1.0);
        return;
    }

    // ==== PASS 1: Relief lighting ====
    vec2 texel = 1.0 / RENDERSIZE;
    float valC = dot(texture2D(paintBuf, uv).rgb, vec3(0.333));
    float valR = dot(texture2D(paintBuf, uv + vec2(texel.x, 0.0)).rgb, vec3(0.333));
    float valL = dot(texture2D(paintBuf, uv - vec2(texel.x, 0.0)).rgb, vec3(0.333));
    float valU = dot(texture2D(paintBuf, uv + vec2(0.0, texel.y)).rgb, vec3(0.333));
    float valD = dot(texture2D(paintBuf, uv - vec2(0.0, texel.y)).rgb, vec3(0.333));

    vec3 norm = normalize(vec3(
        (valR - valL) / texel.x,
        (valU - valD) / texel.y,
        150.0
    ));

    vec3 light = normalize(vec3(-1.0, 1.0, 1.4));
    float diff = clamp(dot(norm, light), 0.0, 1.0);
    float spec = pow(clamp(dot(reflect(light, norm), vec3(0.0, 0.0, -1.0)), 0.0, 1.0), 12.0) * paintSpec;

    gl_FragColor = texture2D(paintBuf, uv) * mix(diff, 1.0, 0.9)
                 + spec * vec4(0.85, 1.0, 1.15, 1.0);

    // Vignette
    if (vignetteAmt > 0.0) {
        vec2 scc = (pos - 0.5 * RENDERSIZE) / RENDERSIZE.x;
        float vign = 1.1 - vignetteAmt * dot(scc, scc);
        vign *= 1.0 - 0.7 * vignetteAmt * exp(-sin(pos.x / RENDERSIZE.x * PI) * 40.0);
        vign *= 1.0 - 0.7 * vignetteAmt * exp(-sin(pos.y / RENDERSIZE.y * PI) * 20.0);
        gl_FragColor.xyz *= vign;
    }

    gl_FragColor.w = 1.0;
}
