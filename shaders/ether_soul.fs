/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, Ether family (after nimitz's Ether)",
	"DESCRIPTION": "Ether Soul — a radiant heart of light with comets of glow orbiting it on tilted paths, leaving fading trails, like a small living star. Comet count, orbit speed, trail length, two colours. Bass swells the heart, highs brighten the comets.",
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
			"DEFAULT": 0,
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
				0.9,
				0.7,
				1
			],
			"LABEL": "Core"
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				0.5,
				0.75,
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
			"NAME": "comets",
			"TYPE": "float",
			"DEFAULT": 5,
			"MIN": 1,
			"MAX": 9
		},
		{
			"NAME": "orbitSpeed",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 2,
			"LABEL": "Orbit Speed"
		},
		{
			"NAME": "trail",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1
		}
	]
}*/

////////////////////////////////////////////////////////////
// Ether Soul
// a radiant heart of light with comets of glow orbiting it on tilted paths, leaving fading trails, like a small living star.
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
	float heart = length(p) - 0.32*(1.0 + 0.25*gB + 0.05*sin(T*1.7));
	float d = heart;
	float cometGlow = 0.0, cometK = 0.0, cometW = 0.0;
	k = 0.0;
	for (int i = 0; i < 9; i++) {
		float fi = float(i);
		if (fi >= floor(comets)) break;
		vec3 q = p;
		q.yz *= rot(fi*1.1 + 0.4);
		q.xy *= rot(fi*0.7);
		float r = 0.85 + 0.35*h11(fi + 3.0);
		float ang = atan(q.z, q.x);
		float head = T*orbitSpeed*(0.6 + 0.5*h11(fi)) + fi*2.1;
		// Angular distance behind the comet head (0 at the head).
		float behind = mod(head - ang, 6.28318);
		float tl = 0.2 + 2.6*trail;
		// Tapers to zero at both ends of the trail, so nothing cuts off at the head.
		float fade = smoothstep(tl, 0.0, behind) * smoothstep(0.0, 0.3, behind);
		float tube = length(vec2(length(q.xz) - r, q.y)) - (0.006 + 0.04*fade*fade)*(1.0 + gH);
		// Each comet adds its own glow (weighted by its trail fade) relative to the
		// march's exp(-|d|*sharp), so a dark stretch of one orbit can't hide another.
		cometGlow += fade*fade*1.6*exp(-tube*gSharp);
		cometK += (1.0 - 0.6*fade)*fade;
		cometW += fade;
		d = min(d, tube);
		// The comet's head: a small bright point just ahead of its trail.
		vec3 hp = vec3(r*cos(head), 0.0, r*sin(head));
		float hd = length(q - hp) - 0.03*(1.0 + gH);
		cometGlow += 1.4*exp(-hd*gSharp);
		cometK += 0.4; cometW += 1.0;
		d = min(d, hd);
	}
	float base = exp(-abs(d)*gSharp);
	float heartGlow = exp(-abs(heart)*gSharp);
	gInt = (heartGlow + cometGlow) / max(base, 1e-5);
	k = mix(0.0, cometK/max(cometW, 1e-4), cometGlow/max(heartGlow + cometGlow, 1e-5));
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
	vec3 col = acc * glow * 2.4 * (1.0 + 0.35*aL);
	col = hueRotate(col, hueShift);
	col += hueRotate(colorA.rgb, hueShift) * exp(-length(uv)*3.5) * 0.06 * glow;
	col = 1.0 - exp(-col);
	gl_FragColor = vec4(background.rgb*(1.0 - max(col.r, max(col.g, col.b))) + col, 1.0);
}
