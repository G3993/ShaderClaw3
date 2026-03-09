// ShaderClaw Shader Audit
// Run: node test_shaders.js
// Catches ISF parsing issues, missing files, main() signature mismatches,
// and other common problems that cause shaders to silently fail at runtime.

import { readFileSync, existsSync } from 'fs';

const manifest = JSON.parse(readFileSync('shaders/manifest.json', 'utf8'));

// Must match the regex in buildFragmentShader (index.html)
const mainRe = /void\s+main\s*\(\s*(void)?\s*\)/;

let pass = 0;
let fail = 0;
const failures = [];

function ok(shader, msg) { pass++; }
function bad(shader, msg) { fail++; failures.push(`  ${shader}: ${msg}`); }

for (const entry of manifest) {
  const f = 'shaders/' + entry.file;
  const name = entry.file;

  // Skip scene files
  if (name.endsWith('.scene.js')) { ok(name, 'scene skip'); continue; }

  // 1. File exists
  if (!existsSync(f)) { bad(name, 'FILE MISSING'); continue; }

  const src = readFileSync(f, 'utf8');

  // 2. ISF JSON block present and parseable
  const match = src.match(/\/\*\s*(\{[\s\S]*?\})\s*\*\//);
  if (!match) { bad(name, 'no ISF comment block'); continue; }

  let meta;
  try { meta = JSON.parse(match[1]); }
  catch (e) { bad(name, 'ISF JSON parse error: ' + e.message); continue; }

  // 3. GLSL body non-empty
  const glsl = src.slice(src.indexOf(match[0]) + match[0].length).trim();
  const cleaned = glsl.replace(/#version\s+\d+.*/g, '');
  if (cleaned.length < 10) { bad(name, 'GLSL body too short (' + cleaned.length + ' chars)'); continue; }

  // 4. main() function present and matches wrapper regex
  if (!mainRe.test(cleaned)) {
    bad(name, 'no void main() or void main(void) — shader will fail to render');
    continue;
  }

  // 5. Inputs have required fields
  let inputsOk = true;
  for (const inp of meta.INPUTS || []) {
    if (!inp.NAME) { bad(name, 'input missing NAME: ' + JSON.stringify(inp)); inputsOk = false; }
    if (!inp.TYPE) { bad(name, 'input missing TYPE: ' + (inp.NAME || '??')); inputsOk = false; }
    const validTypes = ['float', 'color', 'bool', 'point2D', 'image', 'long', 'text'];
    if (inp.TYPE && !validTypes.includes(inp.TYPE)) {
      bad(name, `unknown input type "${inp.TYPE}" for ${inp.NAME}`);
      inputsOk = false;
    }
  }

  // 6. Check for duplicate uniform declarations that would conflict with header
  const headerUniforms = ['TIME', 'RENDERSIZE', 'PASSINDEX', 'FRAMEINDEX', 'mousePos',
    'mouseDelta', 'mouseDown', 'pinchHold', 'audioFFT', 'audioLevel', 'audioBass',
    'audioMid', 'audioHigh', 'varFontTex', 'fontAtlasTex', 'useFontAtlas',
    '_voiceGlitch', 'mpHandLandmarks', 'mpFaceLandmarks', 'mpPoseLandmarks',
    'mpSegMask', 'mpHandCount', 'mpHandPos', 'mpHandPos2', '_transparentBg'];
  for (const inp of meta.INPUTS || []) {
    if (headerUniforms.includes(inp.NAME)) {
      bad(name, `input "${inp.NAME}" conflicts with built-in header uniform`);
      inputsOk = false;
    }
  }

  // 7. Check manifest entry has required fields
  if (!entry.title) bad(name, 'manifest entry missing title');
  if (!entry.file) bad(name, 'manifest entry missing file');

  if (inputsOk) ok(name, 'all checks passed');
}

// Summary
console.log(`\nShaderClaw Shader Audit`);
console.log(`${'='.repeat(40)}`);
console.log(`${pass} passed, ${fail} failed (${manifest.length} total)`);
if (failures.length > 0) {
  console.log(`\nFailures:`);
  failures.forEach(f => console.log(f));
  process.exit(1);
}
