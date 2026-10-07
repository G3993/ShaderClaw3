// ShaderClaw — Media Inputs + Audio + Texture Helpers

// ============================================================
// Media Inputs Store
// ============================================================

const mediaInputs = [];
let mediaIdCounter = 0;

// Global mask state (persists across shader switches)
let _maskMediaId = null;
let _maskMode = 0; // 0=off, 1=multiply, 2=invert

// Audio-reactive system.
// media.js OWNS the analyser + activeAudioEntry state; ALL analysis and
// smoothing lives in the shared EaselAudio core (js/easel-audio.js), which
// is the single window._audioBus writer. AGC replaces the old noise-floor
// gate + _AUDIO_GAIN; every envelope is dt-derived (no per-frame alphas).
let audioCtx = null;
let audioAnalyser = null;
let audioDataArray = null;
let audioFFTGLTexture = null;
let audioFFTThreeTexture = null;
let audioLevel = 0, audioBass = 0, audioMid = 0, audioHigh = 0;

// --- Binding-source signals (state.js AUDIO_SIGNALS + app.js live panels) ---
// Mirrored from the EaselAudio bus every analysis frame.
var audioBassHit = 0, audioMidHit = 0, audioHighHit = 0;
var audioBassTime = 0, audioMidTime = 0, audioHighTime = 0;
// Asymmetric envelope followers (fast attack ~10ms, release ~200ms, dt-derived)
var _envBass = 0, _envMid = 0, _envHigh = 0, _envLevel = 0;
let activeAudioEntry = null;
let _micAudioStream = null;
let _micAudioSourceNode = null;
let _micAudioEntry = null;

// Variable font texture system (for Text shader "Variable Font" effect)
let _vfCanvas = null;
let _vfCtx = null;
let _vfGLTexture = null;
let _vfLastMsg = '';
let _vfWeight = 400;
const _fontFamilies = [
  '"Inter", "Segoe UI Variable", "SF Pro", sans-serif',
  '"Times New Roman", "Times", Georgia, serif',
  '"Libre Caslon Text", "Palatino Linotype", "Book Antiqua", serif',
  '"Outfit", "Inter", "Segoe UI Variable", sans-serif',
];

function _getFontStack(inputValues) {
  const idx = Math.round(inputValues['fontFamily'] || 0);
  return _fontFamilies[idx] || _fontFamilies[0];
}

// Invalidate font texture cache when Google Fonts finish loading
if (typeof document !== 'undefined' && document.fonts) {
  document.fonts.ready.then(() => { _vfLastMsg = ''; _fontAtlasLastKey = ''; _glyphAtlasLastKey = ''; _msdfFallbackKey = ''; });
}

// Message text for the varFont / breathing canvases: prefer the SCTEXT/1
// record (mixed case, any character — works for shaders without the shim
// uniforms), else rebuild from the legacy msg_i codes.
function _msgTextFromInputs(inputValues, maxLen) {
  const run = inputValues['__textrun_msg'];
  if (run && typeof run.shown === 'string') return run.shown.slice(0, maxLen);
  if (run && typeof run.text === 'string') return run.text.slice(0, maxLen);
  let msg = '';
  const msgLen = inputValues['msg_len'];
  const len = (msgLen != null && msgLen > 0) ? Math.min(msgLen, maxLen) : 0;
  for (let i = 0; i < len; i++) {
    const code = inputValues['msg_' + i];
    if (code == null || code === 26) msg += ' ';
    else if (code >= 0 && code <= 25) msg += String.fromCharCode(65 + code);
    else msg += ' ';
  }
  return msg;
}

