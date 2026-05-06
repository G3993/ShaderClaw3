/*{
  "DESCRIPTION": "Glacial Crystal Cave — 3D raymarched ice stalactites and floor crystals, cold blue studio lighting",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "density",    "LABEL": "Crystal Density", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.2,  "MAX": 0.9 },
    { "NAME": "orbitSpeed", "LABEL": "Orbit Speed",     "TYPE": "float", "DEFAULT": 0.14, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "iceColor",   "LABEL": "Ice Hue",         "TYPE": "color", "DEFAULT": [0.1, 0.55, 1.0, 1.0] },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",        "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio React",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

const int   MAX_STEPS = 64;
const float FAR       = 8.0;
const float PI        = 3.14159265;

// Rounded cone (stalactite shape): from base r1 at y=0 to tip r2 at y=h, pointing DOWN
float sdRoundCone(vec3 p, float r1, float r2, float h) {
    vec2 q = vec2(length(p.xz), -p.y);
    float b = (r1 - r2) / h;
    float a = sqrt(1.0 - b * b);
    float k = dot(q, vec2(-b, a));
    if (k < 0.0) return length(q) - r1;
    if (k > a * h) return length(q - vec2(0.0, h)) - r2;
    return dot(q, vec2(a, b)) - r1;
}

// Ice floor plane
float sdFloor(vec3 p) { return p.y + 1.0; }

// Cell-repeated stalactites and floor crystals
float sdCave(vec3 p) {
    float cs   = density * 0.7 + 0.2; // cell size

    // Stalactites from ceiling at y=+1.5
    vec3 pc = p;
    pc.xz = mod(pc.xz + cs * 0.5, cs) - cs * 0.5;
    float h   = 0.4 + 0.25 * sin(floor(p.x / cs) * 7.3 + floor(p.z / cs) * 11.1);
    float tip = 0.005 + 0.002 * sin(floor(p.x / cs) * 5.7 + floor(p.z / cs) * 8.3);
    float base = 0.04 + 0.02 * cos(floor(p.x / cs) * 3.9 + floor(p.z / cs) * 6.7);
    float dStal = sdRoundCone(vec3(pc.x, p.y - 1.5, pc.z), base, tip, h);

    // Floor crystals (smaller, pointing up)
    float ch  = 0.12 + 0.08 * sin(floor(p.x / cs) * 9.1 + floor(p.z / cs) * 13.7);
    float dCryst = sdRoundCone(vec3(pc.x, -(p.y + 0.95), pc.z), 0.025, 0.002, ch);

    return min(dStal, min(dCryst, sdFloor(p)));
}

float sdScene(vec3 p) { return sdCave(p); }

vec3 sceneNormal(vec3 p) {
    float e = 0.002;
    return normalize(vec3(
        sdScene(p + vec3(e,0,0)) - sdScene(p - vec3(e,0,0)),
        sdScene(p + vec3(0,e,0)) - sdScene(p - vec3(0,e,0)),
        sdScene(p + vec3(0,0,e)) - sdScene(p - vec3(0,0,e))
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel * 0.4 + audioBass * 0.6) * audioReact;
    float ct    = TIME * orbitSpeed;

    // Camera below stalactites looking up at an angle
    vec3 ro = vec3(sin(ct) * 1.6, -0.3 + sin(TIME * 0.17) * 0.15, cos(ct) * 1.6);
    vec3 target = vec3(0.0, 0.8, 0.0);
    vec3 fw = normalize(target - ro);
    vec3 rt = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(rt, fw);
    vec3 rd = normalize(fw + uv.x * rt + uv.y * up);

    float dist = 0.0;
    int   hit  = 0;
    for (int i = 0; i < MAX_STEPS; i++) {
        float d = sdScene(ro + rd * dist);
        if (d < 0.001 || dist > FAR) { hit = 1; break; }
        dist += d * 0.7;
    }

    vec3 col = vec3(0.0, 0.01, 0.03);

    if (hit == 1 && dist < FAR) {
        vec3 p = ro + rd * dist;
        vec3 n = sceneNormal(p);

        // Cold key light from above-front, icy cyan rim from below-back
        vec3 Lkey = normalize(vec3(0.5, 1.2, 0.8));
        vec3 Lrim = normalize(vec3(-0.4, -1.0, -0.6));

        float diffKey = clamp(dot(n, Lkey), 0.0, 1.0);
        float diffRim = clamp(dot(n, Lrim), 0.0, 1.0) * 0.3;
        float spec    = pow(clamp(dot(reflect(-Lkey, n), -rd), 0.0, 1.0), 40.0);

        // Ice palette: deep navy → ice blue → white-hot specular
        float height = clamp((p.y + 1.0) / 2.5, 0.0, 1.0);
        vec3  iceBase = mix(vec3(0.02, 0.08, 0.25), iceColor.rgb, height);
        vec3  rimCol  = vec3(0.0, 0.9, 1.0);

        col  = iceBase * (diffKey * 0.8 + 0.15) * hdrPeak * audio;
        col += rimCol * diffRim * hdrPeak * 0.6;
        col += vec3(2.5, 2.8, 3.0) * spec;    // white-cold specular

        // Black ink edge via fwidth normal gradient
        float edge = 1.0 - smoothstep(0.0, fwidth(diffKey) * 8.0, diffKey);
        col *= (1.0 - edge * 0.7);
    }

    // Ambient ice glow fog
    float fog = exp(-dist * 0.4);
    col = mix(col, vec3(0.0, 0.04, 0.12) * hdrPeak * 0.2, (1.0 - fog) * 0.4);

    gl_FragColor = vec4(col, 1.0);
}
