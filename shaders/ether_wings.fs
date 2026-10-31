/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, Ether family (after nimitz's Ether)",
	"DESCRIPTION": "Ether Wings — a pair of great wings made of light, slowly beating, each traced with feather ridges that glow toward the tips. Beat rate, span, feathers, two colours. Bass deepens the wingbeat, highs light the feather tips.",
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
			"DEFAULT": 0.04,
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
				0.95,
				0.85,
				1
			],
			"LABEL": "Core"
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.55,
				0.25,
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
			"DEFAULT": -0.05,
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
			"NAME": "beat",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 2
		},
		{
			"NAME": "span",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.5,
			"MAX": 1.6
		},
		{
			"NAME": "feathers",
			"TYPE": "float",
			"DEFAULT": 14,
			"MIN": 6,
			"MAX": 24
		}
	]
}*/

////////////////////////////////////////////////////////////
// Ether Wings
// a pair of great wings made of light, slowly beating, each traced with feather ridges that glow toward the tips.
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
	p.xz *= rot(0.35*sin(gSpin));
	p.yz *= rot(-1.25);
	float side = sign(p.x);
	p.x = abs(p.x);
	// Flap: rotate the wing plane about the body axis.
	float flap = 0.45*sin(T*beat*3.0) + 0.15 + 0.15*gB;
	p.xy = rot(-flap) * p.xy;
	// Wing in its plane (x out, z back); shape = rounded fan.
	vec2 w = vec2(p.x/span, p.z);
	float fanR = length(w - vec2(0.0, 0.25));
	float ang = atan(w.y - 0.25, w.x);
	float outline = fanR - (1.35 + 0.18*cos(ang*2.0)) * smoothstep(-1.9, -0.3, ang) * smoothstep(1.6, 0.4, ang);
	float ridge = abs(sin(ang*feathers*0.5))*0.04;
	float sheet = abs(p.y + ridge*fanR*0.6) - 0.006;
	float d = max(sheet, outline);
	d = max(d, -p.x + 0.02);
	float body = length(vec2(p.x, p.y)) - 0.05 + 0.0*side;
	body = max(body, abs(p.z) - 0.6);
	k = clamp(fanR/1.4, 0.0, 1.0)*(0.7 + 0.3*gH);
	float vane = pow(abs(sin(ang*feathers*0.5)), 6.0);
	gInt = 0.25 + 1.6*vane*smoothstep(0.2, 1.2, fanR) + 1.5*smoothstep(0.08, 0.0, abs(outline));
	if (body < d) { d = body; k = 0.0; gInt = 1.0; }
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
		float stepLen = clamp(abs(d)*0.5, 0.02, 0.25);
		float g = exp(-abs(d)*sharp);
		vec3 tint = mix(colorA.rgb, colorB.rgb, clamp(k, 0.0, 1.0));
		acc += tint * g * stepLen * gInt;
		tt += stepLen;
		if (tt > 7.2) break;
	}
	vec3 col = acc * glow * 2.0 * (1.0 + 0.35*aL);
	col = hueRotate(col, hueShift);
	col += hueRotate(colorA.rgb, hueShift) * exp(-length(uv)*3.5) * 0.06 * glow;
	col = 1.0 - exp(-col);
	gl_FragColor = vec4(background.rgb*(1.0 - max(col.r, max(col.g, col.b))) + col, 1.0);
}
