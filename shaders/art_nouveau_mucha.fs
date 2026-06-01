/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Art Nouveau (Mucha / Klimt mood) — ABSTRACT. Flowing organic metaball blobs in jewel pigments (rose, viridian, violet, teal) merge and morph behind sinuous whiplash gold lines and curling filigree, with leaded stained-glass ink outlines, radial floral motifs, and a gilt glow. Warm domain-warped cream-to-jewel ground. Bass swells the blobs; treble shimmers the gold. Single-pass, HDR gold for bloom.",
  "INPUTS": [
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "flow",        "LABEL": "Flow Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6 },
    { "NAME": "blobCount",   "LABEL": "Blobs",        "TYPE": "long",  "DEFAULT": 6, "VALUES": [4,5,6,7,8], "LABELS": ["4","5","6","7","8"] },
    { "NAME": "goldAmt",     "LABEL": "Gold Lines",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "warmth",      "LABEL": "Palette Warmth","TYPE": "float","MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "inkOutline",  "LABEL": "Leaded Outline","TYPE": "float","MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7 }
  ]
}*/

// Art Nouveau — abstract organic blobs + whiplash gold + stained glass.

const vec3 GOLD   = vec3(2.40, 1.70, 0.55);   // HDR gilt
const vec3 ROSE   = vec3(0.80, 0.34, 0.42);
const vec3 VIRID  = vec3(0.06, 0.52, 0.40);
const vec3 VIOLET = vec3(0.42, 0.22, 0.62);
const vec3 TEAL   = vec3(0.10, 0.40, 0.50);
const vec3 CREAM  = vec3(0.93, 0.86, 0.72);
const vec3 INK    = vec3(0.04, 0.03, 0.05);

mat2 rot(float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float hash21(vec2 p){ p = fract(p * vec2(127.1, 311.7)); p += dot(p, p + 34.5); return fract(p.x * p.y); }

vec3 blobColor(int i){
    if (i == 0) return ROSE;
    if (i == 1) return VIRID;
    if (i == 2) return VIOLET;
    if (i == 3) return TEAL;
    if (i == 4) return ROSE * 0.8 + GOLD * 0.04;
    if (i == 5) return VIRID * 0.9 + VIOLET * 0.2;
    if (i == 6) return TEAL * 0.9 + ROSE * 0.15;
    return VIOLET * 0.8 + TEAL * 0.3;
}

void main(){
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t    = TIME * (0.25 + flow);
    float bass = clamp(audioBass * audioReact, 0.0, 1.5);
    float treb = clamp(audioHigh * audioReact, 0.0, 1.5);

    // ── Ground: warm cream → jewel, domain-warped for organic flow. ──
    vec2 w = uv + 0.18 * vec2(sin(uv.y * 2.3 + t * 0.5), cos(uv.x * 2.1 - t * 0.4));
    vec3 jewel = mix(TEAL, VIOLET, 0.5 + 0.5 * sin(w.x * 1.5 + t * 0.3));
    vec3 col = mix(CREAM * 0.9, jewel * 0.5, smoothstep(-0.6, 0.7, w.y));
    col = mix(col, col * vec3(1.08, 0.98, 0.86), warmth * 0.4);

    // ── Metaball field (abstract organic blobs). ──
    int N = int(blobCount + 0.5);
    float field = 0.0;
    vec3  acol = vec3(0.0);
    for (int i = 0; i < 8; i++){
        if (i >= N) break;
        float fi = float(i);
        // Lissajous drift + slow morph.
        vec2 c = 0.62 * vec2(sin(t * (0.35 + 0.05 * fi) + fi * 1.7),
                             cos(t * (0.30 + 0.06 * fi) + fi * 2.3));
        c.x *= 1.25;
        float r = (0.26 + 0.10 * sin(t * 0.5 + fi * 2.0)) * (1.0 + 0.18 * bass);
        float d = length((uv - c) * vec2(1.0, 1.0 + 0.25 * sin(fi)));
        float wgt = (r * r) / (d * d + 0.004);
        field += wgt;
        acol  += wgt * blobColor(i);
    }
    acol /= max(field, 1e-3);

    // Iso ~1: translucent jewel fill + leaded stained-glass ink edge.
    float inside = smoothstep(0.85, 1.15, field);
    vec3 glassFill = acol * (0.55 + 0.45 * inside);
    col = mix(col, glassFill, inside * 0.92);
    // Leaded outline at the iso boundary.
    float edge = smoothstep(0.14, 0.0, abs(field - 1.0));
    col = mix(col, INK, edge * inkOutline);
    // Inner highlight (glassy sheen on blob tops).
    col += acol * smoothstep(1.4, 2.4, field) * 0.35;

    // ── Whiplash gold lines (the Art Nouveau signature). ──
    float gold = 0.0;
    for (int i = 0; i < 4; i++){
        float fi = float(i);
        float ph = fi * 1.9 + t * (0.4 + 0.1 * fi);
        // Sinuous flowing curve y = f(x); distance to it gives the line.
        float curve = 0.36 * sin(uv.x * (1.6 + 0.4 * fi) + ph)
                    + 0.12 * sin(uv.x * 4.0 - ph * 1.3)
                    + 0.18 * (fi - 1.5);
        float dy = abs(uv.y - curve);
        float thick = 0.012 + 0.010 * (0.5 + 0.5 * sin(uv.x * 3.0 + ph));
        gold += smoothstep(thick, 0.0, dy);
        gold += 0.35 * smoothstep(thick * 4.0, 0.0, dy);   // soft halo
    }
    // Curling filigree spirals at a couple anchor points.
    for (int i = 0; i < 3; i++){
        float fi = float(i);
        vec2 a = vec2(mix(-1.0, 1.0, fract(fi * 0.37 + 0.2)), 0.5 * sin(fi * 2.1 + t * 0.2));
        vec2 q = uv - a;
        float ang = atan(q.y, q.x);
        float rad = length(q);
        float spiral = abs(rad - (0.05 + 0.04 * (ang + t * 0.3) / 6.2831));
        gold += smoothstep(0.012, 0.0, spiral) * smoothstep(0.22, 0.0, rad);
    }
    gold = clamp(gold, 0.0, 1.5);
    vec3 goldCol = GOLD * (1.0 + 0.5 * treb);
    col += goldCol * gold * goldAmt * 0.6;

    // ── Radial floral motif accents. ──
    for (int i = 0; i < 2; i++){
        float fi = float(i);
        vec2 fc = vec2(mix(-0.6, 0.6, fi), mix(0.5, -0.4, fi));
        vec2 q = (uv - fc) * rot(t * 0.1 + fi);
        float ang = atan(q.y, q.x);
        float petals = 0.10 + 0.045 * cos(ang * 6.0);   // 6-petal flower
        float fl = smoothstep(0.012, 0.0, abs(length(q) - petals)) * smoothstep(0.2, 0.0, length(q));
        col += GOLD * fl * 0.5;
        col = mix(col, ROSE * 1.2, smoothstep(petals, petals - 0.03, length(q)) * 0.4);
    }

    // ── Finish: gilt vignette + fine grain. ──
    float vig = 1.0 - smoothstep(0.55, 1.3, length(uv));
    col *= mix(0.7, 1.0, vig);
    col += GOLD * 0.05 * (1.0 - vig) * 0.5;   // warm gilt edge
    float g = hash21(gl_FragCoord.xy + floor(TIME * 24.0));
    col += (g - 0.5) * 0.03;

    gl_FragColor = vec4(max(col, 0.0), 1.0);
}
