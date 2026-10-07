#!/usr/bin/env node
// SCFONT/1 MSDF atlas generator (Phase D, "Words on the Wall").
// Contract: .claude/caption-pipeline/phase-D-design.md §1.1-1.3.
//
// Usage:  node tools/fonts/gen_msdf_atlas.mjs [--fetch-tool] [--outfit] [--force]
//   --fetch-tool  download msdf-atlas-gen 1.3 win64 into tools/fonts/.bin/
//                 (also done automatically when MSDF_ATLAS_GEN is unset and
//                 no exe is found in .bin/)
//   --outfit      also generate outfit-400 / outfit-700 (variable font, needs
//                 msdf-atlas-gen -varfont support)
//   --force       regenerate even when the outputs already exist
//
// Outputs (checked in): assets/fonts/{fonts.json, <id>.json, <id>.msdf.png,
// <id>.metrics.png, src/<ttf>, src/OFL-*.txt}. Zero npm deps, node >= 18.

import { execFileSync, spawnSync } from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { encodeGlyphTexels, encodePng, TEX_W, TEX_H } from './metrics-codec.mjs';

const HERE = path.dirname(fileURLToPath(import.meta.url));
const REPO = path.resolve(HERE, '..', '..');
const BIN = path.join(HERE, '.bin');
const OUT = path.join(REPO, 'assets', 'fonts');
const SRC = path.join(OUT, 'src');
const CHARSET = path.join(HERE, 'charset.txt');

const args = process.argv.slice(2);
const FETCH_TOOL = args.includes('--fetch-tool');
const WANT_OUTFIT = args.includes('--outfit');
const FORCE = args.includes('--force');

const TOOL_ZIP_URL = 'https://github.com/Chlumsky/msdf-atlas-gen/releases/download/v1.3/msdf-atlas-gen-1.3-win64.zip';
const INTER_ZIP_URL = 'https://github.com/rsms/inter/releases/download/v4.1/Inter-4.1.zip';
const GF_RAW = 'https://raw.githubusercontent.com/google/fonts/main';

const FACES = [
  { id: 'inter-400', family: 'Inter', weight: 400, style: 'normal', ttf: 'Inter-Regular.ttf' },
  { id: 'inter-700', family: 'Inter', weight: 700, style: 'normal', ttf: 'Inter-Bold.ttf' },
  { id: 'barlow-semicondensed-600', family: 'Barlow Semi Condensed', weight: 600, style: 'normal', ttf: 'BarlowSemiCondensed-SemiBold.ttf' },
];
if (WANT_OUTFIT) {
  FACES.push(
    { id: 'outfit-400', family: 'Outfit', weight: 400, style: 'normal', ttf: 'Outfit[wght].ttf', varwght: 400 },
    { id: 'outfit-700', family: 'Outfit', weight: 700, style: 'normal', ttf: 'Outfit[wght].ttf', varwght: 700 },
  );
}

fs.mkdirSync(BIN, { recursive: true });
fs.mkdirSync(SRC, { recursive: true });

async function download(url, dest) {
  console.log('  downloading', url);
  const res = await fetch(url, { redirect: 'follow' });
  if (!res.ok) throw new Error('HTTP ' + res.status + ' for ' + url);
  fs.writeFileSync(dest, Buffer.from(await res.arrayBuffer()));
  return dest;
}

function findExe(dir, name) {
  if (!fs.existsSync(dir)) return null;
  for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, e.name);
    if (e.isDirectory()) { const r = findExe(p, name); if (r) return r; }
    else if (e.name.toLowerCase() === name) return p;
  }
  return null;
}

// Windows 10+ ships bsdtar (System32\tar.exe) which reads zips; a GNU tar
// earlier on PATH (Git Bash) does not, so resolve the system one explicitly.
const TAR = process.platform === 'win32'
  ? path.join(process.env.SystemRoot || 'C:\\Windows', 'System32', 'tar.exe')
  : 'tar';

