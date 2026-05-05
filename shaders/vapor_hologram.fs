/*{
  "DESCRIPTION": "Retro 8-bit Sunset — CGA/EGA pixel art vaporwave: chunky color blocks, pixelated sun stripes, hard-edged grid floor. 80s computer aesthetic.",
  "CREDIT": "ShaderClaw auto-improve v15",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Glitch"],
  "INPUTS": [
    { "NAME": "pixelSize",    "LABEL": "Pixel Size",    "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 16.0 },
    { "NAME": "horizonY",     "LABEL": "Horizon",       "TYPE": "float", "DEFAULT": 0.52, "MIN": 0.3,  "MAX": 0.7 },
    { "NAME": "sunBands",     "LABEL": "Sun Bands",     "TYPE": "float", "DEFAULT": 8.0,  "MIN": 2.0,  "MAX": 20.0 },
    { "NAME": "gridDensity",  "LABEL": "Grid Lines",    "TYPE": "float", "DEFAULT": 10.0, "MIN": 4.0,  "MAX": 24.0 },
    { "NAME": "hdrPeak",      "LABEL": "HDR Peak",      "TYPE": "float", "DEFAULT": 2.2,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioMod",     "LABEL": "Audio Mod",     "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

void main() {
    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.3) * audioMod;

    // Pixelate UV
    vec2 res = RENDERSIZE;
    float px = pixelSize;
    vec2 pixelRes = floor(res / px);
    vec2 pixUV = floor(isf_FragNormCoord.xy * pixelRes) / pixelRes;
    vec2 uv = pixUV;
    float aspect = res.x / res.y;

    // CGA 4-color palette per region: magenta, cyan, black, white (CGA palette 1)
    // Extended for vaporwave: hot pink, cyan, deep purple, orange, black
    // Using hard indexed palette
    vec3 P[5];
    P[0] = vec3(1.0, 0.07, 0.55);  // hot magenta
    P[1] = vec3(0.0, 0.85, 1.0);   // electric cyan
    P[2] = vec3(0.12, 0.0, 0.30);  // deep purple
    P[3] = vec3(1.0, 0.40, 0.0);   // chrome orange
    P[4] = vec3(0.0, 0.0, 0.0);    // black

    vec3 col;

    if (uv.y >= horizonY) {
        // Sky: horizontal dithered bands of magenta→purple
        float skyBand = floor((uv.y - horizonY) / (1.0 - horizonY) * 6.0);
        if (skyBand < 2.0) col = P[0];       // hot pink sky
        else if (skyBand < 4.0) col = P[2];  // purple mid-sky
        else col = P[4];                      // black zenith

        // Pixelated sun
        vec2 sunCenter = vec2(0.5, horizonY + 0.18);
        vec2 sd = vec2((uv.x - sunCenter.x) * aspect, uv.y - sunCenter.y);
        float sunR = 0.15;
        float inSun = step(length(sd), sunR);
        if (inSun > 0.5) {
            // Sun bands: alternating orange and black
            float bandFrac = (uv.y - (horizonY + 0.02)) / (sunR * 1.6);
            float band = floor(bandFrac * sunBands);
            float bandMod = mod(band, 2.0);
            // Animate bands scrolling down slowly
            float animBand = floor((bandFrac + t * 0.05) * sunBands);
            col = (mod(animBand, 2.0) < 1.0) ? P[3] : P[4]; // orange or black
            // Top of sun: bright cyan stripe
            if (uv.y > horizonY + sunR * 1.2) col = P[1];
        }
    } else {
        // Floor: perspective grid
        float dh = max(horizonY - uv.y, 0.001);
        float persp = 1.0 / (dh * 8.0 + 0.1);

        // Grid lines (hard-edged)
        float gx = fract((uv.x - 0.5) * persp * gridDensity) ;
        float gy = fract(persp - t * 0.3);
        float lineX = step(0.92, gx) + step(0.92, 1.0 - gx);
        float lineY = step(0.90, gy) + step(0.90, 1.0 - gy);
        float onLine = clamp(lineX + lineY, 0.0, 1.0);

        // Floor color zones
        float floorBand = floor(uv.y / horizonY * 4.0);
        if (floorBand < 2.0) col = P[2];  // dark purple near horizon
        else col = P[4];                   // black near bottom

        // Grid lines: cyan or magenta alternating
        if (onLine > 0.5) {
            col = (mod(floor((uv.x - 0.5) * persp * gridDensity), 2.0) < 1.0) ? P[1] : P[0];
        }
    }

    // HDR boost on bright colors (not black)
    float lum = max(col.r, max(col.g, col.b));
    vec3 finalCol = col * hdrPeak * audio * (lum > 0.05 ? 1.0 : 0.0);
    finalCol += col * max(lum - 0.3, 0.0) * hdrPeak * 0.5; // extra boost on bright

    // Hard scanlines (every 2 pixel rows)
    float scanline = mod(floor(isf_FragNormCoord.y * pixelRes.y), 2.0);
    finalCol *= 0.85 + 0.15 * scanline;

    gl_FragColor = vec4(finalCol, 1.0);
}
