/*{
  "DESCRIPTION": "Robot arm — 2-link inverse kinematics follows hand tracking or mouse. Up to 8 arms. Pinch to grip, hold to fire laser or bullets.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "armMode", "LABEL": "Arms", "TYPE": "long", "DEFAULT": 1, "VALUES": [1, 2, 3, 4, 5, 6, 7, 8], "LABELS": ["1", "2", "3", "4", "5", "6", "7", "8"] },
    { "NAME": "armColor", "LABEL": "Arm", "TYPE": "color", "DEFAULT": [1.0, 0.239, 0.239, 1.0] },
    { "NAME": "accentColor", "LABEL": "Accent", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "laserColor", "LABEL": "Laser", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "armScale", "LABEL": "Size", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 2.0 },
    { "NAME": "segWidth", "LABEL": "Thickness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 2.5 },
    { "NAME": "laserSize", "LABEL": "Laser Size", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "laserLength", "LABEL": "Laser Length", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.3, "MAX": 5.0 },
    { "NAME": "laserGlow", "LABEL": "Laser Glow", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "bulletSpeed", "LABEL": "Bullet Speed", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 20.0 },
    { "NAME": "showGrid", "LABEL": "Grid", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.035, 0.035, 0.055, 1.0] }
  ]
}*/

float sdCapsule(vec2 p, vec2 a, vec2 b, float r) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

vec3 shadeCapsule(vec2 p, vec2 a, vec2 b, float r, vec3 color, vec3 L, float px, out float mask) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    vec2 closest = a + ba * h;
    float dist = length(p - closest);
    mask = smoothstep(r + px, r - px, dist);
    if (mask < 0.001) return vec3(0.0);

    float t = clamp(dist / r, 0.0, 1.0);
    float nz = sqrt(max(0.0, 1.0 - t * t));
    vec3 N = normalize(vec3((p - closest) / max(dist, 0.0001), nz));

    float diff = max(0.0, dot(N, L));
    float spec = pow(max(0.0, dot(reflect(-L, N), vec3(0.0, 0.0, 1.0))), 80.0);
    float rim = pow(1.0 - nz, 2.5);

    // Panel groove lines
    float groove = smoothstep(0.015, 0.0, abs(fract(h * 5.0 + 0.5) - 0.5) - 0.485) * 0.25;

    vec3 c = color * (1.0 - groove);
    return c * (0.12 + 0.58 * diff) + vec3(0.7) * spec + c * rim * 0.22;
}

vec3 shadeSphere(vec2 p, vec2 c, float r, vec3 color, vec3 L, float px, out float mask) {
    float dist = length(p - c);
    mask = smoothstep(r + px, r - px, dist);
    if (mask < 0.001) return vec3(0.0);

    float t = clamp(dist / r, 0.0, 1.0);
    float nz = sqrt(max(0.0, 1.0 - t * t));
    vec3 N = normalize(vec3((p - c) / max(dist, 0.0001), nz));

    float diff = max(0.0, dot(N, L));
    float spec = pow(max(0.0, dot(reflect(-L, N), vec3(0.0, 0.0, 1.0))), 100.0);

    // Single servo ring groove
    float ring = smoothstep(0.03, 0.0, abs(t - 0.55)) * 0.18;
    vec3 base = color * (1.0 - ring);

    return base * (0.10 + 0.55 * diff) + vec3(0.9) * spec + base * pow(1.0 - nz, 2.0) * 0.35;
}

