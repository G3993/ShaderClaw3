/*{
  "DESCRIPTION": "VHS Horror Playback — analog tape degradation nightmare: deep red/amber damage artifacts, tracking errors, image ghost burns, static grain forming dark figures",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "degradeRate",  "LABEL": "Degrade Rate",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8  },
    { "NAME": "trackingErr",  "LABEL": "Tracking Error","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5  },
    { "NAME": "ghostBurn",    "LABEL": "Ghost Burn",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0  },
    { "NAME": "hdrBoost",     "LABEL": "HDR Boost",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.2  },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: VHS horror ───────────────────────────────────────────────────────
const vec3 BLOOD_RED   = vec3(0.90, 0.00, 0.02);
const vec3 AMBER_BURN  = vec3(1.00, 0.45, 0.00);
const vec3 GHOST_WHITE = vec3(2.00, 1.80, 1.40);
const vec3 DEEP_BLACK  = vec3(0.00, 0.00, 0.00);
const vec3 STATIC_GRAY = vec3(0.30, 0.25, 0.20);

float hash(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash2(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// VHS grain noise (coarse horizontal bands)
float vhsGrain(vec2 uv, float t) {
    float line = floor(uv.y * 240.0);
    float frame = floor(t * degradeRate * 15.0);
    return hash(line * 7.3 + frame * 19.1 + uv.x * 0.3);
}

// Tracking error: horizontal scan displacement bands
float trackingShift(float y, float t) {
    float band = floor(y * 6.0 + t * degradeRate * 0.7);
    float noise = hash(band * 3.7 + floor(t * 2.0) * 11.3);
    float active = step(1.0 - trackingErr * 0.5, noise);
    return (noise - 0.5) * 0.12 * active;
}

// Ghost image: faint afterimage displaced left
vec3 ghostLayer(vec2 uv, float t) {
    float ghostX = uv.x - 0.04 * ghostBurn - hash(floor(uv.y * 30.0 + t)) * 0.01;
    float ghostY = uv.y + (hash(floor(uv.y * 50.0)) - 0.5) * 0.003;
    // Generate a "ghost signal" as dark vertical smear shapes
    float smear = 0.0;
    for (int i = 0; i < 4; i++) {
        float cx = hash(float(i) * 3.7 + floor(t * degradeRate * 0.3) * 7.0);
        float cy = hash(float(i) * 5.1 + floor(t * degradeRate * 0.2) * 9.0);
        float w  = 0.02 + hash(float(i) * 2.3) * 0.05;
        float h  = 0.15 + hash(float(i) * 4.1) * 0.35;
        float bx = smoothstep(w, 0.0, abs(ghostX - cx));
        float by = smoothstep(0.0, h, abs(ghostY - cy));
        smear = max(smear, bx * (1.0 - by));
    }
    return BLOOD_RED * smear * ghostBurn;
}

// Damage artifact: bright horizontal band tears
vec3 tearBand(vec2 uv, float t) {
    float band = fract(uv.y * 3.0 + t * degradeRate * 0.15);
    float tear = smoothstep(0.96, 1.0, band) * hash2(vec2(floor(t * degradeRate * 2.0), floor(uv.y * 3.0)));
    return AMBER_BURN * tear * 1.5;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float t  = TIME;
    float aud = 1.0 + (audioLevel + audioBass * 0.8) * audioReact * 0.5;

    // Apply tracking error displacement
    float xShift = trackingShift(uv.y, t);
    vec2 uvD = vec2(uv.x + xShift, uv.y);

    // Base: dark static grain
    float grain = vhsGrain(uv, t);
    vec3 col = DEEP_BLACK;

    // Static grain color (amber-tinted static)
    float staticMask = step(0.65, grain);
    col += STATIC_GRAY * staticMask * 0.4 * (0.5 + 0.5 * hash2(uv * 200.0 + t));

    // Scanline darkening (CRT-style)
    float scanline = 0.85 + 0.15 * sin(uv.y * 480.0 * 3.14159);
    col *= scanline;

    // Dark shape silhouettes emerging from static (the "horror figure")
    float figure = 0.0;
    vec2 figPos = vec2(0.5 + 0.05 * sin(t * degradeRate * 0.1), 0.5);
    // Head
    float head = length((uv - figPos - vec2(0, 0.22)) / vec2(0.07, 0.09)) - 1.0;
    figure = max(figure, smoothstep(0.05, -0.02, head));
    // Body
    float body = length((uv - figPos) / vec2(0.06, 0.20)) - 1.0;
    figure = max(figure, smoothstep(0.05, -0.02, body));
    // Flicker: figure fades in and out
    float flicker = step(0.35, abs(sin(t * degradeRate * 1.7 + 0.3)));
    col = mix(col, DEEP_BLACK, figure * flicker * 0.9);

    // Blood red ghost layer
    col += ghostLayer(uvD, t) * aud;

    // Amber tear bands
    col += tearBand(uvD, t) * aud;

    // Red damage channel in corrupted zones
    float damage = step(0.88, hash2(vec2(floor(uv.y * 48.0), floor(t * degradeRate * 3.0))));
    col += BLOOD_RED * damage * 0.5 * aud;

    // HDR boost
    col *= hdrBoost;

    // White-hot static sparks
    float spark = step(0.997, hash2(uv * 300.0 + t));
    col += GHOST_WHITE * spark * hdrBoost;

    gl_FragColor = vec4(col, 1.0);
}
