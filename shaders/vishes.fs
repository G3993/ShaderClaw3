/*{
  "DESCRIPTION": "Bioluminescent Reef 3D — ocean-floor raymarcher with sdCapsule coral branches, brain coral, tube worms, emission-only bioluminescent glow. No external lighting.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw — full rewrite to 3D bioluminescent reef raymarcher",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Sway Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 0.6 },
    { "NAME": "reefDensity","LABEL": "Reef Density",  "TYPE": "float", "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",      "TYPE": "float", "MIN": 1.0,  "MAX": 6.0,  "DEFAULT": 2.5 },
    { "NAME": "audioReact", "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5 }
  ]
}*/

// ── Hash helpers ──────────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// ── SDF primitives ────────────────────────────────────────────────────────────
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

// ── Coral geometry helpers ────────────────────────────────────────────────────
void getCoralBranch(int i, float tm,
                    out vec3 base, out vec3 tip, out float radius) {
    float fi   = float(i);
    float sway = sin(tm * speed * 0.5 + fi * 1.2) * 0.12;
    base   = vec3(sin(fi * 2.4) * 1.8 * reefDensity,
                  0.0,
                  cos(fi * 1.9) * 1.5 * reefDensity);
    tip    = base + vec3(sway,
                         1.2 + 0.4 * sin(fi * 1.7),
                         sway * 0.5);
    radius = 0.055 + hash11(fi * 3.71) * 0.04;
}

void getTubeWorm(int i, float tm, out vec3 base, out vec3 tip) {
    float fi   = float(i) + 10.0;
    float sway = sin(tm * speed * 0.7 + fi * 2.3) * 0.06;
    base = vec3(sin(fi * 3.1) * 1.3 * reefDensity,
                0.0,
                cos(fi * 2.7) * 1.2 * reefDensity);
    tip  = base + vec3(sway, 1.6 + hash11(fi) * 0.6, sway * 0.3);
}

vec3 brainCoralCenter(int i) {
    float fi = float(i) + 20.0;
    return vec3(sin(fi * 1.3) * 2.2 * reefDensity,
                0.28,
                cos(fi * 1.7) * 1.8 * reefDensity);
}

// ── Scene SDF ────────────────────────────────────────────────────────────────
// matId: 1=floor, 2=coral_cyan, 3=coral_magenta, 4=worm_body, 5=worm_tip, 6=brain_coral
vec2 sceneMap(vec3 p, float tm) {
    vec2 best = vec2(p.y, 1.0);   // sea floor at y=0

    // 8 coral branches
    for (int i = 0; i < 8; i++) {
        vec3 base, tip; float r;
        getCoralBranch(i, tm, base, tip, r);
        float d   = sdCapsule(p, base, tip, r);
        float mat = (mod(float(i), 2.0) < 1.0) ? 2.0 : 3.0;
        if (d < best.x) best = vec2(d, mat);
    }

    // 4 tube worms
    for (int i = 0; i < 4; i++) {
        vec3 base, tip;
        getTubeWorm(i, tm, base, tip);
        float db = sdCapsule(p, base, mix(base, tip, 0.85), 0.03);
        if (db < best.x) best = vec2(db, 4.0);
        float dt = sdSphere(p - tip, 0.07);
        if (dt < best.x) best = vec2(dt, 5.0);
    }

    // 3 brain corals
    for (int i = 0; i < 3; i++) {
        vec3 ctr = brainCoralCenter(i);
        float bs = sdSphere(p - ctr, 0.25 + hash11(float(i) * 7.3) * 0.1);
        if (bs < best.x) best = vec2(bs, 6.0);
    }

    return best;
}

// ── Normal via central differences ───────────────────────────────────────────
vec3 calcNormal(vec3 p, float tm) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneMap(p + e.xyy, tm).x - sceneMap(p - e.xyy, tm).x,
        sceneMap(p + e.yxy, tm).x - sceneMap(p - e.yxy, tm).x,
        sceneMap(p + e.yyx, tm).x - sceneMap(p - e.yyx, tm).x
    ));
}

