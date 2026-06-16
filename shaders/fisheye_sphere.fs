/*{
  "DESCRIPTION": "3D Fisheye Sphere — glass ball centered on screen (diameter locked to screen height) that refracts the source image/layer below with chromatic dispersion",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Effect", "3D"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Source", "TYPE": "image" },
    { "NAME": "sourceScale", "LABEL": "Source Scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "sourceOffsetX", "LABEL": "Source Offset X", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "sourceOffsetY", "LABEL": "Source Offset Y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "lensSize", "LABEL": "Lens Size", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.4, "MAX": 1.3 },
    { "NAME": "ior", "LABEL": "IOR", "TYPE": "float", "DEFAULT": 1.5, "MIN": 1.0, "MAX": 2.5 },
    { "NAME": "chromatic", "LABEL": "Chromatic Split", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.0, "MAX": 0.25 },
    { "NAME": "perspective", "LABEL": "Perspective", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "spin", "LABEL": "Spin Speed", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "rimSoftness", "LABEL": "Rim Softness", "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.0, "MAX": 0.08 },
    { "NAME": "rimGlow", "LABEL": "Rim Glow", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.2 },
    { "NAME": "neonIntensity", "LABEL": "Neon Ring", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "neonWidth", "LABEL": "Neon Width", "TYPE": "float", "DEFAULT": 0.015, "MIN": 0.002, "MAX": 0.08 },
    { "NAME": "neonBloom", "LABEL": "Neon Bloom", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.01, "MAX": 0.6 },
    { "NAME": "neonColor", "LABEL": "Neon Color", "TYPE": "color", "DEFAULT": [1.0, 0.2, 0.75, 1.0] },
    { "NAME": "neonPulse", "LABEL": "Neon Pulse", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fresnel", "LABEL": "Fresnel", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "flare", "LABEL": "Lens Flare", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "flareAngle", "LABEL": "Flare Angle", "TYPE": "float", "DEFAULT": -0.6, "MIN": -3.1416, "MAX": 3.1416 },
    { "NAME": "flareX", "LABEL": "Flare X", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "flareY", "LABEL": "Flare Y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "flareColor", "LABEL": "Flare Color", "TYPE": "color", "DEFAULT": [1.0, 0.92, 0.78, 1.0] },
    { "NAME": "ghostColor", "LABEL": "Ghost Color", "TYPE": "color", "DEFAULT": [0.6, 0.8, 1.0, 1.0] },
    { "NAME": "streakColor", "LABEL": "Streak Color", "TYPE": "color", "DEFAULT": [1.0, 0.88, 0.72, 1.0] },
    { "NAME": "shadowStrength", "LABEL": "Shadow Strength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shadowSize", "LABEL": "Shadow Size", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.01, "MAX": 1.0 },
    { "NAME": "shadowOffsetX", "LABEL": "Shadow Offset X", "TYPE": "float", "DEFAULT": 0.05, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "shadowOffsetY", "LABEL": "Shadow Offset Y", "TYPE": "float", "DEFAULT": -0.08, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "shadowColor", "LABEL": "Shadow Color", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "bgMode", "LABEL": "Outside Lens", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Source", "Black", "Transparent", "Dispersion"] },
    { "NAME": "dispersionHue", "LABEL": "Dispersion Hue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "dispersionIntensity", "LABEL": "Dispersion Intensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

// 3D Fisheye Sphere
// Glass sphere sits at origin of a screen-aligned coord system where y spans [-1, 1]
// (x scaled by aspect). Radius = lensSize, so at lensSize=1 the sphere diameter
// equals the full screen height regardless of aspect ratio. Rays from the camera
// refract entering and exiting the sphere, then project onto a plane behind it
// and sample the source image at the resulting UV.

bool sphereHit(vec3 ro, vec3 rd, vec3 c, float r, out float t0, out float t1) {
    vec3 oc = ro - c;
    float b = dot(oc, rd);
    float cc = dot(oc, oc) - r * r;
    float h = b * b - cc;
    if (h < 0.0) return false;
    h = sqrt(h);
    t0 = -b - h;
    t1 = -b + h;
    return true;
}

// Trace a ray through the glass sphere with given IOR; return sampled UV on the
// back-plane, or (-1, -1) if ray misses or totally reflects out.
vec2 traceLens(vec3 ro, vec3 rd, vec3 sc, float r, float eta, float planeZ, float spinA) {
    float t0, t1;
    if (!sphereHit(ro, rd, sc, r, t0, t1)) return vec2(-1.0);

    vec3 p0 = ro + rd * t0;
    vec3 n0 = normalize(p0 - sc);
    vec3 rd1 = refract(rd, n0, 1.0 / eta);
    if (dot(rd1, rd1) < 1e-5) return vec2(-1.0);

    // Second intersection with sphere along refracted ray
    vec3 oc = p0 - sc;
    float b = dot(oc, rd1);
    float cc = dot(oc, oc) - r * r;
    float h = b * b - cc;
    if (h < 0.0) return vec2(-1.0);
    float tExit = -b + sqrt(h);
    vec3 p1 = p0 + rd1 * tExit;
    vec3 n1 = normalize(sc - p1); // inward-facing normal at exit
    vec3 rd2 = refract(rd1, n1, eta);
    if (dot(rd2, rd2) < 1e-5) rd2 = reflect(rd1, n1);

    // Project onto background plane behind sphere
    if (rd2.z <= 0.0001) return vec2(-1.0);
    float tPlane = (planeZ - p1.z) / rd2.z;
    vec3 hit = p1 + rd2 * tPlane;

    // Optional in-plane rotation of the sampled pattern
    float cs = cos(spinA), sn = sin(spinA);
    hit.xy = mat2(cs, -sn, sn, cs) * hit.xy;

    // Map back to source-image UV. Source spans aspect-wide, height = 2.
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uv = vec2(hit.x / aspect, hit.y) * 0.5 + 0.5;
    return uv;
}

// Apply user-controlled scale (around center) + offset to a UV in [0,1]
vec2 transformUV(vec2 uv) {
    vec2 centered = uv - 0.5;
    centered /= max(sourceScale, 1e-4);
    centered += vec2(sourceOffsetX, sourceOffsetY) * 0.5;
    return centered + 0.5;
}

vec4 sampleSource(vec2 uv) {
    vec2 t = transformUV(uv);
    if (t.x < 0.0 || t.x > 1.0 || t.y < 0.0 || t.y > 1.0) return vec4(0.0);
    return texture2D(inputTex, t);
}

// HSV → RGB for the dispersion spectrum
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Procedural prismatic dispersion background — rainbow streaks radiating
// from screen center, softly animated. `d` is distance-from-center, `r` is lens radius.
vec3 dispersionBG(vec2 p, float d, float r) {
    float ang = atan(p.y, p.x);
    float t = TIME * 0.08;

    // Radial hue ramp + angular streaks
    float radial = d / max(r, 1e-4);
    float hueBase = dispersionHue + radial * 0.35 + sin(ang * 3.0 + t) * 0.08;
    vec3 spectrum = hsv2rgb(vec3(fract(hueBase), 0.85, 1.0));

    // Streaks: high-frequency angular bands modulated by radius
    float streaks = 0.5 + 0.5 * sin(ang * 24.0 + t * 4.0 + radial * 6.0);
    streaks = pow(streaks, 6.0);

    // Glow falloff: brightest near the sphere rim, fading outward, plus subtle core halo
    float rimGlowField = exp(-pow(d - r * 1.02, 2.0) * 18.0);
    float outerFalloff = exp(-max(d - r, 0.0) * 1.6);
    float innerFalloff = 1.0 - smoothstep(0.0, r * 0.9, d);

    vec3 col = spectrum * (streaks * 0.6 + 0.25) * outerFalloff;
    col += spectrum * rimGlowField * 1.4;
    col += spectrum * innerFalloff * 0.15;

    // Dark vignette undertone so the streaks pop
    col *= 0.9 + 0.4 * pow(1.0 - clamp(d / (r * 2.5), 0.0, 1.0), 1.5);

    return col * dispersionIntensity;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Centered screen coords: y in [-1, 1], x scaled by aspect
    vec2 p = (fragCoord / RENDERSIZE.y) * 2.0 - vec2(aspect, 1.0);

    // Glass sphere
    vec3 sc = vec3(0.0, 0.0, 0.0);
    float r = lensSize;
    float planeZ = r * 1.5; // background plane sits just behind the sphere

    // Camera: orthographic base + optional perspective splay for depth feel
    vec3 ro = vec3(p, -2.0);
    vec3 rd = normalize(vec3(p * perspective, 1.0));

    float d = length(p);
    float mask = 1.0 - smoothstep(r - rimSoftness, r + rimSoftness, d);
    float spinA = TIME * spin;

    bool hasTex = IMG_SIZE_inputTex.x > 0.5;
    vec4 bg = hasTex ? sampleSource(uv) : vec4(0.0);

    // Outside-lens backdrop selection
    vec4 outsideBg;
    if (bgMode == 0 && hasTex) outsideBg = bg;
    else if (bgMode == 3)      outsideBg = vec4(dispersionBG(p, d, r), 1.0);
    else if (bgMode == 2)      outsideBg = vec4(0.0);
    else                        outsideBg = vec4(0.0, 0.0, 0.0, 1.0);

    // Drop shadow — sphere casts a soft dark disc onto the backdrop behind it.
    // Distance to shadow center, normalised by sphere radius + user spread.
    if (shadowStrength > 0.001) {
        vec2 shadowCenter = vec2(shadowOffsetX, shadowOffsetY);
        float sd = length(p - shadowCenter);
        float sr = r + shadowSize;
        // Soft falloff from the virtual shadow edge outward
        float shadow = 1.0 - smoothstep(r * 0.6, sr, sd);
        shadow *= shadowStrength;
        outsideBg.rgb = mix(outsideBg.rgb, shadowColor.rgb, shadow);
        // Keep the backdrop opaque where the shadow sits (important for Transparent mode)
        outsideBg.a = max(outsideBg.a, shadow);
    }

    vec4 col;
    if (mask > 0.001 && hasTex) {
        // Chromatic dispersion: three IOR samples
        vec2 uvR = traceLens(ro, rd, sc, r, ior - chromatic, planeZ, spinA);
        vec2 uvG = traceLens(ro, rd, sc, r, ior,             planeZ, spinA);
        vec2 uvB = traceLens(ro, rd, sc, r, ior + chromatic, planeZ, spinA);

        vec4 sR = sampleSource(uvR);
        vec4 sG = sampleSource(uvG);
        vec4 sB = sampleSource(uvB);
        vec4 lens = vec4(sR.r, sG.g, sB.b, max(max(sR.a, sG.a), sB.a));

        // Fresnel darkening + rim glow at grazing angles
        float fres = pow(smoothstep(r * 0.4, r, d), 2.0);
        lens.rgb *= mix(1.0, 1.0 - fresnel, fres);
        float rim = pow(smoothstep(r * 0.82, r, d), 3.0);
        lens.rgb += rim * rimGlow;

        col = mix(outsideBg, lens, mask);
        if (bgMode == 2) col.a = mix(outsideBg.a, lens.a, mask);
    } else if (mask > 0.001) {
        // No texture bound: fill the disc with a subtle radial fallback so the lens is visible
        float glow = 1.0 - smoothstep(0.0, r, d);
        vec3 fill = vec3(0.08, 0.12, 0.18) + glow * vec3(0.15, 0.2, 0.3);
        float rim = pow(smoothstep(r * 0.82, r, d), 3.0);
        fill += rim * rimGlow;
        vec4 lens = vec4(fill, 1.0);
        col = mix(outsideBg, lens, mask);
        if (bgMode == 2) col.a = mix(outsideBg.a, lens.a, mask);
    } else {
        col = outsideBg;
    }

    // --- Neon ring light around the sphere rim ---
    // Bright saturated core at the rim + soft outer halo that bleeds into the backdrop.
    if (neonIntensity > 0.001) {
        float ringDist = abs(d - r);
        // Sharp core line — falls off quickly in both directions
        float core = exp(-(ringDist * ringDist) / max(neonWidth * neonWidth, 1e-6));
        // Wide outer/inner halo — slow exponential falloff
        float halo = exp(-ringDist / max(neonBloom, 1e-4));
        // Optional sine-wave pulse (0 = steady, 1 = deep breath)
        float pulse = 1.0 - neonPulse * 0.5 + neonPulse * 0.5 * sin(TIME * 2.2);
        vec3 neon = neonColor.rgb * (core * 1.8 + halo * 0.45) * neonIntensity * pulse;
        col.rgb += neon;
    }

    // --- Lens flare (lives inside the sphere, warped by glass curvature) ---
    if (flare > 0.001 && mask > 0.001) {
        // Warp screen-space position using the sphere's surface projection:
        // z = sqrt(r^2 - |p|^2) is the depth of the glass surface at this pixel.
        // Dividing by z stretches space toward the rim — the classic fisheye warp.
        float z = sqrt(max(r * r - dot(p, p), 1e-4));
        vec2 pw = p * (r / z); // warped pixel coord

        // Light source in warped space: angle places it on/near the rim, X/Y offset
        vec2 lightPos = vec2(cos(flareAngle), sin(flareAngle)) * (r * 0.9)
                        + vec2(flareX, flareY);
        vec2 toLight = lightPos - pw;
        float distToLight = length(toLight);
        vec2 lightDir = toLight / max(distToLight, 1e-4);

        vec3 fc = flareColor.rgb;
        vec3 sc2 = streakColor.rgb;
        vec3 gc = ghostColor.rgb;
        vec3 flareCol = vec3(0.0);

        // 1. Hotspot — specular core at the light position (warped)
        float hot = exp(-distToLight * distToLight * 80.0);
        flareCol += fc * hot * 3.0;

        // 2. Anamorphic horizontal streak — also warped so it bends with the glass
        vec2 streakDir = vec2(1.0, 0.0);
        vec2 rel = pw - lightPos;
        float along = dot(rel, streakDir);
        float perp  = dot(rel, vec2(-streakDir.y, streakDir.x));
        float streak = exp(-perp * perp * 1000.0) * exp(-abs(along) * 1.4);
        flareCol += sc2 * streak * 1.2;

        // 3. Inner ring / caustic near the rim, biased toward the light
        float ring = exp(-pow(d - r * 0.94, 2.0) * 600.0);
        float dirWeight = clamp(dot(normalize(p + 1e-4), vec2(cos(flareAngle), sin(flareAngle))) * 0.5 + 0.5, 0.0, 1.0);
        flareCol += fc * ring * dirWeight * 0.9;

        // 4. Ghost dots along the axis through sphere center — positions in warped space
        for (int i = 0; i < 5; i++) {
            float gi = float(i);
            float t = -0.35 + gi * 0.35;
            vec2 gpos = lightPos * t;
            float gd = distance(pw, gpos);
            float gsize = 0.05 + 0.02 * gi;
            float ghost = exp(-gd * gd / (gsize * gsize));
            vec3 ghostTint = mix(fc, gc, fract(gi * 0.37));
            flareCol += ghostTint * ghost * 0.35;
        }

        // 5. Soft radial bloom — falls off with warped distance so it stretches near the rim
        float bloom = 0.08 / (distToLight * distToLight + 0.02);
        flareCol += fc * bloom * 0.4;

        // Keep the flare strictly inside the sphere, fading with the rim mask
        col.rgb += flareCol * flare * mask;
    }

    if (transparentBg) {
        col.a = mask;
    }

    gl_FragColor = col;
}
