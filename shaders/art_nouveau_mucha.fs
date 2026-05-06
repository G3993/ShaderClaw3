/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Art Nouveau (Mucha) — 3D raymarched. Sarah-Bernhardt-as-Médée bust in profile rendered as a stylized SDF relief, wrapped by 3D twisted gold ribbon-tendrils orbiting an invisible torus, halo cartouche of floating star primitives behind the head. Studio 3-point lighting with warm gilt key, dark walnut plinth with subtle reflection. Mood enum cycles four Mucha-print pose+palette variations (Reverie 1897, Médée 1898, La Dame aux Camélias 1896, Job 1896). Audio low-band pulses halo radius; treble shimmers ribbon brass. Returns LINEAR HDR.",
  "INPUTS": [
    { "NAME": "mood",          "LABEL": "Mood",            "TYPE": "long",  "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Reverie 1897","Médée 1898","La Dame aux Camélias 1896","Job 1896"] },
    { "NAME": "ribbonCount",   "LABEL": "Ribbons",         "TYPE": "long",  "DEFAULT": 5, "VALUES": [3,4,5,6,8], "LABELS": ["3","4","5","6","8"] },
    { "NAME": "ribbonRadius",  "LABEL": "Ribbon Radius",   "TYPE": "float", "MIN": 0.04, "MAX": 0.16, "DEFAULT": 0.085 },
    { "NAME": "ribbonTwist",   "LABEL": "Ribbon Twist",    "TYPE": "float", "MIN": 0.0,  "MAX": 6.0,  "DEFAULT": 2.4 },
    { "NAME": "starCount",     "LABEL": "Halo Stars",      "TYPE": "long",  "DEFAULT": 12, "VALUES": [0,6,8,12,16,24], "LABELS": ["Off","6","8","12","16","24"] },
    { "NAME": "haloRadius",    "LABEL": "Halo Radius",     "TYPE": "float", "MIN": 0.5,  "MAX": 1.6,  "DEFAULT": 0.95 },
    { "NAME": "rotateSpeed",   "LABEL": "Rotation Speed",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.18 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "camDist",       "LABEL": "Camera Distance", "TYPE": "float", "MIN": 2.0,  "MAX": 8.0,  "DEFAULT": 3.6 },
    { "NAME": "camHeight",     "LABEL": "Camera Height",   "TYPE": "float", "MIN": -1.0, "MAX": 2.0,  "DEFAULT": 0.35 },
    { "NAME": "camOrbitSpeed", "LABEL": "Camera Orbit",    "TYPE": "float", "MIN": -0.5, "MAX": 0.5,  "DEFAULT": 0.04 },
    { "NAME": "camAzimuth",    "LABEL": "Camera Azimuth",  "TYPE": "float", "MIN": -3.1416,"MAX": 3.1416,"DEFAULT": 0.35 },
    { "NAME": "keyAngle",      "LABEL": "Key Angle",       "TYPE": "float", "MIN": -3.1416,"MAX": 3.1416,"DEFAULT": -0.7 },
    { "NAME": "keyElevation",  "LABEL": "Key Elevation",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.5708,"DEFAULT": 0.55 },
    { "NAME": "keyColor",      "LABEL": "Key Color",       "TYPE": "color", "DEFAULT": [1.45, 1.18, 0.72, 1.0] },
    { "NAME": "fillColor",     "LABEL": "Fill Color",      "TYPE": "color", "DEFAULT": [0.32, 0.36, 0.46, 1.0] },
    { "NAME": "ambient",       "LABEL": "Ambient",         "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.14 },
    { "NAME": "rimStrength",   "LABEL": "Rim Strength",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.5,  "DEFAULT": 0.85 },
    { "NAME": "exposure",      "LABEL": "Exposure",        "TYPE": "float", "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "inputTex",      "LABEL": "Texture",         "TYPE": "image" }
  ]
}*/

//  Mucha — 3D rewrite. Raymarched SDF: profile bust on plinth, 3D ribbon
//  tendrils on hidden torus, floating star halo. Studio 3-point gilt key.

#define MAX_STEPS 96
#define MAX_DIST  18.0
#define SURF_EPS  0.0008

mat2 rot2(float a){ float c=cos(a),s=sin(a); return mat2(c,-s,s,c); }
float smin(float a, float b, float k){
    float h = clamp(0.5 + 0.5*(b-a)/k, 0.0, 1.0);
    return mix(b, a, h) - k*h*(1.0-h);
}