// ── Emission colour by material ───────────────────────────────────────────────
vec3 emitColor(float matId, vec3 hp, float pk, float audio) {
    if (matId < 1.5)
        return vec3(0.0, 0.008, 0.018) + vec3(0.0, 0.05, 0.2) * 0.12;
    if (matId < 2.5)
        return vec3(0.0, 3.0, 1.5) * pk * audio;          // coral cyan
    if (matId < 3.5)
        return vec3(2.0, 0.0, 1.2) * pk * audio;          // coral magenta
    if (matId < 4.5)
        return vec3(0.0, 0.05, 0.4) * pk;                 // worm body dim blue
    if (matId < 5.5)
        return vec3(0.0, 3.0, 1.5) * 1.5 * pk * audio;   // worm tip hyper-cyan
    // brain coral: animated stripe mix
    float stripe = 0.5 + 0.5 * sin(hp.x * 12.0 + hp.z * 10.0);
    return mix(vec3(2.0, 0.0, 1.2), vec3(0.0, 3.0, 1.5), stripe) * pk * audio * 0.7;
}

// ── Volumetric water glow along ray ──────────────────────────────────────────
vec3 volumeGlow(vec3 ro, vec3 rd, float tHit, float tm, float pk, float audio) {
    vec3  glow = vec3(0.0);
    float tEnd = min(tHit, 12.0);
    float dt   = tEnd / 20.0;

    for (int s = 0; s < 20; s++) {
        vec3 p = ro + rd * ((float(s) + 0.5) * dt);

        for (int i = 0; i < 8; i++) {
            vec3 base, tip; float r;
            getCoralBranch(i, tm, base, tip, r);
            vec3  ba   = tip - base;
            vec3  pa   = p - base;
            float h    = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
            float dist = length(pa - ba * h);
            float g    = exp(-dist * 5.0) * dt * 0.25;
            vec3  c    = (mod(float(i), 2.0) < 1.0)
                         ? vec3(0.0, 3.0, 1.5) : vec3(2.0, 0.0, 1.2);
            glow += c * g * pk * audio;
        }

        for (int i = 0; i < 4; i++) {
            vec3 base, tip;
            getTubeWorm(i, tm, base, tip);
            float dist = length(p - tip);
            float g    = exp(-dist * 6.0) * dt * 0.35;
            glow += vec3(0.0, 3.0, 1.5) * g * pk * audio;
        }
    }
    return glow;
}

// ── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv  = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x    *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float audio = 1.0 + (audioLevel * 0.3 + audioBass * 0.7) * audioReact;
    float tm    = TIME;

    // Camera: ocean-floor wide shot, looking down-forward at reef
    float camSway = sin(tm * 0.23) * 0.12;
    vec3  ro      = vec3(camSway, 3.0, -4.0);
    vec3  target  = vec3(0.0, 0.5, 0.0);
    vec3  forward = normalize(target - ro);
    vec3  right   = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3  up      = cross(right, forward);
    vec3  rd      = normalize(forward + uv.x * right * 0.65 + uv.y * up * 0.65);

    // ── Raymarching — 64 steps ──
    float t    = 0.02;
    float tMax = 18.0;
    float matId = 0.0;
    bool  hit  = false;

    for (int i = 0; i < 64; i++) {
        vec3  p   = ro + rd * t;
        vec2  res = sceneMap(p, tm);
        if (res.x < 0.001) {
            matId = res.y;
            hit   = true;
            break;
        }
        t += max(res.x * 0.85, 0.002);
        if (t > tMax) break;
    }

    // Void ocean background with faint horizon blue
    float horizGlow = exp(-max(abs(rd.y) - 0.05, 0.0) * 4.0);
    vec3  col = vec3(0.0, 0.01, 0.03)
              + vec3(0.0, 0.1, 1.5) * horizGlow * 0.08;

    // Volumetric scatter glow
    col += volumeGlow(ro, rd, hit ? t : tMax, tm, hdrPeak, audio);

    if (hit) {
        vec3 hp = ro + rd * t;

        vec3 emit = emitColor(matId, hp, hdrPeak, audio);

        // fwidth AA on SDF boundary
        vec2  res2    = sceneMap(hp, tm);
        float fw      = fwidth(res2.x);
        float edgeMask = smoothstep(fw * 2.0, 0.0, abs(res2.x));
        emit *= (0.6 + 0.4 * edgeMask);

        // Depth fog
        float fog = exp(-t * 0.18);
        col += emit * fog;
    }

    // Cinematic vignette
    float vig = 1.0 - 0.4 * dot(uv * 0.7, uv * 0.7);
    col *= vig;

    gl_FragColor = vec4(col, 1.0);
}