function updateVarFontTexture(gl, inputValues) {
  // Build msg from the text run / character uniforms
  const maxLen = 24;
  let msg = _msgTextFromInputs(inputValues, maxLen);
  msg = msg.trim() || 'ETHEREA';

  // Sync weight from ISF param if available
  const iw = inputValues['fontWeight'];
  if (iw != null) _vfWeight = Math.max(100, Math.min(900, iw));

  // Only re-render canvas if text, weight, or font changed
  const fontStack = _getFontStack(inputValues);
  const key = msg + '|' + _vfWeight + '|' + fontStack;
  if (key === _vfLastMsg && _vfGLTexture) return;
  _vfLastMsg = key;

  if (!_vfCanvas) {
    _vfCanvas = document.createElement('canvas');
    _vfCanvas.width = 2048;
    _vfCanvas.height = 512;
    _vfCtx = _vfCanvas.getContext('2d');
  }

  const c = _vfCanvas;
  const ctx = _vfCtx;
  ctx.clearRect(0, 0, c.width, c.height);
  ctx.save();
  // Flip vertically for GL coord system
  ctx.translate(0, c.height);
  ctx.scale(1, -1);
  const w = Math.round(_vfWeight);
  ctx.font = w + ' ' + Math.round(c.height * 0.35) + 'px ' + fontStack;
  ctx.fillStyle = '#ffffff';
  ctx.textAlign = 'center';
  ctx.textBaseline = 'middle';
  ctx.fillText(msg, c.width / 2, c.height / 2);
  ctx.restore();

  if (!_vfGLTexture) {
    _vfGLTexture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, _vfGLTexture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  } else {
    gl.bindTexture(gl.TEXTURE_2D, _vfGLTexture);
  }
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, c);
}

// Breathing texture: per-character variable font weight wave (effect 22)
// Renders each character at a staggered weight based on time, creating a
// "breathing" wave across the text (inspired by Splitting.js + variable fonts)
let _breatheStartTime = performance.now();

function updateBreathingTexture(gl, inputValues) {
  const maxLen = 24;
  let msg = _msgTextFromInputs(inputValues, maxLen);
  msg = msg.trim() || 'ETHEREA';

  const fontStack = _getFontStack(inputValues);
  const spd = inputValues['speed'] != null ? inputValues['speed'] : 0.5;
  const intens = inputValues['intensity'] != null ? inputValues['intensity'] : 0.5;
  const elapsed = (performance.now() - _breatheStartTime) / 1000;

  // Weight range driven by intensity: center ± spread
  const baseWeight = inputValues['fontWeight'] != null ? inputValues['fontWeight'] : 400;
  const spread = intens * 400; // 0..400 range from center
  const minW = Math.max(100, baseWeight - spread);
  const maxW = Math.min(900, baseWeight + spread);

  // Stagger delay per char driven by density
  const dens = inputValues['density'] != null ? inputValues['density'] : 0.5;
  const charDelay = 0.15 + (1.0 - dens) * 0.85; // 0.15s (tight wave) to 1.0s (wide wave)

  if (!_vfCanvas) {
    _vfCanvas = document.createElement('canvas');
    _vfCanvas.width = 2048;
    _vfCanvas.height = 512;
    _vfCtx = _vfCanvas.getContext('2d');
  }

  const c = _vfCanvas;
  const ctx = _vfCtx;
  ctx.clearRect(0, 0, c.width, c.height);
  ctx.save();
  ctx.translate(0, c.height);
  ctx.scale(1, -1);

  const fontSize = Math.round(c.height * 0.35);
  ctx.fillStyle = '#ffffff';
  ctx.textAlign = 'left';
  ctx.textBaseline = 'middle';

  // Measure total width at mid-weight to center the text
  const midWeight = Math.round((minW + maxW) / 2);
  ctx.font = midWeight + ' ' + fontSize + 'px ' + fontStack;
  const totalWidth = ctx.measureText(msg).width;
  let x = (c.width - totalWidth) / 2;

  // Draw each character with its own staggered weight
  for (let i = 0; i < msg.length; i++) {
    const phase = elapsed * spd * Math.PI * 2 - i * charDelay;
    const t = (Math.sin(phase) + 1) / 2; // 0..1
    const w = Math.round(minW + t * (maxW - minW));

    ctx.font = w + ' ' + fontSize + 'px ' + fontStack;
    ctx.fillText(msg[i], x, c.height / 2);
    x += ctx.measureText(msg[i]).width;
  }

  ctx.restore();

  if (!_vfGLTexture) {
    _vfGLTexture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, _vfGLTexture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  } else {
    gl.bindTexture(gl.TEXTURE_2D, _vfGLTexture);
  }
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, c);
  // Invalidate varfont cache so switching back to effect 20 re-renders
  _vfLastMsg = '';
}

