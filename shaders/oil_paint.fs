/*{
    "DESCRIPTION": "Azulejo Tiles — Portuguese cobalt-blue geometric ceramic tiling with glaze shimmer",
    "CATEGORIES": ["Generator", "Geometry"],
    "CREDIT": "ShaderClaw / Azulejo v1",
    "INPUTS": [
        { "NAME": "tileSize",        "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.04, "MAX": 0.40, "LABEL": "Tile Size" },
        { "NAME": "groutW",          "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.15, "LABEL": "Grout Width" },
        { "NAME": "arcRadius",       "TYPE": "float", "DEFAULT": 0.37, "MIN": 0.10, "MAX": 0.55, "LABEL": "Arc Radius" },
        { "NAME": "crossW",          "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.03, "MAX": 0.30, "LABEL": "Cross Width" },
        { "NAME": "hdrBlue",         "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5,  "MAX": 5.0,  "LABEL": "Blue HDR" },
        { "NAME": "hdrWhite",        "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.5,  "MAX": 5.0,  "LABEL": "White HDR" },
        { "NAME": "pulse",           "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Bass Pulse" },
        { "NAME": "shimmer",         "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0,  "MAX": 0.25, "LABEL": "Glaze Shimmer" },
        { "NAME": "audioReactivity", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio" }
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uvA = vec2(uv.x * aspect, uv.y);

    float breathe = 1.0 + audioBass * pulse * 0.06;

    // Tile grid
    vec2 tileCoord = uvA / (tileSize * breathe);
    vec2 tileIdx   = floor(tileCoord);
    vec2 lp        = fract(tileCoord);

    // Grout: smooth black border near tile edges
    float groutMask = smoothstep(0.0, groutW,
                        min(min(lp.x, 1.0 - lp.x),
                            min(lp.y, 1.0 - lp.y)));

    // Fold to first quadrant — produces 4-fold symmetric pattern
    vec2 qp = abs(lp - 0.5);

    // Corner quarter-circles: arc centered at each tile corner (abs-folded to (0.5,0.5))
    float arcDist  = length(qp - vec2(0.5, 0.5)) - arcRadius;
    float arcAA    = fwidth(arcDist) * 1.5;
    float cornerPat = smoothstep(arcAA, -arcAA, arcDist);

    // Center cross spanning full tile width and height
    float crossAA  = fwidth(qp.x) * 1.5;
    float crossPat = smoothstep(crossAA, -crossAA,
                        max(qp.x - crossW, qp.y - crossW));

    float pat = max(cornerPat, crossPat);

    // Checkerboard: alternate blue-on-white / white-on-blue
    float inv = mod(tileIdx.x + tileIdx.y, 2.0);
    if (inv > 0.5) pat = 1.0 - pat;

    // Glaze shimmer — slow sinusoidal brightness wave across each tile
    float glaze = 1.0 - shimmer
                + shimmer * sin(lp.x * 6.28318 + TIME * 0.38)
                           * cos(lp.y * 4.50000 + TIME * 0.26);

    // Audio brightness boost
    float audioBright = 1.0 + audioBass * audioReactivity * 0.3;

    vec3 BLUE  = vec3(0.06, 0.18, 1.00) * hdrBlue  * glaze * audioBright;
    vec3 WHITE = vec3(1.00, 0.97, 0.92) * hdrWhite * glaze * audioBright;

    vec3 col = mix(BLUE, WHITE, pat);

    // Grout seam: near-void black
    col = mix(vec3(0.0), col, groutMask);

    gl_FragColor = vec4(col, 1.0);
}
