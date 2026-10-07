// SCTEXT/1 encoder tests — run: node --test tests/text-buffer.test.mjs
import { test } from 'node:test';
import assert from 'node:assert/strict';
import '../js/text-buffer.js';

const TB = globalThis.SCTextBuffer;

// The five legacy charToCode copies (app.js/layers.js/renderer.js/params.js) all did this:
function oldCharToCode(ch) {
  if (!ch || ch === ' ') return 26;
  const code = ch.toUpperCase().charCodeAt(0);
  if (code >= 65 && code <= 90) return code - 65;
  if (code >= 48 && code <= 57) return code - 48 + 27;
  return 26;
}

function glyph(enc, i) {
  const o = i * 8;
  const b = enc.bytes;
  return {
    cp: b[o] | (b[o + 1] << 8), word: b[o + 2], a0: b[o + 3],
    reveal: b[o + 4] | (b[o + 5] << 8), speaker: b[o + 6], a1: b[o + 7],
  };
}

test('module exposes the SCTEXT/1 API on globalThis', () => {
  assert.ok(TB, 'globalThis.SCTextBuffer');
  for (const fn of ['encodeTextRun', 'legacyCharCode', 'optionsFromInput', 'applyToInputValues', 'alignWords']) {
    assert.equal(typeof TB[fn], 'function', fn);
  }
});

test('ASCII encode: cp lo/hi, len, buffer size, padding zero-filled', () => {
  const enc = TB.encodeTextRun({ text: 'Hi 42' }, { cap: 8, shimCap: 8 });
  assert.equal(enc.len, 5);
  assert.equal(enc.bytes.length, 2 * 8 * 4);
  assert.equal(glyph(enc, 0).cp, 'H'.codePointAt(0));
  assert.equal(glyph(enc, 1).cp, 'i'.codePointAt(0));
  assert.equal(glyph(enc, 2).cp, 32);
  assert.equal(glyph(enc, 3).cp, 52);
  assert.equal(glyph(enc, 4).cp, 50);
  for (let i = 5; i < 8; i++) {
    const g = glyph(enc, i);
    assert.equal(g.cp, 0); assert.equal(g.word, 0); assert.equal(g.reveal, 0); assert.equal(g.speaker, 0);
  }
});

test('alpha is 255 on every texel (glyph + timing, filled + padding)', () => {
  const enc = TB.encodeTextRun({ text: 'abc' }, { cap: 6, shimCap: 6 });
  for (let t = 0; t < 12; t++) assert.equal(enc.bytes[t * 4 + 3], 255, 'texel ' + t);
});

test('UTF-8: BMP code points kept (hi byte), astral -> U+FFFD', () => {
  const enc = TB.encodeTextRun({ text: 'é€😀' }, { cap: 4 });
  assert.equal(glyph(enc, 0).cp, 0xE9);
  assert.equal(glyph(enc, 1).cp, 0x20AC);
  assert.equal(glyph(enc, 2).cp, 0xFFFD);
  assert.equal(enc.len, 3);
});

test('CASE upper / lower / preserve (default)', () => {
  assert.equal(glyph(TB.encodeTextRun('Ab', { cap: 2, caseMode: 'upper' }), 1).cp, 66);
  assert.equal(glyph(TB.encodeTextRun('Ab', { cap: 2, caseMode: 'lower' }), 0).cp, 97);
  const p = TB.encodeTextRun('Ab', { cap: 2 });
  assert.equal(glyph(p, 0).cp, 65); assert.equal(glyph(p, 1).cp, 98);
});

test('OVERFLOW: tail (default) keeps the end, head keeps the start; shim window independent', () => {
  const tail = TB.encodeTextRun('abcdefgh', { cap: 3, shimCap: 2 });
  assert.equal(tail.len, 3);
  assert.equal(String.fromCharCode(glyph(tail, 0).cp, glyph(tail, 1).cp, glyph(tail, 2).cp), 'fgh');
  assert.deepEqual(tail.shimCodes, ['g', 'h'].map((c) => oldCharToCode(c)));
  assert.equal(tail.shimLen, 2);
  const head = TB.encodeTextRun('abcdefgh', { cap: 3, shimCap: 2, overflowHead: true });
  assert.equal(String.fromCharCode(glyph(head, 0).cp, glyph(head, 1).cp, glyph(head, 2).cp), 'abc');
  assert.deepEqual(head.shimCodes, ['a', 'b'].map((c) => oldCharToCode(c)));
});

test('shim codes match the legacy charToCode for A-Z, a-z, 0-9, space, punctuation', () => {
  const chars = [];
  for (let c = 32; c < 127; c++) chars.push(String.fromCharCode(c));
  const text = chars.join('');
  const enc = TB.encodeTextRun(text, { cap: 1024, shimCap: 64, overflowHead: true });
  for (let i = 0; i < 64; i++) {
    assert.equal(enc.shimCodes[i], oldCharToCode(text[i]), 'char ' + JSON.stringify(text[i]));
    assert.equal(TB.legacyCharCode(text.charCodeAt(i)), oldCharToCode(text[i]));
  }
  // lowercase folds like the old copies (ch.toUpperCase())
  for (const ch of 'azmq') assert.equal(TB.legacyCharCode(ch.charCodeAt(0)), oldCharToCode(ch));
  assert.equal(TB.legacyCharCode(0xE9), 26);
  // empty shim slots are 26 (space) like the old `!ch -> 26`
  const short = TB.encodeTextRun('AB', { cap: 4, shimCap: 4 });
  assert.deepEqual(short.shimCodes, [0, 1, 26, 26]);
  assert.equal(short.shimLen, 2);
});

