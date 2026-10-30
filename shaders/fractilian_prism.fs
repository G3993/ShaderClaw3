/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Prism — the original rendered once per colour channel with a slightly different scale, so every edge splits into rainbow fringes around white-hot cores. Dispersion, contrast, brightness; bass widens the split.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Chromatic",
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
			"NAME": "dispersion",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0,
			"MAX": 4
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.1,
			"MAX": 5
		},
		{
			"NAME": "bleed",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.01,
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
			"NAME": "contrast",
			"TYPE": "float",
			"DEFAULT": 1.1,
			"MIN": 0.5,
			"MAX": 2.5
		},
		{
			"NAME": "color",
			"TYPE": "color",
			"DEFAULT": [
				0.6,
				0.4,
				0.9,
				1
			],
			"LABEL": "Tint"
		},
		{
			"NAME": "tint",
			"TYPE": "float",
			"DEFAULT": 0.35,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "invert",
			"TYPE": "bool",
			"DEFAULT": false
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
// Fractilian Prism
// the original rendered once per colour channel with a slightly different scale, so every edge splits into rainbow fringes around white-hot cores.
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

float chan(vec2 pos, float T, float scl) {
	vec2 lm = soup(pos, T, scl);
	float c = lm.x*lm.x*brightness;
	c = mix(c, pow(c, lm.y), 1.0 - edge);
	c = mix(c, c*fract(lm.y*c + lm.y), bleed);
	return c;
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
	float d = 0.006*dispersion*(1.0 + 0.8*aB);
	vec3 col = vec3(chan(pos, T, scale - d), chan(pos, T, scale), chan(pos, T, scale + d));
	col *= mix(vec3(1.0), color.rgb*1.6, tint) * (1.0 + 0.25*aL);
	col = 1.0 - exp(-col);
	col = clamp((col - 0.5)*contrast + 0.5, 0.0, 1.0);
	if (invert) col = 1.0 - col;
	gl_FragColor = vec4(col, 1.0);
}
