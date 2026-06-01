/*{
  "CATEGORIES": ["Generator", "Optical", "Audio Reactive"],
  "DESCRIPTION": "Abstract fluid prism dispersion meets octagonal sound temple — rainbow light bleeds through rotating prismatic geometry while spectral sectors pulse, orbit, and breathe with audio. Snell-law refraction warps into fluid chromatic tendrils orbiting a living core.",
  "INPUTS": [
    { "NAME": "prismCenter",      "LABEL": "Prism Center",     "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0,0.0], "MAX": [1.0,1.0] },
    { "NAME": "prismSize",        "LABEL": "Prism Size",       "TYPE": "float",  "MIN": 0.05, "MAX": 0.50, "DEFAULT": 0.22 },
    { "NAME": "prismRotation",    "LABEL": "Prism Rotation",   "TYPE": "float",  "MIN": -3.1416, "MAX": 3.1416, "DEFAULT": 0.0 },
    { "NAME": "beamWidth",        "LABEL": "Beam Width",       "TYPE": "float",  "MIN": 0.002, "MAX": 0.06, "DEFAULT": 0.016 },
    { "NAME": "beamBrightness",   "LABEL": "Beam Brightness",  "TYPE": "float",  "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.4 },
    { "NAME": "dispersionAmount", "LABEL": "Dispersion",       "TYPE": "float",  "MIN": 0.0,  "MAX": 1.2,  "DEFAULT": 0.38 },
    { "NAME": "baseN",            "LABEL": "Base Index n0",    "TYPE": "float",  "MIN": 1.0,  "MAX": 2.0,  "DEFAULT": 1.45 },
    { "NAME": "spreadLength",     "LABEL": "Spread Length",    "TYPE": "float",  "MIN": 0.2,  "MAX": 2.0,  "DEFAULT": 1.1 },
    { "NAME": "audioReact",       "LABEL": "Audio React",      "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "sectors",          "LABEL": "Sectors",          "TYPE": "float",  "MIN": 4.0,  "MAX": 12.0, "DEFAULT": 8.0 },
    { "NAME": "pillarHeight",     "LABEL": "Pillar Height",    "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.55 },
    { "NAME": "goldGlow",         "LABEL": "Gold Glow",        "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "pulseSpeed",       "LABEL": "Pulse Speed",      "TYPE": "float",  "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 0.6 },
    { "NAME": "pulseWidth",       "LABEL": "Pulse Width",      "TYPE": "float",  "MIN": 0.01, "MAX": 0.25, "DEFAULT": 0.07 },
    { "NAME": "coreSize",         "LABEL": "Core Size",        "TYPE": "float",  "MIN": 0.0,  "MAX": 0.5,  "DEFAULT": 0.13 },
    { "NAME": "fluidWarp",        "LABEL": "Fluid Warp",       "TYPE": "float",  "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "paletteShift",     "LABEL": "Palette Shift",    "TYPE": "float",  "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0 },
    { "NAME": "trail",            "LABEL": "Trail",            "TYPE": "float",  "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.4 },
    { "NAME": "rainbowGain",      "LABEL": "Rainbow Gain",     "TYPE": "float",  "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 2.0 },
    { "NAME": "bgColor",          "LABEL": "Background",       "TYPE": "color",  "DEFAULT": [0.01, 0.01, 0.02, 1.0] },
    { "NAME": "glassTint",        "LABEL": "Glass Tint",       "TYPE": "color",  "DEFAULT": [0.55, 0.62, 0.75, 0.15] }
  ]
}*/

#define TAU 6.28318530718
#define PI  3.14159265358

// ===================== MATH HELPERS =====================

mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    return length(pa - ba * h);
}

float insideTriangle(vec2 p, vec2 a, vec2 b, vec2 c) {
    float d1 = (p.x - b.x)*(a.y - b.y) - (a.x - b.x)*(p.y - b.y);
    float d2 = (p.x - c.x)*(b.y - c.y) - (b.x - c.x)*(p.y - c.y);
    float d3 = (p.x - a.x)*(c.y - a.y) - (c.x - a.x)*(p.y - a.y);
    bool hasNeg = (d1 < 0.0) || (d2 < 0.0) || (d3 < 0.0);
    bool hasPos = (d1 > 0.0) || (d2 > 0.0) || (d3 > 0.0);
    return (hasNeg && hasPos) ? 0.0 : 1.0;
}

// ===================== COLOUR =====================

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 q = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(q - K.xxx, 0.0, 1.0), c.y);
}

