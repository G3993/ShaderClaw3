/*{
  "CATEGORIES": ["Generator", "Optical", "Audio Reactive"],
  "DESCRIPTION": "White-light beam striking a triangular glass prism and splitting into a continuous rainbow spectrum on the other side — Snell's law refraction with wavelength-dependent index gives the iconic Pink Floyd Dark Side cover. The beam can be redirected by audio bass; treble shifts the dispersion factor",
  "INPUTS": [
    { "NAME": "prismCenter",       "LABEL": "Prism Center",      "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] },
    { "NAME": "prismSize",         "LABEL": "Prism Size",        "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.18 },
    { "NAME": "prismRotation",     "LABEL": "Prism Rotation",    "TYPE": "float", "MIN": -3.1416, "MAX": 3.1416, "DEFAULT": 0.0 },
    { "NAME": "beamWidth",         "LABEL": "Beam Width",        "TYPE": "float", "MIN": 0.002, "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "beamBrightness",    "LABEL": "Beam Brightness",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.4 },
    { "NAME": "dispersionAmount",  "LABEL": "Dispersion",        "TYPE": "float", "MIN": 0.0, "MAX": 0.8, "DEFAULT": 0.28 },
    { "NAME": "baseN",             "LABEL": "Base Index n0",     "TYPE": "float", "MIN": 1.0, "MAX": 2.0, "DEFAULT": 1.45 },
    { "NAME": "spreadLength",      "LABEL": "Spread Length",     "TYPE": "float", "MIN": 0.2, "MAX": 1.6, "DEFAULT": 0.85 },
    { "NAME": "audioReact",        "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "bgColor",           "LABEL": "Background",        "TYPE": "color", "DEFAULT": [0.01, 0.01, 0.02, 1.0] },
    { "NAME": "glassTint",         "LABEL": "Glass Tint",        "TYPE": "color", "DEFAULT": [0.55, 0.62, 0.75, 0.18] },
    { "NAME": "rainbowGain",       "LABEL": "Rainbow Gain",      "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.6 }
  ]
}*/

// ---------- helpers ----------
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// distance from p to segment a-b
float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    return length(pa - ba * h);
}

// signed-area test for point in triangle
float insideTriangle(vec2 p, vec2 a, vec2 b, vec2 c) {
    float d1 = (p.x - b.x) * (a.y - b.y) - (a.x - b.x) * (p.y - b.y);
    float d2 = (p.x - c.x) * (b.y - c.y) - (b.x - c.x) * (p.y - c.y);
    float d3 = (p.x - a.x) * (c.y - a.y) - (c.x - a.x) * (p.y - a.y);
    bool hasNeg = (d1 < 0.0) || (d2 < 0.0) || (d3 < 0.0);
    bool hasPos = (d1 > 0.0) || (d2 > 0.0) || (d3 > 0.0);
    return (hasNeg && hasPos) ? 0.0 : 1.0;
}

// Wavelength (nm) → approximate sRGB.  Linear, not gamma-corrected.
vec3 wavelengthToRGB(float w) {
    vec3 c = vec3(0.0);
    if (w >= 380.0 && w < 440.0) c = vec3(-(w - 440.0) / 60.0, 0.0, 1.0);
    else if (w < 490.0)          c = vec3(0.0, (w - 440.0) / 50.0, 1.0);
    else if (w < 510.0)          c = vec3(0.0, 1.0, -(w - 510.0) / 20.0);
    else if (w < 580.0)          c = vec3((w - 510.0) / 70.0, 1.0, 0.0);
    else if (w < 645.0)          c = vec3(1.0, -(w - 645.0) / 65.0, 0.0);
    else if (w <= 780.0)         c = vec3(1.0, 0.0, 0.0);
    // edge attenuation
    float att = 1.0;
    if (w < 420.0)      att = 0.3 + 0.7 * (w - 380.0) / 40.0;
    else if (w > 700.0) att = 0.3 + 0.7 * (780.0 - w) / 80.0;
    return c * att;
}

// 2D Snell refraction. n1*sin(t1) = n2*sin(t2). Returns refracted dir or
// reflects if total internal reflection. nrm must point against incident.
vec2 refract2D(vec2 incident, vec2 nrm, float eta) {
    float cosi = -dot(incident, nrm);
    float k = 1.0 - eta * eta * (1.0 - cosi * cosi);
    if (k < 0.0) return reflect(incident, nrm);
    return eta * incident + (eta * cosi - sqrt(k)) * nrm;
}

// Soft glow along an oriented capsule from A toward dir, length L, half-width w.
// Returns intensity (0..1+).
float beamGlow(vec2 p, vec2 a, vec2 dir, float L, float w) {
    vec2 b = a + dir * L;
    float d = sdSegment(p, a, b);
    return exp(-(d * d) / (w * w));
}

