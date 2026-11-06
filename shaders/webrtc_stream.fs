/*{
  "CATEGORIES": [
    "Generator",
    "Live Input",
    "Audio Reactive"
  ],
  "DESCRIPTION": "WebRTC Stream: the live WebRTC (WHEP) feed as a shader. Easel feeds the 'liveStream' input automatically from the stream layer, so it loads on a SHOW deck like any other shader and gets knobs: fit, zoom, pan, mirror, a colour grade and four looks (Clean, Mono, Film, Glitch), with beat pulse and glitch driven by the music. With no stream connected it shows a quiet 'waiting' card.",
  "INPUTS": [
    { "NAME": "liveStream", "LABEL": "Stream", "TYPE": "image" },

    { "NAME": "look", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3],
      "LABELS": ["Clean","Mono","Film","Glitch"] },
    { "NAME": "fitMode", "LABEL": "Fit", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2],
      "LABELS": ["Fill (crop)","Fit (letterbox)","Stretch"], "GROUP": "Frame" },
    { "NAME": "zoom", "LABEL": "Zoom", "TYPE": "float", "MIN": 0.5, "MAX": 4, "DEFAULT": 1, "GROUP": "Frame" },
    { "NAME": "panX", "LABEL": "Pan X", "TYPE": "float", "MIN": -1, "MAX": 1, "DEFAULT": 0, "GROUP": "Frame" },
    { "NAME": "panY", "LABEL": "Pan Y", "TYPE": "float", "MIN": -1, "MAX": 1, "DEFAULT": 0, "GROUP": "Frame" },
    { "NAME": "rotation", "LABEL": "Rotation", "TYPE": "float", "MIN": -3.1416, "MAX": 3.1416, "DEFAULT": 0, "GROUP": "Frame" },
    { "NAME": "mirror", "LABEL": "Mirror", "TYPE": "bool", "DEFAULT": false, "GROUP": "Frame" },
    { "NAME": "letterboxColor", "LABEL": "Letterbox", "TYPE": "color", "DEFAULT": [0,0,0,1], "GROUP": "Frame" },

    { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Color" },
    { "NAME": "contrast", "LABEL": "Contrast", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Color" },
    { "NAME": "saturation", "LABEL": "Saturation", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Color" },
    { "NAME": "tint", "LABEL": "Tint", "TYPE": "color", "DEFAULT": [1,1,1,1], "GROUP": "Color" },
    { "NAME": "vignette", "LABEL": "Vignette", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0, "GROUP": "Color" },

    { "NAME": "beatPulse", "LABEL": "Beat Pulse", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0, "GROUP": "Audio Reactivity" },
    { "NAME": "glitchAmount", "LABEL": "Glitch Amount", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.5, "GROUP": "Audio Reactivity" }
  ]
}*/

// The stream texture is bound by Easel (any image input named liveStream is
// fed from the WebRTC/WHEP layer). IMG_SIZE_liveStream = 0 until frames arrive.

float h21(vec2 p) { p = fract(p * vec2(234.34, 435.345)); p += dot(p, p + 34.23); return fract(p.x * p.y); }

vec3 grade(vec3 c) {
    c *= brightness;
    c = (c - 0.5) * contrast + 0.5;
    float l = dot(c, vec3(0.2126, 0.7152, 0.0722));
    c = mix(vec3(l), c, saturation);
    return c * tint.rgb;
}

