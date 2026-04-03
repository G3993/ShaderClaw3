(function(THREE) {
    // Pretext Variable Typographic ASCII
    // Particle-driven brightness field rendered as variable-weight ASCII characters
    // Uses @chenglou/pretext for text measurement, Canvas 2D for rendering,
    // uploaded as a Three.js texture for compositing.

    var INPUTS = [
        { NAME: "text", TYPE: "text", DEFAULT: "ETHEREA" },
        { NAME: "cols", LABEL: "Columns", TYPE: "float", DEFAULT: 60, MIN: 20, MAX: 120 },
        { NAME: "fontSize", LABEL: "Font Size", TYPE: "float", DEFAULT: 14, MIN: 8, MAX: 32 },
        { NAME: "particleCount", LABEL: "Particles", TYPE: "float", DEFAULT: 120, MIN: 20, MAX: 300 },
        { NAME: "attractorSpeed", LABEL: "Attract Speed", TYPE: "float", DEFAULT: 1.0, MIN: 0.1, MAX: 3.0 },
        { NAME: "attractorForce", LABEL: "Attract Force", TYPE: "float", DEFAULT: 0.22, MIN: 0.01, MAX: 1.0 },
        { NAME: "fieldDecay", LABEL: "Field Decay", TYPE: "float", DEFAULT: 0.82, MIN: 0.5, MAX: 0.99 },
        { NAME: "mode", LABEL: "Render Mode", TYPE: "long", DEFAULT: 0, VALUES: [0, 1, 2], LABELS: ["Variable Type", "Monospace", "Density Only"] },
        { NAME: "fontFamily", LABEL: "Font", TYPE: "text", DEFAULT: "Georgia" },
        { NAME: "fgColor", TYPE: "color", DEFAULT: [1.0, 1.0, 1.0, 1.0] },
        { NAME: "bgColor", TYPE: "color", DEFAULT: [0.0, 0.0, 0.0, 1.0] },
        { NAME: "transparentBg", TYPE: "bool", DEFAULT: true }
    ];

    // === Constants ===
    var FIELD_OVERSAMPLE = 2;
    var SPRITE_R = 14;
    var ATTRACTOR_R = 12;
    var LARGE_ATTRACTOR_R = 30;
    var MONO_RAMP = ' .`-_:,;^=+/|)\\!?0oOQ#%@';
    var CHARSET = ' .,:;!+-=*#@%&abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    var WEIGHTS = [300, 500, 800];
    var STYLES = ['normal', 'italic'];

    // === Brightness measurement ===
    function createBrightnessCanvas() {
        var c = document.createElement('canvas');
        c.width = 28; c.height = 28;
        return c;
    }

    function estimateBrightness(ch, font, bCanvas) {
        var ctx = bCanvas.getContext('2d', { willReadFrequently: true });
        var size = 28;
        ctx.clearRect(0, 0, size, size);
        ctx.font = font;
        ctx.fillStyle = '#fff';
        ctx.textBaseline = 'middle';
        ctx.fillText(ch, 1, size / 2);
        var data = ctx.getImageData(0, 0, size, size).data;
        var sum = 0;
        for (var i = 3; i < data.length; i += 4) sum += data[i];
        return sum / (255 * size * size);
    }

    // === Palette builder ===
    function buildPalette(fontFamily, fontSize, bCanvas) {
        var palette = [];
        for (var si = 0; si < STYLES.length; si++) {
            var style = STYLES[si];
            for (var wi = 0; wi < WEIGHTS.length; wi++) {
                var weight = WEIGHTS[wi];
                var font = (style === 'italic' ? 'italic ' : '') + weight + ' ' + fontSize + 'px ' + fontFamily;
                for (var ci = 0; ci < CHARSET.length; ci++) {
                    var ch = CHARSET[ci];
                    if (ch === ' ') continue;
                    var brightness = estimateBrightness(ch, font, bCanvas);
                    if (brightness <= 0) continue;
                    palette.push({ char: ch, weight: weight, style: style, font: font, brightness: brightness });
                }
            }
        }
        // Normalize brightness
        var maxB = 0;
        for (var i = 0; i < palette.length; i++) {
            if (palette[i].brightness > maxB) maxB = palette[i].brightness;
        }
        if (maxB > 0) {
            for (var i = 0; i < palette.length; i++) {
                palette[i].brightness /= maxB;
            }
        }
        palette.sort(function(a, b) { return a.brightness - b.brightness; });
        return palette;
    }

    function findBest(palette, targetBrightness, targetCellW) {
        // Binary search for closest brightness
        var lo = 0, hi = palette.length - 1;
        while (lo < hi) {
            var mid = (lo + hi) >> 1;
            if (palette[mid].brightness < targetBrightness) lo = mid + 1;
            else hi = mid;
        }
        var bestScore = Infinity, best = palette[lo];
        var start = Math.max(0, lo - 15);
        var end = Math.min(palette.length, lo + 15);
        for (var i = start; i < end; i++) {
            var entry = palette[i];
            var bErr = Math.abs(entry.brightness - targetBrightness) * 2.5;
            var score = bErr;
            if (score < bestScore) {
                bestScore = score;
                best = entry;
            }
        }
        return best;
    }

    // === Particle simulation ===
    function createParticles(n, w, h) {
        var particles = [];
        for (var i = 0; i < n; i++) {
            var angle = Math.random() * Math.PI * 2;
            var radius = Math.random() * 40 + 20;
            particles.push({
                x: w / 2 + Math.cos(angle) * radius,
                y: h / 2 + Math.sin(angle) * radius,
                vx: (Math.random() - 0.5) * 0.8,
                vy: (Math.random() - 0.5) * 0.8
            });
        }
        return particles;
    }

    function spriteAlphaAt(d) {
        if (d >= 1) return 0;
        if (d <= 0.35) return 0.45 + (0.15 - 0.45) * (d / 0.35);
        return 0.15 * (1 - (d - 0.35) / 0.65);
    }

    function createFieldStamp(radiusPx, scaleX, scaleY) {
        var frx = radiusPx * scaleX;
        var fry = radiusPx * scaleY;
        var rx = Math.ceil(frx);
        var ry = Math.ceil(fry);
        var sx = rx * 2 + 1;
        var sy = ry * 2 + 1;
        var vals = new Float32Array(sx * sy);
        for (var y = -ry; y <= ry; y++) {
            for (var x = -rx; x <= rx; x++) {
                var nd = Math.sqrt((x / frx) * (x / frx) + (y / fry) * (y / fry));
                vals[(y + ry) * sx + x + rx] = spriteAlphaAt(nd);
            }
        }
        return { radiusX: rx, radiusY: ry, sizeX: sx, sizeY: sy, values: vals };
    }

    function splatStamp(field, fieldCols, fieldRows, cx, cy, stamp) {
        var gcx = Math.round(cx);
        var gcy = Math.round(cy);
        for (var y = -stamp.radiusY; y <= stamp.radiusY; y++) {
            var gy = gcy + y;
            if (gy < 0 || gy >= fieldRows) continue;
            var fro = gy * fieldCols;
            var sro = (y + stamp.radiusY) * stamp.sizeX;
            for (var x = -stamp.radiusX; x <= stamp.radiusX; x++) {
                var gx = gcx + x;
                if (gx < 0 || gx >= fieldCols) continue;
                var sv = stamp.values[sro + x + stamp.radiusX];
                if (sv === 0) continue;
                var fi = fro + gx;
                field[fi] = Math.min(1, field[fi] + sv);
            }
        }
    }

    // === Scene creation ===
    function create(renderer, canvas, media) {
        var scene = new THREE.Scene();
        scene.background = null;

        var camera = new THREE.OrthographicCamera(-0.5, 0.5, 0.5, -0.5, 0.1, 10);
        camera.position.z = 1;

        // Text rendering canvas
        var COLS = 60;
        var ROWS = 34;
        var FONT_SIZE = 14;
        var LINE_HEIGHT = 16;
        var CANVAS_W = 220;
        var CANVAS_H = Math.round(CANVAS_W * ((ROWS * LINE_HEIGHT) / (COLS * 8)));
        var FIELD_COLS = COLS * FIELD_OVERSAMPLE;
        var FIELD_ROWS = ROWS * FIELD_OVERSAMPLE;
        var FIELD_SX = FIELD_COLS / CANVAS_W;
        var FIELD_SY = FIELD_ROWS / CANVAS_H;

        var textCanvas = document.createElement('canvas');
        textCanvas.width = 1024;
        textCanvas.height = 1024;
        var tCtx = textCanvas.getContext('2d');

        var bCanvas = createBrightnessCanvas();
        var palette = null;
        var particles = createParticles(120, CANVAS_W, CANVAS_H);
        var brightnessField = new Float32Array(FIELD_COLS * FIELD_ROWS);

        var particleStamp = createFieldStamp(SPRITE_R, FIELD_SX, FIELD_SY);
        var largeStamp = createFieldStamp(LARGE_ATTRACTOR_R, FIELD_SX, FIELD_SY);
        var smallStamp = createFieldStamp(ATTRACTOR_R, FIELD_SX, FIELD_SY);

        var texture = new THREE.CanvasTexture(textCanvas);
        texture.minFilter = THREE.LinearFilter;
        texture.magFilter = THREE.LinearFilter;

        var planeMat = new THREE.MeshBasicMaterial({
            map: texture,
            transparent: true,
            side: THREE.DoubleSide
        });
        var planeGeom = new THREE.PlaneGeometry(1, 1);
        var plane = new THREE.Mesh(planeGeom, planeMat);
        scene.add(plane);

        var lastFontFamily = '';
        var lastFontSize = 0;

        return {
            scene: scene,
            camera: camera,
            update: function(time, values, mediaList) {
                var cols = Math.round(values.cols || 60);
                var rows = Math.round(cols * 0.56);
                var fSize = Math.round(values.fontSize || 14);
                var lineH = Math.round(fSize * 1.14);
                var nParticles = Math.round(values.particleCount || 120);
                var aSpeed = values.attractorSpeed != null ? values.attractorSpeed : 1.0;
                var aForce = values.attractorForce != null ? values.attractorForce : 0.22;
                var decay = values.fieldDecay != null ? values.fieldDecay : 0.82;
                var mode = values.mode != null ? Math.round(values.mode) : 0;
                var fontFamily = values.fontFamily || 'Georgia';
                var wantTransparent = values.transparentBg != null ? !!values.transparentBg : true;

                var fg = values.fgColor || [1, 1, 1, 1];
                var bg = values.bgColor || [0, 0, 0, 1];
                var fgCSS = 'rgb(' + Math.round(fg[0]*255) + ',' + Math.round(fg[1]*255) + ',' + Math.round(fg[2]*255) + ')';
                var bgCSS = 'rgb(' + Math.round(bg[0]*255) + ',' + Math.round(bg[1]*255) + ',' + Math.round(bg[2]*255) + ')';

                // Rebuild palette if font changed
                if (fontFamily !== lastFontFamily || fSize !== lastFontSize) {
                    palette = buildPalette(fontFamily, fSize, bCanvas);
                    lastFontFamily = fontFamily;
                    lastFontSize = fSize;
                }
                if (!palette || palette.length === 0) return;

                // Resize particle array if needed
                while (particles.length < nParticles) {
                    particles.push({
                        x: CANVAS_W / 2 + (Math.random() - 0.5) * 80,
                        y: CANVAS_H / 2 + (Math.random() - 0.5) * 80,
                        vx: (Math.random() - 0.5) * 0.8,
                        vy: (Math.random() - 0.5) * 0.8
                    });
                }
                if (particles.length > nParticles) particles.length = nParticles;

                var now = time * 1000 * aSpeed;

                // Attractor positions (orbiting)
                var a1x = Math.cos(now * 0.0007) * CANVAS_W * 0.25 + CANVAS_W / 2;
                var a1y = Math.sin(now * 0.0011) * CANVAS_H * 0.3 + CANVAS_H / 2;
                var a2x = Math.cos(now * 0.0013 + Math.PI) * CANVAS_W * 0.2 + CANVAS_W / 2;
                var a2y = Math.sin(now * 0.0009 + Math.PI) * CANVAS_H * 0.25 + CANVAS_H / 2;

                // Update particles
                for (var i = 0; i < particles.length; i++) {
                    var p = particles[i];
                    var d1x = a1x - p.x, d1y = a1y - p.y;
                    var d2x = a2x - p.x, d2y = a2y - p.y;
                    var dist1 = d1x * d1x + d1y * d1y;
                    var dist2 = d2x * d2x + d2y * d2y;
                    var ax = dist1 < dist2 ? d1x : d2x;
                    var ay = dist1 < dist2 ? d1y : d2y;
                    var dist = Math.sqrt(Math.min(dist1, dist2)) + 1;
                    p.vx += ax / dist * aForce;
                    p.vy += ay / dist * aForce;
                    // Secondary attractor
                    var sx = dist1 < dist2 ? d2x : d1x;
                    var sy = dist1 < dist2 ? d2y : d1y;
                    var sDist = Math.sqrt(Math.max(dist1, dist2)) + 1;
                    p.vx += sx / sDist * aForce * 0.23;
                    p.vy += sy / sDist * aForce * 0.23;
                    p.vx *= 0.97;
                    p.vy *= 0.97;
                    p.x += p.vx;
                    p.y += p.vy;
                    // Wrap
                    if (p.x < 0) p.x += CANVAS_W;
                    if (p.x >= CANVAS_W) p.x -= CANVAS_W;
                    if (p.y < 0) p.y += CANVAS_H;
                    if (p.y >= CANVAS_H) p.y -= CANVAS_H;
                }

                // Decay brightness field
                for (var i = 0; i < brightnessField.length; i++) {
                    brightnessField[i] *= decay;
                }

                // Splat particles into field
                for (var i = 0; i < particles.length; i++) {
                    splatStamp(brightnessField, FIELD_COLS, FIELD_ROWS,
                        particles[i].x * FIELD_SX, particles[i].y * FIELD_SY, particleStamp);
                }
                // Splat attractors
                splatStamp(brightnessField, FIELD_COLS, FIELD_ROWS, a1x * FIELD_SX, a1y * FIELD_SY, largeStamp);
                splatStamp(brightnessField, FIELD_COLS, FIELD_ROWS, a2x * FIELD_SX, a2y * FIELD_SY, smallStamp);

                // Render text to canvas
                var cw = textCanvas.width;
                var ch = textCanvas.height;
                tCtx.clearRect(0, 0, cw, ch);

                if (!wantTransparent) {
                    tCtx.fillStyle = bgCSS;
                    tCtx.fillRect(0, 0, cw, ch);
                }

                var cellW = cw / cols;
                var cellH = ch / rows;
                tCtx.textBaseline = 'middle';

                for (var row = 0; row < rows; row++) {
                    for (var col = 0; col < cols; col++) {
                        // Sample brightness field (bilinear from oversampled grid)
                        var fx = (col + 0.5) / cols * FIELD_COLS;
                        var fy = (row + 0.5) / rows * FIELD_ROWS;
                        var fix = Math.min(FIELD_COLS - 1, Math.max(0, Math.floor(fx)));
                        var fiy = Math.min(FIELD_ROWS - 1, Math.max(0, Math.floor(fy)));
                        var brightness = brightnessField[fiy * FIELD_COLS + fix];
                        brightness = Math.min(1, brightness);

                        if (brightness < 0.03) continue;

                        var x = col * cellW;
                        var y = row * cellH + cellH * 0.5;

                        if (mode === 2) {
                            // Density only — simple mono ramp
                            var ci = Math.min(MONO_RAMP.length - 1, Math.floor(brightness * MONO_RAMP.length));
                            tCtx.font = fSize + 'px monospace';
                            tCtx.globalAlpha = brightness;
                            tCtx.fillStyle = fgCSS;
                            tCtx.fillText(MONO_RAMP[ci], x, y);
                            tCtx.globalAlpha = 1;
                        } else if (mode === 1) {
                            // Monospace ASCII
                            var ci = Math.min(MONO_RAMP.length - 1, Math.floor(brightness * MONO_RAMP.length));
                            tCtx.font = fSize + 'px monospace';
                            tCtx.globalAlpha = Math.max(0.1, brightness);
                            tCtx.fillStyle = fgCSS;
                            tCtx.fillText(MONO_RAMP[ci], x, y);
                            tCtx.globalAlpha = 1;
                        } else {
                            // Variable typographic — pick best char/weight/style from palette
                            var entry = findBest(palette, brightness, cellW);
                            tCtx.font = entry.font;
                            var alpha = Math.max(0.1, Math.min(1, brightness));
                            tCtx.globalAlpha = alpha;
                            tCtx.fillStyle = fgCSS;
                            tCtx.fillText(entry.char, x, y);
                            tCtx.globalAlpha = 1;
                        }
                    }
                }

                texture.needsUpdate = true;

                // Transparent bg
                if (wantTransparent) {
                    scene.background = null;
                } else {
                    if (!scene.background) scene.background = new THREE.Color();
                    scene.background.setRGB(bg[0], bg[1], bg[2]);
                }
            }
        };
    }

    return { create: create, INPUTS: INPUTS };
})(typeof THREE !== 'undefined' ? THREE : null);
