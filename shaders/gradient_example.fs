/*{
  "DESCRIPTION": "Gradient Nebula — raymarched 3D volumetric fog with cinematic lighting. Three-color palette, audio reactive. HDR linear output.",
  "CREDIT": "ShaderClaw auto-improve 2026-05-05",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "INPUTS": [
    {"NAME":"colorA","LABEL":"Color A","TYPE":"color","DEFAULT":[0.91,0.25,0.34,1.0]},
    {"NAME":"colorB","LABEL":"Color B","TYPE":"color","DEFAULT":[0.20,0.50,1.00,1.0]},
    {"NAME":"colorC","LABEL":"Color C","TYPE":"color","DEFAULT":[1.00,0.70,0.15,1.0]},
    {"NAME":"speed","LABEL":"Speed","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.4},
    {"NAME":"density","LABEL":"Density","TYPE":"float","MIN":0.1,"MAX":3.0,"DEFAULT":1.2},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float hash3(vec3 p) {
    return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453);
}

float noise(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n000 = hash3(i),           n100 = hash3(i + vec3(1,0,0));
    float n010 = hash3(i+vec3(0,1,0)), n110 = hash3(i+vec3(1,1,0));
    float n001 = hash3(i+vec3(0,0,1)), n101 = hash3(i+vec3(1,0,1));
    float n011 = hash3(i+vec3(0,1,1)), n111 = hash3(i+vec3(1,1,1));
    return mix(mix(mix(n000,n100,f.x), mix(n010,n110,f.x), f.y),
               mix(mix(n001,n101,f.x), mix(n011,n111,f.x), f.y), f.z);
}

float fbm(vec3 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p = p * 2.1 + vec3(31.9, 17.3, 5.7);
        a *= 0.5;
    }
    return v;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * speed;
    float audioMod = 0.5 + 0.5 * audioLevel * audioReact;

    // Slow cinematic flythrough
    vec3 ro = vec3(cos(t * 0.17) * 1.5, sin(t * 0.13) * 0.6, t * 0.5 + 3.0);
    vec3 ta = ro + vec3(cos(t * 0.25) * 0.4, sin(t * 0.21) * 0.3, 2.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(vec3(0,1,0), ww));
    vec3 vv = cross(ww, uu);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.6 * ww);

    vec3 col = vec3(0.0);
    float transmit = 1.0;
    float dz = 0.15;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * (float(i) * dz + 0.05);
        float f1 = fbm(p * 0.55 + vec3(0.0, 0.0, t * 0.08));
        float f2 = fbm(p * 1.1  - vec3(t * 0.06, t * 0.04, 0.0));
        float dens = max(0.0, f1 * f2 * density * audioMod - 0.08);
        if (dens > 0.001) {
            float b1 = 0.5 + 0.5 * sin(p.x * 0.65 + t * 0.28);
            float b2 = 0.5 + 0.5 * sin(p.y * 0.55 - t * 0.19 + 1.57);
            vec3 c = mix(colorA.rgb, colorB.rgb, b1);
            c = mix(c, colorC.rgb, b2 * 0.5);
            float emit = dens * (1.8 + f2 * 3.0);
            // HDR: dense nebula cores push well past 1.0
            col += transmit * c * emit * dz * (1.0 + dens * 4.0);
            transmit *= exp(-dens * dz * 0.9);
            if (transmit < 0.01) break;
        }
    }

    // Starfield — HDR white dots
    vec2 sg = uv * 40.0 + vec2(t * 0.04, -t * 0.02);
    vec2 si = floor(sg), sf = fract(sg) - 0.5;
    float sh = hash3(vec3(si, 1.0));
    float sr = 0.015 + sh * 0.025;
    float star = smoothstep(sr, sr * 0.15, length(sf));
    col += transmit * star * (0.8 + 0.2 * sin(TIME * 2.7 + sh * 6.28)) * vec3(0.9, 0.95, 1.0) * 2.2;

    // HDR lift — dense peaks bloom past 1.0
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col += max(0.0, lum - 0.7) * colorA.rgb * 0.7;
    col *= 1.0 + audioBass * audioReact * 0.4;

    // Linear HDR output — host applies ACES
    gl_FragColor = vec4(col, 1.0);
}
