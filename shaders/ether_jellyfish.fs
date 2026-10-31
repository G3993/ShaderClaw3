/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, Ether family (after nimitz's Ether)",
	"DESCRIPTION": "Ether Jellyfish — a jellyfish made of light: a pulsing translucent bell trailing slow, swaying tendrils as it drifts in the dark. Pulse, tendril count and length, sway, two colours. Bass pulses the bell, highs shimmer the tendrils.",
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
			"DEFAULT": 0.72,
			"MIN": 0.4,
			"MAX": 2
		},
		{
			"NAME": "spin",
			"TYPE": "float",
			"DEFAULT": 0.08,
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
				0.55,
				0.85,
				1,
				1
			],
			"LABEL": "Core"
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.45,
				0.85,
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
			"NAME": "pulse",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1.5
		},
		{
			"NAME": "tendrils",
			"TYPE": "float",
			"DEFAULT": 8,
			"MIN": 3,
			"MAX": 14
		},
		{
			"NAME": "tendrilLength",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.3,
			"MAX": 1.8,
			"LABEL": "Tendril Length"
		},
		{
			"NAME": "sway",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1.5
		}
	]
}*/

////////////////////////////////////////////////////////////
// Ether Jellyfish
// a jellyfish made of light: a pulsing translucent bell trailing slow, swaying tendrils as it drifts in the dark.
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
	p.y -= 0.1 + 0.12*sin(T*0.5);
	p.xz *= rot(gSpin);
	p.xy *= rot(0.12*sin(T*0.37));
	// Bell: a squashed sphere shell, open underneath, pulsing.
	float pl = 1.0 + pulse*0.12*sin(T*2.0) + 0.1*gB;
	vec3 q = p; q.y *= 1.35*pl; q.xz /= pl;
	float r = length(q);
	float rim = 0.06*sin(atan(q.z, q.x)*16.0 + T*2.0);
	float bell = abs(r - 0.85 - rim*smoothstep(0.2, -0.3, q.y)) - 0.03;
	bell = max(bell, -q.y - 0.12);
	// Tendrils: polar repetition of thin swaying threads hanging under the bell.
	float n = floor(tendrils);
	float a = atan(p.z, p.x);
	float sec = 6.28318/n;
	float id = floor(a/sec + 0.5);
	float aa = a - id*sec;
	float rr = length(p.xz);
	vec2 tq = vec2(rr*cos(aa), rr*sin(aa));
	float y = -p.y;
	float L = 2.2*tendrilLength;
	float wob = sway*(0.12*sin(y*2.4 - T*1.6 + id*1.7) + 0.05*sin(y*5.0 - T*2.3 + id))*smoothstep(0.0, 1.0, y);
	vec2 off = tq - vec2(0.55*pl - 0.12*y/L, wob);
	float thread = length(off) - 0.012*(1.0 + gH) - 0.02*(1.0 - y/L);
	thread = max(thread, max(-y, y - L));
	k = clamp(y/L, 0.0, 1.0);
	if (bell < thread) k = 0.15*smoothstep(0.9, 0.0, r);
	return min(bell, thread);
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
	float sharp = 16.0 * sharpness;
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
	vec3 col = acc * glow * 2.6 * (1.0 + 0.35*aL);
	col = hueRotate(col, hueShift);
	col += hueRotate(colorA.rgb, hueShift) * exp(-length(uv)*3.5) * 0.06 * glow;
	col = 1.0 - exp(-col);
	gl_FragColor = vec4(background.rgb*(1.0 - max(col.r, max(col.g, col.b))) + col, 1.0);
}