// Font atlas for bitmap effects (0-19) using web fonts
let _fontAtlasCanvas = null;
let _fontAtlasCtx = null;
let _fontAtlasGLTexture = null;
let _fontAtlasLastKey = '';

function updateFontAtlas(gl, inputValues) {
  const fontFamilyIdx = Math.round(inputValues['fontFamily'] || 0);
  // Always generate atlas (even for fontFamily=0) to avoid 26-branch charData() in shader
  const fontStack = _fontFamilies[fontFamilyIdx] || _fontFamilies[0];
  const weight = Math.round(inputValues['fontWeight'] || 400);
  const key = fontStack + '|' + weight + '|512';
  if (key === _fontAtlasLastKey && _fontAtlasGLTexture) return;
  _fontAtlasLastKey = key;

  const cellW = 384, cellH = 360;
  const ATLAS_CHARS = 37; // A-Z (0-25), space (26), 0-9 (27-36)
  const totalW = ATLAS_CHARS * cellW; // 37 * 384 = 14,208 (under 16,384 max)

  if (!_fontAtlasCanvas || _fontAtlasCanvas.width !== totalW) {
    _fontAtlasCanvas = document.createElement('canvas');
    _fontAtlasCanvas.width = totalW;
    _fontAtlasCanvas.height = cellH;
    _fontAtlasCtx = _fontAtlasCanvas.getContext('2d');
  }

  const c = _fontAtlasCanvas;
  const ctx = _fontAtlasCtx;
  ctx.clearRect(0, 0, c.width, c.height);
  ctx.save();
  // Flip vertically for GL coord system
  ctx.translate(0, c.height);
  ctx.scale(1, -1);

  const fontSize = Math.round(cellH * 0.85);
  ctx.font = weight + ' ' + fontSize + 'px ' + fontStack;
  ctx.fillStyle = '#ffffff';
  ctx.textAlign = 'center';
  ctx.textBaseline = 'middle';

  for (let i = 0; i < 26; i++) {
    ctx.fillText(String.fromCharCode(65 + i), (i + 0.5) * cellW, cellH / 2);
  }
  // Index 26 = space (leave blank)
  // Index 27-36 = digits 0-9
  for (let i = 0; i < 10; i++) {
    ctx.fillText(String(i), (27 + i + 0.5) * cellW, cellH / 2);
  }

  ctx.restore();

  if (!_fontAtlasGLTexture) {
    _fontAtlasGLTexture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, _fontAtlasGLTexture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  } else {
    gl.bindTexture(gl.TEXTURE_2D, _fontAtlasGLTexture);
  }
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, c);
}

// SCTEXT/1 glyph atlas for converted text shaders (`glyphAtlasTex`):
// 16 cols x 6 rows = 96 cells, cp 32..126 -> cell cp-32, cell 95 = '?'
// (replacement glyph). 192x270 cells -> 3072x1620 canvas (under the 4096
// mobile limit). Same font/weight cache key + fonts.ready invalidation and
// the same canvas pre-flip (no UNPACK_FLIP_Y) as updateFontAtlas, so
// row 0 (cp 32..47) is the TOP band in texture space and glyph-local
// uv.y = 1 is the glyph top — see sc_text v1 scGlyphUV() in js/isf.js.
let _glyphAtlasCanvas = null;
let _glyphAtlasCtx = null;
let _glyphAtlasGLTexture = null;
let _glyphAtlasLastKey = '';

