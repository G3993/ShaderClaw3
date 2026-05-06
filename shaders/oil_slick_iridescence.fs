/*{
  "CATEGORIES": ["Generator", "Atmospheric", "Audio Reactive"],
  "DESCRIPTION": "Thin-film interference colours on a flowing surface — rainbow patches that shift hue as the film thickness varies, computed from real Fresnel-ish thin-film equations. Like a soap bubble or oil-on-water in slow flow, with audio-driven thickness modulation",
  "INPUTS": [
    { "NAME": "filmScale",          "LABEL": "Film Scale",          "TYPE": "float", "MIN": 0.5,   "MAX": 8.0,   "DEFAULT": 2.4 },
    { "NAME": "filmDriftSpeed",     "LABEL": "Drift Speed",         "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.18 },
    { "NAME": "filmThicknessBase",  "LABEL": "Thickness Base (nm)", "TYPE": "float", "MIN": 100.0, "MAX": 900.0, "DEFAULT": 420.0 },
    { "NAME": "filmThicknessRange", "LABEL": "Thickness Range (nm)","TYPE": "float", "MIN": 0.0,   "MAX": 700.0, "DEFAULT": 320.0 },
    { "NAME": "secondaryNoise",     "LABEL": "Secondary Detail",    "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.45 },
    { "NAME": "refractIndex",       "LABEL": "Refraction Index n",  "TYPE": "float", "MIN": 1.0,   "MAX": 2.0,   "DEFAULT": 1.33 },
    { "NAME": "viewAngle",          "LABEL": "View Angle",          "TYPE": "float", "MIN": 0.0,   "MAX": 1.4,   "DEFAULT": 0.0 },
    { "NAME": "audioReact",         "LABEL": "Audio React",         "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0 },
    { "NAME": "hueRotate",          "LABEL": "Hue Rotate",          "TYPE": "float", "MIN": -1.0,  "MAX": 1.0,   "DEFAULT": 0.0 },
    { "NAME": "saturation",         "LABEL": "Saturation",          "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.15 },
    { "NAME": "filmGloss",          "LABEL": "Gloss",               "TYPE": "float", "MIN": 0.0,   "MAX": 1.5,   "DEFAULT": 0.7 },
    { "NAME": "bgColor",            "LABEL": "Background",          "TYPE": "color", "DEFAULT": [0.012, 0.018, 0.028, 1.0] }
  ]
}*/

// ============================================================
// Oil Slick Iridescence
// Real thin-film interference colours on a curl-noise advected surface.
// Colour comes from cos(4 pi n t / lambda) at three primary wavelengths,
// where t is film thickness in nm derived from layered fbm.
// Audio: bass thickens the film globally; treble adds fine ripple.
// ============================================================

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 r = mat2(0.8, -0.6, 0.6, 0.8);
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = r * p * 2.03;
        a *= 0.5;
    }
    return v;
}

// Lower-octave fbm for the curl flow — keeps motion soft & gloopy.
float fbmFlow(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 r = mat2(0.86, -0.51, 0.51, 0.86);
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p = r * p * 2.0;
        a *= 0.55;
    }
    return v;
}

// Curl of a scalar potential -> divergence-free 2D flow.
vec2 curl(vec2 p) {
    float e = 0.08;
    float n1 = fbmFlow(p + vec2(0.0, e));
    float n2 = fbmFlow(p - vec2(0.0, e));
    float n3 = fbmFlow(p + vec2(e, 0.0));
    float n4 = fbmFlow(p - vec2(e, 0.0));
    return vec2(n1 - n2, -(n3 - n4)) / (2.0 * e);
}

// Real interference: reflectance peaks where 2 n t cos(theta) = (m+1/2) lambda.
// Evaluate at primary R/G/B wavelengths -> classic iridescence palette.
vec3 thinFilm(float t_nm, float n, float cosTheta) {
    float opt = 2.0 * n * t_nm * cosTheta;             // optical thickness, nm
    const float TWO_PI = 6.2831853;
    vec3 c = vec3(
        0.5 + 0.5 * cos(TWO_PI * opt / 620.0),         // R 620 nm
        0.5 + 0.5 * cos(TWO_PI * opt / 550.0 + 1.04),  // G 550 nm + small phase
        0.5 + 0.5 * cos(TWO_PI * opt / 470.0 + 2.09)   // B 470 nm + small phase
    );
    // 2nd-order fringes give the deep magenta/teal bands between rainbows.
    c.r = mix(c.r, 0.5 + 0.5 * cos(TWO_PI * opt / 720.0), 0.18);
    c.b = mix(c.b, 0.5 + 0.5 * cos(TWO_PI * opt / 430.0), 0.18);
    return c;
}

