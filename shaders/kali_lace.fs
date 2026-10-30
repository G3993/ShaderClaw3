/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after Kali's Kaliset (p = |p|/dot(p,p) - c)",
	"DESCRIPTION": "Kali Lace — the 2D Kaliset: fold to the positive quadrant, invert in the unit circle, shift, repeat. Fine glowing filaments knit into lace, spirals and filigree as the seed drifts. Looks: Lace (cosine palette on black), Gold Leaf, Ink. Bass shifts the seed, highs brighten the threads.",
	"CATEGORIES": [ "Generator", "Fractal", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Lace", "Gold Leaf", "Ink" ] },
		{ "NAME": "seed", "TYPE": "point2D", "DEFAULT": [ 0.8, 0.5 ], "MIN": [ 0.2, 0.2 ], "MAX": [ 1.2, 1.2 ] },
		{ "NAME": "wander", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "twist", "TYPE": "float", "DEFAULT": 0.12, "MIN": -1.0, "MAX": 1.0 },
		{ "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 6.0 },
		{ "NAME": "spin", "TYPE": "float", "DEFAULT": 0.02, "MIN": -0.3, "MAX": 0.3 },
		{ "NAME": "iters", "LABEL": "Iterations", "TYPE": "float", "DEFAULT": 10.0, "MIN": 4.0, "MAX": 40.0 },
		{ "NAME": "fineness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0 },
		{ "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.95, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "hueSpread", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.0, "MAX": 0.3 },
		{ "NAME": "mirror", "TYPE": "bool", "DEFAULT": true },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Kali Lace
// The Kaliset (Pablo Roman Andrioli / Kali): p = abs(p)/dot(p,p) - c.
// Glow accumulates where successive orbit radii barely change, which
// draws the filaments; the iteration index picks the colour.
////////////////////////////////////////////////////////////

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	if (mirror) uv.x = abs(uv.x);
	float ra = TIME*spin;
	uv = mat2(cos(ra), -sin(ra), sin(ra), cos(ra)) * uv;
	vec2 p = uv * zoom;

	float T = TIME*rate;
	vec2 c = seed + wander*vec2(0.09*sin(T*1.1), 0.07*cos(T*0.87)) + 0.03*aB;
	float ct = cos(twist), st = sin(twist);
	mat2 tw = mat2(ct, -st, st, ct);

	vec3 acc = vec3(0.0);
	float prev = 0.0, total = 0.0, trap = 1e9;
	for (int i = 0; i < 40; i++) {
		if (float(i) >= iters) break;
		p = abs(p) / dot(p, p) - c;
		p = tw * p;
		float l = length(p);
		float d = abs(l - prev);
		prev = l;
		float g = exp(-d * 6.0 / fineness);
		acc += pal(hue + float(i)*hueSpread) * g;
		total += g;
		trap = min(trap, abs(p.x*p.y));
	}
	acc /= max(iters*0.25, 1.0);
	total /= max(iters*0.25, 1.0);
	float k = brightness * (1.0 + 0.4*aL);

	vec3 col;
	int lk = int(look);
	if (lk == 0) {
		col = acc * k * 1.2;
		col += vec3(1.0) * exp(-trap*40.0) * 0.3 * (1.0 + 2.0*aH);
		col = 1.0 - exp(-col*col*1.8);
	} else if (lk == 1) {
		float v = 1.0 - exp(-total*total*k*1.5);
		vec3 gold = vec3(1.0, 0.76, 0.34);
		col = mix(vec3(0.04, 0.02, 0.03), gold*1.1, v);
		col += vec3(1.0, 0.95, 0.8) * exp(-trap*80.0) * 0.5 * (1.0 + 2.0*aH);
		col = clamp(col, 0.0, 1.0);
	} else {
		float v = 1.0 - exp(-total*total*k*1.2);
		col = mix(vec3(0.95, 0.93, 0.88), vec3(0.06, 0.06, 0.1), v);
		col = mix(col, vec3(0.8, 0.2, 0.12), exp(-trap*120.0)*0.6);
	}
	gl_FragColor = vec4(col, 1.0);
}
