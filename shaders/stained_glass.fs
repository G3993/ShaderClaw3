/*{
    "DESCRIPTION": "Morphing stained glass window — 3D lead cames, luminous glass panels, image texture support",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "inputImage", "LABEL": "Image", "TYPE": "image" },
        { "NAME": "imageMix", "LABEL": "Image Mix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "cells", "LABEL": "Cells", "TYPE": "float", "DEFAULT": 16.0, "MIN": 3.0, "MAX": 40.0 },
        { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "borderWidth", "LABEL": "Lead Width", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.15 },
        { "NAME": "warp", "LABEL": "Warp", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "hueShift", "LABEL": "Hue Shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "saturation", "LABEL": "Saturation", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 1.5 },
        { "NAME": "glassZoom", "LABEL": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 5.0 },
        { "NAME": "drift", "LABEL": "Drift", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.5 }
    ]
}*/

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec2 cellHash(float n) {
    return fract(sin(vec2(n * 127.1, n * 269.5)) * 43758.5453);
}

float cellHash1(float n) {
    return fract(sin(n * 113.1) * 43758.5453);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0) * glassZoom;

    float t = TIME * speed;

    // Mouse repulsion — applied before warp so it tracks 1:1
    vec2 mp = (mousePos - 0.5) * vec2(aspect, 1.0) * glassZoom;
    vec2 toMouse = p - mp;
    float md = dot(toMouse, toMouse) + 0.1;
    vec2 w = p + toMouse * 1.5 / (md * 3.0 + 0.3);

    // Domain warp — organic flowing shapes
    w += warp * vec2(
        sin(w.y * 1.8 + t * 0.7) * 0.25 + sin(w.y * 0.6 + t * 0.3) * 0.15 + sin(w.x * 1.2 - t * 0.5) * 0.1,
        cos(w.x * 1.8 + t * 0.5) * 0.25 + cos(w.x * 0.7 + t * 0.4) * 0.15 + cos(w.y * 1.3 + t * 0.6) * 0.1
    );

    // Voronoi
    float d1 = 100.0;
    float d2 = 100.0;
    float cId = 0.0;
    vec2 nearest = vec2(0.0);
    vec2 secondNearest = vec2(0.0);
    int nc = int(cells);

    for (int i = 0; i < 40; i++) {
        if (i >= nc) break;
        float fi = float(i);
        vec2 base = cellHash(fi * 17.3) * 2.0 - 1.0;
        base *= glassZoom * 0.55;
        float sx = cellHash1(fi * 31.7);
        float sy = cellHash1(fi * 47.3);
        base += drift * vec2(
            sin(t * (0.4 + sx * 0.4) + fi * 1.3) * 0.35,
            cos(t * (0.35 + sy * 0.5) + fi * 0.9) * 0.35
        );
        float d = length(w - base);
        if (d < d1) {
            d2 = d1; secondNearest = nearest;
            d1 = d; nearest = base; cId = fi;
        } else if (d < d2) {
            d2 = d; secondNearest = base;
        }
    }

    // Edge distance and direction
    float edge = d2 - d1;
    float edgeNorm = clamp(edge / borderWidth, 0.0, 1.0);

    // Direction along the came (perpendicular to the edge bisector)
    vec2 edgeMid = (nearest + secondNearest) * 0.5;
    vec2 edgeVec = normalize(secondNearest - nearest + 0.0001);
    float cameCross = edgeNorm;

    // ========== 3D LEAD CAME ==========
    float cameHeight = sqrt(max(1.0 - cameCross * cameCross, 0.0));
    float slopeXY = -cameCross / (cameHeight + 0.001);
    vec2 slopeDir = normalize(w - edgeMid + 0.0001);
    vec3 cameNormal = normalize(vec3(slopeDir * slopeXY * 0.8, 1.0));

    // Lighting
    vec3 lightDir = normalize(vec3(-0.3, 0.5, 0.9));
    vec3 viewDir = vec3(0.0, 0.0, 1.0);
    vec3 halfVec = normalize(lightDir + viewDir);

    float diff = max(dot(cameNormal, lightDir), 0.0);
    float spec = pow(max(dot(cameNormal, halfVec), 0.0), 40.0);
    float fresnel = pow(1.0 - max(dot(cameNormal, viewDir), 0.0), 3.0);

    vec3 metalBase = vec3(0.06, 0.065, 0.07);
    vec3 metalHighlight = vec3(0.4, 0.42, 0.45);
    vec3 cameColor = metalBase * (0.3 + 0.7 * diff)
                   + metalHighlight * spec * 1.2
                   + metalBase * fresnel * 0.5;

    float cameMask = 1.0 - smoothstep(0.0, 1.0, edgeNorm);

    // ========== GRIP ==========
    // Default to full glass; mouse/pinch pulls toward metallic
    float grip = clamp(max(pinchHold, mouseDown) + audioBass, 0.0, 1.0);
    float glassiness = 1.0 - smoothstep(0.0, 0.6, grip);

    // ========== IMAGE TEXTURE SAMPLING ==========
    // Sample image at cell center UV for per-panel color
    vec2 cellCenterUV = (nearest / (glassZoom * vec2(aspect, 1.0))) + 0.5;
    cellCenterUV = clamp(cellCenterUV, 0.0, 1.0);
    // Also sample at pixel UV for detail within panels
    vec4 imgCellColor = texture2D(inputImage, cellCenterUV);
    vec4 imgPixelColor = texture2D(inputImage, uv);
    // Mix: cell-flat color with some pixel detail for texture
    vec3 imgColor = mix(imgCellColor.rgb, imgPixelColor.rgb, 0.3);

    // ========== PANEL BASE ==========
    float panelTone = fract(cId * 0.618033988749 + hueShift);
    float baseVal = brightness * (0.15 + 0.7 * panelTone);

    vec2 gradDir = normalize(vec2(sin(cId * 4.1 + t * 0.3), cos(cId * 3.3 + t * 0.2)));
    vec2 toPixel = w - nearest;
    float gradPos = dot(toPixel, gradDir) / (d2 + 0.001);
    float gradient = smoothstep(-0.5, 0.5, gradPos);
    float lum = mix(baseVal * 0.2, baseVal * 1.4, gradient);
    lum = clamp(lum, 0.0, 1.0);

    // Metallic base
    vec3 metalCol = vec3(lum);
    float brushAngle = cId * 2.4;
    float bx = w.x * cos(brushAngle) + w.y * sin(brushAngle);
    float brushGrain = 0.85 + 0.15 * sin(bx * 80.0 + cId * 13.0);
    float brushFine = 0.93 + 0.07 * sin(bx * 200.0 + cId * 7.0);
    metalCol *= brushGrain * brushFine;

    // Stained glass color: procedural HSV hues per cell
    float hue = fract(panelTone + hueShift + cId * 0.13);
    float sat = mix(0.6, 0.95, fract(cId * 0.37));
    float val = brightness * mix(0.6, 1.2, gradient);
    vec3 proceduralGlass = hsv2rgb(vec3(hue, mix(sat, sat * 0.85, saturation), val));

    // Blend procedural color with image color (imageMix controls blend directly)
    vec3 glassCol = mix(proceduralGlass, imgColor * brightness * 1.3, imageMix);

    // Beer-Lambert: thicker toward center, lighter at edges
    float thickness = 1.0 - edgeNorm * 0.3;
    glassCol *= thickness;
    // Backlight glow
    float backlight = 0.6 + 0.4 * (1.0 - length(uv - 0.5) * 0.8);
    glassCol *= backlight * 1.3;

    // Blend between metallic and stained glass based on grip
    vec3 baseCol = mix(metalCol, glassCol, glassiness);

    // Panel normal
    float tiltX = sin(cId * 3.7) * 0.2;
    float tiltY = cos(cId * 5.1) * 0.2;
    tiltX += gradDir.x * gradient * 0.15;
    tiltY += gradDir.y * gradient * 0.15;
    vec3 panelNormal = normalize(vec3(tiltX, tiltY, 1.0));

    float panelDiff = max(dot(panelNormal, lightDir), 0.0);
    float panelSpec = pow(max(dot(panelNormal, halfVec), 0.0), 60.0);
    float panelFresnel = pow(1.0 - max(dot(panelNormal, viewDir), 0.0), 4.0);

    float metalInfluence = 1.0 - glassiness * 0.7;
    vec3 glass = baseCol * (0.3 + 0.7 * panelDiff * metalInfluence)
               + vec3(1.0) * panelSpec * 0.9 * metalInfluence
               + baseCol * panelFresnel * 0.5 * metalInfluence;

    glass += glassCol * glassiness * 0.3;

    float windowLight = 1.0 - length(uv - 0.5) * 0.5;
    windowLight = max(windowLight, 0.4);
    glass *= windowLight;

    float cameShadow = smoothstep(0.0, borderWidth * 2.0, edge);
    float shadowDepth = mix(0.55, 0.35, glassiness);
    glass *= shadowDepth + (1.0 - shadowDepth) * cameShadow;

    // ========== COMPOSITE ==========
    vec3 finalCame = mix(cameColor, cameColor * 0.5, glassiness);
    vec3 col = mix(glass, finalCame, cameMask);

    gl_FragColor = vec4(col, 1.0);
}
