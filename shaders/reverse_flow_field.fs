/*{
  "DESCRIPTION": "Arctic Aurora Borealis — 3D volumetric raymarched aurora curtains in a night sky: electric green, cyan, violet-blue, viewed from below",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "auroraSpeed", "LABEL": "Aurora Speed",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 0.8 },
    { "NAME": "auroraHeight","LABEL": "Curtain Height","TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 5.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",      "TYPE": "float", "DEFAULT": 2.6,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "starDensity", "LABEL": "Star Density",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI 3.14159265359
#define MAX_STEPS 48

float hash11(float n)  { return fract(sin(n*12.9898)*43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

float smoothNoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f*f*(3.0-2.0*f);
    return mix(mix(hash21(i),      hash21(i+vec2(1,0)),f.x),
               mix(hash21(i+vec2(0,1)),hash21(i+vec2(1,1)),f.x),f.y);
}
float fbm(vec2 p) {
    float v=0.0,a=0.5;
    for(int i=0;i<4;i++){v+=smoothNoise(p)*a;p*=2.1;a*=0.5;}
    return v;
}

// Aurora density at a 3D point (curtain-like structure in XZ plane, varying with Y)
float auroraDensity(vec3 p) {
    float t = TIME * auroraSpeed;
    // Curtain position varies along X by a slow sine
    float curtainX1 = sin(p.x * 0.4 + t * 0.3) * 0.8 + sin(p.x * 0.9 - t * 0.17) * 0.4;
    float curtainX2 = sin(p.x * 0.3 - t * 0.25) * 1.0 + cos(p.x * 0.7 + t * 0.22) * 0.3;

    // Curtain: thin sheet at Z ≈ curtainX1 and Z ≈ curtainX2
    float sheet1 = exp(-abs(p.z - curtainX1) * 3.0);
    float sheet2 = exp(-abs(p.z - curtainX2) * 2.5);

    // Height modulation: aurora is at Y 1..6, fades at bottom and top
    float heightFade = smoothstep(0.5, 2.0, p.y) * smoothstep(auroraHeight*1.1, auroraHeight*0.7, p.y);

    // FBM turbulence
    float turb = fbm(vec2(p.x*0.4+t*0.2, p.y*0.3-t*0.15)) * 1.3;

    return (sheet1 + sheet2 * 0.7) * heightFade * turb;
}

// Aurora color: mix of electric green, cyan, and violet-blue based on height in curtain
vec3 auroraColor(vec3 p) {
    float t = TIME * auroraSpeed * 0.5;
    float heightRel = clamp((p.y - 0.5) / auroraHeight, 0.0, 1.0);

    vec3 low  = vec3(0.0, 1.0, 0.3);   // electric green
    vec3 mid  = vec3(0.0, 0.85, 1.0);  // cyan
    vec3 high = vec3(0.35, 0.1, 1.0);  // violet-blue

    vec3 col;
    if (heightRel < 0.5) col = mix(low, mid, heightRel*2.0);
    else                 col = mix(mid, high, (heightRel-0.5)*2.0);

    // Animate hue: slow oscillation between green-cyan and violet
    float hueShift = sin(t*0.7 + p.x*0.2)*0.3;
    col = mix(col, col.bgr, clamp(hueShift,0.0,0.4));
    return col;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.3 + audioMid*audioReact*0.15;

    // Camera: looking upward at a shallow angle
    float ct = TIME * auroraSpeed * 0.12;
    vec3 ro  = vec3(ct * 0.3, 0.0, 0.0); // slow forward drift through arctic landscape
    // Look direction: slightly upward
    vec3 rd  = normalize(vec3(uv.x, uv.y*0.5 + 0.6, 1.0));

    // Deep midnight sky background
    vec3 col = mix(vec3(0.01, 0.015, 0.04), vec3(0.005, 0.005, 0.02), uv.y*0.5+0.5);

    // Stars
    float starMask = step(1.0 - starDensity*0.003, hash21(floor(rd.xy*180.0)));
    col += vec3(0.7,0.8,1.0) * starMask * 0.9;

    // Horizon glow (cold blue twilight at the bottom)
    float horizon = exp(-max(uv.y+0.3,0.0)*4.0);
    col += vec3(0.05, 0.12, 0.25)*horizon;

    // Ground: dark tundra silhouette at bottom
    if (uv.y < -0.55) {
        float tundra = 0.5 + fbm(vec2(uv.x*3.0, 0.0))*0.5;
        float gndLine = -0.55 - tundra*0.08;
        float gndMask = step(uv.y, gndLine);
        col = mix(col, vec3(0.008, 0.01, 0.018), gndMask);
    }

    // Volumetric aurora raymarch (48 steps into sky)
    vec3 auroraCol = vec3(0.0);
    float transmittance = 1.0;
    float rayT = 0.5;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * rayT;
        if (p.y < 0.0) { rayT += 0.3; continue; }
        float density = auroraDensity(p) * 0.12;
        if (density > 0.001) {
            vec3 ac = auroraColor(p) * hdrPeak * audio;
            auroraCol += ac * density * transmittance;
            transmittance *= exp(-density * 0.8);
        }
        rayT += 0.2 + p.y*0.05;
        if (rayT > 30.0 || transmittance < 0.01) break;
    }

    col += auroraCol;

    // Bright star near aurora (Polaris-style)
    vec2 polarisUV = uv - vec2(aspect*0.3, 0.45);
    float polaris = exp(-dot(polarisUV,polarisUV)*800.0);
    col += vec3(0.9,0.95,1.0)*polaris*2.5*audio;

    gl_FragColor = vec4(col, 1.0);
}