function unzip(zipPath, destDir, entries) {
  // bsdtar handles zip; fall back to PowerShell Expand-Archive.
  fs.mkdirSync(destDir, { recursive: true });
  const tarArgs = ['-xf', zipPath, '-C', destDir, ...(entries || [])];
  let r = spawnSync(TAR, tarArgs, { stdio: 'pipe' });
  if (r.status !== 0) {
    r = spawnSync('powershell.exe', ['-NoProfile', '-Command',
      `Expand-Archive -LiteralPath '${zipPath}' -DestinationPath '${destDir}' -Force`], { stdio: 'pipe' });
    if (r.status !== 0) throw new Error('could not unzip ' + zipPath + ': ' + (r.stderr || '').toString());
  }
}

function zipList(zipPath) {
  const r = spawnSync(TAR, ['-tf', zipPath], { stdio: 'pipe' });
  if (r.status !== 0) return null;
  return r.stdout.toString().split(/\r?\n/).filter(Boolean);
}

async function ensureTool() {
  const envTool = process.env.MSDF_ATLAS_GEN;
  if (envTool && fs.existsSync(envTool)) return envTool;
  let exe = findExe(BIN, 'msdf-atlas-gen.exe');
  if (exe) return exe;
  if (!FETCH_TOOL && envTool) throw new Error('MSDF_ATLAS_GEN points at a missing file: ' + envTool);
  // auto-fetch (design §1.1): --fetch-tool passed, or env unset and exe absent
  const zip = path.join(BIN, 'msdf-atlas-gen-1.3-win64.zip');
  if (!fs.existsSync(zip)) await download(TOOL_ZIP_URL, zip);
  unzip(zip, BIN);
  exe = findExe(BIN, 'msdf-atlas-gen.exe');
  if (!exe) throw new Error('msdf-atlas-gen.exe not found after unzip');
  return exe;
}

async function ensureFonts() {
  const need = FACES.filter((f) => !fs.existsSync(path.join(SRC, f.ttf)));
  const needInter = need.some((f) => f.id.startsWith('inter-'));
  if (needInter) {
    const zip = path.join(BIN, 'Inter-4.1.zip');
    if (!fs.existsSync(zip)) await download(INTER_ZIP_URL, zip);
    const entries = zipList(zip) || [];
    const wanted = ['Inter-Regular.ttf', 'Inter-Bold.ttf'];
    const extractDir = path.join(BIN, 'inter');
    const picks = [];
    for (const w of wanted) {
      const hit = entries.find((e) => e.endsWith('/' + w) && e.includes('ttf')) ||
                  entries.find((e) => e.endsWith('/' + w)) ||
                  entries.find((e) => e === w);
      if (!hit) throw new Error(w + ' not found in Inter zip (entries: ' + entries.slice(0, 20).join(', ') + '…)');
      picks.push([w, hit]);
    }
    const lic = entries.find((e) => /(^|\/)LICENSE\.txt$/.test(e));
    unzip(zip, extractDir, picks.map(([, hit]) => hit).concat(lic ? [lic] : []));
    for (const [w, hit] of picks) fs.copyFileSync(path.join(extractDir, hit), path.join(SRC, w));
    if (lic) fs.copyFileSync(path.join(extractDir, lic), path.join(SRC, 'OFL-Inter.txt'));
  }
  if (need.some((f) => f.id.startsWith('barlow-'))) {
    await download(GF_RAW + '/ofl/barlowsemicondensed/BarlowSemiCondensed-SemiBold.ttf',
      path.join(SRC, 'BarlowSemiCondensed-SemiBold.ttf'));
    await download(GF_RAW + '/ofl/barlowsemicondensed/OFL.txt', path.join(SRC, 'OFL-BarlowSemiCondensed.txt'));
  }
  if (need.some((f) => f.id.startsWith('outfit-'))) {
    await download(GF_RAW + '/ofl/outfit/Outfit%5Bwght%5D.ttf', path.join(SRC, 'Outfit[wght].ttf'));
    await download(GF_RAW + '/ofl/outfit/OFL.txt', path.join(SRC, 'OFL-Outfit.txt'));
  }
}

