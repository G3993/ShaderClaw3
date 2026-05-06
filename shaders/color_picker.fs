/*{
    "DESCRIPTION": "Color Kaleidoscope — 2D N-fold chromatic mandala with rotating prismatic sectors. Standalone HDR generator.",
    "CREDIT": "auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "Abstract"],
    "INPUTS": [
        {"NAME":"speed","TYPE":"float","DEFAULT":0.35,"MIN":0.0,"MAX":2.0,"LABEL":"Rotation Speed"},
        {"NAME":"folds","TYPE":"float","DEFAULT":6.0,"MIN":2.0,"MAX":12.0,"LABEL":"Symmetry Folds"},
        {"NAME":"zoom","TYPE":"float","DEFAULT":1.0,"MIN":0.1,"MAX":3.0,"LABEL":"Zoom"},
        {"NAME":"hueOffset","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0,"LABEL":"Hue Shift"},
        {"NAME":"ringCount","TYPE":"float","DEFAULT":5.0,"MIN":2.0,"MAX":10.0,"LABEL":"Ring Count"},
        {"NAME":"audioMod","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0,"LABEL":"Audio Mod"},
        {"NAME":"hdrPeak","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Peak"}
    ]
}*/

#define TAU 6.28318530718

vec3 hsv2rgb(float h, float s, float v) {
    vec3 k = mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0);
    return v * mix(vec3(1.0), clamp(min(k, 4.0 - k), 0.0, 1.0), s);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv /= max(0.01, zoom);

    float t = TIME * speed;
    float audio = 1.0 + (audioLevel + audioBass * 0.7) * audioMod;
    float N = max(2.0, folds);

    float r = length(uv);
    float a = atan(uv.y, uv.x);

    float sector = TAU / N;
    float foldA = mod(a + t * 0.2, sector);
    if (foldA > sector * 0.5) foldA = sector - foldA;

    vec3 col = vec3(0.0);

    float nRings = max(1.0, ringCount);
    for (float i = 0.0; i < 10.0; i++) {
        if (i >= nRings) break;
        float ringR = (i + 1.0) / (nRings + 1.0) * 0.85 * audio;
        float d = abs(r - ringR);
        float aa = fwidth(d);
        float ring = smoothstep(0.016 + aa, 0.0, d);
        float hue = hueOffset + i / nRings + foldA / sector * 0.5 + t * 0.08;
        col += hsv2rgb(hue, 1.0, 1.0) * ring * hdrPeak;
    }

    float edgeDist = min(foldA, sector - foldA) / (sector * 0.5);
    float spoke = pow(max(0.0, 1.0 - edgeDist * 3.0), 3.0) * exp(-r * 1.5);
    float spokeHue = hueOffset + a / TAU + t * 0.04;
    col += hsv2rgb(spokeHue, 1.0, 1.0) * spoke * hdrPeak * 2.0;

    float burst = exp(-r * r * 8.0 / audio) * 2.0;
    float burstHue = hueOffset + t * 0.15;
    col += hsv2rgb(burstHue, 1.0, 1.0) * burst * hdrPeak;

    gl_FragColor = vec4(col, 1.0);
}
