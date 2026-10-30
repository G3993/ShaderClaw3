/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea variant, after mojovideotech",
	"DESCRIPTION": "Fractilian Coral — the hard abs fold softened so the fractal grows like coral and anemones, in coral/teal/sand under drifting caustic light and marine snow. Softness, caustics, depth, snow and three colours; bass breathes the reef.",
	"CATEGORIES": [
		"Generator",
		"Fractal",
		"Organic",
		"Underwater",
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
			"DEFAULT": 1.9,
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
			"NAME": "softness",
			"TYPE": "float",
			"DEFAULT": 0.04,
			"MIN": 0,
			"MAX": 0.3
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 1.2,
			"MIN": 0.1,
			"MAX": 5
		},
		{
			"NAME": "caustics",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0,
			"MAX": 1.5
		},
		{
			"NAME": "depth",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "snow",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "coral",
			"TYPE": "color",
			"DEFAULT": [
				1,
				0.42,
				0.35,
				1
			],
			"LABEL": "Coral"
		},
		{
			"NAME": "sea",
			"TYPE": "color",
			"DEFAULT": [
				0.04,
				0.42,
				0.5,
				1
			],
			"LABEL": "Sea"
		},
		{
			"NAME": "sand",
			"TYPE": "color",
			"DEFAULT": [
				0.98,
				0.86,
				0.62,
				1
			],
			"LABEL": "Sand"
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
// Fractilian Coral
// the hard abs fold softened so the fractal grows like coral and anemones, in coral/teal/sand under drifting caustic light and marine snow.
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
float caustic(vec2 p, float t) {
	float c = 0.0;
	for (int k = 0; k < 3; k++) {
		float fk = float(k);
		p += vec2(sin(p.y*1.7 + t*(0.6 + 0.2*fk)), cos(p.x*1.3 - t*(0.5 + 0.15*fk)))*0.35;
		c += abs(sin(p.x*2.1 + p.y*1.4 + t));
	}
	return pow(1.0 - c/3.0, 4.0);
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
	float sc = scale*(1.0 + 0.04*aB);
	float l = 0.0, b = 0.0, m = 10000.0;
	vec2 p = pos;
	for(int i=0; i<100; i++) {
		p.x = sqrt(p.x*p.x + softness*softness);
		p = p * sc + spin;
		p = CircleInversion(p, vec2(0.5, 0.5), 1.0);
		l = length(p);
		m = min(l, m);
		m += Parabola(m, drift);
		b += 1.0;
		if (b>loops) break;
	}
	float body = 1.0 - exp(-l*l*brightness*0.3);
	float rim = fract(m*body + m);
	vec3 col = mix(sea.rgb*0.35, coral.rgb, smoothstep(0.15, 0.85, body));
	col = mix(col, sand.rgb, smoothstep(0.55, 1.0, rim)*body*0.7);
	col *= 0.6 + 0.4*rim;
	float y = gl_FragCoord.y / RENDERSIZE.y;
	col = mix(col, sea.rgb*(0.25 + 0.6*y), depth*(1.0 - body)*0.8);
	float cs = caustic(pos*6.0, TIME*0.7);
	col += caustics * cs * mix(vec3(0.6, 0.95, 1.0), sand.rgb, 0.3) * (0.3 + 0.7*y) * (1.0 + 0.3*aL);
	vec2 sg = floor(gl_FragCoord.xy/3.0 + vec2(0.0, TIME*4.0));
	float fl = step(0.997, hash(sg)) * snow;
	col += fl*0.6;
	col = 1.0 - exp(-col*1.3);
	gl_FragColor = vec4(col, 1.0);
}
