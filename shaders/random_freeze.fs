/*{
  "DESCRIPTION": "Frost Mandala — 2D procedural fractal snowflake in polar coordinates. 6-fold + 12-fold sub-branch symmetry. Macro ice crystal aesthetic. Cool blue HDR palette.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "2D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"branchSpacing","TYPE":"float","DEFAULT":0.12,"MIN":0.05,"MAX":0.3, "LABEL":"Branch Spacing"},
    {"NAME":"branchWidth",  "TYPE":"float","DEFAULT":0.012,"MIN":0.002,"MAX":0.04,"LABEL":"Branch Width"},
    {"NAME":"hdrPeak",      "TYPE":"float","DEFAULT":2.5, "MIN":1.0, "MAX":4.0,  "LABEL":"HDR Peak"},
    {"NAME":"audioReact",   "TYPE":"float","DEFAULT":0.6, "MIN":0.0, "MAX":2.0,  "LABEL":"Audio React"},
    {"NAME":"rotSpeed",     "TYPE":"float","DEFAULT":0.05,"MIN":0.0, "MAX":0.5,  "LABEL":"Rotation Speed"}
  ]
}*/

// ---------- constants ----------

#define PI  3.14159265358979
#define TAU 6.28318530717959

// ---------- helper: signed distance to a line segment from origin ----------
// along angle 'segAngle', up to radius 'segLen', half-width 'hw'
// We work in polar space, so we check:
//   1. Is r <= segLen?
//   2. Is |theta - segAngle| <= hw_angular (hw / r)?
// Returns a 0..1 coverage value (1 = fully inside).

float lineCoverage(float r, float theta, float segAngle, float segLen, float hw) {
    if (r > segLen + hw) return 0.0;
    float dTheta = abs(theta - segAngle);
    // half-angular-width scales with 1/r (width in world space stays constant)
    float halfAngW = hw / max(r, 0.001);
    float halfAngW_fw = fwidth(dTheta);
    return 1.0 - smoothstep(0.0, halfAngW + halfAngW_fw, dTheta);
}

// Radial spur: check if r is near r_k AND theta near theta_spur
// Returns coverage
float spurCoverage(float r, float theta, float r_k, float theta_spur, float hw) {
    float dr = abs(r - r_k);
    float dTheta = abs(theta - theta_spur);
    // spur half-width in radial direction
    float hw_r     = hw * 2.0;
    float hw_r_fw  = fwidth(dr);
    float hw_th    = hw / max(r, 0.001);
    float hw_th_fw = fwidth(dTheta);
    float coverR = 1.0 - smoothstep(0.0, hw_r + hw_r_fw, dr);
    float coverT = 1.0 - smoothstep(0.0, hw_th + hw_th_fw, dTheta);
    return coverR * coverT;
}