vec3 sectorColor(float sec, float total, float shift) {
    return hsv2rgb(vec3(fract(sec / total + shift), 0.80, 1.0));
}

vec3 wavelengthToRGB(float w) {
    vec3 c = vec3(0.0);
    if      (w >= 380.0 && w < 440.0) c = vec3(-(w-440.0)/60.0, 0.0, 1.0);
    else if (w < 490.0)               c = vec3(0.0, (w-440.0)/50.0, 1.0);
    else if (w < 510.0)               c = vec3(0.0, 1.0, -(w-510.0)/20.0);
    else if (w < 580.0)               c = vec3((w-510.0)/70.0, 1.0, 0.0);
    else if (w < 645.0)               c = vec3(1.0, -(w-645.0)/65.0, 0.0);
    else if (w <= 780.0)              c = vec3(1.0, 0.0, 0.0);
    float att = 1.0;
    if      (w < 420.0) att = 0.3 + 0.7*(w-380.0)/40.0;
    else if (w > 700.0) att = 0.3 + 0.7*(780.0-w)/80.0;
    return c * att;
}

// ===================== OPTICS =====================

vec2 refract2D(vec2 incident, vec2 nrm, float eta) {
    float cosi = -dot(incident, nrm);
    float k = 1.0 - eta*eta*(1.0 - cosi*cosi);
    if (k < 0.0) return reflect(incident, nrm);
    return eta*incident + (eta*cosi - sqrt(k))*nrm;
}

float beamGlow(vec2 p, vec2 a, vec2 dir, float L, float w) {
    vec2 b = a + dir * L;
    float d = sdSegment(p, a, b);
    return exp(-(d*d)/(w*w));
}

float raySegment(vec2 o, vec2 d, vec2 a, vec2 b, out vec2 nrm) {
    vec2 sd = b - a;
    float denom = d.x*sd.y - d.y*sd.x;
    nrm = vec2(0.0);
    if (abs(denom) < 1e-6) return -1.0;
    vec2 oa = a - o;
    float t = (oa.x*sd.y - oa.y*sd.x) / denom;
    float u = (oa.x*d.y  - oa.y*d.x)  / denom;
    if (t < 1e-4 || u < 0.0 || u > 1.0) return -1.0;
    vec2 n = normalize(vec2(sd.y, -sd.x));
    if (dot(n, d) > 0.0) n = -n;
    nrm = n;
    return t;
}

// ===================== FLUID DOMAIN WARP =====================

// Two-octave domain warp — gives the fluid, smeared quality.
vec2 fluidOffset(vec2 p, float t, float amount) {
    float a1 = sin(p.y*3.1 + t*0.7) * cos(p.x*2.3 - t*0.5);
    float a2 = cos(p.x*4.7 - t*0.9) * sin(p.y*3.9 + t*0.6);
    float b1 = sin(p.x*5.3 + p.y*4.1 + t*1.1);
    float b2 = cos(p.y*6.1 - p.x*3.7 - t*0.8);
    return vec2(a1 + 0.5*b1, a2 + 0.5*b2) * amount * 0.07;
}

// ===================== MAIN =====================

