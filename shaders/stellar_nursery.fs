/*{
    "DESCRIPTION": "A star-forming nebula like Carina or Orion — pillars of dust with hot young stars igniting inside, ionised hydrogen glowing red, oxygen glowing teal, with subtle dust-lane occlusion. Bass triggers proto-star ignition events that flash and persist as new bright pinpoints",
    "CREDIT": "Easel",
    "CATEGORIES": ["GENERATOR"],
    "INPUTS": [
        { "NAME": "dustScale",       "TYPE": "float", "DEFAULT": 1.6,  "MIN": 0.4, "MAX": 4.0 },
        { "NAME": "gasScale",        "TYPE": "float", "DEFAULT": 3.2,  "MIN": 0.8, "MAX": 8.0 },
        { "NAME": "dustOpacity",     "TYPE": "float", "DEFAULT": 0.85, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "starCount",       "TYPE": "float", "DEFAULT": 48.0, "MIN": 8.0, "MAX": 60.0 },
        { "NAME": "starBrightness",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "ionisationGlow",  "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "igniteRate",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.1, "MAX": 4.0 },
        { "NAME": "audioReact",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "bassLevel",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "midLevel",        "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "trebleLevel",     "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "hydrogenColor",   "TYPE": "color", "DEFAULT": [0.95, 0.25, 0.35, 1.0] },
        { "NAME": "oxygenColor",     "TYPE": "color", "DEFAULT": [0.20, 0.85, 0.85, 1.0] },
        { "NAME": "dustColor",       "TYPE": "color", "DEFAULT": [0.55, 0.30, 0.18, 1.0] }
    ]
}*/

float hash11(float p){ p = fract(p*0.1031); p *= p+33.33; p *= p+p; return fract(p); }
float hash12(vec2 p){ vec3 p3 = fract(vec3(p.xyx)*0.1031); p3 += dot(p3, p3.yzx+33.33); return fract((p3.x+p3.y)*p3.z); }
vec2  hash22(vec2 p){
    vec3 p3 = fract(vec3(p.xyx)*vec3(0.1031,0.1030,0.0973));
    p3 += dot(p3, p3.yzx+33.33);
    return fract((p3.xx+p3.yz)*p3.zy);
}

float vnoise(vec2 p){
    vec2 i = floor(p), f = fract(p);
    float a = hash12(i);
    float b = hash12(i+vec2(1.0,0.0));
    float c = hash12(i+vec2(0.0,1.0));
    float d = hash12(i+vec2(1.0,1.0));
    vec2 u = f*f*(3.0-2.0*f);
    return mix(mix(a,b,u.x), mix(c,d,u.x), u.y);
}

float fbm(vec2 p){
    float s = 0.0, a = 0.5;
    mat2 R = mat2(0.8,-0.6,0.6,0.8);
    for(int i=0; i<6; i++){
        s += a*vnoise(p);
        p = R*p*2.02;
        a *= 0.5;
    }
    return s;
}

// crude curl from fbm gradient
vec2 curl(vec2 p){
    float e = 0.08;
    float n1 = fbm(p+vec2(0.0,e));
    float n2 = fbm(p-vec2(0.0,e));
    float n3 = fbm(p+vec2(e,0.0));
    float n4 = fbm(p-vec2(e,0.0));
    return vec2(n1-n2, -(n3-n4));
}

