/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "3D"],
  "DESCRIPTION": "Liquid Ripples 3D — raymarched water surface with cymatic interference ripples sculpted by audio frequencies. Cinematic key lighting, Fresnel reflection, fwidth AA on contours. HDR linear output.",
  "INPUTS": [
    {"NAME":"rippleSources","LABEL":"Sources","TYPE":"float","MIN":1.0,"MAX":6.0,"DEFAULT":4.0},
    {"NAME":"freqScale","LABEL":"Frequency","TYPE":"float","MIN":4.0,"MAX":40.0,"DEFAULT":14.0},
    {"NAME":"speed","LABEL":"Speed","TYPE":"float","MIN":0.0,"MAX":4.0,"DEFAULT":1.5},
    {"NAME":"waveAmp","LABEL":"Amplitude","TYPE":"float","MIN":0.01,"MAX":0.3,"DEFAULT":0.09},
    {"NAME":"idleAmp","LABEL":"Idle Amplitude","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.15},
    {"NAME":"waterColor","LABEL":"Water","TYPE":"color","DEFAULT":[0.03,0.08,0.18,1.0]},
    {"NAME":"specColor","LABEL":"Specular","TYPE":"color","DEFAULT":[0.85,0.95,1.0,1.0]},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"camHeight","LABEL":"Cam Height","TYPE":"float","MIN":0.5,"MAX":5.0,"DEFAULT":2.0},
    {"NAME":"camOrbitSpeed","LABEL":"Orbit Speed","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.12},
    {"NAME":"inputTex","LABEL":"Background","TYPE":"image"}
  ]
}*/

float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

vec2 rippleSource(int i) {
    float fi = float(i);
    return vec2(hash2(vec2(fi, 1.7)), hash2(vec2(fi, 9.3))) * 1.6 - 0.8;
}

float waterH(vec2 p, float t) {
    float h = 0.0;
    int N = int(clamp(rippleSources, 1.0, 6.0));
    for (int i = 0; i < 6; i++) {
        if (i >= N) break;
        float fi = float(i);
        vec2 src = rippleSource(i);
        float dist = length(p - src);
        // Bass->back layers, treble->front — mirrors cymatic frequency mapping
        float bin = clamp(mix(0.05, 0.55, fi / 5.0), 0.0, 1.0);
        float amp = (texture(audioFFT, vec2(bin, 0.5)).r * audioReact + idleAmp);
        h += sin(dist * freqScale - t * speed + fi * 1.3) * amp / (1.0 + dist * 1.5);
    }
    return h * waveAmp;
}

vec3 waterNormal(vec2 p, float t) {
    float e = 0.005;
    float hL = waterH(p - vec2(e, 0.0), t);
    float hR = waterH(p + vec2(e, 0.0), t);
    float hD = waterH(p - vec2(0.0, e), t);
    float hU = waterH(p + vec2(0.0, e), t);
    return normalize(vec3(hL - hR, e * 3.0, hD - hU));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME;
    float audioMod = 0.5 + 0.5 * audioLevel * audioReact;

    // Orbiting camera
    float angle = TIME * camOrbitSpeed;
    vec3 ro = vec3(cos(angle) * 2.5, camHeight, sin(angle) * 2.5);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(vec3(0.0, 1.0, 0.0), ww));
    vec3 vv = cross(ww, uu);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.5 * ww);

    vec3 col = vec3(0.0);
    vec3 keyDir = normalize(vec3(-1.0, 2.5, 0.5));

    if (abs(rd.y) > 0.001) {
        float tPlane = -ro.y / rd.y;
        if (tPlane > 0.0 && tPlane < 50.0) {
            vec3 p = ro + rd * tPlane;
            float h = waterH(p.xz, t);
            // One refinement step toward displaced surface
            float sgn = sign(-rd.y);
            vec3 p2 = ro + rd * (tPlane + h * 0.4 * sgn);
            float h2 = waterH(p2.xz, t);
            p2.y = h2;

            vec3 n = waterNormal(p2.xz, t);
            vec3 v = normalize(-rd);

            // Blinn-Phong
            float diff = max(dot(n, keyDir), 0.0);
            vec3 hv = normalize(keyDir + v);
            float spec = pow(max(dot(n, hv), 0.0), 160.0);

            // Fresnel
            float fres = pow(1.0 - max(dot(n, v), 0.0), 4.0);

            // Sky reflection
            vec3 refl = reflect(rd, n);
            vec3 skyRefl = mix(vec3(0.04, 0.07, 0.14), vec3(0.4, 0.6, 1.0), clamp(refl.y, 0.0, 1.0));
            skyRefl += pow(max(dot(refl, keyDir), 0.0), 32.0) * vec3(1.0, 0.9, 0.7) * 0.8;

            // Input texture as environment if available
            if (IMG_SIZE_inputTex.x > 0.0) {
                vec2 tUV = clamp(refl.xz * 0.3 + 0.5, 0.0, 1.0);
                skyRefl = mix(skyRefl, texture(inputTex, tUV).rgb * 1.2, fres * 0.6);
            }

            // Water base color — depth-tinted
            float depth = clamp(abs(h2) * 6.0, 0.0, 1.0);
            vec3 waterShallow = waterColor.rgb + vec3(0.03, 0.08, 0.12);
            vec3 waterCol = mix(waterShallow, waterColor.rgb, depth);

            // fwidth AA on ripple contour lines
            float cf = h2 * 10.0;
            float fw = fwidth(cf);
            float contour = 1.0 - smoothstep(fw * 0.3, fw * 1.8, abs(fract(cf + 0.5) - 0.5) * 2.0);

            col = waterCol * (0.15 + diff * 0.85) + skyRefl * fres * 0.7;
            // HDR specular — cores bloom past 1.0
            col += specColor.rgb * (spec * 2.2 + pow(spec, 4.0) * 1.5) * audioMod;
            // Contour shimmer — HDR accent
            col += contour * vec3(0.5, 0.9, 1.3) * 0.35;
            // Bass pulse lifts wave crests into HDR
            col += max(0.0, h2 * 6.0) * specColor.rgb * audioBass * audioReact * 0.9;
        }
    } else {
        // Sky background
        float sy = clamp(rd.y * 2.0, 0.0, 1.0);
        col = mix(vec3(0.02, 0.03, 0.08), vec3(0.25, 0.45, 0.85), sy);
        col += vec3(1.0, 0.9, 0.7) * pow(max(dot(rd, keyDir), 0.0), 20.0) * 1.2;
    }

    // Linear HDR output — host applies ACES
    gl_FragColor = vec4(col, 1.0);
}
