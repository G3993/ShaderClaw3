/*{
  "CATEGORIES": ["Generator", "Atmospheric", "Audio Reactive"],
  "DESCRIPTION": "Earth from orbit — rotating globe with day/night terminator line, city lights blooming across the dark side, atmospheric scattering halo at the limb. Bass kicks accelerate rotation, treble drives auroras at the poles. The blue-marble shot every space film loves",
  "INPUTS": [
    { "NAME": "planetRadius",       "LABEL": "Planet Radius",     "TYPE": "float", "MIN": 0.20, "MAX": 0.55, "DEFAULT": 0.38 },
    { "NAME": "rotateSpeed",        "LABEL": "Rotate Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.10 },
    { "NAME": "sunOrbitSpeed",      "LABEL": "Sun Orbit Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.07 },
    { "NAME": "continentThreshold", "LABEL": "Continent Cut",     "TYPE": "float", "MIN": 0.30, "MAX": 0.70, "DEFAULT": 0.50 },
    { "NAME": "cityDensity",        "LABEL": "City Density",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "cityIntensity",      "LABEL": "City Intensity",    "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.4 },
    { "NAME": "atmosphereGlow",     "LABEL": "Atmosphere Glow",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.5,  "DEFAULT": 1.1 },
    { "NAME": "auroraIntensity",    "LABEL": "Aurora Intensity",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.85 },
    { "NAME": "audioReact",         "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "oceanColor",         "LABEL": "Ocean",             "TYPE": "color", "DEFAULT": [0.05, 0.18, 0.40, 1.0] },
    { "NAME": "landColor",          "LABEL": "Land",              "TYPE": "color", "DEFAULT": [0.22, 0.42, 0.18, 1.0] },
    { "NAME": "nightColor",         "LABEL": "Night",             "TYPE": "color", "DEFAULT": [0.02, 0.03, 0.07, 1.0] },
    { "NAME": "atmosphereColor",    "LABEL": "Atmosphere",        "TYPE": "color", "DEFAULT": [0.30, 0.65, 1.00, 1.0] },
    { "NAME": "spaceColor",         "LABEL": "Space",             "TYPE": "color", "DEFAULT": [0.00, 0.00, 0.01, 1.0] }
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 r = mat2(0.8, -0.6, 0.6, 0.8);
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = r * p * 2.07;
        a *= 0.5;
    }
    return v;
}

// Continent generator — wraps in longitude so the seam is invisible.
float continentField(float lon, float lat) {
    // Wrap longitude by sampling cos/sin so values are continuous around the globe.
    vec2 wrap = vec2(cos(lon), sin(lon)) * 1.6;
    float base = fbm(vec2(wrap.x * 1.4, lat * 1.8 + wrap.y * 0.7));
    // Add a second octave keyed to latitude for vertical variation.
    base += 0.35 * fbm(vec2(wrap.x * 3.1 + 5.2, lat * 3.4 - wrap.y * 1.1));
    base = base / 1.35;
    // Squeeze poles slightly so we get less "land at the poles" bias.
    base -= 0.10 * smoothstep(0.55, 1.0, abs(lat) / 1.5708);
    return base;
}

// City light density — clusters per region, denser in mid-latitudes.
float cityField(float lon, float lat) {
    // Hash a coarse grid, blur it.
    vec2 g = vec2(lon * 4.0, lat * 6.0);
    float cluster = fbm(g + 13.7);
    // Mid-latitude weighting (peak around |lat| ~ 0.5-0.9 rad ~ 30-50 deg).
    float latBand = exp(-pow((abs(lat) - 0.7) * 1.6, 2.0));
    return smoothstep(0.45, 0.85, cluster) * latBand;
}

void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 uv = (gl_FragCoord.xy - 0.5 * res) / min(res.x, res.y);

    // Audio shorthand.
    float bass = audioBass * audioReact;
    float treb = audioHigh * audioReact;

    float R = planetRadius;
    float r2 = uv.x * uv.x + uv.y * uv.y;
    float R2 = R * R;

    // Sun direction in scene space — slowly orbits in XZ so terminator sweeps.
    float sa = TIME * sunOrbitSpeed;
    vec3 sunDir = normalize(vec3(cos(sa), 0.18, sin(sa)));

    // Background space + faint star field.
    vec3 col = spaceColor.rgb;
    {
        vec2 sp = floor(gl_FragCoord.xy * 1.0);
        float h = hash21(sp);
        float star = step(0.9985, h);
        float twinkle = 0.5 + 0.5 * sin(TIME * (2.0 + hash11(h * 47.0) * 6.0) + h * 31.0);
        col += vec3(0.9, 0.95, 1.0) * star * twinkle;
    }

    if (r2 < R2) {
        // Sphere surface point (z toward camera).
        float z = sqrt(R2 - r2);
        vec3 N = vec3(uv.x, uv.y, z) / R;

        // Latitude / longitude with rotation.
        float lat = asin(clamp(N.y, -1.0, 1.0));
        float lonBase = atan(N.x, N.z);
        float lon = lonBase + TIME * (rotateSpeed + bass * 0.6);

        // Continents.
        float cont = continentField(lon, lat);
        float land = smoothstep(continentThreshold - 0.04,
                                continentThreshold + 0.04, cont);

        // Elevation tint — higher fbm = more brown, lower land = greener.
        vec3 lo = mix(landColor.rgb, landColor.rgb * vec3(1.45, 1.10, 0.70),
                      smoothstep(continentThreshold + 0.02,
                                 continentThreshold + 0.20, cont));
        // Polar ice caps.
        float ice = smoothstep(1.05, 1.35, abs(lat));
        lo = mix(lo, vec3(0.92, 0.95, 0.98), ice);

        vec3 surface = mix(oceanColor.rgb, lo, land);

        // Sun lighting — Lambert against sphere normal.
        float lambert = clamp(dot(N, sunDir), 0.0, 1.0);
        // Ocean specular highlight.
        vec3 V = vec3(0.0, 0.0, 1.0);
        vec3 H = normalize(sunDir + V);
        float spec = pow(max(dot(N, H), 0.0), 90.0) * (1.0 - land) * lambert;

        // Day color.
        vec3 dayCol = surface * (0.20 + 0.95 * lambert) + vec3(1.0, 0.95, 0.85) * spec * 0.6;

        // Night color — base + city lights.
        float cityBlob = cityField(lon, lat);
        // Pixel-scale dot stipple so individual lights twinkle.
        vec2 cellUV = vec2(lon * 80.0, lat * 60.0);
        float dotH = hash21(floor(cellUV));
        float dotMask = step(1.0 - cityDensity * 0.5, dotH);
        float twinkleC = 0.7 + 0.3 * sin(TIME * 3.0 + dotH * 60.0);
        float cities = land * cityBlob * dotMask * twinkleC;
        vec3 cityCol = mix(vec3(1.0, 0.78, 0.35), vec3(1.0, 0.92, 0.65), dotH);
        vec3 nightCol = nightColor.rgb + cityCol * cities * cityIntensity;

        // Aurora — green/violet ribbons at high latitudes, on night side.
        float poleBand = smoothstep(0.95, 1.30, abs(lat))
                       * (1.0 - smoothstep(1.40, 1.55, abs(lat)));
        float ribbon = fbm(vec2(lon * 4.0 + TIME * 0.6,
                                lat * 8.0 + TIME * 0.3));
        ribbon = smoothstep(0.45, 0.85, ribbon);
        float auroraAmt = poleBand * ribbon
                       * (0.5 + 1.5 * treb)
                       * auroraIntensity
                       * (1.0 - lambert);
        vec3 auroraCol = mix(vec3(0.20, 0.95, 0.55),
                             vec3(0.55, 0.30, 0.95),
                             0.5 + 0.5 * sin(lon * 3.0 + TIME * 0.8));
        nightCol += auroraCol * auroraAmt;

        // Terminator blend — smoothstep around lambert ~ 0.
        // Use the raw signed dot so we get a soft band centered on the line.
        float signedL = dot(N, sunDir);
        float dayMix = smoothstep(-0.12, 0.18, signedL);
        // Warm reddish band right on the terminator.
        float termBand = exp(-pow(signedL * 7.0, 2.0));
        vec3 termTint = vec3(1.0, 0.55, 0.30) * termBand * 0.35;

        vec3 planet = mix(nightCol, dayCol, dayMix) + termTint * (0.5 + dayMix * 0.5);

        // Soft edge anti-alias toward limb.
        float edge = smoothstep(R2, R2 * 0.94, r2);
        col = mix(col, planet, edge);
    }

    // Atmospheric halo — outside the disc, falls off radially.
    {
        float d = sqrt(r2);
        // Inner glow band hugging the lit limb.
        float halo = smoothstep(R * 1.18, R, d) * (1.0 - smoothstep(R, R * 0.985, d));
        // Sun-side enhancement: stronger on the day limb.
        vec2 sunScreen = sunDir.xy;
        float sunFacing = clamp(dot(normalize(uv + 1e-5), normalize(sunScreen + 1e-5)), 0.0, 1.0);
        halo *= 0.55 + 0.85 * sunFacing;
        col += atmosphereColor.rgb * halo * atmosphereGlow * (1.0 + bass * 0.4);

        // Outer faint scatter.
        float outer = smoothstep(R * 1.45, R * 1.02, d) * (1.0 - smoothstep(R * 1.02, R, d));
        col += atmosphereColor.rgb * outer * 0.18 * atmosphereGlow;
    }

    // Slight vignette so the planet pops.
    float vig = 1.0 - 0.25 * dot(uv, uv);
    col *= vig;

    gl_FragColor = vec4(col, 1.0);
}
