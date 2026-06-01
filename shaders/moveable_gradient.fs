/*{
	"DESCRIPTION": "Moves a multicolor (up to 8) gradient across the view with a second detail layer and gentle chromatic shimmer",
	"CREDIT": "by Clutchplate, enhanced",
	"ISFVSN": "2.0",
	"CATEGORIES": [
		"TEST-GLSL"
	],
	"INPUTS": [
		{
			"LABEL": "Color 1",
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [1.0,0.2,0.8,1.0]
		},
		{
			"LABEL": "Color 2",
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [0.2,0.6,1.0,1.0]
		},
		{
			"LABEL": "Color 3",
			"NAME": "color3",
			"TYPE": "color",
			"DEFAULT": [0.1,1.0,0.7,1.0]
		},
		{
			"LABEL": "Color 4",
			"NAME": "color4",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Color 5",
			"NAME": "color5",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Color 6",
			"NAME": "color6",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Color 7",
			"NAME": "color7",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Color 8",
			"NAME": "color8",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Thickness",
			"NAME": "thickness",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.01,
			"MAX": 1.0
		},
		{
			"LABEL": "Falloff",
			"NAME": "falloff",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.001,
			"MAX": 0.3
		},
		{
			"LABEL": "Rotation",
			"NAME": "angle",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 360.0
		},
		{
			"LABEL": "Cycles",
			"NAME": "cycles",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 1.0,
			"MAX": 50.0
		},
		{
			"LABEL": "Time",
			"NAME": "efftime",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Start Offscreen",
			"NAME": "offscreen",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"LABEL": "Detail Layer",
			"NAME": "detailAmount",
			"TYPE": "float",
			"DEFAULT": 0.18,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Detail Scale",
			"NAME": "detailScale",
			"TYPE": "float",
			"DEFAULT": 6.0,
			"MIN": 1.0,
			"MAX": 20.0
		},
		{
			"LABEL": "Detail Speed",
			"NAME": "detailSpeed",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 3.0
		},
		{
			"LABEL": "Shimmer Amount",
			"NAME": "shimmerAmount",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": 0.0,
			"MAX": 0.12
		},
		{
			"LABEL": "Shimmer Speed",
			"NAME": "shimmerSpeed",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{
			"LABEL": "Num Colors",
			"NAME": "numColorsInput",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 1.0,
			"MAX": 8.0
		}
	]
}*/

float XL_DURATION = 4.0;
vec4 black = vec4(0.0, 0.0, 0.0, 1.0);

float myFrac(float f) {
    return f < 0.0 ? 1.0 - (-f - floor(-f)) : (f - floor(f));
}

// --- Noise helpers (inspired by spacy caustics) ---
float h21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(h21(i), h21(i + vec2(1.0, 0.0)), f.x),
        mix(h21(i + vec2(0.0, 1.0)), h21(i + vec2(1.0, 1.0)), f.x),
        f.y
    );
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p = p * 2.0 + vec2(3.7, 1.9);
        a *= 0.5;
    }
    return v;
}

// Detail layer: flowing interference pattern (like caustics)
float detailLayer(vec2 uv, float t) {
    float c1 = fbm(uv + vec2(t * 0.31, t * 0.22));
    float c2 = fbm(uv + vec2(-t * 0.27, t * 0.19) + vec2(2.3, 1.5));
    float caustic = c1 * c2;
    caustic = pow(max(0.0, caustic * 4.0), 0.7);
    return caustic;
}

// --- Gradient color lookup ---
vec4 gradientColor(float loc, int numColors) {
    if (numColors == 1) return color1;
    if (numColors == 2) return mix(color1, color2, loc);
    if (numColors == 3) {
        if (loc < 0.5) return mix(color1, color2, loc * 2.0);
        else           return mix(color2, color3, (loc - 0.5) * 2.0);
    }
    if (numColors == 4) {
        if      (loc < 0.3333) return mix(color1, color2, loc * 3.0);
        else if (loc < 0.6667) return mix(color2, color3, (loc - 0.3333) * 3.0);
        else                   return mix(color3, color4, (loc - 0.6667) * 3.0);
    }
    if (numColors == 5) {
        if      (loc < 0.25) return mix(color1, color2, loc * 4.0);
        else if (loc < 0.5)  return mix(color2, color3, (loc - 0.25) * 4.0);
        else if (loc < 0.75) return mix(color3, color4, (loc - 0.5) * 4.0);
        else                 return mix(color4, color5, (loc - 0.75) * 4.0);
    }
    if (numColors == 6) {
        if      (loc < 0.2) return mix(color1, color2, loc * 5.0);
        else if (loc < 0.4) return mix(color2, color3, (loc - 0.2) * 5.0);
        else if (loc < 0.6) return mix(color3, color4, (loc - 0.4) * 5.0);
        else if (loc < 0.8) return mix(color4, color5, (loc - 0.6) * 5.0);
        else                return mix(color5, color6, (loc - 0.8) * 5.0);
    }
    if (numColors == 7) {
        if      (loc < 0.1667) return mix(color1, color2, loc * 6.0);
        else if (loc < 0.3333) return mix(color2, color3, (loc - 0.1667) * 6.0);
        else if (loc < 0.5)    return mix(color3, color4, (loc - 0.3333) * 6.0);
        else if (loc < 0.6667) return mix(color4, color5, (loc - 0.5) * 6.0);
        else if (loc < 0.8333) return mix(color5, color6, (loc - 0.6667) * 6.0);
        else                   return mix(color6, color7, (loc - 0.8333) * 6.0);
    }
    // 8
    if      (loc < 0.1429) return mix(color1, color2, loc * 7.0);
    else if (loc < 0.2857) return mix(color2, color3, (loc - 0.1429) * 7.0);
    else if (loc < 0.4286) return mix(color3, color4, (loc - 0.2857) * 7.0);
    else if (loc < 0.5714) return mix(color4, color5, (loc - 0.4286) * 7.0);
    else if (loc < 0.7143) return mix(color5, color6, (loc - 0.5714) * 7.0);
    else if (loc < 0.8571) return mix(color6, color7, (loc - 0.7143) * 7.0);
    else                   return mix(color7, color8, (loc - 0.8571) * 7.0);
}