function updateGlyphAtlas(gl, inputValues) {
  const fontFamilyIdx = Math.round(inputValues['fontFamily'] || 0);
  const fontStack = _fontFamilies[fontFamilyIdx] || _fontFamilies[0];
  const weight = Math.round(inputValues['fontWeight'] || 400);
  const key = fontStack + '|' + weight + '|glyph96';
  if (key === _glyphAtlasLastKey && _glyphAtlasGLTexture) return;
  _glyphAtlasLastKey = key;

  const COLS = 16, ROWS = 6, CELLS = 96;
  const cellW = 192, cellH = 270;
  const totalW = COLS * cellW, totalH = ROWS * cellH; // 3072 x 1620

  if (!_glyphAtlasCanvas || _glyphAtlasCanvas.width !== totalW || _glyphAtlasCanvas.height !== totalH) {
    _glyphAtlasCanvas = document.createElement('canvas');
    _glyphAtlasCanvas.width = totalW;
    _glyphAtlasCanvas.height = totalH;
    _glyphAtlasCtx = _glyphAtlasCanvas.getContext('2d');
  }

  const c = _glyphAtlasCanvas;
  const ctx = _glyphAtlasCtx;
  ctx.clearRect(0, 0, c.width, c.height);
  ctx.save();
  // Flip vertically for GL coord system (same convention as updateFontAtlas)
  ctx.translate(0, c.height);
  ctx.scale(1, -1);

  // Glyph centred horizontally, baseline 0.25 cell from the cell bottom,
  // font size 0.72 cellH so ascenders/descenders fit inside the cell.
  const fontSize = Math.round(cellH * 0.72);
  ctx.font = weight + ' ' + fontSize + 'px ' + fontStack;
  ctx.fillStyle = '#ffffff';
  ctx.textAlign = 'center';
  ctx.textBaseline = 'alphabetic';

  for (let idx = 0; idx < CELLS; idx++) {
    const ch = idx < 95 ? String.fromCharCode(32 + idx) : '?';
    if (idx === 0) continue; // space
    const col = idx % COLS;
    const row = Math.floor(idx / COLS);
    // In the flipped ctx, smaller y = higher in texture space: row 0 at y in [0, cellH]
    ctx.fillText(ch, (col + 0.5) * cellW, row * cellH + cellH * 0.75);
  }

  ctx.restore();

  if (!_glyphAtlasGLTexture) {
    _glyphAtlasGLTexture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, _glyphAtlasGLTexture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  } else {
    gl.bindTexture(gl.TEXTURE_2D, _glyphAtlasGLTexture);
  }
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, c);
}

// ============================================================
// SCFONT/1 MSDF faces (Phase D) — assets/fonts/<id>.{json,msdf.png,metrics.png}
// generated by tools/fonts/gen_msdf_atlas.mjs; contract in
// .claude/caption-pipeline/phase-D-design.md. Bound by renderer.js
// _bindMSDFGlyphs when a shader references glyphMSDFTex/glyphMetricsTex.
// When the assets are missing, getMSDFFallbackFace synthesizes an SCFONT/1
// face from updateGlyphAtlas's 16x6 coverage grid (§1.7).
// ============================================================
let _msdfIndex = null;          // parsed assets/fonts/fonts.json
let _msdfIndexState = 'idle';   // idle | loading | ready | failed
const _msdfFaces = {};          // face id -> record (see ensureMSDFFace)
let _msdfFallback = null;       // synthesized SCFONT/1 face from the 16x6 grid
let _msdfFallbackKey = '';
let _msdfLoggedFallback = false;

