/*{
  "DESCRIPTION": "Neon Peacock Plumage — procedural peacock eye-spots and vane lines with iridescent shimmer. Deep navy, electric teal, gold-green, magenta.",
  "CREDIT": "ShaderClaw auto-improve v6",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "featherCount","LABEL": "Feather Count","TYPE": "float", "DEFAULT": 7.0, "MIN": 3.0, "MAX": 14.0 },
    { "NAME": "shimmerSpd",  "LABEL": "Shimmer Speed","TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio",        "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash11(float n) {
    return fract(sin(n * 127.1) * 43758.5453);
}

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Background: deep navy
    vec3 col = vec3(0.0, 0.02, 0.12);

    // Audio-reactive eye spot scale
    float audioScale = 1.0 + audioBass * audioReact * 0.2;

    // ── Loop over feathers (const bound 20, break on featherCount) ──────────
    for (int i = 0; i < 20; i++) {
        if (float(i) >= featherCount) break;

        float fi = float(i);

        // Feather center in UV space
        float fcx = hash11(fi * 1.31);
        float fcy = hash11(fi * 2.73);
        vec2  fc  = vec2(fcx, fcy);

        // Vane direction angle
        float th = hash11(fi * 3.17) * 6.28318;

        // Aspect-corrected vector and distance from fragment to feather center
        vec2  delta = uv - fc;
        delta.x *= aspect;
        float r = length(delta);

        // Eye spot radii
        float r_outer = 0.12 * audioScale;
        float r_mid   = 0.07;
        float r_inner = 0.03;
        float r_pupil = 0.04;
        float r_hi    = 0.012;

        // Anti-alias width
        float aa = 0.004;

        // Iridescent shimmer: phase shifts per angle and time
        float angle   = atan(delta.y, delta.x / max(aspect, 0.001));
        float shimmer = sin(angle * 3.0 + TIME * shimmerSpd + fi * 1.7) * 0.5 + 0.5;

        // Color palette
        vec3 teal      = vec3(0.0, 0.8, 0.7);
        vec3 goldGreen = vec3(0.4, 1.0, 0.0);
        vec3 deepBlue  = vec3(0.0, 0.0, 0.15);
        vec3 white_hdr = vec3(1.5, 1.5, 1.0);
        vec3 magenta   = vec3(1.0, 0.0, 0.9);

        // Outer iris ring (teal <-> magenta shimmer), annulus from r_pupil to r_outer
        float outerRing = smoothstep(r_outer + aa, r_outer - aa, r) *
                          (1.0 - smoothstep(r_pupil + aa, r_pupil - aa, r));
        vec3 iridColor = mix(teal, magenta, shimmer);
        col += iridColor * outerRing * hdrPeak * 0.7;

        // Mid ring (gold-green), annulus from r_inner to r_mid
        float midRing = smoothstep(r_mid + aa, r_mid - aa, r) *
                        (1.0 - smoothstep(r_inner + aa, r_inner - aa, r));
        col += goldGreen * midRing * hdrPeak * 0.8;

        // Pupil (deep blue/black), annulus from r_inner to r_pupil
        float pupilMask = smoothstep(r_pupil + aa, r_pupil - aa, r) *
                          (1.0 - smoothstep(r_inner + aa, r_inner - aa, r));
        col += deepBlue * pupilMask;

        // Inner highlight (white HDR), disk from 0 to r_hi
        float innerMask = smoothstep(r_inner + aa, r_inner - aa, r) *
                          (1.0 - smoothstep(r_hi + aa, r_hi - aa, r));
        col += white_hdr * innerMask * hdrPeak;

        // ── Vane lines: 16 lines radiating from center in a cone ──────────
        for (int v = 0; v < 16; v++) {
            float fv = float(v);
            // Spread within ±pi/4 cone around vane direction
            float spread = (fv / 15.0 - 0.5) * 1.5708;
            float vth    = th + spread;
            vec2  lineDir = vec2(cos(vth), sin(vth));

            // Aspect-corrected displacement
            vec2 d2 = uv - fc;
            d2.x *= aspect;

            // Projection along line direction and perpendicular distance
            float proj = d2.x * lineDir.x + d2.y * lineDir.y;
            float forward  = step(0.0, proj);
            float vaneCap  = 1.0 - smoothstep(r_outer * 0.9, r_outer * 1.5, proj);
            float perpDist2 = max(dot(d2, d2) - proj * proj, 0.0);
            float gaus = exp(-perpDist2 * 400.0);

            // Alternate teal and gold-green
            vec3 vaneCol;
            if (mod(fv, 2.0) < 1.0) {
                vaneCol = teal;
            } else {
                vaneCol = goldGreen;
            }
            // Subtle iridescent shimmer on vanes
            vaneCol = mix(vaneCol, magenta, shimmer * 0.4);

            col += vaneCol * gaus * forward * vaneCap * hdrPeak * 0.35;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
