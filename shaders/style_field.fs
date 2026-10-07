/*{
  "CATEGORIES": [
    "Generator",
    "Minimal"
  ],
  "DESCRIPTION": "Style Field \u2014 a quiet input backdrop for a diffusion style: one field colour (the style's swatch), an optional studio sweep or centre lift, a faint surface (paper, canvas, chalk, felt), slow drift and breathing, soft light pools that wander across the field and a slow camera drift (so the model has motion without structure), and an optional soft centre glow. Built to stay out of the way so the model paints the subject, not the shader.",
  "CREDIT": "ETHEREA",
  "ISFVSN": "2",
  "INPUTS": [
    {
      "NAME": "fieldColor",
      "LABEL": "Field Colour",
      "TYPE": "color",
      "DEFAULT": [
        0.937,
        0.91,
        0.863,
        1.0
      ]
    },
    {
      "NAME": "backdrop",
      "LABEL": "Backdrop",
      "TYPE": "long",
      "DEFAULT": 1,
      "VALUES": [
        0,
        1,
        2
      ],
      "LABELS": [
        "Flat",
        "Studio Sweep",
        "Centre Lift"
      ]
    },
    {
      "NAME": "backdropAmount",
      "LABEL": "Backdrop Amount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.5,
      "DEFAULT": 0.12
    },
    {
      "NAME": "surface",
      "LABEL": "Surface",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2,
        3,
        4
      ],
      "LABELS": [
        "None",
        "Paper",
        "Canvas",
        "Chalk",
        "Felt"
      ]
    },
    {
      "NAME": "surfaceAmount",
      "LABEL": "Surface Amount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.3,
      "DEFAULT": 0.05
    },
    {
      "NAME": "drift",
      "LABEL": "Drift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.2,
      "DEFAULT": 0.04,
      "GROUP": "Motion"
    },
    {
      "NAME": "breathe",
      "LABEL": "Breathe",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.2,
      "DEFAULT": 0.03,
      "GROUP": "Motion"
    },
    {
      "NAME": "breathePeriod",
      "LABEL": "Breathe Period (s)",
      "TYPE": "float",
      "MIN": 4,
      "MAX": 90,
      "DEFAULT": 24,
      "GROUP": "Motion"
    },
    {
      "NAME": "lightPools",
      "LABEL": "Light Pools",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.6,
      "DEFAULT": 0.25,
      "GROUP": "Motion"
    },
    {
      "NAME": "poolSize",
      "LABEL": "Pool Size",
      "TYPE": "float",
      "MIN": 0.2,
      "MAX": 1.2,
      "DEFAULT": 0.6,
      "GROUP": "Motion"
    },
    {
      "NAME": "poolLift",
      "LABEL": "Pool Lift (dark fields)",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.3,
      "DEFAULT": 0,
      "GROUP": "Motion"
    },
    {
      "NAME": "cameraDrift",
      "LABEL": "Camera Drift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.35,
      "GROUP": "Motion"
    },
    {
      "NAME": "motionSpeed",
      "LABEL": "Motion Speed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 3,
      "DEFAULT": 1,
      "GROUP": "Motion"
    },
    {
      "NAME": "glowAmount",
      "LABEL": "Centre Glow",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1.5,
      "DEFAULT": 0,
      "GROUP": "Glow"
    },
    {
      "NAME": "glowColor",
      "LABEL": "Glow Colour",
      "TYPE": "color",
      "DEFAULT": [
        0.86,
        0.92,
        1.0,
        1.0
      ],
      "GROUP": "Glow"
    },
    {
      "NAME": "glowSize",
      "LABEL": "Glow Size",
      "TYPE": "float",
      "MIN": 0.05,
      "MAX": 0.6,
      "DEFAULT": 0.2,
      "GROUP": "Glow"
    },
    {
      "NAME": "glowIridescence",
      "LABEL": "Glow Iridescence",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.5,
      "GROUP": "Glow"
    }
  ]
}*/

