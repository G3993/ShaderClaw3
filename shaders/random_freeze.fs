/*{
  "DESCRIPTION": "Magma Crater Floor — 3D raymarched volcanic crater, procedural obsidian cracks with glowing magma veins, wide environmental overhead scene. Warm palette.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "flowSpeed",   "LABEL": "Lava Flow",    "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "crackScale",  "LABEL": "Crack Scale",  "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "glowWidth",   "LABEL": "Glow Width",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.02,"MAX": 0.40 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0, "MAX": 6.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────────
// Hashing
// ─────────────────────────────────────────────────────────────────────────────
float hash12(vec2 p){
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}
vec2 hash22(vec2 p){
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031,0.1030,0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

// ─────────────────────────────────────────────────────────────────────────────
// Voronoi crack pattern — returns distance to nearest cell edge
// ─────────────────────────────────────────────────────────────────────────────
float voronoiCrack(vec2 uv, float scale){
    uv *= scale;
    vec2 i = floor(uv);
    vec2 f = fract(uv);
    float minD1 = 8.0, minD2 = 8.0;
    for(int y=-1;y<=1;y++) for(int x=-1;x<=1;x++){
        vec2 nb = vec2(float(x), float(y));
        vec2 pt = nb + hash22(i + nb) - f;
        float d = dot(pt, pt);
        if(d < minD1){ minD2 = minD1; minD1 = d; }
        else if(d < minD2){ minD2 = d; }
    }
    // Distance to crack edge = difference between two nearest
    return sqrt(minD2) - sqrt(minD1);
}

// ─────────────────────────────────────────────────────────────────────────────
// FBM for height variation
// ─────────────────────────────────────────────────────────────────────────────
float noise(vec2 p){
    vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
    return mix(mix(hash12(i),hash12(i+vec2(1,0)),f.x),
               mix(hash12(i+vec2(0,1)),hash12(i+vec2(1,1)),f.x),f.y);
}
float fbm(vec2 p){
    float v=0.0,a=0.5;
    for(int i=0;i<5;i++){ v+=a*noise(p); p*=2.03; a*=0.5; }
    return v;
}

// ─────────────────────────────────────────────────────────────────────────────
// Scene: flat crater floor plane displaced upward by fbm
// ─────────────────────────────────────────────────────────────────────────────
float sceneSDF(vec3 p){
    float t = TIME * flowSpeed;
    vec2 warp = vec2(fbm(p.xz * 1.3 + t * 0.2 + 7.7),
                     fbm(p.xz * 1.3 + t * 0.2 + 3.1)) - 0.5;
    float height = fbm((p.xz + warp * 0.4) * 1.0) * 0.35 - 0.08;
    return p.y - height;
}

// ─────────────────────────────────────────────────────────────────────────────
// Magma palette: obsidian → deep crimson → orange → gold → white-hot
// ─────────────────────────────────────────────────────────────────────────────
vec3 magmaPalette(float t){
    t = clamp(t, 0.0, 1.0);
    if(t < 0.35) return mix(vec3(0.02,0.0,0.0),  vec3(0.55,0.0,0.0),  t/0.35);
    if(t < 0.65) return mix(vec3(0.55,0.0,0.0),  vec3(1.0, 0.35,0.0), (t-0.35)/0.30);
    if(t < 0.85) return mix(vec3(1.0, 0.35,0.0), vec3(1.0, 0.82,0.0), (t-0.65)/0.20);
    return mix(vec3(1.0,0.82,0.0), vec3(1.0,1.0,0.9), (t-0.85)/0.15);
}

// ─────────────────────────────────────────────────────────────────────────────
void main(){
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel + audioBass * 0.7) * audioReact;

    // Camera looking down at crater floor from above-and-slightly-to-side
    float camAng = TIME * 0.05;
    vec3 ro = vec3(sin(camAng)*0.4, 2.5, cos(camAng)*0.4 + 0.2);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0,1.0,0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right * 0.9 + uv.y * up * 0.9);

    // Raymarch
    float dist=0.0, hit=0.0;
    vec3 hp = ro;
    for(int i=0;i<64;i++){
        hp = ro + rd * dist;
        float d = sceneSDF(hp);
        if(d < 0.003){ hit=1.0; break; }
        if(dist > 6.0) break;
        dist += d * 0.7;
    }

    vec3 col = vec3(0.0);

    if(hit > 0.5){
        float e = 0.002;
        vec3 n = normalize(vec3(
            sceneSDF(hp+vec3(e,0,0))-sceneSDF(hp-vec3(e,0,0)),
            sceneSDF(hp+vec3(0,e,0))-sceneSDF(hp-vec3(0,e,0)),
            sceneSDF(hp+vec3(0,0,e))-sceneSDF(hp-vec3(0,0,e))
        ));

        float t = TIME * flowSpeed;

        // Voronoi crack pattern — crack distance drives lava emission
        float crack = voronoiCrack(hp.xz + vec2(sin(t*0.3)*0.05, cos(t*0.25)*0.05),
                                   crackScale);

        // Glow along crack edges: inverse of crack distance → magma color
        float glowT = clamp(1.0 - crack / glowWidth, 0.0, 1.0);
        glowT = pow(glowT, 1.5);

        // Animated lava flow within cracks
        float flow = fbm(hp.xz * crackScale * 0.8 + vec2(t * 0.6, t * 0.4)) * 0.5 + 0.5;
        float lavaT = glowT * (0.5 + flow * 0.5);

        // Obsidian surface base color (very dark, rocky)
        float rockNoise = fbm(hp.xz * 8.0) * 0.5 + 0.5;
        vec3 rockCol = vec3(0.02, 0.01, 0.01) * (0.7 + rockNoise * 0.3);

        // Magma glow color
        float magmaHeat = lavaT * (0.6 + audioBass * audioReact * 0.4);
        vec3 lavaCol = magmaPalette(magmaHeat) * hdrPeak * audio;

        // fwidth AA on crack edge
        float fw = fwidth(crack);
        float crackAA = smoothstep(fw*2.0, 0.0, crack - glowWidth * 0.8);

        col = mix(rockCol, lavaCol, crackAA + glowT * 0.8);

        // Surface normal diffuse + rim
        vec3 lightDir = normalize(vec3(0.5, 1.0, 0.3));
        float diff = max(dot(n, lightDir), 0.0) * 0.4;
        col += rockCol * diff * (1.0 - glowT);

        // White-hot specular on brightest magma
        float spec = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 12.0);
        col += vec3(1.0, 0.9, 0.7) * spec * glowT * hdrPeak * audio;
    } else {
        // Sky above crater: deep red-orange smoke at horizon
        float h = uv.y * 0.5 + 0.5;
        col = mix(vec3(0.18, 0.04, 0.0), vec3(0.04, 0.01, 0.01), h);
    }

    gl_FragColor = vec4(col, 1.0);
}
