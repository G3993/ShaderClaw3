/*{
  "DESCRIPTION": "Element Water — a real fluid: a self-advecting velocity field (the CFD Paint solver) carries a water surface with height, foam and depth. Swells are pushed up by wandering currents and ride the flow, foam is born where the water shears and drifts off with it, caustics are the real focusing of the surface, and the body is lit like water: fresnel sky reflection over deep teal, sun glints on the facets, light shafts below. Bass drives the currents, mids widen them, beats fizz the foam, highs glint the crests.",
  "CATEGORIES": ["Generator", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "causticAmt",   "LABEL": "Caustics",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7,  "GROUP": "Shape / Geometry" },
    { "NAME": "rayAmt",       "LABEL": "Light Shafts",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Shape / Geometry" },
    { "NAME": "foamAmt",      "LABEL": "Foam",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6,  "GROUP": "Shape / Geometry" },
    { "NAME": "swirlAmt",     "LABEL": "Currents",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6,  "GROUP": "Shape / Geometry" },
    { "NAME": "waveHeight",   "LABEL": "Wave Height",   "TYPE": "float", "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.2,  "GROUP": "Shape / Geometry" },
    { "NAME": "viscosity",    "LABEL": "Viscosity",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Shape / Geometry" },
    { "NAME": "depthHue",     "LABEL": "Water Hue",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "brightness",   "LABEL": "Brightness",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "motionSpeed",  "LABEL": "Motion Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",     "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2,  "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",     "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Depth / Passes" },
    { "NAME": "aberration",   "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "velBuf",  "PERSISTENT": true },
    { "TARGET": "wtrBuf",  "PERSISTENT": true },
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// ELEMENT WATER — rebuilt on the CFD Paint solver (Lu 2026-10-14: "feels
// cheap, not like real water; look at CFD Paint").
//   Pass 0  velBuf : rotational self-advecting velocity (5-octave curl
//                    stencil, 3 reads per octave — the CFD Paint method),
//                    wandering current splats, viscosity, a slow drift.
//   Pass 1  wtrBuf : the water itself, advected by the velocity —
//                    r = surface height (0.5 = flat), g = foam, b = depth
//                    tracer (shallow / churned water). Swells are pumped by
//                    the currents and relax back; foam is born from shear
//                    and dies off; the tracer lightens churned water.
//   Pass 2  abTrail: shading — normal from the height gradient, caustics
//                    from its curvature (real light focusing), fresnel sky
//                    reflection over the deep body, sun glints, foam,
//                    light shafts, then the luminous trail buffer.
//   Pass 3  present: bloom + lens depth + finish.
// Audio injects force into the sim (bass), widens the currents (mids),
// fizzes foam (beats), glints the crests (highs) — the fluid does the rest.
// ─────────────────────────────────────────────────────────────────────────
#define R RENDERSIZE.xy
#define TAU 6.2831853
#define ANG 2.0943951

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) { v += a * vnoise(p); p = p * 2.07 + vec2(7.3, 3.1); a *= 0.52; }
    return v;
}
vec3 hueRotate(vec3 c, float a) {
    float cs = cos(a), sn = sin(a);
    mat3 m = mat3(
        0.299 + 0.701*cs + 0.168*sn, 0.587 - 0.587*cs + 0.330*sn, 0.114 - 0.114*cs - 0.497*sn,
        0.299 - 0.299*cs - 0.328*sn, 0.587 + 0.413*cs + 0.035*sn, 0.114 - 0.114*cs + 0.292*sn,
        0.299 - 0.300*cs + 1.250*sn, 0.587 - 0.588*cs - 1.050*sn, 0.114 + 0.886*cs - 0.203*sn);
    return clamp(c * m, 0.0, 2.0);
}

// Three wandering currents: where the water is pushed. Shared by the
// velocity and water passes so force and swell land in the same place.
vec2 currentPos(int i, float t) {
    float fi = float(i);
    float ph = t * (0.22 + fi * 0.11) + fi * 2.1;
    return vec2(0.5 + 0.36 * sin(ph) * cos(ph * 0.63 + fi),
                0.5 + 0.30 * cos(ph * 0.77) * sin(ph * 0.41 + fi * 1.7));
}
vec2 currentDir(int i, float t) {
    float fi = float(i);
    float ph = t * (0.22 + fi * 0.11) + fi * 2.1;
    return vec2(cos(ph * 1.15 + fi * 0.9), sin(ph * 0.85 + fi * 2.3));
}

void main() {
    vec2 pos = gl_FragCoord.xy;
    vec2 uv  = pos / R;
    float aspect = R.x / R.y;
    vec2 invRes = 1.0 / R;
    float amt = audioReact;
    float bassP = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    float midP  = pow(smoothstep(0.06, 0.85, audioMid),  1.2);
    float highP = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float levelP = clamp(audioLevel, 0.0, 1.0);
    float beatP  = clamp(audioBeatPulse, 0.0, 1.0);
    float t = TIME * motionSpeed * 0.5;
    float force = swirlAmt * (0.9 + amt * (0.9 * bassP + 0.4 * levelP));   // bass drives the currents
    float splatR = 0.11 * (1.0 + 0.25 * amt * midP);                       // mids widen them
    float splatR2 = splatR * splatR;

    // ═══════════ PASS 0 — velocity (CFD Paint rotational curl solver) ═══
    if (PASSINDEX == 0) {
        float ca = cos(ANG), sa = sin(ANG);
        float cah = cos(ANG * 0.5), sah = sin(ANG * 0.5);
        float rnd = hash21(vec2(float(FRAMEINDEX), 0.37));
        vec2 b = vec2(cos(ANG * rnd), sin(ANG * rnd));
        vec2 v = vec2(0.0);
        float maxR2 = min(R.y * R.y * 0.25, 4000.0);
        for (int level = 0; level < 5; level++) {
            float bb = dot(b, b);
            if (bb > maxR2) break;
            float invBB = 1.0 / bb;
            vec2 d0 = b;
            vec2 d1 = vec2(ca * b.x - sa * b.y, sa * b.x + ca * b.y);
            vec2 d2 = vec2(ca * b.x + sa * b.y, -sa * b.x + ca * b.y);
            vec2 h0 = vec2(cah * b.x - sah * b.y, sah * b.x + cah * b.y);
            vec2 h1 = vec2(ca * h0.x - sa * h0.y, sa * h0.x + ca * h0.y);
            vec2 h2 = vec2(ca * h0.x + sa * h0.y, -sa * h0.x + ca * h0.y);
            vec2 s0 = texture2D(velBuf, fract((pos + d0 + d0) * invRes)).xy - 0.5;
            vec2 s1 = texture2D(velBuf, fract((pos + d1 + d1) * invRes)).xy - 0.5;
            vec2 s2 = texture2D(velBuf, fract((pos + d2 + d2) * invRes)).xy - 0.5;
            v += d0.yx * dot(s0, h0) * invBB * 3.0;
            v += d1.yx * dot(s1, h1) * invBB * 3.0;
            v += d2.yx * dot(s2, h2) * invBB * 3.0;
            b *= 2.0;
        }
        float speed = 1.4;
        vec2 advUV = fract((pos + v * vec2(-1.0, 1.0) * speed) * invRes);
        vec4 vel = texture2D(velBuf, advUV);
        // viscosity: water is thin — a little damping keeps it from boiling
        vel.xy = mix(vel.xy, vec2(0.5), 0.004 + 0.02 * viscosity);
        // the currents
        for (int s = 0; s < 3; s++) {
            vec2 sp = currentPos(s, t);
            vec2 d = uv - sp; d.x *= aspect;
            float dist2 = dot(d, d);
            if (dist2 < splatR2 * 12.0) {
                vec2 dir = currentDir(s, t);
                // a swirl on top of the push so every current rolls
                vec2 tang = vec2(-d.y, d.x) * 2.5;
                vel.xy += (dir + tang * (0.6 + 0.4 * sin(t * 0.9 + float(s)))) * force * 0.045 * exp(-dist2 / splatR2);
            }
        }
        // a slow lateral drift so the whole sea travels
        vel.x += 0.0015 * swirlAmt * (0.6 + 0.4 * sin(t * 0.13));
        vel.xy = clamp(vel.xy, 0.0, 1.0);
        if (FRAMEINDEX < 2) vel = vec4(0.5, 0.5, 0.0, 1.0);
        vel.a = 1.0;
        gl_FragColor = vel;
        return;
    }

    // ═══════════ PASS 1 — the water: height, foam, depth tracer ═════════
    if (PASSINDEX == 1) {
        vec2 vel = (texture2D(velBuf, uv).xy - 0.5) * 2.0;
        vec2 advUV = fract(uv - vel * 0.016 * (1.0 + 0.5 * amt * bassP));
        vec4 w = texture2D(wtrBuf, advUV);
        float h = w.r, foam = w.g, tr = w.b;
        // the surface relaxes toward flat, swells keep their shape as they ride
        h = mix(h, 0.5, 0.012);
        // currents pump the surface: a rolling swell, up on the leading side,
        // down behind, so each current drags a wave train with it
        for (int s = 0; s < 3; s++) {
            vec2 sp = currentPos(s, t);
            vec2 d = uv - sp; d.x *= aspect;
            float dist2 = dot(d, d);
            if (dist2 < splatR2 * 14.0) {
                vec2 dir = currentDir(s, t);
                float lobe = dot(normalize(d + 1e-4), dir);
                float g = exp(-dist2 / (splatR2 * 1.6));
                h += lobe * g * 0.035 * waveHeight * force;
            }
        }
        // shear of the flow births foam, which then drifts with the water
        float dlt = 2.0 * invRes.y;
        vec2 vL = texture2D(velBuf, uv + vec2(-dlt, 0.0)).xy, vR = texture2D(velBuf, uv + vec2(dlt, 0.0)).xy;
        vec2 vU = texture2D(velBuf, uv + vec2(0.0, dlt)).xy,  vD = texture2D(velBuf, uv + vec2(0.0, -dlt)).xy;
        float shear = (length(vR - vL) + length(vU - vD)) * 9.0;
        float crest = smoothstep(0.56, 0.72, h);
        foam = foam * (0.955 - 0.02 * viscosity)
             + smoothstep(0.12, 0.6, shear) * (0.035 + 0.05 * crest) * foamAmt
             + crest * 0.012 * foamAmt
             + amt * 0.03 * beatP * smoothstep(0.08, 0.4, shear) * foamAmt;   // beats fizz the churn
        // churned water is lighter (aerated) — the tracer follows the currents
        tr = tr * 0.985 + smoothstep(0.1, 0.5, shear) * 0.02 + crest * 0.01;
        h = clamp(h, 0.0, 1.0); foam = clamp(foam, 0.0, 1.0); tr = clamp(tr, 0.0, 1.0);
        if (FRAMEINDEX < 2) { h = 0.5 + 0.02 * (fbm(uv * 6.0) - 0.5); foam = 0.0; tr = 0.0; }
        gl_FragColor = vec4(h, foam, tr, 1.0);
        return;
    }

    // ═══════════ PASS 2 — shading into the trail buffer ═════════════════
    if (PASSINDEX == 2) {
        vec2 q = (pos - 0.5 * R) / R.y;
        float delta = 2.5 * invRes.y;
        float hC = texture2D(wtrBuf, uv).r;
        float hL = texture2D(wtrBuf, uv + vec2(-delta, 0.0)).r;
        float hR = texture2D(wtrBuf, uv + vec2( delta, 0.0)).r;
        float hU = texture2D(wtrBuf, uv + vec2(0.0,  delta)).r;
        float hD = texture2D(wtrBuf, uv + vec2(0.0, -delta)).r;
        vec4 w = texture2D(wtrBuf, uv);
        float foam = w.g, tr = w.b;
        float hs = 26.0 * waveHeight;
        vec3 n = normalize(vec3((hL - hR) * hs, (hD - hU) * hs, 1.0));
        // caustics = the surface focusing light: concave spots (negative
        // laplacian) converge rays onto the floor; sharpened into webs
        float lap = (hL + hR + hU + hD - 4.0 * hC) * hs * 1.6;
        float caust = pow(clamp(-lap * 2.2 + 0.15, 0.0, 1.0), 2.2);
        // fine ripple caustic web riding on the sim (keeps the floor alive between swells)
        float web = pow(1.0 - min(0.5 + 0.5 * sin((uv.x * aspect + 0.3 * vnoise(uv * 5.0 + t * 0.2)) * 28.0 + t),
                                  0.5 + 0.5 * sin((uv.y + 0.3 * vnoise(uv * 5.0 + 9.0 - t * 0.17)) * 31.0 - t * 0.8)), 9.0);
        // ── the water body: deep navy, churn lightens it toward teal ─────
        float depth = smoothstep(0.6, -0.6, q.y);                     // 0 top → 1 bottom
        vec3 shallow = vec3(0.10, 0.56, 0.60);
        vec3 midw    = vec3(0.03, 0.27, 0.44);
        vec3 abyss   = vec3(0.005, 0.035, 0.12);
        vec3 body = mix(midw, abyss, smoothstep(0.1, 1.0, depth));
        body = mix(body, shallow, tr * 0.8 + smoothstep(0.5, 0.75, hC) * 0.35);
        // refracted floor light: caustics brighter where the body is shallow
        vec3 causCol = vec3(0.55, 0.92, 0.95);
        body += causCol * (caust * 1.25 + web * 0.14) * causticAmt * mix(1.1, 0.35, depth) * (1.0 + amt * 0.5 * midP);
        // ── surface: fresnel sky over the body, sun glints on the facets ─
        vec3 view = normalize(vec3(-q * 0.35, 1.0));
        float fres = pow(1.0 - clamp(dot(n, view), 0.0, 1.0), 3.0);
        vec3 sky = mix(vec3(0.40, 0.70, 0.78), vec3(0.86, 0.95, 0.97), clamp(n.y * 0.5 + 0.5 + 0.3, 0.0, 1.0));
        vec3 col = mix(body, sky, fres * 0.55 + 0.05);
        vec3 sunDir = normalize(vec3(0.45, 0.75, 0.55));
        float spec = pow(clamp(dot(reflect(-sunDir, n), view), 0.0, 1.0), 180.0);
        float spec2 = pow(clamp(dot(reflect(-sunDir, n), view), 0.0, 1.0), 24.0);
        col += vec3(1.0, 0.98, 0.92) * (spec * 1.6 + spec2 * 0.18) * (1.0 + amt * (0.8 * highP + 0.4 * levelP));
        float diff = clamp(dot(n, sunDir), 0.0, 1.0);
        col *= 0.78 + 0.32 * diff;
        // ── light shafts from the surface ─────────────────────────────────
        vec2 sun = vec2(0.35, 0.62);
        vec2 sd = q - sun;
        float ang = atan(sd.x, -sd.y);
        float rays = pow(0.5 + 0.5 * sin(ang * 14.0 + t * 0.30), 3.0)
                   + pow(0.5 + 0.5 * sin(ang * 23.0 - t * 0.19 + 2.0), 4.0) * 0.6;
        rays *= smoothstep(1.5, 0.1, length(sd)) * smoothstep(-0.2, 0.6, q.y + 0.4);
        col += vec3(0.42, 0.78, 0.84) * rays * rayAmt * 0.16 * (1.0 + amt * 0.4 * bassP);
        col += vec3(0.80, 0.96, 0.94) * exp(-length(sd) * 3.2) * 0.35 * rayAmt;
        // ── foam: the sim's foam field, broken up with a dry lace texture ─
        float lace = fbm(uv * vec2(aspect, 1.0) * 22.0 + vec2(t * 0.3, -t * 0.2));
        float fm = smoothstep(0.08, 0.55, foam * (0.6 + 0.8 * lace));
        col = mix(col, vec3(0.86, 0.97, 0.98), clamp(fm * 1.1, 0.0, 0.9));
        // bubbles in the foam catch the sun
        float bub = smoothstep(0.75, 0.95, vnoise(uv * vec2(aspect, 1.0) * 140.0 + t * 0.5)) * fm;
        col += vec3(0.9, 1.0, 1.0) * bub * 0.6;
        // ── finish ────────────────────────────────────────────────────────
        col += (hash21(pos + fract(t) * vec2(17.0, 29.0)) - 0.5) * 0.015;
        if (depthHue > 0.001) col = hueRotate(col, depthHue * TAU);
        col *= brightness * mix(1.0, 0.82 + 0.3 * levelP + 0.2 * bassP, amt);
        // luminous trails (8-bit-safe decay floor)
        vec3 prev = texture2D(abTrail, uv).rgb;
        float decay = 0.40 + 0.50 * trailAmt;
        col = max(col, prev * decay - 0.0045);
        gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
        return;
    }

    // ═══════════ PASS 3 — present: bloom + lens depth ═══════════════════
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
    bl = max(bl - 0.55, 0.0);
    vec3 col = base + bl * bl * bloomAmt * 1.6;
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.35);
    gl_FragColor = vec4(col, 1.0);
}