function _msdfEnsureIndex() {
  if (_msdfIndexState !== 'idle') return;
  _msdfIndexState = 'loading';
  fetch('assets/fonts/fonts.json')
    .then((r) => { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); })
    .then((j) => { _msdfIndex = j; _msdfIndexState = 'ready'; console.log('SCFONT/1 fonts.json loaded (' + ((j.faces || []).length) + ' faces)'); })
    .catch((e) => {
      _msdfIndexState = 'failed';
      if (!_msdfLoggedFallback) {
        _msdfLoggedFallback = true;
        console.warn('SCFONT/1 assets not found at assets/fonts/fonts.json; using synthesized grid face (' + (e && e.message) + ')');
      }
    });
}

// fontFamily/fontWeight -> face id (null until fonts.json has loaded).
function resolveMSDFFace(inputValues) {
  _msdfEnsureIndex();
  if (_msdfIndexState !== 'ready' || !_msdfIndex || typeof SCMSDFMetrics === 'undefined') return null;
  const weightById = {};
  for (const id in _msdfFaces) {
    const f = _msdfFaces[id];
    if (f && f.weight != null) weightById[id] = f.weight;
  }
  const iv = inputValues || {};
  return SCMSDFMetrics.resolveFace(iv['fontFamily'] || 0,
    iv['fontWeight'] != null ? iv['fontWeight'] : 400, _msdfIndex, weightById);
}

// Kick off (or return) the async load of one face. The record's `ready` flag
// flips once <id>.json + both PNGs are fetched and uploaded; callers use the
// fallback face until then.
function ensureMSDFFace(gl, id) {
  if (!id) return null;
  let f = _msdfFaces[id];
  if (f) {
    if (f.ready && f._glCtx !== gl) _msdfUploadFace(gl, f); // GL context changed
    return f;
  }
  f = _msdfFaces[id] = {
    id, weight: null, json: null, atlasImg: null, metricsImg: null,
    atlasTex: null, metricsTex: null, atlasInfo: null, faceInfo: null,
    ready: false, failed: false, _glCtx: null,
  };
  fetch('assets/fonts/' + id + '.json')
    .then((r) => { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); })
    .then((json) => new Promise((resolve, reject) => {
      f.json = json;
      f.weight = (json.sctext && json.sctext.weight != null) ? json.sctext.weight : null;
      const m = json.metrics || {};
      f.atlasInfo = [json.atlas.width, json.atlas.height, json.atlas.distanceRange, json.atlas.size];
      f.faceInfo = [
        m.ascender != null ? m.ascender : 0.93,
        m.descender != null ? m.descender : -0.24,
        m.lineHeight != null ? m.lineHeight : 1.2,
        m.underlineY != null ? m.underlineY : -0.1,
      ];
      let left = 2;
      const done = () => { if (--left === 0) resolve(); };
      const img = (src) => { const i = new Image(); i.onload = done; i.onerror = () => reject(new Error('image ' + src)); i.src = src; return i; };
      f.atlasImg = img('assets/fonts/' + ((json.sctext && json.sctext.atlas) || (id + '.msdf.png')));
      f.metricsImg = img('assets/fonts/' + ((json.sctext && json.sctext.metricsTexture) || (id + '.metrics.png')));
    }))
    .then(() => { _msdfUploadFace(gl, f); })
    .catch((e) => {
      f.failed = true;
      if (!_msdfLoggedFallback) {
        _msdfLoggedFallback = true;
        console.warn('SCFONT/1 face "' + id + '" failed to load; using synthesized grid face (' + (e && e.message) + ')');
      }
    });
  return f;
}

function _msdfUploadFace(gl, f) {
  const tex = (img, filter) => {
    const t = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, t);
    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true); // upright upload: PNG bottom row at v = 0
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, img);
    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, filter);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, filter);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    return t;
  };
  f.atlasTex = tex(f.atlasImg, gl.LINEAR);      // MSDF atlas: LINEAR, no mips
  f.metricsTex = tex(f.metricsImg, gl.NEAREST); // metrics: exact texels
  f._glCtx = gl;
  f.ready = true;
}

