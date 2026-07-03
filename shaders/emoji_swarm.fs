/*{
  "DESCRIPTION": "Emoji Swarm — a voice-driven flock of REAL emoji glyphs (sampled from a 4×4 atlas of 16 Twemoji, CC-BY 4.0). The cue text bound to `msg` makes the swarm come alive: while you're speaking the swarm fills out and brightens; when you go silent it fades. The MESSAGE itself seeds which emojis appear (hashes the chars of what you said). Bind COUNT / SIZE / SPEED / GLOW to audio bands for live reactivity. Provide the included `emoji_swarm_atlas.png` as the `atlas` image input (add it as an Image layer and bind it).",
  "CREDIT": "Easel · emoji_swarm  (emoji art © Twitter/jdecked Twemoji, CC-BY 4.0)",
  "CATEGORIES": ["Text", "Generator", "Voice", "Fun"],
  "INPUTS": [
    { "NAME": "msg",     "LABEL": "Message", "TYPE": "text",  "DEFAULT": "HELLO", "MAX_LENGTH": 32, "BIND": "cue.latest" },
    { "NAME": "atlas",   "LABEL": "Atlas",   "TYPE": "image" },
    { "NAME": "count",   "LABEL": "Count",      "TYPE": "float", "MIN": 4.0,  "MAX": 64.0, "DEFAULT": 32.0 },
    { "NAME": "size",    "LABEL": "Size",       "TYPE": "float", "MIN": 0.02, "MAX": 0.20, "DEFAULT": 0.08 },
    { "NAME": "speed",   "LABEL": "Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 0.6 },
    { "NAME": "spin",    "LABEL": "Spin",       "TYPE": "float", "MIN": -4.0, "MAX": 4.0,  "DEFAULT": 0.3 },
    { "NAME": "spread",  "LABEL": "Spread",     "TYPE": "float", "MIN": 0.5,  "MAX": 1.5,  "DEFAULT": 1.0 },
    { "NAME": "wiggle",  "LABEL": "Wiggle",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.5,  "DEFAULT": 0.14 },
    { "NAME": "glow",    "LABEL": "Glow",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.40 },
    { "NAME": "silentFade", "LABEL": "Silence Fade", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75 },
    { "NAME": "bg",      "LABEL": "Background", "TYPE": "long",
      "VALUES": [0, 1, 2],
      "LABELS": ["Transparent", "Black", "Gradient"],
      "DEFAULT": 0 }
  ]
}*/

// ── hashes ──────────────────────────────────────────────────────────
float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
vec2  h21(float n) { return fract(sin(vec2(n, n + 1.0) * vec2(12.9898, 78.233)) * 43758.5453); }
mat2  rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// Voice-side: msg comes in as 32 small ints (A=0..Z=25, space=26, 0-9=27-36)
// + msg_len. We aggregate them into a single hashed seed (msgSeed) that
// shifts the swarm's emoji distribution AND an activity factor (vox) that
// fades the swarm in/out with speech.
float msgChar(int i) {
    if (i ==  0) return msg_0;  if (i ==  1) return msg_1;  if (i ==  2) return msg_2;
    if (i ==  3) return msg_3;  if (i ==  4) return msg_4;  if (i ==  5) return msg_5;
    if (i ==  6) return msg_6;  if (i ==  7) return msg_7;  if (i ==  8) return msg_8;
    if (i ==  9) return msg_9;  if (i == 10) return msg_10; if (i == 11) return msg_11;
    if (i == 12) return msg_12; if (i == 13) return msg_13; if (i == 14) return msg_14;
    if (i == 15) return msg_15; if (i == 16) return msg_16; if (i == 17) return msg_17;
    if (i == 18) return msg_18; if (i == 19) return msg_19; if (i == 20) return msg_20;
    if (i == 21) return msg_21; if (i == 22) return msg_22; if (i == 23) return msg_23;
    if (i == 24) return msg_24; if (i == 25) return msg_25; if (i == 26) return msg_26;
    if (i == 27) return msg_27; if (i == 28) return msg_28; if (i == 29) return msg_29;
    if (i == 30) return msg_30; return msg_31;
}

float computeMsgSeed() {
    // FNV-ish accumulation over the live characters — small, fast, fine
    // for picking emoji indices. Returns a 0..1 float seed.
    float acc = 216613.0;
    float n = min(msg_len, 32.0);
    for (int i = 0; i < 32; i++) {
        if (float(i) >= n) break;
        acc = mod(acc * 167.0 + msgChar(i) * 131.0, 65521.0);
    }
    return fract(acc / 65521.0);
}

// 0 when silent, 1 when actively speaking. Saturates quickly so any speech
// instantly lights up the swarm, then `silentFade` controls how much the
// idle swarm collapses back when msg_len drops to 0.
float voiceLevel() {
    return mix(1.0 - silentFade, 1.0, smoothstep(0.0, 6.0, msg_len));
}

// Sample one tile (id 0..15) from the 4×4 atlas. Atlas convention:
// row 0 is the TOP row in source PNG; ISF samplers use bottom-left
// origin, so flip v inside the tile to keep glyphs upright.
// Set once in main(): 1.0 when a real atlas image is bound, else 0.0.
float atlasLive;

