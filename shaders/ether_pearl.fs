/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, Ether family (after nimitz's Ether)",
	"DESCRIPTION": "Ether Pearl — an orb of light packed with an Apollonian fractal: bubbles within bubbles, glowing like a pearl lit from inside as the inversion slowly breathes. Inversion, depth, two colours. Fractal and ethereal at once. Bass breathes the packing.",
	"CATEGORIES": [
		"Generator",
		"Ethereal",
		"Fractal",
		"3D",
		"Audio Reactive"
	],
	"INPUTS": [
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.35,
			"MIN": 0,
			"MAX": 2
		},
		{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.4,
			"MAX": 2
		},
		{
			"NAME": "spin",
			"TYPE": "float",
			"DEFAULT": 0.06,
			"MIN": -1,
			"MAX": 1
		},
		{
			"NAME": "glow",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 3
		},
		{
			"NAME": "sharpness",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.3,
			"MAX": 3,
			"LABEL": "Sharpness"
		},
		{
			"NAME": "colorA",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.92,
				0.85,
				1
			],
			"LABEL": "Core"
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				0.55,
				0.7,
				1,
				1
			],
			"LABEL": "Edge"
		},
		{
			"NAME": "hueShift",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": 0,
			"MAX": 1,
			"LABEL": "Hue Shift"
		},
		{
			"NAME": "centerX",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": -0.8,
			"MAX": 0.8,
			"LABEL": "Center X"
		},
		{
			"NAME": "centerY",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": -0.5,
			"MAX": 0.5,
			"LABEL": "Center Y"
		},
		{
			"NAME": "background",
			"TYPE": "color",
			"DEFAULT": [
				0,
				0,
				0,
				1
			],
			"LABEL": "Background"
		},
		{
			"NAME": "quality",
			"TYPE": "float",
			"DEFAULT": 72,
			"MIN": 32,
			"MAX": 128,
			"LABEL": "Quality"
		},
		{
			"NAME": "audioReact",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 2,
			"LABEL": "Audio React",
			"GROUP": "Audio Reactivity"
		},
		{
			"NAME": "inversion",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 1,
			"MAX": 1.5
		},
		{
			"NAME": "iters",
			"TYPE": "float",
			"DEFAULT": 6,
			"MIN": 3,
			"MAX": 8,
			"LABEL": "Iterations"
		},
		{
			"NAME": "density",
			"TYPE": "float",
			"DEFAULT": 2.2,
			"MIN": 1,
			"MAX": 4
		}
	]
}*/

////////////////////////////////////////////////////////////
// Ether Pearl
// an orb of light packed with an Apollonian fractal: bubbles within bubbles, glowing like a pearl lit from inside as the inversion slowly breathes.
// Ether family: a glow raymarch through a soft form, accumulating light
// near its surface so it reads as light, not a solid.
////////////////////////////////////////////////////////////

vec3 hueRotate(vec3 c, float h) {
	// Rodrigues rotation about the grey axis.
	float a = h*6.28318;
	vec3 k = vec3(0.57735);
	return c*cos(a) + cross(k, c)*sin(a) + k*dot(k, c)*(1.0 - cos(a));
}
mat2 rot(float a) { float s = sin(a), co = cos(a); return mat2(co, -s, s, co); }
float h11(float n) { return fract(sin(n*12.9898) * 43758.5453); }

float gB, gM, gH, gSpin;
float gInt;   // per-sample brightness a form may set (default 1)
float gSharp; // the march's glow falloff, for forms that sum several glows

float form(vec3 p, float T, out float k) {
	float r0 = length(p);
	float ball = r0 - 1.1;
	vec3 q = p;
	q.xz *= rot(gSpin);
	q.xy *= rot(T*0.07);
	q *= density;
	float s = inversion + 0.08*sin(T*0.2) + 0.03*gB;
	float scale = 1.0, trap = 10.0;
	for (int i = 0; i < 8; i++) {
		if (float(i) >= iters) break;
		q = -1.0 + 2.0*fract(0.5*q + 0.5);
		float r2 = dot(q, q);
		trap = min(trap, r2);
		float kk = s/r2;
		q *= kk;
		scale *= kk;
	}
	float frac = 0.25*(length(q)-0.35)/scale/density;
	k = clamp(trap*1.5, 0.0, 1.0);
	return max(frac, ball);
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aM = smoothstep(0.05, 0.85, audioMid) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;
	gB = aB; gM = aM; gH = aH;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y - vec2(centerX, centerY);
	float T = TIME*speed;
	gSpin = TIME*spin;
	vec3 ro = vec3(0.0, 0.0, 4.2);
	vec3 rd = normalize(vec3(uv, -1.6));
	float sz = size * (1.0 + 0.06*aB);

	vec3 acc = vec3(0.0);
	float tt = 1.2;
	float sharp = 50.0 * sharpness;
	gSharp = sharp;
	for (int i = 0; i < 128; i++) {
		if (float(i) >= quality) break;
		vec3 p = (ro + rd*tt) / sz;
		float k;
		gInt = 1.0;
		float d = form(p, T, k) * sz;
		float stepLen = clamp(abs(d)*0.5, 0.006, 0.25);
		float g = exp(-abs(d)*sharp);
		vec3 tint = mix(colorA.rgb, colorB.rgb, clamp(k, 0.0, 1.0));
		acc += tint * g * stepLen * gInt;
		tt += stepLen;
		if (tt > 7.2) break;
	}
	vec3 col = acc * glow * 3.0 * (1.0 + 0.35*aL);
	col = hueRotate(col, hueShift);
	col += hueRotate(colorA.rgb, hueShift) * exp(-length(uv)*3.5) * 0.06 * glow;
	col = 1.0 - exp(-col);
	gl_FragColor = vec4(background.rgb*(1.0 - max(col.r, max(col.g, col.b))) + col, 1.0);
}
