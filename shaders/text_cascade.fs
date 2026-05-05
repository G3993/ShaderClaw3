/*{
    "DESCRIPTION": "Bioluminescent Tunnel — 3D raymarched dark cave tunnel with glowing bioluminescent orb formations on walls. Deep ocean palette: void black, bio-cyan, violet, deep teal.",
    "CATEGORIES": ["Generator", "3D", "Bioluminescent", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "tunnelRadius", "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.5, "MAX": 3.0,  "LABEL": "Tunnel Radius" },
        { "NAME": "orbDensity",   "TYPE": "float", "DEFAULT": 12.0, "MIN": 2.0, "MAX": 24.0, "LABEL": "Orb Density" },
        { "NAME": "hdrPeak",      "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",     "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n)  { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
vec3  hash31(float n)  { return fract(sin(n*vec3(127.1,311.7,74.7))*43758.5453); }

// Tunnel SDF: infinite cylinder along Z axis
float sdTunnel(vec3 p) {
    return tunnelRadius - length(p.xy); // inside = negative
}

// Bioluminescent orbs on tunnel wall
float orbGlow(vec3 p, float t, out vec3 orbCol) {
    float minD = 1e6;
    orbCol = vec3(0.0);
    float pzCell = floor(p.z * orbDensity * 0.15);
    // Check nearby cells
    for (int k = -1; k <= 1; k++) {
        float cz = pzCell + float(k);
        for (int j = 0; j < 5; j++) {
            float seed = cz * 17.3 + float(j) * 7.91;
            // Place orb on tunnel wall: random angle + z position
            float ang = hash11(seed * 1.31) * 6.2832;
            float rOrb = tunnelRadius * 0.92; // surface of tunnel wall
            float zOrb = (cz + hash11(seed * 2.17)) / (orbDensity * 0.15);
            vec3 orbCenter = vec3(cos(ang) * rOrb, sin(ang) * rOrb, zOrb);
            float d = length(p - orbCenter);
            if (d < minD) {
                minD = d;
                vec3 hc = hash31(seed * 3.77);
                // Bio palette: cyan, violet, teal — force saturation
                if (hc.x < 0.33)      orbCol = vec3(0.0,  0.9,  1.0); // bio-cyan
                else if (hc.x < 0.66) orbCol = vec3(0.55, 0.0,  1.0); // violet
                else                   orbCol = vec3(0.0,  0.7,  0.5); // deep teal
            }
        }
    }
    return minD;
}

vec3 calcNormal(vec3 p) {
    float R = tunnelRadius - length(p.xy);
    return normalize(-vec3(p.xy, 0.0)); // outward from cylinder axis
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // Camera flying forward through tunnel
    float camZ = t * 1.2;
    vec3 ro = vec3(0.0, 0.0, camZ);
    // Slight camera sway
    ro.xy += vec2(sin(t*0.4)*0.08, cos(t*0.3)*0.06);
    vec3 fw = vec3(0.0, 0.0, 1.0);
    vec3 rgt = vec3(1.0, 0.0, 0.0);
    vec3 up_ = vec3(0.0, 1.0, 0.0);
    vec3 rd  = normalize(fw + uv.x * rgt * 0.6 + uv.y * up_ * 0.6);

    // March to tunnel wall
    float dist = 0.0;
    bool hit = false;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        float d = -sdTunnel(p); // inside tunnel, d is negative; use abs for wall
        float wallD = abs(length(p.xy) - tunnelRadius);
        if (wallD < 0.005) { hit = true; break; }
        dist += max(wallD * 0.7, 0.01);
        if (dist > 25.0) break;
    }

    vec3 col = vec3(0.0, 0.0, 0.01); // void black

    if (hit) {
        vec3 p = ro + rd * dist;
        vec3 N = calcNormal(p);

        // Cave wall: very dark teal stone
        vec3 stoneCol = vec3(0.02, 0.06, 0.07);

        // Bioluminescent orbs: glow contribution
        vec3 orbCol;
        float orbD = orbGlow(p, t, orbCol);

        // Orb glow: exponential falloff, HDR
        float glow = exp(-orbD * 5.0) * hdrPeak * audio;
        float bloom = exp(-orbD * 1.5) * hdrPeak * 0.4 * audio;

        // Pulse each orb slowly
        float pulse = 0.85 + 0.15 * sin(t * 1.7 + orbD * 3.0);

        col = stoneCol + orbCol * (glow + bloom) * pulse;

        // Depth fog: far wall gets dimmer
        float fog = exp(-dist * 0.08);
        col *= fog;
    }

    // Atmospheric glow along tunnel axis (bio-cyan mist)
    float axisGlow = exp(-length(uv) * 2.5) * 0.06;
    col += vec3(0.0, 0.6, 0.5) * axisGlow;

    gl_FragColor = vec4(col, 1.0);
}