void main(){
    vec2 uv = isf_FragNormCoord;
    vec2 p  = uv*2.0 - 1.0;
    p.x *= RENDERSIZE.x/RENDERSIZE.y;

    float t   = TIME;
    float bass    = bassLevel    * audioReact;
    float mid     = midLevel     * audioReact;
    float treble  = trebleLevel  * audioReact;

    // slow drift
    vec2 drift = vec2(t*0.018, t*0.012);
    float expansion = 1.0 + 0.06*sin(t*0.13) + 0.10*mid;

    // Layer 1: dust (large fbm) — pillars via smoothstep threshold
    vec2 dp = p*dustScale*expansion + drift;
    float dust = fbm(dp + 0.6*curl(dp*0.5));
    // pillar mask: vertical stretch
    float pillar = fbm(vec2(p.x*dustScale*1.3 + drift.x, p.y*dustScale*0.45 - drift.y*0.5));
    float dustMask = smoothstep(0.35, 0.78, dust*0.65 + pillar*0.55);
    dustMask *= dustOpacity;

    // Layer 2: ionised gas — warped by curl
    vec2 gp = p*gasScale + drift*1.7;
    gp += 0.45*curl(gp*0.6 + t*0.05);
    float gasH = fbm(gp);                          // hydrogen channel
    float gasO = fbm(gp*1.35 + vec2(7.3,-2.1));    // oxygen channel
    gasH = smoothstep(0.38, 0.95, gasH);
    gasO = smoothstep(0.42, 0.92, gasO);

    // ionisation front: edges where dust meets gas
    float dustGrad = abs(fbm(dp+vec2(0.04,0.0)) - fbm(dp-vec2(0.04,0.0)))
                   + abs(fbm(dp+vec2(0.0,0.04)) - fbm(dp-vec2(0.0,0.04)));
    float front = smoothstep(0.05, 0.18, dustGrad) * smoothstep(0.2, 0.6, gasH+gasO);

    // Doppler tint across canvas
    float doppler = uv.x - 0.5;

    vec3 colDust = dustColor.rgb * (0.5 + 0.7*dust);
    vec3 colH    = hydrogenColor.rgb * gasH;
    vec3 colO    = oxygenColor.rgb   * gasO;
    colH *= (1.0 + 0.25*doppler);
    colO *= (1.0 - 0.25*doppler);

    vec3 nebula = colH*1.2 + colO*1.0;
    nebula += front * (hydrogenColor.rgb*0.6 + oxygenColor.rgb*0.4) * ionisationGlow * 1.4;

    // dust occlusion (in front of gas)
    nebula = mix(nebula, colDust*0.6, dustMask*0.85);

    // base background — deep purple-black
    vec3 col = vec3(0.015, 0.012, 0.028) + nebula;

    // Stars: hashed positions, twinkle, halos, ignition events
    int N = int(clamp(starCount, 1.0, 60.0));
    float twinkleHz = 1.5 + 4.0*treble;
    for(int i=0; i<60; i++){
        if(i>=N) break;
        float fi = float(i);
        vec2 sp = hash22(vec2(fi, 17.0)) * 2.0 - 1.0;
        sp.x *= RENDERSIZE.x/RENDERSIZE.y;
        // gentle drift binding to nebula motion
        sp += 0.04*vec2(sin(t*0.07+fi), cos(t*0.05+fi*0.7));

        // ignition bucket: each star has its own clock; bucket index advances with igniteRate
        float clock = t*igniteRate*0.25 + hash11(fi*3.17)*1000.0;
        float bucket = floor(clock);
        float local = fract(clock);
        // ignition triggers when bucket hash crosses threshold (rare)
        float ignH = hash11(fi*7.91 + bucket*53.7);
        float ignite = step(0.94, ignH);
        // 0..1 envelope: fast rise, ~2sec decay (within bucket span)
        float ignEnv = ignite * exp(-local*4.0) * (1.0 - exp(-local*40.0));
        // bass boost on ignition
        ignEnv *= (1.0 + 1.2*bass);

        float twinklePhase = hash11(fi*1.91)*6.2831;
        float twinkle = 0.6 + 0.4*sin(t*twinkleHz + twinklePhase);

        float baseMag = mix(0.25, 1.0, hash11(fi*5.13));
        float mag = baseMag*twinkle + 2.5*ignEnv;

        float d = length(p - sp);
        // sharp core
        float core = exp(-d*d*9000.0/(1.0+8.0*ignEnv));
        // halo
        float halo = exp(-d*d*55.0)*0.35;

        // star colour: blue-white young stars, occasional yellow
        float ct = hash11(fi*2.71);
        vec3 starCol = mix(vec3(0.75,0.85,1.0), vec3(1.0,0.95,0.78), step(0.7, ct));
        starCol = mix(starCol, vec3(1.0,0.6,0.4), step(0.92, ct)); // rare red

        // ionised halo around bright/igniting stars
        vec3 nebHalo = mix(hydrogenColor.rgb, oxygenColor.rgb, hash11(fi*4.4));
        float haloStrength = (0.5 + ignEnv*1.5) * ionisationGlow;

        col += starCol * core * mag * starBrightness;
        col += starCol * halo * mag * starBrightness * 0.6;
        col += nebHalo * halo * haloStrength * (0.3 + ignEnv);
    }

    // soft vignette
    float vig = smoothstep(1.6, 0.4, length(p));
    col *= mix(0.7, 1.05, vig);

    // gentle global lift on bass
    col *= 1.0 + 0.15*bass;

    // tonemap-ish
    col = col / (1.0 + col*0.55);
    col = pow(max(col, 0.0), vec3(0.95));

    gl_FragColor = vec4(col, 1.0);
}