vec4 sampleEmoji(int id, vec2 local) {
    // local in [-1,1] centered. Map to tile uv [0,1].
    vec2 t = local * 0.5 + 0.5;        // [0,1] inside tile
    if (any(lessThan(t, vec2(0.0))) || any(greaterThan(t, vec2(1.0)))) {
        return vec4(0.0);
    }
    if (atlasLive < 0.5) {
        // Procedural fallback glyph (no atlas bound): a glossy smiley ball
        // tinted per id so the swarm still reads as emoji.
        float r = length(local);
        float a = smoothstep(0.95, 0.80, r);
        if (a <= 0.001) return vec4(0.0);
        vec3 tint = 0.55 + 0.45 * cos(6.2831 * (float(id) / 16.0) + vec3(0.0, 2.1, 4.2));
        float hl = smoothstep(0.9, 0.0, length(local - vec2(-0.35, 0.40)));
        vec3 c = tint * (0.72 + 0.5 * hl);
        float eyes = smoothstep(0.15, 0.09, length(local - vec2(-0.30, 0.18)))
                   + smoothstep(0.15, 0.09, length(local - vec2( 0.30, 0.18)));
        float mouth = smoothstep(0.10, 0.04, abs(length(local - vec2(0.0, 0.10)) - 0.48))
                    * smoothstep(-0.10, -0.28, local.y);
        c = mix(c, vec3(0.08, 0.06, 0.05), clamp(eyes + mouth, 0.0, 1.0));
        return vec4(c, a);
    }
    // WebGL1: no bitwise ops — derive col/row with float math.
    float fid  = float(id);
    float colF = mod(fid, 4.0);        // id % 4
    float rowF = floor(fid * 0.25);    // id / 4
    // Flip row to compensate for ISF (origin bottom-left) vs atlas (top-left).
    rowF = 3.0 - rowF;
    vec2 tileSize = vec2(0.25);
    vec2 origin   = vec2(colF, rowF) * tileSize;
    // Flip v inside the tile so the glyph reads upright.
    vec2 uv = origin + vec2(t.x, 1.0 - t.y) * tileSize;
    return IMG_NORM_PIXEL(atlas, uv);
}

// ── main ────────────────────────────────────────────────────────────
void main() {
    // Detect whether a real atlas is bound (size set AND texels non-empty);
    // otherwise sampleEmoji falls back to procedural glyphs.
    vec4 probe = IMG_NORM_PIXEL(atlas, vec2(0.125, 0.875))
               + IMG_NORM_PIXEL(atlas, vec2(0.5, 0.5));
    atlasLive = (IMG_SIZE_atlas.x > 1.0 && (probe.a > 0.01 || dot(probe.rgb, vec3(1.0)) > 0.01)) ? 1.0 : 0.0;

    vec2  uv     = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / max(1.0, RENDERSIZE.y);
    vec2  p      = (uv - 0.5) * vec2(aspect, 1.0);

    vec3  col = vec3(0.0);
    float bgA = 1.0;
    float bgMode = float(bg);   // 'long' inputs arrive as float uniforms
    if (bgMode < 0.5)      { bgA = 0.0; }
    else if (bgMode < 1.5) { col = vec3(0.02, 0.03, 0.06); }
    else                   { col = mix(vec3(0.06, 0.04, 0.18),
                                       vec3(0.18, 0.06, 0.22), uv.y); }

    float t    = TIME;
    float vox  = voiceLevel();                          // 0..1 speech activity
    float seed = computeMsgSeed();                       // 0..1 from msg chars
    int   live = int(clamp(count * (0.30 + 0.70 * vox), 4.0, 64.0));

    // Hard upper bound 64 lets the compiler unroll; `break` gates by live count.
    for (int i = 0; i < 64; i++) {
        if (i >= live) break;
        float fi = float(i);
        vec2  s  = h21(fi * 7.13 + 0.91);

        // Emoji type — message seed shifts which emojis appear preferentially,
        // so saying different things produces visibly different swarms.
        float typeF = fract(s.x * 11.0 + seed * 7.0 + s.y);
        int   type  = int(floor(typeF * 16.0));   // 0..15 → 4×4 atlas

        // Layout
        vec2 base = vec2((s.x - 0.5) * aspect * 2.0,
                         (s.y - 0.5) * spread * 1.6);

        float spd = (0.40 + 0.60 * s.y) * speed * (0.5 + 0.5 * vox);
        float ang = t * spd * 0.30 + fi * 1.70;
        vec2  off = vec2(cos(ang), sin(ang * 1.13)) * wiggle;
        off.y += sin(t * (1.0 + s.x * 2.0) + fi) * 0.035;

        // Soft wrap so glyphs re-enter from the opposite edge.
        vec2 center = base + off;
        center.x = mod(center.x + aspect, aspect * 2.0) - aspect;
        center.y = mod(center.y + 0.90, 1.80) - 0.90;

        vec2  local = p - center;
        float sz    = size * (0.70 + 0.60 * s.y);
        local /= sz;
        if (abs(local.x) > 1.10 || abs(local.y) > 1.10) continue;

        // Spin
        float a = t * spin * (0.40 + s.x * 0.80) + fi * 2.0;
        local   = rot(a) * local;

        vec4 e = sampleEmoji(type, local);
        if (e.a < 0.001) continue;

        // Voice-driven brightness lift + halo so active speech = vibrant swarm.
        vec3 halo = e.rgb * glow * vox * 0.6;
        col = mix(col, e.rgb + halo, e.a);
    }

    gl_FragColor = vec4(col, bgA);
}
