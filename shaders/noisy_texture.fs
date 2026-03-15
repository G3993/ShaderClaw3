/*{
  "DESCRIPTION": "Multi-color gradient with grainy noise-textured distortion. Inspired by paper.design grain-gradient shader.",
  "CATEGORIES": ["Generator"],
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "color1", "TYPE": "color", "DEFAULT": [0.35, 0.0, 0.9, 1.0], "LABEL": "Color 1" },
    { "NAME": "color2", "TYPE": "color", "DEFAULT": [0.85, 0.45, 0.75, 1.0], "LABEL": "Color 2" },
    { "NAME": "color3", "TYPE": "color", "DEFAULT": [0.0, 0.55, 0.95, 1.0], "LABEL": "Color 3" },
    { "NAME": "colorBack", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "LABEL": "Background" },
    { "NAME": "softness", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Softness" },
    { "NAME": "intensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Intensity" },
    { "NAME": "noise", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0, "LABEL": "Noise" },
    { "NAME": "shape", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Wave", "Dots", "Truchet", "Corners", "Ripple", "Blob", "Sphere"] },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0, "LABEL": "Speed" },
    { "NAME": "scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0, "LABEL": "Scale" },
    { "NAME": "inputImage", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "texMix", "LABEL": "Texture Mix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 }
  ],
  "CREDIT": "Adapted from paper-design/shaders grain-gradient"
}*/

#define TWO_PI 6.28318530718
#define PI 3.14159265358979323846

vec3 permute(vec3 x) { return mod(((x * 34.0) + 1.0) * x, 289.0); }
float snoise(vec2 v) {
    const vec4 C = vec4(0.211324865405187, 0.366025403784439,
        -0.577350269189626, 0.024390243902439);
    vec2 i = floor(v + dot(v, C.yy));
    vec2 x0 = v - i + dot(i, C.xx);
    vec2 i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    i = mod(i, 289.0);
    vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0))
        + i.x + vec3(0.0, i1.x, 1.0));
    vec3 m = max(0.5 - vec3(dot(x0, x0), dot(x12.xy, x12.xy),
        dot(x12.zw, x12.zw)), 0.0);
    m = m * m;
    m = m * m;
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
    vec3 g;
    g.x = a0.x * x0.x + h.x * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}

vec2 rotate(vec2 uv, float th) {
    return mat2(cos(th), sin(th), -sin(th), cos(th)) * uv;
}

float hash11(float p) {
    p = fract(p * 0.3183099) + 0.1;
    p *= p + 19.19;
    return fract(p * p);
}

float hash21(vec2 p) {
    p = fract(p * vec2(0.3183099, 0.3678794)) + 0.1;
    p += dot(p, p + 19.19);
    return fract(p.x * p.y);
}

float randomR(vec2 p) {
    return hash21(floor(p));
}

float valueNoiseR(vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);
    float a = randomR(i);
    float b = randomR(i + vec2(1.0, 0.0));
    float c = randomR(i + vec2(0.0, 1.0));
    float d = randomR(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    float x1 = mix(a, b, u.x);
    float x2 = mix(c, d, u.x);
    return mix(x1, x2, u.y);
}

vec4 fbmR(vec2 n0, vec2 n1, vec2 n2, vec2 n3) {
    float amplitude = 0.2;
    vec4 total = vec4(0.);
    for (int i = 0; i < 3; i++) {
        n0 = rotate(n0, 0.3);
        n1 = rotate(n1, 0.3);
        n2 = rotate(n2, 0.3);
        n3 = rotate(n3, 0.3);
        total.x += valueNoiseR(n0) * amplitude;
        total.y += valueNoiseR(n1) * amplitude;
        total.z += valueNoiseR(n2) * amplitude;
        total.w += valueNoiseR(n3) * amplitude;
        n0 *= 1.99;
        n1 *= 1.99;
        n2 *= 1.99;
        n3 *= 1.99;
        amplitude *= 0.6;
    }
    return total;
}