// Small circle SDF coverage (for tips)
float circCoverage(vec2 uv, vec2 center, float radius) {
    float d = length(uv - center) - radius;
    float fw = fwidth(d);
    return 1.0 - smoothstep(0.0, fw * 1.5, d);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // audio
    float aLevel = 1.0 + audioLevel * audioReact;
    float aBass  = 1.0 + audioBass  * audioReact * 0.8;

    // slow rotation of whole snowflake
    float rot = TIME * rotSpeed;
    float uvCos = cos(rot), uvSin = sin(rot);
    vec2 uvRot = vec2(uv.x * uvCos - uv.y * uvSin,
                      uv.x * uvSin + uv.y * uvCos);

    float r     = length(uvRot);
    float angle = atan(uvRot.y, uvRot.x);

    // 6-fold symmetry: fold into sector [0, PI/3], then mirror to [-PI/6, PI/6]
    float sectorAngle = PI / 3.0;
    float theta6 = mod(angle, sectorAngle);          // [0, PI/3]
    theta6 = abs(theta6 - sectorAngle * 0.5);        // [0, PI/6], mirrored

    // branch width pulses with time
    float bw = branchWidth * (1.0 + sin(TIME * 0.4) * 0.1) * aBass;

    // palette
    vec3 colorDeep    = vec3(0.0,  0.3,  0.8)  * 1.5;   // ice blue deep
    vec3 colorCrystal = vec3(0.4,  0.8,  1.0)  * 2.0;   // crystal edge
    vec3 colorTip     = vec3(0.9,  0.95, 1.0)  * hdrPeak * aLevel; // white-hot tip

    // accumulate snowflake coverage
    float snowCov  = 0.0;  // for color mixing
    float deepCov  = 0.0;
    float crystCov = 0.0;
    float tipCov   = 0.0;

    // ---- MAIN SPINE ----
    // Thin line at theta6 = 0, running from r=0 to r=0.95
    float spineCov = lineCoverage(r, theta6, 0.0, 0.95, bw * (1.0 - r * 0.4));
    deepCov  += spineCov * 0.4;
    crystCov += spineCov * 0.6;

    // ---- SPURS (sub-branches perpendicular to spine, at regular r_k) ----
    // For each r_k along main spine, add branches at ±30° and ±60°
    int numSpurs = int(floor(0.95 / branchSpacing));
    for (int k = 1; k <= 20; k++) {
        if (k > numSpurs) break;
        float fk = float(k);
        float rk = fk * branchSpacing;
        if (rk > 0.95) break;

        // spur half-length shrinks toward tips
        float spurLen = bw * 18.0 * (1.0 - rk * 0.8);
        spurLen = max(spurLen, bw * 3.0);

        // spur angles: ±PI/6 (30°) and ±PI/3 (60°) off main spine (theta=0)
        // In our theta6 space (already mirrored), spurs appear at theta6 = PI/6 and PI/3
        // but we need to handle the fold carefully:
        // Primary spurs at PI/6
        float sp1Len = spurLen * 1.0;
        // 30-degree spur
        {
            // spur is radial at rk, within angular range of PI/6
            float dTheta30 = abs(theta6 - PI / 6.0);
            float fw30 = fwidth(dTheta30);
            float angCov = 1.0 - smoothstep(0.0, bw / max(rk, 0.001) + fw30, dTheta30);
            float dr30 = abs(r - rk);
            float drFw = fwidth(dr30);
            float radCov = 1.0 - smoothstep(0.0, sp1Len + drFw, dr30);
            float s30 = angCov * radCov;
            crystCov += s30 * 0.8;
            deepCov  += s30 * 0.3;
        }
        // 60-degree spur (near sector edge — only shows at theta6 ~ PI/6 as well after fold,
        // but use a secondary fold: take mod(theta6, PI/6) - PI/12 to create sub-branches)
        {
            float theta12 = mod(theta6, PI / 6.0);
            theta12 = abs(theta12 - PI / 12.0);
            float dTheta60 = theta12;
            float fw60 = fwidth(dTheta60);
            float angCov60 = 1.0 - smoothstep(0.0, bw * 0.7 / max(rk, 0.001) + fw60, dTheta60);
            float dr60 = abs(r - rk);
            float drFw60 = fwidth(dr60);
            float sp2Len = spurLen * 0.55;
            float radCov60 = 1.0 - smoothstep(0.0, sp2Len + drFw60, dr60);
            float s60 = angCov60 * radCov60;
            crystCov += s60 * 0.5;
        }

        // tip dot at spur ends
        // main spine tip at (rk, 0) in polar => uvRot = (rk, 0) rotated by sector multiple
        // We check all 6 sectors for tips
        for (int sector = 0; sector < 6; sector++) {
            float secAngle = float(sector) * sectorAngle + rot;
            vec2 tipPos = vec2(cos(secAngle) * rk, sin(secAngle) * rk);
            float tc = circCoverage(uvRot, tipPos, bw * 2.0);
            tipCov += tc;

            // also 30-degree spur tips
            float sp1Angle = secAngle + PI / 6.0;
            float sp1End = rk + sp1Len;
            vec2 sp1Tip = vec2(cos(sp1Angle) * sp1End, sin(sp1Angle) * sp1End);
            float tc1 = circCoverage(uvRot, sp1Tip, bw * 1.5);
            tipCov += tc1;

            float sp1AngleN = secAngle - PI / 6.0;
            vec2 sp1TipN = vec2(cos(sp1AngleN) * sp1End, sin(sp1AngleN) * sp1End);
            float tc1n = circCoverage(uvRot, sp1TipN, bw * 1.5);
            tipCov += tc1n;
        }
    }

    // ---- outermost tip dot on main spine ----
    for (int sector = 0; sector < 6; sector++) {
        float secAngle = float(sector) * sectorAngle + rot;
        vec2 tipPos = vec2(cos(secAngle) * 0.95, sin(secAngle) * 0.95);
        float tc = circCoverage(uvRot, tipPos, bw * 3.0);
        tipCov += tc;
    }

    // ---- clamp coverages ----
    deepCov  = clamp(deepCov,  0.0, 1.0);
    crystCov = clamp(crystCov, 0.0, 1.0);
    tipCov   = clamp(tipCov,   0.0, 1.0);

    // ---- background gradient ----
    float bgR = r * 0.8;
    vec3 bgColor = mix(vec3(0.0), vec3(0.0, 0.0, 0.04), bgR * bgR);

    // ---- composite ----
    vec3 snowColor = colorDeep * deepCov + colorCrystal * crystCov;
    snowColor = max(snowColor, colorTip * tipCov);

    // overall snowflake presence
    float snowMask = clamp(deepCov + crystCov + tipCov, 0.0, 1.0);

    // glow halo around snowflake
    float glowR = r;
    float glowHalo = exp(-glowR * 2.5) * 0.12;
    vec3 glowColor = colorDeep * glowHalo;

    vec3 col = bgColor + glowColor + snowColor;

    gl_FragColor = vec4(col, 1.0);
}