// ─── primitive SDFs ─────────────────────────────────────────────────────
float sdSphere(vec3 p, float r){ return length(p) - r; }
float sdEllipsoid(vec3 p, vec3 r){
    float k0 = length(p/r);
    float k1 = length(p/(r*r));
    return k0*(k0-1.0)/k1;
}
float sdCapsule(vec3 p, vec3 a, vec3 b, float r){
    vec3 pa = p-a, ba = b-a;
    float h = clamp(dot(pa,ba)/dot(ba,ba), 0.0, 1.0);
    return length(pa - ba*h) - r;
}
float sdTorus(vec3 p, vec2 t){
    vec2 q = vec2(length(p.xz)-t.x, p.y);
    return length(q)-t.y;
}
float sdBox(vec3 p, vec3 b){
    vec3 d = abs(p)-b;
    return length(max(d,0.0)) + min(max(d.x,max(d.y,d.z)),0.0);
}
// 2D star -> extruded into 3D thin chip
float sdStar2D(vec2 p, float r, int n, float m){
    float an = 3.141593/float(n);
    float en = 3.141593/m;
    vec2  acs = vec2(cos(an), sin(an));
    vec2  ecs = vec2(cos(en), sin(en));
    float bn = mod(atan(p.x,p.y), 2.0*an) - an;
    p = length(p)*vec2(cos(bn), abs(sin(bn)));
    p -= r*acs;
    p += ecs*clamp(-dot(p,ecs), 0.0, r*acs.y/ecs.y);
    return length(p)*sign(p.x);
}
float sdStar3D(vec3 p, float r, float thick){
    float d2 = sdStar2D(p.xy, r, 8, 3.0);
    vec2 w = vec2(d2, abs(p.z) - thick);
    return min(max(w.x,w.y),0.0) + length(max(w,0.0));
}

// ─── palette ────────────────────────────────────────────────────────────
// Mucha print palette HDR-tuned. Mood selects pose+tint biases.
const vec3 GOLD   = vec3(2.50, 1.80, 0.60);
const vec3 CREAM  = vec3(0.92, 0.86, 0.74);
const vec3 ROSE   = vec3(0.78, 0.42, 0.45);
const vec3 TEAL   = vec3(0.18, 0.42, 0.45);
const vec3 BONE   = vec3(0.96, 0.93, 0.82);

vec3 moodTint(int m){
    if (m == 0) return TEAL  * 0.55 + ROSE * 0.10;       // Reverie
    if (m == 1) return ROSE  * 0.65 + vec3(0.10,0.0,0.0); // Médée
    if (m == 2) return CREAM * 0.70;                      // Camélias
    return GOLD * 0.30;                                   // Job
}

// ─── scene ──────────────────────────────────────────────────────────────
// Material id is packed alongside distance.
struct Hit { float d; int id; };
Hit hitMin(Hit a, Hit b){ return (a.d < b.d) ? a : b; }

// Bust in profile facing +X. Built from primitives blended with smin.
// id: 1 = bust (cream porcelain), 2 = ribbon (gold), 3 = star (gold),
//     4 = plinth (walnut), 5 = backplate (rose).
float sdBust(vec3 p, int mood){
    // Pose tweaks per mood — head tilt + chin lift.
    float tilt = (mood == 1) ?  0.18 :
                 (mood == 2) ? -0.10 :
                 (mood == 3) ?  0.05 : 0.0;
    p.yz = rot2(tilt) * p.yz;

    // Head — ellipsoid, slightly tall
    vec3 hp = p - vec3(0.04, 0.78, 0.0);
    float head = sdEllipsoid(hp, vec3(0.34, 0.40, 0.36));

    // Nose ridge — small capsule extending +X
    float nose = sdCapsule(p, vec3(0.30, 0.78, 0.0), vec3(0.40, 0.72, 0.0), 0.045);
    head = smin(head, nose, 0.05);

    // Chin / jaw — sphere blended below
    float jaw = sdSphere(p - vec3(0.18, 0.55, 0.0), 0.18);
    head = smin(head, jaw, 0.10);

    // Neck — capsule
    float neck = sdCapsule(p, vec3(0.05, 0.55, 0.0), vec3(0.0, 0.20, 0.0), 0.14);
    float bust = smin(head, neck, 0.12);

    // Shoulders / bust silhouette — wide flattened ellipsoid
    float shoulders = sdEllipsoid(p - vec3(0.0, -0.05, 0.0), vec3(0.55, 0.28, 0.40));
    bust = smin(bust, shoulders, 0.18);

    // Hair bun behind head
    float bun = sdEllipsoid(p - vec3(-0.22, 0.82, 0.0), vec3(0.26, 0.28, 0.30));
    bust = smin(bust, bun, 0.10);

    return bust;
}

