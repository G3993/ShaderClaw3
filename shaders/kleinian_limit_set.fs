/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after Jos Leys' Maskit-slice algorithm",
	"DESCRIPTION": "Kleinian Limit Set — the lace left behind by a Kleinian group (a translation plus a circle inversion, Maskit slice), the 'Indra's Pearls' fractal. The trace slowly wanders so the necklaces breathe and re-link. Looks: Stained Glass, Pearl Ink, Neon. Bass bends the group, highs glint the lace.",
	"CATEGORIES": [ "Generator", "Fractal", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Stained Glass", "Pearl Ink", "Neon" ] },
		{ "NAME": "traceA", "LABEL": "Trace (height)", "TYPE": "float", "DEFAULT": 1.94, "MIN": 1.82, "MAX": 2.02 },
		{ "NAME": "traceB", "LABEL": "Trace (twist)", "TYPE": "float", "DEFAULT": 0.03, "MIN": -0.3, "MAX": 0.3 },
		{ "NAME": "wander", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 6.0 },
		{ "NAME": "pan", "TYPE": "float", "DEFAULT": 0.04, "MIN": -0.5, "MAX": 0.5 },
		{ "NAME": "iters", "LABEL": "Iterations", "TYPE": "float", "DEFAULT": 70.0, "MIN": 10.0, "MAX": 150.0 },
		{ "NAME": "lead", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
		{ "NAME": "glow", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Kleinian Limit Set
// Maskit slice: generators z -> z + 2 and z -> t + 1/z, t = -b + i a.
// Each step folds the point into the fundamental strip, half-turns it
// across the separating line, then applies the inversion generator.
// Points that leave the strip 0 <= Im z <= a escape; the escape count
// tiles the plane and the limit set is what never escapes.
////////////////////////////////////////////////////////////

float hash1(float n) { return fract(sin(n*12.9898) * 43758.5453); }

vec3 jewel(float k) {
	k = floor(fract(k)*6.0);
	if (k < 1.0) return vec3(0.78, 0.06, 0.12);
	if (k < 2.0) return vec3(0.07, 0.22, 0.8);
	if (k < 3.0) return vec3(0.05, 0.56, 0.28);
	if (k < 4.0) return vec3(0.97, 0.62, 0.08);
	if (k < 5.0) return vec3(0.52, 0.12, 0.68);
	return vec3(0.1, 0.62, 0.7);
}

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

// Returns the tile index: iterations until the orbit settles into a 2-cycle
// (inside the domain of discontinuity) or leaves the strip (+300 / +600).
// -1 = never settled within iters: the limit set itself.
// trap = closest the orbit came to the strip edges.
float kleinian(vec2 z, float a, float b, out float trap) {
	float f = sign(b);
	trap = 1e9;
	vec2 z1 = vec2(1e5), z2 = vec2(1e5);
	for (int i = 0; i < 150; i++) {
		if (float(i) >= iters) break;
		z.x += b/a*z.y;
		z.x -= 2.0*floor((z.x + 1.0)*0.5);
		z.x -= b/a*z.y;
		float sep = a*0.5 + f*(2.0*a - 1.95)*0.25*sign(z.x + b*0.5)
			* (1.0 - exp(-(7.2 - (1.95 - a)*15.0)*abs(z.x + b*0.5)));
		if (z.y >= sep) z = vec2(-b, a) - z;
		z = -z / dot(z, z);
		z.x = -b - z.x;
		z.y = a + z.y;
		if (z.y < 0.0) return float(i) + 300.0;
		if (z.y > a) return float(i) + 600.0;
		trap = min(trap, min(z.y, a - z.y));
		if (dot(z - z2, z - z2) < 1e-8) return float(i);
		z2 = z1; z1 = z;
	}
	return -1.0;
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	float T = TIME*rate;
	float a = traceA + wander*0.035*sin(T*1.3) - 0.02*aB;
	float b = traceB + wander*0.06*sin(T*0.71 + 1.0);

	// The limit set lives in the strip 0..a and repeats every 2 in x:
	// fit the strip to the screen height and scroll along it.
	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	vec2 z = vec2(uv.x*a/zoom + TIME*pan, uv.y*a/zoom + 0.5*a);

	float trap, tx, ty;
	float n = kleinian(z, a, b, trap);
	// Lead lines: the tile index differs from a neighbour (lead px wide).
	float px = a/zoom/RENDERSIZE.y*max(lead, 0.01);
	float nx = kleinian(z + vec2(px, 0.0), a, b, tx);
	float ny = kleinian(z + vec2(0.0, px), a, b, ty);
	bool inside = n < 0.0;
	float edge = (inside || n != nx || n != ny) ? 1.0 : 0.0;
	float lace = exp(-trap*60.0/glow);

	vec3 col;
	int lk = int(look);
	if (lk == 0) {
		float cell = n + 17.0*floor(z.x*0.5 + 0.5);
		vec3 g = jewel(hash1(n*3.1 + hue*7.0));
		g *= 0.6 + 0.4*hash1(cell + 2.0);
		float sun = 0.55 + 0.6*exp(-2.5*length(uv - vec2(0.25, 0.3)));
		col = g * sun * glow * (1.0 + 0.3*aL);
		col = mix(col, vec3(0.012, 0.01, 0.008), edge*step(0.01, lead));
		col += vec3(1.0, 0.9, 0.7) * lace * 0.25 * (1.0 + 2.0*aH) * (inside ? 0.0 : 1.0);
		col = 1.0 - exp(-col*1.8);
	} else if (lk == 1) {
		vec3 paper = vec3(0.95, 0.93, 0.88);
		vec3 wash = mix(paper, pal(hue + n*0.04)*0.35 + 0.6, 0.35*(1.0 - exp(-n*0.08)));
		col = mix(wash, vec3(0.05, 0.05, 0.08), max(edge*0.85, inside ? 1.0 : 0.0));
		col = mix(col, vec3(0.75, 0.6, 0.35), lace*0.4*(1.0 + aH));
	} else {
		vec3 tint = pal(hue + n*0.035 + TIME*0.01);
		col = tint * (0.04 + 0.5*exp(-n*0.05)) * glow;
		col += tint * edge * 1.2 * (1.0 + aH);
		col += vec3(0.8, 0.9, 1.0) * lace * 0.6 * glow * (1.0 + 0.4*aL);
		col = 1.0 - exp(-col*1.5);
	}
	gl_FragColor = vec4(col, 1.0);
}
