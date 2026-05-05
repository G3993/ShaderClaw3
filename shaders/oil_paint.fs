/*{
  "DESCRIPTION": "Ink Drop Fluid — 3D raymarched ink bloom spreading through water viewed from above. Sumi-e / de Kooning cool-palette expressionism. No inputImage required.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "flowSpeed",  "LABEL": "Flow Speed",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "inkDensity", "LABEL": "Ink Density", "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.2,  "MAX": 3.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 5.0 },
    { "NAME": "inkHue",     "LABEL": "Ink Hue",     "TYPE": "float", "DEFAULT": 0.72, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────────
// Hashing + FBM
// ─────────────────────────────────────────────────────────────────────────────
float hash12(vec2 p){
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float noise(vec2 p){
    vec2 i = floor(p), f = fract(p);
    f = f*f*(3.0-2.0*f);
    return mix(mix(hash12(i+vec2(0,0)),hash12(i+vec2(1,0)),f.x),
               mix(hash12(i+vec2(0,1)),hash12(i+vec2(1,1)),f.x), f.y);
}

float fbm(vec2 p, int oct){
    float v=0.0, a=0.5;
    for(int i=0;i<6;i++){
        if(i>=oct) break;
        v+=a*noise(p); p*=2.1; a*=0.5;
    }
    return v;
}

// ─────────────────────────────────────────────────────────────────────────────
// Ink blob SDF — a warped sphere sitting on y=0 plane
// Multiple ink drops, different sizes and ages
// ─────────────────────────────────────────────────────────────────────────────
float inkBlob(vec3 p, vec2 center, float radius, float warpStrength){
    vec3 q = p - vec3(center.x, 0.0, center.y);
    // Domain warp with 2D FBM for organic shape
    float t = TIME * flowSpeed;
    vec2 warp = vec2(fbm(q.xz * 2.5 + t * 0.4, 4),
                     fbm(q.xz * 2.5 + t * 0.4 + 5.7, 4)) - 0.5;
    q.xz += warp * warpStrength;
    // Flattened ellipsoid (thin pool of ink)
    q.y *= 3.5;
    return length(q) - radius;
}

float sceneSDF(vec3 p){
    float t = TIME * flowSpeed * 0.2;
    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioReact;

    // Three ink drops with breathing radii
    float r1 = inkDensity * 0.32 * (0.9 + 0.1 * sin(t * 1.1)) * audio;
    float r2 = inkDensity * 0.22 * (0.8 + 0.2 * sin(t * 0.7 + 1.3));
    float r3 = inkDensity * 0.15 * (0.85 + 0.15 * sin(t * 1.7 + 2.6));

    float d1 = inkBlob(p, vec2(0.0,  0.0),  r1, 0.40);
    float d2 = inkBlob(p, vec2(0.55, 0.3),  r2, 0.30);
    float d3 = inkBlob(p, vec2(-0.4, -0.45),r3, 0.25);

    // Smooth union
    float k = 0.25;
    float d = d1;
    d = d - k + sqrt((d-d2)*(d-d2) + k*k) * 0.5 + min(d,d2) * 0.5 - k * 0.5;
    float tmp = d - k + sqrt((d-d3)*(d-d3) + k*k) * 0.5 + min(d,d3) * 0.5 - k * 0.5;
    d = tmp;

    // Water plane at y = -0.05
    float water = p.y + 0.05;
    return min(d, water);
}

// ─────────────────────────────────────────────────────────────────────────────
// Palette: cool ink — indigo, violet, cyan mist, white highlight, deep void bg
// ─────────────────────────────────────────────────────────────────────────────
vec3 inkColor(float density, float hue){
    vec3 deepInk  = vec3(0.02, 0.0, 0.12);   // deep indigo-void
    vec3 midInk   = vec3(0.1, 0.0, 0.6);     // violet
    vec3 diffuseC = vec3(0.0, 0.6, 1.0);     // cyan mist at edges
    float t = clamp(density, 0.0, 1.0);
    vec3 base = mix(diffuseC, mix(midInk, deepInk, t), t);
    return base;
}

void main(){
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioReact;

    // Camera looking down at ink pool (expressionist overhead angle)
    vec3 ro = vec3(0.0, 2.8, 0.6);  // slightly tilted to see depth
    vec3 target = vec3(0.0, 0.0, -0.1);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right * 0.9 + uv.y * up * 0.9);

    // Slow camera pan
    ro.xz += vec2(sin(TIME * 0.08) * 0.12, cos(TIME * 0.06) * 0.08);

    // Raymarch
    float dist = 0.0;
    float hit  = 0.0;
    vec3  hp   = ro;
    for(int i = 0; i < 64; i++){
        hp = ro + rd * dist;
        float d = sceneSDF(hp);
        if(d < 0.002){ hit = 1.0; break; }
        if(dist > 8.0) break;
        dist += d * 0.65;
    }

    vec3 col = vec3(0.0);

    if(hit > 0.5){
        float e = 0.002;
        vec3 n = normalize(vec3(
            sceneSDF(hp+vec3(e,0,0))-sceneSDF(hp-vec3(e,0,0)),
            sceneSDF(hp+vec3(0,e,0))-sceneSDF(hp-vec3(0,e,0)),
            sceneSDF(hp+vec3(0,0,e))-sceneSDF(hp-vec3(0,0,e))
        ));

        // Is it the water plane or ink blob?
        bool isWater = (hp.y < -0.03);

        if(isWater){
            // Water surface: faint ripple pattern
            float ripple = fbm(hp.xz * 8.0 + TIME * flowSpeed * 0.3, 3) * 0.5 + 0.5;
            vec3 waterCol = vec3(0.0, 0.04, 0.14) * (0.8 + ripple * 0.4);
            // Specular glint on water
            vec3 lightDir = normalize(vec3(-0.4, 1.0, 0.6));
            float spec = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 24.0);
            waterCol += vec3(0.4, 0.8, 1.0) * spec * hdrPeak * 0.5;
            col = waterCol;
        } else {
            // Ink blob: density-based coloring
            float density = clamp(1.0 - dist / 3.0, 0.0, 1.0);
            vec3 base = inkColor(density, inkHue);

            // Lighting: soft from above
            vec3 lightDir = normalize(vec3(-0.4, 1.0, 0.6));
            float diff = max(dot(n, lightDir), 0.0);

            // fwidth edge AA for sharp ink perimeter
            float fw = fwidth(sceneSDF(hp)) * 200.0;
            float edge = smoothstep(1.0, 0.0, fw);

            col = base * (diff * 0.6 + 0.4) * hdrPeak * edge;

            // White-hot specular on wet ink surface
            float spec = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 16.0);
            col += vec3(0.5, 0.7, 1.0) * spec * hdrPeak * 0.8 * audio;

            // Ink edge diffusion — cyan mist spreading outward
            float edgeMist = 1.0 - smoothstep(0.0, 0.15, abs(sceneSDF(hp)));
            col += vec3(0.0, 0.4, 0.9) * edgeMist * 0.7;
        }
    } else {
        // Background: deep ocean void
        col = vec3(0.0, 0.01, 0.06) + vec3(0.0, 0.01, 0.03) * exp(-dot(uv,uv) * 1.5);
    }

    gl_FragColor = vec4(col, 1.0);
}