vec2 truchet(vec2 uv, float idx) {
    idx = fract(((idx - 0.5) * 2.0));
    if (idx > 0.75) {
        uv = vec2(1.0) - uv;
    } else if (idx > 0.5) {
        uv = vec2(1.0 - uv.x, uv.y);
    } else if (idx > 0.25) {
        uv = 1.0 - vec2(1.0 - uv.x, uv.y);
    }
    return uv;
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float colorsCount = 3.0;
    float firstFrameOffset = 7.0;
    float t = 0.1 * (TIME * speed + firstFrameOffset);

    vec2 shape_uv = uv / scale;
    vec2 grain_uv = uv * RENDERSIZE.y * 0.7 / scale;

    float shapeVal = 0.0;
    int iShape = int(shape + 0.5);

    if (iShape == 0) {
        float wave = cos(0.5 * shape_uv.x - 4.0 * t) * sin(1.5 * shape_uv.x + 2.0 * t) * (0.75 + 0.25 * cos(6.0 * t));
        shapeVal = 1.0 - smoothstep(-1.0, 1.0, shape_uv.y + wave);
    } else if (iShape ==1) {
        float stripeIdx = floor(2.0 * shape_uv.x / TWO_PI);
        float rand = hash11(stripeIdx * 100.0);
        rand = sign(rand - 0.5) * pow(4.0 * abs(rand), 0.3);
        shapeVal = sin(shape_uv.x) * cos(shape_uv.y - 5.0 * rand * t);
        shapeVal = pow(abs(shapeVal), 4.0);
    } else if (iShape ==2) {
        float n2 = valueNoiseR(shape_uv * 0.4 - 3.75 * t);
        shape_uv.x += 10.0;
        shape_uv *= 0.6;
        vec2 tile = truchet(fract(shape_uv), randomR(floor(shape_uv)));
        float distance1 = length(tile);
        float distance2 = length(tile - vec2(1.0));
        n2 -= 0.5;
        n2 *= 0.1;
        shapeVal = smoothstep(0.2, 0.55, distance1 + n2) * (1.0 - smoothstep(0.45, 0.8, distance1 - n2));
        shapeVal += smoothstep(0.2, 0.55, distance2 + n2) * (1.0 - smoothstep(0.45, 0.8, distance2 - n2));
        shapeVal = pow(shapeVal, 1.5);
    } else if (iShape ==3) {
        shape_uv *= 0.6;
        vec2 outer = vec2(0.5);
        vec2 bl = smoothstep(vec2(0.0), outer, shape_uv + vec2(0.1 + 0.1 * sin(3.0 * t), 0.2 - 0.1 * sin(5.25 * t)));
        vec2 tr = smoothstep(vec2(0.0), outer, 1.0 - shape_uv);
        shapeVal = 1.0 - bl.x * bl.y * tr.x * tr.y;
        shape_uv = -shape_uv;
        bl = smoothstep(vec2(0.0), outer, shape_uv + vec2(0.1 + 0.1 * sin(3.0 * t), 0.2 - 0.1 * cos(5.25 * t)));
        tr = smoothstep(vec2(0.0), outer, 1.0 - shape_uv);
        shapeVal -= bl.x * bl.y * tr.x * tr.y;
        shapeVal = 1.0 - smoothstep(0.0, 1.0, shapeVal);
    } else if (iShape ==4) {
        shape_uv *= 2.0;
        float dist = length(0.4 * shape_uv);
        float waves = sin(pow(dist, 1.2) * 5.0 - 3.0 * t) * 0.5 + 0.5;
        shapeVal = waves;
    } else if (iShape ==5) {
        float bt = t * 2.0;
        vec2 f1_traj = 0.25 * vec2(1.3 * sin(bt), 0.2 + 1.3 * cos(0.6 * bt + 4.0));
        vec2 f2_traj = 0.2 * vec2(1.2 * sin(-bt), 1.3 * sin(1.6 * bt));
        vec2 f3_traj = 0.25 * vec2(1.7 * cos(-0.6 * bt), cos(-1.6 * bt));
        vec2 f4_traj = 0.3 * vec2(1.4 * cos(0.8 * bt), 1.2 * sin(-0.6 * bt - 3.0));
        shapeVal = 0.5 * pow(1.0 - clamp(length(shape_uv + f1_traj), 0.0, 1.0), 5.0);
        shapeVal += 0.5 * pow(1.0 - clamp(length(shape_uv + f2_traj), 0.0, 1.0), 5.0);
        shapeVal += 0.5 * pow(1.0 - clamp(length(shape_uv + f3_traj), 0.0, 1.0), 5.0);
        shapeVal += 0.5 * pow(1.0 - clamp(length(shape_uv + f4_traj), 0.0, 1.0), 5.0);
        shapeVal = smoothstep(0.0, 0.9, shapeVal);
        float edge = smoothstep(0.25, 0.3, shapeVal);
        shapeVal = mix(0.0, shapeVal, edge);
    } else {
        shape_uv *= 2.0;
        float d = 1.0 - pow(length(shape_uv), 2.0);
        vec3 pos = vec3(shape_uv, sqrt(max(d, 0.0)));
        vec3 lightPos = normalize(vec3(cos(1.5 * t), 0.8, sin(1.25 * t)));
        shapeVal = 0.5 + 0.5 * dot(lightPos, pos);
        shapeVal *= step(0.0, d);
    }

    float baseNoise = snoise(grain_uv * 0.5);
    vec4 fbmVals = fbmR(
        0.002 * grain_uv + 10.0,
        0.003 * grain_uv,
        0.001 * grain_uv,
        rotate(0.4 * grain_uv, 2.0)
    );
    float grainDist = baseNoise * snoise(grain_uv * 0.2) - fbmVals.x - fbmVals.y;
    float rawNoise = 0.75 * baseNoise - fbmVals.w - fbmVals.z;
    float noiseVal = clamp(rawNoise, 0.0, 1.0);

    shapeVal += intensity * 2.0 / colorsCount * (grainDist + 0.5);
    shapeVal += noise * 10.0 / colorsCount * noiseVal;

    float aa = 0.005;

    shapeVal = clamp(shapeVal - 0.5 / colorsCount, 0.0, 1.0);
    float totalShape = smoothstep(0.0, softness + 2.0 * aa, clamp(shapeVal * colorsCount, 0.0, 1.0));
    float mixer = shapeVal * (colorsCount - 1.0);

    // Color gradient blending
    vec4 c0 = color1; c0.rgb *= c0.a;
    vec4 c1 = color2; c1.rgb *= c1.a;
    vec4 c2 = color3; c2.rgb *= c2.a;

    float t1 = clamp(mixer, 0.0, 1.0);
    t1 = smoothstep(0.5 - 0.5 * softness - aa, 0.5 + 0.5 * softness + aa, t1);
    vec4 gradient = mix(c0, c1, t1);

    float t2 = clamp(mixer - 1.0, 0.0, 1.0);
    t2 = smoothstep(0.5 - 0.5 * softness - aa, 0.5 + 0.5 * softness + aa, t2);
    gradient = mix(gradient, c2, t2);

    vec3 outColor = gradient.rgb * totalShape;
    float opacity = gradient.a * totalShape;

    // Blend with input texture
    if (texMix > 0.0) {
        vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
        vec3 texCol = texture2D(inputImage, texUV).rgb;
        outColor = mix(outColor, texCol * totalShape, texMix);
    }

    vec3 bgColor = colorBack.rgb * colorBack.a;
    outColor = outColor + bgColor * (1.0 - opacity);
    opacity = opacity + colorBack.a * (1.0 - opacity);

    gl_FragColor = vec4(outColor, opacity);
}