void main() {
    float t = TIME;

    // Audio-reactive values from builtins
    float bass   = clamp(audioBass  * audioReact, 0.0, 1.0);
    float treble = clamp(audioHigh  * audioReact, 0.0, 1.0);
    float mid    = clamp(audioMid   * audioReact, 0.0, 1.0);
    float pulse  = 0.85 + 0.15*sin(t*3.0) + bass*0.2;

    // Screen coords
    vec2 uv     = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Two coordinate systems: prism uses aspect-corrected XY, temple uses
    // uniform-scale polar so the ring stays circular.
    vec2 p      = vec2((uv.x - 0.5)*aspect, uv.y - 0.5);
    vec2 pTemple = (gl_FragCoord.xy - 0.5*RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y) * 2.0;

    // Domain warp for fluid feel (applied to BOTH coordinate systems)
    float warpAmt = fluidWarp * (1.0 + bass*0.6);
    vec2 warpOff  = fluidOffset(p, t, warpAmt);
    vec2 pw       = p       + warpOff;
    vec2 pwT      = pTemple + fluidOffset(pTemple, t*1.1, warpAmt*0.8);

    // Polar coords for temple (warped)
    float r  = length(pwT);
    float th = atan(pwT.y, pwT.x);

    // ---- Prism geometry ----
    vec2 pc = vec2((prismCenter.x - 0.5)*aspect, prismCenter.y - 0.5);
    float autoRot = 0.18*sin(t*0.25) + bass*0.4*audioReact;
    float rot = prismRotation + autoRot;
    mat2  R   = rot2(rot);

    vec2 v0 = pc + R * vec2(0.0,              prismSize);
    vec2 v1 = pc + R * vec2(-0.866*prismSize, -0.5*prismSize);
    vec2 v2 = pc + R * vec2( 0.866*prismSize, -0.5*prismSize);

    // Start with background
    vec3 col = bgColor.rgb;

    // ===================== TEMPLE LAYER =====================

    float secs = max(4.0, sectors);
    float sec  = floor((th + PI) / TAU * secs);
    float secF = sec / secs;

    // Pillar lattice
    float pCount = secs;
    float pAng   = (th + PI) / TAU * pCount;
    float pFrac  = abs(fract(pAng) - 0.5) * 2.0;
    float pillarMask = smoothstep(0.55, 0.95, pFrac);
    float pillarBand = smoothstep(1.08, 0.25, r);
    float pillar = pillarMask * pillarBand * pillarHeight;

    // Amplitude from builtins (no FFT texture needed)
    float bandT  = sec / secs;
    float amp    = 0.3 + 0.35*sin(bandT*TAU + t*2.1)
                       + 0.35*(audioBass*smoothstep(0.0,0.25,bandT)
                             + audioMid *smoothstep(0.25,0.6,bandT)
                             + audioHigh*smoothstep(0.6,1.0,bandT));
    amp = clamp(amp, 0.0, 2.0);
    float punchF = 1.0 + bass*bass*2.0;
    amp *= punchF;

    // Travelling pulse
    float pulsePhase = fract(t*pulseSpeed - secF);
    float pulseSig   = smoothstep(pulseWidth, 0.0, abs(r - pulsePhase));
    float trailPhase = fract(t*pulseSpeed - secF - 0.12);
    float trailSig   = smoothstep(pulseWidth*2.0, 0.0, abs(r - trailPhase)) * trail;

    // Seam
    float seamFrac = fract((th + PI) / TAU * secs);
    float seam     = 1.0 - smoothstep(0.0, 0.015*secs, min(seamFrac, 1.0-seamFrac));

    // Sector hue — tinted by wavelength spectrum bands + paletteShift
    vec3 hue = sectorColor(sec, secs, paletteShift);
    // Blend sector hue toward prism wavelength colour for coherence
    float waveBlend = fract(sec / secs);
    float waveNm    = mix(410.0, 660.0, waveBlend);
    vec3  waveHue   = wavelengthToRGB(waveNm);
    hue = mix(hue, waveHue, 0.5);

    vec3 templeCol = hue * (pulseSig + trailSig) * (0.4 + amp*1.6);
    templeCol += hue * 0.03;

    // Gold pillars
    vec3  goldHue    = vec3(1.35, 0.95, 0.45);
    float pillarLit  = pillar * (0.5 + amp*1.3);
    templeCol += goldHue * pillarLit;
    float rim  = smoothstep(0.78, 0.98, pFrac) * pillarBand;
    templeCol += goldHue * rim * goldGlow * (1.4 + bass*2.5);
    templeCol += vec3(0.95) * seam * 0.14;

    // Centre core breathing
    float core = smoothstep(coreSize, coreSize*0.45, r);
    templeCol += vec3(1.0, 0.96, 0.92) * core * (0.5 + bass*1.8*punchF);

    // Radial outer falloff
    templeCol *= smoothstep(1.3, 0.85, r);

    col += templeCol;

    // ===================== PRISM + GLASS BODY =====================

    // Use warped coords for glass body and edges
    if (insideTriangle(pw, v0, v1, v2) > 0.5) {
        col = mix(col, glassTint.rgb + templeCol*0.15, glassTint.a);
    }
    float edge = min(min(sdSegment(pw, v0, v1), sdSegment(pw, v1, v2)), sdSegment(pw, v2, v0));
    col += vec3(0.35, 0.5, 0.75) * exp(-edge*edge/(0.004*0.004)) * 0.4;

    // ===================== BEAM + DISPERSION =====================

    vec2 beamOrigin = vec2(-0.55*aspect, 0.38 + sin(t*0.31)*0.04 + bass*0.05);
    vec2 leftMid    = 0.5*(v0 + v1);
    vec2 beamDir    = normalize(leftMid - beamOrigin);

    // Find entry intersection (use unwarped p for ray geometry)
    float tEntry = 1e9;
    vec2  nEntry = vec2(0.0);
    vec2  nrm;
    float tt;
    tt = raySegment(beamOrigin, beamDir, v0, v1, nrm);
    if (tt > 0.0 && tt < tEntry) { tEntry = tt; nEntry = nrm; }
    tt = raySegment(beamOrigin, beamDir, v1, v2, nrm);
    if (tt > 0.0 && tt < tEntry) { tEntry = tt; nEntry = nrm; }
    tt = raySegment(beamOrigin, beamDir, v2, v0, nrm);
    if (tt > 0.0 && tt < tEntry) { tEntry = tt; nEntry = nrm; }

    if (tEntry < 1e8) {
        vec2 entryPt = beamOrigin + beamDir * tEntry;

        // Incoming white beam — sample warped coords for glow
        float inLen = length(entryPt - beamOrigin);
        float gIn   = beamGlow(pw, beamOrigin, beamDir, inLen, beamWidth);
        col += vec3(1.0) * gIn * beamBrightness * pulse;

        // Extra wide halo for dreamy fluid look
        float gHalo = beamGlow(pw, beamOrigin, beamDir, inLen, beamWidth*5.0);
        col += vec3(0.6, 0.75, 1.0) * gHalo * 0.18 * beamBrightness;

        // Per-wavelength refraction
        float dispK = dispersionAmount * (1.0 + treble*audioReact*0.8);

        const int NW = 9;
        float waves[9];
        waves[0] = 660.0; waves[1] = 625.0; waves[2] = 590.0;
        waves[3] = 555.0; waves[4] = 520.0; waves[5] = 490.0;
        waves[6] = 460.0; waves[7] = 430.0; waves[8] = 405.0;

        for (int i = 0; i < NW; i++) {
            float w      = waves[i];
            float lamUm  = w * 1e-3;
            float n      = baseN + dispK * 0.022 / (lamUm*lamUm);
            float eta1   = 1.0 / n;
            vec2  dirIn  = refract2D(beamDir, nEntry, eta1);

            // Exit intersection
            float tExit = 1e9;
            vec2  nExit = vec2(0.0);
            vec2  q;
            tt = raySegment(entryPt, dirIn, v0, v1, q);
            if (tt > 0.0 && tt < tExit) { tExit = tt; nExit = q; }
            tt = raySegment(entryPt, dirIn, v1, v2, q);
            if (tt > 0.0 && tt < tExit) { tExit = tt; nExit = q; }
            tt = raySegment(entryPt, dirIn, v2, v0, q);
            if (tt > 0.0 && tt < tExit) { tExit = tt; nExit = q; }
            if (tExit > 1e8) continue;

            vec2 exitPt = entryPt + dirIn * tExit;
            vec2 dirOut = refract2D(dirIn, nExit, n);

            // Warp exit direction slightly for fluid smear
            float warpAngle = sin(t*1.3 + float(i)*0.7) * warpAmt * 0.18;
            dirOut = rot2(warpAngle) * dirOut;

            vec3  wcol = wavelengthToRGB(w);
            float wt   = 1.0 / float(NW);

            // Inside-prism (warped coords)
            float gMid = beamGlow(pw, entryPt, normalize(exitPt - entryPt),
                                  length(exitPt - entryPt), beamWidth*1.1);
            float inMask = insideTriangle(pw, v0, v1, v2);
            col += wcol * gMid * 0.45 * inMask;

            // Outgoing rainbow — wide soft core + narrow bright spine
            float gOut     = beamGlow(pw, exitPt, dirOut, spreadLength, beamWidth);
            float gOutSoft = beamGlow(pw, exitPt, dirOut, spreadLength, beamWidth*4.0);
            float audioBoost = 1.0 + mid*1.5 + bass*0.8;
            col += wcol * (gOut * beamBrightness * rainbowGain * wt * 2.0
                         + gOutSoft * 0.25) * audioBoost;

            // Interference fringes — thin iridescent bands across the spread
            float along = dot(pw - exitPt, dirOut);
            float fringe = 0.5 + 0.5*sin(along*120.0 + t*2.0 + float(i)*0.9);
            float fringeMask = gOutSoft * fringe * 0.12;
            col += wcol * fringeMask;
        }
    }

    // ===================== GLOBAL FX =====================

    // Subtle chromatic vignette
    float vig = smoothstep(1.25, 0.38, length(p));
    col *= mix(0.6, 1.0, vig);

    // Gentle overall bloom — add a blurred copy of col in hue space
    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    col += col * lum * 0.12;

    // Tone map to avoid harsh clipping
    col = col / (col + vec3(0.75));
    col = pow(clamp(col, 0.0, 1.0), vec3(0.88));

    gl_FragColor = vec4(col, 1.0);
}