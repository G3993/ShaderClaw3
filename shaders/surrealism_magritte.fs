/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Surrealism after Magritte's Empire of Light (1953) and Golconda (1953) — photoreal procedural sky with illustrative clouds, a floating SDF object (apple, bowler hat, rock or pipe), long hard-light shadow on the ground, and Golconda-style ghost duplicates spawning on bass impacts. The impossible rendered with deadpan illustration.",
  "INPUTS": [
    { "NAME": "magritteScene", "LABEL": "Scene", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Standard Sky", "Empire of Light", "Castle of the Pyrenees", "Treachery of Images", "Black Magic"] },
    { "NAME": "objectChoice", "LABEL": "Object", "TYPE": "long", "DEFAULT": 4, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Apple", "Bowler Hat", "Rock", "Pipe", "Son of Man"] },
    { "NAME": "objectSize", "LABEL": "Object Size", "TYPE": "float", "MIN": 0.04, "MAX": 0.30, "DEFAULT": 0.13 },
    { "NAME": "objectX", "LABEL": "Object X", "TYPE": "float", "MIN": 0.2, "MAX": 0.8, "DEFAULT": 0.5 },
    { "NAME": "horizonY", "LABEL": "Horizon", "TYPE": "float", "MIN": 0.25, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyTopColor", "LABEL": "Sky Top", "TYPE": "color", "DEFAULT": [0.30, 0.55, 0.85, 1.0] },
    { "NAME": "skyHorizonColor", "LABEL": "Sky Horizon", "TYPE": "color", "DEFAULT": [0.95, 0.88, 0.75, 1.0] },
    { "NAME": "groundColor", "LABEL": "Ground", "TYPE": "color", "DEFAULT": [0.32, 0.30, 0.28, 1.0] },
    { "NAME": "cloudCoverage", "LABEL": "Cloud Coverage", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45 },
    { "NAME": "cloudSharpness", "LABEL": "Cloud Sharpness", "TYPE": "float", "MIN": 0.05, "MAX": 0.5, "DEFAULT": 0.18 },
    { "NAME": "cloudDrift", "LABEL": "Cloud Drift", "TYPE": "float", "MIN": 0.0, "MAX": 0.3, "DEFAULT": 0.06 },
    { "NAME": "hoverAmp", "LABEL": "Hover Amount", "TYPE": "float", "MIN": 0.0, "MAX": 0.10, "DEFAULT": 0.025 },
    { "NAME": "shadowStrength", "LABEL": "Shadow", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "shadowAngle", "LABEL": "Shadow Angle", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 5.5 },
    { "NAME": "ghostMultiply", "LABEL": "Golconda Ghosts", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "ghostCount", "LABEL": "Ghost Count", "TYPE": "float", "MIN": 0.0, "MAX": 12.0, "DEFAULT": 10.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "useTex", "LABEL": "Texture as Object Skin", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
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

// Magritte cloud — fbm-of-fbm with sharp threshold to get the illustrative
// shape, then soft falloff just at the silhouette so it doesn't pixel-edge.
float magritteCloud(vec2 uv, float t, float coverage, float sharpness) {
    vec2 p = uv * vec2(1.4, 2.6) + vec2(t * 0.3, 0.0);
    float n = fbm(p) * 0.6 + fbm(p * 2.5 + 7.3) * 0.4;
    float threshold = 1.0 - coverage * 0.9;
    return smoothstep(threshold - sharpness * 0.5,
                      threshold + sharpness * 0.5, n);
}

// Object SDFs (centred at origin, scaled by `s`).
float sdApple(vec2 p, float s) {
    float body = length(p) - s;
    // Stem
    float stem = max(abs(p.x) - s * 0.06, abs(p.y - s * 1.05) - s * 0.18);
    return min(body, stem);
}
float sdBowler(vec2 p, float s) {
    // Hat body — half-circle on top, brim slab on bottom.
    float crown = length(vec2(p.x, max(p.y, 0.0))) - s * 0.85;
    float brim  = max(abs(p.x) - s * 1.2, abs(p.y + s * 0.05) - s * 0.10);
    return min(crown, brim);
}
float sdRock(vec2 p, float s) {
    // Lumpy rounded square — irregular blob.
    vec2 q = abs(p) - vec2(s * 0.85, s * 0.7);
    float d = length(max(q, 0.0)) - s * 0.18;
    d -= 0.04 * s * vnoise(p * 6.0);
    return d;
}
float sdPipe(vec2 p, float s) {
    // Bowl + stem pipe.
    float bowl = length(p - vec2(-s * 0.35, 0.0)) - s * 0.4;
    float stem = max(abs(p.y) - s * 0.05,
                     abs(p.x - s * 0.4) - s * 0.55);
    return min(bowl, stem);
}

// Son of Man — bowler-hat figure with green apple covering face.
// The single most-reproduced Magritte; previously unreachable.
// Returns the union SDF; the renderer treats the apple region as a
// distinct material via a colour test inside main().
float sdSonOfMan(vec2 p, float s) {
    // Hat sitting atop a head + shoulders profile
    vec2 hp = p - vec2(0.0, s * 0.65);
    float crown = length(vec2(hp.x * 1.05, max(hp.y, 0.0))) - s * 0.42;
    float brim  = max(abs(hp.x) - s * 0.62, abs(hp.y + s * 0.04) - s * 0.06);
    float hat   = min(crown, brim);
    // Head (hidden behind apple)
    float head  = length(p - vec2(0.0, s * 0.10)) - s * 0.42;
    // Shoulders
    float sh = length(max(vec2(abs(p.x) - s * 0.55,
                                abs(p.y + s * 0.42) - s * 0.10), 0.0))
             - s * 0.20;
    // Apple in front of face — slightly off-centre and forward.
    float apple = length(p - vec2(0.0, s * 0.06)) - s * 0.30;
    return min(min(min(hat, head), sh), apple);
}

bool sonOfManApple(vec2 p, float s) {
    return length(p - vec2(0.0, s * 0.06)) < s * 0.30;
}

float objectSDF(vec2 p, float s, int kind) {
    if (kind == 0) return sdApple(p, s);
    if (kind == 1) return sdBowler(p, s);
    if (kind == 2) return sdRock(p, s);
    if (kind == 3) return sdPipe(p, s);
    return sdSonOfMan(p, s);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Per-scene sky/ground palette. Magritte's compositional set spans:
    // Standard daytime sky (default), Empire of Light's day-sky-over-
    // night-street paradox, the Castle of the Pyrenees floating-rock
    // sea, the Treachery of Images flat copperplate cream, and the
    // Black Magic dark-sky figure-on-cliff scene.
    int scene = int(magritteScene);
    vec3 skyTop = skyTopColor.rgb;
    vec3 skyHor = skyHorizonColor.rgb;
    vec3 grdC   = groundColor.rgb;
    if (scene == 1) {        // Empire of Light — day sky, night street
        skyTop = vec3(0.42, 0.62, 0.85);
        skyHor = vec3(0.95, 0.92, 0.78);
        grdC   = vec3(0.05, 0.05, 0.08);   // pitch-black street
    } else if (scene == 2) { // Castle of the Pyrenees — sea + sky
        skyTop = vec3(0.28, 0.40, 0.62);
        skyHor = vec3(0.50, 0.62, 0.75);
        grdC   = vec3(0.18, 0.30, 0.42);   // dark stormy sea
    } else if (scene == 3) { // Treachery of Images — flat cream ground
        skyTop = vec3(0.94, 0.90, 0.78);
        skyHor = vec3(0.94, 0.90, 0.78);
        grdC   = vec3(0.94, 0.90, 0.78);
    } else if (scene == 4) { // Black Magic — dark sky, pale figure
        skyTop = vec3(0.06, 0.05, 0.10);
        skyHor = vec3(0.18, 0.16, 0.22);
        grdC   = vec3(0.10, 0.10, 0.14);
    }

    vec3 sky = mix(skyHor, skyTop, smoothstep(horizonY, 1.0, uv.y));
    vec3 ground = mix(grdC * 0.65, grdC * 1.05,
                      smoothstep(0.0, horizonY, uv.y));
    vec3 col = (uv.y > horizonY) ? sky : ground;

    // Magritte clouds — only above horizon.
    if (uv.y > horizonY) {
        float c = magritteCloud(vec2(uv.x * aspect, uv.y - horizonY * 0.6),
                                TIME * cloudDrift + audioMid * audioReact * 0.05,
                                cloudCoverage, cloudSharpness);
        // Subtle underside shadow
        float underside = 1.0 - smoothstep(0.0, 0.04,
                                  uv.y - horizonY - 0.25);
        col = mix(col, vec3(0.96, 0.94, 0.90), c * 0.85);
        col = mix(col, vec3(0.84, 0.84, 0.86),
                  c * underside * 0.25);
    }

    // Object — hovering above the horizon with slight oscillation.
    vec2 objCtr = vec2(objectX, horizonY + 0.12
                                + sin(TIME * 0.5) * hoverAmp
                                * (1.0 + audioLevel * audioReact));
    vec2 d = (uv - objCtr);
    d.x *= aspect;
    int kind = int(objectChoice);

    // Long shadow first — rake along shadowAngle direction projected onto
    // the ground plane below horizon.
    if (shadowStrength > 0.0 && uv.y < horizonY) {
        // Shadow direction slowly rotates — long sundial-like sweep
        // across the ground rather than a fixed-angle silhouette.
        float shA = shadowAngle + TIME * 0.05;
        vec2 shadowDir = vec2(cos(shA), sin(shA));
        // Project this fragment back along shadow direction; if the
        // resulting point lies within the object SDF, we're in shadow.
        // Stretch projection so shadow is long.
        float toGround = (horizonY - uv.y);
        vec2 proj = uv + shadowDir * toGround * 2.4;
        proj.y = objCtr.y; // pin to object's y for sampling
        vec2 pp = (proj - objCtr); pp.x *= aspect;
        float sd = objectSDF(pp, objectSize * 1.05, kind);
        float shadow = 1.0 - smoothstep(0.0, 0.04, sd);
        // Shadow fades with distance from object's base on ground.
        float fadeDist = abs(uv.y - horizonY) + 0.04;
        col *= 1.0 - shadow * shadowStrength
                   * smoothstep(0.5, 0.0, fadeDist);
    }

    // Object fill — hard SDF, lit from upper-left so it has volume.
    float od = objectSDF(d, objectSize, kind);
    if (od < 0.0) {
        vec3 obj;
        if (useTex && IMG_SIZE_inputTex.x > 0.0) {
            obj = texture(inputTex, (uv - objCtr) / objectSize * 0.5 + 0.5).rgb;
        } else {
            // Default object skin per kind.
            if (kind == 0)      obj = vec3(0.45, 0.78, 0.20); // apple green
            else if (kind == 1) obj = vec3(0.10, 0.08, 0.06); // bowler black
            else if (kind == 2) obj = vec3(0.62, 0.55, 0.45); // rock
            else if (kind == 3) obj = vec3(0.26, 0.16, 0.10); // pipe brown
            else {
                // Son of Man — apple is green, hat is black, body is
                // dark suit. Test which sub-region this fragment is in.
                if (sonOfManApple(d, objectSize)) {
                    obj = vec3(0.42, 0.74, 0.20);   // apple green
                } else if (d.y > objectSize * 0.55) {
                    obj = vec3(0.10, 0.08, 0.06);   // bowler hat
                } else {
                    obj = vec3(0.16, 0.14, 0.18);   // dark suit / body
                }
            }
        }
        // Lambert shading from upper-left
        float lit = 0.55 + 0.45 * dot(normalize(d + vec2(0.001)),
                                      normalize(vec2(-0.5, 0.7)));
        obj *= lit;
        col = obj;
    }

    // Golconda ghost duplicates — always present, fading in and out
    // continuously so the canvas is never just one apple. Bass adds a
    // momentary boost to the visible count.
    if (ghostMultiply > 0.0) {
        int GN = int(clamp(ghostCount, 0.0, 12.0));
        for (int g = 0; g < 12; g++) {
            if (g >= GN) break;
            float fg = float(g);
            // Golconda grid — 4×3 lattice of bowler-hats drifting gently.
            vec2 grid = vec2(mod(fg, 4.0), floor(fg / 4.0));
            vec2 off = (grid - vec2(1.5, 1.5)) * 0.22
                     + 0.04 * vec2(sin(TIME * 0.20 + fg),
                                   cos(TIME * 0.17 + fg));
            vec2 gctr = objCtr + off;
            vec2 gd = uv - gctr; gd.x *= aspect;
            float gsd = objectSDF(gd, objectSize * 0.8, kind);
            if (gsd < 0.01) {
                float ghostOpacity = (0.35 + 0.45 * audioBass * audioReact)
                                   * (0.5 + 0.5 * sin(TIME * 0.30 + fg * 1.7));
                col = mix(col, vec3(0.10, 0.08, 0.06),
                          ghostMultiply * ghostOpacity * 0.85
                          * (1.0 - hash11(fg * 5.3) * 0.4));
            }
        }
    }

    // Surprise: every ~33s a green apple silhouette grows then deflates
    // at the canvas centre — Magritte's "Son of Man". Briefly impossible.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 33.0);
        float _f  = smoothstep(0.0, 0.06, _ph) * smoothstep(0.32, 0.20, _ph);
        float _scale = 0.10 * sin(_ph * 9.4);
        vec2 _d = (_suv - vec2(0.5, 0.5)) / max(_scale, 1e-3);
        float _apple = smoothstep(1.10, 0.95, length(_d) + 0.10 * cos(atan(_d.y, _d.x) * 5.0));
        col = mix(col, vec3(0.20, 0.55, 0.18), _f * _apple * 0.65);
    }

    gl_FragColor = vec4(col, 1.0);
}
