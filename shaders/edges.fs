/*{
  "DESCRIPTION": "Solar Magnetosphere — 3D raymarched star with looping magnetic dipole field lines, cinematic studio lighting",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "fieldLines",  "LABEL": "Field Lines",  "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0,  "MAX": 16.0 },
    { "NAME": "orbitSpeed",  "LABEL": "Orbit Speed",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "fieldRadius", "LABEL": "Field Radius", "TYPE": "float", "DEFAULT": 1.1,  "MIN": 0.5,  "MAX": 2.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

const float PI  = 3.14159265359;
const float TAU = 6.28318530718;
const int   MAX_STEPS = 64;
const float FAR = 5.0;
const float STAR_R = 0.28;

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

// Dipole field line: parametric arc at azimuth phi, t in [0,1]
vec3 flPoint(float phi, float t) {
    float lat = (t - 0.5) * PI;
    float r   = fieldRadius * cos(lat) * cos(lat);
    float y   = fieldRadius * cos(lat) * sin(lat);
    return vec3(r * cos(phi), y, r * sin(phi));
}

float sdFieldLines(vec3 p) {
    float d    = FAR;
    float capR = 0.009;
    int   N    = int(clamp(fieldLines, 3.0, 16.0));
    for (int i = 0; i < 16; i++) {
        if (i >= N) break;
        float phi = float(i) * TAU / float(N);
        for (int s = 0; s < 8; s++) {
            float ta = float(s)     / 8.0;
            float tb = float(s + 1) / 8.0;
            d = min(d, sdCapsule(p, flPoint(phi, ta), flPoint(phi, tb), capR));
        }
    }
    return d;
}

float sdStar(vec3 p) {
    float base = length(p) - STAR_R * (1.0 + audioBass * audioReact * 0.08);
    float bump = 0.018 * sin(p.x * 17.0 + TIME * 3.1)
                       * sin(p.y * 13.0 + TIME * 2.3)
                       * sin(p.z * 11.0 + TIME * 1.7);
    return base + bump;
}

vec2 sceneSDF(vec3 p) {
    float dStar  = sdStar(p);
    float dField = sdFieldLines(p);
    return dField < dStar ? vec2(dField, 2.0) : vec2(dStar, 1.0);
}

vec3 starNormal(vec3 p) {
    float e = 0.001;
    return normalize(vec3(
        sdStar(p + vec3(e,0,0)) - sdStar(p - vec3(e,0,0)),
        sdStar(p + vec3(0,e,0)) - sdStar(p - vec3(0,e,0)),
        sdStar(p + vec3(0,0,e)) - sdStar(p - vec3(0,0,e))
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.7) * audioReact;
    float ct    = TIME * orbitSpeed;

    vec3 ro = vec3(sin(ct) * 2.5, 0.6 + sin(TIME * 0.22) * 0.25, cos(ct) * 2.5);
    vec3 fw = normalize(-ro);
    vec3 rt = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(rt, fw);
    vec3 rd = normalize(fw + uv.x * rt + uv.y * up);

    float dist  = 0.0;
    float hitId = 0.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec2 h = sceneSDF(ro + rd * dist);
        if (h.x < 0.0005 || dist > FAR) { hitId = h.y; break; }
        dist += h.x * 0.65;
    }

    vec3 col = vec3(0.0, 0.0, 0.015);

    if (dist < FAR) {
        vec3 p = ro + rd * dist;
        vec3 L = normalize(vec3(1.3, 0.9, 0.6));

        if (hitId > 1.5) {
            // Field line: blue at poles, gold at equator
            float fy  = clamp(p.y / fieldRadius + 0.5, 0.0, 1.0);
            vec3  fc  = mix(vec3(0.0, 0.6, 1.0), vec3(1.0, 0.7, 0.0), fy * fy);
            float ao  = clamp(length(p) / (STAR_R * 2.5), 0.0, 1.0);
            col = fc * hdrPeak * audio * (0.4 + ao * 0.6);
            col += vec3(2.5) * pow(max(0.0, dot(L, normalize(p))), 20.0);
        } else {
            // Star: orange-gold Lambertian + white-hot specular
            vec3  n    = starNormal(p);
            float diff = clamp(dot(n, L), 0.05, 1.0);
            float spec = pow(clamp(dot(reflect(-L, n), -rd), 0.0, 1.0), 16.0);
            vec3  base = mix(vec3(1.0, 0.35, 0.0), vec3(1.0, 0.8, 0.15), diff);
            col = base * hdrPeak * diff * audio + vec3(3.0) * spec;
        }
    }

    // Volumetric corona
    for (int i = 0; i < 40; i++) {
        float s = float(i) * 0.07;
        if (s > FAR) break;
        vec3 p  = ro + rd * s;
        float d = max(0.0, STAR_R * 2.6 - length(p));
        col += vec3(1.0, 0.35, 0.05) * d * 0.045 * audio * hdrPeak;
    }

    gl_FragColor = vec4(col, 1.0);
}