// Synthesized SCFONT/1 face from the 16x6 coverage grid (design §1.7): the
// MSDF median of a white coverage glyph IS the coverage, so the grid texture
// doubles as glyphMSDFTex with pxrange 2. Metrics texels cover cp 32..126 +
// U+FFFD (cell 95); advances measured with Canvas2D, plane box = the cell
// geometry (em = 0.72*cellH, baseline 0.75*cellH from the cell top).
function getMSDFFallbackFace(gl, inputValues) {
  updateGlyphAtlas(gl, inputValues || {});
  if (!_glyphAtlasGLTexture || !_glyphAtlasCtx || typeof SCMSDFMetrics === 'undefined') return null;
  const key = _glyphAtlasLastKey + '|' + '2048x256';
  if (_msdfFallback && _msdfFallbackKey === key && _msdfFallback._glCtx === gl) {
    _msdfFallback.atlasTex = _glyphAtlasGLTexture; // grid texture may have been re-created
    return _msdfFallback;
  }
  const COLS = 16, cellW = 192, cellH = 270;
  const fontSize = Math.round(cellH * 0.72);
  const iv = inputValues || {};
  const fontStack = _fontFamilies[Math.round(iv['fontFamily'] || 0)] || _fontFamilies[0];
  const weight = Math.round(iv['fontWeight'] || 400);
  const ctx = _glyphAtlasCtx;
  ctx.save();
  ctx.font = weight + ' ' + fontSize + 'px ' + fontStack;
  const glyphs = [];
  for (let n = 0; n <= 95; n++) {
    const cp = n < 95 ? 32 + n : 0xFFFD;
    const idx = n < 95 ? n : 95;
    const ch = n < 95 ? String.fromCharCode(cp) : '?';
    const col = idx % COLS, row = Math.floor(idx / COLS);
    let adv = 0.6;
    try { const w = ctx.measureText(ch).width; if (w > 0) adv = w / fontSize; } catch (e) {}
    const left = col * cellW, bottom = (5 - row) * cellH; // upright px, y up
    glyphs.push({
      unicode: cp, advance: adv,
      planeBounds: { left: 0.0, bottom: -0.347, right: 0.987, top: 1.042 },
      atlasBounds: { left, bottom, right: left + cellW, top: bottom + cellH },
    });
  }
  ctx.restore();
  // Upload rows bottom-up with FLIP_Y=false so page p lands at GL row p.
  const bytes = SCMSDFMetrics.encodeGlyphTexels(glyphs, { rowOrder: 'bottom-up' });
  const reuse = _msdfFallback && _msdfFallback._glCtx === gl ? _msdfFallback.metricsTex : null;
  const mtex = reuse || gl.createTexture();
  gl.bindTexture(gl.TEXTURE_2D, mtex);
  gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, SCMSDFMetrics.TEX_W, SCMSDFMetrics.TEX_H, 0, gl.RGBA, gl.UNSIGNED_BYTE, bytes);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  _msdfFallback = {
    id: '(grid-fallback)', atlasTex: _glyphAtlasGLTexture, metricsTex: mtex,
    atlasInfo: [3072, 1620, 2.0, 0.72 * 270], faceInfo: [0.93, -0.24, 1.2, -0.1],
    ready: true, _glCtx: gl,
  };
  _msdfFallbackKey = key;
  return _msdfFallback;
}

// Audio-reactive per-frame update (global scope — called from Renderer.render)
// Cached DOM elements for audio UI updates (avoid per-frame getElementById)
let _cachedAudioSignalFill = null;
let _cachedAudioLevelFill = null;
let _cachedAudioBarFill = null;
let _cachedAudioBarId = null;
let _audioFrameCount = 0;

// Analysis throttle: render loops call updateAudioUniforms several times per
// composited frame (main render + per layer). The bus is dt-derived so
// multiple steps stay correct, but re-analyzing the same analyser snapshot is
// wasted work — re-ingest at most every ~4ms.
let _lastAudioIngestMs = 0;

