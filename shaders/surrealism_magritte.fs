/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Surrealism after Magritte — the dislocation of expected images. Five moods: Golconda's raining bowler-hatted men, Empire of Light's impossible day-sky-over-night-street, Son of Man's apple-obscured bowler figure, Treachery's procedural pipe with hand-stroked 'Ceci n'est pas une pipe', and The Lovers' two veil-covered head silhouettes. Patient and still — Magritte is uncanny, not animated. Bass nudges rain tempo, mid shifts the day/night line, treble dusts pearl sparks. Output linear HDR.",
  "INPUTS": [
    { "NAME": "magritteMood", "LABEL": "Mood", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3,4], "LABELS": ["Golconda","Empire of Light","Son of Man","Treachery","The Lovers"] },
    { "NAME": "rainDensity",  "LABEL": "Rain Density",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "rainTempo",    "LABEL": "Rain Tempo",     "TYPE": "float", "MIN": 0.05,"MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "daynightLine", "LABEL": "Day/Night Line", "TYPE": "float", "MIN": 0.20,"MAX": 0.80, "DEFAULT": 0.50 },
    { "NAME": "appleHover",   "LABEL": "Apple Hover",    "TYPE": "float", "MIN": 0.0, "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "cloudCoverage","LABEL": "Cloud Coverage", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "uncannyAmount","LABEL": "Uncanny",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.65 },
    { "NAME": "audioReact",   "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/
// Magritte — the paradox image. Calm, restrained, slightly off.
#define SKY_BLUE  vec3(0.45, 0.55, 0.65)
#define CREAM     vec3(0.92, 0.88, 0.78)
#define APPLE_GRN vec3(0.55, 0.80, 0.30)
#define HAT_BLACK vec3(0.04, 0.04, 0.06)
#define NIGHT_IND vec3(0.10, 0.12, 0.22)
#define PEARL     vec3(0.95, 0.94, 0.90)

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2  p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip), b = hash21(ip + vec2(1,0));
    float c = hash21(ip + vec2(0,1)), d = hash21(ip + vec2(1,1));
    return mix(mix(a,b,fp.x), mix(c,d,fp.x), fp.y);
}
float fbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) {
        s += a * vnoise(p);
        p = mat2(1.6, 1.2, -1.2, 1.6) * p;
        a *= 0.5;
    }
    return s;
}

// Magritte's illustrative cloud — fbm with a sharp threshold, soft edge.
float magritteCloud(vec2 p, float drift, float coverage) {
    p = vec2(p.x * 1.3, p.y * 2.3) + vec2(drift, 0.0);
    float n = fbm(p) * 0.6 + fbm(p * 2.5 + 7.3) * 0.4;
    float th = 1.0 - coverage * 0.85;
    return smoothstep(th - 0.10, th + 0.10, n);
}

// Bowler-hatted man silhouette: round crown, slim brim, oval head, coat.
float sdBowlerMan(vec2 p, float s) {
    vec2 hp = p - vec2(0.0, s * 1.05);
    float crown = length(vec2(hp.x * 1.05, max(hp.y, 0.0))) - s * 0.42;
    float brim  = max(abs(hp.x) - s * 0.66, abs(hp.y + s * 0.04) - s * 0.06);
    float head  = length(p - vec2(0.0, s * 0.55)) - s * 0.32;
    vec2 cp = p - vec2(0.0, -s * 0.15);
    float taper = mix(0.55, 0.42, clamp(-cp.y / s + 0.5, 0.0, 1.0));
    float coat  = max(abs(cp.x) - s * taper, abs(cp.y) - s * 0.65);
    return min(min(min(crown, brim), head), coat);
}

