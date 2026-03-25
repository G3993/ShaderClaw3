/*{
    "CATEGORIES": ["Filter", "Generator"],
    "CREDIT": "Florian Berger (flockaroo) — Shadertoy MsGSRd",
    "DESCRIPTION": "Single-pass computational fluid dynamics with specular lighting",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "inputImageAmount",
            "LABEL": "Input image amount",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "fluidSpeed",
            "LABEL": "Fluid speed",
            "TYPE": "float",
            "DEFAULT": 2,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "fluidHeight",
            "LABEL": "Fluid height",
            "TYPE": "float",
            "DEFAULT": 150,
            "MIN": 1,
            "MAX": 500
        },
        {
            "NAME": "specularReflectionAmount",
            "LABEL": "Specular",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 3
        },
        {
            "NAME": "motorLocation",
            "LABEL": "Motor location",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5],
            "MIN": [0, 0],
            "MAX": [1, 1]
        },
        {
            "NAME": "motorSize",
            "LABEL": "Motor size",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0,
            "MAX": 0.1
        },
        {
            "NAME": "motorAttenuation",
            "LABEL": "Motor attenuation",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.01,
            "MAX": 1
        },
        {
            "NAME": "dripSize",
            "LABEL": "Drip size",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "lightRadius",
            "LABEL": "Light distance",
            "TYPE": "float",
            "DEFAULT": 2.4494897428,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "lightPhi",
            "LABEL": "Light phi",
            "TYPE": "float",
            "DEFAULT": 35.2643896828,
            "MIN": 0,
            "MAX": 180
        },
        {
            "NAME": "lightTheta",
            "LABEL": "Light theta",
            "TYPE": "float",
            "DEFAULT": 45,
            "MIN": 0,
            "MAX": 360
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "mainPass",
            "PERSISTENT": true
        },
        {}
    ]
}*/

#define PI 3.1415926535897932384626433832795
#define TWO_PI 6.2831853071795864769252867665590
#define DEG2RAD (PI / 180.0)

#define RotNum 5

const float ang = TWO_PI / float(RotNum);
mat2 m = mat2(cos(ang), sin(ang), -sin(ang), cos(ang));

vec2 polar2cart(in vec2 polar) {
    return vec2(cos(polar.x), sin(polar.x)) * polar.y;
}

vec3 polar2cart(in float r, in float phi, in float theta) {
    float x = r * cos(theta) * sin(phi);
    float y = r * sin(theta) * sin(phi);
    float z = r * cos(phi);
    return vec3(x, y, z);
}

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

vec3 hash31(float p) {
    vec3 p3 = fract(vec3(p) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}

float getRot(vec2 pos, vec2 b) {
    vec2 p = b;
    float rot = 0.0;
    for (int i = 0; i < RotNum; i++) {
        rot += dot(
            IMG_NORM_PIXEL(mainPass, fract((pos + p) / RENDERSIZE)).xy - vec2(0.5),
            p.yx * vec2(1, -1)
        );
        p = m * p;
    }
    return rot / float(RotNum) / dot(b, b);
}

// Generate a colorful initial image when no input is loaded
vec4 generateInitialImage(vec2 uv) {
    vec3 col = vec3(0.0);
    // Swirling color bands
    float t = 0.0;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        vec3 h = hash31(fi * 7.13 + 3.7);
        vec2 center = h.xy;
        float r = length(uv - center);
        float wave = sin(r * 20.0 - fi * 1.5) * 0.5 + 0.5;
        col += h * wave * 0.4;
    }
    col = clamp(col, 0.0, 1.0);
    return vec4(col, 1.0);
}

void main() {
    vec2 pos = gl_FragCoord.xy;
    vec2 inverseSize = 1.0 / RENDERSIZE;
    vec2 uv = pos * inverseSize;

    if (PASSINDEX == 0) {
        // --- Fluid simulation pass (Buffer A equivalent) ---
        float rnd = hash11(TIME / RENDERSIZE.x);

        vec2 b = vec2(cos(ang * rnd), sin(ang * rnd));
        vec2 v = vec2(0);

        float bbMax = dripSize * RENDERSIZE.y;
        bbMax *= bbMax;

        for (int l = 0; l < 20; l++) {
            if (dot(b, b) > bbMax) break;

            vec2 p = b;

            for (int i = 0; i < RotNum; i++) {
                v += p.yx * getRot(pos + p, b);
                p = m * p;
            }

            b *= 2.0;
        }

        // Self-advection
        gl_FragColor = IMG_NORM_PIXEL(mainPass, fract((pos + v * vec2(-1, 1) * fluidSpeed) / RENDERSIZE));

        // Motor — injects rotation at a point
        vec2 scr = 2.0 * (uv - motorLocation);
        gl_FragColor.xy += motorSize * (1.0 + audioBass * 5.0) * scr.xy / (dot(scr, scr) / 0.1 + motorAttenuation);

        // Continuous input image injection (only when amount > 0)
        if (inputImageAmount > 0.001) {
            gl_FragColor = (1.0 - inputImageAmount) * gl_FragColor + inputImageAmount * IMG_PIXEL(inputImage, pos);
        }

        // Initialize: load input image on first frames, or generate pattern if no image
        if (FRAMEINDEX < 5) {
            vec4 img = IMG_PIXEL(inputImage, pos);
            // Check if input image has content (not all black)
            float imgEnergy = dot(img.rgb, vec3(1.0));
            if (imgEnergy > 0.01) {
                gl_FragColor = img;
            } else {
                gl_FragColor = generateInitialImage(uv);
            }
        }
    }
    else {
        // --- Lighting pass (Image tab equivalent) ---
        vec2 d = vec2(inverseSize.y, 0);
        vec3 n = vec3(
            (length(IMG_NORM_PIXEL(mainPass, uv + d.xy).xyz) - length(IMG_NORM_PIXEL(mainPass, uv - d.xy).xyz)) / inverseSize.y,
            (length(IMG_NORM_PIXEL(mainPass, uv + d.yx).xyz) - length(IMG_NORM_PIXEL(mainPass, uv - d.yx).xyz)) / inverseSize.y,
            fluidHeight
        );

        n = normalize(n);

        vec3 light = normalize(polar2cart(lightRadius, lightPhi * DEG2RAD, lightTheta * DEG2RAD));
        float diff = clamp(dot(n, light), 0.5, 1.0);
        float spec = clamp(dot(reflect(light, n), vec3(0, 0, -1)), 0.0, 1.0);
        spec = pow(spec, 36.0) * 2.5;

        gl_FragColor = IMG_NORM_PIXEL(mainPass, uv) * vec4(diff) + specularReflectionAmount * vec4(spec);
    }
}