function runAtlasGen(exe, face, size) {
  const ttf = path.join(SRC, face.ttf);
  const tmpJson = path.join(BIN, face.id + '.tmp.json');
  const png = path.join(OUT, face.id + '.msdf.png');
  const fontArgs = face.varwght != null
    ? ['-varfont', ttf + '?wght=' + face.varwght]
    : ['-font', ttf];
  const argv = [...fontArgs,
    '-charset', CHARSET, '-type', 'msdf', '-format', 'png', '-yorigin', 'bottom',
    '-size', String(size), '-pxrange', '6', '-potr',
    '-imageout', png, '-json', tmpJson];
  execFileSync(exe, argv, { stdio: 'pipe' });
  const json = JSON.parse(fs.readFileSync(tmpJson, 'utf8'));
  return json;
}

function writeFace(face, json, generator) {
  json.atlas.type = json.atlas.type || 'msdf';
  // Guarantee U+FFFD: alias to '?' when the font has no replacement glyph.
  if (!json.glyphs.some((g) => g.unicode === 0xFFFD)) {
    const q = json.glyphs.find((g) => g.unicode === 63);
    if (q) json.glyphs.push({ ...q, unicode: 0xFFFD });
  }
  const sctext = {
    version: 1, id: face.id, family: face.family, weight: face.weight, style: face.style,
    atlas: face.id + '.msdf.png', metricsTexture: face.id + '.metrics.png',
    metricsLayout: { width: TEX_W, height: TEX_H, texelsPerGlyph: 8, fixedPointDenominator: 4096 },
    charset: 'tools/fonts/charset.txt', generator,
  };
  const out = { sctext, ...json };
  fs.writeFileSync(path.join(OUT, face.id + '.json'), JSON.stringify(out, null, 1) + '\n');
  fs.writeFileSync(path.join(OUT, face.id + '.metrics.png'), encodePng(encodeGlyphTexels(out), TEX_W, TEX_H));
  return out;
}

function writeIndex(builtIds) {
  const has = (id) => builtIds.includes(id);
  const interFaces = ['inter-400', 'inter-700'].filter(has);
  const outfitFaces = ['outfit-400', 'outfit-700'].filter(has);
  const index = {
    version: 1,
    default: 'inter-400',
    caption: 'barlow-semicondensed-600',
    faces: builtIds,
    families: [
      { index: 0, label: 'Inter', faces: interFaces },
      { index: 1, label: 'Times New Roman', faces: interFaces },
      { index: 2, label: 'Libre Caslon', faces: interFaces },
      { index: 3, label: 'Outfit', faces: outfitFaces.length ? outfitFaces : interFaces },
      { index: 4, label: 'Caption', faces: ['barlow-semicondensed-600'].filter(has) },
    ],
  };
  fs.writeFileSync(path.join(OUT, 'fonts.json'), JSON.stringify(index, null, 1) + '\n');
  return index;
}

