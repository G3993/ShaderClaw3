/*{
  "DESCRIPTION": "Polka dot pixel tiles — flipping mosaic that reveals image through animated circular pixels",
  "CREDIT": "Florian Berger (flockaroo), adapted for ISF by ShaderClaw",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "tileScale", "LABEL": "Tile Size", "TYPE": "float", "DEFAULT": 20.0, "MIN": 5.0, "MAX": 60.0 },
    { "NAME": "flipSpeed", "LABEL": "Flip Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "flipInterval", "LABEL": "Flip Interval", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 4.0 },
    { "NAME": "dotSize", "LABEL": "Dot Size", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 0.5 },
    { "NAME": "dotPulse",  "LABEL": "Dot Pulse",  "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "scrollX",   "LABEL": "Scroll X",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": -1.0,"MAX": 1.0 },
    { "NAME": "scrollY",   "LABEL": "Scroll Y",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": -1.0,"MAX": 1.0 },
    { "NAME": "rotateGrid","LABEL": "Rotate Grid","TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioReact","LABEL": "Audio React","TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "specAmt", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "vignetteAmt", "LABEL": "Vignette", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "gapShade", "LABEL": "Gap Shade", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "bgColor", "LABEL": "Gap Color", "TYPE": "color", "DEFAULT": [0.2, 0.3, 0.4, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ],
  "PASSES": [
    { "TARGET": "histBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Number of history tiles in each direction
#define Xnum 10
#define Ynum 10

// Hash for pseudo-random per-tile values
float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec4 hash42(vec2 p) {
    vec4 p4 = fract(vec4(p.xyxy) * vec4(0.1031, 0.1030, 0.0973, 0.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}

// Aspect-correct UV for texture sampling
vec2 fitUV(vec2 pos) {
    return (pos - 0.5 * RENDERSIZE) * min(IMG_SIZE_inputImage.y / RENDERSIZE.y, IMG_SIZE_inputImage.x / RENDERSIZE.x) / IMG_SIZE_inputImage + 0.5;
}

vec2 frameUV(int frame, vec2 uv) {
    frame = int(mod(float(frame), float(Xnum * Ynum)));
    return (uv + vec2(mod(float(frame), float(Xnum)), float(frame / Xnum))) / vec2(float(Xnum), float(Ynum));
}

vec4 getColor(int frame, vec2 uv) {
    return texture2D(histBuf, frameUV(frame, uv));
}

float circle(vec2 uv, float r) {
    float l = length(uv - 0.5);
    return 1.0 - smoothstep(r - 0.05, r + 0.05, l);
}

float circleMask(vec2 uv, float y, float r1, float r2) {
    return circle(uv + vec2(0.0, y - 1.0), r1) + circle(uv + vec2(0.0, y), r2);
}

float rectMask(float b, float w, vec2 uv) {
    vec4 e = smoothstep(vec4(-b - 0.5 * w), vec4(-b + 0.5 * w), vec4(uv, vec2(1.0) - uv));
    return e.x * e.y * e.z * e.w;
}

float getVign(vec2 fragCoord) {
    float rs = length(fragCoord - RENDERSIZE * 0.5) / RENDERSIZE.x;
    return 1.0 - rs * rs * rs;
}

void main() {
    vec2 pos = gl_FragCoord.xy;

    // Frame counter from TIME (since ISF has no iFrame)
    int iFrame = int(TIME * 30.0);
    // How many frames between flips
    int DFrame = int(30.0 * flipInterval);
    if (DFrame < 1) DFrame = 1;

    // Tile size in pixels, scaled proportionally
    float TileSize = tileScale * sqrt(RENDERSIZE.y / 1080.0);

    // ==== PASS 0: Record frame history in tile grid ====
    if (PASSINDEX == 0) {
        vec2 uv0 = pos / RENDERSIZE;
        int fr = int(uv0.x * float(Xnum)) + int(uv0.y * float(Ynum)) * Xnum;
        int slot = int(mod(float(iFrame), float(Xnum * Ynum)));

        if (fr == slot || FRAMEINDEX < 5) {
            // This tile slot is current — write fresh input
            vec2 tileUV = fract(uv0 * vec2(float(Xnum), float(Ynum)));
            gl_FragColor = texture2D(inputImage, fitUV(tileUV * RENDERSIZE));
        } else {
            // Keep previous frame data
            gl_FragColor = texture2D(histBuf, uv0);
        }
        return;
    }

    // ==== PASS 1: Polka dot mosaic composite ====
    int actFrame = (iFrame / DFrame) * DFrame;
    int prevFrame = ((iFrame / DFrame) - 1) * DFrame;

    // Optional grid rotation + scroll — adds the dynamic motion the user wants.
    vec2 mUV = pos / RENDERSIZE;
    if (rotateGrid > 0.001) {
        vec2 c = mUV - 0.5;
        float ra = TIME * rotateGrid * 0.10;
        float ca = cos(ra), sa = sin(ra);
        mUV = 0.5 + vec2(ca * c.x - sa * c.y, sa * c.x + ca * c.y);
    }
    mUV += vec2(scrollX * TIME * 0.05, scrollY * TIME * 0.05);
    vec2 movedPos = mUV * RENDERSIZE;

    // Per-tile random (seeded by tile position + frame cycle)
    vec2 tileIdx = floor(movedPos / TileSize + float(iFrame / DFrame) * 13.0) + 0.5;
    vec4 rand = hash42(tileIdx);

    vec2 uvQ = floor(movedPos / TileSize) * TileSize / RENDERSIZE;
    vec2 uv = movedPos / RENDERSIZE;
    vec2 duv = (uv - uvQ) * RENDERSIZE / TileSize;

    vec4 c1 = getColor(actFrame, uvQ);
    vec4 c2 = getColor(prevFrame, uvQ);

    // Flip animation: stagger per-tile using random offset
    float y = -rand.x * 2.0 + 3.0 * float(iFrame - actFrame) / float(DFrame) * flipSpeed;
    y = clamp(y, 0.0, 1.0);
    y *= y;

    // Pulsing dot size — bass and time both modulate the radius.
    float pulse = 1.0 + sin(TIME * 2.5 + rand.x * 6.28) * dotPulse * 0.4
                + audioBass * audioReact * dotPulse * 0.3;
    float r1 = dotSize * pulse;
    float r2 = dotSize * pulse;

    // Mix between current and previous frame
    vec4 col = mix(c1, c2, smoothstep(y - 0.1, y + 0.1, 1.0 - duv.y));

    // Gap shading between dots
    float cmask = circleMask(duv, y, r1, r2);
    col = mix(col, bgColor, gapShade - gapShade * cmask);

    // Rectangular ambient darkening per tile
    col *= 0.5 + 0.5 * rectMask(0.2 * dot(col.xyz, vec3(0.333)), 0.7, duv);

    // Specular edge highlight on dots
    float spec = clamp(
        circleMask(duv - 0.02, y, r1, r2) - circleMask(duv + 0.02, y, r1, r2),
        -0.4, 1.0
    );
    col.xyz += specAmt * spec;

    // Vignette
    if (vignetteAmt > 0.0) {
        col *= mix(1.0, 1.2 * getVign(pos), vignetteAmt);
    }

    gl_FragColor = col;
    gl_FragColor.w = 1.0;
}
