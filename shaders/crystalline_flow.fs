/*{
  "DESCRIPTION": "Crystalline Flow — a swarm of particles marches through a value-noise field, but each one snaps its heading to one of N discrete facets, so smooth curl shatters into angular rivers of light. Hue encodes direction; a decay buffer paints long-exposure trails. Reborn from a Shadertoy multi-buffer flow field: the soul is 'watch a noise field reveal itself as self-organizing crystalline light, painted by where things are going.' Buffer A advects particles, Buffer B draws+accumulates glowing dots, the image pass outputs the trail buffer.",
  "CREDIT": "Reinterpreted for Easel ISF (flow-field light-painting lineage).",
  "CATEGORIES": ["Generator", "Simulation", "Particles"],
  "INPUTS": [
    { "NAME": "facets",     "LABEL": "Facets",        "TYPE": "float", "MIN": 2.0,   "MAX": 32.0,  "DEFAULT": 8.0 },
    { "NAME": "flowSpeed",  "LABEL": "Flow Speed",    "TYPE": "float", "MIN": 0.1,   "MAX": 4.0,   "DEFAULT": 1.0 },
    { "NAME": "simSpeed",   "LABEL": "Field Drift",   "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.1 },
    { "NAME": "noiseScale", "LABEL": "Field Scale",   "TYPE": "float", "MIN": 0.5,   "MAX": 12.0,  "DEFAULT": 4.0 },
    { "NAME": "trail",      "LABEL": "Trail",         "TYPE": "float", "MIN": 0.0,   "MAX": 0.995, "DEFAULT": 0.97 },
    { "NAME": "density",    "LABEL": "Density",       "TYPE": "float", "MIN": 0.05,  "MAX": 1.0,   "DEFAULT": 0.7 },
    { "NAME": "glow",       "LABEL": "Glow",          "TYPE": "float", "MIN": 0.3,   "MAX": 3.0,   "DEFAULT": 1.0 },
    { "NAME": "sharpness",  "LABEL": "Glow Sharpness","TYPE": "float", "MIN": 1.0,   "MAX": 2.2,   "DEFAULT": 1.4 },
    { "NAME": "hueSpeed",   "LABEL": "Hue Cycle",     "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,   "DEFAULT": 1.0 },
    { "NAME": "saturation", "LABEL": "Saturation",    "TYPE": "float", "MIN": 0.0,   "MAX": 1.5,   "DEFAULT": 1.0 },
    { "NAME": "audioReact", "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.0 }
  ],
  "PASSES": [
    { "TARGET": "simBuf",   "PERSISTENT": true },
    { "TARGET": "trailBuf", "PERSISTENT": true },
    {}
  ]
}*/

// ─────────────────────────────────────────────────────────────────────
// Crystalline Flow.  Three passes:
//   PASSINDEX 0 -> simBuf   : one particle per texel in column 0 (rows 0..COUNT-1).
//                             Sample noise at the particle, quantize the heading to
//                             `facets` directions, step forward, wrap/respawn.
//   PASSINDEX 1 -> trailBuf : draw every active particle as a glowing dot coloured
//                             by heading, max-composited over the decaying trail.
//   PASSINDEX 2 -> image    : output the trail buffer.
// Particles live in the aspect-correct centred space  uv = (frag*2 - R)/R.y.
// ─────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define ASP  (RENDERSIZE.x / RENDERSIZE.y)
#define PI   3.1415926535
#define COUNT 200

// ---- hashing / noise (folded in from the original 'common' tab) ----
vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy) * 2.0 - 1.0;
}

float hash31(vec3 p3) {
    p3 = fract(p3 * 0.1031);
    p3 += dot(p3, p3.zyx + 31.32);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec3 x) {
    vec3 i = floor(x);
    vec3 f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(hash31(i + vec3(0,0,0)), hash31(i + vec3(1,0,0)), f.x),
                   mix(hash31(i + vec3(0,1,0)), hash31(i + vec3(1,1,0)), f.x), f.y),
               mix(mix(hash31(i + vec3(0,0,1)), hash31(i + vec3(1,0,1)), f.x),
                   mix(hash31(i + vec3(0,1,1)), hash31(i + vec3(1,1,1)), f.x), f.y), f.z);
}

// glowing point kernel — the 1/dist falloff that gives sharp luminous cores
float drawPoint(vec2 uv, vec2 p, float g, float sharp) {
    return pow((0.0065 * g) / max(length(uv - p), 1e-4), sharp);
}

