/*{
    "DESCRIPTION": "Stained Glass Window — Gothic cathedral jewel-toned panes with black lead lines",
    "CATEGORIES": ["Generator", "Geometry"],
    "CREDIT": "ShaderClaw / Stained Glass v1",
    "INPUTS": [
        { "NAME": "paneRows",        "TYPE": "float", "DEFAULT": 6.0,  "MIN": 2.0,  "MAX": 12.0, "LABEL": "Rows" },
        { "NAME": "paneCols",        "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0,  "MAX": 8.0,  "LABEL": "Columns" },
        { "NAME": "leadWidth",       "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.02, "MAX": 0.20, "LABEL": "Lead Width" },
        { "NAME": "archSharpness",   "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0,  "MAX": 1.50, "LABEL": "Arch Point" },
        { "NAME": "hdrPeak",         "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 5.0,  "LABEL": "Glow HDR" },
        { "NAME": "shimmer",         "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0,  "MAX": 0.40, "LABEL": "Light Shimmer" },
        { "NAME": "pulse",           "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Bass Pulse" },
        { "NAME": "audioReactivity", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio" }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Aspect-corrected centred coordinates
    vec2 uvC = (uv - 0.5) * 2.0;
    uvC.x *= aspect;

    float audio = audioLevel + audioBass * pulse * audioReactivity;

    // Gothic pointed arch: rectangular lower body + tent-function top
    float archW         =  0.65;
    float archYb        = -0.85;   // bottom
    float archYr        =  0.60;   // top of rectangular section
    float archTopCenter = archYr + archSharpness * 0.30;
    float archTop       = archYr + max(0.0,
                              archSharpness * 0.30 * (1.0 - abs(uvC.x) / archW));

    bool inWindow = abs(uvC.x) < archW
                 && uvC.y > archYb
                 && uvC.y < archTop;

    if (!inWindow) {
        // Stone surround — near-void warm dark
        gl_FragColor = vec4(vec3(0.04, 0.03, 0.02), 1.0);
        return;
    }

    // Map window interior to [0,1] pane-grid space
    vec2 paneUV = vec2(
        (uvC.x + archW) / (2.0 * archW),
        (uvC.y - archYb) / (archTopCenter - archYb)
    );

    float pc = floor(paneCols);
    float pr = floor(paneRows);
    vec2 tileUV   = paneUV * vec2(pc, pr);
    vec2 tileIdx  = floor(tileUV);
    vec2 tileFrac = fract(tileUV);

    // Lead lines: smooth black border around each pane edge
    float edgeDist = min(min(tileFrac.x, 1.0 - tileFrac.x),
                         min(tileFrac.y, 1.0 - tileFrac.y));
    float lw     = leadWidth * 0.5;
    float inLead = 1.0 - smoothstep(lw, lw * 2.0, edgeDist);

    // Outer window frame lead (slightly thicker border)
    float borderDist = min(min(paneUV.x, 1.0 - paneUV.x),
                           min(paneUV.y, 1.0 - paneUV.y));
    inLead = max(inLead, 1.0 - smoothstep(0.0, 0.015, borderDist));

    // Per-pane jewel color — 6-hue cathedral palette
    float ph = hash11(tileIdx.x * 7.31 + tileIdx.y * 3.17);
    int ci = int(floor(ph * 6.0));
    vec3 jewel;
    if      (ci == 0) jewel = vec3(0.95, 0.04, 0.08); // ruby
    else if (ci == 1) jewel = vec3(0.04, 0.12, 0.95); // cobalt
    else if (ci == 2) jewel = vec3(0.02, 0.88, 0.15); // emerald
    else if (ci == 3) jewel = vec3(1.00, 0.78, 0.02); // gold
    else if (ci == 4) jewel = vec3(0.62, 0.02, 0.95); // violet
    else              jewel = vec3(0.02, 0.92, 0.98); // cyan

    // Per-pane light shimmer: slow sinusoidal brightness variation
    float shimPhase = hash11(tileIdx.x * 13.7 + tileIdx.y * 5.3) * 6.28318;
    float light     = 1.0 - shimmer + shimmer * sin(TIME * 0.55 + shimPhase);
    float audioBright = 1.0 + audioBass * audioReactivity * 0.35;

    vec3 paneColor = jewel * hdrPeak * light * audioBright;

    // Black lead lines over glowing panes
    vec3 col = mix(paneColor, vec3(0.0), inLead);

    gl_FragColor = vec4(col, 1.0);
}
