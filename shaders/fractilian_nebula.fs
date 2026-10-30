/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Nebula — the inversion orbit accumulated as glowing gas instead of hard bands: three-colour clouds with dark dust lanes over a twinkling starfield. Glow, falloff, dust, stars, three colours; mids swell the gas, highs make the stars sparkle.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Space",
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
			"NAME": "glow",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0.05,
			"MAX": 3
		},
		{
			"NAME": "falloff",
			"TYPE": "float",
			"DEFAULT": 5,
			"MIN": 0.5,
			"MAX": 16
		},
		{
			"NAME": "dust",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "stars",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "colorA",
			"TYPE": "color",
			"DEFAULT": [
				0.95,
				0.25,
				0.6,
				1
			],
			"LABEL": "Inner"
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				0.2,
				0.45,
				1,
				1
			],
			"LABEL": "Middle"
		},
		{
			"NAME": "colorC",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.8,
				0.45,
				1
			],
			"LABEL": "Outer"
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
// Fractilian Nebula
// the inversion orbit accumulated as glowing gas instead of hard bands: three-colour clouds with dark dust lanes over a twinkling starfield.
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

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

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
	vec2 p = pos;
	float m = 10000.0, b = 0.0;
	vec3 gas = vec3(0.0);
	for(int i=0; i<100; i++) {
		p.x = abs(p.x);
		p = p * scale + spin;
		p = CircleInversion(p, vec2(0.5, 0.5), 1.0);
		float l = length(p);
		m = min(l, m);
		m += Parabola(m, drift);
		float k = b / max(loops, 1.0);
		vec3 tint = k < 0.5 ? mix(colorA.rgb, colorB.rgb, k*2.0) : mix(colorB.rgb, colorC.rgb, k*2.0 - 1.0);
		gas += tint * exp(-falloff * abs(l - 1.0));
		b += 1.0;
		if (b>loops) break;
	}
	gas *= glow * (1.0 + 0.5*aM) / sqrt(max(b, 1.0));
	float lane = clamp(m, 0.0, 1.0);
	gas *= mix(1.0, smoothstep(0.0, 0.6, lane), dust);
	vec2 g = floor(gl_FragCoord.xy / 2.0);
	float s = hash(g);
	float tw = 0.5 + 0.5*sin(TIME*(2.0 + 6.0*hash(g + 7.0)) + s*40.0);
	float star = step(0.9965, s) * tw * stars * (1.0 + 2.0*aH);
	vec3 col = 1.0 - exp(-gas*gas*1.5);
	col += star * (1.0 - col);
	gl_FragColor = vec4(col, 1.0);
}