vec4 sampleStream(vec2 uv) {
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return vec4(letterboxColor.rgb, 0.0);
    return vec4(IMG_NORM_PIXEL(liveStream, uv).rgb, 1.0);
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 frag = gl_FragCoord.xy;
    vec2 uv = frag / res;
    vec2 imgSize = IMG_SIZE_liveStream;
    int lk = int(look + 0.5);

    // ── waiting card (no stream yet) ─────────────────────────────────
    if (imgSize.x < 2.0 || imgSize.y < 2.0) {
        vec2 q = (frag - 0.5 * res) / res.y;
        float pulse = 0.5 + 0.5 * sin(TIME * 2.0);
        float ring = smoothstep(0.012, 0.0, abs(length(q) - 0.12 - 0.01 * pulse));
        float dot1 = smoothstep(0.03, 0.025, length(q));
        vec3 c = vec3(0.06) + vec3(0.04) * (1.0 - length(q));
        c += vec3(0.75) * ring * (0.35 + 0.4 * pulse) + vec3(0.9, 0.25, 0.2) * dot1 * pulse;
        gl_FragColor = vec4(c, 1.0);
        return;
    }

    // ── frame mapping: fill / fit / stretch + zoom, pan, rotate ──────
    float sAsp = res.x / res.y, iAsp = imgSize.x / imgSize.y;
    float bass = clamp(audioBass, 0.0, 1.0);
    float z = zoom * (1.0 + 0.08 * beatPulse * bass);
    vec2 p = uv - 0.5;
    p.x *= sAsp;
    float cr = cos(rotation), sr = sin(rotation);
    p = mat2(cr, -sr, sr, cr) * p;
    p /= z;
    int fm = int(fitMode + 0.5);
    if (fm == 0) {            // fill: cover the screen, crop the overflow
        if (sAsp > iAsp) p /= vec2(sAsp, sAsp / iAsp); else p /= vec2(iAsp, 1.0);
    } else if (fm == 1) {     // fit: whole frame visible, letterbox the rest
        if (sAsp > iAsp) p /= vec2(iAsp, 1.0); else p /= vec2(sAsp, sAsp / iAsp);
    } else {                  // stretch
        p /= vec2(sAsp, 1.0);
    }
    vec2 suv = p + 0.5 + vec2(panX, panY) * 0.5;
    if (mirror) suv.x = 1.0 - suv.x;

    vec3 col;
    float inside = 1.0;
    if (lk == 3) {
        // Glitch: beat-gated horizontal slice shifts + RGB split
        float g = glitchAmount * clamp(audioBass * 1.2 + audioHigh * 0.6, 0.0, 1.0);
        float band = floor(uv.y * 24.0);
        float tq = floor(TIME * 12.0);
        float hit = step(1.0 - 0.45 * g, h21(vec2(band, tq)));
        vec2 off = vec2((h21(vec2(band + 3.1, tq)) - 0.5) * 0.12 * g * hit, 0.0);
        float split = 0.006 + 0.02 * g;
        vec4 r = sampleStream(suv + off + vec2(split, 0.0));
        vec4 gg = sampleStream(suv + off);
        vec4 b = sampleStream(suv + off - vec2(split, 0.0));
        col = vec3(r.r, gg.g, b.b);
        inside = gg.a;
        col = mix(col, col.gbr, hit * 0.35 * g);
        col *= 1.0 - 0.15 * step(0.5, fract(frag.y * 0.5)) * g;   // scanlines
    } else {
        vec4 s = sampleStream(suv);
        col = s.rgb; inside = s.a;
    }

    col = grade(col);

    if (lk == 1) {
        float l = dot(col, vec3(0.2126, 0.7152, 0.0722));
        col = vec3(smoothstep(0.02, 0.98, l));
    } else if (lk == 2) {
        // Film: warm toe, halation on highlights, grain, gate weave
        float l = dot(col, vec3(0.2126, 0.7152, 0.0722));
        col = mix(col, col * vec3(1.06, 1.0, 0.9), 0.7);
        col += vec3(0.9, 0.3, 0.15) * smoothstep(0.7, 1.0, l) * 0.18;
        col = col / (1.0 + col * 0.25) * 1.2;
        col += (h21(frag + fract(TIME * 24.0) * 91.0) - 0.5) * 0.07;
    }

    col = mix(letterboxColor.rgb, col, inside);
    vec2 vq = uv - 0.5;
    col *= 1.0 - vignette * dot(vq, vq) * 2.8;
    col *= 1.0 + 0.25 * beatPulse * smoothstep(0.4, 1.0, bass);

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
