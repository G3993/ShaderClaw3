/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after Markus & Hess (Lyapunov fractals) and Zircon Zity",
	"DESCRIPTION": "Lyapunov Zircon — the Markus–Lyapunov fractal: the logistic map x <- r x (1 - x) is driven by alternating growth rates a and b, and every pixel shows whether that rhythm settles (gold, embossed like carved amber) or goes chaotic (deep blue). Slow drift through the 'Zircon Zity' region. Pattern lengths, light angle, relief, colours. Bass deepens the relief, highs flash the chaos edges.",
	"CATEGORIES": [ "Generator", "Fractal", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "countA", "LABEL": "A Steps", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 8.0 },
		{ "NAME": "countB", "LABEL": "B Steps", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 8.0 },
		{ "NAME": "focus", "TYPE": "point2D", "DEFAULT": [ 2.95, 3.6 ], "MIN": [ 2.0, 2.0 ], "MAX": [ 4.0, 4.0 ] },
		{ "NAME": "span", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.02, "MAX": 2.0 },
		{ "NAME": "drift", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "iters", "LABEL": "Iterations", "TYPE": "float", "DEFAULT": 120.0, "MIN": 30.0, "MAX": 300.0 },
		{ "NAME": "relief", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
		{ "NAME": "lightAngle", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 6.283 },
		{ "NAME": "stableColor", "LABEL": "Stable", "TYPE": "color", "DEFAULT": [ 1.0, 0.78, 0.3, 1.0 ] },
		{ "NAME": "chaosColor", "LABEL": "Chaos", "TYPE": "color", "DEFAULT": [ 0.05, 0.12, 0.45, 1.0 ] },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Lyapunov Zircon
// lambda = mean log|r (1 - 2x)| over the orbit; < 0 stable, > 0 chaotic.
// Sequence: countA steps of a, then countB steps of b, repeated.
////////////////////////////////////////////////////////////

float lyap(vec2 ab, float na, float nb) {
	float x = 0.5;
	float lam = 0.0;
	float period = na + nb;
	for (int i = 0; i < 300; i++) {
		if (float(i) >= iters) break;
		float r = mod(float(i), period) < na ? ab.x : ab.y;
		x = r*x*(1.0 - x);
		if (i >= 10) lam += log(abs(r*(1.0 - 2.0*x)) + 1e-6);
	}
	return lam / max(iters - 10.0, 1.0);
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float T = TIME*rate;
	vec2 ctr = focus + drift*vec2(0.12*sin(T), 0.08*sin(T*0.77 + 1.3));
	float sp = span * (1.0 + 0.25*drift*sin(T*0.53));
	// Classic orientation: a grows to the right, b grows upward.
	vec2 ab = clamp(ctr + uv*sp, vec2(0.0), vec2(4.0));
	float na = floor(countA + 0.5), nb = floor(countB + 0.5);

	float lam = lyap(ab, na, nb);
	float e = sp / RENDERSIZE.y;
	float lx = lyap(clamp(ab + vec2(e, 0.0), vec2(0.0), vec2(4.0)), na, nb);
	float ly = lyap(clamp(ab + vec2(0.0, e), vec2(0.0), vec2(4.0)), na, nb);

	vec3 col;
	if (lam < 0.0) {
		// Height = how strongly it settles; light it as embossed amber.
		float h = 1.0 - exp(lam*2.0);
		float s = relief * (1.0 + 0.6*aB) * 4.0;
		vec3 nrm = normalize(vec3(-(lx - lam)*s/e*0.002, -(ly - lam)*s/e*0.002, 1.0));
		vec3 L = normalize(vec3(cos(lightAngle), sin(lightAngle), 0.9));
		float diff = max(dot(nrm, L), 0.0);
		float spec = pow(max(dot(reflect(-L, nrm), vec3(0.0, 0.0, 1.0)), 0.0), 20.0);
		col = stableColor.rgb * (0.15 + 0.85*h) * (0.35 + 0.75*diff) + spec*0.45;
	} else {
		float k = 1.0 - exp(-lam*3.0);
		col = mix(chaosColor.rgb, vec3(0.0), k);
		col += vec3(0.6, 0.8, 1.0) * exp(-lam*60.0) * 0.4 * (1.0 + 2.0*aH);
	}
	col *= 1.0 + 0.2*aL;
	gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
