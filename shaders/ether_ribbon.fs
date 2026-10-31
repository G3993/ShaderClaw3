/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, Ether family (after nimitz's Ether)",
	"DESCRIPTION": "Ether Ribbon — one long silk ribbon of light twisting and folding through the void like an aurora caught in a breeze. Twist, waviness, width, two colours. Bass swells the folds, highs shimmer the edges.",
	"CATEGORIES": [
		"Generator",
		"Ethereal",
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
				0.4,
				1,
				0.75,
				1
			],
			"LABEL": "Core"
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				0.55,
				0.35,
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
			"NAME": "twist",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0,
			"MAX": 4
		},
		{
			"NAME": "wave",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1.5
		},
		{
			"NAME": "ribbonWidth",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.1,
			"MAX": 1.2,
			"LABEL": "Width"
		}
	]
}*/

////////////////////////////////////////////////////////////
// Ether Ribbon
// one long silk ribbon of light twisting and folding through the void like an aurora caught in a breeze.
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
	p.xz *= rot(gSpin*0.5);
	p.xy *= rot(0.5);
	float y = p.y;
	// Bend the ribbon's spine through space.
	vec3 q = p;
	q.x -= wave*(0.6*sin(y*1.1 + T*0.7) + 0.25*sin(y*2.3 - T*0.9))*(1.0 + 0.3*gB);
	q.z -= wave*0.5*cos(y*0.9 - T*0.5);
	q.xz *= rot(y*twist + T*0.6);
	float w = ribbonWidth*(0.8 + 0.2*sin(y*1.7 + T));
	float d = max(abs(q.z) - 0.01, abs(q.x) - w);
	d = max(d, abs(y) - 2.6);
	k = 0.5 + 0.5*sin(y*0.9 + T*0.4) * (q.x/w);
	return d;
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
	float sharp = 20.0 * sharpness;
	gSharp = sharp;
	for (int i = 0; i < 128; i++) {
		if (float(i) >= quality) break;
		vec3 p = (ro + rd*tt) / sz;
		float k;
		gInt = 1.0;
		float d = form(p, T, k) * sz;
		float stepLen = clamp(abs(d)*0.5, 0.015, 0.25);
		float g = exp(-abs(d)*sharp);
		vec3 tint = mix(colorA.rgb, colorB.rgb, clamp(k, 0.0, 1.0));
		acc += tint * g * stepLen * gInt;
		tt += stepLen;
		if (tt > 7.2) break;
	}
	vec3 col = acc * glow * 3.4 * (1.0 + 0.35*aL);
	col = hueRotate(col, hueShift);
	col += hueRotate(colorA.rgb, hueShift) * exp(-length(uv)*3.5) * 0.06 * glow;
	col = 1.0 - exp(-col);
	gl_FragColor = vec4(background.rgb*(1.0 - max(col.r, max(col.g, col.b))) + col, 1.0);
}
