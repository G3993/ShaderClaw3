/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Neon Contour — the fractal drawn as glowing contour lines on black, like a neon topographic map, with the lines flowing outward. Line density, thickness, halo, flow, palette; bass flows, highs flare the halos.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Neon",
		"Lines",
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
			"NAME": "density",
			"TYPE": "float",
			"DEFAULT": 3,
			"MIN": 0.5,
			"MAX": 12
		},
		{
			"NAME": "thickness",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.3,
			"MAX": 5
		},
		{
			"NAME": "halo",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 2
		},
		{
			"NAME": "flow",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": -2,
			"MAX": 2
		},
		{
			"NAME": "hue",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "hueSpread",
			"TYPE": "float",
			"DEFAULT": 0.12,
			"MIN": 0,
			"MAX": 0.5
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
// Fractilian Neon Contour
// the fractal drawn as glowing contour lines on black, like a neon topographic map, with the lines flowing outward.
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
	vec2 pos = gl_FragCoord.xy/RENDERSIZE.xy;
	if (mirrorV) { if (pos.y < 0.5) pos.y = 1.0-pos.y; }
	if (mirrorH) { if (pos.x < 0.5) pos.x = 1.0-pos.x; }
	pos.x *= RENDERSIZE.x / RENDERSIZE.y;
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aM = smoothstep(0.05, 0.85, audioMid) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;
	float T = TIME * rate + fine + 0.6*aB;
	vec2 lm = soup(pos, T, scale);
	float l = lm.x, m = lm.y;
	float fld = log(l + 1e-3)*density + 0.5*m - TIME*flow*(1.0 + 0.6*aB);
	float w = max(fwidth(fld), 1e-4);
	float d = abs(fract(fld) - 0.5);
	float core = 1.0 - smoothstep(0.0, w*thickness, 0.5 - d);
	float glowL = exp(-(0.5 - d)/(w*thickness*4.0)) * halo * (1.0 + aH);
	float clarity = 1.0 / (1.0 + w*6.0);
	vec3 tint = pal(hue + floor(fld)*hueSpread);
	vec3 col = tint * (core + glowL) * clarity * (1.0 + 0.3*aL);
	col += tint * 0.04 * clarity;
	col = 1.0 - exp(-col*1.4);
	gl_FragColor = vec4(col, 1.0);
}
