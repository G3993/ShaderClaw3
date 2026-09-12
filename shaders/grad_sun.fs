/*{
  "DESCRIPTION": "Gradient Sun — a grainy sunburst fan: hot orange light-rays converging to a point above the frame, sweeping down through a pink dawn band into teal sky, over a dark mountain rising from below — the reference's airbrushed ray poster, alive. Rays sway and shimmer, the dawn band breathes, the mountain exhales haze. Bass swells the ray glow, mids sway the fan, beats flare a ray, highs sparkle the grain.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "rayCount",    "LABEL": "Ray Count",     "TYPE": "float", "MIN": 4.0, "MAX": 16.0, "DEFAULT": 8.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "rayWidth",    "LABEL": "Ray Width",     "TYPE": "float", "MIN": 0.2, "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Shape / Geometry" },
    { "NAME": "hillAmt",     "LABEL": "Mountain",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.8,  "GROUP": "Shape / Geometry" },
    { "NAME": "swaySpeed",   "LABEL": "Fan Sway",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Motion / Animation" },
    { "NAME": "motionSpeed", "LABEL": "Motion Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "grainAmt",    "LABEL": "Film Grain",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Color" },
    { "NAME": "brightness",  "LABEL": "Brightness",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",    "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.08, "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",    "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "Depth / Passes" },
    { "NAME": "aberration",  "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// GRAD SUN — after the teal/orange grainy sunburst reference.
//   A fan of crisp orange rays radiates from a vanishing point just
//   above top-center. The sky behind is a layered airbrush: magenta
//   dawn at the very top, wide teal field, then a warm orange/cream
//   glow band where the rays land, falling into deep navy where a dark
//   mountain rises bottom-center. Rays are drawn in ANGLE space around
//   the vanishing point with pixel-tight cores + soft halos; each ray
//   sways individually and one flares on the beat. Heavy film grain.
// ─────────────────────────────────────────────────────────────────────────

#define R RENDERSIZE.xy
#define TAU 6.2831853

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}
float hash11(float n) { return fract(sin(n) * 43758.5453123); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x), f.y);
}

vec3 hueRotate(vec3 c, float a) {
    float an = a * TAU;
    vec3 k = vec3(0.57735);
    return c * cos(an) + cross(k, c) * sin(an) + k * dot(k, c) * (1.0 - cos(an));
}

vec3 renderScene() {
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 q = (fc - 0.5 * R) / R.y;

    float amt   = audioReact;
    float bassP = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    float midP  = pow(smoothstep(0.06, 0.85, audioMid), 1.2);
    float highP = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float beatP = clamp(audioBeatPulse, 0.0, 1.0);
    float t = TIME * motionSpeed;

    // ── layered airbrush sky ────────────────────────────────────────────
    vec3 cMag  = vec3(0.94, 0.22, 0.55);   // dawn magenta
    vec3 cTeal = vec3(0.14, 0.75, 0.82);   // big teal field
    vec3 cGlow = vec3(1.00, 0.62, 0.20);   // landing glow band
    vec3 cCream= vec3(1.00, 0.90, 0.75);
    vec3 cNavy = vec3(0.045, 0.06, 0.16);  // deep bottom

    float y = uv.y;
    float breathe = 1.0 + 0.03 * sin(t * 0.4) + amt * 0.05 * bassP;
    vec3 sky = cTeal;
    sky = mix(sky, cMag, smoothstep(0.80, 0.99, y));
    // the glow band sits ~1/3 up, breathing
    float band = exp(-pow((y - 0.38 * breathe) * 5.5, 2.0));
    sky = mix(sky, cGlow, band * 0.85);
    sky = mix(sky, cCream, exp(-pow((y - 0.40 * breathe) * 9.0, 2.0)) * 0.45);
    sky = mix(sky, cMag, exp(-pow((y - 0.52) * 8.0, 2.0)) * 0.35);
    sky = mix(cNavy, sky, smoothstep(0.06, 0.34, y));

    // ── dark mountain rising bottom-center ──────────────────────────────
    float mx = q.x;
    float ridge = 0.34 * hillAmt - mx * mx * 0.55
                + 0.015 * vnoise(vec2(mx * 3.0, 1.0)) * hillAmt;
    float mMask = smoothstep(ridge + 0.004, ridge - 0.004, uv.y - 0.0);
    // haze exhaling off the ridge line
    float haze = exp(-abs(uv.y - ridge) * 22.0) * (0.4 + 0.2 * sin(t * 0.3));
    sky = mix(sky, cNavy * 0.9, mMask);
    sky += cGlow * haze * 0.15 * hillAmt;

    // ── the ray fan from a vanishing point above top-center ─────────────
    vec2 vp = vec2(0.0, 0.5 * R.y / R.y + 0.12);      // just above frame
    vec2 d = q - vp;
    float ang = atan(d.x, -d.y);                       // 0 = straight down
    float dist = length(d);

    float N = floor(rayCount + 0.5);
    float spread = 1.25;                               // fan half-angle
    vec3 rays = vec3(0.0);
    for (int i = 0; i < 16; i++) {
        if (float(i) >= N) break;
        float fi = float(i);
        float hr = hash11(fi * 9.7);
        float a0 = (fi / (N - 1.0) - 0.5) * 2.0 * spread;
        // individual sway (mid pushes the sway phase)
        a0 += 0.05 * swaySpeed * sin(t * (0.3 + 0.25 * hr) + hr * TAU + amt * 0.6 * midP);
        float da = ang - a0;
        // angular width shrinks with distance-from-vp for perspective
        float w = 0.007 * rayWidth * (1.0 + 1.3 * dist);
        float core = exp(-pow(da / (w * 0.4), 2.0));
        float halo = exp(-pow(da / (w * 2.2), 2.0));
        // beat flare travels smoothly around the fan (no snapping)
        float flare = pow(0.5 + 0.5 * cos(fi * 2.4 - t * 1.6), 6.0) * amt * beatP;
        float gain = 0.85 + 0.5 * flare + amt * (0.2 * bassP + 0.3 * clamp(audioLevel, 0.0, 1.0));
        // rays fade as they approach the vp (top) and strengthen mid-frame
        float reach = smoothstep(0.05, 0.4, dist) * (1.0 - smoothstep(1.2, 1.8, dist));
        rays += (cGlow * halo * 0.45 + mix(cGlow, cCream, 0.15) * core * 0.6) * gain * reach;
    }
    // rays dim where the mountain blocks them, but rim-light the ridge
    rays *= 1.0 - mMask * 0.85;
    vec3 col = sky + rays * 0.8;

    // vanishing-point warmth (kept below blowout)
    col += cGlow * exp(-dist * dist * 2.2) * 0.10;

    // ── film grain (the reference's texture) + high sparkle ─────────────
    float g1 = hash21(fc * 0.9 + floor(fract(t) * 24.0) * vec2(31.0, 17.0)) - 0.5;
    col += g1 * 0.05 * grainAmt * (1.0 + amt * 0.5 * highP);

    col = hueRotate(col, hueShift);
    return col;
}

void main() {
    vec2 uv = gl_FragCoord.xy / R;
    if (PASSINDEX == 0) {
        vec3 col = renderScene();
        vec3 prev = texture2D(abTrail, uv).rgb;
        float decay = 0.50 + 0.46 * trailAmt;
        col = max(col, prev * decay - 0.0045);
        col += (hash21(gl_FragCoord.xy + fract(TIME) * 61.0) - 0.5) * 0.006;
        gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
    } else {
        vec2 dir = uv - 0.5;
        float ab = aberration * 0.0045;
        vec3 base;
        base.r = texture2D(abTrail, uv + dir * ab).r;
        base.g = texture2D(abTrail, uv).g;
        base.b = texture2D(abTrail, uv - dir * ab).b;
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float an = float(i) * 0.7853982;
            vec2 o = vec2(cos(an), sin(an)) * (3.5 / R.y);
            bl += texture2D(abTrail, uv + o).rgb;
            bl += texture2D(abTrail, uv + o * 2.6).rgb * 0.6;
        }
        bl /= 12.8;
        bl = max(bl - 0.52, 0.0);
        vec3 col = base + bl * bl * bloomAmt * 1.8;
        float lvl = clamp(audioLevel, 0.0, 1.0);
        col *= brightness * mix(1.0, 0.64 + 0.36 * lvl, audioReact)
             * (1.0 + audioReact * 0.06 * clamp(audioBeatPulse, 0.0, 1.0));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
        float lumHD = dot(col, vec3(0.299, 0.587, 0.114));
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        gl_FragColor = vec4(col, 1.0);
    }
}