// Son-of-Man parts: hat, head, coat, apple — kept separate so the apple
// can be coloured Granny Smith and the hat can be black, with an ear that
// peeks past the apple (Magritte's deliberate imperfect occlusion).
float sdSonHat(vec2 p, float s) {
    vec2 hp = p - vec2(0.0, s * 1.10);
    float crown = length(vec2(hp.x * 1.05, max(hp.y, 0.0))) - s * 0.46;
    float brim  = max(abs(hp.x) - s * 0.74, abs(hp.y + s * 0.04) - s * 0.07);
    return min(crown, brim);
}
float sdSonHead(vec2 p, float s) { return length(p - vec2(0.0, s * 0.55)) - s * 0.36; }
float sdSonCoat(vec2 p, float s) {
    vec2 cp = p - vec2(0.0, -s * 0.10);
    float taper = mix(0.60, 0.48, clamp(-cp.y / s + 0.6, 0.0, 1.0));
    return max(abs(cp.x) - s * taper, abs(cp.y) - s * 0.80);
}
float sdSonApple(vec2 p, float s) { return length(p - vec2(0.0, s * 0.55)) - s * 0.30; }

// Treachery pipe — bowl + arced stem + mouthpiece.
float sdPipe(vec2 p, float s) {
    vec2 bp = p - vec2(-s * 0.55, s * 0.05);
    float bowl = min(length(vec2(bp.x, max(bp.y, 0.0))) - s * 0.20,
                     max(abs(bp.x) - s * 0.18, abs(bp.y + s * 0.10) - s * 0.18));
    float stemY = -s * 0.05 - s * 0.10 * sin((p.x + s * 0.3) * 1.4);
    float stem  = max(abs(p.y - stemY) - s * 0.05, abs(p.x - s * 0.10) - s * 0.70);
    float mouth = length(p - vec2(s * 0.78, -s * 0.10)) - s * 0.085;
    return min(min(bowl, stem), mouth);
}