// Long inputs are ints in Easel and floats in the web harness: only ever use int(x).

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
    float s = 0.0;
    float a = 0.5;
    for (int i = 0; i < 4; i++) {
        s += a * vnoise(p);
        p = p * 2.03 + 17.1;
        a *= 0.5;
    }
    return s;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 p = vec2((uv.x - 0.5) * aspect, uv.y - 0.5);
    float t = TIME;
    float ms = motionSpeed;
    vec3 col = fieldColor.rgb;

    // Camera drift: a slow zoom and pan of everything textured (drift mottling, surface),
    // which the model can read as the camera moving through the scene.
    float zoom = 1.0 + cameraDrift * 0.22 * sin(t * 0.045 * ms);
    vec2 pan = cameraDrift * 0.35 * vec2(sin(t * 0.029 * ms), cos(t * 0.021 * ms));
    vec2 q = p / zoom + pan;

    // Backdrop: a lightness multiplier around 1.0, never a new shape.
    float shape = 0.0;
    int bd = int(backdrop);
    if (bd == 1) {
        // Studio sweep: a seamless backdrop, brightest on the upper wall, falling off
        // gently toward the floor and the sides.
        float wall = smoothstep(-0.55, 0.35, p.y);
        float side = 1.0 - smoothstep(0.25, 0.95, abs(p.x) / aspect * 1.4);
        shape = 0.6 * wall + 0.4 * side - 0.5;
    } else if (bd == 2) {
        shape = 0.5 - smoothstep(0.0, 0.85, length(p));
    }
    col *= 1.0 + backdropAmount * shape * 2.0;

    // Drift: low-frequency mottling that flows, so the field is never frozen.
    float m = fbm(q * 1.6 + vec2(t * 0.013, -t * 0.009) * ms) - 0.5;
    col *= 1.0 + drift * m * 2.0;

    col *= 1.0 + breathe * sin(6.2831853 * t / max(breathePeriod, 0.1));

    // Light pools: large soft pools of light wandering on slow, unrelated paths, like
    // cloud shadows or a lamp being carried past. Multiplicative on lit fields; poolLift
    // adds a faint glow of the same pools so dark fields move too.
    float pools = 0.0;
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        vec2 c = vec2(sin(t * ms * (0.061 + 0.017 * fi) + fi * 2.1) * 0.62 * aspect,
                      cos(t * ms * (0.047 + 0.013 * fi) + fi * 1.3) * 0.36);
        float d = length(p - c) / max(poolSize, 0.05);
        pools += exp(-d * d * 2.0);
    }
    col *= 1.0 + lightPools * (pools * 0.8 - 0.4) * 2.0;
    col += glowColor.rgb * poolLift * pools * 0.5;

    // Surface: fine texture of the style's medium, kept faint; it rides the camera drift.
    int sf = int(surface);
    vec2 px = (q + vec2(0.5 * aspect, 0.5)) * RENDERSIZE.y;
    float tex = 0.0;
    if (sf == 1) {
        tex = (vnoise(px * 0.9) - 0.5) * 0.7 + (vnoise(px * vec2(0.08, 0.9) + 3.0) - 0.5) * 0.3;
    } else if (sf == 2) {
        tex = sin(px.x * 1.3) * sin(px.y * 1.3) * 0.5 + (vnoise(px * 0.5) - 0.5) * 0.5;
    } else if (sf == 3) {
        tex = (fbm(q * 3.0 + 5.0) - 0.5) * 1.2 + (hash21(floor(px * 0.7)) - 0.5) * 0.25;
    } else if (sf == 4) {
        tex = (fbm(px * 0.12) - 0.5) + (vnoise(px * 1.1) - 0.5) * 0.4;
    }
    col *= 1.0 + surfaceAmount * tex * 2.0;

    // Centre glow: a soft luminous orb that gives the model one place to put a single
    // glowing form, without lighting anything else. It sways a little with the motion.
    if (glowAmount > 0.0) {
        vec2 gp = p - lightPools * 0.12 * vec2(sin(t * 0.07 * ms), cos(t * 0.053 * ms));
        float r = length(gp) / max(glowSize, 0.01);
        float core = exp(-r * r * 2.2);
        float halo = exp(-r * 1.6) * 0.35;
        float ang = atan(gp.y, gp.x);
        // sin(2*ang) keeps the hue continuous across atan's seam at +-pi.
        vec3 irid = 0.5 + 0.5 * cos(6.2831853 * (vec3(0.0, 0.33, 0.67) + r * 0.35 + 0.08 * sin(2.0 * ang) + t * 0.02));
        vec3 g = mix(glowColor.rgb, irid, glowIridescence * smoothstep(0.1, 0.9, r));
        col += g * (core + halo) * glowAmount;
    }

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
