/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Vortex — the fractal wrapped into a spiralling log-polar tunnel that keeps flowing toward the centre. Arms, twist, speed, depth fog, colour; bass surges the flow.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Tunnel",
		"Audio Reactive"
	],
	"INPUTS": [
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			],
			"MAX": [
				1,
				1
			],
			"MIN": [
				-1,
				-1
			]
		},
		{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 2,
			"MIN": 0.5,
			"MAX": 3
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": -0.5,
			"MAX": 0.5
		},
		{
			"NAME": "fine",
			"TYPE": "float",
			"DEFAULT": 0.35,
			"MIN": 0.01,
			"MAX": 0.5
		},
		{
			"NAME": "loops",
			"TYPE": "float",
			"DEFAULT": 32,
			"MIN": 12,
			"MAX": 100
		},
		{
			"NAME": "r1",
			"TYPE": "float",
			"DEFAULT": -0.25,
			"MIN": -0.5,
			"MAX": 0.5
		},
		{
			"NAME": "r2",
			"TYPE": "float",
			"DEFAULT": 0.275,
			"MIN": -0.5,
			"MAX": 0.5
		},
		{
			"NAME": "arms",
			"TYPE": "float",
			"DEFAULT": 3,
			"MIN": 1,
			"MAX": 8
		},
		{
			"NAME": "twist",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": -3,
			"MAX": 3
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": -1.5,
			"MAX": 1.5
		},
		{
			"NAME": "rings",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0.3,
			"MAX": 4
		},
		{
			"NAME": "fog",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0.1,
			"MAX": 5
		},
		{
			"NAME": "bleed",
			"TYPE": "float",
			"DEFAULT": 0.65,
			"MIN": 0,
			"MAX": 0.99
		},
		{
			"NAME": "edge",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.01,
			"MAX": 0.99
		},
		{
			"NAME": "color",
			"TYPE": "color",
			"DEFAULT": [
				0.6,
				0.4,
				0.9,
				1
			]
		},
		{
			"NAME": "hueDepth",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "audioReact",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 2,
			"LABEL": "Audio React",
			"GROUP": "Audio Reactivity"
		}
	]
}*/

////////////////////////////////////////////////////////////
// Fractilian Vortex
// the fractal wrapped into a spiralling log-polar tunnel that keeps flowing toward the centre.
//
// Variant of FractilianParabolicCircleInversion by mojovideotech,
// based on Fractal Soup by @P_Malin (shadertoy.com/view/lsB3zR)
//
// License:
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

vec2 CircleInversion(vec2 vPos, vec2 vOrigin, float fRadius) {
	vec2 vOP = vPos - vOrigin;
	return vOrigin - vOP * fRadius * fRadius / dot(vOP, vOP);
}

float Parabola( float x, float n ) { return pow( 4.0*x*(1.0-x), n ); }

vec2 soup(vec2 p, float T, float scl) {
	float TT = T * 0.05;
	float drift = mix(p.x+sin(TT),sin(p.x)*cos(p.x-TT),sin(p.y-TT));
	vec2 spin = vec2( sin(T*r1), -sin(T*r2))+center;
	float l = 0.0, b = 0.0, m = 10000.0;
	for(int i=0; i<100; i++) {
		p.x = abs(p.x);
		p = p * scl + spin;
		p = CircleInversion(p, vec2(0.5, 0.5), 1.0);
		l = length(p);
		m = min(l, m);
		m += Parabola(m, drift);
		b += 1.0;
		if (b>loops) break;
	}
	return vec2(l, m);
}

float tri(float x) { return abs(fract(x)*2.0 - 1.0); }

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aM = smoothstep(0.05, 0.85, audioMid) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;
	vec2 q = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float r = max(length(q), 1e-4);
	float lr = log(r);
	float a = atan(q.y, q.x);
	float flow = TIME*speed*(1.0 + 0.6*aB);
	float th = a*floor(arms) + twist*lr - flow*0.5;
	float rad = 0.15 + 0.55*tri(lr*rings*0.5 + flow*0.25);
	vec2 pos = vec2(0.5) + rad*vec2(cos(th), sin(th));
	float T = TIME * rate + fine + 0.6*aB;
	vec2 lm = soup(pos, T, scale);
	float l = lm.x, m = lm.y;
	vec3 tint = color.rgb;
	float hd = hueDepth*(lr + flow*0.25);
	tint = mix(tint, tint.gbr, 0.5 + 0.5*sin(hd*3.0));
	vec3 col = tint * l * l * brightness * (1.0 + 0.25*aL);
	col = mix(col, pow(col, vec3(m)), 1.0 - edge);
	col = mix(col, col*fract(m*col + m), bleed);
	col *= mix(1.0, smoothstep(0.0, 0.35, r), fog);
	col = 1.0 - exp(-col);
	gl_FragColor = vec4(col, 1.0);
}
