/*{
  "DESCRIPTION": "Cosmic Dust Cloud — 3D raymarched volumetric nebula swept by magnetic field lines. Cool blue/violet/white palette. Fully standalone.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "fieldSpeed",  "LABEL": "Field Speed",  "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "dustDensity", "LABEL": "Dust Density", "TYPE": "float", "DEFAULT": 3.5,  "MIN": 0.5, "MAX": 8.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "nebulaHue",   "LABEL": "Nebula Hue",   "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────────
float hash13(vec3 p){
    p = fract(p * 0.1031);
    p += dot(p, p.zyx + 31.32);
    return fract((p.x + p.y) * p.z);
}
float hash12(vec2 p){
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float noise3(vec3 p){
    vec3 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
    return mix(mix(mix(hash13(i+vec3(0,0,0)),hash13(i+vec3(1,0,0)),f.x),
                   mix(hash13(i+vec3(0,1,0)),hash13(i+vec3(1,1,0)),f.x),f.y),
               mix(mix(hash13(i+vec3(0,0,1)),hash13(i+vec3(1,0,1)),f.x),
                   mix(hash13(i+vec3(0,1,1)),hash13(i+vec3(1,1,1)),f.x),f.y),f.z);
}

float fbm3(vec3 p){
    float v=0.0,a=0.5;
    for(int i=0;i<5;i++){ v+=a*noise3(p); p*=2.02; p+=vec3(1.7,9.2,3.4); a*=0.5; }
    return v;
}

// ─────────────────────────────────────────────────────────────────────────────
// Volumetric dust density function
// ─────────────────────────────────────────────────────────────────────────────
float dustField(vec3 p){
    float t = TIME * fieldSpeed;
    // Animated field warp — magnetic field lines rotating slowly
    vec3 warp = vec3(
        fbm3(p * 0.8 + vec3(t * 0.2, 0.0, 0.0)),
        fbm3(p * 0.8 + vec3(0.0, t * 0.15, 1.3)),
        fbm3(p * 0.8 + vec3(0.7, 0.0, t * 0.25))
    ) - 0.5;
    vec3 wp = p + warp * 1.2;
    float base = fbm3(wp * 1.5 + t * 0.08);
    // Two nested cloud structures
    float detail = fbm3(wp * 3.0 - t * 0.12) * 0.4;
    return base + detail;
}

// ─────────────────────────────────────────────────────────────────────────────
// Nebula color: cool blue → violet → white (fully saturated, no white mixing in mid-tones)
// ─────────────────────────────────────────────────────────────────────────────
vec3 nebulaColor(float density, float hue){
    vec3 voidBlue    = vec3(0.0,  0.02, 0.12);
    vec3 dustBlue    = vec3(0.05, 0.25, 0.85);
    vec3 coreViolet  = vec3(0.55, 0.0,  1.0);
    vec3 hotWhite    = vec3(1.0,  0.95, 1.0);
    float t = clamp(density, 0.0, 1.0);
    if(t < 0.3) return mix(voidBlue, dustBlue, t/0.3);
    if(t < 0.7) return mix(dustBlue, coreViolet, (t-0.3)/0.4);
    return mix(coreViolet, hotWhite, (t-0.7)/0.3);
}

void main(){
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioReact;

    // Camera sweeps slowly through the nebula volume
    float camT = TIME * fieldSpeed * 0.15;
    vec3 ro = vec3(sin(camT*1.1)*1.2, cos(camT*0.7)*0.5, camT - 3.0);
    vec3 rd = normalize(vec3(uv, 1.0));
    // Slight camera roll
    float roll = sin(camT * 0.4) * 0.1;
    rd.xy = vec2(rd.x*cos(roll) - rd.y*sin(roll),
                 rd.x*sin(roll) + rd.y*cos(roll));

    // Ray march through volume (transmittance integration)
    float transmission = 1.0;
    vec3  accLight = vec3(0.0);
    float stepSize = 0.12;
    int   steps    = 48;

    for(int i = 0; i < 48; i++){
        vec3 p = ro + rd * (float(i) * stepSize);
        float density = dustField(p) * dustDensity;
        density = max(density - 0.35, 0.0);   // threshold to clear void

        if(density > 0.001){
            float alpha = clamp(density * stepSize * 0.8, 0.0, 1.0);
            alpha *= audio;

            // Local emission color (density-based)
            float localD = clamp(density * 0.5, 0.0, 1.0);
            vec3 emitCol = nebulaColor(localD, nebulaHue) * hdrPeak;

            // Star-particle scintillation (sparse bright stars inside nebula)
            float starN = hash13(floor(p * 18.0));
            float starB = step(0.988, starN) * 3.0;
            emitCol += vec3(0.8, 0.9, 1.0) * starB;

            accLight += emitCol * transmission * alpha;
            transmission *= (1.0 - alpha);
            if(transmission < 0.01) break;
        }
    }

    // Background: deep space void with a few background stars
    vec3 bg = vec3(0.0, 0.0, 0.02);
    float bgStar = pow(hash12(floor(uv * 200.0)), 32.0) * 2.0;
    bg += vec3(0.7, 0.8, 1.0) * bgStar;

    vec3 col = bg * transmission + accLight;

    gl_FragColor = vec4(col, 1.0);
}
