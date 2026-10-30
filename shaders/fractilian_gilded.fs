/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Gilded — the fractal traced as raised gold filigree on black lacquer, lit by a slowly circling light. Line width, relief, sheen, metal and lacquer colours; bass swings the light, highs glint.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Ornament",
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
			"NAME": "lineWidth",
			"TYPE": "float",
			"DEFAULT": 0.35,
			"MIN": 0.05,
			"MAX": 1
		},
		{
			"NAME": "relief",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 3
		},
		{
			"NAME": "sheen",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 2
		},
		{
			"NAME": "lightRate",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": -1,
			"MAX": 1
		},
		{
			"NAME": "metal",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.76,
				0.33,
				1
			],
			"LABEL": "Metal"
		},
		{
			"NAME": "lacquer",
			"TYPE": "color",
			"DEFAULT": [
				0.03,
				0.02,
				0.05,
				1
			],
			"LABEL": "Lacquer"
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
// Fractilian Gilded
// the fractal traced as raised gold filigree on black lacquer, lit by a slowly circling light.
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

float drift0;
vec2 spin0;
// Filigree height: how close the orbit passes the fold axes.
float height(vec2 p) {
	float trap = 10000.0, m = 10000.0, b = 0.0;
	for(int i=0; i<100; i++) {
		p.x = abs(p.x);
		p = p * scale + spin0;
		p = CircleInversion(p, vec2(0.5, 0.5), 1.0);
		m = min(length(p), m);
		m += Parabola(m, drift0);
		trap = min(trap, min(abs(p.x - 0.5), abs(p.y - 0.5)) / (1.0 + 0.1*length(p)));
		b += 1.0;
		if (b>loops) break;
	}
	return exp(-trap / (0.02*lineWidth));
}

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
	drift0 = mix(pos.x+sin(TT),sin(pos.x)*cos(pos.x-TT),sin(pos.y-TT));
	spin0 = vec2( sin(T*r1), -sin(T*r2))+center;
	float e = 1.0 / RENDERSIZE.y;
	float h = height(pos);
	float hx = height(pos + vec2(e, 0.0)) - h;
	float hy = height(pos + vec2(0.0, e)) - h;
	vec3 n = normalize(vec3(-hx*relief*6.0, -hy*relief*6.0, 1.0));
	float la = TIME*lightRate + 1.2*aB;
	vec3 L = normalize(vec3(cos(la), sin(la), 0.8));
	float diff = max(dot(n, L), 0.0);
	float spec = pow(max(dot(reflect(-L, n), vec3(0.0, 0.0, 1.0)), 0.0), 24.0) * sheen * (1.0 + 1.5*aH);
	vec3 gold = metal.rgb * (0.25 + 0.85*diff) + spec * mix(metal.rgb, vec3(1.0), 0.6);
	vec3 col = mix(lacquer.rgb, gold, clamp(h, 0.0, 1.0));
	col *= 1.0 + 0.2*aL;
	gl_FragColor = vec4(col, 1.0);
}
