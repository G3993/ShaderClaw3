/*{
  "DESCRIPTION": "Swirl Spin — moving Lissajous spot painted into a slowly rotating/shrinking feedback field, by TekF. HDR linear output.",
  "CREDIT": "Ported from Shadertoy XsyGzz",
  "CATEGORIES": ["Generator", "Feedback"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",       "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "spotSize",   "LABEL": "Spot Size",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.2, "MAX": 4.0 },
    { "NAME": "shrink",     "LABEL": "Shrink",      "TYPE": "float", "DEFAULT": 0.98, "MIN": 0.90, "MAX": 1.00 },
    { "NAME": "drift",      "LABEL": "Drift",       "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "twist",      "LABEL": "Twist",       "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.20 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ],
  "PASSES": [
    { "TARGET": "swirlBuf", "PERSISTENT": true },
    {}
  ]
}*/

vec4 passSwirl(vec2 fragCoord) {
    vec2 res = RENDERSIZE;
    vec2 c2  = fragCoord - res * 0.5;

    float audioMod = 0.5 + 0.5 * audioLevel * audioReact;
    float bassMod  = 0.5 + 0.5 * audioBass  * audioReact;

    // Bass briefly counteracts shrink → expansion flash on kick
    float shrinkMod = shrink - (bassMod - 0.5) * 0.015;

    vec2 sampleP = (fragCoord * shrinkMod
                   + res * drift
                   + c2.yx * vec2(-twist, twist)) / res;
    vec4 prev = texture(swirlBuf, sampleP);

    float t = TIME * speed * (1.0 + audioLevel * audioReact);

    // Spot color: HDR peaks driven by audio (hdrPeak * audioMod → 0.75–2.25 range)
    vec3 spotRGB = sin(t * vec3(13.0, 11.0, 17.0)) * 0.5 + 0.5;
    spotRGB *= hdrPeak * audioMod;
    vec4 col = vec4(spotRGB, 1.0);

    float spotRadius = max(6.0 * spotSize, 1.0);
    vec2 spotCenter  = sin(vec2(11.0, 13.0) * t) * 60.0 + res * 0.5;
    float idx = smoothstep(spotRadius, spotRadius * 3.3, length(fragCoord - spotCenter));

    // Warm hue bg fades out once buffer has content
    vec3 bgHue = 0.5 + 0.5 * sin(t * 0.3 + vec3(0.0, 2.094, 4.189));
    vec4 warm = mix(vec4(bgHue * 0.2, 1.0), prev, min(length(prev.rgb) * 5.0, 1.0));
    return mix(col, warm, idx);
}

vec4 passFinal(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE;
    // Linear HDR output — host applies ACES; no gamma correction here
    return texture(swirlBuf, uv);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    if (PASSINDEX == 0) FragColor = passSwirl(fragCoord);
    else                FragColor = passFinal(fragCoord);
}
