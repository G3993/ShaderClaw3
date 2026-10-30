/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after White & Nylander's Mandelbulb",
	"DESCRIPTION": "Mandelbulb Bloom — the 3D Mandelbulb (z <- z^n + c in spherical coordinates), raymarched and lit, its power slowly breathing so the bulb blooms from smooth petals into deep fractal coral and back. Looks: Coral, Jewel, Neon Rim. Power, bloom, camera orbit. Bass pushes the power, highs light the rims.",
	"CATEGORIES": [ "Generator", "Fractal", "3D", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Coral", "Jewel", "Neon Rim" ] },
		{ "NAME": "power", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 12.0 },
		{ "NAME": "bloom", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "orbit", "LABEL": "Camera Orbit", "TYPE": "float", "DEFAULT": 0.06, "MIN": -0.5, "MAX": 0.5 },
		{ "NAME": "camDistance", "LABEL": "Distance", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.4, "MAX": 5.0 },
		{ "NAME": "iters", "LABEL": "Iterations", "TYPE": "float", "DEFAULT": 7.0, "MIN": 3.0, "MAX": 12.0 },
		{ "NAME": "quality", "TYPE": "float", "DEFAULT": 110.0, "MIN": 40.0, "MAX": 200.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "background", "TYPE": "color", "DEFAULT": [ 0.03, 0.03, 0.06, 1.0 ] },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Mandelbulb Bloom
// w <- w^n + c with w^n = r^n (sin(n theta) sin(n phi), cos(n theta), sin(n theta) cos(n phi)).
// DE = 0.25 log(r^2) r / dr. Orbit trap (|w|, r^2) for colour and occlusion.
////////////////////////////////////////////////////////////

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

float gPow;

float de(vec3 p, out vec4 trap) {
	vec3 w = p;
	float m = dot(w, w);
	trap = vec4(abs(w), m);
	float dz = 1.0;
	for (int i = 0; i < 12; i++) {
		if (float(i) >= iters) break;
		dz = gPow*pow(m, (gPow - 1.0)*0.5)*dz + 1.0;
		float r = sqrt(m);
		float th = gPow*acos(clamp(w.y/r, -1.0, 1.0));
		float ph = gPow*atan(w.x, w.z);
		w = p + pow(r, gPow)*vec3(sin(th)*sin(ph), cos(th), sin(th)*cos(ph));
		trap = min(trap, vec4(abs(w), m));
		m = dot(w, w);
		if (m > 256.0) break;
	}
	return 0.25*log(m)*sqrt(m)/dz;
}

float deOnly(vec3 p) { vec4 t; return de(p, t); }

vec3 normalAt(vec3 p, float e) {
	vec2 k = vec2(1.0, -1.0);
	return normalize(k.xyy*deOnly(p + k.xyy*e) + k.yyx*deOnly(p + k.yyx*e)
		+ k.yxy*deOnly(p + k.yxy*e) + k.xxx*deOnly(p + k.xxx*e));
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float T = TIME*rate;
	gPow = power + bloom*2.5*sin(T) + 0.4*aB;

	float ca = TIME*orbit + 0.6;
	vec3 ro = vec3(camDistance*sin(ca), 0.6*sin(ca*0.6) + 0.3, camDistance*cos(ca));
	vec3 ww = normalize(-ro);
	vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
	vec3 vv = cross(uu, ww);
	vec3 rd = normalize(uv.x*uu + uv.y*vv + 1.8*ww);

	// Start at the bounding sphere (radius 1.25).
	float b = dot(ro, rd), c = dot(ro, ro) - 1.5625;
	float h = b*b - c;
	vec3 bg = background.rgb + vec3(0.04, 0.03, 0.07)*(1.0 - length(uv));
	vec3 col = bg;
	float glowAmt = 0.0;
	if (h > 0.0) {
		float t = max(-b - sqrt(h), 0.0), tEnd = -b + sqrt(h);
		vec4 trap = vec4(0.0);
		bool hit = false;
		float steps = 0.0, minD = 1e9;
		for (int i = 0; i < 200; i++) {
			if (float(i) >= quality) break;
			float d = de(ro + rd*t, trap);
			minD = min(minD, d);
			if (d < 0.0005*t) { hit = true; break; }
			t += d;
			steps += 1.0;
			if (t > tEnd) break;
		}
		glowAmt = exp(-minD*25.0);
		if (hit) {
			vec3 p = ro + rd*t;
			vec3 n = normalAt(p, 0.0003*t);
			vec3 L = normalize(vec3(0.577, 0.577, -0.577));
			float diff = max(dot(n, L), 0.0);
			float amb = 0.6 + 0.4*n.y;
			float ao = clamp(trap.w*1.6, 0.0, 1.0) * (1.0 - steps/max(quality, 1.0)*0.6);
			float spec = pow(max(dot(reflect(rd, n), L), 0.0), 24.0);
			float rim = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
			int lk = int(look);
			if (lk == 0) {
				vec3 base = vec3(0.01);
				base = mix(base, vec3(0.10, 0.20, 0.30), clamp(trap.y, 0.0, 1.0));
				base = mix(base, vec3(0.02, 0.10, 0.30), clamp(trap.z*trap.z, 0.0, 1.0));
				base = mix(base, vec3(0.30, 0.10, 0.02), clamp(pow(trap.w, 6.0), 0.0, 1.0));
				base = mix(base, pal(hue)*0.6, 0.3);
				base *= 3.0;
				col = base*(0.4*amb*ao + 1.2*diff*ao) + spec*0.6*ao + rim*0.3*base;
			} else if (lk == 1) {
				vec3 base = pal(hue + 0.6*trap.w + 0.3*trap.x);
				col = base*(0.25*amb + 0.9*diff)*ao + spec*(0.8 + aH)*ao + rim*base*0.5;
			} else {
				col = bg + pal(hue + trap.w*0.8) * (rim*1.8 + 0.04) * (1.0 + aH);
			}
		}
	}
	col += pal(hue + 0.5) * glowAmt * 0.12 * (1.0 + 0.5*aL);
	col = 1.0 - exp(-col*1.3);
	gl_FragColor = vec4(pow(col, vec3(0.85)), 1.0);
}
