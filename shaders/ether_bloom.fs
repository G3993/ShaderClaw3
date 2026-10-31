/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, Ether-core family (light model after nimitz's Ether)",
	"DESCRIPTION": "Ether Bloom — Ether opening like a flower: its tendrils folded into a ring of petals of light that slowly unfurl, turn and close again around a bright heart. Petals, opening, morph, two colours. Built as an input for the AI: one luminous centrepiece in dark space. Bass breathes the bloom.",
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
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 2
		},
		{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0.4,
			"MAX": 2
		},
		{
			"NAME": "morph",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 2,
			"LABEL": "Morph"
		},
		{
			"NAME": "twist",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 3
		},
		{
			"NAME": "baseColor",
			"TYPE": "color",
			"DEFAULT": [
				0.12,
				0.08,
				0.2,
				1
			],
			"LABEL": "Body"
		},
		{
			"NAME": "lightColor",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.55,
				0.75,
				1
			],
			"LABEL": "Light"
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.2,
			"MAX": 3
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
			"NAME": "audioReact",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 2,
			"LABEL": "Audio React",
			"GROUP": "Audio Reactivity"
		},
		{
			"NAME": "petals",
			"TYPE": "float",
			"DEFAULT": 6,
			"MIN": 3,
			"MAX": 12
		},
		{
			"NAME": "opening",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		}
	]
}*/

////////////////////////////////////////////////////////////
// Ether Bloom
// Ether opening like a flower: its tendrils folded into a ring of petals of light that slowly unfurl, turn and close again around a bright heart.
// Ether-core: the light model of nimitz's "Ether" (6-step march, colour
// multiplied through the field) applied to a new form.
////////////////////////////////////////////////////////////

mat2 rot(float a) { float s = sin(a), co = cos(a); return mat2(co, -s, s, co); }
vec3 hueRotate(vec3 col, float h) {
	float a = h*6.28318;
	vec3 k = vec3(0.57735);
	return col*cos(a) + cross(k, col)*sin(a) + k*dot(k, col)*(1.0 - cos(a));
}
// Ether's skin: nested sines that make the tendrils.
float skin(vec3 p, float t) {
	vec3 q = p*2.0 + t;
	return sin(q.x + sin(q.z + sin(q.y)))*0.5;
}
float gB, gM, gH;
float form(vec3 p, float t) {
	p.xy *= rot(t*0.1*twist);
	// Fold space into petals around the view axis.
	float n = floor(petals);
	float a = atan(p.y, p.x), r = length(p.xy);
	float sec = 6.28318/n;
	a = mod(a + sec*0.5, sec) - sec*0.5;
	vec3 q = vec3(r*cos(a), r*sin(a), p.z);
	q.x -= 0.6 + 0.5*(opening + 0.25*sin(t*0.3)) + 0.1*gB;
	q.xz *= rot(0.4*sin(t*0.2));
	float len = length(q*vec3(0.55, 1.6, 1.0));
	return len*log(len + 1.0) + skin(q*1.2, t)*morph - 0.9;
}

void main()
{
	gB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	gM = smoothstep(0.05, 0.85, audioMid) * audioReact;
	gH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;
	// Constant clock plus a small bounded audio phase (no tempo jumps).
	float t = TIME*speed + 0.3*gB;

	vec2 p = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y - vec2(centerX, centerY);
	p /= size*(1.0 + 0.1*gB);
	vec3 cl = vec3(0.0);
	float d = 2.5;
	for (int i = 0; i <= 5; i++) {
		vec3 pos = vec3(0.0, 0.0, 5.0) + normalize(vec3(p, -1.0))*d;
		float rz = form(pos, t);
		float fl = clamp((rz - form(pos + 0.1, t))*0.5, -0.1, 1.0);
		vec3 l = baseColor.rgb + lightColor.rgb*5.0*fl;
		float core = smoothstep(2.5, 0.0, rz);
		cl = cl*l + core*0.7*l;
		d += min(rz, 1.0);
	}
	cl *= brightness*(1.0 + 0.3*aL);
	cl = hueRotate(cl, hueShift);
	vec3 col = 1.0 - exp(-max(cl, 0.0)*1.1);
	gl_FragColor = vec4(background.rgb*(1.0 - max(col.r, max(col.g, col.b))) + col, 1.0);
}
