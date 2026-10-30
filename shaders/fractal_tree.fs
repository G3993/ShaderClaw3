/*{
	"ISFVSN": "2",
	"CREDIT": "Etherea",
	"DESCRIPTION": "Fractal Tree — a single trunk that splits in two, and again, twelve generations deep (4,096 twigs) by folding space at each fork. Wind sways every generation on its own phase so the crown moves like a real tree; blossoms open at the tips. Looks: Blossom (dawn sky), Neon, Ink. Branch angle, length ratio, taper, wind, blossom size. Bass gusts the wind, highs make blossoms glow.",
	"CATEGORIES": [ "Generator", "Fractal", "Nature", "Audio Reactive" ],
	"INPUTS": [
		{ "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [ 0, 1, 2 ], "LABELS": [ "Blossom", "Neon", "Ink" ] },
		{ "NAME": "spread", "LABEL": "Branch Angle", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.1, "MAX": 1.3 },
		{ "NAME": "ratio", "LABEL": "Length Ratio", "TYPE": "float", "DEFAULT": 0.74, "MIN": 0.55, "MAX": 0.82 },
		{ "NAME": "depth", "LABEL": "Generations", "TYPE": "float", "DEFAULT": 11.0, "MIN": 4.0, "MAX": 14.0 },
		{ "NAME": "trunk", "LABEL": "Trunk Length", "TYPE": "float", "DEFAULT": 0.24, "MIN": 0.1, "MAX": 0.4 },
		{ "NAME": "thickness", "TYPE": "float", "DEFAULT": 0.018, "MIN": 0.002, "MAX": 0.05 },
		{ "NAME": "wind", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
		{ "NAME": "windRate", "LABEL": "Wind Rate", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 3.0 },
		{ "NAME": "lean", "TYPE": "float", "DEFAULT": 0.05, "MIN": -0.4, "MAX": 0.4 },
		{ "NAME": "asymmetry", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "jitter", "LABEL": "Randomness", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
		{ "NAME": "treeSeed", "LABEL": "Seed", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 100.0 },
		{ "NAME": "blossom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
		{ "NAME": "blossomColor", "LABEL": "Blossom Colour", "TYPE": "color", "DEFAULT": [ 1.0, 0.55, 0.7, 1.0 ] },
		{ "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "GROUP": "Audio Reactivity" }
	]
}*/

////////////////////////////////////////////////////////////
// Fractal Tree
// Each generation: distance to this branch segment, move to its tip,
// pick the child on this pixel's side of the fork, rotate it onto the y
// axis and scale. Each branch has its own angle/length jitter from a hash
// of its path, so 2^depth twigs from depth iterations, all different.
////////////////////////////////////////////////////////////

vec3 pal(float t) { return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }
float hash1(float n) { return fract(sin(n*12.9898 + treeSeed*7.31) * 43758.5453); }

float sdSeg(vec2 p, float len) {
	p.y -= clamp(p.y, 0.0, len);
	return length(p);
}

