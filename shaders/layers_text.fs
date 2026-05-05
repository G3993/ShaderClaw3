/*{
	"CATEGORIES": ["Generator", "Text"],
	"DESCRIPTION": "Text repeated at multiple depth layers with parallax — HDR front emission, neon depth palette, animated background",
	"INPUTS": [
		{ "NAME": "msg", "TYPE": "text", "DEFAULT": "ETHEREA", "MAX_LENGTH": 12 },
		{ "NAME": "speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
		{ "NAME": "layerCount", "TYPE": "float", "MIN": 2.0, "MAX": 6.0, "DEFAULT": 4.0 },
		{ "NAME": "depthSpread", "TYPE": "float", "MIN": 0.1, "MAX": 1.0, "DEFAULT": 0.5 },
		{ "NAME": "frontColor", "TYPE": "color", "DEFAULT": [1.0, 0.5, 0.0, 1.0] },
		{ "NAME": "backColor", "TYPE": "color", "DEFAULT": [0.05, 0.05, 0.4, 1.0] },
		{ "NAME": "bgColor", "TYPE": "color", "DEFAULT": [0.0, 0.01, 0.05, 1.0] },
		{ "NAME": "textScale", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 0.8 },
		{ "NAME": "hdrPeak", "TYPE": "float", "MIN": 1.0, "MAX": 5.0, "DEFAULT": 2.5 },
		{ "NAME": "audioReact", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 },
		{ "NAME": "transparentBg", "TYPE": "bool", "DEFAULT": false }
	]
}*/

// ---- Font engine ----

vec2 charData(int ch) {
	if (ch == 0)  return vec2(1033777.0, 14897.0);
	if (ch == 1)  return vec2(1001022.0, 31281.0);
	if (ch == 2)  return vec2(541230.0, 14896.0);
	if (ch == 3)  return vec2(575068.0, 29265.0);
	if (ch == 4)  return vec2(999967.0, 32272.0);
	if (ch == 5)  return vec2(999952.0, 32272.0);
	if (ch == 6)  return vec2(771630.0, 14896.0);
	if (ch == 7)  return vec2(1033777.0, 17969.0);
	if (ch == 8)  return vec2(135310.0, 14468.0);
	if (ch == 9)  return vec2(68172.0, 7234.0);
	if (ch == 10) return vec2(807505.0, 18004.0);
	if (ch == 11) return vec2(541215.0, 16912.0);
	if (ch == 12) return vec2(706097.0, 18293.0);
	if (ch == 13) return vec2(640561.0, 18229.0);
	if (ch == 14) return vec2(575022.0, 14897.0);
	if (ch == 15) return vec2(999952.0, 31281.0);
	if (ch == 16) return vec2(579149.0, 14897.0);
	if (ch == 17) return vec2(1004113.0, 31281.0);
	if (ch == 18) return vec2(460334.0, 14896.0);
	if (ch == 19) return vec2(135300.0, 31876.0);
	if (ch == 20) return vec2(575022.0, 17969.0);
	if (ch == 21) return vec2(567620.0, 17969.0);
	if (ch == 22) return vec2(710513.0, 17969.0);
	if (ch == 23) return vec2(141873.0, 17962.0);
	if (ch == 24) return vec2(135300.0, 17962.0);
	if (ch == 25) return vec2(139807.0, 31778.0);
	return vec2(0.0, 0.0);
}

float charPixel(int ch, float col, float row) {
	vec2 data = charData(ch);
	float rowIdx = floor(row);
	float rowVal;
	if (rowIdx < 4.0) { rowVal = mod(floor(data.x / pow(32.0, rowIdx)), 32.0); }
	else { rowVal = mod(floor(data.y / pow(32.0, rowIdx - 4.0)), 32.0); }
	return mod(floor(rowVal / pow(2.0, 4.0 - floor(col))), 2.0);
}

int getChar(int slot) {
	if (slot == 0) return int(msg_0); if (slot == 1) return int(msg_1);
	if (slot == 2) return int(msg_2); if (slot == 3) return int(msg_3);
	if (slot == 4) return int(msg_4); if (slot == 5) return int(msg_5);
	if (slot == 6) return int(msg_6); if (slot == 7) return int(msg_7);
	if (slot == 8) return int(msg_8); if (slot == 9) return int(msg_9);
	if (slot == 10) return int(msg_10); return int(msg_11);
}

int charCount() { int n = int(msg_len); return n > 0 ? n : 1; }

void main() {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	float aspect = RENDERSIZE.x / RENDERSIZE.y;

	int numChars = charCount();

	// Audio-reactive HDR peak boost
	float audio = 1.0 + (audioLevel + audioBass * 0.8) * audioReact;

	// Animated neon background — slow diagonal gradient
	vec3 bg = bgColor.rgb;
	if (!transparentBg) {
		float gx = sin(uv.x * 2.0 + TIME * 0.2) * 0.5 + 0.5;
		float gy = cos(uv.y * 1.5 + TIME * 0.15) * 0.5 + 0.5;
		bg += vec3(gx * 0.06, gy * 0.02, (1.0 - gx * gy) * 0.12);
	}

	vec3 col = bg;
	float alpha = transparentBg ? 0.0 : 1.0;

	// Render layers back to front
	for (int L = 0; L < 6; L++) {
		if (float(L) >= layerCount) break;

		float depth = float(L) / (layerCount - 1.0);

		float layerScale = textScale * (0.5 + depth * 0.8);

		float drift = sin(TIME * speed * (0.3 + depth * 0.7) + depth * 2.0) * 0.1 * depthSpread;
		float vDrift = cos(TIME * speed * (0.2 + depth * 0.5) + depth * 3.5) * 0.03 * depthSpread;

		// HDR color: back layers are dim saturated blue, front layers are bright neon
		vec3 layerCol = mix(backColor.rgb, frontColor.rgb, depth);
		// Front layers get HDR multiplier; back layers stay dim
		float layerHDR = mix(0.25, hdrPeak * audio, depth * depth);
		layerCol *= layerHDR;

		// Back layers more transparent, front fully opaque
		float layerAlpha = 0.3 + depth * 0.7;

		float charW = 0.09 * layerScale;
		float charH = charW * 1.5;
		float gap = charW * 0.25;

		float totalW = float(numChars) * charW + float(numChars - 1) * gap;

		vec2 p = vec2((uv.x - 0.5) * aspect + 0.5, uv.y);

		p.x -= drift;
		p.y -= vDrift;

		float startX = 0.5 - totalW * 0.5;
		float startY = 0.5 - charH * 0.5;

		for (int i = 0; i < 12; i++) {
			if (i >= numChars) break;

			int ch = getChar(i);
			if (ch == 26) continue;

			float cx = startX + float(i) * (charW + gap);
			float cy = startY;

			float localX = (p.x - cx) / charW;
			float localY = (p.y - cy) / charH;

			if (localX >= 0.0 && localX < 1.0 && localY >= 0.0 && localY < 1.0) {
				float gridCol = localX * 5.0;
				float gridRow = localY * 7.0;

				float filled = charPixel(ch, gridCol, gridRow);

				col = mix(col, layerCol, filled * layerAlpha);
				alpha = mix(alpha, 1.0, filled * layerAlpha);
			}
		}
	}

	gl_FragColor = vec4(col, alpha);
}
