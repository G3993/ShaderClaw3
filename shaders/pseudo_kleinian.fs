/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after Knighty's pseudo-Kleinian fractal (Theli-at's box size)",
	"DESCRIPTION": "Pseudo-Kleinian — a slow drift through the caverns of a 3D pseudo-Kleinian fractal: box folds and sphere inversions (3D cousins of the Kleinian necklaces) carve endless arches, pillars and lace walls. Lit by a lantern that travels with the camera. Looks: Marble, Gold, Neon. Bass breathes the lantern, highs sparkle.",
	"CATEGORIES": [ "Generator", "Fractal", "3D", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Marble", "Gold", "Neon" ] },
		{ "NAME": "speed", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.0, "MAX": 0.3 },
		{ "NAME": "boxSize", "LABEL": "Box Size", "TYPE": "float", "DEFAULT": 0.92, "MIN": 0.7, "MAX": 1.1 },
		{ "NAME": "thickness", "TYPE": "float", "DEFAULT": 0.93, "MIN": 0.5, "MAX": 1.2 },
		{ "NAME": "foldIters", "LABEL": "Fold Iterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 4.0, "MAX": 12.0 },
		{ "NAME": "quality", "TYPE": "float", "DEFAULT": 100.0, "MIN": 40.0, "MAX": 180.0 },
		{ "NAME": "fov", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.6, "MAX": 2.5 },
		{ "NAME": "lantern", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.0, "MAX": 3.0 },
		{ "NAME": "fog", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 2.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Pseudo-Kleinian
// p = 2 clamp(p, -C, C) - p ; k = max(1/dot(p,p), 1) ; p *= k
// DE = max(rxy - thickness, |rxy * p.z| / |p|) / scale
////////////////////////////////////////////////////////////

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

float de(vec3 p, out float trap) {
	vec3 C = vec3(boxSize, boxSize*0.982, boxSize);
	float s = 1.0;
	trap = 1e9;
	for (int i = 0; i < 12; i++) {
		if (float(i) >= foldIters) break;
		p = 2.0*clamp(p, -C, C) - p;
		float r2 = dot(p, p);
		float k = max(1.0/r2, 1.0);
		p *= k;
		s *= k;
		trap = min(trap, r2);
	}
	float rxy = length(p.xy);
	return 0.5*max(rxy - thickness, abs(rxy*p.z)/length(p)) / s;
}

float deOnly(vec3 p) { float t; return de(p, t); }

vec3 normalAt(vec3 p, float e) {
	vec2 k = vec2(1.0, -1.0);
	return normalize(k.xyy*deOnly(p + k.xyy*e) + k.yyx*deOnly(p + k.yyx*e)
		+ k.yxy*deOnly(p + k.yxy*e) + k.xxx*deOnly(p + k.xxx*e));
}

vec3 camPath(float t) {
	return vec3(0.6*sin(t*0.37), 0.35 + 0.25*sin(t*0.29 + 1.0), 0.6*cos(t*0.23));
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	float T = TIME*speed*6.0;
	vec3 ro = camPath(T);
	vec3 ta = camPath(T + 0.8) + vec3(0.0, -0.05, 0.0);
	vec3 ww = normalize(ta - ro);
	vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
	vec3 vv = cross(uu, ww);
	vec3 rd = normalize(uv.x*uu + uv.y*vv + fov*ww);

	float t = 0.002, trap = 0.0, steps = 0.0;
	bool hit = false;
	for (int i = 0; i < 180; i++) {
		if (float(i) >= quality) break;
		float d = de(ro + rd*t, trap);
		if (d < 0.0004*t) { hit = true; break; }
		t += d;
		steps += 1.0;
		if (t > 6.0) break;
	}

	int lk = int(look);
	vec3 fogCol = lk == 2 ? vec3(0.0) : mix(vec3(0.05, 0.04, 0.06), pal(hue)*0.12, 0.5);
	vec3 col = fogCol;
	if (hit) {
		vec3 p = ro + rd*t;
		vec3 n = normalAt(p, 0.0003*t + 0.00005);
		vec3 Lp = ro + uu*0.05 + vv*0.08;
		vec3 L = normalize(Lp - p);
		float att = lantern*(1.0 + 0.5*aB) / (1.0 + 1.5*dot(Lp - p, Lp - p));
		// Cool fill from above so distant walls still read.
		float fill = 0.35*max(n.y*0.5 + 0.5, 0.0);
		float diff = max(dot(n, L), 0.0);
		float ao = clamp(1.0 - steps/max(quality, 1.0)*1.3, 0.0, 1.0);
		float spec = pow(max(dot(reflect(rd, n), L), 0.0), 30.0);
		float veins = 0.5 + 0.5*sin(log(trap + 1e-4)*6.0 + p.x*8.0);
		if (lk == 0) {
			vec3 stone = mix(vec3(0.92, 0.9, 0.86), pal(hue)*0.9, 0.35*veins);
			col = stone*((0.12 + fill)*ao + diff*att*1.6*ao) + spec*att*(0.6 + aH);
		} else if (lk == 1) {
			vec3 gold = mix(vec3(1.0, 0.75, 0.3), vec3(0.6, 0.3, 0.1), veins*0.6);
			col = gold*((0.08 + fill*0.7)*ao + diff*att*1.8*ao) + spec*att*vec3(1.0, 0.9, 0.7)*(1.2 + aH);
		} else {
			float edge = pow(1.0 - abs(dot(n, rd)), 4.0);
			col = pal(hue + log(trap + 1e-3)*0.15) * (edge*1.6 + 0.05) * (0.5 + att) * (1.0 + aH);
		}
		col = mix(col, fogCol, 1.0 - exp(-fog*t*t*0.8));
	}
	col *= 1.0 + 0.2*aL;
	col = 1.0 - exp(-col*1.6);
	gl_FragColor = vec4(pow(col, vec3(0.85)), 1.0);
}
