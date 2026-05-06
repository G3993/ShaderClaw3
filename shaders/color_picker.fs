/*{
  "DESCRIPTION": "Chromatic Prism — 3D raymarched glass triangular prism dispersing white light into a rainbow spectrum with caustic floor pools",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "camSpeed",   "LABEL": "Cam Speed",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "dispersion", "LABEL": "Dispersion",  "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "hdrPeak",   "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "audioReact","LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define MAX_DIST  18.0
#define SURF_DIST 0.003

float hash21(vec2 p) { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// Equilateral triangular prism SDF, Z-axis aligned
float sdTriPrism(vec3 p, float r, float h) {
    const float k = 0.866025; // sqrt(3)/2
    vec3 q = vec3(abs(p.x), p.y, abs(p.z));
    float d = max(q.x * k + q.y * 0.5 - r * k, -q.y - r * 0.5);
    return max(d, q.z - h);
}

float map(vec3 p) {
    float t = TIME * camSpeed;
    float ca = cos(t), sa = sin(t);
    vec3 q = vec3(ca*p.x - sa*p.z, p.y, sa*p.x + ca*p.z);
    float prism = sdTriPrism(q, 0.38, 0.75);
    float floorP = p.y + 1.1;
    return min(prism, floorP);
}

vec3 calcNormal(vec3 p) {
    const vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p+e.xyy)-map(p-e.xyy),
        map(p+e.yxy)-map(p-e.yxy),
        map(p+e.yyx)-map(p-e.yyx)));
}

// Wavelength 0..1 -> saturated spectrum color
vec3 spectrum(float t) {
    // violet=0, blue=0.17, cyan=0.33, green=0.5, gold=0.67, red=0.83, crimson=1.0
    vec3 c;
    c.r = smoothstep(0.5,0.7,t) + (1.0-smoothstep(0.0,0.15,t));
    c.g = smoothstep(0.15,0.4,t)*smoothstep(0.85,0.65,t);
    c.b = smoothstep(0.05,0.25,t)*smoothstep(0.5,0.3,t) + (1.0-smoothstep(0.0,0.15,t))*0.5;
    return clamp(c, 0.0, 1.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel * audioReact * 0.35 + audioBass * audioReact * 0.15;

    // Orbiting camera
    float camT = TIME * camSpeed * 0.7;
    vec3 ro = vec3(sin(camT)*2.8, 0.6+sin(camT*0.31)*0.25, cos(camT)*2.8);
    vec3 ta  = vec3(0.0, -0.2, 0.0);
    vec3 ww  = normalize(ta - ro);
    vec3 uu  = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv  = cross(uu, ww);
    vec3 rd  = normalize(uv.x*uu + uv.y*vv + 1.8*ww);

    // March
    float dist = 0.0;
    float hitT = MAX_DIST;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd*dist;
        float d = map(p);
        if (d < SURF_DIST) { hitT = dist; break; }
        dist += d;
        if (dist > MAX_DIST) break;
    }

    vec3 col = vec3(0.005, 0.005, 0.02); // deep void

    if (hitT < MAX_DIST) {
        vec3 p   = ro + rd*hitT;
        vec3 n   = calcNormal(p);
        vec3 lDir = normalize(vec3(-1.5, 2.2, -0.8));
        float diff = max(dot(n, lDir), 0.0);
        float spec = pow(max(dot(reflect(-lDir,n),-rd),0.0), 48.0);

        bool isFloor = (p.y < -1.08);
        if (isFloor) {
            // Spectrum caustic pools (5 bands)
            vec3 fc = vec3(0.01, 0.01, 0.03);
            for (int k = 0; k < 5; k++) {
                float wl = float(k)/4.0;
                vec3 sc  = spectrum(wl);
                // Caustic blob position on floor, spread by dispersion
                vec2 cPos = vec2(-0.5 + wl*dispersion*0.7, 0.1 + wl*0.2);
                float r2  = dot(p.xz - cPos, p.xz - cPos);
                float blob = exp(-r2 * (3.0 + dispersion));
                fc += sc * blob * hdrPeak * audio;
            }
            // Dark floor texture
            float checker = step(0.5, fract(p.x*1.5)) * step(0.5, fract(p.z*1.5))
                          + step(0.5, fract(p.x*1.5+0.5)) * step(0.5, fract(p.z*1.5+0.5));
            fc *= 0.8 + checker*0.2;
            col = fc;
        } else {
            // Prism glass: interior wavelength-tinted
            float ct = TIME * camSpeed;
            float ca2 = cos(ct), sa2 = sin(ct);
            vec3 q2 = vec3(ca2*p.x - sa2*p.z, p.y, sa2*p.x + ca2*p.z);
            float wl = clamp((q2.y+0.38)/0.76, 0.0, 1.0);
            vec3 glassCol = spectrum(wl)*0.4 + vec3(0.15, 0.18, 0.25);
            float aa = fwidth(sdTriPrism(q2, 0.38, 0.75));
            glassCol  = mix(glassCol, glassCol*0.5, smoothstep(0.0, aa*2.0, abs(sdTriPrism(q2, 0.38, 0.75))));
            col  = glassCol + diff*0.2 + spec*hdrPeak*audio*vec3(1.0,0.95,0.8);
        }
    }

    // Analytical spectrum beams in void (entering beam = white, exiting = spectrum fan)
    // Entry beam: from top-left toward prism
    {
        vec3 bO = vec3(-1.5, 1.4, 0.05);
        vec3 bD = normalize(vec3(0.55, -0.8, -0.05));
        vec3 oc = ro - bO;
        vec3 cr = cross(rd, bD);
        float cr2 = dot(cr,cr);
        if (cr2 > 1e-6) {
            float tb = dot(cross(oc, bD), cr)/cr2;
            if (tb > 0.0) {
                vec3 np = ro + rd*tb;
                float bd = length(cross(np - bO, bD));
                float bw = exp(-bd*bd*350.0);
                col += vec3(1.1,1.05,1.0)*bw*hdrPeak*0.6*audio;
            }
        }
    }
    // Exit spectrum fan (5 rays)
    for (int k = 0; k < 5; k++) {
        float wl = float(k)/4.0;
        vec3 sc  = spectrum(wl);
        vec3 bO  = vec3(0.0, 0.0, 0.0);
        vec3 bD  = normalize(vec3(0.3 + wl*dispersion*0.18, -0.6 + wl*0.12, 0.0));
        vec3 oc  = ro - bO;
        vec3 cr  = cross(rd, bD);
        float cr2 = dot(cr,cr);
        if (cr2 > 1e-6) {
            float tb = dot(cross(oc, bD), cr)/cr2;
            if (tb > 0.0) {
                vec3 np = ro + rd*tb;
                float bd = length(cross(np - bO, bD));
                float bw = exp(-bd*bd*250.0);
                col += sc*bw*hdrPeak*0.7*audio;
            }
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