function updateAudioUniforms(gl) {
  const engine = (typeof window !== 'undefined') ? window.EaselAudio : null;
  const nowMs = performance.now();

  if (!audioAnalyser || !activeAudioEntry) {
    audioLevel = audioBass = audioMid = audioHigh = 0;
    audioBassHit = audioMidHit = audioHighHit = 0;
    _envBass = _envMid = _envHigh = _envLevel = 0;
    // Bus must exist and keep being written even with no source (all zeros,
    // Time clocks frozen) so consumers never see a null/stale bus.
    if (engine && nowMs - _lastAudioIngestMs >= 4) {
      _lastAudioIngestMs = nowMs;
      engine.ingestSilence();
      engine.publishBus();
    }
    // Only update DOM every 8th frame when idle
    if ((_audioFrameCount++ & 7) === 0) {
      if (!_cachedAudioSignalFill) _cachedAudioSignalFill = document.getElementById('audio-signal-fill');
      if (_cachedAudioSignalFill) _cachedAudioSignalFill.style.width = '0%';
      if (!_cachedAudioLevelFill) _cachedAudioLevelFill = document.getElementById('audio-level-fill');
      if (_cachedAudioLevelFill) _cachedAudioLevelFill.style.width = '0%';
    }
    return;
  }
  _audioFrameCount++;

  if (nowMs - _lastAudioIngestMs >= 4) {
    const dtSec = Math.min(0.1, Math.max(0.001, (_lastAudioIngestMs > 0 ? nowMs - _lastAudioIngestMs : 16.7) / 1000));
    _lastAudioIngestMs = nowMs;
    audioAnalyser.getByteFrequencyData(audioDataArray);
    const len = audioDataArray.length; // frequencyBinCount (1024 @ fftSize 2048)

    // Raw RMS level (linear) — the engine converts to dB for the level AGC
    let sum = 0;
    for (let i = 0; i < len; i++) sum += audioDataArray[i] * audioDataArray[i];
    const rawLevel = Math.sqrt(sum / len) / 255.0;

    if (engine) {
      engine.ingestSpectrum(audioDataArray, {
        sampleRate: audioCtx ? audioCtx.sampleRate : 44100,
        level: rawLevel,
        byteMinDb: audioAnalyser.minDecibels,
        byteMaxDb: audioAnalyser.maxDecibels,
      });
      const bus = engine.publishBus();
      const fl = bus.floats;
      // Legacy quartet: same names, same 0-1 ranges, AGC'd + visual-binding
      // conditioned (10/500ms == the house 0.85 feel) under the hood.
      audioLevel = fl.audioLevel;
      audioBass = fl.audioBass;
      audioMid = fl.audioMid;
      audioHigh = fl.audioHigh;
      // Binding-source mirrors (state.js AUDIO_SIGNALS)
      audioBassHit = fl.audioBassHit;
      audioMidHit = fl.audioMidHit;
      audioHighHit = fl.audioHighHit;
      audioBassTime = fl.audioBassTime;
      audioMidTime = fl.audioMidTime;
      audioHighTime = fl.audioHighTime;
      // Envelope followers (fast attack, ~200ms release) over the raw AGC'd
      // bands — punchier than the conditioned quartet, dt-derived.
      const raw = engine.raw, slewAR = engine.dsp.slewAR;
      _envBass = slewAR(_envBass, raw.bass, dtSec, 0.01, 0.2);
      _envMid = slewAR(_envMid, raw.mid, dtSec, 0.01, 0.2);
      _envHigh = slewAR(_envHigh, raw.high, dtSec, 0.01, 0.2);
      _envLevel = slewAR(_envLevel, raw.level, dtSec, 0.01, 0.2);
    } else {
      // Core script missing (should not happen in index.html) — minimal
      // unsmoothed fallback so the app still shows signal.
      const nyquist = (audioCtx ? audioCtx.sampleRate : 44100) / 2;
      const binAvg = (fLo, fHi) => {
        let a = Math.max(0, Math.floor((fLo / nyquist) * len));
        let b = Math.min(len, Math.ceil((fHi / nyquist) * len));
        if (b <= a) b = a + 1;
        let s = 0;
        for (let i = a; i < b; i++) s += audioDataArray[i];
        return s / ((b - a) * 255);
      };
      audioLevel = rawLevel;
      audioBass = binAvg(20, 250);
      audioMid = binAvg(250, 2000);
      audioHigh = binAvg(2000, 16000);
    }

    // Upload FFT data to GL texture (len x 1 LUMINANCE — layout unchanged)
    // Use a dedicated texture unit to avoid overwriting the currently active unit
    if (!audioFFTGLTexture) {
      audioFFTGLTexture = gl.createTexture();
      gl.activeTexture(gl.TEXTURE15);
      gl.bindTexture(gl.TEXTURE_2D, audioFFTGLTexture);
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    }
    gl.activeTexture(gl.TEXTURE15);
    gl.bindTexture(gl.TEXTURE_2D, audioFFTGLTexture);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.LUMINANCE, len, 1, 0, gl.LUMINANCE, gl.UNSIGNED_BYTE, audioDataArray);

    // Update THREE DataTexture
    if (!audioFFTThreeTexture) {
      audioFFTThreeTexture = new THREE.DataTexture(audioDataArray, len, 1, THREE.LuminanceFormat);
      audioFFTThreeTexture.needsUpdate = true;
    } else {
      audioFFTThreeTexture.needsUpdate = true;
    }
  }

  // Update audio UI bars only every 4th frame (DOM writes are expensive)
  if ((_audioFrameCount & 3) === 0) {
    if (activeAudioEntry) {
      if (_cachedAudioBarId !== activeAudioEntry.id) {
        _cachedAudioBarFill = document.querySelector('.audio-bar-fill[data-audio-id="' + activeAudioEntry.id + '"]');
        _cachedAudioBarId = activeAudioEntry.id;
      }
      if (_cachedAudioBarFill) _cachedAudioBarFill.style.width = (audioLevel * 100) + '%';
    }
    if (!_cachedAudioSignalFill) _cachedAudioSignalFill = document.getElementById('audio-signal-fill');
    if (_cachedAudioSignalFill) _cachedAudioSignalFill.style.width = (audioLevel * 100) + '%';
    if (!_cachedAudioLevelFill) _cachedAudioLevelFill = document.getElementById('audio-level-fill');
    if (_cachedAudioLevelFill) _cachedAudioLevelFill.style.width = (audioLevel * 100) + '%';
  }
}

function detectMediaType(file) {
  const ext = file.name.split('.').pop().toLowerCase();
  if (['glb', 'gltf', 'stl', 'fbx', 'obj'].includes(ext)) return 'model';
  if (['mp3', 'wav', 'ogg'].includes(ext)) return 'audio';
  if (ext === 'svg') return 'svg';
  if (file.type.startsWith('video/')) return 'video';
  return 'image';
}

function mediaTypeIcon(type, name) {
  if (type === 'video' && name === 'Webcam') return '\u{1F4F9}';
  if (type === 'video') return '\u{1F3AC}';
  if (type === 'model') return '\u{1F9CA}';
  if (type === 'audio' && name === 'Microphone') return '\u{1F3A4}';
  if (type === 'audio') return '\u{1F50A}';
  if (type === 'svg') return '\u{2712}';
  return '\u{1F5BC}';
}

function createGLTexture(gl, source) {
  const tex = gl.createTexture();
  gl.bindTexture(gl.TEXTURE_2D, tex);
  gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, source);
  gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
  gl.bindTexture(gl.TEXTURE_2D, null);
  return tex;
}
