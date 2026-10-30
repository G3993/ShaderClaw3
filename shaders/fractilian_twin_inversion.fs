/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Twin Inversion — a second, orbiting inversion circle folds every orbit that lands inside it, so the fractal grows nested bubbles and stranger structures; coloured like oil-slick thin film. Orbit, second radius, film frequency, shimmer; bass pushes the orbit.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Iridescent",
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
			"NAME": "mirrorH",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "mirrorV",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "orbit",
			"TYPE": "float",
			"DEFAULT": 0.35,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "radius2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.1,
			"MAX": 1.2
		},
		{
			"NAME": "orbitRate",
			"TYPE": "float",
			"DEFAULT": 0.07,
			"MIN": -0.5,
			"MAX": 0.5
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 1.3,
			"MIN": 0.1,
			"MAX": 5
		},
		{
			"NAME": "film",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0.2,
			"MAX": 6
		},
		{
			"NAME": "shimmer",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": -0.3,
			"MAX": 0.3
		},
		{
			"NAME": "bleed",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0,
			"MAX": 0.99
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
// Fractilian Twin Inversion
// a second, orbiting inversion circle folds every orbit that lands inside it, so the fractal grows nested bubbles and stranger structures; coloured like oil-slick thin film.
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

void main()
{
	vec2 pos = gl_FragCoord.xy/RENDERSIZE.xy;
	if (mirrorV) { if (pos.y < 0.5) pos.y = 1.0-pos.y; }
	if (mirrorH) { if (pos.x < 0.5) pos.x = 1.0-pos.x; }
	pos.x *= RENDERSIZE.x / RENDERSIZE.y;
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aM = smoothstep(0.05, 0.85, audioMid) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;
	float T = TIME * rate + fine + 0.6*aB;
	float TT = T * 0.05;
	float drift = mix(pos.x+sin(TT),sin(pos.x)*cos(pos.x-TT),sin(pos.y-TT));
	vec2 spin = vec2( sin(T*r1), -sin(T*r2))+center;
	float oa = TIME*orbitRate;
	vec2 c2 = vec2(0.5) + orbit*(1.0 + 0.3*aB)*vec2(cos(oa), 0.7*sin(oa*1.3));
	float l = 0.0, b = 0.0, m = 10000.0;
	vec2 p = pos;
	for(int i=0; i<100; i++) {
		p.x = abs(p.x);
		p = p * scale + spin;
		p = CircleInversion(p, vec2(0.5, 0.5), 1.0);
		vec2 d2 = p - c2;
		if (dot(d2, d2) < radius2*radius2) p = CircleInversion(p, c2, radius2);
		l = length(p);
		m = min(l, m);
		m += Parabola(m, drift);
		b += 1.0;
		if (b>loops) break;
	}
	float lum = clamp(l*l*brightness*0.35*(1.0 + 0.25*aL), 0.0, 3.0);
	vec3 iri = 0.5 + 0.5*cos(6.28318*(film*log(1.0 + l) + 0.5*m + TIME*shimmer + vec3(0.0, 0.33, 0.67)));
	vec3 col = iri * lum;
	col = mix(col, col*fract(m*lum + m), bleed);
	col += 0.4*pow(clamp(1.0 - m, 0.0, 1.0), 6.0);
	col = 1.0 - exp(-col*1.5);
	gl_FragColor = vec4(col, 1.0);
}
