/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Sumi Ink — the fractal brushed in layered ink washes on warm paper, with pooled pigment at the wash edges, paper grain and a vermilion seal accent. Ink, wash, pooling, grain, accent, ink/paper/accent colours; bass deepens the ink.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Ink",
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
			"NAME": "ink",
			"TYPE": "float",
			"DEFAULT": 0.55,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "wash",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "pooling",
			"TYPE": "float",
			"DEFAULT": 0.7,
			"MIN": 0,
			"MAX": 1.5
		},
		{
			"NAME": "grain",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "accent",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "inkColor",
			"TYPE": "color",
			"DEFAULT": [
				0.06,
				0.07,
				0.11,
				1
			],
			"LABEL": "Ink"
		},
		{
			"NAME": "paper",
			"TYPE": "color",
			"DEFAULT": [
				0.95,
				0.93,
				0.87,
				1
			],
			"LABEL": "Paper"
		},
		{
			"NAME": "accentColor",
			"TYPE": "color",
			"DEFAULT": [
				0.85,
				0.18,
				0.1,
				1
			],
			"LABEL": "Accent"
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
// Fractilian Sumi Ink
// the fractal brushed in layered ink washes on warm paper, with pooled pigment at the wash edges, paper grain and a vermilion seal accent.
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

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float noise(vec2 p) {
	vec2 i = floor(p), f = fract(p);
	f = f*f*(3.0-2.0*f);
	return mix(mix(hash(i), hash(i + vec2(1.0, 0.0)), f.x), mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), f.x), f.y);
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
	vec2 lm = soup(pos, T, scale);
	float l = lm.x, m = lm.y;
	// Darkness: 0 = bare paper, 1 = full ink. The original's bright areas become ink.
	float v = 1.0 - exp(-l*l*0.6*(0.5 + ink)*(1.0 + 0.4*aB));
	float fiber = noise(gl_FragCoord.xy*vec2(0.9, 0.12)) * noise(gl_FragCoord.xy*0.05);
	v += (fiber - 0.25)*0.12*grain;
	float layers = 0.0;
	layers += 0.35*wash*smoothstep(0.25, 0.3, v);
	layers += 0.35*wash*smoothstep(0.5, 0.55, v);
	layers += smoothstep(0.78, 0.84, v);
	float pool = 0.0;
	pool += 1.0 - smoothstep(0.0, 0.02, abs(v - 0.28));
	pool += 1.0 - smoothstep(0.0, 0.02, abs(v - 0.53));
	layers = clamp(layers + 0.25*pooling*pool*wash, 0.0, 1.0);
	vec3 paperCol = paper.rgb * (1.0 - 0.08*grain*(noise(gl_FragCoord.xy*0.02) - 0.5));
	vec3 col = mix(paperCol, inkColor.rgb, layers);
	float seal = (1.0 - smoothstep(0.0, 0.04, abs(fract(m*2.0) - 0.5))) * smoothstep(0.6, 0.9, v) * accent;
	col = mix(col, accentColor.rgb, clamp(seal, 0.0, 1.0)*0.9);
	gl_FragColor = vec4(col, 1.0);
}
