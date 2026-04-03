/*{
    "DESCRIPTION": "Stained glass — adaptive Voronoi subdivision with glass refraction, lead cames, and backlit luminance",
    "CREDIT": "Inspired by flockaroo (Shadertoy WsS3Dc), adapted for ISF by ShaderClaw",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "inputImage", "LABEL": "Image", "TYPE": "image" },
        { "NAME": "imageMix", "LABEL": "Image Mix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "subdivisions", "LABEL": "Detail", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 8.0 },
        { "NAME": "detailThresh", "LABEL": "Threshold", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.01, "MAX": 0.5 },
        { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "leadWidth", "LABEL": "Lead Width", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 8.0 },
        { "NAME": "glassDepth", "LABEL": "Glass Depth", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "refraction", "LABEL": "Refraction", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 2.0 },
        { "NAME": "hueShift", "LABEL": "Hue Shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "saturation", "LABEL": "Saturation", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "vignette", "LABEL": "Vignette", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
    ]
}*/

// --- Hash functions ---
vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// Smooth value noise for glass surface
float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(i), hash12(i + vec2(1, 0)), f.x),
               mix(hash12(i + vec2(0, 1)), hash12(i + vec2(1, 1)), f.x), f.y);
}

// FBM noise for glass surface distortion
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p *= 2.1;
        a *= 0.5;
    }
    return v;
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Aspect-correct texture UV
vec2 fitUV(vec2 pos) {
    return (pos - 0.5 * RENDERSIZE) * min(IMG_SIZE_inputImage.y / RENDERSIZE.y, IMG_SIZE_inputImage.x / RENDERSIZE.x) / IMG_SIZE_inputImage + 0.5;
}

// Adaptive Voronoi: recursively subdivide cells where color contrast is high
// Returns: xy = nearest cell center, z = edge distance, w = cell scale
// Voronoi output stored in globals (GLSL ES has no struct returns in all implementations)
vec2 vor_nearest, vor_second;
float vor_cellId, vor_edgeDist, vor_cellScale;

// Get color at a cell position for subdivision decisions
vec3 sampleColor(vec2 cellUV) {
    if (imageMix > 0.01) {
        return texture2D(inputImage, fitUV(cellUV)).rgb;
    }
    float h = fract(hash12(floor(cellUV / 40.0)) + hueShift);
    return hsv2rgb(vec3(h, saturation, 0.7));
}

float colorContrast(vec2 c1, vec2 c2) {
    vec3 col1 = sampleColor(c1);
    vec3 col2 = sampleColor(c2);
    return abs(dot(col1 - col2, vec3(0.333)));
}

void adaptiveVoronoi(vec2 pos) {
    float t = TIME * speed;
    float SC = RENDERSIZE.x / 600.0;
    int maxLevel = int(subdivisions);

    // Start with a coarse grid
    vec2 cellSize = RENDERSIZE / 4.0;
    vec2 cellPos = (floor(pos / cellSize) + 0.5) * cellSize;

    // Adaptive subdivision: split cells where there's color contrast
    for (int level = 0; level < 8; level++) {
        if (level >= maxLevel) break;

        vec2 halfCell = cellSize * 0.5;
        vec2 quadrant = step(cellPos, pos) * 2.0 - 1.0;
        vec2 subCenter = cellPos + quadrant * halfCell * 0.5;
        vec2 neighborCenter = cellPos - quadrant * halfCell * 0.5;
        float contrast = colorContrast(subCenter, neighborCenter);

        if (contrast < detailThresh) break;

        cellSize *= 0.5;
        cellPos = subCenter;
    }

    // Voronoi within the final cell grid: check 3x3 neighborhood
    float d1 = 100000.0, d2 = 100000.0;
    vec2 nearest = pos, second = pos;
    float cellId = 0.0;

    for (int j = 0; j < 9; j++) {
        int x = j / 3 - 1;
        int y = int(mod(float(j), 3.0)) - 1;
        vec2 neighbor = cellPos + vec2(float(x), float(y)) * cellSize;
        vec2 gridCell = floor(neighbor / cellSize);

        vec2 jitter = hash22(gridCell * 7.3 + 0.5) - 0.5;
        jitter += 0.3 * vec2(
            sin(t * (0.4 + jitter.x * 0.6) + gridCell.x * 1.3),
            cos(t * (0.35 + jitter.y * 0.5) + gridCell.y * 0.9)
        );

        vec2 point = (gridCell + 0.5 + jitter * 0.45) * cellSize;
        float d = length(pos - point);

        if (d < d1) {
            d2 = d1; second = nearest;
            d1 = d; nearest = point;
            cellId = hash12(gridCell * 17.3);
        } else if (d < d2) {
            d2 = d; second = point;
        }
    }

    vor_nearest = nearest;
    vor_second = second;
    vor_cellId = cellId;
    vor_edgeDist = abs(dot(pos - (nearest + second) * 0.5, normalize(nearest - second + 0.001)));
    vor_cellScale = length(cellSize);
}

