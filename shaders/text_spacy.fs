/*{
    "DESCRIPTION": "Nebula Warp — 3D star-warp tunnel with volumetric warm-violet nebula clouds. Amber/magenta/violet palette: contrasts prior cool blue starfield. Wide deep-space environmental composition.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Volumetric"],
    "INPUTS": [
        { "NAME": "warpSpeed",  "LABEL": "Warp Speed",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "nebulaAmt",  "LABEL": "Nebula",      "TYPE": "float", "DEFAULT": 1.8,  "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "starDensity","LABEL": "Stars",        "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",   "TYPE": "float", "DEFAULT": 2.6,  "MIN": 1.0, "MAX": 4.0 },
        { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

float hash31(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 73.1) * 43758.5); }

// 3D value noise
float vnoise3(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(mix(hash31(i),              hash31(i + vec3(1,0,0)), f.x),
            mix(hash31(i + vec3(0,1,0)), hash31(i + vec3(1,1,0)), f.x), f.y),
        mix(mix(hash31(i + vec3(0,0,1)), hash31(i + vec3(1,0,1)), f.x),
            mix(hash31(i + vec3(0,1,1)), hash31(i + vec3(1,1,1)), f.x), f.y),
        f.z
    );
}

float fbm3(vec3 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise3(p);
        p = p * 2.1 + vec3(1.73, 9.31, 4.17);
        a *= 0.5;
    }
    return v;
}

// Star streak: classic warp-speed elongated star
float starStreak(vec3 p, float t, float seed) {
    float h1 = hash11(seed);
    float h2 = hash11(seed * 2.3 + 0.5);
    float h3 = hash11(seed * 4.7 + 1.0);

    vec3 dir = normalize(vec3(h1 - 0.5, h2 - 0.5, -1.0));
    float speed = warpSpeed * (0.5 + h3 * 1.0);
    float phase = fract(t * speed * 0.1 + h1);
    vec3 starBase = dir * (phase * 10.0 - 5.0);

    vec3 toStar = p - starBase;
    float projLen = dot(toStar, dir);
    float streak = clamp(-projLen / (speed * 0.3 + 0.05), 0.0, 1.0);
    vec3 closest = toStar - dir * projLen;
    float radDist = length(closest);

    return exp(-radDist * radDist * 600.0) * streak;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.3;

    // Straight-ahead camera flying through space
    vec3 ro = vec3(sin(t * 0.05) * 0.2, cos(t * 0.04) * 0.15, 0.0);
    vec3 rd = normalize(vec3(uv * 0.85, -1.0));

    // 4-color nebula palette: warm violet, hot amber, magenta, white-hot
    vec3 warmViolet = vec3(0.45, 0.05, 0.70);
    vec3 hotAmber   = vec3(1.00, 0.55, 0.05);
    vec3 magenta    = vec3(1.00, 0.05, 0.55);
    vec3 whiteHot   = vec3(1.00, 0.90, 0.80);

    // Deep space background
    vec3 col = vec3(0.005, 0.002, 0.012);

    // Nebula: volumetric march
    float dt = 0.1;
    for (int i = 0; i < 48; i++) {
        vec3 p = ro + rd * dt;
        float neb = fbm3(p * 0.4 + vec3(t * 0.04, 0.0, t * 0.03));
        neb = smoothstep(0.45, 0.75, neb);

        if (neb > 0.02) {
            float nebHue = fbm3(p * 0.3 + vec3(0.0, t * 0.02, 0.0));
            vec3 nebCol = mix(warmViolet, magenta, nebHue);
            nebCol = mix(nebCol, hotAmber, nebHue * nebHue);
            col += nebCol * neb * 0.04 * nebulaAmt * audio;
        }
        dt += 0.18;
        if (dt > 12.0) break;
    }

    // Warp stars: streak bright specks
    int nStars = int(clamp(starDensity * 40.0, 1.0, 40.0));
    for (int i = 0; i < 40; i++) {
        if (i >= nStars) break;
        float fi  = float(i);
        float streak = starStreak(ro, t, fi * 3.17);
        vec3  starCol = mix(whiteHot, hotAmber, hash11(fi * 5.37));
        col += starCol * streak * hdrBoost * audio;
    }

    // Bright star field (static)
    float sf = hash21(floor(isf_FragNormCoord * RENDERSIZE * 0.5));
    col += whiteHot * step(0.995, sf) * hdrBoost * 0.4;

    // Edge darkening (vignette pulls focus inward)
    float vign = 1.0 - dot(isf_FragNormCoord - 0.5, isf_FragNormCoord - 0.5) * 1.5;
    col *= max(0.0, vign);

    gl_FragColor = vec4(col, 1.0);
}
