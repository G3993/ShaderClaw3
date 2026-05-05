/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "3D"],
  "DESCRIPTION": "Sound Temple 3D — eight frequency bands illuminate an octagonal tunnel. Audio pulses travel toward the camera like a Turrell light installation.",
  "INPUTS": [
    {"NAME":"sectors",      "TYPE":"float","MIN":4.0,"MAX":12.0,"DEFAULT":8.0},
    {"NAME":"pulseSpeed",   "TYPE":"float","MIN":0.0,"MAX":3.0, "DEFAULT":0.6},
    {"NAME":"pulseWidth",   "TYPE":"float","MIN":0.01,"MAX":0.2,"DEFAULT":0.06},
    {"NAME":"seamSoftness", "TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.008},
    {"NAME":"coreSize",     "TYPE":"float","MIN":0.0,"MAX":0.4, "DEFAULT":0.12},
    {"NAME":"texMix",       "TYPE":"float","MIN":0.0,"MAX":1.0, "DEFAULT":0.3},
    {"NAME":"paletteShift", "TYPE":"float","MIN":0.0,"MAX":1.0, "DEFAULT":0.0},
    {"NAME":"trail",        "TYPE":"float","MIN":0.0,"MAX":1.0, "DEFAULT":0.4},
    {"NAME":"audioReact",   "TYPE":"float","MIN":0.0,"MAX":2.0, "DEFAULT":1.0},
    {"NAME":"inputTex",     "TYPE":"image"}
  ]
}*/

#define TAU 6.28318530718
#define PI  3.14159265358

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 sectorColor(float sec, float total, float shift) {
    return hsv2rgb(vec3(fract(sec / total + shift), 0.78, 1.0));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Camera flies slowly along tunnel axis; slight sway
    float camZ = TIME * 0.35;
    float sway = sin(TIME * 0.23) * 0.05;
    vec3 ro = vec3(sway, 0.0, camZ);
    vec2 ndc = (uv - 0.5) * vec2(aspect, 1.0);
    vec3 rd = normalize(vec3(ndc, 1.6));

    float secs = max(4.0, floor(sectors));
    float R = 0.9;  // tunnel inscribed radius

    // Ray-octagon intersection: find the nearest face the ray hits
    float bestT = 1e10;
    int bestSec = -1;

    for (int k = 0; k < 12; k++) {
        if (float(k) >= secs) break;
        float faceAng = float(k) * TAU / secs;
        vec2 faceN = vec2(cos(faceAng), sin(faceAng));
        // Face plane: dot(p.xy, faceN) = R
        float denom = dot(rd.xy, faceN);
        if (denom < 0.001) continue;  // back-facing or parallel
        float t = (R - dot(ro.xy, faceN)) / denom;
        if (t < 0.05 || t > bestT) continue;
        // Angular range check: hit must be within ±π/N of face center
        vec3 hp = ro + t * rd;
        float hitAng = atan(hp.y, hp.x);
        float dAng = abs(mod(hitAng - faceAng + PI, TAU) - PI);
        if (dAng > PI / secs + 0.02) continue;
        bestT = t;
        bestSec = k;
    }

    vec3 col = vec3(0.004, 0.003, 0.006);  // deep tunnel atmosphere

    if (bestSec >= 0) {
        vec3 hit = ro + bestT * rd;
        float secF = float(bestSec);

        float bin = (secF + 0.5) / secs * 0.85;
        float amp = texture(audioFFT, vec2(bin, 0.5)).r;
        float bass = 0.5 + 0.5 * audioBass * audioReact;

        // Pulse travels toward camera in Z: use Z relative to camera as phase
        float relZ = hit.z - camZ;
        float pOff  = secF / secs;
        float phase = fract(-relZ * 0.18 + TIME * pulseSpeed - pOff);

        float pulse = smoothstep(pulseWidth, 0.0, abs(phase));
        float trailPhase = fract(-relZ * 0.18 + TIME * pulseSpeed - pOff - 0.12);
        float trailPulse = smoothstep(pulseWidth * 2.0, 0.0, abs(trailPhase)) * trail;

        vec3 hue = sectorColor(secF, secs, paletteShift);

        // Wall glow — HDR peaks on pulse crest with audio
        col = hue * 0.05;
        col += hue * (pulse + trailPulse) * (0.5 + amp * 2.5) * (0.6 + bass * 0.9);

        // Depth fog — tunnel vanishes in the distance
        float fog = exp(-abs(relZ) * 0.25);
        col *= 0.25 + 0.75 * fog;

        // Seam highlight at face edges — architectural read
        float faceAng = secF * TAU / secs;
        vec2 faceN = vec2(cos(faceAng), sin(faceAng));
        vec2 faceRight = vec2(-faceN.y, faceN.x);
        float faceHalfW = R * sin(PI / secs);
        float faceX = dot(hit.xy - faceN * R, faceRight);
        float edgeDist = faceHalfW - abs(faceX);
        float seam = smoothstep(seamSoftness * secs * 0.5, 0.0, edgeDist);
        col += vec3(0.9, 0.95, 1.0) * seam * (0.3 + amp * 0.8) * 1.6 * fog;  // HDR seam

        // Optional panoramic texture mapped to face UV
        if (IMG_SIZE_inputTex.x > 0.0) {
            float texU = faceX / (2.0 * faceHalfW) + 0.5;
            float texV = fract(relZ * 0.05);
            vec3 tx = texture(inputTex, vec2(texU, texV)).rgb;
            col = mix(col, tx * (0.4 + amp), texMix);
        }
    }

    // Core glow from tunnel center direction (bass-reactive)
    float bass2 = 0.5 + 0.5 * audioBass * audioReact;
    float coreR = length((uv - 0.5) * vec2(aspect, 1.0));
    float core = smoothstep(coreSize * 1.5, coreSize * 0.4, coreR);
    col += vec3(1.0, 0.96, 0.92) * core * (0.4 + audioBass * 2.2) * bass2;

    // Outer vignette
    col *= smoothstep(1.3, 0.75, length(uv - 0.5) * 2.0);

    gl_FragColor = vec4(col, 1.0);
}
