/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after Michelin & Rossetto's Burning Ship fractal",
	"DESCRIPTION": "Burning Ship — the Mandelbrot formula with absolute values (z <- (|x| + i|y|)^2 + c), which turns the set into a burning ship with an armada of smaller ships and masts trailing off its bow. A slow dive from the whole ship into the armada and back. Looks: Fire, Ice, Gold. Bass stokes the flames, highs light the rigging.",
	"CATEGORIES": [ "Generator", "Fractal", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Fire", "Ice", "Gold" ] },
		{ "NAME": "target", "TYPE": "point2D", "DEFAULT": [ -1.7615, -0.0285 ], "MIN": [ -2.0, -1.0 ], "MAX": [ 1.0, 1.0 ] },
		{ "NAME": "zoomOut", "LABEL": "Wide View", "TYPE": "float", "DEFAULT": 2.4, "MIN": 0.05, "MAX": 4.0 },
		{ "NAME": "zoomIn", "LABEL": "Close View", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.002, "MAX": 1.0 },
		{ "NAME": "diveRate", "LABEL": "Dive Rate", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.3 },
		{ "NAME": "iters", "LABEL": "Iterations", "TYPE": "float", "DEFAULT": 160.0, "MIN": 40.0, "MAX": 400.0 },
		{ "NAME": "bands", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.01, "MAX": 0.5 },
		{ "NAME": "flow", "TYPE": "float", "DEFAULT": 0.04, "MIN": -0.5, "MAX": 0.5 },
		{ "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0 },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Burning Ship
// Escape-time with smooth iteration count; imaginary axis flipped so the
// ship sails upright. View width eases (log-space) between Wide and Close
// while the centre slides from the whole ship to the target.
////////////////////////////////////////////////////////////

vec3 fire(float t) {
	t = clamp(t, 0.0, 1.0);
	vec3 c = mix(vec3(0.0), vec3(0.6, 0.02, 0.0), smoothstep(0.0, 0.25, t));
	c = mix(c, vec3(1.0, 0.35, 0.0), smoothstep(0.2, 0.5, t));
	c = mix(c, vec3(1.0, 0.85, 0.25), smoothstep(0.45, 0.8, t));
	return mix(c, vec3(1.0, 1.0, 0.9), smoothstep(0.8, 1.0, t));
}
vec3 ice(float t) {
	t = clamp(t, 0.0, 1.0);
	vec3 c = mix(vec3(0.0, 0.01, 0.04), vec3(0.05, 0.15, 0.45), smoothstep(0.0, 0.3, t));
	c = mix(c, vec3(0.2, 0.6, 0.95), smoothstep(0.25, 0.6, t));
	return mix(c, vec3(0.92, 0.98, 1.0), smoothstep(0.6, 1.0, t));
}
vec3 gold(float t) {
	t = clamp(t, 0.0, 1.0);
	vec3 c = mix(vec3(0.02, 0.01, 0.0), vec3(0.35, 0.18, 0.03), smoothstep(0.0, 0.3, t));
	c = mix(c, vec3(0.95, 0.7, 0.25), smoothstep(0.3, 0.7, t));
	return mix(c, vec3(1.0, 0.97, 0.85), smoothstep(0.75, 1.0, t));
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float k = 0.5 - 0.5*cos(TIME*diveRate);             // 0 wide .. 1 close
	k = smoothstep(0.0, 1.0, k);
	float width = exp(mix(log(zoomOut), log(zoomIn), k));
	vec2 wideCentre = vec2(-0.5, -0.45);
	vec2 centre = mix(wideCentre, target, smoothstep(0.0, 0.6, k));
	vec2 c = centre + vec2(uv.x, -uv.y)*width;   // screen up = -Im: the ship sails upright

	vec2 z = vec2(0.0);
	float n = 0.0;
	float m2 = 0.0;
	for (int i = 0; i < 400; i++) {
		if (float(i) >= iters) break;
		z = abs(z);
		z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
		m2 = dot(z, z);
		if (m2 > 256.0) break;
		n += 1.0;
	}
	vec3 col;
	if (n >= iters) {
		col = vec3(0.0);
	} else {
		float sn = n - log2(log2(m2)) + 4.0;
		// Monotone heat (no repeating bands to alias in the chaos) plus a soft ripple.
		float heat = 1.0 - exp(-sn*bands*0.35);
		heat += 0.08*sin(sn*bands*6.0 - TIME*flow*6.28);
		heat *= brightness * (1.0 + 0.35*aB);
		float t = fract(sn*bands - TIME*flow);
		int lk = int(look);
		col = lk == 0 ? fire(heat) : (lk == 1 ? ice(heat) : gold(heat));
		col += vec3(1.0, 0.9, 0.7) * smoothstep(0.92, 1.0, t) * 0.4 * aH;
		col *= 1.0 + 0.15*aL;
	}
	gl_FragColor = vec4(col, 1.0);
}
