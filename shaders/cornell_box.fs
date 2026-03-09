/*{
    "DESCRIPTION": "Raymarched Cornell box — bounce lighting, ambient occlusion, movable sphere. Based on @XorDev",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "camDist", "LABEL": "Camera Dist", "TYPE": "float", "DEFAULT": 2.8, "MIN": 1.5, "MAX": 8.0 },
        { "NAME": "camX", "LABEL": "Camera X", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.5, "MAX": 1.5 },
        { "NAME": "camY", "LABEL": "Camera Y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.5, "MAX": 1.5 },
        { "NAME": "fov", "LABEL": "FOV", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 2.5 },
        { "NAME": "leftColor", "LABEL": "Left Wall", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
        { "NAME": "rightColor", "LABEL": "Right Wall", "TYPE": "color", "DEFAULT": [0.2, 0.8, 0.3, 1.0] },
        { "NAME": "backColor", "LABEL": "Back Wall", "TYPE": "color", "DEFAULT": [0.9, 0.9, 0.85, 1.0] },
        { "NAME": "lightBright", "LABEL": "Light", "TYPE": "float", "DEFAULT": 0.012, "MIN": 0.001, "MAX": 0.05 },
        { "NAME": "bounceStr", "LABEL": "Bounce", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "sphereX", "LABEL": "Sphere X", "TYPE": "float", "DEFAULT": 0.3, "MIN": -0.9, "MAX": 0.9 },
        { "NAME": "sphereY", "LABEL": "Sphere Y", "TYPE": "float", "DEFAULT": -0.65, "MIN": -0.9, "MAX": 0.9 },
        { "NAME": "sphereZ", "LABEL": "Sphere Z", "TYPE": "float", "DEFAULT": -0.3, "MIN": -1.5, "MAX": 0.5 },
        { "NAME": "sphereR", "LABEL": "Sphere Size", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.1, "MAX": 0.9 },
        { "NAME": "aoStrength", "LABEL": "AO", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

// Box: 5-walled room, open front. Walls at x=±1, y=±1, back z=-1.
// Camera looks in from the front opening.

// SDF: distance to nearest wall of a 5-walled box (no front wall)
float sdRoom(vec3 p) {
    float dLeft  = p.x + 1.0;   // x = -1
    float dRight = 1.0 - p.x;   // x = +1
    float dFloor = p.y + 1.0;   // y = -1
    float dCeil  = 1.0 - p.y;   // y = +1
    float dBack  = p.z + 1.0;   // z = -1
    return min(dLeft, min(dRight, min(dFloor, min(dCeil, dBack))));
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;

    // Camera positioned outside the open front, looking in (-z)
    vec3 ro = vec3(camX, camY, camDist);
    vec2 look = (mousePos - 0.5) * 0.3;
    vec3 rd = normalize(vec3(uv * fov + look, -1.0));

    vec3 p = ro;
    vec3 sph = vec3(sphereX, sphereY, sphereZ);

    // Raymarch
    float totalDist = 0.0;
    int steps = 0;
    bool hitSphere = false;

    for (int i = 0; i < 100; i++) {
        // Room interior distance
        float roomD = sdRoom(p);

        // Sphere
        float sphD = length(p - sph) - sphereR;

        float d = min(roomD, sphD);
        d = max(d, 0.001);

        p += rd * d;
        totalDist += d;
        steps = i;

        if (d < 0.0005) {
            hitSphere = (sphD < roomD);
            break;
        }
        if (totalDist > 12.0) break;
    }

    // If we didn't hit anything (ray escaped through front opening), dark bg
    if (totalDist > 11.0) {
        gl_FragColor = vec4(vec3(0.02), 1.0);
        return;
    }

    // Determine surface normal and color
    vec3 n;
    vec3 surfColor;

    if (hitSphere) {
        n = normalize(p - sph);
        surfColor = vec3(0.85);
    } else {
        float dLeft  = abs(p.x + 1.0);
        float dRight = abs(p.x - 1.0);
        float dFloor = abs(p.y + 1.0);
        float dCeil  = abs(p.y - 1.0);
        float dBack  = abs(p.z + 1.0);

        float closest = min(dLeft, min(dRight, min(dFloor, min(dCeil, dBack))));
        float thr = 0.02;

        if (dLeft < thr) {
            n = vec3(1.0, 0.0, 0.0);
            surfColor = leftColor.rgb;
        } else if (dRight < thr) {
            n = vec3(-1.0, 0.0, 0.0);
            surfColor = rightColor.rgb;
        } else if (dFloor < thr) {
            n = vec3(0.0, 1.0, 0.0);
            surfColor = vec3(0.9);
        } else if (dCeil < thr) {
            n = vec3(0.0, -1.0, 0.0);
            surfColor = vec3(0.95);
        } else {
            n = vec3(0.0, 0.0, 1.0);
            surfColor = backColor.rgb;
        }
    }

    // ── Lighting ──

    // Ceiling light — rectangular area centered on ceiling
    vec3 lightPos = vec3(0.0, 0.92, -0.2);
    vec3 toLight = normalize(lightPos - p);
    float diff = max(dot(n, toLight), 0.0);

    // Area light glow on ceiling
    float isLight = step(0.98, p.y) * step(abs(p.x), 0.35) * step(abs(p.z + 0.2), 0.35);
    vec3 lightEmit = vec3(isLight) * 2.0;

    // Soft top light falloff
    float lightDist2 = dot(lightPos - p, lightPos - p);
    float topLight = lightBright / (lightDist2 + 0.01);

    // Color bleeding — walls tint nearby surfaces
    float bleedLeft  = bounceStr * max(0.0, 0.5 - abs(p.x + 1.0)) * 0.2;
    float bleedRight = bounceStr * max(0.0, 0.5 - abs(p.x - 1.0)) * 0.2;
    vec3 bounce = leftColor.rgb * bleedLeft + rightColor.rgb * bleedRight;

    // Combine
    vec3 col = surfColor * (diff * 0.65 + 0.08) + topLight * vec3(1.0, 0.98, 0.92) + bounce + lightEmit;

    // AO — darkens corners and crevices
    float ao = 1.0 - aoStrength * float(steps) / 100.0;
    col *= ao;

    // Subtle distance fog
    col *= exp(-totalDist * 0.04);

    // Audio reactivity — light flickers with bass
    col *= 1.0 + audioBass * 0.3;

    // Gamma
    col = pow(clamp(col, 0.0, 1.0), vec3(0.45));

    gl_FragColor = vec4(col, 1.0);
}