float sdRibbons(vec3 p, float t, int n, float radius, float twist){
    // Tubes that wrap an invisible torus around the head, slowly rotating.
    // Sample N capsules along each ribbon's parametric path.
    vec3 c = vec3(0.04, 0.78, 0.0);    // ribbon center (head)
    vec3 rp = p - c;
    rp.xz = rot2(t) * rp.xz;

    float best = 1e9;
    for (int i = 0; i < 8; i++){
        if (i >= n) break;
        float fi = float(i);
        float phase = fi / float(n) * 6.2832;
        // SDF sample around a torus path: parameterize by toroidal angle u.
        // Approximate by snapping current point's toroidal angle then offsetting.
        float u = atan(rp.z, rp.x);
        float du = u + phase + t*0.4;
        float majR = 0.55;
        float minR = radius * (0.7 + 0.3*sin(du*twist + fi*1.7));
        // Position on torus at angle du, with poloidal twist
        vec3 ringCenter = vec3(cos(u)*majR, 0.0, sin(u)*majR);
        // Poloidal angle threads around with twist
        float v = du*twist + fi*0.9;
        vec3 offs = vec3(cos(u)*cos(v), sin(v), sin(u)*cos(v)) * minR;
        vec3 q = rp - (ringCenter + offs);
        float tube = length(q) - 0.022;
        best = min(best, tube);
    }
    return best;
}

float sdHaloStars(vec3 p, float t, int n, float radius, float audio){
    // Floating ring of small star chips, behind the head (negative X side
    // is "behind" given our profile faces +X; we'll place them on -X plane).
    vec3 c = vec3(-0.05, 0.82, 0.0);
    vec3 rp = p - c;
    // Push into a plane behind head (z plane)
    // Stars sit on a circle in the X=const plane facing camera.
    float pulse = 1.0 + 0.08 * audio * sin(t*3.0);
    float R = radius * pulse;

    float best = 1e9;
    for (int i = 0; i < 24; i++){
        if (i >= n) break;
        float fi = float(i);
        float a = fi/float(n)*6.2832 + t*0.35;
        vec3 sp = vec3(-0.05 + 0.02*sin(a*2.0+t),  // slight depth wobble behind
                        0.82 + sin(a)*R*0.55,
                        cos(a)*R);
        vec3 q = p - sp;
        // Orient star to face camera roughly (rotate around y by a)
        q.xz = rot2(-a) * q.xz;
        float bob = 0.5 + 0.5*sin(t*2.0 + fi*1.3);
        float ds = sdStar3D(q, 0.045 + 0.012*bob, 0.012);
        best = min(best, ds);
    }
    return best;
}

Hit map(vec3 p, float t, int mood, int nR, float rR, float twist,
        int nS, float hR, float audio){
    // Plinth — dark walnut box below the bust.
    float plinth = sdBox(p - vec3(0.0, -0.85, 0.0), vec3(1.0, 0.18, 0.7));
    Hit h = Hit(plinth, 4);

    // Backplate — rose-tinted rectangular cartouche behind everything.
    float back = sdBox(p - vec3(-0.4, 0.4, 0.0), vec3(0.04, 1.1, 1.1));
    h = hitMin(h, Hit(back, 5));

    // Bust
    float b = sdBust(p, mood);
    h = hitMin(h, Hit(b, 1));

    // Ribbons (gold)
    float r = sdRibbons(p, t, nR, rR, twist);
    h = hitMin(h, Hit(r, 2));

    // Halo stars
    if (nS > 0){
        float s = sdHaloStars(p, t, nS, hR, audio);
        h = hitMin(h, Hit(s, 3));
    }
    return h;
}

vec3 calcNormal(vec3 p, float t, int mood, int nR, float rR, float twist,
                int nS, float hR, float audio){
    const vec2 e = vec2(0.0008, 0.0);
    return normalize(vec3(
        map(p+e.xyy, t, mood, nR, rR, twist, nS, hR, audio).d -
        map(p-e.xyy, t, mood, nR, rR, twist, nS, hR, audio).d,
        map(p+e.yxy, t, mood, nR, rR, twist, nS, hR, audio).d -
        map(p-e.yxy, t, mood, nR, rR, twist, nS, hR, audio).d,
        map(p+e.yyx, t, mood, nR, rR, twist, nS, hR, audio).d -
        map(p-e.yyx, t, mood, nR, rR, twist, nS, hR, audio).d
    ));
}

