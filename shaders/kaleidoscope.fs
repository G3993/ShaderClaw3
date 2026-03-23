/*{
  "DESCRIPTION": "Kaleidoscope — mirror-fold video into mesmerizing symmetry patterns with rotation and zoom",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "folds", "LABEL": "Folds", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 24.0 },
    { "NAME": "rotation", "LABEL": "Rotation", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "zoom", "LABEL": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 4.0 },
    { "NAME": "offsetX", "LABEL": "Offset X", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "offsetY", "LABEL": "Offset Y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "colorShift", "LABEL": "Color Shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioDrive", "LABEL": "Audio Drive", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "accentColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

#define PI 3.14159265359
#define TAU 6.28318530718

vec3 hueShift(vec3 col, float shift) {
    float angle = shift * TAU;
    float s = sin(angle), c = cos(angle);
    vec3 k = vec3(0.577350269);
    return col * c + cross(k, col) * s + k * dot(k, col) * (1.0 - c);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Audio
    float bass = audioBass * audioDrive;
    float mid = audioMid * audioDrive;

    // Center and aspect-correct
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    // Rotation — continuous + audio pulse
    float rot = TIME * rotation + bass * 0.15;
    float c = cos(rot), s = sin(rot);
    p = mat2(c, -s, s, c) * p;

    // Zoom — audio pumps it
    float z = zoom + bass * 0.3;
    p /= z;

    // Convert to polar
    float angle = atan(p.y, p.x);
    float r = length(p);

    // Fold into N segments
    float nFolds = floor(folds + mid * 2.0);
    float seg = TAU / nFolds;
    angle = mod(angle + seg * 0.5, seg) - seg * 0.5;
    angle = abs(angle); // mirror within each fold

    // Back to cartesian → sample UV
    vec2 foldedP = vec2(cos(angle), sin(angle)) * r;

    // Map back to UV space + offset
    vec2 sampleUV = foldedP / vec2(aspect, 1.0) + 0.5;
    sampleUV += vec2(offsetX, offsetY);

    // Tile/mirror to keep it in bounds
    sampleUV = abs(mod(sampleUV, 2.0) - 1.0);

    vec3 col = texture2D(inputImage, sampleUV).rgb;

    // Color shift — hue rotation based on angle and time
    if (colorShift > 0.001) {
        float hue = colorShift * (angle / seg + TIME * 0.1);
        col = hueShift(col, hue);
    }

    // Subtle edge glow at fold boundaries
    float foldEdge = abs(angle) / seg;
    float edgeGlow = smoothstep(0.98, 1.0, foldEdge) + smoothstep(0.02, 0.0, foldEdge);
    col += accentColor.rgb * edgeGlow * 0.3;

    // Center glow on beat
    float centerGlow = exp(-r * 8.0) * bass * 0.5;
    col += accentColor.rgb * centerGlow;

    float alpha = 1.0;
    if (transparentBg) {
        alpha = dot(col, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.15, alpha);
    }

    gl_FragColor = vec4(col, alpha);
}
