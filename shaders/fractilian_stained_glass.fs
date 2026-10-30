/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Stained Glass — fractal cells filled with jewel glass (ruby, sapphire, emerald, amber, amethyst) between dark leading, backlit with a soft sun. Facets, lead width, glow, glass texture, sun position; bass brightens the sun.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Glass",
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
			"NAME": "facets",
			"TYPE": "float",
			"DEFAULT": 3,
			"MIN": 1,
			"MAX": 8
		},
		{
			"NAME": "lead",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0,
			"MAX": 4
		},
		{
			"NAME": "glow",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0.2,
			"MAX": 3
		},
		{
			"NAME": "glassTexture",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1,
			"LABEL": "Texture"
		},
		{
			"NAME": "sunX",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "sunY",
			"TYPE": "float",
			"DEFAULT": 0.75,
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
// Fractilian Stained Glass
// fractal cells filled with jewel glass (ruby, sapphire, emerald, amber, amethyst) between dark leading, backlit with a soft sun.
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

float hash1(float n) { return fract(sin(n*12.9898) * 43758.5453); }
float noise(vec2 p) {
	vec2 i = floor(p), f = fract(p);
	f = f*f*(3.0-2.0*f);
	float a = fract(sin(dot(i, vec2(127.1, 311.7)))*43758.5453);
	float b = fract(sin(dot(i + vec2(1.0, 0.0), vec2(127.1, 311.7)))*43758.5453);
	float c = fract(sin(dot(i + vec2(0.0, 1.0), vec2(127.1, 311.7)))*43758.5453);
	float d = fract(sin(dot(i + vec2(1.0, 1.0), vec2(127.1, 311.7)))*43758.5453);
	return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}
vec3 jewel(float k) {
	k = floor(k*5.0);
	if (k < 1.0) return vec3(0.75, 0.05, 0.12);
	if (k < 2.0) return vec3(0.08, 0.2, 0.75);
	if (k < 3.0) return vec3(0.05, 0.55, 0.25);
	if (k < 4.0) return vec3(0.95, 0.6, 0.08);
	return vec3(0.5, 0.12, 0.65);
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
	float drift = mix(pos.x+sin(TT),sin(pos.x)*cos(pos.x-TT),sin(pos.y-TT));
	vec2 spin = vec2( sin(T*r1), -sin(T*r2))+center;
	vec2 p = pos;
	float m = 10000.0, b = 0.0, best = 10000.0, bi = 0.0, ba = 0.0;
	for(int i=0; i<100; i++) {
		p.x = abs(p.x);
		p = p * scale + spin;
		p = CircleInversion(p, vec2(0.5, 0.5), 1.0);
		float l = length(p);
		if (l < best) { best = l; bi = b; ba = atan(p.y - 0.5, p.x - 0.5); }
		m = min(l, m);
		m += Parabola(m, drift);
		b += 1.0;
		if (b>loops) break;
	}
	float cell = bi*7.0 + floor((ba/6.28318 + 0.5)*facets) + floor(log(best + 1e-4)*1.5)*13.0;
	vec3 glass = jewel(hash1(cell));
	float shade = 0.65 + 0.35*hash1(cell + 3.1);
	vec2 sun = vec2(sunX*RENDERSIZE.x/RENDERSIZE.y, sunY);
	float light = glow * (0.45 + 0.9*exp(-2.5*length(pos - sun))) * (1.0 + 0.35*aB + 0.15*aL);
	float tex = mix(1.0, 0.7 + 0.6*noise(gl_FragCoord.xy*0.08) * noise(gl_FragCoord.xy*0.013 + 3.0), glassTexture);
	vec3 col = glass * shade * light * tex;
	float w = fwidth(cell);
	float band = abs(fract(log(best + 1e-4)*1.5) - 0.5);
	float leadMask = max(step(0.5, w), 1.0 - smoothstep(0.0, 0.03*lead + 1e-3, 0.5 - band));
	col = mix(col, vec3(0.015, 0.012, 0.01), clamp(leadMask*step(0.01, lead), 0.0, 1.0));
	col = 1.0 - exp(-col*1.8);
	gl_FragColor = vec4(col, 1.0);
}