void main() {
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = pos / RENDERSIZE;
    float SC = RENDERSIZE.x / 600.0;
    float t = TIME * speed;

    // Apply subtle domain jitter for organic feel
    vec2 jitterPos = pos + 1.5 * sqrt(SC) * (vec2(
        fbm(pos * 0.005 + t * 0.1) - 0.5,
        fbm(pos * 0.005 + vec2(7.0, 3.0) + t * 0.1) - 0.5
    ));

    adaptiveVoronoi(jitterPos);

    float leadMask = smoothstep(leadWidth * SC, leadWidth * SC * 0.3, vor_edgeDist);

    // ========== GLASS PANEL ==========
    // Glass surface normal from noise (for refraction and lighting)
    vec2 glassGrad = vec2(
        fbm(pos * 0.03 / sqrt(SC) + vec2(17.5, 0.0)) - fbm(pos * 0.03 / sqrt(SC) - vec2(17.5, 0.0)),
        fbm(pos * 0.03 / sqrt(SC) + vec2(0.0, 13.5)) - fbm(pos * 0.03 / sqrt(SC) - vec2(0.0, 13.5))
    );
    vec3 glassNormal = normalize(vec3(glassGrad * glassDepth * 2.0, 1.0));

    // View and light
    vec2 scr = uv * 2.0 - 1.0;
    vec3 viewDir = normalize(vec3(scr, -2.0));
    vec3 lightDir = normalize(vec3(-0.3, 0.5, 1.4));
    vec3 halfVec = normalize(lightDir + vec3(0.0, 0.0, 1.0));

    // Refracted backlight sampling
    vec3 refracted = refract(viewDir, glassNormal, 1.0 / 1.5);
    vec3 reflected = reflect(viewDir, glassNormal);

    // Panel color: procedural or from image — one flat color per cell
    vec3 panelColor;
    if (imageMix > 0.01) {
        panelColor = texture2D(inputImage, fitUV(vor_nearest)).rgb * brightness;
    } else {
        float hue = fract(vor_cellId * 0.618 + hueShift);
        float sat = mix(0.5, 0.95, fract(vor_cellId * 3.7)) * saturation;
        float val = brightness * mix(0.6, 1.0, fract(vor_cellId * 2.3));
        panelColor = hsv2rgb(vec3(hue, sat, val));
    }

    // Flat glass color with subtle AO near lead edges
    vec3 glassColor = panelColor;
    float ao = 0.5 + 0.5 * smoothstep(0.0, 4.0 * SC, vor_edgeDist);
    ao *= 0.7 + 0.3 * smoothstep(0.0, 2.0 * SC, vor_edgeDist);
    glassColor *= ao;

    // ========== LEAD CAME ==========
    vec3 leadNormal = normalize(vec3(
        -vor_edgeDist * normalize(pos - (vor_nearest + vor_second) * 0.5 + 0.001) * 0.8,
        1.0
    ));
    float leadDiff = max(dot(leadNormal, lightDir), 0.0);
    float leadSpec = pow(max(dot(leadNormal, halfVec), 0.0), 20.0);
    vec3 leadBase = vec3(0.06, 0.065, 0.07);
    vec3 leadColor = leadBase * (0.3 + 0.7 * leadDiff) + vec3(0.4, 0.42, 0.45) * leadSpec * 0.8;

    // ========== COMPOSITE ==========
    vec3 col = mix(glassColor, leadColor, leadMask);

    // Vignette
    if (vignette > 0.0) {
        vec2 scc = (pos - 0.5 * RENDERSIZE) / RENDERSIZE.x;
        float v = 1.0 - vignette * 0.7 * dot(scc, scc);
        v *= 1.0 - vignette * 0.7 * exp(-sin(uv.x * 3.1416) * 20.0);
        v *= 1.0 - vignette * 0.7 * exp(-sin(uv.y * 3.1416) * 10.0);
        col *= v;
    }

    gl_FragColor = vec4(col, 1.0);
}