// reacts: movement, flow, energy, grain, palette, build-up, texture
// emphasis: flow
void main() {
    // --- Audio Feature Bus coupling. Living baseline: the manual INPUTS are
    // the rest state; the bus modulates around them, scaled by audioReact. ---
    float amt      = audioReact;
    float flowMul  = 1.0 + amt * (0.4*audioArousal + 0.8*audioFlux + 0.6*length(audioFlow)); // movement+flow
    float glowMul  = 1.0 + amt * (1.2*audioPunch + 0.6*audioBreath());                       // grain+energy
    float trailAdd = amt * 0.025 * audioEnergy;                                              // build-up lengthens trails
    float facetsA  = facets + amt * 8.0 * audioTension;                                      // tension -> more crystalline channels
    float sharpA   = sharpness + amt * 0.5 * (audioTexture - 0.5);                            // crispy(+) / smooth(-) point cores

    // ───────── PASS 0 — particle simulation (simBuf) ─────────
    if (PASSINDEX == 0) {
        vec4 s = texture2D(simBuf, gl_FragCoord.xy / R);

        // seed: random position in centred aspect space, marked not-yet-alive
        if (FRAMEINDEX < 1) {
            vec2 q = hash22(gl_FragCoord.xy);
            q.x *= ASP;
            gl_FragColor = vec4(q, 0.0, 0.0);
            return;
        }

        vec2 p = s.rg;

        // sample the field, quantize the heading into `facets` crystalline directions
        float n   = vnoise(vec3(p * noiseScale, TIME * simSpeed));
        n         = floor(n * facets) / facets;
        float ang = n * PI * 2.0;

        // march along the quantized heading
        n         = floor(n * facetsA) / max(facetsA, 1.0);   // audio-modulated facets
        ang       = n * PI * 2.0;
        vec2 vel = vec2(cos(ang), sin(ang)) * (0.008 * flowSpeed * flowMul);
        p += vel;

        vec4 outS = vec4(p, ang, 1.0);   // .a = 1.0 means "alive, draw me"

        // wrap at the edges by respawning; not drawn on the respawn frame (.a = 0)
        if (abs(p.x) > ASP || abs(p.y) > 1.0) {
            vec2 q = hash22(gl_FragCoord.xy + floor(TIME));
            q.x *= ASP;
            outS = vec4(q, 0.0, 0.0);
        }

        gl_FragColor = outS;
        return;
    }

    // ───────── PASS 1 — render dots + accumulate trail (trailBuf) ─────────
    if (PASSINDEX == 1) {
        vec2 uv = (gl_FragCoord.xy * 2.0 - R) / R.y;

        vec3 col      = vec3(0.0);
        float nActive = floor(float(COUNT) * density);

        for (int i = 0; i < COUNT; i++) {
            if (float(i) >= nActive) break;
            vec4 t = texture2D(simBuf, vec2(0.5, float(i) + 0.5) / R);
            vec2 p   = t.rg;
            float ang = t.b;
            float alive = t.a;

            // colour: heading-hue, blended toward the audio palette (synesthesia)
            float ct = 0.5 + 0.5 * sin(ang * 1.5 + TIME * hueSpeed);
            vec3 heatPal = 0.5 + 0.5 * cos(vec3(1.0, 2.0, 4.0) + ang * 1.5 + TIME * hueSpeed);
            vec3 pal = mix(heatPal, audioPalette(ct), amt * 0.85);
            pal += audioPalAccent * audioHit() * amt * 0.6;     // onset sparkle (grain)
            pal = mix(vec3(dot(pal, vec3(0.3333))), pal, saturation);

            col = mix(col, pal, drawPoint(uv, p, glow * glowMul, sharpA) * alive);
        }

        // long-exposure: build-ups lengthen the trails; drops flash the field
        vec3 prev = texture(trailBuf, gl_FragCoord.xy / R).rgb;
        col = max(col, prev * min(trail + trailAdd, 0.995));
        col *= 1.0 + amt * audioDrop * 0.8;

        gl_FragColor = vec4(col, 1.0);
        return;
    }

    // ───────── PASS 2 — image ─────────
    gl_FragColor = vec4(texture(trailBuf, gl_FragCoord.xy / R).rgb, 1.0);
}