vec3 satAdjust(vec3 c, float s) {
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(l), c, s);
}

vec3 hueShift(vec3 c, float turns) {
    if (abs(turns) < 0.001) return c;
    float a = turns * 6.2831853;
    float ca = cos(a), sa = sin(a);
    mat3 m = mat3(
        0.299 + 0.701 * ca + 0.168 * sa, 0.587 - 0.587 * ca + 0.330 * sa, 0.114 - 0.114 * ca - 0.497 * sa,
        0.299 - 0.299 * ca - 0.328 * sa, 0.587 + 0.413 * ca + 0.035 * sa, 0.114 - 0.114 * ca + 0.292 * sa,
        0.299 - 0.300 * ca + 1.250 * sa, 0.587 - 0.588 * ca - 1.050 * sa, 0.114 + 0.886 * ca - 0.203 * sa
    );
    return clamp(m * c, 0.0, 4.0);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 p = uv - 0.5;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    float bass = audioBass * audioReact;
    float trbl = audioHigh * audioReact;
    float midA = audioMid  * audioReact;

    // Curl-noise flow advection: rainbow patches drift on a divergence-free
    // velocity field so the slick keeps moving without sinks/sources.
    float t = TIME * filmDriftSpeed;
    vec2 flow = curl(p * (filmScale * 0.7) + vec2(t * 0.6, -t * 0.4));
    vec2 advected = p + flow * 0.18;

    // Layered thickness fbm: large patches + fine swirl + treble ripple.
    float baseN   = fbm(advected * filmScale + vec2(0.0, t * 1.3));
    vec2  q       = advected * filmScale * 3.1 + curl(advected * filmScale * 1.8 + t) * 0.3;
    float detailN = fbm(q + vec2(t * 0.7, 0.0));
    float ripple  = fbm(advected * filmScale * 7.0 - t * 1.7);

    float thick01 = baseN
                  + (detailN - 0.5) * secondaryNoise * 0.9
                  + (ripple  - 0.5) * (0.15 + trbl * 0.6);

    // Map to nanometres; bass thickens the film globally.
    float t_nm = filmThicknessBase + (thick01 - 0.5) * filmThicknessRange + bass * 140.0;
    t_nm = max(t_nm, 60.0);

    // viewAngle controls cos(theta_t) inside the film -> blue-shifts bands.
    float cosTheta = clamp(cos(viewAngle), 0.25, 1.0);

    vec3 film = thinFilm(t_nm, refractIndex, cosTheta);
    film = satAdjust(film, saturation);
    film = hueShift(film, hueRotate);

    // Cheap surface lighting from local thickness gradient -> wet sheen.
    float gx = fbm(advected * filmScale + vec2(0.01, 0.0)) - baseN;
    float gy = fbm(advected * filmScale + vec2(0.0, 0.01)) - baseN;
    vec3 nrm = normalize(vec3(-gx, -gy, 0.04));
    float spec = pow(max(dot(nrm, normalize(vec3(0.4, 0.7, 0.6))), 0.0), 24.0) * filmGloss;

    // Vignette + audio-mid-modulated film coverage over dark water.
    float vig   = smoothstep(1.05, 0.25, length(p));
    float cover = clamp(0.55 + 0.45 * baseN + midA * 0.15, 0.0, 1.0);
    vec3 col    = mix(bgColor.rgb, film, cover * vig);
    col += spec * vec3(0.85, 0.95, 1.0);

    // Black band where film is very thin (< ~120 nm) — the dark fringe
    // that appears on real soap bubbles just before they pop.
    float blackBand = smoothstep(120.0, 60.0, t_nm);
    col *= mix(1.0, 0.25, blackBand);

    col = col / (1.0 + col * 0.35);
    gl_FragColor = vec4(col, 1.0);
}