// Lighting per material id.
vec3 shade(vec3 pos, vec3 nor, vec3 rd, int id, vec3 keyDir, vec3 keyCol,
           vec3 fillDir, vec3 fillCol, float amb, float rim, int mood){
    vec3 base; float spec; float gloss;
    if      (id == 1){ base = CREAM * (0.85 + 0.15*moodTint(mood)); spec = 0.12; gloss = 16.0; }
    else if (id == 2){ base = GOLD;                                  spec = 1.20; gloss = 48.0; }
    else if (id == 3){ base = GOLD * 1.15;                           spec = 1.40; gloss = 64.0; }
    else if (id == 4){ base = vec3(0.10, 0.06, 0.04);               spec = 0.25; gloss = 24.0; }
    else             { base = ROSE * 0.55 + moodTint(mood)*0.15;    spec = 0.08; gloss = 12.0; }

    float ndK = max(dot(nor, keyDir),  0.0);
    float ndF = max(dot(nor, fillDir), 0.0);
    vec3  hK  = normalize(keyDir  - rd);
    float spK = pow(max(dot(nor, hK), 0.0), gloss) * spec;

    float rimT = pow(1.0 - max(dot(nor, -rd), 0.0), 3.0);

    vec3 col  = base * (amb + ndK*keyCol + ndF*fillCol*0.5);
    col += keyCol * spK;
    col += GOLD * rimT * rim * 0.25;     // warm rim regardless of material
    return col;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 ndc = (uv * 2.0 - 1.0) * vec2(aspect, 1.0);

    int   mt    = int(mood + 0.5);
    int   nR    = int(ribbonCount + 0.5);
    int   nS    = int(starCount + 0.5);
    float t     = TIME * rotateSpeed;
    float audio = clamp(audioReact, 0.0, 2.0);

    // ─── Camera (orbiting) ─────────────────────────────────────────────
    float az = camAzimuth + TIME * camOrbitSpeed;
    vec3  ro = vec3(sin(az)*camDist, camHeight, cos(az)*camDist);
    vec3  ta = vec3(0.0, 0.4, 0.0);
    vec3  fwd = normalize(ta - ro);
    vec3  rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3  upv = cross(rgt, fwd);
    vec3  rd  = normalize(fwd + rgt*ndc.x*0.85 + upv*ndc.y*0.85);

    // ─── Lights ────────────────────────────────────────────────────────
    vec3 keyDir = normalize(vec3(
        cos(keyElevation)*cos(keyAngle),
        sin(keyElevation),
        cos(keyElevation)*sin(keyAngle)
    ));
    vec3 fillDir = normalize(vec3(-0.6, 0.3, 0.7));

    // ─── March ─────────────────────────────────────────────────────────
    float d = 0.0;
    int   id = 0;
    vec3  pos = ro;
    for (int i = 0; i < MAX_STEPS; i++){
        pos = ro + rd*d;
        Hit h = map(pos, t, mt, nR, ribbonRadius, ribbonTwist, nS, haloRadius, audio);
        if (h.d < SURF_EPS){ id = h.id; break; }
        if (d > MAX_DIST) break;
        d += h.d * 0.85;
    }

    // ─── Background gradient (warm gilt -> dusty teal) ─────────────────
    vec3 bg = mix(GOLD*0.18, TEAL*0.35, smoothstep(-0.3, 0.6, ndc.y));
    bg += moodTint(mt) * 0.20;
    bg *= 0.9 + 0.1*length(ndc);

    vec3 col = bg;

    if (id != 0){
        vec3 nor = calcNormal(pos, t, mt, nR, ribbonRadius, ribbonTwist,
                              nS, haloRadius, audio);
        col = shade(pos, nor, rd, id, keyDir, keyColor.rgb,
                    fillDir, fillColor.rgb, ambient, rimStrength, mt);

        // Plinth reflection — mirror ray, single bounce, sample bust only.
        if (id == 4 && nor.y > 0.5){
            vec3  rrd = reflect(rd, nor);
            float rd2 = 0.0; vec3 rpos = pos + nor*0.01; int rid = 0;
            for (int j = 0; j < 32; j++){
                rpos = (pos + nor*0.01) + rrd*rd2;
                Hit rh = map(rpos, t, mt, nR, ribbonRadius, ribbonTwist,
                             nS, haloRadius, audio);
                if (rh.d < SURF_EPS){ rid = rh.id; break; }
                if (rd2 > 6.0) break;
                rd2 += rh.d * 0.9;
            }
            vec3 refCol = bg * 0.4;
            if (rid != 0){
                vec3 rn = calcNormal(rpos, t, mt, nR, ribbonRadius, ribbonTwist,
                                     nS, haloRadius, audio);
                refCol = shade(rpos, rn, rrd, rid, keyDir, keyColor.rgb,
                               fillDir, fillColor.rgb, ambient, rimStrength, mt);
            }
            col = mix(col, refCol, 0.18);
        }
    }

    // Treble shimmer on gold (id 2/3) handled via audio modulating exposure tail.
    col *= exposure;
    col += GOLD * 0.04 * audio * float(id == 2 || id == 3);

    gl_FragColor = vec4(col, 1.0);
}
