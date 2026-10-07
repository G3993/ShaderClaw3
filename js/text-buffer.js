// ============================================================
// ShaderClaw — SCTEXT/1 text encoder (glyph buffer + legacy shim)
// ============================================================
// Shared contract with Easel (GlyphBuffer.cpp) and Etherea
// (text-buffer-encoder.js). See .claude/caption-pipeline/phase-C-design.md.
//
// Classic script: no import/export, no top-level `this`/`module`, so the
// same file loads from index.html (<script>) and from node as a
// side-effect ES module (`import '../js/text-buffer.js'` -> globalThis.SCTextBuffer).
//
// Glyph slot i (0 <= i < cap) = two RGBA8 texels in textBuf_<name>:
//   texel 2i   : R = cp & 255, G = cp >> 8, B = wordIndex (255 = none), A = 255
//   texel 2i+1 : R,G = revealMs lo/hi (65535 = unknown), B = speakerId (255 = unknown), A = 255
// Slots >= len are zero-filled (A still 255). Alpha is always 255 (load-bearing
// for the etherea canvas upload path — never put data in A).

const SCTextBuffer = (function () {
  'use strict';

  const CAP_MAX = 1024;
  const SHIM_CAP_MAX = 64;
  const SHIM_CAP_MAX_MOBILE = 48;
  const WORD_NONE = 255;
  const WORD_MAX = 254;
  const REVEAL_UNKNOWN = 65535;
  const REVEAL_MAX = 65534;
  const SPEAKER_UNKNOWN = 255;
  const SPEAKER_MAX = 254;

  // Same test as js/isf.js isfInputToUniform (mobile GPU uniform budget).
  function isMobile() {
    return typeof window !== 'undefined' && typeof navigator !== 'undefined' &&
      (window.innerWidth <= 900 || /Mobi|Android|iPhone/i.test(navigator.userAgent));
  }

  // Legacy 37-cell alphabet: A-Z/a-z -> 0..25, space -> 26, 0-9 -> 27..36, else 26.
  function legacyCharCode(cp) {
    if (cp >= 65 && cp <= 90) return cp - 65;
    if (cp >= 97 && cp <= 122) return cp - 97;
    if (cp >= 48 && cp <= 57) return cp - 48 + 27;
    return 26;
  }

  // JS string -> array of code points (astral -> U+FFFD, lone surrogates -> U+FFFD).
  function toCodePoints(text) {
    const s = String(text == null ? '' : text);
    const out = [];
    for (let i = 0; i < s.length;) {
      const cp = s.codePointAt(i);
      i += cp > 0xFFFF ? 2 : 1;
      if (cp > 0xFFFF || (cp >= 0xD800 && cp <= 0xDFFF)) out.push(0xFFFD);
      else out.push(cp);
    }
    return out;
  }

  function isSpaceCp(cp) {
    return cp === 32 || cp === 9 || cp === 10 || cp === 13 || cp === 11 || cp === 12 || cp === 0xA0 ||
      (cp > 127 && /\s/.test(String.fromCodePoint(cp)));
  }

  // Per-code-point case fold that never changes the glyph count (so word
  // indices into the original text stay valid). Multi-char folds (ß -> SS)
  // keep the original code point.
  function foldCase(cp, mode) {
    if (mode !== 'upper' && mode !== 'lower') return cp;
    if (cp < 128) {
      if (mode === 'upper') return (cp >= 97 && cp <= 122) ? cp - 32 : cp;
      return (cp >= 65 && cp <= 90) ? cp + 32 : cp;
    }
    const ch = String.fromCodePoint(cp);
    const f = mode === 'upper' ? ch.toUpperCase() : ch.toLowerCase();
    if (f.length === 0) return cp;
    const fcp = f.codePointAt(0);
    const fl = fcp > 0xFFFF ? 2 : 1;
    return (f.length === fl && fcp <= 0xFFFF) ? fcp : cp;
  }

  function normCaseMode(v) {
    const m = String(v == null ? 'preserve' : v).toLowerCase();
    return (m === 'upper' || m === 'lower') ? m : 'preserve';
  }

  function clampInt(v, lo, hi) {
    v = Math.floor(Number(v));
    if (!isFinite(v)) v = lo;
    return Math.max(lo, Math.min(hi, v));
  }

  /**
   * encodeTextRun(run, opts) -> { len, bytes: Uint8Array(2*cap*4), shimCodes: Array(shimCap), shimLen }
   * run  = { text, words?: [{start, end, revealMs?, speakerId?}], speakerId? }  (string accepted as text)
   * opts = { cap, shimCap, caseMode: 'preserve'|'upper'|'lower', overflowHead: bool }
   */
  function encodeTextRun(run, opts) {
    if (typeof run === 'string' || run == null) run = { text: run };
    opts = opts || {};
    const cap = clampInt(opts.cap != null ? opts.cap : 12, 1, CAP_MAX);
    const shimCap = clampInt(opts.shimCap != null ? opts.shimCap : Math.min(cap, SHIM_CAP_MAX), 0, SHIM_CAP_MAX);
    const caseMode = normCaseMode(opts.caseMode);
    const overflowHead = !!opts.overflowHead;
    const runSpeaker = (run.speakerId != null && run.speakerId >= 0) ? clampInt(run.speakerId, 0, SPEAKER_MAX) : SPEAKER_UNKNOWN;

    const all = toCodePoints(run.text);
    for (let i = 0; i < all.length; i++) all[i] = foldCase(all[i], caseMode);

    // Per-glyph timing / speaker from run.words (indices into the ORIGINAL glyph list)
    const revealAll = new Array(all.length).fill(REVEAL_UNKNOWN);
    const speakerAll = new Array(all.length).fill(runSpeaker);
    if (Array.isArray(run.words)) {
      for (const w of run.words) {
        if (!w) continue;
        const ws = clampInt(w.start != null ? w.start : 0, 0, all.length);
        const we = clampInt(w.end != null ? w.end : ws, ws, all.length);
        const rm = (w.revealMs != null && w.revealMs >= 0) ? clampInt(w.revealMs, 0, REVEAL_MAX) : REVEAL_UNKNOWN;
        const sp = (w.speakerId != null && w.speakerId >= 0) ? clampInt(w.speakerId, 0, SPEAKER_MAX) : runSpeaker;
        for (let g = ws; g < we; g++) { revealAll[g] = rm; speakerAll[g] = sp; }
      }
    }

    // OVERFLOW window (applied BEFORE word-index assignment: indices restart at 0 for the kept window)
    const pick = (n) => {
      if (all.length <= n) return 0;
      return overflowHead ? 0 : all.length - n;
    };
    const off = pick(cap);
    const len = Math.min(all.length, cap);

    const bytes = new Uint8Array(2 * cap * 4);
    for (let t = 0; t < 2 * cap; t++) bytes[t * 4 + 3] = 255;
    let wordIdx = -1, inWord = false;
    for (let i = 0; i < len; i++) {
      const cp = all[off + i];
      const sp = isSpaceCp(cp);
      if (!sp && !inWord) wordIdx++;
      inWord = !sp;
      const wi = sp ? WORD_NONE : Math.min(wordIdx, WORD_MAX);
      const rm = revealAll[off + i];
      const o = i * 8;
      bytes[o + 0] = cp & 255;
      bytes[o + 1] = (cp >> 8) & 255;
      bytes[o + 2] = wi;
      bytes[o + 4] = rm & 255;
      bytes[o + 5] = (rm >> 8) & 255;
      bytes[o + 6] = speakerAll[off + i];
    }

    // Legacy shim: same glyph list, independent window on shimCap glyphs
    const soff = pick(shimCap);
    const shimLen = Math.min(all.length, shimCap);
    const shimCodes = new Array(shimCap);
    for (let i = 0; i < shimCap; i++) shimCodes[i] = i < shimLen ? legacyCharCode(all[soff + i]) : 26;

    const shownCps = all.slice(off, off + len);
    return { len, bytes, shimCodes, shimLen, cap, shimCap, shown: String.fromCodePoint.apply(null, shownCps) };
  }

  /** ISF TYPE:"text" input -> encoder options (MAX_LENGTH / CASE / OVERFLOW). */
  function optionsFromInput(inp) {
    inp = inp || {};
    const maxLen = (inp.MAX_LENGTH != null && inp.MAX_LENGTH > 0) ? inp.MAX_LENGTH : 12;
    return {
      cap: clampInt(maxLen, 1, CAP_MAX),
      shimCap: Math.min(Math.floor(maxLen), isMobile() ? SHIM_CAP_MAX_MOBILE : SHIM_CAP_MAX),
      caseMode: normCaseMode(inp.CASE),
      overflowHead: String(inp.OVERFLOW == null ? 'tail' : inp.OVERFLOW).toLowerCase() === 'head',
    };
  }

  /**
   * Encode `text` (string or TextRun) for ISF input `inp` into inputValues:
   *   inputValues[NAME+'_len']            = len
   *   inputValues[NAME+'_'+i] (i<shimCap) = legacy codes, only when inp._shim (undefined = shim)
   *   inputValues['__textrun_'+NAME]      = { text, shown, len, cap, bytes, dirty:true } (record object reused
   *                                          across calls so the renderer can cache its GL texture on it)
   */
  function applyToInputValues(inputValues, inp, text) {
    if (!inputValues || !inp || !inp.NAME) return null;
    const n = inp.NAME;
    const opts = optionsFromInput(inp);
    const enc = encodeTextRun(text, opts);
    inputValues[n + '_len'] = enc.len;
    if (inp._shim !== false) {
      for (let i = 0; i < opts.shimCap; i++) inputValues[n + '_' + i] = enc.shimCodes[i];
    }
    const key = '__textrun_' + n;
    const prev = inputValues[key];
    const rec = (prev && typeof prev === 'object') ? prev : {};
    rec.text = (typeof text === 'string') ? text : String((text && text.text) || '');
    rec.shown = enc.shown;
    rec.len = enc.len;
    rec.cap = enc.cap;
    rec.bytes = enc.bytes;
    rec.dirty = true;
    inputValues[key] = rec;
    return rec;
  }

  // ---- word alignment (for Cue / VoxTerm word timings) ----
  function normToken(s) { return String(s == null ? '' : s).toLowerCase().replace(/[^\p{L}\p{N}]+/gu, ''); }
  function speakerToId(sp) {
    if (sp == null) return -1;
    if (typeof sp === 'number') return sp >= 0 ? Math.floor(sp) : -1;
    const m = String(sp).match(/(\d+)\s*$/);
    return m ? parseInt(m[1], 10) : -1;
  }

  /**
   * alignWords(shownText, words, startMs) -> TextRun words [{start, end, revealMs, speakerId}]
   * `words` accepts Cue shape {w, s, e, speaker} (ms) or {text|word, start, end, speaker} (seconds).
   * Walks words sequentially from the START of shownText, case-insensitive, comparing alphanumerics
   * only; unmatched tokens get revealMs -1. revealMs = round(word.s - startMs) clamped to [0, 65534]
   * (only when both are > 0).
   */
  function alignWords(shownText, words, startMs) {
    const cps = toCodePoints(shownText);
    const tokens = [];
    let ts = -1;
    for (let i = 0; i <= cps.length; i++) {
      const sp = i === cps.length || isSpaceCp(cps[i]);
      if (!sp && ts < 0) ts = i;
      if (sp && ts >= 0) { tokens.push({ start: ts, end: i, norm: normToken(String.fromCodePoint.apply(null, cps.slice(ts, i))) }); ts = -1; }
    }
    const list = Array.isArray(words) ? words : [];
    const cw = list.map((w) => {
      if (!w) return null;
      const isCue = w.w != null || w.s != null;
      const txt = isCue ? w.w : (w.word != null ? w.word : w.text);
      const sMs = isCue ? w.s : (w.start != null ? w.start * 1000 : null);
      return { norm: normToken(txt), sMs: (typeof sMs === 'number' && isFinite(sMs)) ? sMs : null, speaker: speakerToId(w.speaker != null ? w.speaker : w.speakerId) };
    });
    const base = (typeof startMs === 'number' && startMs > 0) ? startMs : 0;
    let wi = 0;
    const out = [];
    for (const t of tokens) {
      let revealMs = -1, speakerId = -1;
      if (t.norm) {
        let hit = -1;
        for (let k = wi; k < cw.length && k < wi + 4; k++) { if (cw[k] && cw[k].norm === t.norm) { hit = k; break; } }
        if (hit >= 0) {
          const w = cw[hit];
          if (w.sMs != null && w.sMs > 0 && base > 0) revealMs = clampInt(Math.round(w.sMs - base), 0, REVEAL_MAX);
          speakerId = w.speaker;
          wi = hit + 1;
        }
      }
      out.push({ start: t.start, end: t.end, revealMs, speakerId });
    }
    return out;
  }

  return {
    CAP_MAX, SHIM_CAP_MAX, SHIM_CAP_MAX_MOBILE,
    encodeTextRun, legacyCharCode, optionsFromInput, applyToInputValues, alignWords,
    toCodePoints, isMobile,
  };
})();

if (typeof globalThis !== 'undefined') globalThis.SCTextBuffer = SCTextBuffer;
