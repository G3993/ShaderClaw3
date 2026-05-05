/*{
  "DESCRIPTION": "Quartzite Grotto — 3D raymarched rose quartz crystal cluster with amber glow",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",    "LABEL": "Sway Speed",  "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",  "LABEL": "Brightness",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 6.0 },
    { "NAME": "fov",      "LABEL": "FOV",         "TYPE": "float", "DEFAULT": 1.6,  "MIN": 0.8, "MAX": 3.0 },
    { "NAME": "specPow",  "LABEL": "Shimmer",     "TYPE": "float", "DEFAULT": 48.0, "MIN": 4.0, "MAX": 128.0 }
  ]
}*/

#define ROSE    vec3(0.85, 0.35, 0.55)
#define AMBER   vec3(1.0,  0.60, 0.12)
#define BASALT  vec3(0.06, 0.04, 0.06)
#define WHTHT   vec3(1.4,  1.2,  1.5)
#define VOID    vec3(0.0,  0.0,  0.012)

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    return length(pa - ba * clamp(dot(pa,ba)/dot(ba,ba), 0.0, 1.0)) - r;
}

// Returns vec2(dist, matID): 1=crystal, 2=floor
vec2 scene(vec3 p) {
    // Floor plane
    float d = p.y + 0.82;
    float mat = 2.0;

    // Six rose quartz spires (deterministic positions)
    float c;
    c = sdCapsule(p, vec3(-0.30,-0.82, 0.50), vec3(-0.24, 0.42, 0.46), 0.08);
    if (c < d) { d = c; mat = 1.0; }
    c = sdCapsule(p, vec3( 0.40,-0.82, 0.22), vec3( 0.37, 0.68, 0.18), 0.10);
    if (c < d) { d = c; mat = 1.0; }
    c = sdCapsule(p, vec3(-0.50,-0.82,-0.30), vec3(-0.47, 0.30,-0.28), 0.07);
    if (c < d) { d = c; mat = 1.0; }
    c = sdCapsule(p, vec3( 0.16,-0.82, 0.72), vec3( 0.13, 0.52, 0.69), 0.09);
    if (c < d) { d = c; mat = 1.0; }
    c = sdCapsule(p, vec3(-0.10,-0.82,-0.62), vec3(-0.08, 0.40,-0.61), 0.07);
    if (c < d) { d = c; mat = 1.0; }
    c = sdCapsule(p, vec3(-0.42,-0.82, 0.12), vec3(-0.44, 0.78, 0.10), 0.11);
    if (c < d) { d = c; mat = 1.0; }

    return vec2(d, mat);
}

vec3 calcNormal(vec3 p) {
    float e = 0.0005;
    return normalize(vec3(
        scene(p+vec3(e,0,0)).x - scene(p-vec3(e,0,0)).x,
        scene(p+vec3(0,e,0)).x - scene(p-vec3(0,e,0)).x,
        scene(p+vec3(0,0,e)).x - scene(p-vec3(0,0,e)).x
    ));
}

vec4 renderGrotto(vec2 uv) {
    float t = TIME * speed;

    // Camera: slight sway with audioBass + slow pendulum
    float sway = sin(t * 0.6) * 0.08 + audioBass * 0.05;
    vec3 ro   = vec3(sway, 0.12, -1.75);
    vec3 ta   = vec3(0.0,  0.18,  0.5);
    vec3 fwd  = normalize(ta - ro);
    vec3 rgt  = normalize(cross(fwd, vec3(0,1,0)));
    vec3 up   = cross(rgt, fwd);
    vec3 rd   = normalize(uv.x * rgt + uv.y * up + fov * fwd);

    // Warm amber light above crystal cluster; cool secondary from upper-left
    vec3 L1   = normalize(vec3(0.1, 1.8, 0.6));
    vec3 L2   = normalize(vec3(-1.2, 1.0, -0.3));

    vec3 col  = VOID;
    float dist = 0.0;
    vec2  hit  = vec2(-1.0, 0.0);

    for (int i = 0; i < 80; i++) {
        vec3  p = ro + rd * dist;
        vec2  s = scene(p);
        if (s.x < 0.0006) { hit = vec2(dist, s.y); break; }
        if (dist > 8.0)    break;
        dist += s.x;
    }

    if (hit.x > 0.0) {
        vec3 p = ro + rd * hit.x;
        vec3 n = calcNormal(p);
        int  m = int(hit.y + 0.5);

        float d1   = clamp(dot(n, L1), 0.0, 1.0);
        float d2   = clamp(dot(n, L2), 0.0, 1.0);
        float spec = pow(clamp(dot(reflect(-L1, n), -rd), 0.0, 1.0), specPow);
        float rim  = pow(1.0 - abs(dot(-rd, n)), 3.0);

        if (m == 1) {
            // Rose quartz crystal
            float audioShim = 1.0 + audioHigh * 0.6;
            vec3 base = ROSE * (d1 * 0.65 + d2 * 0.2 + 0.15) * hdrPeak * 1.7;
            vec3 sp   = WHTHT * spec * hdrPeak * 3.5 * audioShim;
            vec3 rl   = ROSE  * rim  * hdrPeak * 2.2;
            // Amber subsurface warmth from base
            float floorDist = p.y + 0.82;
            vec3 sss  = AMBER * hdrPeak * 0.9 * exp(-floorDist * 2.5);
            col = base + sp + rl + sss;
        } else {
            // Basalt floor
            col = BASALT * (d1 * 0.5 + d2 * 0.15 + 0.1) * hdrPeak * 0.7;
            // Amber reflected glow pooling on floor
            col += AMBER * hdrPeak * 0.35 * exp(-dot(p.xz, p.xz) * 1.4);
        }

        // Distance fog to cave atmosphere
        float fog = 1.0 - exp(-hit.x * 0.22);
        col = mix(col, VOID, fog * 0.5);
    } else {
        // Cave far background — deep darkness with faint amber haze
        col = VOID + AMBER * 0.012 * hdrPeak;
    }

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);
    vec4 col = renderGrotto(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band       = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise  = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec4 cR = renderGrotto(uv + vec2(shift + chromaAmt, 0.0));
        vec4 cG = renderGrotto(uv + vec2(shift, chromaAmt * 0.5));
        vec4 cB = renderGrotto(uv + vec2(shift - chromaAmt, 0.0));
        vec4 glitched = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        float scanline   = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        float blockX     = floor(uv.x * 6.0);
        float blockY     = floor(uv.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(t * 8.0)) * 43758.5);
        float dropout    = step(1.0 - g * 0.15, blockNoise);
        glitched.rgb *= scanline;
        glitched.rgb *= 1.0 - dropout;
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = col;
}