void main()
{
	float aB = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * audioReact;
	float aH = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * audioReact;
	float aL = smoothstep(0.05, 0.9, audioLevel) * audioReact;

	vec2 frag = gl_FragCoord.xy / RENDERSIZE.y;
	vec2 p = (gl_FragCoord.xy - vec2(0.5*RENDERSIZE.x, 0.04*RENDERSIZE.y)) / RENDERSIZE.y;
	float px = 1.0 / RENDERSIZE.y;

	float d = 1e9, s = 1.0, gen = 0.0, tipD = 1e9, path = 1.0;
	float glowAcc = 0.0;
	float L = trunk;
	float gust = wind*(1.0 + 0.8*aB);
	for (int i = 0; i < 14; i++) {
		if (float(i) >= depth) break;
		float fi = float(i);
		float w = thickness*pow(0.72, fi) + 0.6*px;
		float seg = sdSeg(p, L)*s - w;
		if (seg < d) { d = seg; gen = fi; }
		glowAcc += exp(-max(seg, 0.0)/(0.004 + 0.02*pow(0.8, fi))) * pow(0.85, fi);
		p.y -= L;
		// Sway this whole sub-crown, then split into two mirrored children.
		float hp = hash1(path);
		float sway = (i == 0 ? lean : 0.0) + gust*0.07*(fi*0.12 + 0.3)*sin(TIME*windRate*(0.8 + 0.4*hp) + fi*0.9 + hp*6.28);
		p = mat2(cos(sway), -sin(sway), sin(sway), cos(sway)) * p;
		// This fork's two child angles (left positive) and length ratios.
		float aL = spread*(1.0 + asymmetry*(hp - 0.5)*1.4) + jitter*0.8*(hash1(path + 0.37) - 0.5);
		float aR = spread*(1.0 - asymmetry*(hp - 0.5)*1.4) + jitter*0.8*(hash1(path + 0.71) - 0.5);
		float rL = ratio*(1.0 + jitter*0.3*(hash1(path + 1.3) - 0.5));
		float rR = ratio*(1.0 + jitter*0.3*(hash1(path + 2.9) - 0.5));
		// Side of the bisector between the two children.
		float bis = 0.5*(aL - aR);
		bool left = p.x*cos(bis) + p.y*sin(bis) < 0.0;
		float a = left ? -aL : aR;
		float r = left ? rL : rR;
		p = mat2(cos(a), sin(a), -sin(a), cos(a)) * p;
		p /= r;
		s *= r;
		path = path*2.0 + (left ? 0.0 : 1.0);
		path = mod(path, 65521.0);
	}
	// Blossoms on the last twigs.
	tipD = (length(p - vec2(0.0, L*0.5)) - L*0.75)*s;
	float bl = blossom*(1.0 + 0.6*aH);
	float flower = (1.0 - smoothstep(-px, px*2.0, tipD - 0.002*bl)) * step(0.01, blossom);
	float bark = 1.0 - smoothstep(-px, px, d);
	float k = gen / max(depth - 1.0, 1.0);

	vec3 col;
	int lk = int(look);
	if (lk == 0) {
		float y = frag.y;
		vec3 sky = mix(vec3(0.98, 0.84, 0.72), vec3(0.55, 0.62, 0.85), smoothstep(0.0, 1.0, y));
		sky += vec3(1.0, 0.85, 0.6) * exp(-6.0*length(frag - vec2(0.75*RENDERSIZE.x/RENDERSIZE.y, 0.35))) * 0.5;
		col = sky;
		col = mix(col, mix(vec3(0.16, 0.1, 0.09), vec3(0.32, 0.22, 0.2), k), bark);
		vec3 bc = blossomColor.rgb * (0.85 + 0.3*fract(sin(dot(floor(gl_FragCoord.xy/3.0), vec2(12.9, 78.2)))*437.5));
		col = mix(col, bc, flower*0.9);
		col += blossomColor.rgb * exp(-max(tipD, 0.0)*120.0) * 0.15 * bl;
	} else if (lk == 1) {
		col = vec3(0.0);
		col += pal(0.55 + k*0.5 + TIME*0.01) * glowAcc * 0.25 * (1.0 + 0.4*aL);
		col += pal(0.55 + k*0.5) * bark;
		col += blossomColor.rgb * (flower + exp(-max(tipD, 0.0)*80.0)*0.4) * bl;
		col = 1.0 - exp(-col*1.3);
	} else {
		vec3 paper = vec3(0.95, 0.93, 0.88);
		col = mix(paper, vec3(0.06, 0.06, 0.09), bark*(0.75 + 0.25*(1.0 - k)));
		col = mix(col, vec3(0.8, 0.2, 0.15), flower*0.85);
	}
	gl_FragColor = vec4(col, 1.0);
}
