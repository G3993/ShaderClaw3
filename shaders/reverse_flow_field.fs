/*{
    "DESCRIPTION": "Starling Murmuration — flock of bird silhouettes wheeling against a dusk sky",
    "CATEGORIES": ["Generator", "Particles"],
    "CREDIT": "ShaderClaw / Starling Murmuration v1",
    "INPUTS": [
        { "NAME": "birdSize",        "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,  "MAX": 3.0,  "LABEL": "Bird Size" },
        { "NAME": "birdStretch",     "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.2,  "MAX": 4.0,  "LABEL": "Streak" },
        { "NAME": "flockSpeed",      "TYPE": "float", "DEFAULT": 0.40, "MIN": 0.05, "MAX": 1.50, "LABEL": "Flock Speed" },
        { "NAME": "cohesionAmt",     "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.05, "MAX": 0.80, "LABEL": "Cohesion" },
        { "NAME": "hdrPeak",         "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 5.0,  "LABEL": "Sky HDR" },
        { "NAME": "pulse",           "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Bass Pulse" },
        { "NAME": "audioReactivity", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio" }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float bounce01(float x) { return abs(fract(x * 0.5) * 2.0 - 1.0); }

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = audioLevel + audioBass * pulse * audioReactivity;

    // Dusk sky: amber horizon → purple mid → deep violet zenith
    float h = uv.y * 0.5 + 0.5;
    vec3 skyTop    = vec3(0.04, 0.02, 0.18);
    vec3 skyPurple = vec3(0.28, 0.06, 0.48) * hdrPeak * 0.55;
    vec3 skyAmber  = vec3(1.00, 0.45, 0.08) * hdrPeak * 0.90;
    vec3 sky = mix(skyAmber,
                   mix(skyPurple, skyTop, smoothstep(0.25, 0.80, h)),
                   smoothstep(0.0, 0.25, h));

    // Sun near horizon
    vec2 sunPos  = vec2(0.28 * aspect, -0.12);
    float sunD   = length(uv - sunPos);
    sky += vec3(1.0, 0.90, 0.55) * hdrPeak * exp(-sunD * sunD * 10.0) * 0.7;
    sky += skyAmber * exp(-sunD * 3.5) * 0.4;

    // Flock center drifts slowly across frame
    float fcx = sin(TIME * 0.22) * 0.55 * aspect;
    float fcy = cos(TIME * 0.17) * 0.28;

    // Cohesion oscillates — flock compresses and expands like a real murmuration
    float coh = cohesionAmt
              * (0.80 + 0.20 * sin(TIME * 0.43))
              * (1.0 + audioBass * pulse * 0.40);

    // Accumulate bird silhouettes (black capsule streaks)
    float birdDark = 0.0;
    for (int i = 0; i < 400; i++) {
        float fi = float(i);
        float s1 = hash11(fi * 1.37);
        float s2 = hash11(fi * 2.91 + 0.5);
        float s3 = hash11(fi * 4.17 + 0.3);
        float s4 = hash11(fi * 7.53 + 0.7);

        float dt  = 0.018;
        float bSx = (0.15 + s1 * 0.55) * flockSpeed;
        float bSy = (0.15 + s2 * 0.55) * flockSpeed;

        float bxA = fcx + (bounce01(TIME       * bSx + s3 * 6.28) * 2.0 - 1.0) * coh;
        float byA = fcy + (bounce01(TIME       * bSy + s4 * 6.28) * 2.0 - 1.0) * coh * 0.65;
        float bxB = fcx + (bounce01((TIME+dt)  * bSx + s3 * 6.28) * 2.0 - 1.0) * coh;
        float byB = fcy + (bounce01((TIME+dt)  * bSy + s4 * 6.28) * 2.0 - 1.0) * coh * 0.65;

        vec2 posA = vec2(bxA, byA);
        vec2 posB = vec2(bxB, byB);
        vec2 vel  = posB - posA;
        float spd = max(length(vel), 1e-5);

        // Oriented capsule — direction-aligned streak per bird
        float stretchLen = 0.012 * birdStretch * (0.5 + audio * audioReactivity * 0.4);
        vec2 vn = vel / spd;
        vec2 a  = posA - vn * stretchLen;
        vec2 b  = posA + vn * stretchLen;

        vec2 pa = uv - a;
        vec2 ba = b - a;
        float hh = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
        float d  = length(pa - ba * hh);

        float r = 0.0045 * birdSize;
        birdDark += smoothstep(r, 0.0, d);
    }

    // Black silhouettes over sky
    vec3 col = mix(sky, vec3(0.0), min(birdDark, 1.0));

    gl_FragColor = vec4(col, 1.0);
}
