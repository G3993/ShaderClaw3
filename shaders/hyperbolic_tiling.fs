/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea",
	"DESCRIPTION": "Hyperbolic Tiling — an Escher 'Circle Limit' {p,q} tiling of the hyperbolic plane, built by reflecting each point into one triangle across two lines and a circle (circle inversion again). The tiles shrink endlessly toward the edge while the plane glides and turns. Models: Poincaré Disk, Band (fills a wide screen), Half-Plane. Looks: Stained Glass, Two-Tone, Neon. Bass glides the plane, highs light the lead.",
	"CATEGORIES": [ "Generator", "Fractal", "Geometry", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "model", "LABEL": "Model", "TYPE": "long", "DEFAULT": 1, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Poincaré Disk", "Band", "Half-Plane" ] },
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Stained Glass", "Two-Tone", "Neon" ] },
		{ "NAME": "pSides", "LABEL": "p (polygon sides)", "TYPE": "float", "DEFAULT": 7.0, "MIN": 3.0, "MAX": 12.0 },
		{ "NAME": "qMeet", "LABEL": "q (meet at vertex)", "TYPE": "float", "DEFAULT": 3.0, "MIN": 3.0, "MAX": 12.0 },
		{ "NAME": "glide", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.95 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "spin", "TYPE": "float", "DEFAULT": 0.03, "MIN": -0.3, "MAX": 0.3 },
		{ "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
		{ "NAME": "lead", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
		{ "NAME": "glow", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.2, "MAX": 3.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Hyperbolic Tiling
// Fundamental triangle: y >= 0, below the line at angle pi/p, and outside
// the circle C (center (d,0), radius r) orthogonal to the unit circle that
// meets that line at angle pi/q:
//   r = sin(pi/p)/sqrt(cos^2(pi/q) - sin^2(pi/p)), d = sqrt(1 + r^2).
// Reflect until the point lands in the triangle; the reflection count
// identifies the tile.
////////////////////////////////////////////////////////////

vec2 cmul(vec2 a, vec2 b) { return vec2(a.x*b.x - a.y*b.y, a.x*b.y + a.y*b.x); }
vec2 cdiv(vec2 a, vec2 b) { return vec2(a.x*b.x + a.y*b.y, a.y*b.x - a.x*b.y) / max(dot(b, b), 1e-12); }
vec2 cexp(vec2 z) { return exp(z.x)*vec2(cos(z.y), sin(z.y)); }
float hash1(float n) { return fract(sin(n*12.9898) * 43758.5453); }
vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }
vec3 jewel(float k) {
	k = floor(fract(k)*6.0);
	if (k < 1.0) return vec3(0.78, 0.06, 0.12);
	if (k < 2.0) return vec3(0.07, 0.22, 0.8);
	if (k < 3.0) return vec3(0.05, 0.56, 0.28);
	if (k < 4.0) return vec3(0.97, 0.62, 0.08);
	if (k < 5.0) return vec3(0.52, 0.12, 0.68);
	return vec3(0.1, 0.62, 0.7);
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float pix = 1.0 / RENDERSIZE.y;

	// Screen -> Poincaré disk.
	vec2 z;
	float scaleJ = 1.0;   // rough |dz/dscreen| for line widths
	int md = int(model);
	if (md == 0) {
		z = uv * 2.0 / zoom * 0.98;
		scaleJ = 2.0/zoom*0.98;
	} else if (md == 1) {
		// Band model: tanh maps the strip |Im w| < pi/4 onto the disk.
		vec2 w = uv * (1.5707963 / zoom);
		vec2 e = cexp(2.0*w);
		z = cdiv(e - vec2(1.0, 0.0), e + vec2(1.0, 0.0));
		scaleJ = 1.5707963/zoom * length(vec2(1.0, 0.0) - cmul(z, z));
	} else {
		// Upper half-plane -> disk (Cayley).
		vec2 w = vec2(uv.x, uv.y + 0.5) * (2.0/zoom) + vec2(0.0, 0.002);
		z = cdiv(w - vec2(0.0, 1.0), w + vec2(0.0, 1.0));
		scaleJ = 2.0/zoom * 2.0 / max(dot(w + vec2(0.0, 1.0), w + vec2(0.0, 1.0)), 1e-6);
	}
	float outside = step(1.0, dot(z, z));
	float pre = max(1.0 - dot(z, z), 1e-6);

	// Hyperbolic motion: rotate, then translate by a gliding point (Möbius).
	float T = TIME*rate;
	float ra = TIME*spin;
	z = cmul(z, vec2(cos(ra), sin(ra)));
	vec2 g = (glide*0.6 + 0.08*aB) * vec2(sin(T), sin(T*0.71 + 0.6));
	z = cdiv(z + g, vec2(1.0, 0.0) + cmul(vec2(g.x, -g.y), z));

	float P = floor(pSides + 0.5), Q = floor(qMeet + 0.5);
	if ((P - 2.0)*(Q - 2.0) <= 4.0) Q = floor(4.0/(P - 2.0)) + 3.0;   // keep it hyperbolic
	float A = 3.14159265/P, B = 3.14159265/Q;
	float den = sqrt(max(cos(B)*cos(B) - sin(A)*sin(A), 1e-6));
	float r = sin(A)/den;
	vec2 cc = vec2(cos(B)/den, 0.0);
	vec2 nrm = vec2(-sin(A), cos(A));

	float flips = 0.0, inv = 0.0, rot = 0.0;
	for (int i = 0; i < 60; i++) {
		bool moved = false;
		if (z.y < 0.0) { z.y = -z.y; flips += 1.0; rot -= 1.0; moved = true; }
		float dl = dot(z, nrm);
		if (dl > 0.0) { z -= 2.0*dl*nrm; flips += 1.0; rot += 1.0; moved = true; }
		vec2 dz = z - cc;
		float d2 = dot(dz, dz);
		if (d2 < r*r) { z = cc + dz*r*r/d2; flips += 1.0; inv += 1.0; moved = true; }
		if (!moved) break;
	}

	// Distance to the triangle's sides in the disk, then to screen pixels via the
	// hyperbolic metric factor (1 - |z|^2) the tiles shrink by.
	float dSide = min(z.y, -dot(z, nrm));
	float dCirc = length(z - cc) - r;
	// Hyperbolic isometries scale lengths by (1 - |z_after|^2)/(1 - |z_before|^2).
	float lw = lead * 1.6 * pix * scaleJ * max(1.0 - dot(z, z), 1e-6) / pre;
	float tileEdge = 1.0 - smoothstep(lw*0.5, lw*1.5, dCirc);
	float spokes = 1.0 - smoothstep(lw*0.3, lw*0.9, dSide);
	float parity = mod(flips, 2.0);

	vec3 col;
	int lk = int(look);
	if (lk == 0) {
		float id = inv*1.37 + floor(mod(rot, 2.0*P)*0.5)*0.21;
		vec3 jw = jewel(hash1(id + hue*5.0));
		jw *= 0.75 + 0.25*parity;
		float sun = 0.5 + 0.7*exp(-2.0*length(uv - vec2(0.3, 0.25)));
		col = jw * sun * glow * (1.0 + 0.3*aL);
		col = mix(col, col*0.75, spokes*0.6);
		col = mix(col, vec3(0.012, 0.01, 0.008), tileEdge);
		col = 1.0 - exp(-col*1.8);
	} else if (lk == 1) {
		vec3 a = vec3(0.95, 0.93, 0.88), b = vec3(0.08, 0.08, 0.12);
		col = mix(a, b, parity);
		col = mix(col, vec3(0.75, 0.2, 0.12), tileEdge*0.8);
	} else {
		vec3 t = pal(hue + inv*0.08 + TIME*0.01);
		col = t * 0.06 * (1.0 + parity);
		col += t * (tileEdge*1.4 + spokes*0.4) * glow * (1.0 + aH);
		col = 1.0 - exp(-col*1.6);
	}
	col = mix(col, vec3(0.0), outside);
	gl_FragColor = vec4(col, 1.0);
}
