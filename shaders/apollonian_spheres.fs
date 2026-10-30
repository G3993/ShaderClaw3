/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea, after Inigo Quilez's Apollonian sphere-inversion fractal",
	"DESCRIPTION": "Apollonian Spheres — a raymarched 3D Apollonian packing: repeat space, invert in a sphere, repeat again, until every gap is filled with ever smaller bubbles and tunnels. The camera wanders through it as the inversion radius breathes. Looks: Pearl, Stained Glass, Ember. Bass breathes the inversion, highs sparkle.",
	"CATEGORIES": [ "Generator", "Fractal", "3D", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Pearl", "Stained Glass", "Ember" ] },
		{ "NAME": "inversion", "TYPE": "float", "DEFAULT": 1.25, "MIN": 1.0, "MAX": 1.6 },
		{ "NAME": "breathe", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "speed", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "foldIters", "LABEL": "Fold Iterations", "TYPE": "float", "DEFAULT": 8.0, "MIN": 4.0, "MAX": 10.0 },
		{ "NAME": "quality", "TYPE": "float", "DEFAULT": 110.0, "MIN": 40.0, "MAX": 200.0 },
		{ "NAME": "fog", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
		{ "NAME": "hue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Apollonian Spheres
// p = -1 + 2 fract(p/2 + 1/2) ; k = s/dot(p,p) ; p *= k, repeated.
// DE = 0.25 |p.y| / scale. Orbit trap (|p|, r2) drives colour.
////////////////////////////////////////////////////////////

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }
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

float gInv;

float de(vec3 p, out vec4 orb) {
	float scale = 1.0;
	orb = vec4(1000.0);
	for (int i = 0; i < 10; i++) {
		if (float(i) >= foldIters) break;
		p = -1.0 + 2.0*fract(0.5*p + 0.5);
		float r2 = dot(p, p);
		orb = min(orb, vec4(abs(p), r2));
		float k = gInv / r2;
		p *= k;
		scale *= k;
	}
	return 0.25*abs(p.y)/scale;
}

float deOnly(vec3 p) { vec4 o; return de(p, o); }

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
	float T = TIME*speed;
	gInv = inversion + breathe*0.15*sin(TIME*0.07) + 0.03*aB;

	vec3 ro = vec3(2.8*cos(0.1 + 0.33*T), 0.4 + 0.3*cos(0.37*T), 2.8*cos(0.5 + 0.35*T));
	vec3 ta = vec3(1.9*cos(1.2 + 0.41*T), 0.4 + 0.1*cos(0.27*T), 1.9*cos(2.0 + 0.38*T));
	float roll = 0.2*cos(0.1*T);
	vec3 ww = normalize(ta - ro);
	vec3 uu = normalize(cross(ww, vec3(sin(roll), cos(roll), 0.0)));
	vec3 vv = cross(uu, ww);
	vec3 rd = normalize(uv.x*uu + uv.y*vv + 2.0*ww);

	float t = 0.01, steps = 0.0;
	vec4 orb = vec4(0.0);
	bool hit = false;
	for (int i = 0; i < 200; i++) {
		if (float(i) >= quality) break;
		float d = de(ro + rd*t, orb);
		if (d < 0.0003*t) { hit = true; break; }
		t += d;
		steps += 1.0;
		if (t > 12.0) break;
	}

	int lk = int(look);
	vec3 bg = lk == 2 ? vec3(0.02, 0.0, 0.0) : vec3(0.06, 0.06, 0.09);
	vec3 col = bg;
	if (hit) {
		vec3 p = ro + rd*t;
		vec3 n = normalAt(p, 0.0002*t + 0.00002);
		vec3 L = normalize(vec3(0.6, 0.7, -0.4));
		float diff = max(dot(n, L), 0.0);
		float amb = 0.5 + 0.5*n.y;
		float ao = clamp(1.0 - steps/max(quality, 1.0)*1.2, 0.0, 1.0);
		ao *= clamp(orb.w*2.0, 0.0, 1.0);
		float spec = pow(max(dot(reflect(rd, n), L), 0.0), 32.0);
		if (lk == 0) {
			vec3 base = mix(vec3(1.0), vec3(1.0, 0.82, 0.62), clamp(6.0*orb.y, 0.0, 1.0));
			base = mix(base, pal(hue + 0.6)*0.8 + 0.2, pow(clamp(1.0 - 2.0*orb.z, 0.0, 1.0), 8.0));
			col = base*(0.35*amb + 0.75*diff)*ao + spec*0.5*ao*(1.0 + aH);
		} else if (lk == 1) {
			float id = floor(orb.x*9.0) + floor(orb.z*7.0)*3.0;
			vec3 g = jewel(hash1(id + hue*5.0));
			col = g*(0.3*amb + 0.9*diff)*ao + spec*0.6*ao;
			col = mix(col, vec3(0.01), smoothstep(0.85, 1.0, fract(orb.x*9.0)) + smoothstep(0.85, 1.0, fract(orb.z*7.0)));
		} else {
			vec3 ember = mix(vec3(1.0, 0.35, 0.05), vec3(1.0, 0.85, 0.4), clamp(orb.y*4.0, 0.0, 1.0));
			col = ember*pow(ao, 2.0)*(0.3 + diff)*1.4 + spec*vec3(1.0, 0.6, 0.3)*(1.0 + aH);
		}
		col = mix(col, bg, 1.0 - exp(-fog*0.05*t*t));
	}
	col *= 1.35*(1.0 + 0.2*aL);
	gl_FragColor = vec4(pow(clamp(col, 0.0, 1.0), vec3(0.4545)), 1.0);
}