void drawArm(vec2 p, vec2 base, vec2 target, float grip, float sc, float sw,
             vec4 aCol, vec4 accCol, vec3 L, float px, float elbowSign,
             inout vec3 col, inout float armMask,
             out vec2 outWrist, out vec2 outFDir,
             out vec2 outF1, out vec2 outF2, out vec2 outF3) {

    float L1 = 0.25 * sc;
    float L2 = 0.22 * sc;
    float w1 = 0.024 * sw;
    float w2 = 0.018 * sw;
    float jR = 0.028 * sw;

    // IK
    vec2 toTarget = target - base;
    float d = length(toTarget);
    float maxReach = L1 + L2 - 0.005;
    d = clamp(d, abs(L1 - L2) + 0.005, maxReach);
    vec2 dir = normalize(toTarget) * d;

    float cosT2 = clamp((d * d - L1 * L1 - L2 * L2) / (2.0 * L1 * L2), -1.0, 1.0);
    float theta2 = acos(cosT2);
    float bAngle = atan(dir.y, dir.x);
    float theta1 = bAngle - elbowSign * atan(L2 * sin(theta2), L1 + L2 * cos(theta2));
    float theta2f = elbowSign * theta2;

    vec2 elbow = base + L1 * vec2(cos(theta1), sin(theta1));
    vec2 wrist = elbow + L2 * vec2(cos(theta1 + theta2f), sin(theta1 + theta2f));

    // Claw geometry -- 2-segment tapered talons
    vec2 fDir_ = normalize(target - base);
    float fAngle = atan(fDir_.y, fDir_.x);
    float clawBase = 0.032 * sc;
    float clawTip  = 0.028 * sc;
    float baseW = 0.010 * sw;
    float tipW  = 0.004 * sw;
    float openA = 0.40 * (1.0 - grip * 0.85);
    float hookA = 0.18 + grip * 0.28;

    vec2 f1Mid = wrist + clawBase * vec2(cos(fAngle + openA), sin(fAngle + openA));
    vec2 f1End = f1Mid + clawTip * vec2(cos(fAngle + openA - hookA), sin(fAngle + openA - hookA));
    vec2 f2Mid = wrist + clawBase * vec2(cos(fAngle - openA), sin(fAngle - openA));
    vec2 f2End = f2Mid + clawTip * vec2(cos(fAngle - openA + hookA), sin(fAngle - openA + hookA));
    vec2 f3Mid = wrist + 0.036 * sc * vec2(cos(fAngle), sin(fAngle));
    vec2 f3End = f3Mid + clawTip * vec2(cos(fAngle), sin(fAngle));

    outWrist = wrist; outFDir = fDir_;
    outF1 = f1End; outF2 = f2End; outF3 = f3End;

    // Piston geometry
    vec2 ax1 = normalize(elbow - base);
    vec2 perp1 = vec2(-ax1.y, ax1.x);
    vec2 pA1 = base + ax1 * L1 * 0.18 + perp1 * w1 * 0.55;
    vec2 pB1 = elbow - ax1 * L1 * 0.12 + perp1 * w1 * 0.55;
    vec2 ax2 = normalize(wrist - elbow);
    vec2 perp2 = vec2(-ax2.y, ax2.x);
    vec2 pA2 = elbow + ax2 * L2 * 0.2 + perp2 * w2 * 0.6;
    vec2 pB2 = wrist - ax2 * L2 * 0.15 + perp2 * w2 * 0.6;

    // Joint glow only (subtle)
    col += accCol.rgb * (exp(-45.0 * length(p - wrist))) * 0.15;

    float mask;
    vec3 ec;
    vec3 pistonCol = aCol.rgb * 0.6 + vec3(0.2);

    // Base mount
    ec = shadeCapsule(p, base - vec2(0.045, 0.0), base + vec2(0.045, 0.0), 0.032, aCol.rgb * 0.45, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Upper arm piston
    ec = shadeCapsule(p, pA1, pB1, w1 * 0.18, pistonCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Upper arm
    ec = shadeCapsule(p, base, elbow, w1, aCol.rgb, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Shoulder joint
    ec = shadeSphere(p, base, jR, mix(aCol.rgb, accCol.rgb, 0.35), L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Forearm piston
    ec = shadeCapsule(p, pA2, pB2, w2 * 0.2, pistonCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Forearm
    ec = shadeCapsule(p, elbow, wrist, w2, aCol.rgb * 0.95, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Elbow joint
    ec = shadeSphere(p, elbow, jR, mix(aCol.rgb, accCol.rgb, 0.5), L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Claw bases (thicker)
    vec3 clawCol = mix(aCol.rgb, accCol.rgb, 0.45);
    ec = shadeCapsule(p, wrist, f1Mid, baseW, clawCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);
    ec = shadeCapsule(p, wrist, f2Mid, baseW, clawCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);
    ec = shadeCapsule(p, wrist, f3Mid, baseW, clawCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Claw tips (thin, sharp)
    vec3 tipCol = accCol.rgb * 0.9;
    ec = shadeCapsule(p, f1Mid, f1End, tipW, tipCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);
    ec = shadeCapsule(p, f2Mid, f2End, tipW, tipCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);
    ec = shadeCapsule(p, f3Mid, f3End, tipW, tipCol, L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Wrist joint (on top of claws)
    ec = shadeSphere(p, wrist, jR * 0.75, mix(aCol.rgb, accCol.rgb, 0.6), L, px, mask);
    col = mix(col, ec, mask); armMask = max(armMask, mask);

    // Tip + joint highlights
    col += accCol.rgb * (exp(-160.0 * length(p - f1End)) + exp(-160.0 * length(p - f2End)) + exp(-160.0 * length(p - f3End))) * 0.4;
    col += accCol.rgb * (exp(-90.0 * length(p - elbow)) * 0.4 + exp(-100.0 * length(p - wrist)) * 0.3);
}

void drawLaser(vec2 p, vec2 origin, vec2 dir, float grip, vec3 beamColor, float px,
               float size, float shotMode, float lenMul, float glowMul,
               inout vec3 col, inout float armMask) {
    if (grip < 0.05) return;

    float intensity = smoothstep(0.05, 0.5, grip);
    float beamLen = mix(0.05, 0.6, intensity) * (1.0 + audioBass * 1.5) * lenMul;
    intensity *= (0.85 + 0.15 * sin(TIME * (18.0 + audioHigh * 40.0) + origin.x * 40.0))
               * (0.9 + 0.1 * sin(TIME * 7.0));

    vec2 tip = origin + dir * beamLen;
    vec2 po = p - origin, bo = tip - origin;
    float t = clamp(dot(po, bo) / dot(bo, bo), 0.0, 1.0);
    float d = length(p - (origin + bo * t));
    float taper = mix(0.008 * size, 0.002 * size, t);

    // Shot mode: fast dashed lines when second hand pinches
    float shotMask = 1.0;
    if (shotMode > 0.01) {
        float along = dot(p - origin, dir);
        float dashSpeed = 8.0;
        float dashFreq = 28.0;
        float dash = smoothstep(0.3, 0.7, fract(along * dashFreq - TIME * dashSpeed));
        shotMask = mix(1.0, dash, smoothstep(0.0, 0.3, shotMode));
    }

    float safeGlow = max(glowMul, 0.01);
    float core = smoothstep(taper + px, taper * 0.3, d) * intensity * shotMask;
    col += beamColor * 1.8 * core * glowMul;
    armMask = max(armMask, core * 0.6);
    col += beamColor * (exp(-d * (60.0 / safeGlow / size) * mix(0.5, 1.5, t)) * 0.7 * glowMul
         + exp(-d * (20.0 / safeGlow / size) * mix(0.4, 1.0, t)) * 0.4 * glowMul) * intensity * shotMask;
    col += (beamColor + vec3(0.3)) * exp(-40.0 / safeGlow * length(p - origin)) * intensity * 1.5 * glowMul;
}

void drawBullets(vec2 p, vec2 origin, vec2 dir, float grip, vec3 beamColor, float px,
                 float size, float bSpeed, float glowMul, float bulletMix,
                 inout vec3 col, inout float armMask) {
    if (bulletMix < 0.001 || grip < 0.05) return;

    float intensity = smoothstep(0.05, 0.5, grip);
    float safeGlow = max(glowMul, 0.01);

    // Emit bullets along the beam direction
    float bulletFreq = 12.0;
    float reach = 0.8 * (1.0 + audioBass * 1.0);
    for (int k = 0; k < 8; k++) {
        float idx = float(k);
        float phase = fract(idx / bulletFreq - TIME * bSpeed * 0.15);
        float dist = phase * reach;
        vec2 bulletPos = origin + dir * dist;

        // Fade out as bullet travels and at birth
        float fadeTrail = exp(-2.5 * phase);
        float fadeBirth = smoothstep(0.0, 0.05, phase);
        float fade = fadeTrail * fadeBirth * intensity;

        float bDist = length(p - bulletPos);
        float bullet = exp(-200.0 / (size * safeGlow) * bDist) * fade;
        // Trailing glow
        float trail = exp(-60.0 / safeGlow * bDist) * fade * 0.4;

        col += beamColor * (bullet * 2.5 + trail) * bulletMix * glowMul;
        armMask = max(armMask, bullet * bulletMix * 0.6);
    }

    // Origin glow
    col += (beamColor + vec3(0.2)) * exp(-50.0 / safeGlow * length(p - origin)) * intensity * bulletMix * glowMul;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);
    float px = 1.5 / RENDERSIZE.y;
    vec3 L = normalize(vec3(-0.4, 0.6, 0.8));

    float numArms = float(armMode);
    float spread = aspect * 0.4;

    // Base positions for up to 8 arms
    vec2 base1, base2, base3, base4, base5, base6, base7, base8;

    if (numArms < 1.5) {
        base1 = vec2(0.0, -0.38);
        base2 = base1; base3 = base1; base4 = base1;
        base5 = base1; base6 = base1; base7 = base1; base8 = base1;
    } else if (numArms < 2.5) {
        base1 = vec2(-spread, -0.38);
        base2 = vec2( spread, -0.38);
        base3 = base1; base4 = base2;
        base5 = base1; base6 = base2; base7 = base1; base8 = base2;
    } else if (numArms < 3.5) {
        base1 = vec2(-spread, -0.38);
        base2 = vec2( 0.00, -0.38);
        base3 = vec2( spread, -0.38);
        base4 = base2;
        base5 = base1; base6 = base2; base7 = base3; base8 = base2;
    } else if (numArms < 4.5) {
        // 4 arms: corners
        base1 = vec2(-spread * 1.3, -0.38);
        base2 = vec2( spread * 1.3, -0.38);
        base3 = vec2(-spread * 1.3,  0.38);
        base4 = vec2( spread * 1.3,  0.38);
        base5 = base1; base6 = base2; base7 = base3; base8 = base4;
    } else if (numArms < 5.5) {
        // 5 arms: 3 bottom + 2 top
        base1 = vec2(-spread, -0.38);
        base2 = vec2( 0.0,   -0.38);
        base3 = vec2( spread, -0.38);
        base4 = vec2(-spread * 0.7, 0.38);
        base5 = vec2( spread * 0.7, 0.38);
        base6 = base1; base7 = base2; base8 = base3;
    } else if (numArms < 6.5) {
        // 6 arms: 3 bottom + 3 top
        base1 = vec2(-spread, -0.38);
        base2 = vec2( 0.0,   -0.38);
        base3 = vec2( spread, -0.38);
        base4 = vec2(-spread,  0.38);
        base5 = vec2( 0.0,    0.38);
        base6 = vec2( spread,  0.38);
        base7 = base1; base8 = base2;
    } else if (numArms < 7.5) {
        // 7 arms: 4 bottom + 3 top
        base1 = vec2(-spread * 1.3, -0.38);
        base2 = vec2(-spread * 0.43, -0.38);
        base3 = vec2( spread * 0.43, -0.38);
        base4 = vec2( spread * 1.3, -0.38);
        base5 = vec2(-spread, 0.38);
        base6 = vec2( 0.0,   0.38);
        base7 = vec2( spread, 0.38);
        base8 = base1;
    } else {
        // 8 arms: 4 bottom + 4 top
        base1 = vec2(-spread * 1.3, -0.38);
        base2 = vec2(-spread * 0.43, -0.38);
        base3 = vec2( spread * 0.43, -0.38);
        base4 = vec2( spread * 1.3, -0.38);
        base5 = vec2(-spread * 1.3, 0.38);
        base6 = vec2(-spread * 0.43, 0.38);
        base7 = vec2( spread * 0.43, 0.38);
        base8 = vec2( spread * 1.3, 0.38);
    }

    float t = TIME;
    vec2 mouseTgt = (mousePos - 0.5) * vec2(aspect, 1.0);
    vec2 hand1Tgt = (vec2(1.0 - mpHandPos.x, mpHandPos.y) - 0.5) * vec2(aspect, 1.0);
    vec2 hand2Tgt = (vec2(1.0 - mpHandPos2.x, mpHandPos2.y) - 0.5) * vec2(aspect, 1.0);

    vec2 handL = hand1Tgt, handR = hand2Tgt;
    if (mpHandCount >= 1.5 && handL.x > handR.x) { vec2 tmp = handL; handL = handR; handR = tmp; }

    float activity = clamp(inputActivity, 0.0, 1.0);
    vec2 mm = vec2(-mouseTgt.x, mouseTgt.y);
    vec2 mmFlipY = vec2(mouseTgt.x, -mouseTgt.y);
    vec2 mmFlipXY = vec2(-mouseTgt.x, -mouseTgt.y);

    // Idle offsets -- bottom arms drift up, top arms drift down
    vec2 idleB1 = vec2(sin(t * 0.70) * 0.08, cos(t * 0.50) * 0.06 + 0.18);
    vec2 idleB2 = vec2(sin(t * 0.60 + 2.1) * 0.08, cos(t * 0.45 + 1.3) * 0.06 + 0.18);
    vec2 idleB3 = vec2(sin(t * 0.55 + 4.2) * 0.08, cos(t * 0.40 + 2.6) * 0.06 + 0.18);
    vec2 idleB4 = vec2(sin(t * 0.65 + 5.8) * 0.08, cos(t * 0.48 + 3.9) * 0.06 + 0.18);
    vec2 idleT1 = vec2(sin(t * 0.55 + 4.2) * 0.08, -(cos(t * 0.40 + 2.6) * 0.06 + 0.18));
    vec2 idleT2 = vec2(sin(t * 0.65 + 5.8) * 0.08, -(cos(t * 0.48 + 3.9) * 0.06 + 0.18));
    vec2 idleT3 = vec2(sin(t * 0.50 + 1.0) * 0.08, -(cos(t * 0.42 + 0.7) * 0.06 + 0.18));
    vec2 idleT4 = vec2(sin(t * 0.58 + 3.3) * 0.08, -(cos(t * 0.46 + 2.0) * 0.06 + 0.18));

    // Per-arm idle and live targets
    vec2 idle1, idle2, idle3, idle4, idle5, idle6, idle7, idle8;
    vec2 live1, live2, live3, live4, live5, live6, live7, live8;

    bool hasHands = mpHandCount >= 1.5;
    bool hasOneHand = mpHandCount > 0.5;

    if (numArms < 1.5) {
        idle1 = idleB1;
        live1 = hasOneHand ? hand1Tgt : mouseTgt;
        idle2 = idle1; idle3 = idle1; idle4 = idle1;
        idle5 = idle1; idle6 = idle1; idle7 = idle1; idle8 = idle1;
        live2 = live1; live3 = live1; live4 = live1;
        live5 = live1; live6 = live1; live7 = live1; live8 = live1;
    } else if (numArms < 2.5) {
        idle1 = idleB1; idle2 = idleB2;
        live1 = hasHands ? handL : mouseTgt;
        live2 = hasHands ? handR : mm;
        idle3 = idle1; idle4 = idle2; idle5 = idle1; idle6 = idle2; idle7 = idle1; idle8 = idle2;
        live3 = live1; live4 = live2; live5 = live1; live6 = live2; live7 = live1; live8 = live2;
    } else if (numArms < 3.5) {
        idle1 = idleB1; idle2 = idleB2; idle3 = idleB3;
        live1 = hasHands ? handL : mouseTgt;
        live3 = hasHands ? handR : mm;
        live2 = hasHands ? (handL + handR) * 0.5 : mouseTgt * vec2(0.0, 1.0);
        idle4 = idle2; idle5 = idle1; idle6 = idle2; idle7 = idle3; idle8 = idle2;
        live4 = live2; live5 = live1; live6 = live2; live7 = live3; live8 = live2;
    } else if (numArms < 4.5) {
        idle1 = idleB1; idle2 = idleB2; idle3 = idleT1; idle4 = idleT2;
        if (hasHands) {
            live1 = handL; live2 = handR; live3 = handL; live4 = handR;
        } else {
            live1 = mouseTgt; live2 = mm;
            live3 = mmFlipY; live4 = mmFlipXY;
        }
        idle5 = idle1; idle6 = idle2; idle7 = idle3; idle8 = idle4;
        live5 = live1; live6 = live2; live7 = live3; live8 = live4;
    } else if (numArms < 5.5) {
        // 5: 3 bottom + 2 top
        idle1 = idleB1; idle2 = idleB2; idle3 = idleB3;
        idle4 = idleT1; idle5 = idleT2;
        if (hasHands) {
            live1 = handL; live2 = handR; live3 = handL;
            live4 = handR; live5 = handL;
        } else {
            live1 = mouseTgt; live2 = mouseTgt * vec2(0.0, 1.0); live3 = mm;
            live4 = mmFlipY; live5 = mmFlipXY;
        }
        idle6 = idle1; idle7 = idle2; idle8 = idle3;
        live6 = live1; live7 = live2; live8 = live3;
    } else if (numArms < 6.5) {
        // 6: 3 bottom + 3 top
        idle1 = idleB1; idle2 = idleB2; idle3 = idleB3;
        idle4 = idleT1; idle5 = idleT2; idle6 = idleT3;
        if (hasHands) {
            live1 = handL; live2 = handR; live3 = handL;
            live4 = handR; live5 = handL; live6 = handR;
        } else {
            live1 = mouseTgt; live2 = mouseTgt * vec2(0.0, 1.0); live3 = mm;
            live4 = mmFlipY; live5 = mmFlipXY; live6 = vec2(0.0, -mouseTgt.y);
        }
        idle7 = idle1; idle8 = idle2;
        live7 = live1; live8 = live2;
    } else if (numArms < 7.5) {
        // 7: 4 bottom + 3 top
        idle1 = idleB1; idle2 = idleB2; idle3 = idleB3; idle4 = idleB4;
        idle5 = idleT1; idle6 = idleT2; idle7 = idleT3;
        if (hasHands) {
            live1 = handL; live2 = handR; live3 = handL; live4 = handR;
            live5 = handL; live6 = handR; live7 = handL;
        } else {
            live1 = mouseTgt; live2 = vec2(mouseTgt.x * 0.3, mouseTgt.y);
            live3 = vec2(-mouseTgt.x * 0.3, mouseTgt.y); live4 = mm;
            live5 = mmFlipY; live6 = mmFlipXY; live7 = vec2(0.0, -mouseTgt.y);
        }
        idle8 = idle1;
        live8 = live1;
    } else {
        // 8: 4 bottom + 4 top
        idle1 = idleB1; idle2 = idleB2; idle3 = idleB3; idle4 = idleB4;
        idle5 = idleT1; idle6 = idleT2; idle7 = idleT3; idle8 = idleT4;
        if (hasHands) {
            live1 = handL; live2 = handR; live3 = handL; live4 = handR;
            live5 = handL; live6 = handR; live7 = handL; live8 = handR;
        } else {
            live1 = mouseTgt; live2 = vec2(mouseTgt.x * 0.3, mouseTgt.y);
            live3 = vec2(-mouseTgt.x * 0.3, mouseTgt.y); live4 = mm;
            live5 = mmFlipY; live6 = vec2(-mouseTgt.x * 0.3, -mouseTgt.y);
            live7 = vec2(mouseTgt.x * 0.3, -mouseTgt.y); live8 = mmFlipXY;
        }
    }

    vec2 target1 = mix(base1 + idle1, live1, activity);
    vec2 target2 = mix(base2 + idle2, live2, activity);
    vec2 target3 = mix(base3 + idle3, live3, activity);
    vec2 target4 = mix(base4 + idle4, live4, activity);
    vec2 target5 = mix(base5 + idle5, live5, activity);
    vec2 target6 = mix(base6 + idle6, live6, activity);
    vec2 target7 = mix(base7 + idle7, live7, activity);
    vec2 target8 = mix(base8 + idle8, live8, activity);

    // Grid overlay
    vec3 col = bgColor.rgb;
    if (showGrid) {
        float gx = abs(fract(p.x * 10.0) - 0.5);
        float gy = abs(fract(p.y * 10.0) - 0.5);
        float grid = smoothstep(0.48, 0.5, max(gx, gy));
        col += vec3(0.04) * grid;
    }

    float armMask = 0.0;

    float grip = max(pinchHold, mouseDown);
    float shot = pinchHold2;
    vec3 beamColor = laserColor.rgb * 2.5;
    float lenMul = laserLength;
    float glowMul = laserGlow;

    // Bullet mode: grip held > 1.5 seconds
    // We approximate hold duration using grip * TIME modulation
    // gripStart tracks when grip first exceeded 0.5
    // Since we can't store state, use a smooth ramp: bulletMix grows when grip is sustained
    float holdTime = grip * clamp((grip - 0.5) * 6.0, 0.0, 1.0);
    // Use audio reactivity as bullet intensity boost
    float bulletMix = smoothstep(0.8, 1.0, holdTime) * smoothstep(0.0, 0.3, grip - 0.5);
    float laserMix = 1.0 - bulletMix;

    // Helper macro: draw one arm + its 3 lasers/bullets
    // We inline this for each arm since GLSL ES 1.0 can't do function arrays

    // Arm 1
    vec2 w1, fd1, f1a, f1b, f1c;
    drawArm(p, base1, target1, grip, armScale, segWidth,
            armColor, accentColor, L, px, (numArms < 1.5) ? 1.0 : -1.0,
            col, armMask, w1, fd1, f1a, f1b, f1c);
    float a1 = atan(fd1.y, fd1.x);
    if (laserMix > 0.001) {
        drawLaser(p, f1c, fd1, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        drawLaser(p, f1a, vec2(cos(a1 + 0.25), sin(a1 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        drawLaser(p, f1b, vec2(cos(a1 - 0.25), sin(a1 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
    }
    if (bulletMix > 0.001) {
        drawBullets(p, f1c, fd1, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        drawBullets(p, f1a, vec2(cos(a1 + 0.25), sin(a1 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        drawBullets(p, f1b, vec2(cos(a1 - 0.25), sin(a1 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
    }

    // Arm 2
    if (numArms > 1.5) {
        vec2 w2, fd2, f2a, f2b, f2c;
        drawArm(p, base2, target2, grip, armScale, segWidth,
                armColor, accentColor, L, px, 1.0,
                col, armMask, w2, fd2, f2a, f2b, f2c);
        float a2 = atan(fd2.y, fd2.x);
        if (laserMix > 0.001) {
            drawLaser(p, f2c, fd2, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f2a, vec2(cos(a2 + 0.25), sin(a2 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f2b, vec2(cos(a2 - 0.25), sin(a2 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f2c, fd2, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f2a, vec2(cos(a2 + 0.25), sin(a2 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f2b, vec2(cos(a2 - 0.25), sin(a2 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    // Arm 3
    if (numArms > 2.5) {
        vec2 w3, fd3, f3a, f3b, f3c;
        drawArm(p, base3, target3, grip, armScale, segWidth,
                armColor, accentColor, L, px, 1.0,
                col, armMask, w3, fd3, f3a, f3b, f3c);
        float a3 = atan(fd3.y, fd3.x);
        if (laserMix > 0.001) {
            drawLaser(p, f3c, fd3, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f3a, vec2(cos(a3 + 0.25), sin(a3 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f3b, vec2(cos(a3 - 0.25), sin(a3 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f3c, fd3, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f3a, vec2(cos(a3 + 0.25), sin(a3 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f3b, vec2(cos(a3 - 0.25), sin(a3 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    // Arm 4
    if (numArms > 3.5) {
        vec2 w4, fd4, f4a, f4b, f4c;
        drawArm(p, base4, target4, grip, armScale, segWidth,
                armColor, accentColor, L, px, -1.0,
                col, armMask, w4, fd4, f4a, f4b, f4c);
        float a4 = atan(fd4.y, fd4.x);
        if (laserMix > 0.001) {
            drawLaser(p, f4c, fd4, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f4a, vec2(cos(a4 + 0.25), sin(a4 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f4b, vec2(cos(a4 - 0.25), sin(a4 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f4c, fd4, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f4a, vec2(cos(a4 + 0.25), sin(a4 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f4b, vec2(cos(a4 - 0.25), sin(a4 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    // Arm 5
    if (numArms > 4.5) {
        vec2 w5, fd5, f5a, f5b, f5c;
        drawArm(p, base5, target5, grip, armScale, segWidth,
                armColor, accentColor, L, px, -1.0,
                col, armMask, w5, fd5, f5a, f5b, f5c);
        float a5 = atan(fd5.y, fd5.x);
        if (laserMix > 0.001) {
            drawLaser(p, f5c, fd5, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f5a, vec2(cos(a5 + 0.25), sin(a5 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f5b, vec2(cos(a5 - 0.25), sin(a5 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f5c, fd5, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f5a, vec2(cos(a5 + 0.25), sin(a5 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f5b, vec2(cos(a5 - 0.25), sin(a5 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    // Arm 6
    if (numArms > 5.5) {
        vec2 w6, fd6, f6a, f6b, f6c;
        drawArm(p, base6, target6, grip, armScale, segWidth,
                armColor, accentColor, L, px, 1.0,
                col, armMask, w6, fd6, f6a, f6b, f6c);
        float a6 = atan(fd6.y, fd6.x);
        if (laserMix > 0.001) {
            drawLaser(p, f6c, fd6, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f6a, vec2(cos(a6 + 0.25), sin(a6 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f6b, vec2(cos(a6 - 0.25), sin(a6 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f6c, fd6, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f6a, vec2(cos(a6 + 0.25), sin(a6 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f6b, vec2(cos(a6 - 0.25), sin(a6 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    // Arm 7
    if (numArms > 6.5) {
        vec2 w7, fd7, f7a, f7b, f7c;
        drawArm(p, base7, target7, grip, armScale, segWidth,
                armColor, accentColor, L, px, -1.0,
                col, armMask, w7, fd7, f7a, f7b, f7c);
        float a7 = atan(fd7.y, fd7.x);
        if (laserMix > 0.001) {
            drawLaser(p, f7c, fd7, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f7a, vec2(cos(a7 + 0.25), sin(a7 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f7b, vec2(cos(a7 - 0.25), sin(a7 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f7c, fd7, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f7a, vec2(cos(a7 + 0.25), sin(a7 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f7b, vec2(cos(a7 - 0.25), sin(a7 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    // Arm 8
    if (numArms > 7.5) {
        vec2 w8, fd8, f8a, f8b, f8c;
        drawArm(p, base8, target8, grip, armScale, segWidth,
                armColor, accentColor, L, px, 1.0,
                col, armMask, w8, fd8, f8a, f8b, f8c);
        float a8 = atan(fd8.y, fd8.x);
        if (laserMix > 0.001) {
            drawLaser(p, f8c, fd8, grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f8a, vec2(cos(a8 + 0.25), sin(a8 + 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
            drawLaser(p, f8b, vec2(cos(a8 - 0.25), sin(a8 - 0.25)), grip, beamColor * laserMix, px, laserSize, shot, lenMul, glowMul, col, armMask);
        }
        if (bulletMix > 0.001) {
            drawBullets(p, f8c, fd8, grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f8a, vec2(cos(a8 + 0.25), sin(a8 + 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
            drawBullets(p, f8b, vec2(cos(a8 - 0.25), sin(a8 - 0.25)), grip, beamColor, px, laserSize, bulletSpeed, glowMul, bulletMix, col, armMask);
        }
    }

    gl_FragColor = vec4(col, transparentBg ? armMask : 1.0);
}
