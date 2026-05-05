/*{
    "DESCRIPTION": "Electric Torus Knot — raymarched p(2,3) torus knot SDF with neon iso-surface edge glow. HDR saturated palette on black velvet.",
    "CREDIT": "ShaderClaw auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "knotScale",  "LABEL": "Knot Scale",   "TYPE": "float", "DEFAULT": 0.85, "MIN": 0.4,  "MAX": 1.5 },
        { "NAME": "tubeRadius", "LABEL": "Tube Radius",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.04, "MAX": 0.40 },
        { "NAME": "isoLines",   "LABEL": "Iso Lines",    "TYPE": "float", "DEFAULT": 6.0,  "MIN": 1.0,  "MAX": 20.0 },
        { "NAME": "rotSpeed",   "LABEL": "Rotate Speed", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0,  "MAX": 1.5 },
        { "NAME": "glowPeak",   "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 5.0 },
        { "NAME": "audioReact", "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

// ─────────────────────────────────────────────────────────────────────────────
// 4-color neon palette (fully saturated, NO white mixing)
// violet, cyan, gold, magenta
// ─────────────────────────────────────────────────────────────────────────────
vec3 neonPalette(float t){
    t = fract(t);
    if(t < 0.25) return mix(vec3(0.6,0.0,1.0), vec3(0.0,1.0,1.0), t*4.0);
    if(t < 0.50) return mix(vec3(0.0,1.0,1.0), vec3(1.0,0.8,0.0), (t-0.25)*4.0);
    if(t < 0.75) return mix(vec3(1.0,0.8,0.0), vec3(1.0,0.0,0.8), (t-0.50)*4.0);
    return mix(vec3(1.0,0.0,0.8), vec3(0.6,0.0,1.0), (t-0.75)*4.0);
}

// ─────────────────────────────────────────────────────────────────────────────
// Torus knot SDF — p=2, q=3
// ─────────────────────────────────────────────────────────────────────────────
float sdTorusKnot(vec3 p, float scale, float r){
    p /= scale;
    // Parameterize by angle phi around the knot axis
    float phi = atan(p.z, p.x);
    // Approximate nearest point on the p(2,3) knot curve
    vec3 bestPt = vec3(0.0);
    float bestD = 1e9;
    for(int i = 0; i < 64; i++){
        float t = (float(i) + 0.5) / 64.0 * 6.28318;
        float R = 1.0 + 0.5 * cos(3.0 * t);
        vec3 kp = vec3(R * cos(2.0 * t), 0.5 * sin(3.0 * t), R * sin(2.0 * t));
        float d = length(p - kp);
        if(d < bestD){ bestD = d; bestPt = kp; }
    }
    return (bestD - r) * scale;
}

// ─────────────────────────────────────────────────────────────────────────────
// Rotate helper
// ─────────────────────────────────────────────────────────────────────────────
vec3 rotY(vec3 p, float a){ float c=cos(a),s=sin(a); return vec3(c*p.x+s*p.z, p.y, -s*p.x+c*p.z); }
vec3 rotX(vec3 p, float a){ float c=cos(a),s=sin(a); return vec3(p.x, c*p.y-s*p.z, s*p.y+c*p.z); }

// ─────────────────────────────────────────────────────────────────────────────
// Main
// ─────────────────────────────────────────────────────────────────────────────
void main(){
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel + audioBass * 0.8) * audioReact;

    // Camera orbit
    float ang = TIME * rotSpeed;
    vec3 ro = rotY(vec3(0.0, 0.8, 4.2), ang * 0.7);
    ro = rotX(ro, sin(TIME * 0.19) * 0.25);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

    float scale = knotScale * (1.0 + audioBass * audioReact * 0.08);
    float r     = tubeRadius * (1.0 + audioLevel * audioReact * 0.12);

    // Raymarch
    float dist = 0.0;
    float hit  = 0.0;
    vec3  hp   = ro;
    for(int i = 0; i < 72; i++){
        hp = ro + rd * dist;
        float d = sdTorusKnot(hp, scale, r);
        if(d < 0.002){ hit = 1.0; break; }
        if(dist > 12.0) break;
        dist += max(d * 0.55, 0.005);
    }

    vec3 col = vec3(0.0);

    if(hit > 0.5){
        // Normal
        float e = 0.003;
        vec3 n = normalize(vec3(
            sdTorusKnot(hp+vec3(e,0,0), scale, r) - sdTorusKnot(hp-vec3(e,0,0), scale, r),
            sdTorusKnot(hp+vec3(0,e,0), scale, r) - sdTorusKnot(hp-vec3(0,e,0), scale, r),
            sdTorusKnot(hp+vec3(0,0,e), scale, r) - sdTorusKnot(hp-vec3(0,0,e), scale, r)
        ));

        // Iso-surface lines: rings around the tube cross-section using fwidth() AA
        float phi = atan(hp.z, hp.x);         // angle around knot loop
        float theta = atan(hp.y, length(hp.xz));  // tube angle
        float isoT  = fract(theta / 6.28318 * isoLines + TIME * rotSpeed * 0.5);
        float fw = fwidth(isoT);
        float iso = smoothstep(fw*2.0, 0.0, min(isoT, 1.0-isoT));

        // Color from tube angle position
        float hueT = fract(phi / 6.28318 + TIME * rotSpeed * 0.15);
        vec3 neonCol = neonPalette(hueT);

        // Neon glow on iso-surface lines
        col = neonCol * iso * glowPeak * audio;

        // Surface diffuse fill (dim, lets iso lines dominate)
        vec3 lightDir = normalize(vec3(1.0, 1.5, 0.8));
        float diff = max(dot(n, lightDir), 0.0);
        col += neonCol * diff * 0.3;

        // Specular peak (HDR white)
        float spec = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 32.0);
        col += vec3(1.0) * spec * 2.0 * audio;

        // Ink-black depth fade for contrast
        float depth = 1.0 - clamp((dist - 2.0) / 8.0, 0.0, 1.0);
        col *= depth * depth;
    } else {
        // Background: black velvet with faint neon haze at edges
        float rim = length(uv) * 0.35;
        vec3 hazeCol = neonPalette(TIME * 0.07 + rim * 0.3);
        col = hazeCol * exp(-rim * rim * 3.0) * 0.15;
    }

    gl_FragColor = vec4(col, 1.0);
}
