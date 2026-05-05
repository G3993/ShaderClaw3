/*{
  "DESCRIPTION": "Gothic Cathedral Interior — 3D raymarched vaulted nave with stained glass light beams, stone arches, and colored illumination",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "archCount",   "LABEL": "Arch Spans",   "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0, "MAX": 8.0  },
    { "NAME": "stoneColor",  "LABEL": "Stone Color",  "TYPE": "color", "DEFAULT": [0.08, 0.06, 0.1, 1.0]       },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0  },
    { "NAME": "audioPulse",  "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "walkSpeed",   "LABEL": "Walk Speed",   "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 1.0  }
  ]
}*/

// Gothic arch SDF: tall pointed arch = two overlapping circles
float sdGothicArch(vec2 p, float w, float h) {
    // Two circles centered at ±w/2, radius r such that they meet at height h
    float r = sqrt((w*0.5)*(w*0.5) + h*h) * 0.5 + w*0.15;
    float d1 = length(p - vec2(-w*0.5, 0.0)) - r;
    float d2 = length(p - vec2( w*0.5, 0.0)) - r;
    // Interior of arch = intersection of two circle interiors + box below
    float interior = max(d1, d2);    // inside both
    float box = min(abs(p.x) - w*0.5, -p.y);  // below arch crown
    return max(interior, -box);      // 2D arch opening
}

float sdBox3(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

vec2 scene(vec3 p) {
    // Walk down the nave
    float walk = TIME * walkSpeed * 0.5;
    float span  = 2.5;   // distance between arch pairs
    float navW  = 1.8;   // half-width of nave
    float navH  = 4.0;   // arch height

    // Tiled arch spans using mod
    float sz = p.z - walk;
    float cell = mod(sz, span) - span*0.5;
    float cellZ = floor(sz / span);

    // Nave walls (box)
    float wallL = abs(p.x + navW) - 0.25;
    float wallR = abs(p.x - navW) - 0.25;
    float walls = min(wallL, wallR);

    // Floor
    float floor_ = p.y + 0.05;

    // Vaulted ceiling arch (extruded in Z)
    vec2 archP = vec2(p.x, p.y - 0.05);
    float archD = -sdGothicArch(archP, navW*2.0, navH) - 0.25;
    float ceiling = max(archD, 0.04 - abs(cell)); // only in arch slab

    float d = min(walls, min(floor_, max(-(navH - p.y), abs(p.x) - navW - 0.5)));
    float matId = 0.0; // stone

    // Stained glass window openings in side walls (arched)
    vec2 winP  = vec2(p.y - navH*0.55, cell);
    float win  = sdGothicArch(winP, 0.7, 0.9) - 0.06;
    float winD = min(abs(p.x + navW) - 0.02, abs(p.x - navW) - 0.02);
    if (winD < 0.05) {
        d = max(d, -win);
    }

    return vec2(d, matId);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.004, 0.0);
    return normalize(vec3(
        scene(p+e.xyy).x - scene(p-e.xyy).x,
        scene(p+e.yxy).x - scene(p-e.yxy).x,
        scene(p+e.yyx).x - scene(p-e.yyx).x
    ));
}

// Window color: stained glass pattern based on position
vec3 windowColor(vec3 dir, float t) {
    // 5 color panels in each window
    float panel = floor(dir.x * 4.0 + 2.0);
    float hue = fract(panel * 0.19 + t * 0.05);
    // Saturated colors cycling
    if (int(mod(panel, 5.0)) == 0) return vec3(1.0, 0.0, 0.15);    // crimson
    if (int(mod(panel, 5.0)) == 1) return vec3(0.0, 0.4, 1.0);     // royal blue
    if (int(mod(panel, 5.0)) == 2) return vec3(0.0, 0.85, 0.2);    // emerald
    if (int(mod(panel, 5.0)) == 3) return vec3(1.0, 0.65, 0.0);    // amber
                                   return vec3(0.55, 0.0, 1.0);    // violet
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Walk down the nave, looking slightly upward
    float walk = TIME * walkSpeed * 0.5;
    vec3 ro = vec3(0.0, 1.4, walk);
    vec3 ta = vec3(sin(TIME*0.12)*0.4, 2.2, walk + 5.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.6*fw);

    vec3 bg = vec3(0.0, 0.0, 0.005);

    float t = 0.05; bool hit = false;
    for (int i = 0; i < 96; i++) {
        float d = scene(ro + rd*t).x;
        if (d < 0.003) { hit = true; break; }
        if (t > 30.0) break;
        t += d * 0.7;
    }

    vec3 col = bg;
    // Atmospheric light shafts from windows
    float shaft = 0.0;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float side = float(i % 2)*2.0 - 1.0;
        float wz = floor((ro.z + fi*2.5) / 2.5) * 2.5 + fi*0.3;
        vec3 src = vec3(side * 1.9, 2.8, wz + TIME * walkSpeed * 0.5);
        vec3 dv  = src - ro;
        float tcl = clamp(dot(rd, normalize(dv)), 0.0, 1.0);
        float d2  = length(dv - normalize(dv) * dot(rd, normalize(dv)) * length(dv));
        if (d2 < 0.3 && tcl > 0.1) {
            float si = int(mod(fi, 5.0));
            vec3 sc = (si == 0) ? vec3(1.0,0.2,0.05) :
                      (si == 1) ? vec3(0.0,0.4,1.0) :
                      (si == 2) ? vec3(0.0,0.85,0.2) :
                      (si == 3) ? vec3(1.0,0.65,0.0) : vec3(0.55,0.0,1.0);
            shaft += (0.3 - d2) * 4.0 * hdrPeak * (1.0 + audioBass * audioPulse * 0.4);
            col += sc * shaft * 0.08;
        }
    }

    if (hit) {
        vec3 p  = ro + rd*t;
        vec3 n  = getNormal(p);
        vec3 L  = normalize(vec3(0.2, 1.0, 0.3));

        // Stone diffuse
        float diff = max(dot(n,L), 0.0)*0.5 + 0.15;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 64.0) * 0.3;
        vec3 stone = stoneColor.rgb;

        col += stone * diff * hdrPeak * 0.5;
        col += vec3(1.0) * spec * hdrPeak * 0.3;

        // Colored light bleeding from windows onto stone
        for (int i = 0; i < 5; i++) {
            float fi = float(i);
            float side = float(i%2)*2.0 - 1.0;
            vec3 wdir = normalize(p - vec3(side*1.9, 2.8, p.z));
            float wdiff = max(dot(n, -wdir), 0.0) * 0.6;
            int si = int(mod(fi, 5.0));
            vec3 sc = (si == 0) ? vec3(1.0,0.2,0.05) :
                      (si == 1) ? vec3(0.0,0.4,1.0) :
                      (si == 2) ? vec3(0.0,0.85,0.2) :
                      (si == 3) ? vec3(1.0,0.65,0.0) : vec3(0.55,0.0,1.0);
            col += sc * wdiff * hdrPeak * 0.35 * (1.0 + audioMid * audioPulse * 0.3);
        }
    }

    col = mix(col, bg, clamp((t-15.0)/15.0, 0.0, 1.0)*0.8);
    gl_FragColor = vec4(col, 1.0);
}
