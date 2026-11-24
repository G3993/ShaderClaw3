/*{
  "DESCRIPTION": "Liberty: the Statue of Liberty, modelled as code in Sand3D (Sandflow's open 3D-as-code language), baked to a signed distance field and raymarched live. Three looks: Patina (copper green at night over the harbour, torch burning), Contours (minimal topographic lines sliced through her, one red), Hologram (glowing iso-shells and scanlines). She turns slowly; drag or touch to spin her. Bass feeds the torch and the line scroll, beats flash the rim.",
  "CREDIT": "Gelso · Shades of New York · model written in Sand3D (Apache-2.0)",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive", "New York"],
  "INPUTS": [
    { "NAME": "look", "LABEL": "Look", "TYPE": "long", "VALUES": [0, 1, 2], "LABELS": ["Patina", "Contours", "Hologram"], "DEFAULT": 0 },
    { "NAME": "spin", "LABEL": "Spin", "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.18 },
    { "NAME": "zoom", "LABEL": "Zoom", "TYPE": "float", "MIN": 0.6, "MAX": 2.4, "DEFAULT": 1.0 },
    { "NAME": "camHeight", "LABEL": "Camera Height", "TYPE": "float", "MIN": -0.3, "MAX": 1.0, "DEFAULT": 0.15 },
    { "NAME": "lineDensity", "LABEL": "Line Density", "TYPE": "float", "MIN": 20.0, "MAX": 160.0, "DEFAULT": 70.0 },
    { "NAME": "torch", "LABEL": "Torch", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.2 },
    { "NAME": "fogAmt", "LABEL": "Harbour Fog", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "skyline", "LABEL": "Skyline", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.6 },
    { "NAME": "hueShift", "LABEL": "Hue Shift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 }
  ],
  "IMPORTED": {
    "libSDF": { "PATH": "ny_liberty_sdf.png" }
  }
}*/

// Figure SDF atlas: 256 slices up Y (0..0.72 m above the pedestal), each 96x96 over x,z in [-0.13, 0.13] m,
// 16 tiles a row, 1536x1536, value 0.5 = surface, +-0.5 = +-4 cm. She faces +Z. The fort and pedestal are analytic.
const float AW = 1536.0, CELL = 96.0, NS = 256.0, TOP = 0.356;
const vec3 BMIN = vec3(-0.13, TOP, -0.13), BSIZE = vec3(0.26, 0.72, 0.26);
const vec3 TORCH = vec3(-0.082, 1.022, 0.022);

float slice(float k, vec2 xz) {
    float tx = mod(k, 16.0), ty = floor(k / 16.0);
    return texture2D(libSDF, (vec2(tx, ty) * CELL + xz * CELL) / AW).r;
}
float statue(vec3 p) {
    vec3 q = (p - BMIN) / BSIZE;
    vec2 xz = clamp(q.xz, 0.5 / CELL, 1.0 - 0.5 / CELL);
    float fy = clamp(q.y * NS - 0.5, 0.0, NS - 1.0);
    float k0 = floor(fy), t = fy - k0;
    return (mix(slice(k0, xz), slice(min(k0 + 1.0, NS - 1.0), xz), t) - 0.5) * 0.08;
}
float sdBox(vec3 p, vec3 c, vec3 h) { vec3 d = abs(p - c) - h; return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0); }
// Fort Wood: the eleven-point star the pedestal stands in
float sdStar(vec2 p, float r, float n, float m) {
    float an = 3.141593 / n, en = 3.141593 / m;
    vec2 acs = vec2(cos(an), sin(an)), ecs = vec2(cos(en), sin(en));
    float bn = mod(atan(p.x, p.y), 2.0 * an) - an;
    p = length(p) * vec2(cos(bn), abs(sin(bn)));
    p -= r * acs;
    p += ecs * clamp(-dot(p, ecs), 0.0, r * acs.y / ecs.y);
    return length(p) * sign(p.x);
}
float pedestal(vec3 p) {
    vec2 w = vec2(sdStar(p.xz, 0.215, 11.0, 4.2), abs(p.y - 0.02) - 0.02);
    float d = min(max(w.x, w.y), 0.0) + length(max(w, 0.0));
    d = min(d, sdBox(p, vec3(0.0, 0.07, 0.0), vec3(0.095, 0.03, 0.095)));                 // plinth
    float hw = mix(0.075, 0.066, clamp((p.y - 0.1) / 0.23, 0.0, 1.0));                     // tapered shaft
    d = min(d, sdBox(p, vec3(0.0, 0.215, 0.0), vec3(hw, 0.115, hw)));
    d = min(d, sdBox(p, vec3(0.0, 0.258, 0.0), vec3(0.079, 0.006, 0.079)));                // the balcony ledge
    d = min(d, sdBox(p, vec3(0.0, 0.343, 0.0), vec3(0.075, 0.013, 0.075)) - 0.002);         // cap
    return d;
}
float map(vec3 p) {
    float d = pedestal(p);
    float bx = sdBox(p, vec3(0.0, TOP + 0.36, 0.0), vec3(0.125, 0.355, 0.125));
    return min(d, bx > 0.02 ? bx : max(statue(p), bx));
}
vec3 normalAt(vec3 p) {
    const vec2 k = vec2(1.0, -1.0); const float e = 0.0025;
    return normalize(k.xyy * map(p + k.xyy * e) + k.yyx * map(p + k.yyx * e) + k.yxy * map(p + k.yxy * e) + k.xxx * map(p + k.xxx * e));
}
float hash(float n) { return fract(sin(n) * 43758.5453); }
vec3 hue(vec3 c, float h) {
    float a = h * 6.2831853, cs = cos(a), sn = sin(a);
    mat3 m = mat3(0.299, 0.587, 0.114, 0.299, 0.587, 0.114, 0.299, 0.587, 0.114)
           + cs * mat3(0.701, -0.587, -0.114, -0.299, 0.413, -0.114, -0.300, -0.588, 0.886)
           + sn * mat3(0.168, 0.330, -0.497, -0.328, 0.035, 0.292, 1.250, -1.050, -0.203);
    return clamp(m * c, 0.0, 1.0);
}

