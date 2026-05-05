/*{
  "DESCRIPTION": "Plasma Torus — 3D raymarched spinning torus with iridescent plasma bands and neon rim lighting. Dark cinematic style.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "torusR",     "LABEL": "Major Radius","TYPE": "float","DEFAULT": 1.2, "MIN": 0.5,  "MAX": 2.0  },
    { "NAME": "tubeR",      "LABEL": "Tube Radius", "TYPE": "float","DEFAULT": 0.38,"MIN": 0.08, "MAX": 0.7  },
    { "NAME": "spinSpeed",  "LABEL": "Spin Speed",  "TYPE": "float","DEFAULT": 0.3, "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float","DEFAULT": 2.5, "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "audioPulse", "LABEL": "Audio Pulse", "TYPE": "float","DEFAULT": 1.0, "MIN": 0.0,  "MAX": 2.0  }
  ]
}*/

float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

float scene(vec3 p) {
    // Axis spin
    float ang = TIME * spinSpeed;
    float ca = cos(ang); float sa = sin(ang);
    vec3 q = vec3(ca*p.x + sa*p.z, p.y, -sa*p.x + ca*p.z);
    // 30-degree tilt for a dramatic angle
    float tilt = 0.52;
    vec3 tp = vec3(q.x,
                   cos(tilt)*q.y - sin(tilt)*q.z,
                   sin(tilt)*q.y + cos(tilt)*q.z);
    float pulse = 1.0 + audioBass * audioPulse * 0.07;
    return sdTorus(tp, torusR * pulse, tubeR);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p+e.xyy)-scene(p-e.xyy),
        scene(p+e.yxy)-scene(p-e.yxy),
        scene(p+e.yyx)-scene(p-e.yyx)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float camA = TIME * 0.07;
    float camEl = sin(TIME * 0.11) * 0.25 + 0.55;
    float camDist = 4.2;
    vec3 ro = vec3(sin(camA)*camDist*cos(camEl), sin(camEl)*camDist, cos(camA)*camDist*cos(camEl));
    vec3 ta = vec3(0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.5*fw);

    // Deep void background with faint violet haze
    float bgH = dot(rd, vec3(0.0, 1.0, 0.0)) * 0.5 + 0.5;
    vec3 bg = mix(vec3(0.0, 0.0, 0.008), vec3(0.015, 0.0, 0.04), bgH*bgH);

    float t = 0.1; bool hit = false;
    for (int i = 0; i < 64; i++) {
        float d = scene(ro + rd*t);
        if (d < 0.001) { hit = true; break; }
        if (t > 12.0) break;
        t += d * 0.85;
    }

    vec3 col = bg;
    if (hit) {
        vec3 p = ro + rd*t;
        vec3 n = getNormal(p);
        vec3 L1 = normalize(vec3(1.0, 1.5, 0.3));
        vec3 L2 = normalize(vec3(-0.4, 0.4, -1.0));

        // Plasma color from surface angles on torus
        float az = atan(p.z, p.x);                       // azimuthal
        float po = atan(p.y, length(p.xz) - torusR);     // poloidal
        float plasma = sin(az * 3.0 + TIME * 1.3) * 0.4
                     + sin(po * 5.0 + TIME * 0.7) * 0.4
                     + sin(az * 7.0 - po * 2.0 + TIME * 0.5) * 0.2;
        plasma = clamp(plasma * 0.5 + 0.5, 0.0, 1.0);

        // 4-color fully saturated: cyan → magenta → gold → violet
        vec3 c0 = vec3(0.0,  1.0,  0.9);
        vec3 c1 = vec3(1.0,  0.0,  0.7);
        vec3 c2 = vec3(1.0,  0.75, 0.0);
        vec3 c3 = vec3(0.45, 0.0,  1.0);
        vec3 basecol;
        float ph4 = plasma * 4.0;
        if      (ph4 < 1.0) basecol = mix(c0, c1, ph4);
        else if (ph4 < 2.0) basecol = mix(c1, c2, ph4 - 1.0);
        else if (ph4 < 3.0) basecol = mix(c2, c3, ph4 - 2.0);
        else                basecol = mix(c3, c0, ph4 - 3.0);

        float diff = max(dot(n, L1), 0.0)*0.55 + max(dot(n, L2), 0.0)*0.2 + 0.25;
        float spec = pow(max(dot(reflect(-L1, n), -rd), 0.0), 48.0);
        float rim  = pow(clamp(1.0 - dot(n, -rd), 0.0, 1.0), 3.5);
        // Ink silhouette: darken at grazing angles
        float face = smoothstep(0.0, 0.22, dot(n, -rd));

        col  = basecol * diff * hdrPeak * face;
        col += basecol * rim  * hdrPeak * 1.3;  // HDR rim
        col += vec3(1.0) * spec * hdrPeak;       // HDR spec white
    }

    gl_FragColor = vec4(col, 1.0);
}
