/*{
  "DESCRIPTION": "Sonoluminescence 3D — raymarched glass beaker with plasma bubble core, underwater caustics, concentric ripples, audio-reactive pulse. Cinematic studio lighting. HDR linear output.",
  "CREDIT": "ShaderClaw auto-improve 2026-05-05",
  "CATEGORIES": ["Generator", "Nature", "3D"],
  "INPUTS": [
    {"NAME":"rippleCount","LABEL":"Ripples","TYPE":"float","DEFAULT":8.0,"MIN":3.0,"MAX":20.0},
    {"NAME":"rippleSpeed","LABEL":"Ripple Speed","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":2.0},
    {"NAME":"glowIntensity","LABEL":"Glow","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":3.0},
    {"NAME":"waterLevel","LABEL":"Water Level","TYPE":"float","DEFAULT":0.42,"MIN":0.2,"MAX":0.7},
    {"NAME":"pulseRate","LABEL":"Pulse Rate","TYPE":"float","DEFAULT":1.0,"MIN":0.1,"MAX":4.0},
    {"NAME":"colorTemp","LABEL":"Color Temp","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0},
    {"NAME":"camOrbitSpeed","LABEL":"Orbit Speed","TYPE":"float","DEFAULT":0.18,"MIN":0.0,"MAX":1.0},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/

#define PI 3.14159265

float hash2(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash2(i),   hash2(i+vec2(1,0)), f.x),
               mix(hash2(i+vec2(0,1)), hash2(i+vec2(1,1)), f.x), f.y);
}

float sdCylinder(vec3 p, float r, float h) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

// Concentric ripple height field at XZ position
float rippleH(vec2 xz, float t, float aPulse) {
    float rT = t * rippleSpeed;
    float r  = length(xz);
    float h  = 0.0;
    for (float ri = 0.0; ri < 20.0; ri++) {
        if (ri >= rippleCount) break;
        float ph  = fract(rT * 0.3 + ri / rippleCount);
        float rad = ph * 0.28;
        float fade = (1.0 - ph) * (1.0 - ph);
        h += smoothstep(0.007, 0.0, abs(r - rad)) * fade * 0.007 * (0.5 + 0.5 * aPulse);
    }
    return h;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t  = TIME;

    float bass      = audioBass;
    float pulse     = 0.5 + 0.5 * sin(t * pulseRate * PI * 2.0);
    float audioPulse = mix(pulse, bass, step(0.01, bass)) * audioReact;
    float audioMod  = 0.5 + 0.5 * audioPulse;

    // Palette (4–6 colors grounded in real sonoluminescence light)
    vec3 waterDeep   = mix(vec3(0.00, 0.02, 0.08), vec3(0.00, 0.04, 0.14), colorTemp);
    vec3 waterBright = mix(vec3(0.05, 0.20, 0.60), vec3(0.10, 0.40, 0.80), colorTemp);
    vec3 glowCol     = mix(vec3(0.30, 0.60, 1.00), vec3(0.60, 0.85, 1.00), colorTemp);
    vec3 specHDR     = mix(vec3(0.85, 0.95, 1.00), vec3(1.00, 0.90, 0.80), colorTemp);

    float glassR = 0.30; float glassH = 0.40; float wallT = 0.013;
    float waterY = -0.05 + (waterLevel - 0.42) * glassH * 2.2;

    vec3 bubblePos = vec3(0.0, waterY - 0.10 + 0.008 * sin(t * 1.5), 0.0);
    float bubbleR  = 0.013 * (0.7 + 0.3 * audioPulse);

    // Orbiting camera
    float angle = t * camOrbitSpeed;
    vec3 ro = vec3(sin(angle) * 0.82, 0.06, cos(angle) * 0.82);
    vec3 ta = vec3(0.0, -0.02, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(vec3(0,1,0), ww));
    vec3 vv = cross(ww, uu);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.9 * ww);
    vec3 key = normalize(vec3(0.4, 1.5, 0.3));

    vec3  col  = vec3(0.003, 0.005, 0.012);
    float minT = 1e9;

    // --- Bubble (analytic sphere intersection) ---
    {
        vec3  oc   = ro - bubblePos;
        float bB   = dot(oc, rd);
        float bC   = dot(oc, oc) - bubbleR * bubbleR;
        float disc = bB * bB - bC;
        if (disc > 0.0) {
            float bt = -bB - sqrt(disc);
            if (bt > 0.001 && bt < minT) {
                minT = bt;
                vec3 bP = ro + rd * bt;
                vec3 bN = normalize(bP - bubblePos);
                vec3 v  = normalize(-rd);
                float spec = pow(max(dot(normalize(key + v), bN), 0.0), 64.0);
                // HDR plasma core — peaks at ~5×
                col  = glowCol * glowIntensity * (2.2 + audioPulse * 2.5);
                col += specHDR * spec * 4.0;
            }
        }
    }

    // --- Water surface ---
    if (abs(rd.y) > 0.001) {
        float tW = (waterY - ro.y) / rd.y;
        if (tW > 0.001 && tW < minT) {
            vec3 wP = ro + rd * tW;
            if (length(wP.xz) < glassR - wallT) {
                minT = tW;
                float e2 = 0.006;
                float h0 = rippleH(wP.xz,                   t, audioPulse);
                float hL = rippleH(wP.xz - vec2(e2, 0.0),   t, audioPulse);
                float hR = rippleH(wP.xz + vec2(e2, 0.0),   t, audioPulse);
                float hD = rippleH(wP.xz - vec2(0.0, e2),   t, audioPulse);
                float hU = rippleH(wP.xz + vec2(0.0, e2),   t, audioPulse);
                vec3 wN = normalize(vec3(hL - hR, e2 * 4.0, hD - hU));

                vec3  v    = normalize(-rd);
                float diff = max(dot(wN, key), 0.0);
                float spec = pow(max(dot(normalize(key + v), wN), 0.0), 128.0);
                float fres = pow(1.0 - max(dot(wN, v), 0.0), 4.0);

                // Bubble glow upwelling through water
                float bGlow = exp(-length(wP.xz - bubblePos.xz) * 14.0) * glowIntensity * audioMod;

                // fwidth AA on concentric ripple iso-rings
                float cf = length(wP.xz) * 20.0;
                float fw = fwidth(cf);
                float contour = 1.0 - smoothstep(fw * 0.3, fw * 1.5, abs(fract(cf + 0.5) - 0.5) * 2.0);

                // Caustic shimmer
                float caust = pow(vnoise(wP.xz * 14.0 + t * 0.3), 3.0) * 0.4;

                // Sky reflected in surface
                vec3 refl = reflect(rd, wN);
                vec3 sky  = mix(vec3(0.03, 0.06, 0.14), vec3(0.3, 0.55, 1.0), clamp(refl.y, 0.0, 1.0));

                vec3 waterCol = mix(waterDeep, waterBright * 0.3, 0.3) + caust * waterBright * 0.5;
                col  = waterCol * (0.15 + diff * 0.8) + sky * fres * 0.5;
                col += glowCol * bGlow * 2.2;
                // HDR specular peaks bloom-ready
                col += specHDR * (spec * 2.0 + pow(spec, 4.0) * 1.5);
                col += contour * specHDR * 0.4;
            }
        }
    }

    // --- Glass vessel shell ---
    {
        float dist2 = 0.01;
        for (int i = 0; i < 48; i++) {
            vec3  p  = ro + rd * dist2;
            float dO = sdCylinder(p, glassR + wallT, glassH);
            float dI = sdCylinder(p, glassR,         glassH);
            float dG = max(dO, -dI);
            if (dG < 0.004 && dist2 < minT) {
                float fresG = pow(1.0 - abs(dot(normalize(vec3(p.x, 0.0, p.z)), rd)), 3.0);
                vec3 glassShade = vec3(0.06, 0.11, 0.18) * (0.4 + fresG * 0.6);
                glassShade += specHDR * fresG * 0.5;
                col = mix(col, glassShade, 0.45);
                minT = dist2;
                break;
            }
            if (dist2 > 3.0) break;
            dist2 += max(dO * 0.8, 0.004);
        }
    }

    // Volumetric bubble halo along ray (additive HDR)
    {
        vec3  oc      = ro - bubblePos;
        float closest = length(oc - rd * clamp(dot(oc, rd), 0.0, 5.0));
        float halo    = exp(-closest * closest * 280.0) * glowIntensity * audioMod;
        col += glowCol * halo * 1.8;
    }

    // Subtle vignette
    col *= 1.0 - 0.45 * dot(uv, uv);

    // Linear HDR output — host applies ACES
    gl_FragColor = vec4(col, 1.0);
}
