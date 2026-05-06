/*{
    "DESCRIPTION": "Spectral Prism 3D — glass triangular prism with chromatic dispersion beams",
    "CREDIT": "ShaderClaw auto-improve 2026-05-06",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 3.0,
            "LABEL": "Camera Speed"
        },
        {
            "NAME": "beamSpread",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.5,
            "LABEL": "Beam Spread"
        },
        {
            "NAME": "hdrPeak",
            "TYPE": "float",
            "DEFAULT": 2.5,
            "MIN": 1.0,
            "MAX": 5.0,
            "LABEL": "HDR Peak"
        },
        {
            "NAME": "audioReact",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Audio React"
        }
    ]
}*/

// ── SDF helpers ──────────────────────────────────────────────────────────────

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float distToSegment(vec3 p, vec3 a, vec3 b) {
    vec3 pa = p - a;
    vec3 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// ── Scene SDF ────────────────────────────────────────────────────────────────

float sceneSDF(vec3 p) {
    // Glass prism (sdBox approximation for triangular prism)
    float prism = sdBox(p, vec3(0.22, 0.75, 0.20));
    return prism;
}

vec3 sceneNormal(vec3 p) {
    float e = 0.001;
    return normalize(vec3(
        sceneSDF(p + vec3(e, 0.0, 0.0)) - sceneSDF(p - vec3(e, 0.0, 0.0)),
        sceneSDF(p + vec3(0.0, e, 0.0)) - sceneSDF(p - vec3(0.0, e, 0.0)),
        sceneSDF(p + vec3(0.0, 0.0, e)) - sceneSDF(p - vec3(0.0, 0.0, e))
    ));
}

// ── Volumetric beam glow ─────────────────────────────────────────────────────

// Accumulate glow along a ray for a single beam (capsule from a to b, width w)
float beamGlow(vec3 ro, vec3 rd, vec3 a, vec3 b, float w) {
    float acc = 0.0;
    float stepSize = 0.04;
    for (int i = 0; i < 32; i++) {
        float t = float(i) * stepSize;
        vec3 p = ro + rd * t;
        float d = distToSegment(p, a, b);
        acc += exp(-d / w) * stepSize;
    }
    return acc;
}

// ── Main ─────────────────────────────────────────────────────────────────────

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Audio modulator
    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.5) * audioReact;

    // Camera orbit around prism
    float tm = TIME * speed * 0.12;
    float camR = 3.5;
    vec3 ro = vec3(sin(tm) * camR, 0.5 + sin(tm * 0.41) * 0.4, cos(tm) * camR);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 forward = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up = cross(forward, right);
    vec3 rd = normalize(forward + uv.x * right + uv.y * up);

    // ── Beam endpoints ────────────────────────────────────────────────────────
    // Input white beam: enters from the left
    vec3 beamInA = vec3(-3.5, 0.0, 0.0);
    vec3 beamInB = vec3(-0.22, 0.0, 0.0);

    // Beam spread based on parameter
    float spread = beamSpread * 0.5 + 0.1;

    // Output beam: crimson — goes up-right
    vec3 crimsonA = vec3(0.22, 0.0, 0.0);
    vec3 crimsonB = vec3(3.0, 1.8 * spread, 0.0);

    // Output beam: electric blue — goes right-center
    vec3 blueA = vec3(0.22, 0.0, 0.0);
    vec3 blueB = vec3(3.0, 0.0, 0.0);

    // Output beam: acid yellow — goes down-right
    vec3 yellowA = vec3(0.22, 0.0, 0.0);
    vec3 yellowB = vec3(3.0, -1.8 * spread, 0.0);

    // ── Raymarch ──────────────────────────────────────────────────────────────
    float tMarch = 0.0;
    bool hit = false;
    vec3 hitPos = vec3(0.0);

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * tMarch;
        float d = sceneSDF(p);
        if (d < 0.001) {
            hit = true;
            hitPos = p;
            break;
        }
        if (tMarch > 20.0) break;
        tMarch += d;
    }

    // ── Volumetric beam accumulation ──────────────────────────────────────────
    float bw = 0.07;  // beam width

    float glowIn  = beamGlow(ro, rd, beamInA, beamInB, bw);
    float glowCrimson = beamGlow(ro, rd, crimsonA, crimsonB, bw * 0.8);
    float glowBlue    = beamGlow(ro, rd, blueA,    blueB,    bw * 0.8);
    float glowYellow  = beamGlow(ro, rd, yellowA,  yellowB,  bw * 0.8);

    // ── Color composition ─────────────────────────────────────────────────────

    // HDR beam colors (fully saturated)
    vec3 crimsonCol = vec3(2.0, 0.0, 0.05) * hdrPeak * 0.8;
    vec3 blueCol    = vec3(0.0, 0.5, 3.0)  * hdrPeak * 0.8;
    vec3 yellowCol  = vec3(2.5, 1.8, 0.0)  * hdrPeak * 0.8;
    vec3 whiteCol   = vec3(2.0, 1.9, 1.7);

    vec3 col = vec3(0.0);  // void black background

    // Volumetric beam contributions
    col += crimsonCol * glowCrimson * audio * 0.4;
    col += blueCol    * glowBlue    * audio * 0.4;
    col += yellowCol  * glowYellow  * audio * 0.4;
    col += whiteCol   * glowIn      * audio * 0.25;

    // Prism surface shading on hit
    if (hit) {
        vec3 n = sceneNormal(hitPos);
        // Glass-like tint: prismatic sheen based on normal
        vec3 refDir = reflect(rd, n);
        float rim = pow(max(1.0 - dot(-rd, n), 0.0), 3.0);

        // Prismatic color: blend between beam colors based on normal.y
        float t = n.y * 0.5 + 0.5;
        vec3 prismCol = mix(crimsonCol, mix(blueCol, yellowCol, t), t * 0.5);
        prismCol = mix(prismCol, vec3(2.0, 2.0, 2.2), rim * 0.6);

        float diff = max(dot(n, normalize(vec3(1.0, 1.0, 2.0))), 0.0);
        col += prismCol * (0.3 + diff * 0.5) * 0.6;

        // AA edge with fwidth
        float edgeDist = abs(sceneSDF(hitPos));
        float fw = fwidth(edgeDist);
        float edgeMask = smoothstep(fw * 2.0, 0.0, edgeDist);
        col += vec3(1.5, 1.8, 2.0) * edgeMask * 0.4;
    }

    gl_FragColor = vec4(col, 1.0);
}