// Fallback A (design §1.1): npx msdf-bmfont-xml -t msdf -f json + BMFont->SCFONT.
function runBmfontFallback(face, size) {
  const ttf = path.join(SRC, face.ttf);
  const outBase = path.join(BIN, face.id + '.bmf');
  const r = spawnSync('npx', ['--yes', 'msdf-bmfont-xml', '-t', 'msdf', '-f', 'json',
    '-s', String(size), '-r', '6', '-o', outBase + '.png', ttf],
    { stdio: 'pipe', shell: process.platform === 'win32' });
  if (r.status !== 0) throw new Error('msdf-bmfont-xml failed: ' + (r.stderr || '').toString().slice(0, 400));
  const bmf = JSON.parse(fs.readFileSync(ttf.replace(/\.ttf$/i, '.json'), 'utf8'));
  const em = bmf.info.size;
  const scaleH = bmf.common.scaleH, scaleW = bmf.common.scaleW;
  const base = bmf.common.base;
  const glyphs = (bmf.chars || []).map((c) => {
    const g = { unicode: c.id, advance: c.xadvance / em };
    if (c.width > 0 && c.height > 0) {
      // BMFont: x,y top-left in a top-down image; convert to y-up atlas px + em plane
      g.atlasBounds = { left: c.x, right: c.x + c.width, top: scaleH - c.y, bottom: scaleH - (c.y + c.height) };
      const planeL = c.xoffset / em, planeT = (base - c.yoffset) / em;
      g.planeBounds = { left: planeL, right: planeL + c.width / em, top: planeT, bottom: planeT - c.height / em };
    }
    return g;
  });
  // msdf-bmfont-xml writes the PNG top-down; our contract only needs the JSON
  // bounds to match the PNG under upright upload, which the flip above gives.
  fs.copyFileSync(outBase + '.png', path.join(OUT, face.id + '.msdf.png'));
  return {
    atlas: { type: 'msdf', distanceRange: 6, size: em, width: scaleW, height: scaleH, yOrigin: 'bottom' },
    metrics: {
      emSize: 1, lineHeight: bmf.common.lineHeight / em,
      ascender: base / em, descender: (base - bmf.common.lineHeight) / em,
      underlineY: -0.1, underlineThickness: 0.05,
    },
    glyphs,
    kerning: (bmf.kernings || []).map((k) => ({ unicode1: k.first, unicode2: k.second, advance: k.amount / em })),
  };
}

async function main() {
  console.log('SCFONT/1 generator — output', OUT);
  await ensureFonts();

  let exe = null, toolErr = null;
  try { exe = await ensureTool(); } catch (e) { toolErr = e; }
  if (exe) console.log('  tool:', exe);
  else console.log('  msdf-atlas-gen unavailable (' + toolErr.message + '); will try npx msdf-bmfont-xml');

  const built = [];
  const summary = [];
  for (const face of FACES) {
    const jsonPath = path.join(OUT, face.id + '.json');
    if (!FORCE && fs.existsSync(jsonPath) && fs.existsSync(path.join(OUT, face.id + '.msdf.png')) &&
        fs.existsSync(path.join(OUT, face.id + '.metrics.png'))) {
      console.log('  ' + face.id + ': exists, skipping (use --force to regenerate)');
      built.push(face.id);
      const j = JSON.parse(fs.readFileSync(jsonPath, 'utf8'));
      summary.push([face.id, j.glyphs.length, j.atlas.width + 'x' + j.atlas.height, '(cached)']);
      continue;
    }
    let json = null, generator = null;
    if (exe) {
      json = runAtlasGen(exe, face, 48);
      if (json.atlas.width > 1024 || json.atlas.height > 1024) json = runAtlasGen(exe, face, 40);
      generator = 'msdf-atlas-gen 1.3';
    } else {
      json = runBmfontFallback(face, 48);
      generator = 'msdf-bmfont-xml (fallback)';
    }
    if (json.atlas.width > 2048 || json.atlas.height > 2048) {
      throw new Error(face.id + ': atlas ' + json.atlas.width + 'x' + json.atlas.height + ' exceeds 2048');
    }
    const out = writeFace(face, json, generator);
    built.push(face.id);
    const missing = out.glyphs.filter((g) => !g.atlasBounds && g.unicode !== 32).length;
    summary.push([face.id, out.glyphs.length, out.atlas.width + 'x' + out.atlas.height, missing + ' boundless']);
  }
  writeIndex(built);
  console.log('\n  face                        glyphs  atlas       notes');
  for (const [id, n, dims, note] of summary) {
    console.log('  ' + id.padEnd(28) + String(n).padEnd(8) + dims.padEnd(12) + note);
  }
  console.log('\n  wrote ' + path.join(OUT, 'fonts.json') + ' (faces: ' + built.join(', ') + ')');
}

main().catch((e) => {
  console.error('FAILED:', e.message);
  // Last resort (design §1.1 fallback B): leave fonts.json only so consumers
  // exercise their synthesized-grid fallback.
  try {
    if (!fs.existsSync(path.join(OUT, 'fonts.json'))) writeIndex([]);
  } catch {}
  process.exit(1);
});