// Procedural cursive — short curved ink strokes that read as Magritte's
// looping handwriting without being literal letterforms.
float scriptStrokes(vec2 p, float t) {
    float cell = 0.045;
    float ix = floor(p.x / cell), fx = fract(p.x / cell);
    float seed = hash11(ix * 1.913);
    float yC = 0.012 * sin(fx * 6.2831 + seed * 6.28) - 0.018 * step(0.92, seed);
    float pen = 0.0028 + 0.0014 * sin(fx * 12.566 + seed * 8.0);
    float skel = abs(p.y - yC) - pen;
    float gap = step(0.86, hash11(ix * 0.31));
    return smoothstep(0.001, -0.002, skel) * (1.0 - gap)
         * (0.95 + 0.05 * sin(t * 0.6 + ix));
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    // Magritte is patient — slow everything to ~0.3x.
    float t = TIME * 0.3;
    float audio = clamp(audioReact, 0.0, 2.0);
    float aBass = audioBass, aMid = audioMid, aTreb = audioHigh;
    int mood = int(magritteMood + 0.5);
    vec3 col = SKY_BLUE;

    // ─── MOOD 0 — GOLCONDA: raining bowler-hatted men ─────────────────
    if (mood == 0) {
        float buildingTop = 0.35;
        if (uv.y < buildingTop) {
            vec2 g = vec2(uv.x * 8.0, uv.y * 5.0);
            vec2 ig = floor(g), fg = fract(g);
            float win = step(0.18, fg.x) * step(fg.x, 0.82)
                      * step(0.30, fg.y) * step(fg.y, 0.78);
            vec3 wall = mix(CREAM * 0.78, CREAM * 0.62, hash21(ig) * 0.5 + 0.25);
            col = mix(wall, NIGHT_IND * 1.4, win * 0.55);
        } else {
            float c = magritteCloud(vec2(uv.x * aspect, uv.y - 0.3),
                                    t * 0.04, cloudCoverage);
            vec3 sky = mix(SKY_BLUE, SKY_BLUE * 1.18,
                           smoothstep(buildingTop, 1.0, uv.y));
            col = mix(sky, PEARL, c * 0.85);
        }
        // Rain of men — 7-column lattice, 3 men per column, staggered.
        // Halved tempo: the men barely fall, they hover.
        float tempo = (rainTempo + 0.30 * aBass * audio) * 0.5;
        float fall  = t * tempo;
        float density = rainDensity * (1.0 + 0.4 * aBass * audio);
        for (int c0 = 0; c0 < 7; c0++) {
            float fc = float(c0);
            float colX = (fc + 0.5) / 7.0;
            for (int r0 = 0; r0 < 3; r0++) {
                float fr = float(r0);
                float seed = hash11(fc * 3.7 + fr * 11.1);
                if (seed > density + 0.1) continue;
                float colPhase = hash11(fc * 1.7) * 1.3;
                float yPhase = fract(fall + colPhase + fr * 0.34);
                float manY = 1.20 - yPhase * 1.40;
                float manX = colX + 0.018 * sin(t * 0.3 + fc * 1.7 + fr);
                vec2 mp = uv - vec2(manX, manY); mp.x *= aspect;
                float s = 0.045 + 0.005 * hash11(seed * 9.1);
                float d = sdBowlerMan(mp, s);
                if (d < 0.0) {
                    float key = clamp(0.55 + 0.45 * dot(normalize(mp + vec2(0.001)),
                                          normalize(vec2(-0.4, 0.8))), 0.0, 1.0);
                    col = mix(HAT_BLACK, SKY_BLUE * 0.18, key * 0.35);
                } else if (d < 0.004) {
                    col = mix(col, HAT_BLACK, smoothstep(0.004, 0.0, d) * 0.7);
                }
            }
        }
    }
    // ─── MOOD 1 — EMPIRE OF LIGHT: the impossible time ────────────────
    else if (mood == 1) {
        float line = clamp(daynightLine + 0.04 * (aMid * audio - 0.3), 0.20, 0.85);
        if (uv.y > line) {
            vec3 sky = mix(SKY_BLUE * 1.05, SKY_BLUE * 1.25,
                           smoothstep(line, 1.0, uv.y));
            float c = magritteCloud(vec2(uv.x * aspect, uv.y - line * 0.6),
                                    t * 0.05, cloudCoverage);
            col = mix(sky, PEARL, c * 0.90);
        } else {
            float groundShade = smoothstep(0.0, line, uv.y);
            col = mix(NIGHT_IND * 0.55, NIGHT_IND * 1.10, groundShade);
            // House
            float wallL = 0.40, wallR = 0.62;
            float wallTop = line - 0.20, wallBot = line - 0.42;
            float wall = step(wallL, uv.x) * step(uv.x, wallR)
                       * step(wallBot, uv.y) * step(uv.y, wallTop);
            float midX = 0.51, slope = (wallR - wallL) * 0.5;
            float roofH = abs(uv.x - midX);
            float roof = step(uv.y, wallTop + (1.0 - roofH / slope) * 0.10)
                       * step(wallTop, uv.y) * step(uv.y, line - 0.06)
                       * step(roofH, slope);
            col = mix(col, NIGHT_IND * 0.30, max(wall, roof));
            // Lit window
            vec2 wd = uv - vec2(0.51, wallBot + 0.10);
            float win = step(abs(wd.x), 0.014) * step(abs(wd.y), 0.020);
            col = mix(col, vec3(1.0, 0.78, 0.40), win * 0.95);
            // Trees — three dark crowns + trunks.
            for (int i = 0; i < 3; i++) {
                float fi = float(i);
                float cx = 0.18 + fi * 0.30;
                float trunk = step(abs(uv.x - cx), 0.01)
                            * step(uv.y, line) * step(line - 0.18, uv.y);
                vec2 cp = uv - vec2(cx, line - 0.02);
                float crown = smoothstep(0.10, 0.06,
                              length(cp * vec2(1.0, 1.7))
                              + 0.02 * vnoise(cp * 30.0));
                col = mix(col, HAT_BLACK, max(trunk, crown));
            }
            // Streetlamp — only light source in the night.
            vec2 lampPos = vec2(0.78, line - 0.18);
            float post = step(abs(uv.x - lampPos.x), 0.004)
                       * step(uv.y, lampPos.y) * step(line - 0.42, uv.y);
            col = mix(col, HAT_BLACK, post);
            vec2 ld = uv - lampPos; ld.x *= aspect;
            float r = length(ld);
            float halo = exp(-r * 7.0) * 0.85 + exp(-r * 22.0) * 0.6
                       + smoothstep(0.018, 0.012, r) * 1.8;
            col += vec3(1.05, 0.85, 0.55) * halo * (0.55 + 0.10 * sin(t * 0.85));
        }
        // Hairline horizon — the soft seam between the two times.
        float lineEdge = 1.0 - smoothstep(0.0, 0.004, abs(uv.y - line));
        col = mix(col, PEARL * 0.85, lineEdge * 0.18);
    }
    // ─── MOOD 2 — SON OF MAN: bowler figure, apple covering face ──────
    else if (mood == 2) {
        float wallTop = 0.22;
        if (uv.y < wallTop) {
            col = mix(CREAM * 0.78, CREAM, uv.y / wallTop);
        } else {
            float c = magritteCloud(vec2(uv.x * aspect, uv.y - 0.3),
                                    t * 0.03, cloudCoverage * 0.6);
            col = mix(SKY_BLUE, SKY_BLUE * 1.18,
                      smoothstep(wallTop, 1.0, uv.y));
            col = mix(col, PEARL, c * 0.75);
        }
        float figSize = 0.34;
        vec2 fp = uv - vec2(0.5, 0.42); fp.x *= aspect;
        float coat = sdSonCoat(fp, figSize);
        if (coat < 0.0) {
            float key = 0.55 + 0.45 * clamp(dot(normalize(fp + vec2(0.001)),
                              normalize(vec2(-0.4, 0.7))), 0.0, 1.0);
            col = mix(NIGHT_IND * 0.5, NIGHT_IND * 0.9, key);
        }
        float head = sdSonHead(fp, figSize);
        if (head < 0.0) col = mix(vec3(0.78, 0.68, 0.58),
                                  vec3(0.92, 0.82, 0.70), 0.5 + 0.5 * fp.y);
        // Ear — uncanny detail, peeks past the apple on the right.
        vec2 ear = fp - vec2(figSize * 0.34, figSize * 0.55);
        float earD = length(ear) - figSize * 0.045;
        float hat = sdSonHat(fp, figSize);
        if (hat < 0.0) col = HAT_BLACK;
        // Apple — slightly off-centre + hover with mid-band audio.
        vec2 appleOffset = vec2(-0.005, appleHover * sin(t * 0.35 + aMid * audio * 2.0));
        vec2 ap = fp - appleOffset;
        float apple = sdSonApple(ap, figSize);
        if (earD < 0.0 && apple > 0.0 && hat > 0.0) col = vec3(0.85, 0.70, 0.62);
        if (apple < 0.0) {
            float r = length(ap - vec2(0.0, figSize * 0.55)) / (figSize * 0.30);
            vec3 g = mix(APPLE_GRN * 1.05, APPLE_GRN * 0.55, smoothstep(0.5, 1.0, r));
            float hi = smoothstep(figSize * 0.06, 0.0,
                       length(ap - vec2(-figSize * 0.08, figSize * 0.65)));
            g += PEARL * hi * 0.5;
            float dim = smoothstep(figSize * 0.04, 0.0,
                        length(ap - vec2(0.0, figSize * 0.85)));
            col = mix(g, APPLE_GRN * 0.35, dim);
        }
    }

    // ─── MOOD 3 — TREACHERY OF IMAGES: pipe + script ─────────────────
    else if (mood == 3) {
        col = CREAM;
        col -= 0.025 * (vnoise(uv * RENDERSIZE.xy * 0.05) - 0.5);
        vec2 pp = uv - vec2(0.5, 0.58); pp.x *= aspect;
        float pd = sdPipe(pp, 0.28);
        if (pd < 0.0) {
            float key = 0.55 + 0.45 * clamp(dot(normalize(pp + vec2(0.001)),
                              normalize(vec2(-0.5, 0.7))), 0.0, 1.0);
            col = mix(vec3(0.18, 0.12, 0.08), vec3(0.42, 0.28, 0.18), key);
        } else if (pd < 0.008) {
            col = mix(col, vec3(0.10, 0.07, 0.05), smoothstep(0.008, 0.0, pd) * 0.95);
        }
        // Script line — "Ceci n'est pas une pipe" as procedural strokes.
        vec2 sp = (uv - vec2(0.50, 0.20)); sp.x *= aspect;
        if (abs(sp.x) < 0.31 && abs(sp.y) < 0.04) {
            float ink = scriptStrokes(sp, t);
            col = mix(col, vec3(0.10, 0.07, 0.05), ink);
        }
        // The uncanny: a ghost of smoke that shouldn't exist in Magritte's
        // painting briefly rises from the bowl, then catches itself.
        if (uncannyAmount > 0.0) {
            float ph = fract(t * 0.025);
            float vis = smoothstep(0.0, 0.05, ph) * smoothstep(0.18, 0.10, ph);
            vec2 sd = uv - (vec2(0.5, 0.58) + vec2(-0.28 * 0.55 / aspect, 0.28 * 0.20));
            sd.y -= ph * 0.18;
            sd.x -= 0.02 * sin(sd.y * 9.0 + t * 0.5);
            float smoke = exp(-dot(sd, sd) * 220.0);
            col = mix(col, PEARL, smoke * vis * uncannyAmount * 0.35);
        }
    }
    // ─── MOOD 4 — THE LOVERS (1928): two veil-shrouded heads, embracing ─
    else {
        // Warm interior — papered wall, low light. Quiet. Held breath.
        vec3 wallTop = vec3(0.46, 0.30, 0.26);   // dim red-brown wall
        vec3 wallBot = vec3(0.30, 0.18, 0.16);
        col = mix(wallBot, wallTop, smoothstep(0.0, 1.0, uv.y));
        // Subtle wall paper grain so the room reads as a room, not a void.
        float grain = vnoise(uv * RENDERSIZE.xy * 0.018);
        col *= 0.94 + 0.10 * grain;
        // Soft warm key from upper-left — like an unseen window.
        vec2 lp = uv - vec2(0.18, 0.92);
        float key = exp(-dot(lp, lp) * 1.6);
        col += vec3(0.55, 0.42, 0.30) * key * 0.30;

        // Two head silhouettes, leaning in toward each other.
        // Left head: tilts slightly right; Right head: tilts slightly left.
        vec2 q = uv - vec2(0.5, 0.50); q.x *= aspect;
        float headR = 0.20;
        // Tiny breath sway — extremely slow.
        float sway = 0.004 * sin(t * 0.4);
        vec2 cL = vec2(-0.13 + sway, 0.02);
        vec2 cR = vec2( 0.13 - sway, 0.02);

        // Each head: an oval (wider at bottom, jaw) + neck + shoulder.
        vec2 hL = q - cL;
        vec2 hR = q - cR;
        // Slight inward tilt: rotate each head a few degrees toward the centre.
        float ang = 0.18;
        mat2 rotL = mat2( cos( ang), -sin( ang), sin( ang),  cos( ang));
        mat2 rotR = mat2( cos(-ang), -sin(-ang), sin(-ang),  cos(-ang));
        vec2 hLr = rotL * hL;
        vec2 hRr = rotR * hR;

        // Head ovals (taller than wide, slight jaw).
        float ovalL = length(hLr / vec2(0.85, 1.05)) - headR;
        float ovalR = length(hRr / vec2(0.85, 1.05)) - headR;
        // Necks + shoulders, descending into the bottom of frame.
        vec2 nL = q - vec2(cL.x - 0.02, -0.18);
        vec2 nR = q - vec2(cR.x + 0.02, -0.18);
        float neckL = max(abs(nL.x) - 0.07, abs(nL.y) - 0.22);
        float neckR = max(abs(nR.x) - 0.07, abs(nR.y) - 0.22);
        float shoulderY = -0.22;
        float shoulders = step(uv.y, 0.5 + shoulderY)
                        * smoothstep(0.0, 0.04, 0.34 - abs(q.x));
        // Combined silhouette mask (1 inside head/neck/shoulder).
        float silL = smoothstep(0.004, -0.004, min(ovalL, neckL));
        float silR = smoothstep(0.004, -0.004, min(ovalR, neckR));
        float sil  = max(max(silL, silR), shoulders);

        // Veil cloth — pale grey-white linen with directional drape folds.
        // Two scales of fbm, biased to vertical streaks (drape).
        vec2 dL = vec2(hLr.x * 1.2, hLr.y * 0.55);
        vec2 dR = vec2(hRr.x * 1.2, hRr.y * 0.55);
        // Slow cloth shimmer — barely-there breathing of the fabric.
        float drapeT = t * 0.08;
        float foldL = fbm(dL * 9.0 + vec2(0.0, drapeT))
                    + 0.4 * fbm(dL * 22.0 - vec2(drapeT, 0.0));
        float foldR = fbm(dR * 9.0 - vec2(0.0, drapeT))
                    + 0.4 * fbm(dR * 22.0 + vec2(drapeT, 0.0));
        // Vertical creases — strong narrow bands.
        float creaseL = 0.5 + 0.5 * sin(hLr.x * 32.0 + foldL * 3.0);
        float creaseR = 0.5 + 0.5 * sin(hRr.x * 32.0 + foldR * 3.0);
        creaseL = pow(creaseL, 2.5);
        creaseR = pow(creaseR, 2.5);

        // Veil shading: pale linen, key from upper-left, deep shadow in folds.
        vec3 linen = vec3(0.86, 0.83, 0.78);
        vec3 linenShade = vec3(0.34, 0.30, 0.28);
        // Per-head shading directions (point away from inward tilt).
        vec3 veilL = mix(linenShade, linen, clamp(0.55 + 0.45 * (-hLr.x + hLr.y), 0.0, 1.0));
        vec3 veilR = mix(linenShade, linen, clamp(0.55 + 0.45 * ( hRr.x + hRr.y), 0.0, 1.0));
        veilL = mix(veilL, linenShade * 0.85, creaseL * 0.55);
        veilR = mix(veilR, linenShade * 0.85, creaseR * 0.55);
        // The knotted twist at the back of each head.
        vec2 knotL = hLr - vec2(-0.10, 0.10);
        vec2 knotR = hRr - vec2( 0.10, 0.10);
        float knL = exp(-dot(knotL, knotL) * 380.0);
        float knR = exp(-dot(knotR, knotR) * 380.0);
        veilL = mix(veilL, linenShade, knL * 0.6);
        veilR = mix(veilR, linenShade, knR * 0.6);

        // Composite veils onto silhouettes.
        // Pick the nearer head by which oval is more inside.
        float pickR = step(ovalR, ovalL);
        vec3 veil = mix(veilL, veilR, pickR);

        // Shoulders darker (suit/coat), separate tone.
        vec3 coat = vec3(0.10, 0.08, 0.10);
        // If the pixel is below the heads, treat as coat.
        float coatMask = step(uv.y, 0.5 + shoulderY + 0.02);

        // Soft rim where heads meet (kiss line — slightly darker).
        float kiss = exp(-pow((q.x) / 0.012, 2.0)) * smoothstep(-0.05, 0.10, q.y) * smoothstep(0.18, 0.05, q.y);

        col = mix(col, veil, sil * (1.0 - coatMask));
        col = mix(col, coat, sil * coatMask);
        col = mix(col, linenShade * 0.6, sil * kiss * 0.55);

        // Treble: faint pearl beadwork along veil edges (rare, on shadow).
        float edge = smoothstep(0.010, 0.0, abs(min(ovalL, ovalR)));
        if (uncannyAmount > 0.0) {
            col += PEARL * edge * 0.06 * uncannyAmount;
        }
    }

    // Treble pearl sparks — only on shadowed regions.
    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    if (lum < 0.35) {
        float sparkSeed = hash21(floor(uv * RENDERSIZE * 0.8 + t * 0.5));
        col += PEARL * step(0.998, sparkSeed) * aTreb * audio * 0.40;
    }
    // Soft vignette — Magritte's deadpan framing.
    vec2 c2 = uv - 0.5;
    col *= 1.0 - 0.18 * dot(c2, c2);
    gl_FragColor = vec4(col, 1.0);
}
