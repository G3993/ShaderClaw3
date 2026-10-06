/*{
  "DESCRIPTION": "3D Color Prints — bold rotating cubes and spheres in a riso-poster color world. Raymarched SDF scene with moon-phase disc backgrounds, rainbow gradient bars, audio-reactive bass pumping, bloom, chromatic aberration, and paper grain.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "gridN",       "LABEL": "Grid Cells",    "TYPE": "float", "MIN": 4.0,  "MAX": 14.0, "DEFAULT": 7.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "shapeMix",    "LABEL": "Bars vs Discs", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Shape / Geometry" },
    { "NAME": "embossAmt",   "LABEL": "Print Emboss",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Shape / Geometry" },
    { "NAME": "cubeCount",   "LABEL": "3D Object Count","TYPE": "float","MIN": 1.0,  "MAX": 12.0, "DEFAULT": 6.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "objScale",    "LABEL": "Object Scale",  "TYPE": "float", "MIN": 0.02, "MAX": 0.25, "DEFAULT": 0.09, "GROUP": "Shape / Geometry" },
    { "NAME": "morphSpeed",  "LABEL": "Morph Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Motion / Animation" },
    { "NAME": "pourSpeed",   "LABEL": "Gradient Pour", "TYPE": "float", "MIN": -2.0, "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Motion / Animation" },
    { "NAME": "motionSpeed", "LABEL": "Motion Speed",  "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "spinSpeed",   "LABEL": "Spin Speed",    "TYPE": "float", "MIN": 0.0,  "MAX": 4.0,  "DEFAULT": 1.2,  "GROUP": "Motion / Animation" },
    { "NAME": "hueDrift",    "LABEL": "Hue Drift",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Color" },
    { "NAME": "brightness",  "LABEL": "Brightness",    "TYPE": "float", "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",    "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.08, "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",    "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Depth / Passes" },
    { "NAME": "aberration",  "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.28, "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

#define R RENDERSIZE.xy
#define TAU 6.2831853
#define PI  3.1415926

// ── helpers ────────────────────────────────────────────────────────────────
float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}
float hash11(float f){ return fract(sin(f * 127.1) * 43758.5453); }

vec3 pal(float h) {
    return clamp(vec3(
        0.55 + 0.55 * cos(TAU * (h + 0.00)),
        0.50 + 0.55 * cos(TAU * (h + 0.67)),
        0.50 + 0.55 * cos(TAU * (h + 0.33))), 0.0, 1.0);
}

// ── rotation matrices ──────────────────────────────────────────────────────
mat3 rotX(float a){ float c=cos(a),s=sin(a); return mat3(1.,0.,0., 0.,c,-s, 0.,s,c); }
mat3 rotY(float a){ float c=cos(a),s=sin(a); return mat3(c,0.,s,  0.,1.,0., -s,0.,c); }
mat3 rotZ(float a){ float c=cos(a),s=sin(a); return mat3(c,-s,0., s,c,0.,  0.,0.,1.); }

// ── SDFs ───────────────────────────────────────────────────────────────────
float sdSphere(vec3 p, float r){ return length(p) - r; }
float sdBox(vec3 p, vec3 b){
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}
float sdRoundBox(vec3 p, vec3 b, float r){
    vec3 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
}

// ── 3-D scene ──────────────────────────────────────────────────────────────
// Packs: dist, object-index (0=cube,1=sphere,2=torus), hue-seed
struct Hit { float d; float idx; float hue; };

Hit sceneHit(vec3 ro, vec3 rd, float t, float bassP, float amt) {
    float minD = 1e9;
    float minIdx = 0.0;
    float minHue = 0.0;
    int N = int(clamp(cubeCount, 1.0, 12.0));
    float sc = objScale * (1.0 + amt * 0.18 * bassP);

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        // orbit parameters
        float orbitR   = 0.28 + hash11(fi * 1.3) * 0.32;
        float orbitSpd = (0.4 + hash11(fi * 2.7) * 0.8) * spinSpeed;
        float orbitOff = hash11(fi * 4.1) * TAU;
        float tiltY    = (hash11(fi * 5.3) - 0.5) * 1.2;
        float tiltZ    = (hash11(fi * 6.1) - 0.5) * 0.8;
        float angT     = t * orbitSpd + orbitOff;

        vec3 center = vec3(cos(angT) * orbitR, sin(angT * 0.73 + tiltY) * 0.24, sin(angT) * orbitR * sin(tiltZ + 0.4));
        // bob vertically
        center.y += sin(t * 0.55 + fi * 1.1) * 0.07;

        // object-local rotation
        float rx = t * (0.6 + hash11(fi * 3.3) * 0.8) * spinSpeed + fi;
        float ry = t * (0.4 + hash11(fi * 7.7) * 0.9) * spinSpeed + fi * 0.7;
        vec3 lp = ro + rd * 0.0 - center; // will raymarch below — store center
        // We'll compute the actual SDF inline during raymarch, so here just store params.
        // Actually: march along ray analytically to bounding sphere first.
        vec3 oc = ro - center;
        float b2 = dot(oc, rd);
        float c2 = dot(oc, oc) - (sc * 2.8) * (sc * 2.8);
        float disc2 = b2 * b2 - c2;
        if (disc2 < 0.0) continue;
        float tNear = -b2 - sqrt(disc2);
        float tFar  = -b2 + sqrt(disc2);
        if (tFar < 0.001) continue;
        float tS = max(tNear, 0.001);

        // mini-march inside bounding sphere
        float ot = tS;
        float od = 1e9;
        for (int s = 0; s < 28; s++) {
            vec3 p = ro + rd * ot - center;
            // rotate local point
            p = rotX(rx) * rotY(ry) * p;
            float type = hash11(fi * 8.8);
            float d;
            if (type < 0.45) {
                d = sdRoundBox(p, vec3(sc), sc * 0.18);
            } else if (type < 0.78) {
                d = sdSphere(p, sc * 1.05);
            } else {
                // rounded flat disc / torus
                vec2 q2 = vec2(length(p.xz) - sc * 0.9, p.y);
                d = length(q2) - sc * 0.28;
            }
            if (d < 0.0015) { od = d; break; }
            if (ot > tFar + 0.01) break;
            ot += max(d * 0.85, 0.001);
            od = d;
        }
        if (od < 0.005 && ot < minD) {
            minD = ot;
            minIdx = hash11(fi * 8.8) < 0.45 ? 0.0 : (hash11(fi * 8.8) < 0.78 ? 1.0 : 2.0);
            minHue = hash11(fi * 9.1);
        }
    }
    Hit h; h.d = minD; h.idx = minIdx; h.hue = minHue;
    return h;
}

// Normal via finite differences (cheaper, avoids re-march cost)
vec3 sceneNormal(vec3 p_world, vec3 center, float rx, float ry, float sc, float type) {
    vec2 e = vec2(0.001, 0.0);
    // local rotate helper inline
    vec3 lp = rotX(rx) * rotY(ry) * (p_world - center);
    float d;
    vec3 n;
    if (type < 0.45) {
        vec3 px = rotX(rx)*rotY(ry)*(p_world+e.xyy-center); vec3 mx = rotX(rx)*rotY(ry)*(p_world-e.xyy-center);
        vec3 py = rotX(rx)*rotY(ry)*(p_world+e.yxy-center); vec3 my = rotX(rx)*rotY(ry)*(p_world-e.yxy-center);
        vec3 pz = rotX(rx)*rotY(ry)*(p_world+e.yyx-center); vec3 mz = rotX(rx)*rotY(ry)*(p_world-e.yyx-center);
        n = vec3(
            sdRoundBox(px,vec3(sc),sc*0.18) - sdRoundBox(mx,vec3(sc),sc*0.18),
            sdRoundBox(py,vec3(sc),sc*0.18) - sdRoundBox(my,vec3(sc),sc*0.18),
            sdRoundBox(pz,vec3(sc),sc*0.18) - sdRoundBox(mz,vec3(sc),sc*0.18));
    } else if (type < 0.78) {
        n = normalize(p_world - center);
        return n;
    } else {
        vec3 lpx = rotX(rx)*rotY(ry)*(p_world+e.xyy-center);
        vec3 lmx = rotX(rx)*rotY(ry)*(p_world-e.xyy-center);
        vec3 lpy = rotX(rx)*rotY(ry)*(p_world+e.yxy-center);
        vec3 lmy = rotX(rx)*rotY(ry)*(p_world-e.yxy-center);
        vec3 lpz = rotX(rx)*rotY(ry)*(p_world+e.yyx-center);
        vec3 lmz = rotX(rx)*rotY(ry)*(p_world-e.yyx-center);
        vec2 q1,q2;
        q1=vec2(length(lpx.xz)-sc*0.9,lpx.y); q2=vec2(length(lmx.xz)-sc*0.9,lmx.y);
        float dx=length(q1)-sc*0.28-(length(q2)-sc*0.28);
        q1=vec2(length(lpy.xz)-sc*0.9,lpy.y); q2=vec2(length(lmy.xz)-sc*0.9,lmy.y);
        float dy=length(q1)-sc*0.28-(length(q2)-sc*0.28);
        q1=vec2(length(lpz.xz)-sc*0.9,lpz.y); q2=vec2(length(lmz.xz)-sc*0.9,lmz.y);
        float dz=length(q1)-sc*0.28-(length(q2)-sc*0.28);
        n = vec3(dx,dy,dz);
    }
    return normalize(n);
}

// ── full 3-D render (returns color + alpha) ────────────────────────────────
vec4 render3D(vec2 uv, float t, float bassP, float amt) {
    // camera: slightly angled, pulls back with bass
    vec3 ro = vec3(0.0, 0.18, 1.1 - amt * 0.08 * bassP);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(ri, fw);
    vec2 sc2 = (uv * 2.0 - 1.0) * vec2(R.x / R.y, 1.0);
    vec3 rd = normalize(fw + ri * sc2.x * 0.85 + up * sc2.y * 0.85);

    int N = int(clamp(cubeCount, 1.0, 12.0));
    float sc = objScale * (1.0 + amt * 0.18 * bassP);

    float minT = 1e9;
    vec3 hitCol = vec3(0.0);
    float hitAlpha = 0.0;

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        float orbitR   = 0.28 + hash11(fi * 1.3) * 0.32;
        float orbitSpd = (0.4 + hash11(fi * 2.7) * 0.8) * spinSpeed;
        float orbitOff = hash11(fi * 4.1) * TAU;
        float tiltY    = (hash11(fi * 5.3) - 0.5) * 1.2;
        float tiltZ    = (hash11(fi * 6.1) - 0.5) * 0.8;
        float angT     = t * orbitSpd + orbitOff;
        vec3 center = vec3(cos(angT)*orbitR, sin(angT*0.73+tiltY)*0.24 + sin(t*0.55+fi*1.1)*0.07, sin(angT)*orbitR*sin(tiltZ+0.4));

        float rx = t*(0.6+hash11(fi*3.3)*0.8)*spinSpeed+fi;
        float ry = t*(0.4+hash11(fi*7.7)*0.9)*spinSpeed+fi*0.7;
        float type = hash11(fi*8.8);

        // bounding sphere cull
        vec3 oc = ro - center;
        float b2 = dot(oc, rd);
        float c2 = dot(oc, oc) - (sc*2.8)*(sc*2.8);
        float disc2 = b2*b2 - c2;
        if (disc2 < 0.0) continue;
        float tNear = -b2 - sqrt(disc2);
        float tFar  = -b2 + sqrt(disc2);
        if (tFar < 0.001) continue;
        float ot = max(tNear, 0.001);

        // mini raymarch
        float od = 1e9;
        for (int s = 0; s < 32; s++) {
            vec3 p = rotX(rx)*rotY(ry)*(ro + rd*ot - center);
            float d;
            if (type < 0.45) {
                d = sdRoundBox(p, vec3(sc), sc*0.18);
            } else if (type < 0.78) {
                d = sdSphere(p, sc*1.05);
            } else {
                vec2 q2 = vec2(length(p.xz)-sc*0.9, p.y);
                d = length(q2)-sc*0.28;
            }
            od = d;
            if (d < 0.0015 || ot > tFar+0.01) break;
            ot += max(d*0.8, 0.0008);
        }
        if (od < 0.004 && ot < minT) {
            minT = ot;
            vec3 hitP = ro + rd*ot;
            vec3 nrm = sceneNormal(hitP, center, rx, ry, sc, type);

            // riso ink shading
            float hue = hash11(fi*9.1) + hueDrift*(0.12*t + fi*0.17);
            vec3 baseC = pal(hue);
            vec3 baseC2 = pal(hue + 0.15);

            // two-tone ramp across normal
            float rim = dot(nrm, -rd);
            vec3 objC = mix(baseC2, baseC, clamp(rim*1.3, 0.0, 1.0));

            // specular highlight (sharp — looks like printed metallic ink)
            vec3 lightDir = normalize(vec3(0.6, 1.0, 0.5));
            float diff = clamp(dot(nrm, lightDir), 0.0, 1.0);
            vec3 halfV = normalize(lightDir - rd);
            float spec = pow(clamp(dot(nrm, halfV), 0.0, 1.0), 28.0);

            objC = objC * (0.35 + 0.65*diff) + vec3(spec)*0.55;
            objC *= 1.0 + amt*0.12*bassP;

            // fresnel edge glow
            float fr = pow(1.0 - clamp(rim, 0.0, 1.0), 3.0);
            objC += pal(hue+0.5) * fr * 0.4;

            hitCol = objC;
            hitAlpha = 1.0;
        }
    }
    return vec4(hitCol, hitAlpha);
}

// ── flat background scene (riso grid) ─────────────────────────────────────
vec3 renderBG() {
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float asp = R.x / R.y;

    float amt   = audioReact;
    float bassP = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    float midP  = pow(smoothstep(0.06, 0.85, audioMid),  1.2);
    float highP = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);

    float t = TIME * motionSpeed;

    vec3 paper = vec3(0.93, 0.92, 0.89);
    float grain = hash21(fc * 0.7) - 0.5;
    paper += grain * 0.03;

    vec2 m = vec2(0.05/asp, 0.05);
    vec2 inUV = (uv - m) / (1.0 - 2.0*m);
    float inside = step(0.0,inUV.x)*step(inUV.x,1.0)*step(0.0,inUV.y)*step(inUV.y,1.0);

    float N = floor(gridN + 0.5);
    vec2 g = inUV * vec2(N*min(asp,1.6), N);
    vec2 id = floor(g);
    vec2 lp = fract(g) - 0.5;
    float aa = 2.0*N/R.y;

    float h  = hash21(id + 7.3);
    float h2 = hash21(id*1.7 + 3.1);

    vec2 quad = step(vec2(0.5), inUV);
    float qi = quad.x + quad.y*2.0;
    vec3 plate = pal(qi*0.23 + 0.05 + hueDrift*0.2*sin(t*0.05));
    plate = mix(plate, paper, 0.45);

    float baseHue = h*0.9 + hueDrift*(0.08*t + 0.2*sin(t*0.11));
    float clk = t*morphSpeed*0.35 + (id.x+id.y)*0.11 + amt*0.25*midP;
    float phase = fract(clk + h2*0.3);

    vec3 col = plate;
    float isBar = step(1.0 - shapeMix, hash21(id*3.7 + 11.0));

    if (isBar > 0.5) {
        float bw = 0.36;
        float bar = smoothstep(bw+aa, bw-aa, abs(lp.x)) * smoothstep(0.48+aa, 0.48-aa, abs(lp.y));
        float pour = lp.y*1.3 + t*pourSpeed*0.5 + h*4.0 + amt*0.4*midP;
        vec3 barC = pal(baseHue + pour*0.35);
        barC *= (0.85 + 0.25*cos(lp.x/bw*1.57)) * (1.0 + amt*0.14*clamp(audioLevel,0.0,1.0));
        col = mix(col, barC, bar);
    } else {
        float rad = 0.40*(1.0 + amt*0.10*bassP);
        float d   = length(lp);
        float disc = smoothstep(rad+aa, rad-aa, d);
        float ph   = phase*TAU;
        float cutX = cos(ph)*rad*2.2;
        float dcut = length(lp - vec2(cutX, 0.0));
        float cut  = smoothstep(rad+aa, rad-aa, dcut);
        float moon = clamp(disc - cut*0.94, 0.0, 1.0);
        vec3 discC = pal(baseHue);
        discC *= 0.80 + 0.28*smoothstep(rad, rad*0.2, d);
        discC = mix(discC, pal(baseHue+0.14), clamp(lp.y+0.5,0.0,1.0)*0.55);
        discC *= 1.0 + amt*0.14*clamp(audioLevel,0.0,1.0);
        col = mix(col, discC, moon);
        float rim = smoothstep(rad, rad-0.05, d) - smoothstep(rad-0.10, rad-0.15, d);
        col += pal(baseHue+0.5) * max(rim,0.0) * amt * clamp(audioBeatPulse,0.0,1.0) * 0.35;
    }

    // emboss
    vec2 eg = vec2(
        smoothstep(0.5,0.42,abs(lp.x+0.04)) - smoothstep(0.5,0.42,abs(lp.x-0.04)),
        smoothstep(0.5,0.42,abs(lp.y+0.04)) - smoothstep(0.5,0.42,abs(lp.y-0.04)));
    float ex = smoothstep(0.5,0.42,abs(lp.x))*smoothstep(0.5,0.42,abs(lp.y));
    col *= 1.0 + embossAmt*0.10*(eg.x+eg.y);
    col = mix(col, col*(0.94+0.06*ex), embossAmt);

    col = mix(paper, col, inside);
    col += grain*(0.035 + amt*0.03*highP);
    return col;
}

// ── main ───────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = gl_FragCoord.xy / R;

    if (PASSINDEX == 0) {
        float amt   = audioReact;
        float bassP = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
        float t     = TIME * motionSpeed;

        vec3 bg  = renderBG();
        vec4 obj = render3D(uv, t * spinSpeed * 0.35, bassP, amt);

        // composite: objects over background with subtle shadow
        float shadow = obj.a * 0.22;
        vec3 col = mix(bg * (1.0 - shadow), obj.rgb, obj.a);

        // trail feedback
        vec3 prev  = texture2D(abTrail, uv).rgb;
        float decay = 0.50 + 0.46*trailAmt;
        col = max(col, prev*decay - 0.004);
        col += (hash21(gl_FragCoord.xy + fract(TIME)*61.0) - 0.5)*0.006;

        gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);

    } else {
        // pass 1: bloom + chromatic aberration + final grade
        vec2 dir = uv - 0.5;
        float ab = aberration * 0.005;
        vec3 base;
        base.r = texture2D(abTrail, uv + dir*ab).r;
        base.g = texture2D(abTrail, uv).g;
        base.b = texture2D(abTrail, uv - dir*ab).b;

        vec3 bl = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float an = float(i)*0.7853982;
            vec2 o = vec2(cos(an),sin(an))*(3.5/R.y);
            bl += texture2D(abTrail, uv + o).rgb;
            bl += texture2D(abTrail, uv + o*2.6).rgb*0.6;
        }
        bl /= 12.8;
        bl = max(bl - 0.50, 0.0);

        vec3 col = base + bl*bl*bloomAmt*2.0;

        float lvl = clamp(audioLevel, 0.0, 1.0);
        col *= brightness * mix(1.0, 0.64+0.36*lvl, audioReact)
             * (1.0 + audioReact*0.07*clamp(audioBeatPulse,0.0,1.0));

        col = clamp(col, 0.0, 1.0);
        col = mix(col, col*col*(3.0-2.0*col), 0.55);
        float lum = dot(col, vec3(0.299,0.587,0.114));
        col = clamp(mix(vec3(lum), col, 1.25), 0.0, 1.0);

        gl_FragColor = vec4(col, 1.0);
    }
}