// far Manhattan: a band of towers along the horizon, lit windows
vec3 sky(vec3 rd, float st, float pulse) {
    float y = rd.y;
    vec3 col = mix(vec3(0.012, 0.018, 0.04), vec3(0.05, 0.07, 0.13), smoothstep(-0.05, 0.25, y));
    if (st > 0.5) col = mix(vec3(0.0), vec3(0.012), smoothstep(-0.2, 0.4, y));
    if (skyline) {
        float ang = atan(rd.x, -rd.z) * 9.0;
        float id = floor(ang * 3.0);
        float hgt = 0.012 + 0.06 * pow(hash(id * 1.7), 3.0) + 0.02 * hash(id + 9.0);
        float tower = step(y, hgt) * step(-0.02, y);
        vec2 w = vec2(fract(ang * 3.0 * 6.0), fract(y * 260.0));
        float lit = step(0.62, hash(floor(ang * 18.0) * 3.1 + floor(y * 260.0) * 7.3)) * step(0.3, w.x) * step(0.35, w.y);
        vec3 tc = st < 0.5 ? vec3(0.008, 0.01, 0.02) + lit * vec3(1.0, 0.75, 0.42) * (0.35 + 0.4 * pulse)
                : st < 1.5 ? vec3(0.0) + lit * vec3(0.25)
                : vec3(0.0, 0.02, 0.03) + lit * vec3(0.2, 0.9, 1.0) * 0.4;
        col = mix(col, tc, tower);
    }
    return col;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float bass = clamp(audioBass, 0.0, 1.0) * audioReact;
    float lvl = clamp(audioLevel, 0.0, 1.0) * audioReact;
    float beat = clamp(audioBeatPulse, 0.0, 1.0) * audioReact;
    float st = float(look);

    // camera: slow orbit, drag/touch adds a turn
    float ang = TIME * spin * 0.5 + (mousePos.x - 0.5) * 6.2831853;
    float r = 2.45 / zoom;
    vec3 tgt = vec3(0.0, 0.56 + camHeight * 0.15, 0.0);
    vec3 ro = vec3(sin(ang) * r, 0.36 + camHeight, cos(ang) * r);
    vec3 fw = normalize(tgt - ro), rt = normalize(cross(fw, vec3(0.0, 1.0, 0.0))), up = cross(rt, fw);
    vec3 rd = normalize(fw * 1.9 + rt * uv.x + up * uv.y);

    // march
    // only march inside her bounds (fort to torch): most pixels skip straight to sky and water
    vec3 inv = 1.0 / rd, b0 = (vec3(-0.23, 0.0, -0.23) - ro) * inv, b1 = (vec3(0.23, 1.075, 0.23) - ro) * inv;
    vec3 bmn = min(b0, b1), bmx = max(b0, b1);
    float tin = max(max(bmn.x, bmn.y), max(bmn.z, 0.0)), tout = min(bmx.x, min(bmx.y, bmx.z));
    float t = tin, hit = 0.0, glow = 0.0, shells = 0.0;
    if (tout > tin) for (int i = 0; i < 88; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        glow += exp(-max(d, 0.0) * 60.0) * 0.012;
        shells += (1.0 - smoothstep(0.0, 0.006, abs(fract(max(d, 0.0) * 18.0 - TIME * 0.6) - 0.5) * 0.05)) * exp(-max(d, 0.0) * 25.0) * 0.008;
        if (d < 0.0012) { hit = 1.0; break; }
        t += max(d * 0.9, 0.0015);
        if (t > tout) break;
    }
    vec3 p = ro + rd * t;
    vec3 col = sky(rd, st, lvl);

    // water: the harbour, a dark mirror below the fort
    float tw = -ro.y / rd.y;
    if (rd.y < 0.0 && (hit < 0.5 || tw < t)) {
        vec3 wp = ro + rd * tw;
        vec3 rr = reflect(rd, vec3(0.0, 1.0, 0.0));
        float ripple = sin(wp.x * 40.0 + TIME) * sin(wp.z * 34.0 - TIME * 0.7) * 0.02;
        vec3 wc = sky(normalize(rr + vec3(ripple, 0.0, ripple)), st, lvl) * 0.5;
        float tl = exp(-length(wp.xz - TORCH.xz) * 2.0) * torch * (0.15 + 0.5 * bass);
        if (st < 0.5) wc += vec3(1.0, 0.6, 0.25) * tl * 0.25 * (0.6 + 0.4 * sin(wp.x * 90.0 + TIME * 3.0));
        if (st > 0.5 && st < 1.5) wc = vec3(0.0) + step(0.97, fract(length(wp.xz) * lineDensity * 0.25 - TIME * 0.4)) * 0.18;
        col = wc;
        hit = 0.0;
    }

    if (hit > 0.5) {
        vec3 n = normalAt(p);
        float flame = smoothstep(0.03, 0.0, length(p - TORCH) - 0.012) * step(1.0, p.y);
        bool stone = p.y < 0.357;
        float ao = clamp(map(p + n * 0.02) / 0.02, 0.0, 1.0);
        if (st < 0.5) {
            // PATINA: copper green, moonlight from above-left, torchlight, cyan rim
            vec3 base = stone ? vec3(0.52, 0.5, 0.46) * (0.88 + 0.12 * step(0.25, fract(p.y * 70.0))) : vec3(0.30, 0.62, 0.52);
            vec3 L = normalize(vec3(-0.5, 0.8, 0.6));
            float dif = max(dot(n, L), 0.0);
            vec3 tl = TORCH - p; float tdist = length(tl);
            float tdif = max(dot(n, tl / tdist), 0.0) / (1.0 + tdist * tdist * 18.0);
            float rim = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
            float fill = max(dot(n, normalize(vec3(0.4, 0.2, 0.9) * vec3(sin(ang), 1.0, cos(ang)))), 0.0);
            col = base * (0.16 + 0.85 * dif * vec3(0.8, 0.88, 1.0) + 0.3 * fill * vec3(0.55, 0.7, 0.9)) * (0.5 + 0.5 * ao);
            col += base * tdif * vec3(1.0, 0.62, 0.28) * torch * (0.6 + 1.2 * bass) * 1.6;
            col += rim * vec3(0.25, 0.8, 0.95) * (0.45 + 0.9 * beat);
            col = mix(col, vec3(1.0, 0.72, 0.3) * (1.5 + 2.0 * bass) * torch, flame);
        } else if (st < 1.5) {
            // CONTOURS: black form, white slices up her body, one red line climbing
            float k = p.y * lineDensity - TIME * (0.6 + 2.5 * bass);
            float line = 1.0 - smoothstep(0.0, 0.08, abs(fract(k) - 0.5) - 0.42);
            float redY = fract(TIME * 0.05) * 1.1;
            float red = 1.0 - smoothstep(0.0, 0.004, abs(p.y - redY) - 0.002);
            float sil = pow(1.0 - max(dot(n, -rd), 0.0), 4.0);
            col = vec3(line * (0.55 + 0.45 * ao)) + sil * 0.35;
            col = mix(col, vec3(0.95, 0.12, 0.1), red);
            col += flame * vec3(1.0) * torch;
        } else {
            // HOLOGRAM: thin shells, scanlines, cyan
            float fres = pow(1.0 - max(dot(n, -rd), 0.0), 2.0);
            float scan = 0.6 + 0.4 * sin(gl_FragCoord.y * 1.4 + TIME * 6.0);
            col = vec3(0.1, 0.85, 1.0) * (0.15 + 1.1 * fres) * scan * (0.8 + 0.6 * lvl);
            col += vec3(1.0, 0.3, 0.8) * flame * torch;
        }
        // harbour fog
        col = mix(col, sky(rd, st, lvl) + vec3(0.03, 0.04, 0.06) * (st < 0.5 ? 1.0 : 0.0), clamp(fogAmt * (t - 1.0) * 0.35, 0.0, 0.8));
    }

    // glow around her (torch and hologram)
    if (st < 0.5) col += vec3(1.0, 0.6, 0.25) * exp(-length(cross(rd, TORCH - ro)) * 40.0) * torch * (0.25 + 0.8 * bass);
    if (st > 1.5) col += vec3(0.1, 0.8, 1.0) * (glow * 0.6 + shells * 1.2) * (0.7 + beat);
    col += beat * 0.06 * vec3(0.6, 0.8, 1.0) * (st < 1.5 ? 1.0 : 0.0);

    if (hueShift > 0.0005) col = hue(col, hueShift);
    col = col / (1.0 + col * 0.35);                          // soft shoulder
    col *= 1.0 - 0.35 * dot(uv * 0.9, uv * 0.9);             // vignette
    gl_FragColor = vec4(pow(max(col, 0.0), vec3(0.92)), 1.0);
}
