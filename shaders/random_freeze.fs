/*{
  "DESCRIPTION": "Triple Torus Knot — 3D raymarched interlocking tori in three planes, iridescent neon palette",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "ringScale",  "LABEL": "Ring Scale",   "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.3,  "MAX": 1.8 },
    { "NAME": "spinSpeed",  "LABEL": "Spin Speed",   "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "tubeRadius", "LABEL": "Tube Radius",  "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.04, "MAX": 0.22 },
    { "NAME": "hdrPeak",   "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioReact","LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

const int   MAX_STEPS = 64;
const float FAR       = 6.0;
const float PI        = 3.14159265;

float sdTorus(vec3 p, float R, float r) {
    return length(vec2(length(p.xz) - R, p.y)) - r;
}

// Rotate p around X axis by angle a
vec3 rotX(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
}
vec3 rotZ(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c * p.x - s * p.y, s * p.x + c * p.y, p.z);
}
vec3 rotY(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
}

vec2 sceneSDF(vec3 p) {
    float t   = TIME * spinSpeed;
    float R   = ringScale;
    float tr  = tubeRadius * (1.0 + audioBass * audioReact * 0.15);

    // Torus 1: XZ plane, rotating around Y
    float d1 = sdTorus(rotY(p, t), R, tr);
    // Torus 2: YZ plane (tilted 90° in X), counter-rotating
    float d2 = sdTorus(rotX(rotY(p, -t * 0.7), PI * 0.5), R * 0.85, tr);
    // Torus 3: diagonal tilt, slower spin
    float d3 = sdTorus(rotZ(rotX(p, PI * 0.25 + t * 0.4), t * 0.3), R * 0.95, tr);

    if (d1 < d2 && d1 < d3) return vec2(d1, 1.0);
    if (d2 < d3) return vec2(d2, 2.0);
    return vec2(d3, 3.0);
}

float sceneD(vec3 p) { return sceneSDF(p).x; }

vec3 sceneNormal(vec3 p) {
    float e = 0.001;
    return normalize(vec3(
        sceneD(p + vec3(e,0,0)) - sceneD(p - vec3(e,0,0)),
        sceneD(p + vec3(0,e,0)) - sceneD(p - vec3(0,e,0)),
        sceneD(p + vec3(0,0,e)) - sceneD(p - vec3(0,0,e))
    ));
}

// Iridescent palette: each torus gets a distinct fully-saturated hue
vec3 torusColor(float id, float position) {
    float hue = mod(id * 0.33 + position * 0.05 + TIME * 0.03, 1.0) * 3.0;
    // 3-color cycle: electric violet, hot magenta, acid gold
    vec3 c0 = vec3(0.5, 0.0, 1.0);  // violet
    vec3 c1 = vec3(1.0, 0.0, 0.7);  // magenta
    vec3 c2 = vec3(1.0, 0.8, 0.0);  // gold
    if (hue < 1.0) return mix(c0, c1, hue);
    if (hue < 2.0) return mix(c1, c2, hue - 1.0);
    return mix(c2, c0, hue - 2.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel * 0.4 + audioBass * 0.7) * audioReact;
    float ct    = TIME * spinSpeed * 0.3;

    vec3 ro = vec3(sin(ct) * 3.2, 0.9 + sin(TIME * 0.25) * 0.3, cos(ct) * 3.2);
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

    vec3 col = vec3(0.0, 0.0, 0.01);

    if (dist < FAR) {
        vec3 p  = ro + rd * dist;
        vec3 n  = sceneNormal(p);
        vec3 L  = normalize(vec3(1.2, 1.0, 0.6));

        float diff = clamp(dot(n, L), 0.05, 1.0);
        float spec = pow(clamp(dot(reflect(-L, n), -rd), 0.0, 1.0), 20.0);

        float pos  = atan(p.z, p.x) / (2.0 * PI); // position on ring for iridescence
        vec3  base = torusColor(hitId, pos);

        // fwidth edge darkening (black ink contrast on silhouette)
        float fw = fwidth(diff);
        float ink = smoothstep(fw * 2.0, 0.0, diff - 0.15);
        col = base * diff * hdrPeak * audio * (1.0 - ink * 0.9);
        col += vec3(3.0) * spec;
    }

    gl_FragColor = vec4(col, 1.0);
}
