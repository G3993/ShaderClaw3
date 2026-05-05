/*{
    "DESCRIPTION": "Glass Prism Spectrum — standalone 3D raymarched glass prism splitting white light into a rainbow caustic fan. HDR spectral peaks on deep navy velvet.",
    "CREDIT": "ShaderClaw auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "prismTilt",  "LABEL": "Prism Tilt",   "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "specWidth",  "LABEL": "Spectrum Width","TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.2,  "MAX": 2.5 },
        { "NAME": "causticGlow","LABEL": "Caustic Glow",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5,  "MAX": 5.0 },
        { "NAME": "rotSpeed",   "LABEL": "Rotate Speed",  "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "audioReact", "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

// ──────────────────────────────────────────────────────────────────────────────
// Helpers
// ──────────────────────────────────────────────────────────────────────────────
float hash11(float n){ return fract(sin(n*127.1)*43758.5453); }

vec3 hue2rgb(float h){
    vec3 k = mod(vec3(0.0,4.0,2.0)+h*6.0, 6.0);
    return clamp(min(k, 4.0-k), 0.0, 1.0);
}

// SDF: triangular prism (axis along Z, equilateral cross-section of side ~0.45)
float sdPrism(vec3 p, float h){
    vec3 q = abs(p);
    return max(q.z - h,
               max(q.x * 0.866 + p.y * 0.5, -p.y) - 0.45 * 0.5);
}

// ──────────────────────────────────────────────────────────────────────────────
// Scene SDF
// ──────────────────────────────────────────────────────────────────────────────
float sceneSDF(vec3 p, float tilt){
    // Rotate prism slowly around Y
    float ang = TIME * rotSpeed + tilt * 1.0;
    float ca = cos(ang), sa = sin(ang);
    vec3 rp = vec3(ca*p.x + sa*p.z, p.y, -sa*p.x + ca*p.z);
    // Additional X tilt
    float ta = mix(-0.25, 0.25, tilt);
    float ct = cos(ta), st = sin(ta);
    rp = vec3(rp.x, ct*rp.y - st*rp.z, st*rp.y + ct*rp.z);
    return sdPrism(rp, 0.9);
}

// ──────────────────────────────────────────────────────────────────────────────
// Spectral caustic ray: map (uv.x offset from prism exit point) → hue color
// ──────────────────────────────────────────────────────────────────────────────
vec3 spectrumCaustic(vec2 uv, vec3 prismCenter, float audioBoost){
    // Fan of refracted rays exiting the prism to the right side
    vec2 rel = uv - vec2(prismCenter.x * 0.5, prismCenter.y * 0.25);
    // Vertical spread → hue mapping
    float t = rel.y * (1.0 / specWidth) + 0.5;
    vec3 spec = hue2rgb(clamp(t, 0.0, 1.0));

    // Ray brightness: gaussian along x distance from prism exit
    float xDist = max(0.0, rel.x - 0.1);
    float brightness = exp(-xDist * xDist * 4.0) * exp(-abs(rel.y) * 2.5 / specWidth);
    brightness *= causticGlow * (1.0 + audioLevel * audioReact * audioBoost);

    return spec * brightness;
}

// ──────────────────────────────────────────────────────────────────────────────
// Main raymarch
// ──────────────────────────────────────────────────────────────────────────────
void main(){
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioReact;

    // Camera
    vec3 ro = vec3(0.0, 0.0, 3.5);
    vec3 rd = normalize(vec3(uv, -1.8));

    // Subtle camera bob
    ro.y += sin(TIME * 0.31) * 0.08;

    float tilt = prismTilt + sin(TIME * 0.17) * 0.05;

    // Raymarch
    float dist = 0.0;
    float hit  = 0.0;
    vec3  hp   = ro;
    for(int i = 0; i < 64; i++){
        hp = ro + rd * dist;
        float d = sceneSDF(hp, tilt);
        if(d < 0.001){ hit = 1.0; break; }
        if(dist > 10.0) break;
        dist += d * 0.7;
    }

    // Normal for shading
    vec3 col = vec3(0.0);
    if(hit > 0.5){
        float e = 0.001;
        vec3 n = normalize(vec3(
            sceneSDF(hp+vec3(e,0,0), tilt) - sceneSDF(hp-vec3(e,0,0), tilt),
            sceneSDF(hp+vec3(0,e,0), tilt) - sceneSDF(hp-vec3(0,e,0), tilt),
            sceneSDF(hp+vec3(0,0,e), tilt) - sceneSDF(hp-vec3(0,0,e), tilt)
        ));

        // Glass-like Fresnel
        float fresnel = pow(1.0 - abs(dot(n, -rd)), 3.0);

        // Interior spectral dispersion: tint based on normal.y (wavelength separation)
        float hue = n.y * 0.5 + 0.5;
        vec3 glassColor = hue2rgb(hue) * (0.6 + fresnel * 1.4);

        // White specular highlight (HDR)
        vec3 lightDir = normalize(vec3(-1.2, 1.5, 0.8));
        float spec2 = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 48.0);
        glassColor += vec3(1.0) * spec2 * 3.0 * audio;

        // fwidth edge glow (ink-black silhouette inversion)
        float fw = fwidth(sceneSDF(hp, tilt));
        float edge = smoothstep(0.008, 0.0, fw);
        glassColor = mix(glassColor, vec3(0.0), edge * 0.7);

        col = glassColor * causticGlow * 0.7;
    }

    // Background: deep navy velvet
    vec3 bg = vec3(0.01, 0.01, 0.06) + vec3(0.0, 0.0, 0.04) * exp(-dot(uv, uv) * 2.0);

    // Spectral caustic fan (exits prism to the right)
    vec3 caustic = spectrumCaustic(uv, vec3(0.35, 0.0, 0.0), audio);

    // Combine
    vec3 final = bg + caustic;
    if(hit > 0.5) final = col;

    // Subtle star field on bg
    float star = pow(hash11(floor(uv.x * 120.0) + floor(uv.y * 90.0) * 200.0), 28.0);
    final += vec3(0.8, 0.9, 1.0) * star * (1.0 - hit) * 1.5;

    gl_FragColor = vec4(final, 1.0);
}