test('wordIndex: 0-based per whitespace token, 255 on spaces, restarts in the kept window', () => {
  const enc = TB.encodeTextRun('ab  cd e', { cap: 16 });
  const w = [...Array(8)].map((_, i) => glyph(enc, i).word);
  assert.deepEqual(w, [0, 0, 255, 255, 1, 1, 255, 2]);
  const tail = TB.encodeTextRun('one two three', { cap: 5 });
  assert.deepEqual([...Array(5)].map((_, i) => glyph(tail, i).word), [0, 0, 0, 0, 0]);
});

test('sentinels: revealMs 65535 / speaker 255 when unknown; set from words[]', () => {
  const unknown = TB.encodeTextRun('hey', { cap: 3 });
  assert.equal(glyph(unknown, 0).reveal, 65535);
  assert.equal(glyph(unknown, 0).speaker, 255);
  const run = { text: 'hi there', words: [{ start: 0, end: 2, revealMs: 120, speakerId: 1 }, { start: 3, end: 8, revealMs: 700000 }], speakerId: 3 };
  const enc = TB.encodeTextRun(run, { cap: 8 });
  assert.equal(glyph(enc, 0).reveal, 120); assert.equal(glyph(enc, 0).speaker, 1);
  assert.equal(glyph(enc, 2).reveal, 65535); assert.equal(glyph(enc, 2).speaker, 3); // space: run speaker
  assert.equal(glyph(enc, 3).reveal, 65534); // clamped
  assert.equal(glyph(enc, 3).speaker, 3);
});

test('optionsFromInput: cap clamp, shimCap <= 64 (desktop), CASE/OVERFLOW parsing', () => {
  const o = TB.optionsFromInput({ NAME: 'msg', TYPE: 'text', MAX_LENGTH: 200, CASE: 'Upper', OVERFLOW: 'head' });
  assert.equal(o.cap, 200); assert.equal(o.shimCap, 64); assert.equal(o.caseMode, 'upper'); assert.equal(o.overflowHead, true);
  const d = TB.optionsFromInput({ NAME: 'msg', TYPE: 'text' });
  assert.equal(d.cap, 12); assert.equal(d.shimCap, 12); assert.equal(d.caseMode, 'preserve'); assert.equal(d.overflowHead, false);
  assert.equal(TB.optionsFromInput({ MAX_LENGTH: 5000 }).cap, 1024);
});

test('applyToInputValues: _len, shim msg_i only when _shim, __textrun_ record reused + dirty', () => {
  const shim = { NAME: 'msg', TYPE: 'text', MAX_LENGTH: 4, _shim: true };
  const iv = {};
  const rec = TB.applyToInputValues(iv, shim, 'Hello, wall 42!');
  assert.equal(iv.msg_len, 4);
  assert.deepEqual([iv.msg_0, iv.msg_1, iv.msg_2, iv.msg_3], [' ', '4', '2', '!'].map(oldCharToCode)); // tail
  assert.equal(iv.__textrun_msg, rec);
  assert.equal(rec.dirty, true); assert.equal(rec.cap, 4); assert.equal(rec.len, 4); assert.equal(rec.text, 'Hello, wall 42!');
  assert.ok(rec.bytes instanceof Uint8Array); assert.equal(rec.bytes.length, 32);
  rec.dirty = false; rec._glTexture = 'keep';
  const rec2 = TB.applyToInputValues(iv, shim, 'abc');
  assert.equal(rec2, rec, 'record object reused'); assert.equal(rec.dirty, true); assert.equal(rec._glTexture, 'keep');
  assert.equal(iv.msg_len, 3);

  const noShim = { NAME: 'msg', TYPE: 'text', MAX_LENGTH: 4, _shim: false };
  const iv2 = {};
  TB.applyToInputValues(iv2, noShim, 'abc');
  assert.equal(iv2.msg_len, 3);
  assert.equal('msg_0' in iv2, false);
  assert.ok(iv2.__textrun_msg);
});

test('alignWords: punctuation-tolerant, sequential, partial coverage', () => {
  const words = [{ w: 'Hello', s: 1200, e: 1500 }, { w: 'wall', s: 1700, e: 1900, speaker: 'speaker_2' }, { w: 'forty-two', s: 2100, e: 2400 }];
  const out = TB.alignWords('hello, WALL 42!', words, 1000);
  assert.equal(out.length, 3);
  assert.deepEqual(out[0], { start: 0, end: 6, revealMs: 200, speakerId: -1 });
  assert.deepEqual(out[1], { start: 7, end: 11, revealMs: 700, speakerId: 2 });
  assert.equal(out[2].revealMs, -1); // "42!" != "fortytwo"
  // seconds shape
  const sec = TB.alignWords('go now', [{ text: 'go', start: 5.0 }, { text: 'now', start: 5.5 }], 4000);
  assert.equal(sec[0].revealMs, 1000); assert.equal(sec[1].revealMs, 1500);
  // encode end-to-end with aligned words
  const enc = TB.encodeTextRun({ text: 'hello, WALL 42!', words: out }, { cap: 16 });
  assert.equal(glyph(enc, 0).reveal, 200); assert.equal(glyph(enc, 7).reveal, 700); assert.equal(glyph(enc, 7).speaker, 2);
  assert.equal(glyph(enc, 12).reveal, 65535);
});
