/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea",
	"DESCRIPTION": "Newton Nova — Newton's method for z^n = 1 run on every pixel: each point is coloured by the root it falls into, and the basin borders braid into endless fractal chains. A complex relaxation factor slowly orbits, and Nova mode adds a drifting constant that breaks the symmetry into spirals. Degree, relaxation, mode, palette, shading. Bass swings the relaxation, highs light the borders.",
	"CATEGORIES": [ "Generator", "Fractal", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "mode", "LABEL": "Mode", "TYPE": "long", "DEFAULT": 1, "VALUES": [ 0, 1 ], "LABELS": [ "Newton", "Nova" ] },
		{ "NAME": "degree", "TYPE": "float", "DEFAULT": 5.0, "MIN": 3.0, "MAX": 8.0 },
		{ "NAME": "relax", "LABEL": "Relaxation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 2.0 },
		{ "NAME": "swirl", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "nova", "LABEL": "Nova Drift", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "zoom", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.3, "MAX": 8.0 },
		{ "NAME": "spin", "TYPE": "float", "DEFAULT": 0.015, "MIN": -0.3, "MAX": 0.3 },
		{ "NAME": "iters", "LABEL": "Iterations", "TYPE": "float", "DEFAULT": 48.0, "MIN": 10.0, "MAX": 120.0 },
		{ "NAME": "shade", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "borders", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "saturation", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 1.5 },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Newton Nova
// z <- z - R (z^n - 1) / (n z^(n-1)) + c, R complex.
// Newton: z0 = pixel, c = 0. Nova (Julia form): z0 = pixel, c drifts.
////////////////////////////////////////////////////////////

vec2 cmul(vec2 a, vec2 b) { return vec2(a.x*b.x - a.y*b.y, a.x*b.y + a.y*b.x); }
vec2 cdiv(vec2 a, vec2 b) { return vec2(a.x*b.x + a.y*b.y, a.y*b.x - a.x*b.y) / max(dot(b, b), 1e-12); }
vec2 cpow(vec2 z, float n) {
	float r = length(z);
	float t = atan(z.y, z.x);
	return pow(max(r, 1e-12), n) * vec2(cos(n*t), sin(n*t));
}
vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float ra = TIME*spin;
	vec2 z = mat2(cos(ra), -sin(ra), sin(ra), cos(ra)) * uv * zoom;

	float n = floor(degree + 0.5);
	float T = TIME*rate;
	vec2 R = vec2(relax, 0.0) + swirl*(0.45 + 0.25*aB)*vec2(cos(T), sin(T*1.37));
	vec2 c = int(mode) == 1 ? nova*0.9*vec2(cos(T*0.73 + 1.0), sin(T*0.51)) : vec2(0.0);

	float it = 0.0;
	float settle = 0.0;
	for (int i = 0; i < 120; i++) {
		if (float(i) >= iters) break;
		vec2 zn1 = cpow(z, n - 1.0);
		vec2 f = cmul(zn1, z) - vec2(1.0, 0.0);
		vec2 df = n*zn1;
		vec2 dz = cmul(R, cdiv(f, df));
		z = z - dz + c;
		float s = dot(dz, dz);
		if (s < 1e-7) { settle = s; break; }
		it += 1.0;
	}
	// Smooth iteration count from the last step size.
	float smoothIt = it - log2(max(log(max(settle, 1e-30)) / log(1e-7), 1e-3));
	float root = floor(fract(atan(z.y, z.x)/6.28318 + 0.5/n) * n);

	vec3 base = pal(hue + root/n);
	base = mix(vec3(dot(base, vec3(0.299, 0.587, 0.114))), base, saturation);
	float lum = mix(1.0, exp(-smoothIt*0.08), shade);
	vec3 col = base * lum * (1.0 + 0.25*aL);
	float b = smoothstep(0.35, 1.0, smoothIt / max(iters*0.5, 1.0));
	col = mix(col, vec3(1.0, 0.95, 0.85), b*borders*0.5*(1.0 + aH));
	col *= 0.55 + 0.45*(0.5 + 0.5*cos(smoothIt*0.9));
	col = 1.0 - exp(-col*1.6);
	gl_FragColor = vec4(col, 1.0);
}
