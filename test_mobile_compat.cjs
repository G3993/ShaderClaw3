#!/usr/bin/env node
// =============================================================
// Mobile GLSL ES 1.0 Compatibility Checker for ShaderClaw 3
// =============================================================
// Reads each shader from manifest.json, builds it via isf.js's
// buildFragmentShader(), then checks for patterns that break on
// mobile WebGL (GLSL ES 1.0) implementations.
// =============================================================

const fs = require('fs');
const path = require('path');

// --- Load isf.js by eval (it defines globals) ---
// We need to replace const/let with var so eval hoists them to this scope
const isfPath = path.join(__dirname, 'js', 'isf.js');
let isfSource = fs.readFileSync(isfPath, 'utf-8');
isfSource = isfSource.replace(/^const /gm, 'var ').replace(/^let /gm, 'var ');
eval(isfSource);
// Now we have: parseISF, buildFragmentShader, VERT_SHADER, etc.

// --- Load manifest ---
const manifest = JSON.parse(fs.readFileSync(path.join(__dirname, 'shaders', 'manifest.json'), 'utf-8'));

// --- Check functions ---
function countPattern(src, regex) {
  const m = src.match(regex);
  return m ? m.length : 0;
}

function checkShader(name, src) {
  const issues = [];
  const warnings = [];
  const lines = src.split('\n');

  // 1. Count sampler2D declarations
  const samplers = countPattern(src, /uniform\s+sampler2D\s+\w+/g);
  if (samplers > 8) issues.push(`TOO MANY SAMPLERS: ${samplers} sampler2D (mobile limit ~8)`);
  else if (samplers > 6) warnings.push(`${samplers} sampler2D (close to 8 limit)`);

  // 2. Count uniform vectors (rough: each uniform float = 1 component, vec4 = 1 vector)
  const uniformFloats = countPattern(src, /uniform\s+float\s+\w+/g);
  const uniformVec2 = countPattern(src, /uniform\s+vec2\s+\w+/g);
  const uniformVec3 = countPattern(src, /uniform\s+vec3\s+\w+/g);
  const uniformVec4 = countPattern(src, /uniform\s+vec4\s+\w+/g);
  const uniformInt = countPattern(src, /uniform\s+int\s+\w+/g);
  const uniformBool = countPattern(src, /uniform\s+bool\s+\w+/g);
  const uniformSampler = samplers;
  // Each sampler uses 1 uniform location; floats pack 4 per vector
  const uniformVectors = Math.ceil(uniformFloats / 4) + uniformVec2 + uniformVec3 + uniformVec4 +
                          Math.ceil(uniformInt / 4) + Math.ceil(uniformBool / 4) + uniformSampler;
  if (uniformVectors > 128) issues.push(`TOO MANY UNIFORMS: ~${uniformVectors} vectors (mobile limit 128)`);
  else if (uniformVectors > 64) warnings.push(`~${uniformVectors} uniform vectors (mobile min guarantee is 16-128)`);

  // 3. Loop iteration count > 256
  const forLoops = src.matchAll(/for\s*\(\s*\w+\s+\w+\s*=\s*\d+\s*;\s*\w+\s*[<>=!]+\s*(\d+)\s*;/g);
  for (const m of forLoops) {
    const limit = parseInt(m[1]);
    if (limit > 256) issues.push(`LOOP TOO LARGE: for loop with limit ${limit} (>256)`);
    else if (limit > 128) warnings.push(`for loop with limit ${limit} (may be slow on mobile)`);
  }

  // 4. highp without fallback
  const hasHighp = /\bhighp\b/.test(src);
  const hasHighpFallback = /#ifdef\s+GL_FRAGMENT_PRECISION_HIGH/.test(src) ||
                            /precision\s+mediump\s+float/.test(src);
  if (hasHighp && !hasHighpFallback) {
    warnings.push('Uses highp without #ifdef GL_FRAGMENT_PRECISION_HIGH fallback');
  }

  // 5. Missing precision declarations
  const hasPrecision = /precision\s+(highp|mediump|lowp)\s+float\s*;/.test(src);
  if (!hasPrecision) issues.push('MISSING precision declaration for float');

  // 6. gl_FragColor read-after-write across function calls (heuristic)
  // Look for functions that read gl_FragColor (not in main)
  const funcBodies = src.matchAll(/(\w+)\s*\([^)]*\)\s*\{([\s\S]*?)\n\}/g);
  for (const m of funcBodies) {
    const fname = m[1];
    const fbody = m[2];
    if (fname !== 'main' && fname !== '_shaderMain') {
      if (/gl_FragColor/.test(fbody) && /gl_FragColor\s*[^=]/.test(fbody)) {
        // Check for reading gl_FragColor (not just assigning)
        const reads = fbody.match(/[^=!<>]\s*gl_FragColor\b/g);
        const writes = fbody.match(/gl_FragColor\s*[+\-*\/]?=/g);
        if (reads && reads.length > 0) {
          warnings.push(`gl_FragColor read in function ${fname}() (may cause issues on some mobile GPUs)`);
        }
      }
    }
  }

  // 7. Non-constant array indexing
  // Look for array[variable] where variable is not a constant
  const arrayAccesses = src.matchAll(/(\w+)\s*\[\s*([a-zA-Z_]\w*)\s*\]/g);
  for (const m of arrayAccesses) {
    const arrName = m[1];
    const indexVar = m[2];
    // Skip known safe patterns: texture lookups, macro args, common constants
    if (['texture2D', 'IMG_NORM_PIXEL', 'IMG_PIXEL', 'int', 'float', 'vec2', 'vec3', 'vec4',
         'mat2', 'mat3', 'mat4', 'sampler2D'].includes(arrName)) continue;
    // Skip if index looks like a constant or loop variable
    if (/^[A-Z_]+$/.test(indexVar)) continue; // ALL_CAPS = likely a define
    // This is a heuristic — flag it as a warning
    if (!['i', 'j', 'k', 'n', 'x', 'y', 'z'].includes(indexVar)) {
      warnings.push(`Non-constant array index: ${arrName}[${indexVar}] (may fail on some mobile)`);
    }
  }

  // 8. #version directives (should be stripped by buildFragmentShader)
  if (/#version\s+\d+/.test(src)) {
    issues.push('#version directive found (should be stripped for WebGL)');
  }

  // 9. Multiple void main() definitions
  const mainCount = countPattern(src, /void\s+main\s*\(\s*(void)?\s*\)/g);
  if (mainCount > 1) issues.push(`MULTIPLE main() definitions: ${mainCount} found (wrapper issue)`);
  if (mainCount === 0) issues.push('NO main() definition found');

  // 10. Lines longer than 1024 chars
  let longLines = 0;
  for (let i = 0; i < lines.length; i++) {
    if (lines[i].length > 1024) {
      longLines++;
      if (longLines <= 2) warnings.push(`Line ${i + 1} is ${lines[i].length} chars (>1024)`);
    }
  }
  if (longLines > 2) warnings.push(`...and ${longLines - 2} more long lines`);

  // 11. Additional: dFdx/dFdy without extension
  if (/\b(dFdx|dFdy|fwidth)\b/.test(src) && !/#extension\s+GL_OES_standard_derivatives/.test(src)) {
    warnings.push('Uses dFdx/dFdy/fwidth without #extension GL_OES_standard_derivatives');
  }

  // 12. texture2DLod in fragment shader (not available without extension)
  if (/\btexture2DLod\b/.test(src)) {
    warnings.push('Uses texture2DLod (not available in fragment shader without extension)');
  }

  return {
    lineCount: lines.length,
    samplers,
    uniformVectors,
    issues,
    warnings
  };
}

