/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Kaleidoscope — the circle-inversion fractal folded into an N-segment mandala, painted with a cycling cosine palette. Segments, spin, zoom, palette hue/rate/saturation; bass turns the wheel, level lifts the light.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Kaleidoscope",
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
			"NAME": "segments",
			"TYPE": "float",
			"DEFAULT": 8,
			"MIN": 3,
			"MAX": 16
		},
		{
			"NAME": "spinRate",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": -0.3,
			"MAX": 0.3
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 1.1,
			"MIN": 0.3,
			"MAX": 3
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 1.4,
			"MIN": 0.1,
			"MAX": 5
		},
		{
			"NAME": "bleed",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0,
			"MAX": 0.99
		},
		{
			"NAME": "hue",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "hueRate",
			"TYPE": "float",
			"DEFAULT": 0.015,
			"MIN": -0.2,
			"MAX": 0.2
		},
		{
			"NAME": "saturation",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1.5
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
// Fractilian Kaleidoscope
// the circle-inversion fractal folded into an N-segment mandala, painted with a cycling cosine palette.
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

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aM = smoothstep(0.05, 0.85, audioMid) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;
	vec2 q = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float r = length(q);
	float seg = 6.28318 / floor(segments);
	float a = atan(q.y, q.x) + TIME*spinRate + 0.35*aB;
	a = mod(a, seg);
	a = abs(a - 0.5*seg);
	vec2 pos = vec2(0.5) + r*zoom*vec2(cos(a), sin(a));
	float T = TIME * rate + fine + 0.6*aB;
	vec2 lm = soup(pos, T, scale);
	float l = lm.x, m = lm.y;
	float lum = l*l*brightness*0.45*(1.0 + 0.25*aL);
	vec3 col = pal(m*0.6 + log(1.0 + l)*0.35 + hue + TIME*hueRate);
	col = mix(vec3(dot(col, vec3(0.299, 0.587, 0.114))), col, saturation);
	col *= lum;
	col = mix(col, col*fract(m*lum + m), bleed);
	col += 0.25*pal(hue + 0.5) * exp(-r*7.0) * (1.0 + aB);
	col = 1.0 - exp(-col*1.6);
	gl_FragColor = vec4(col, 1.0);
}
