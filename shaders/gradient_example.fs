/*{
	"DESCRIPTION": "Volumetric gradient — two-stop color sampled through a 3D fbm noise field. Camera flies through the field giving depth and parallax. Bright crests punch into HDR (linear). Audio drives flow speed and crest intensity. Output linear HDR.",
	"CREDIT": "by VIDVOX, rebuilt as volumetric noise field",
	"ISFVSN": "2.0",
	"CATEGORIES": [
		"Examples",
		"Generator",
		"Audio Reactive"
	],
	"INPUTS": [
		{
			"NAME": "colorA",
			"LABEL": "Top Color",
			"TYPE": "color",
			"DEFAULT": [0.05, 0.08, 0.22, 1.0]
		},
		{
			"NAME": "colorB",
			"LABEL": "Bottom Color",
			"TYPE": "color",
			"DEFAULT": [0.95, 0.42, 0.18, 1.0]
		},
		{
			"NAME": "noiseScale",
			"LABEL": "Noise Scale",
			"TYPE": "float",
			"MIN": 0.4,
			"MAX": 6.0,
			"DEFAULT": 1.8
		},
		{
			"NAME": "flowSpeed",
			"LABEL": "Flow Speed",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 0.35
		},
		{
			"NAME": "depthIntensity",
			"LABEL": "Depth Intensity",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "audioReact",
			"LABEL": "Audio React",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 1.0
		}
	]
}*/

// ════════════════════════════════════════════════════════════════════════
//   VOLUMETRIC GRADIENT
//   Anadol-style depth: a 3D noise field exists in space; the camera
//   flies forward through it. The gradient lerp factor is modulated by
//   multi-octave fbm sampled along view rays. Bright crests can punch
//   into HDR (linear, up to ~1.6).
// ════════════════════════════════════════════════════════════════════════

#define V_STEPS 14
#define PI 3.14159265

// ---------- hash + value noise -----------------------------------------
float hash13(vec3 p) {
    return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453);
}

float vnoise3(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n000 = hash13(i + vec3(0.0, 0.0, 0.0));
    float n100 = hash13(i + vec3(1.0, 0.0, 0.0));
    float n010 = hash13(i + vec3(0.0, 1.0, 0.0));
    float n110 = hash13(i + vec3(1.0, 1.0, 0.0));
    float n001 = hash13(i + vec3(0.0, 0.0, 1.0));
    float n101 = hash13(i + vec3(1.0, 0.0, 1.0));
    float n011 = hash13(i + vec3(0.0, 1.0, 1.0));
    float n111 = hash13(i + vec3(1.0, 1.0, 1.0));
    return mix(
        mix(mix(n000, n100, f.x), mix(n010, n110, f.x), f.y),
        mix(mix(n001, n101, f.x), mix(n011, n111, f.x), f.y),
        f.z
    );
}

// 5-octave fbm — Anadol-style soft cumulus structure
float fbm3(vec3 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise3(p);
        p = p * 2.03 + vec3(11.7, 5.3, 17.1);
        a *= 0.5;
    }
    return v;
}

void main() {
    // ---------- normalized coords with aspect correction ---------------
    vec2 uv = isf_FragNormCoord;
    vec2 p = uv * 2.0 - 1.0;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    // ---------- audio drivers ------------------------------------------
    float bass = audioBass * audioReact;
    float mid = audioMid * audioReact;
    float treb = audioHigh * audioReact;
    float lvl = audioLevel * audioReact;

    // ---------- camera "flies" through the field -----------------------
    // z grows with time → parallax through volume. Audio bass nudges
    // forward velocity; sway gives subtle drift.
    float t = TIME * flowSpeed;
    float zPos = t * (1.0 + 0.6 * bass);
    vec3 camOff = vec3(
        sin(t * 0.31) * 0.6,
        cos(t * 0.27) * 0.4,
        zPos
    );

    // View ray — perspective: each pixel maps to a ray direction.
    vec3 rd = normalize(vec3(p, 1.4));

    // ---------- raymarch noise field (cheap, no opacity accum) ---------
    // We sample a few depth slices along rd and average — this gives the
    // illusion of looking THROUGH the field. Far slices fade out.
    float density = 0.0;
    float weightSum = 0.0;
    float maxCrest = 0.0;

    // start a little in front of camera, walk forward
    float tNear = 0.4;
    float tFar = 3.6;

    for (int i = 0; i < V_STEPS; i++) {
        float fi = float(i) / float(V_STEPS - 1);
        float zr = mix(tNear, tFar, fi);
        vec3 wp = camOff + rd * zr;

        // sample fbm at this world point
        float n = fbm3(wp * noiseScale);

        // far-plane fade — depth cue (atmospheric perspective)
        float depthFade = 1.0 - smoothstep(0.6, 1.0, fi);
        float w = depthFade;

        density += n * w;
        weightSum += w;
        maxCrest = max(maxCrest, n * depthFade);
    }
    density /= max(weightSum, 0.001);

    // Sharpen the field a little — gives crests definition
    float field = clamp(density * 1.4 - 0.15, 0.0, 1.2);

    // ---------- gradient mix factor: not flat, modulated by field -----
    // Base vertical gradient (uv.y) is bent and pushed by the noise field.
    // depthIntensity controls how strongly the field warps the lerp.
    float base = uv.y;
    float modulator = (field - 0.5) * depthIntensity;
    float k = clamp(base + modulator, 0.0, 1.0);

    // smooth the lerp curve so it feels organic
    k = smoothstep(0.0, 1.0, k);

    vec3 col = mix(colorB.rgb, colorA.rgb, k);

    // ---------- HDR crests ---------------------------------------------
    // Bright noise peaks bloom into HDR. Treble adds shimmer to the
    // crests; mid lifts overall density.
    float crest = smoothstep(0.55, 0.95, maxCrest);
    float crestBoost = crest * (0.55 + 0.45 * treb) * (0.6 + depthIntensity * 0.4);
    // peaks can hit ~1.6 linear
    col += col * crestBoost * 0.9;
    col += vec3(0.7, 0.85, 1.0) * crest * 0.25 * (0.5 + treb);

    // Mid frequencies pump overall brightness gently (kept subtle)
    col *= 1.0 + 0.18 * mid + 0.08 * lvl;

    // ---------- depth-based vignette / parallax cue --------------------
    // Slight darkening at frame edges reinforces "looking into" the field.
    float vig = 1.0 - 0.35 * dot(p * 0.55, p * 0.55);
    col *= clamp(vig, 0.55, 1.0);

    // Output LINEAR HDR — no tonemap, no gamma. Caller handles it.
    gl_FragColor = vec4(col, 1.0);
}