// --- Vertex shader check ---
function checkVertexShader() {
  const src = VERT_SHADER;
  const issues = [];
  const warnings = [];
  const lines = src.split('\n');

  if (!src.includes('gl_Position')) warnings.push('No gl_Position assignment found');
  if (/#version/.test(src)) issues.push('#version directive in vertex shader');

  const mainCount = countPattern(src, /void\s+main\s*\(\s*(void)?\s*\)/g);
  if (mainCount !== 1) issues.push(`Expected 1 main(), found ${mainCount}`);

  // Check for precision (vertex shaders have highp by default in ES, but explicit is better)
  // Actually vertex shaders default to highp in GLSL ES 1.0, so this is fine

  return { lineCount: lines.length, issues, warnings };
}

// ============================================================
// Run
// ============================================================
console.log('='.repeat(70));
console.log('  ShaderClaw 3 — Mobile GLSL ES 1.0 Compatibility Report');
console.log('='.repeat(70));
console.log('');

// Filter: non-scene, non-hidden shaders with .fs files
const shaderEntries = manifest.filter(e => {
  if (e.hidden) return false;
  if (e.type === 'scene') return false;
  if (e.folder === 'scenes') return false;
  if (!e.file || !e.file.endsWith('.fs')) return false;
  return true;
});

console.log(`Testing ${shaderEntries.length} shaders from manifest...\n`);

let totalPass = 0;
let totalWarn = 0;
let totalFail = 0;
const failedShaders = [];

for (const entry of shaderEntries) {
  const filePath = path.join(__dirname, 'shaders', entry.file);
  if (!fs.existsSync(filePath)) {
    console.log(`  SKIP  ${entry.title} (${entry.file}) — file not found`);
    continue;
  }

  const source = fs.readFileSync(filePath, 'utf-8');
  let built;
  try {
    built = buildFragmentShader(source);
  } catch (e) {
    console.log(`  FAIL  ${entry.title} (${entry.file}) — buildFragmentShader threw: ${e.message}`);
    totalFail++;
    failedShaders.push({ title: entry.title, file: entry.file, error: e.message });
    continue;
  }

  const result = checkShader(entry.title, built.frag);
  const status = result.issues.length > 0 ? 'FAIL' : (result.warnings.length > 0 ? 'WARN' : 'PASS');

  if (status === 'PASS') totalPass++;
  else if (status === 'WARN') totalWarn++;
  else { totalFail++; failedShaders.push({ title: entry.title, file: entry.file, issues: result.issues }); }

  const tag = status === 'PASS' ? '  PASS' : status === 'WARN' ? '  WARN' : '  FAIL';
  console.log(`${tag}  ${entry.title.padEnd(20)} ${entry.file.padEnd(30)} ${result.lineCount} lines | ${result.samplers} samplers | ~${result.uniformVectors} uniform vecs`);

  for (const issue of result.issues) {
    console.log(`        ERROR: ${issue}`);
  }
  for (const w of result.warnings) {
    console.log(`        warn:  ${w}`);
  }
}

// --- Vertex shader ---
console.log('\n' + '-'.repeat(70));
console.log('Vertex Shader (VERT_SHADER from isf.js)');
console.log('-'.repeat(70));
const vResult = checkVertexShader();
const vStatus = vResult.issues.length > 0 ? 'FAIL' : 'PASS';
console.log(`  ${vStatus}  ${vResult.lineCount} lines`);
for (const i of vResult.issues) console.log(`        ERROR: ${i}`);
for (const w of vResult.warnings) console.log(`        warn:  ${w}`);

// --- Summary ---
console.log('\n' + '='.repeat(70));
console.log('  SUMMARY');
console.log('='.repeat(70));
console.log(`  PASS: ${totalPass}`);
console.log(`  WARN: ${totalWarn}`);
console.log(`  FAIL: ${totalFail}`);
console.log(`  Total: ${shaderEntries.length}`);

if (failedShaders.length > 0) {
  console.log('\n  Failed shaders:');
  for (const f of failedShaders) {
    console.log(`    - ${f.title} (${f.file})`);
    if (f.error) console.log(`      Build error: ${f.error}`);
    if (f.issues) for (const i of f.issues) console.log(`      ${i}`);
  }
}

console.log('');
process.exit(totalFail > 0 ? 1 : 0);
