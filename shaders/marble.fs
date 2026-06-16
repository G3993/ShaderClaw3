/*{
  "DESCRIPTION": "Marble — swirling wisps with custom texture vein color",
  "CREDIT": "Based on SaturdayShader Week 30 by Joseph Fiola / bonniem",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "LABEL": "Vein Texture",
      "TYPE": "image"
    },
    {
      "NAME": "texMix",
      "LABEL": "Texture Mix",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "lines",
      "LABEL": "Lines",
      "TYPE": "float",
      "DEFAULT": 54,
      "MIN": 1,
      "MAX": 200
    },
    {
      "NAME": "linesStartOffset",
      "LABEL": "Start Offset",
      "TYPE": "float",
      "DEFAULT": 0.785,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "amp",
      "LABEL": "Amplitude",
      "TYPE": "float",
      "DEFAULT": 0.1551,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "glowAmt",
      "LABEL": "Glow",
      "TYPE": "float",
      "DEFAULT": -8.2,
      "MIN": -40,
      "MAX": 0
    },
    {
      "NAME": "mod1",
      "LABEL": "Mod 1",
      "TYPE": "float",
      "DEFAULT": 0.13,
      "MIN": -0.5,
      "MAX": 0.5
    },
    {
      "NAME": "mod2",
      "LABEL": "Mod 2",
      "TYPE": "float",
      "DEFAULT": -0.3,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "twisted",
      "LABEL": "Twist",
      "TYPE": "float",
      "DEFAULT": -0.5,
      "MIN": -0.5,
      "MAX": 1.4095
    },
    {
      "NAME": "zoomAmt",
      "LABEL": "Zoom",
      "TYPE": "float",
      "DEFAULT": 10,
      "MIN": 0,
      "MAX": 100
    },
    {
      "NAME": "rotateCanvas",
      "LABEL": "Rotate",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "scroll",
      "LABEL": "Scroll",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "veinColor",
      "LABEL": "Vein Color",
      "TYPE": "color",
      "DEFAULT": [
        0.91,
        0.25,
        0.34,
        1
      ]
    },
    {
      "NAME": "baseColor",
      "LABEL": "Base Color",
      "TYPE": "color",
      "DEFAULT": [
        0.91,
        0.25,
        0.34,
        1
      ]
    },
    {
      "NAME": "transparentBg",
      "LABEL": "Transparent BG",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

mat2 rotate2d(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 rawUV = uv;
    uv -= vec2(mousePos);
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= zoomAmt;
    uv = rotate2d(rotateCanvas * -TWO_PI) * uv;

    float totalGlow = 0.0;
    float wave_width = 0.01;

    for (float i = 0.0; i < 200.0; i++) {
        uv = rotate2d(amp * (1.0 + audioBass * 3.0) + twisted * -TWO_PI) * uv;
        if (lines <= i) break;

        uv.y += sin(sin(uv.x + i * mod1 + (scroll * TWO_PI)) * amp + (mod2 * PI) + TIME / 4.2);

        if (lines * linesStartOffset - 1.0 <= i) {
            wave_width = abs(1.0 / (50.0 * uv.y * glowAmt * (1.0 + audioLevel * 2.0)));
            totalGlow += wave_width;
        }
    }

    // Clamp the brightness
    float intensity = clamp(totalGlow, 0.0, 3.0);

    // Base marble: blend base color into vein color by wisp intensity
    vec3 marble = mix(baseColor.rgb, veinColor.rgb, intensity / 3.0);
    // Add the bright glow on top
    marble += veinColor.rgb * intensity * 0.4;

    // Blend in custom texture on the vein areas
    if (texMix > 0.001) {
        vec3 tex = texture2D(inputImage, rawUV).rgb;
        // Texture shows through proportional to vein brightness
        float veinMask = clamp(intensity / 1.5, 0.0, 1.0);
        marble = mix(marble, tex * (0.5 + intensity * 0.3), texMix * veinMask);
    }

    // Alpha: transparent background lets the dark areas disappear
    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(intensity * 0.8, 0.0, 1.0);
    }

    // Surprise: every ~45s a single gold vein breaks through the marble
    // — a brief metallic streak (Kintsugi/Pietra Dura inlay) ~1s.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 45.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.20, 0.10, _ph);
        float _y = 0.3 + 0.4 * fract(floor(TIME / 45.0) * 0.71);
        float _vein = exp(-pow((_suv.y - _y - 0.04 * sin(_suv.x * 18.0 + TIME)) * 200.0, 2.0));
        marble = mix(marble, vec3(1.0, 0.82, 0.30), _vein * _f * 0.85);
    }

    gl_FragColor = vec4(marble, alpha);
}