// --- Sample gradient at a UV (used for chromatic shimmer) ---
vec4 sampleGradient(vec2 coord, float normTimeInEffect, int numColors) {
    float p2x = coord.x - 0.5;
    float p2y = coord.y - 0.5;
    float rads = angle * 3.1415927 / 180.0;
    float px = p2x * sin(rads) + p2y * cos(rads);

    float loc = (normTimeInEffect + px + 0.5) / thickness;

    vec4 lastColor = color1;
    if (numColors == 2) lastColor = color2;
    else if (numColors == 3) lastColor = color3;
    else if (numColors == 4) lastColor = color4;
    else if (numColors == 5) lastColor = color5;
    else if (numColors == 6) lastColor = color6;
    else if (numColors == 7) lastColor = color7;
    else if (numColors == 8) lastColor = color8;

    if ((loc > 1.0) || (loc < 0.0)) {
        return black;
    } else if (loc <= falloff) {
        return mix(black, color1, loc / falloff);
    } else if (loc > 1.0 - falloff) {
        return mix(lastColor, black, (loc - 1.0 + falloff) / falloff);
    } else {
        float innerLoc = (loc - falloff) / (1.0 - falloff * 2.0);
        return gradientColor(innerLoc, numColors);
    }
}

void main() {
    float time = efftime * XL_DURATION;
    float MYTIME = myFrac((time / XL_DURATION) * cycles);

    float normTimeInEffect;
    if (offscreen) {
        normTimeInEffect = MYTIME * (1.0 + thickness) - 1.0;
    } else {
        normTimeInEffect = myFrac(MYTIME);
    }

    int numColors = int(clamp(numColorsInput, 1.0, 8.0));

    vec2 uv = isf_FragNormCoord.xy;

    // --- Chromatic shimmer: sample R, G, B at slightly offset UVs ---
    float shimmerT = TIME * shimmerSpeed;
    // Animated per-pixel shimmer offset using noise
    vec2 shimmerUV = uv * 3.0 + vec2(shimmerT * 0.17, shimmerT * 0.13);
    float shimmerNoise = vnoise(shimmerUV) * 2.0 - 1.0;
    float shimmer = shimmerAmount * shimmerNoise;

    float rads = angle * 3.1415927 / 180.0;
    vec2 shimmerDir = vec2(cos(rads + 1.5708), sin(rads + 1.5708)); // perpendicular to gradient direction

    vec2 uvR = uv + shimmerDir * shimmer;
    vec2 uvB = uv - shimmerDir * shimmer;

    vec4 colR = sampleGradient(uvR, normTimeInEffect, numColors);
    vec4 colG = sampleGradient(uv,  normTimeInEffect, numColors);
    vec4 colB = sampleGradient(uvB, normTimeInEffect, numColors);

    vec4 fragcolor = vec4(colR.r, colG.g, colB.b, colG.a);

    // --- Detail layer: flowing caustic-like noise overlay ---
    if (detailAmount > 0.0 && fragcolor.a > 0.0) {
        float dt = TIME * detailSpeed;
        vec2 detailUV = uv * detailScale;

        // Layer 1: coarser flow
        float d1 = detailLayer(detailUV, dt);
        // Layer 2: finer, offset, slightly faster
        float d2 = detailLayer(detailUV * 1.7 + vec2(5.3, 2.1), dt * 1.3);

        float detail = d1 * 0.6 + d2 * 0.4;

        // Modulate detail intensity by alpha (only inside gradient band)
        float bandAlpha = fragcolor.a;

        // Add detail as a brightness modulation (preserves hue, adds shimmer)
        float detailMod = 1.0 + (detail - 0.5) * detailAmount * 2.0 * bandAlpha;
        fragcolor.rgb = clamp(fragcolor.rgb * detailMod, 0.0, 1.0);

        // Also add a subtle luminance lift from the detail
        float detailLift = max(0.0, detail - 0.55) * detailAmount * bandAlpha;
        fragcolor.rgb = clamp(fragcolor.rgb + detailLift * 0.4, 0.0, 1.0);
    }

    gl_FragColor = vec4(fragcolor.rgb, 1.0);
}