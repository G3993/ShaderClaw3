/*{
  "DESCRIPTION":"Julia Drift — a living Julia set whose seed c orbits a smooth path, morphing the fractal forever; smooth-iteration color, breathing zoom, audio-reactive drift/fringe/pulse.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","Fractal","Audio Reactive"],
  "INPUTS":[
    {"NAME":"cSpeed","LABEL":"Morph Speed","TYPE":"float","DEFAULT":0.06,"MIN":0.0,"MAX":1.0},
    {"NAME":"cRadius","TYPE":"float","DEFAULT":0.74,"MIN":0.0,"MAX":1.0},
    {"NAME":"zoom","TYPE":"float","DEFAULT":1.4,"MIN":0.4,"MAX":4.0},
    {"NAME":"maxI","TYPE":"float","DEFAULT":180.0,"MIN":60.0,"MAX":256.0},
    {"NAME":"paletteFreq","TYPE":"float","DEFAULT":0.045,"MIN":0.005,"MAX":0.2},
    {"NAME":"paletteShift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"fringeGlow","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"audioReact","LABEL":"Sound Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/

// Curated cosine palette (house style)
vec3 pal(float t){ return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67))); }

void main() {
    // Live audio bands (runtime auto-provides audioBass/audioMid/audioHigh)
    float bass   = audioBass * audioReact;
    float mid    = audioMid  * audioReact;
    float treble = audioHigh * audioReact;

    // Normalized, aspect-correct coordinates centered on screen
    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Zoom breathes slowly; bass gently pulses it (positional, K<=0.6)
    float breathe = zoom * (1.0 + 0.06*sin(TIME*0.17));
    breathe *= (1.0 + bass*0.6);
    vec2 z = uv * breathe;

    // Seed c drifts along a smooth path — the living morph, sped a touch by mid
    float a = TIME*cSpeed + mid*0.8;
    float r = cRadius * (1.0 + 0.04*sin(TIME*0.23));
    vec2 c = r*vec2(cos(a), sin(a*1.3)) + vec2(-0.4, 0.0);

    // Iterate Julia with continuous (smooth) escape count
    float i = 0.0;
    int cap = int(clamp(maxI, 1.0, 256.0));
    for(int n=0; n<256; n++){
        if(n >= cap) break;
        z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
        if(dot(z,z) > 256.0) break;
        i += 1.0;
    }

    float d2 = dot(z,z);
    vec3 col;

    if(i >= float(cap) - 0.5){
        // Interior: deep near-black with a faint internal shimmer
        float shimmer = 0.012 + 0.010*sin(z.x*9.0 + z.y*7.0 + TIME*0.4);
        col = pal(paletteShift + 0.5) * max(shimmer, 0.0);
    } else {
        // Smooth iteration count (banding-free)
        float sm = i + 1.0 - log(log(max(d2, 1.0001))*0.5/log(2.0)) / log(2.0);

        // Color the exterior with the cosine palette
        col = pal(sm*paletteFreq + paletteShift + TIME*0.02);

        // User image colors the living Julia bands (default texMix 0 = pure palette)
        if (texMix > 0.0) {
            // Sample by the escaped-point direction, scrolled by the smooth iteration value
            vec2 tuv = normalize(z)*0.5 + 0.5;
            tuv = fract(tuv + vec2(sm*paletteFreq, sm*paletteFreq*0.5));
            vec3 img = texture2D(inputImage, tuv).rgb;
            col = mix(col, col*(0.4 + 1.6*img), texMix);
        }

        // Brighten the escape fringe — treble-reactive (additive, K<=0.6)
        float edge = exp(-abs(i - (float(cap)-1.0)) * 0.0);    // baseline 1.0
        float fringe = pow(clamp(1.0 - i/float(cap), 0.0, 1.0), 3.0);
        col += fringe * (fringeGlow * (0.35 + treble*0.6)) * pal(sm*paletteFreq + 0.15);

        // Gentle contrast falloff toward the set boundary keeps it deep
        col *= 0.55 + 0.6*clamp(i/float(cap), 0.0, 1.0);
    }

    // Tonemap + gamma (house style)
    col = col / (1.0 + col);
    col = pow(col, vec3(0.4545));

    gl_FragColor = vec4(col, 1.0);
}
