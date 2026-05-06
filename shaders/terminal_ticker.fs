/*{
  "CATEGORIES": ["Generator", "Text", "Audio Reactive"],
  "DESCRIPTION": "Bloomberg / Reuters terminal — multiple horizontal lanes of scrolling stock-ticker glyphs, real numerals for prices, up/down delta arrows in green/red, and a market-crash glitch on bass kicks where rows desync, flicker red, and corrupt pixel-blocks. CRT phosphor green on charcoal, with a bottom-row volume waveform.",
  "INPUTS": [
    { "NAME": "laneCount",     "LABEL": "Lanes",          "TYPE": "float", "MIN": 1.0,  "MAX": 12.0, "DEFAULT": 5.0 },
    { "NAME": "scrollSpeed",   "LABEL": "Scroll Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 0.55 },
    { "NAME": "tickersPerLane","LABEL": "Tickers / Lane", "TYPE": "float", "MIN": 4.0,  "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "deltaUpProb",   "LABEL": "Up vs Down",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "crashProb",     "LABEL": "Crash Prob",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.18 },
    { "NAME": "scanline",      "LABEL": "Scanlines",      "TYPE": "float", "MIN": 0.0,  "MAX": 0.4,  "DEFAULT": 0.16 },
    { "NAME": "showWaveform",  "LABEL": "Volume Wave",    "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "bgColor",       "LABEL": "Background",     "TYPE": "color", "DEFAULT": [0.04, 0.05, 0.06, 1.0] },
    { "NAME": "fgColor",       "LABEL": "Glyph Color",    "TYPE": "color", "DEFAULT": [0.40, 0.95, 0.55, 1.0] },
    { "NAME": "upColor",       "LABEL": "Up Tint",        "TYPE": "color", "DEFAULT": [0.30, 0.95, 0.45, 1.0] },
    { "NAME": "dnColor",       "LABEL": "Down Tint",      "TYPE": "color", "DEFAULT": [0.95, 0.30, 0.30, 1.0] },
    { "NAME": "audioReact",    "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// Pseudo-letter — looks like a stock symbol when stamped at small size.
// 5×7 hash bitmap with column bias toward letterforms.
float drawLetter(vec2 cuv, float seed) {
    cuv = (cuv - 0.5) / 0.78 + 0.5;
    if (cuv.x < 0.0 || cuv.x > 1.0 || cuv.y < 0.0 || cuv.y > 1.0) return 0.0;
    vec2 g = floor(cuv * vec2(5.0, 7.0));
    float h = hash21(g + vec2(seed * 7.13, seed * 11.1));
    float colBias = (g.x == 0.0 || g.x == 4.0) ? 0.40 : 0.55;
    return step(colBias, h);
}

// Real digit — 5×7 hand-coded bitmap of 0..9. Each digit gets a
// segmented mask via row-by-row patterns.
float drawDigit(vec2 cuv, int d) {
    cuv = (cuv - 0.5) / 0.78 + 0.5;
    if (cuv.x < 0.0 || cuv.x > 1.0 || cuv.y < 0.0 || cuv.y > 1.0) return 0.0;
    vec2 g = floor(cuv * vec2(5.0, 7.0));
    int  x = int(g.x);
    int  y = int(g.y);  // 0 = bottom, 6 = top
    if (x < 0 || x > 4 || y < 0 || y > 6) return 0.0;

    // Segment masks — each digit defined by which of 7 pixel-bars are on.
    bool top    = (y == 6) && (x >= 1 && x <= 3);
    bool bot    = (y == 0) && (x >= 1 && x <= 3);
    bool mid    = (y == 3) && (x >= 1 && x <= 3);
    bool tlV    = (x == 0) && (y >= 4 && y <= 5);
    bool trV    = (x == 4) && (y >= 4 && y <= 5);
    bool blV    = (x == 0) && (y >= 1 && y <= 2);
    bool brV    = (x == 4) && (y >= 1 && y <= 2);

    bool on = false;
    if      (d == 0) on = top || bot || tlV || trV || blV || brV;
    else if (d == 1) on = trV || brV || (x == 4 && y == 6);
    else if (d == 2) on = top || trV || mid || blV || bot;
    else if (d == 3) on = top || trV || mid || brV || bot;
    else if (d == 4) on = tlV || trV || mid || brV;
    else if (d == 5) on = top || tlV || mid || brV || bot;
    else if (d == 6) on = top || tlV || mid || blV || brV || bot;
    else if (d == 7) on = top || trV || brV;
    else if (d == 8) on = top || tlV || trV || mid || blV || brV || bot;
    else if (d == 9) on = top || tlV || trV || mid || brV || bot;

    return on ? 1.0 : 0.0;
}

// Up/down arrow glyph — small triangle, dir=+1 up green, dir=-1 down red.
float drawArrow(vec2 cuv, float dir) {
    vec2 p = cuv - 0.5;
    p.y *= dir;
    if (p.y < -0.30 || p.y > 0.30) return 0.0;
    float w = 0.30 - p.y;       // narrows toward apex
    return step(abs(p.x), w * 0.5);
}

// Plus / minus / period
float drawSym(vec2 cuv, int s) {
    cuv = cuv - 0.5;
    if      (s == 0) {                                 // +
        return step(abs(cuv.x), 0.30) * step(abs(cuv.y), 0.07)
             + step(abs(cuv.y), 0.30) * step(abs(cuv.x), 0.07);
    } else if (s == 1) {                               // -
        return step(abs(cuv.x), 0.30) * step(abs(cuv.y), 0.07);
    } else {                                           // .
        return step(length(cuv - vec2(0.0, -0.30)), 0.06);
    }
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = bgColor.rgb;

    int  NL  = int(clamp(laneCount, 1.0, 12.0));
    float fNL = float(NL);

    // Volume wave row at bottom — uses one lane budget.
    float waveBand = showWaveform ? (1.0 / fNL) : 0.0;
    if (showWaveform && uv.y < waveBand) {
        float xs = uv.x;
        // Pseudo-waveform — hash-driven heights, audio-modulated.
        float bins = 64.0;
        vec2  bin  = vec2(floor(xs * bins),
                          floor(TIME * (1.5 + audioMid * audioReact * 4.0)));
        float h = hash21(bin) * 0.6
                + 0.4 * audioLevel * audioReact;
        float bandY = uv.y / waveBand;
        if (bandY < h) {
            // Vertical bar — fade with height.
            col = mix(fgColor.rgb * 0.45, fgColor.rgb,
                      bandY / max(h, 0.001));
        }
        // Top of waveform — outline line.
        col = mix(col, fgColor.rgb,
                  smoothstep(0.012, 0.0, abs(bandY - h)));
    } else {

        // Per-lane Y band
        float yIn = (uv.y - waveBand) / max(1.0 - waveBand, 0.001);
        if (yIn < 0.0 || yIn > 1.0) {
            gl_FragColor = vec4(col, 1.0);
            return;
        }
        float laneIdx = floor(yIn * fNL);
        float laneSeed = laneIdx * 7.31;
        float laneY    = fract(yIn * fNL);

        // Lane scroll offset
        float dir   = (hash11(laneSeed) < 0.5) ? -1.0 : 1.0;
        float speed = scrollSpeed * (0.6 + hash11(laneSeed * 0.31) * 0.8)
                    * (1.0 + audioMid * audioReact * 0.4);
        // Crash glitch — every few seconds a hashed "crash bucket" hits;
        // those buckets desync lane scroll + flash red.
        float crashBucket = floor(TIME * 0.4);
        float crashRoll   = hash21(vec2(laneIdx, crashBucket));
        bool  crashing    = crashRoll
            < crashProb * (0.4 + audioBass * audioReact * 1.4);
        float t = TIME * speed * dir;
        if (crashing) t += sin(TIME * 33.0) * 0.02;

        // Each lane shows a long ribbon of "tickers". A ticker = 4-letter
        // symbol + space + sign + 4 digits + "." + 2 digits + space.
        // Total = 4 + 1 + 1 + 4 + 1 + 2 + 1 = 14 cell-units wide. We pack
        // tickersPerLane such tickers into the canvas width, plus margin.
        float tCount = clamp(tickersPerLane, 1.0, 24.0);
        float cellsTotal = tCount * 14.0;
        float xPos = uv.x + t;            // scroll
        float xCell = fract(xPos) * cellsTotal;
        float xi = floor(xCell);
        float xf = fract(xCell);

        // Which ticker (slot) and which sub-cell within it.
        float tIdx = floor(xi / 14.0);
        int   sub  = int(mod(xi, 14.0));

        // Per-ticker hashed properties — direction and 8-digit "price".
        float tSeed = hash11(laneSeed * 11.7 + tIdx * 5.3);
        float dirRoll = hash11(tSeed * 3.7);
        float deltaSign = (dirRoll < deltaUpProb) ? 1.0 : -1.0;

        // Glyph aspect: cells get taller-than-wide proportion. Shape uv
        // sample inside cell.
        vec2 cuv = vec2(xf, laneY * (1.0 - 0.20)) - vec2(0.0, -0.10);

        float gy = laneY;
        // Trim row to interior (skip cell edges).
        if (gy < 0.20 || gy > 0.85) {
            gl_FragColor = vec4(col, 1.0);
            return;
        }
        cuv = vec2(xf, (gy - 0.20) / 0.65);

        float a = 0.0;
        // Sub layout: cells 0-3 are letters, 4 is space, 5 is sign,
        // 6-9 are digits, 10 is ".", 11-12 are digits, 13 is space.
        if (sub < 4) {
            float seed = hash11(tSeed * 1.7 + float(sub));
            a = drawLetter(cuv, seed);
        } else if (sub == 4 || sub == 13) {
            a = 0.0;
        } else if (sub == 5) {
            a = drawSym(cuv, deltaSign > 0.0 ? 0 : 1);
        } else if (sub == 10) {
            a = drawSym(cuv, 2);
        } else {
            // Digit — hash to 0-9, time-mutated so prices "tick".
            int  digIdx = sub - 6;
            float d = hash11(tSeed * 23.7 + float(sub)
                           + floor(TIME * 1.5));
            a = drawDigit(cuv, int(d * 10.0) % 10);
        }

        // Tint by direction
        vec3 tint = (sub == 5 || sub >= 6) && sub != 13
                  ? mix(dnColor.rgb, upColor.rgb,
                        step(0.0, deltaSign))
                  : fgColor.rgb;
        // Crashing lanes flash red and add pixel-block corruption.
        if (crashing) {
            tint = mix(tint, dnColor.rgb,
                       0.6 + 0.4 * sin(TIME * 24.0));
            // Block corruption — random rectangles get flat-fill.
            vec2 blk = floor(uv * vec2(80.0, 30.0)
                            + floor(TIME * 8.0));
            float blkRoll = hash21(blk);
            if (blkRoll > 0.92) a = max(a, 1.0);
        }

        col = mix(col, tint, a);
        // Add horizontal arrow at start of each ticker (sub == 5 cell
        // override — small triangle next to sign).
        if (sub == 5) {
            float ar = drawArrow(cuv * vec2(2.0, 1.0),
                                 deltaSign > 0.0 ? 1.0 : -1.0);
            col = mix(col,
                      deltaSign > 0.0 ? upColor.rgb : dnColor.rgb,
                      ar * 0.85);
        }
    }

    // CRT scanlines
    float scan = sin(gl_FragCoord.y * 1.4) * 0.5 + 0.5;
    col *= 1.0 - scanline * (1.0 - scan);

    gl_FragColor = vec4(col, 1.0);
}
