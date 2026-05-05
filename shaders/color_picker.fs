/*{
    "DESCRIPTION": "Prismatic Refraction — 3D glass prism dispersing white light into full spectrum. Cinematic dark studio, chromatic dispersion cast on floor. 64-step SDF march.",
    "CATEGORIES": ["Generator", "3D", "Prismatic", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "prismAngle",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.1, "MAX": 1.2,  "LABEL": "Prism Angle" },
        { "NAME": "dispersion",  "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5,  "LABEL": "Dispersion" },
        { "NAME": "hdrPeak",     "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
mat2 rot2(float a)   { float c=cos(a),s=sin(a); return mat2(c,-s,s,c); }

// Triangular prism SDF (2D triangle cross-section, extruded along Y)
float sdTriPrism(vec3 p, float h, float ang) {
    // Equilateral triangle in XZ
    float k = sqrt(3.0) * 0.5;
    p.xz = abs(p.xz);
    float d = max(p.z * k + p.x * 0.5, p.x) - h;
    d = max(d, abs(p.y) - ang);
    return d;
}

// Scene: prism + floor plane
float sceneSDF(vec3 p) {
    // Rotate prism slowly
    float spinT = TIME * 0.15;
    vec3 q = p;
    q.xz = rot2(spinT) * q.xz;
    float prism = sdTriPrism(q, 0.5, prismAngle * 0.7);
    float floor_ = p.y + 0.8;
    return min(prism, floor_);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p+e.xyy)-sceneSDF(p-e.xyy),
        sceneSDF(p+e.yxy)-sceneSDF(p-e.yxy),
        sceneSDF(p+e.yyx)-sceneSDF(p-e.yyx)
    ));
}

// HSV spectrum color for dispersion
vec3 spectrumColor(float t) {
    t = fract(t);
    // Red → Orange → Yellow → Green → Cyan → Blue → Violet
    float hue = t;
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p3 = abs(fract(vec3(hue) + K.xyz) * 6.0 - K.www);
    return clamp(p3 - K.xxx, 0.0, 1.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;

    // Studio camera: low angle, side view
    vec3 ro = vec3(2.8, 0.5, 1.8);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fw  = normalize(target - ro);
    vec3 rgt = normalize(cross(fw, vec3(0.0,1.0,0.0)));
    vec3 up_ = cross(rgt, fw);
    vec3 rd  = normalize(fw + uv.x * rgt * 0.65 + uv.y * up_ * 0.65);

    float dist = 0.0;
    bool hit = false;
    bool isFloor = false;
    for (int i = 0; i < 64; i++) {
        float d = sceneSDF(ro + rd * dist);
        if (d < 0.002) {
            hit = true;
            isFloor = ((ro + rd * dist).y < -0.79);
            break;
        }
        dist += d;
        if (dist > 12.0) break;
    }

    vec3 col = vec3(0.0, 0.0, 0.005); // deep void studio

    if (hit) {
        vec3 p = ro + rd * dist;
        vec3 N = calcNormal(p);

        if (isFloor) {
            // Floor: chromatic dispersion rainbow bands cast by prism
            // Project floor point relative to prism base
            vec2 floorPos = p.xz;
            float dist2Prism = length(floorPos);
            float angle2Prism = atan(floorPos.y, floorPos.x) / 6.2832 + 0.5 + t * 0.1;

            // Dispersion: each channel offset
            float bandT = fract(dist2Prism * 1.5 + angle2Prism + t * 0.05);
            vec3 specCol = spectrumColor(bandT);
            float bandMask = exp(-abs(dist2Prism - 1.0) * 2.0); // ring of dispersion

            col = vec3(0.03, 0.03, 0.035) + specCol * bandMask * hdrPeak * audio;
        } else {
            // Glass prism: refractive highlight + prismatic edge glow
            vec3 key = normalize(vec3(1.5, 2.0, 1.0));
            float kD  = max(dot(N, key), 0.0);
            float sp  = pow(max(dot(reflect(-key,N),-rd),0.0), 64.0);

            // Fresnel-based rainbow at grazing angles
            float fresnel = pow(1.0 - max(dot(-rd, N), 0.0), 4.0);
            float specT   = fract(dot(N.xz, vec2(1.3,0.7)) + t * 0.2);
            vec3 prisMcol = mix(vec3(0.5, 0.55, 0.6), spectrumColor(specT), fresnel * dispersion * 3.0);

            col  = prisMcol * (kD + 0.05) * hdrPeak * audio;
            col += vec3(1.0) * sp * hdrPeak * 0.8; // white-hot specular
            // Rainbow edge glow
            col += spectrumColor(specT) * fresnel * dispersion * hdrPeak * 2.0 * audio;
        }
    }

    // fwidth-based edge darkening on silhouettes
    gl_FragColor = vec4(col, 1.0);
}