// Ray-segment intersection in 2D. Returns t along ray (>=0) or -1 if miss.
// ray: o + t*d. segment a-b.
float raySegment(vec2 o, vec2 d, vec2 a, vec2 b, out vec2 nrm) {
    vec2 sd = b - a;
    float denom = d.x * sd.y - d.y * sd.x;
    nrm = vec2(0.0);
    if (abs(denom) < 1e-6) return -1.0;
    vec2 oa = a - o;
    float t = (oa.x * sd.y - oa.y * sd.x) / denom;
    float u = (oa.x * d.y - oa.y * d.x) / denom;
    if (t < 1e-4 || u < 0.0 || u > 1.0) return -1.0;
    // outward normal (perp to segment, sign chosen against ray)
    vec2 n = normalize(vec2(sd.y, -sd.x));
    if (dot(n, d) > 0.0) n = -n;
    nrm = n;
    return t;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 p = vec2((uv.x - 0.5) * aspect, uv.y - 0.5);
    vec2 pc = vec2((prismCenter.x - 0.5) * aspect, prismCenter.y - 0.5);

    // ---- Audio surrogate via TIME (no audio uniforms required) ----
    float t = TIME;
    float bass = 0.5 + 0.5 * sin(t * 0.7);          // 0..1
    float treble = 0.5 + 0.5 * sin(t * 1.9 + 1.3);  // 0..1
    float pulse = 0.85 + 0.15 * sin(t * 3.0);

    // ---- Prism geometry (equilateral, slow auto-rotation + bass tilt) ----
    float autoRot = 0.18 * sin(t * 0.25);
    float rot = prismRotation + autoRot + (bass - 0.5) * 0.6 * audioReact;
    mat2 R = rot2(rot);
    vec2 v0 = pc + R * vec2(0.0,           prismSize);
    vec2 v1 = pc + R * vec2(-0.866 * prismSize, -0.5 * prismSize);
    vec2 v2 = pc + R * vec2( 0.866 * prismSize, -0.5 * prismSize);

    vec3 col = bgColor.rgb;

    // ---- Glass body (faint tinted fill + edge highlights) ----
    if (insideTriangle(p, v0, v1, v2) > 0.5) {
        col = mix(col, glassTint.rgb, glassTint.a);
    }
    float edge =
        min(min(sdSegment(p, v0, v1), sdSegment(p, v1, v2)), sdSegment(p, v2, v0));
    col += vec3(0.35, 0.45, 0.6) * exp(-edge * edge / (0.0035 * 0.0035)) * 0.35;

    // ---- Incoming white beam from upper-left ----
    vec2 beamOrigin = vec2(-0.55 * aspect, 0.42);
    vec2 toCenter = normalize(pc - beamOrigin);
    // Aim at left face midpoint so we actually hit the prism.
    vec2 leftMid = 0.5 * (v0 + v1);
    vec2 beamDir = normalize(leftMid - beamOrigin);

    // Find entry point: intersect beam ray with the three edges, take nearest.
    float tEntry = 1e9;
    vec2 nEntry = vec2(0.0);
    vec2 nrm;
    float tt;
    tt = raySegment(beamOrigin, beamDir, v0, v1, nrm);
    if (tt > 0.0 && tt < tEntry) { tEntry = tt; nEntry = nrm; }
    tt = raySegment(beamOrigin, beamDir, v1, v2, nrm);
    if (tt > 0.0 && tt < tEntry) { tEntry = tt; nEntry = nrm; }
    tt = raySegment(beamOrigin, beamDir, v2, v0, nrm);
    if (tt > 0.0 && tt < tEntry) { tEntry = tt; nEntry = nrm; }

    if (tEntry < 1e8) {
        vec2 entryPt = beamOrigin + beamDir * tEntry;

        // White input beam capsule: origin → entryPt
        float inLen = length(entryPt - beamOrigin);
        vec2 inDir = beamDir;
        float gIn = beamGlow(p, beamOrigin, inDir, inLen, beamWidth);
        col += vec3(1.0) * gIn * beamBrightness * pulse;

        // ---- Per-wavelength refraction & exit ----
        const int NW = 7;
        float waves[7];
        waves[0] = 660.0; waves[1] = 610.0; waves[2] = 580.0; waves[3] = 540.0;
        waves[4] = 500.0; waves[5] = 460.0; waves[6] = 410.0;

        float dispK = dispersionAmount * (1.0 + (treble - 0.5) * audioReact);

        for (int i = 0; i < NW; i++) {
            float w = waves[i];
            // Cauchy-style: n(λ) = n0 + k/λ²  (λ in µm for nicer numbers)
            float lamUm = w * 1e-3;
            float n = baseN + dispK * 0.02 / (lamUm * lamUm);

            // Refract into glass at entry.
            float eta1 = 1.0 / n;
            vec2 dirIn = refract2D(inDir, nEntry, eta1);

            // Trace inside prism: find exit edge (the one we didn't enter).
            float tExit = 1e9;
            vec2 nExit = vec2(0.0);
            vec2 q;
            tt = raySegment(entryPt, dirIn, v0, v1, q);
            if (tt > 0.0 && tt < tExit) { tExit = tt; nExit = q; }
            tt = raySegment(entryPt, dirIn, v1, v2, q);
            if (tt > 0.0 && tt < tExit) { tExit = tt; nExit = q; }
            tt = raySegment(entryPt, dirIn, v2, v0, q);
            if (tt > 0.0 && tt < tExit) { tExit = tt; nExit = q; }
            if (tExit > 1e8) continue;

            vec2 exitPt = entryPt + dirIn * tExit;
            // raySegment returns normal oriented against the ray, so it
            // already points back into the prism — pass directly.
            vec2 dirOut = refract2D(dirIn, nExit, n);

            // Render the inside-prism segment (faint, tinted to wavelength).
            vec3 wcol = wavelengthToRGB(w);
            float gMid = beamGlow(p, entryPt, normalize(exitPt - entryPt),
                                  length(exitPt - entryPt), beamWidth * 0.9);
            // Only show inside-prism inside the triangle.
            float insideMask = insideTriangle(p, v0, v1, v2);
            col += wcol * gMid * 0.35 * insideMask;

            // Render the outgoing rainbow ray.
            float gOut = beamGlow(p, exitPt, dirOut, spreadLength, beamWidth);
            col += wcol * gOut * beamBrightness * rainbowGain * (1.0 / float(NW)) * 1.7;
        }
    }

    // ---- subtle vignette ----
    float vig = smoothstep(1.2, 0.4, length(p));
    col *= mix(0.75, 1.0, vig);

    gl_FragColor = vec4(col, 1.0);
}
