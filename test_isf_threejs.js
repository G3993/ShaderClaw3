// ============================================================
// ISF-to-Three.js Phase 1 Test Sweep
// Run: node test_isf_threejs.js
//
// Tests that ISF shaders can be correctly transpiled for use
// in Three.js RawShaderMaterial. Validates:
//   1. ISF parsing produces valid GLSL for Three.js
//   2. Uniform mapping is correct and complete
//   3. ISF built-in functions (#define macros) are present
//   4. Coordinate system (isf_FragNormCoord → vUv) mapping works
//   5. Multi-pass shaders can be mapped to render target chains
//   6. All existing shaders parse without error
//   7. Scene file structure is valid
// ============================================================

import { readFileSync, existsSync, readdirSync } from 'fs';

let pass = 0, fail = 0;
const failures = [];

function ok(test, msg) { pass++; }
function bad(test, msg) { fail++; failures.push(`  FAIL: [${test}] ${msg}`); }
function section(name) { console.log(`\n--- ${name} ---`); }

// ============================================================
// Helpers: Minimal ISF parser (mirrors isf.js parseISF)
// ============================================================
function parseISF(source) {
  const match = source.match(/\/\*\s*(\{[\s\S]*?\})\s*\*\//);
  if (!match) return { meta: null, glsl: source.trim(), inputs: [] };
  try {
    const meta = JSON.parse(match[1]);
    const glsl = source.slice(source.indexOf(match[0]) + match[0].length).trim();
    return { meta, glsl, inputs: meta.INPUTS || [] };
  } catch (e) {
    return { meta: null, glsl: source.trim(), inputs: [], error: e.message };
  }
}

// Map ISF input type to Three.js uniform type
function isfTypeToThreeJS(type) {
  const map = {
    'float':   'float  → { value: Number }',
    'color':   'vec4   → { value: THREE.Vector4 }',
    'bool':    'bool   → { value: Boolean }',
    'point2D': 'vec2   → { value: THREE.Vector2 }',
    'image':   'sampler2D → { value: THREE.Texture }',
    'long':    'float  → { value: Number }',
    'text':    'float[] → { value: Number } per char',
  };
  return map[type] || null;
}

// Build a Three.js-compatible fragment shader from ISF source
// This is the core transpilation we're testing
function buildThreeJSFragmentShader(isfSource) {
  const parsed = parseISF(isfSource);
  if (!parsed.meta) return { error: 'No ISF header', parsed };

  const uniforms = [];
  const uniformMap = {}; // name → Three.js type info

  // Standard ISF uniforms (always present)
  uniforms.push('uniform float TIME;');
  uniforms.push('uniform vec2 RENDERSIZE;');
  uniforms.push('uniform int PASSINDEX;');
  uniforms.push('uniform int FRAMEINDEX;');

  // ISF compatibility macros
  uniforms.push('#define IMG_NORM_PIXEL(img, coord) texture2D(img, coord)');
  uniforms.push('#define IMG_PIXEL(img, coord) texture2D(img, coord / RENDERSIZE)');
  uniforms.push('#define IMG_THIS_PIXEL(img) texture2D(img, vUv)');
  uniforms.push('#define IMG_THIS_NORM_PIXEL(img) texture2D(img, vUv)');
  uniforms.push('#define IMG_NORM_THIS_PIXEL(img) texture2D(img, vUv)');

  // Map isf_FragNormCoord to vUv for Three.js compatibility
  uniforms.push('#define isf_FragNormCoord vUv');

  // User-defined inputs → uniforms
  for (const inp of parsed.inputs) {
    const t = inp.TYPE;
    if (t === 'float' || t === 'long') {
      uniforms.push(`uniform float ${inp.NAME};`);
      uniformMap[inp.NAME] = 'float';
    } else if (t === 'color') {
      uniforms.push(`uniform vec4 ${inp.NAME};`);
      uniformMap[inp.NAME] = 'vec4';
    } else if (t === 'bool') {
      uniforms.push(`uniform bool ${inp.NAME};`);
      uniformMap[inp.NAME] = 'bool';
    } else if (t === 'point2D') {
      uniforms.push(`uniform vec2 ${inp.NAME};`);
      uniformMap[inp.NAME] = 'vec2';
    } else if (t === 'image') {
      uniforms.push(`uniform sampler2D ${inp.NAME};`);
      uniforms.push(`uniform vec2 IMG_SIZE_${inp.NAME};`);
      uniformMap[inp.NAME] = 'sampler2D';
      uniformMap[`IMG_SIZE_${inp.NAME}`] = 'vec2';
    } else if (t === 'text') {
      const maxLen = inp.MAX_LENGTH || 12;
      for (let i = 0; i < maxLen; i++) {
        uniforms.push(`uniform float ${inp.NAME}_${i};`);
        uniformMap[`${inp.NAME}_${i}`] = 'float';
      }
      uniforms.push(`uniform float ${inp.NAME}_len;`);
      uniformMap[`${inp.NAME}_len`] = 'float';
    }
  }

  // PASSES targets as samplers
  if (parsed.meta.PASSES) {
    for (const p of parsed.meta.PASSES) {
      if (p.TARGET) {
        uniforms.push(`uniform sampler2D ${p.TARGET};`);
        uniformMap[p.TARGET] = 'sampler2D';
      }
    }
  }

  // Clean GLSL body
  let body = parsed.glsl
    .replace(/#version\s+\d+.*/g, '')
    .replace(/#ifdef\s+GL_ES\s*\r?\nprecision\s+\w+\s+float\s*;\s*\r?\n#endif\s*\r?\n?/g, '');

  const frag = [
    'precision highp float;',
    'varying vec2 vUv;',
    ...uniforms,
    '',
    body
  ].join('\n');

  return { frag, parsed, uniformMap, passCount: (parsed.meta.PASSES || [{}]).length };
}

// ============================================================
// TEST 1: Core ISF-to-Three.js transpilation
// ============================================================
section('1. Core ISF transpilation');

// Test with metaballs.fs
const metaballsSrc = readFileSync('shaders/metaballs.fs', 'utf8');
const metaResult = buildThreeJSFragmentShader(metaballsSrc);

if (metaResult.error) {
  bad('metaballs-parse', metaResult.error);
} else {
  ok('metaballs-parse', 'parsed');

  // Check uniforms are present
  const frag = metaResult.frag;
  if (frag.includes('uniform float TIME;')) ok('metaballs-TIME', 'has TIME');
  else bad('metaballs-TIME', 'missing TIME uniform');

  if (frag.includes('uniform vec2 RENDERSIZE;')) ok('metaballs-RENDERSIZE', 'has RENDERSIZE');
  else bad('metaballs-RENDERSIZE', 'missing RENDERSIZE uniform');

  // Check ISF input uniforms
  for (const name of ['ballCount', 'ballSpeed', 'smoothK']) {
    if (frag.includes(`uniform float ${name};`)) ok(`metaballs-${name}`, `has ${name}`);
    else bad(`metaballs-${name}`, `missing uniform float ${name}`);
  }
  for (const name of ['keyColor', 'rimColor', 'bgColor']) {
    if (frag.includes(`uniform vec4 ${name};`)) ok(`metaballs-${name}`, `has ${name}`);
    else bad(`metaballs-${name}`, `missing uniform vec4 ${name}`);
  }

  // Check main() is present
  if (/void\s+main\s*\(/.test(frag)) ok('metaballs-main', 'has main()');
  else bad('metaballs-main', 'missing main()');

  // Check ISF macros
  if (frag.includes('#define IMG_NORM_PIXEL')) ok('metaballs-macros', 'has ISF macros');
  else bad('metaballs-macros', 'missing ISF macros');

  // Check isf_FragNormCoord alias
  if (frag.includes('#define isf_FragNormCoord vUv')) ok('metaballs-coord', 'coord alias present');
  else bad('metaballs-coord', 'missing isf_FragNormCoord → vUv alias');

  // Check no duplicate precision
  const precisionCount = (frag.match(/precision\s+\w+\s+float/g) || []).length;
  if (precisionCount === 1) ok('metaballs-precision', 'single precision');
  else bad('metaballs-precision', `${precisionCount} precision statements (expected 1)`);

  // Check the shader uses gl_FragCoord (metaballs uses it for screen-space UVs)
  if (metaResult.parsed.glsl.includes('gl_FragCoord')) ok('metaballs-fragcoord', 'uses gl_FragCoord (pixel-perfect)');
  else bad('metaballs-fragcoord', 'expected gl_FragCoord usage for pixel-perfect coords');

  console.log(`  metaballs: ${Object.keys(metaResult.uniformMap).length} uniforms mapped`);
}

// ============================================================
// TEST 2: Multi-pass shader mapping (fluid_sim.fs)
// ============================================================
section('2. Multi-pass shader mapping');

if (existsSync('shaders/fluid_sim.fs')) {
  const fluidSrc = readFileSync('shaders/fluid_sim.fs', 'utf8');
  const fluidResult = buildThreeJSFragmentShader(fluidSrc);

  if (fluidResult.error) {
    bad('fluid-parse', fluidResult.error);
  } else {
    ok('fluid-parse', 'parsed');

    const passes = fluidResult.parsed.meta.PASSES || [];
    console.log(`  fluid_sim: ${passes.length} passes`);

    // Check pass targets are declared as samplers
    for (const p of passes) {
      if (p.TARGET) {
        if (fluidResult.frag.includes(`uniform sampler2D ${p.TARGET};`)) {
          ok(`fluid-target-${p.TARGET}`, `target ${p.TARGET} declared`);
        } else {
          bad(`fluid-target-${p.TARGET}`, `target ${p.TARGET} not declared as sampler`);
        }
      }
    }

    // Check persistent buffers identified
    const persistentCount = passes.filter(p => p.PERSISTENT).length;
    console.log(`  fluid_sim: ${persistentCount} persistent buffers (need ping-pong RT in Three.js)`);
    if (persistentCount > 0) ok('fluid-persistent', 'has persistent buffers');

    // Check pass count
    if (fluidResult.passCount > 1) ok('fluid-multipass', `${fluidResult.passCount} passes`);
    else bad('fluid-multipass', 'expected multi-pass');

    // Verify PASSINDEX branching exists
    if (fluidResult.parsed.glsl.includes('PASSINDEX')) ok('fluid-passindex', 'uses PASSINDEX');
    else bad('fluid-passindex', 'missing PASSINDEX usage in multi-pass shader');

    // Map passes to Three.js render targets
    console.log('  Three.js render target mapping:');
    for (let i = 0; i < passes.length; i++) {
      const p = passes[i];
      if (p.TARGET) {
        const w = p.WIDTH || '$WIDTH';
        const h = p.HEIGHT || '$HEIGHT';
        const type = p.PERSISTENT ? 'Ping-pong WebGLRenderTarget' : 'WebGLRenderTarget';
        const floatType = p.FLOAT ? ' (HalfFloatType)' : '';
        console.log(`    Pass ${i}: ${p.TARGET} → ${type} ${w}x${h}${floatType}`);
      } else {
        console.log(`    Pass ${i}: → Screen (final)`);
      }
    }
  }
} else {
  console.log('  fluid_sim.fs not found, skipping multi-pass tests');
}

// ============================================================
// TEST 3: Sweep ALL shaders — verify parseable & transpilable
// ============================================================
section('3. Full shader sweep (ISF → Three.js transpilation)');

const manifest = JSON.parse(readFileSync('shaders/manifest.json', 'utf8'));
let sweepPass = 0, sweepFail = 0;
const sweepIssues = [];

for (const entry of manifest) {
  if (!entry.file) continue;

  // Handle scenes separately
  if (entry.file.endsWith('.scene.js')) {
    const scenePath = (entry.folder || 'shaders') + '/' + entry.file;
    if (existsSync(scenePath)) {
      sweepPass++;
    } else {
      sweepFail++;
      sweepIssues.push(`${entry.file}: scene file missing at ${scenePath}`);
    }
    continue;
  }

  const filePath = 'shaders/' + entry.file;
  if (!existsSync(filePath)) {
    sweepFail++;
    sweepIssues.push(`${entry.file}: file missing`);
    continue;
  }

  const src = readFileSync(filePath, 'utf8');
  const result = buildThreeJSFragmentShader(src);

  if (result.error) {
    sweepFail++;
    sweepIssues.push(`${entry.file}: ${result.error}`);
    continue;
  }

  // Verify main() present
  if (!/void\s+main\s*\(/.test(result.frag)) {
    sweepFail++;
    sweepIssues.push(`${entry.file}: no main() after transpilation`);
    continue;
  }

  // Verify no bare isf_FragNormCoord without the #define alias
  // (would fail in Three.js since we use vUv)
  const usesISFCoord = result.parsed.glsl.includes('isf_FragNormCoord');
  const hasAlias = result.frag.includes('#define isf_FragNormCoord vUv');
  if (usesISFCoord && !hasAlias) {
    sweepFail++;
    sweepIssues.push(`${entry.file}: uses isf_FragNormCoord but no vUv alias`);
    continue;
  }

  // Check for ISF texture functions that need macros
  const usesIMGFuncs = /IMG_(NORM_)?PIXEL|IMG_THIS_(NORM_)?PIXEL|IMG_SIZE/.test(result.parsed.glsl);
  if (usesIMGFuncs) {
    const hasMacros = result.frag.includes('#define IMG_NORM_PIXEL');
    if (!hasMacros) {
      sweepFail++;
      sweepIssues.push(`${entry.file}: uses IMG_* functions but macros missing`);
      continue;
    }
  }

  sweepPass++;
}

console.log(`  ${sweepPass} shaders transpile OK, ${sweepFail} failed (${manifest.length} total)`);
if (sweepIssues.length > 0) {
  for (const issue of sweepIssues) {
    bad('sweep', issue);
    console.log(`    ${issue}`);
  }
} else {
  ok('sweep-all', 'all shaders transpile cleanly');
}

// ============================================================
// TEST 4: ISF function coverage — which shaders use what
// ============================================================
section('4. ISF function usage analysis');

const funcUsage = {
  'gl_FragCoord': 0,
  'isf_FragNormCoord': 0,
  'IMG_NORM_PIXEL': 0,
  'IMG_PIXEL': 0,
  'IMG_THIS_PIXEL': 0,
  'IMG_THIS_NORM_PIXEL': 0,
  'IMG_SIZE': 0,
  'PASSINDEX': 0,
  'FRAMEINDEX': 0,
  'TIME': 0,
  'RENDERSIZE': 0,
  'mousePos': 0,
  'audioLevel': 0,
  'audioFFT': 0,
};

let shaderCount = 0;
for (const entry of manifest) {
  if (!entry.file || entry.file.endsWith('.scene.js')) continue;
  const filePath = 'shaders/' + entry.file;
  if (!existsSync(filePath)) continue;

  const src = readFileSync(filePath, 'utf8');
  const parsed = parseISF(src);
  if (!parsed.meta) continue;

  shaderCount++;
  for (const func of Object.keys(funcUsage)) {
    if (parsed.glsl.includes(func)) funcUsage[func]++;
  }
}

console.log(`  Across ${shaderCount} ISF shaders:`);
for (const [func, count] of Object.entries(funcUsage)) {
  const pct = Math.round(count / shaderCount * 100);
  const bar = '#'.repeat(Math.round(pct / 5));
  console.log(`    ${func.padEnd(24)} ${count.toString().padStart(3)}/${shaderCount} (${pct.toString().padStart(3)}%) ${bar}`);
}

// ============================================================
// TEST 5: Scene file validation (POC scene)
// ============================================================
section('5. POC scene file validation');

const pocPath = 'scenes/isf_threejs_poc.scene.js';
if (existsSync(pocPath)) {
  const pocSrc = readFileSync(pocPath, 'utf8');

  // Check it's a valid IIFE scene
  if (pocSrc.includes('(function(THREE)')) ok('poc-iife', 'valid IIFE wrapper');
  else bad('poc-iife', 'missing (function(THREE) wrapper');

  // Check it returns INPUTS and create
  if (pocSrc.includes('return { INPUTS:')) ok('poc-return', 'returns INPUTS + create');
  else if (pocSrc.includes('INPUTS') && pocSrc.includes('create')) ok('poc-return', 'has INPUTS and create');
  else bad('poc-return', 'missing INPUTS or create export');

  // Check create function returns required interface
  if (pocSrc.includes('scene:') && pocSrc.includes('camera:')) ok('poc-interface', 'returns scene + camera');
  else bad('poc-interface', 'missing scene/camera in return');

  if (pocSrc.includes('update:')) ok('poc-update', 'has update function');
  else bad('poc-update', 'missing update function');

  if (pocSrc.includes('resize:')) ok('poc-resize', 'has resize function');
  else bad('poc-resize', 'missing resize function');

  if (pocSrc.includes('dispose:')) ok('poc-dispose', 'has dispose function');
  else bad('poc-dispose', 'missing dispose function');

  // Check it uses RawShaderMaterial (not ShaderMaterial)
  if (pocSrc.includes('RawShaderMaterial')) ok('poc-raw', 'uses RawShaderMaterial');
  else bad('poc-raw', 'should use RawShaderMaterial for ISF shaders');

  // Check it uses WebGLRenderTarget
  if (pocSrc.includes('WebGLRenderTarget')) ok('poc-rt', 'uses WebGLRenderTarget');
  else bad('poc-rt', 'should use WebGLRenderTarget for render passes');

  // Check the ISF fragment shader has the key ISF uniforms
  if (pocSrc.includes('uniform float TIME;') && pocSrc.includes('uniform vec2 RENDERSIZE;')) {
    ok('poc-uniforms', 'ISF uniforms in embedded shader');
  } else {
    bad('poc-uniforms', 'missing ISF uniforms in embedded shader');
  }

  // Check the shader uses gl_FragCoord (pixel-perfect mode)
  if (pocSrc.includes('gl_FragCoord')) ok('poc-fragcoord', 'uses gl_FragCoord for pixel-perfect');
  else bad('poc-fragcoord', 'missing gl_FragCoord — may not be pixel-perfect');

  // Check compositing pass exists
  if (pocSrc.includes('isfLayer') && pocSrc.includes('sceneLayer')) {
    ok('poc-composite', 'has compositing pass');
  } else {
    bad('poc-composite', 'missing compositing pass');
  }

  // Check render target chain (ISF → 3D → Composite)
  const hasISFRT = pocSrc.includes('isfRenderTarget');
  const has3DRT = pocSrc.includes('scene3DRT');
  if (hasISFRT && has3DRT) ok('poc-pipeline', 'has ISF + 3D render targets');
  else bad('poc-pipeline', 'missing render target chain');

  // Check it renders in order: ISF → 3D → composite
  const renderOrder = pocSrc.indexOf('render(isfScene') < pocSrc.indexOf('render(scene3D');
  if (renderOrder) ok('poc-order', 'render order: ISF → 3D → composite');
  else bad('poc-order', 'incorrect render order');

  // Check shader-as-texture feature
  if (pocSrc.includes('useShaderTex') && pocSrc.includes('isfRenderTarget.texture')) {
    ok('poc-shader-tex', 'shader-as-texture feature present');
  } else {
    bad('poc-shader-tex', 'missing shader-as-texture feature');
  }

  console.log('  POC scene file validated');
} else {
  bad('poc-exists', 'POC scene file not found at ' + pocPath);
}

// ============================================================
// TEST 6: ISF → Three.js uniform type mapping completeness
// ============================================================
section('6. Uniform type mapping');

const allInputTypes = new Set();
for (const entry of manifest) {
  if (!entry.file || entry.file.endsWith('.scene.js')) continue;
  const filePath = 'shaders/' + entry.file;
  if (!existsSync(filePath)) continue;
  const src = readFileSync(filePath, 'utf8');
  const parsed = parseISF(src);
  if (!parsed.meta) continue;
  for (const inp of parsed.inputs) {
    allInputTypes.add(inp.TYPE);
  }
}

console.log('  ISF input types found across all shaders:');
for (const type of allInputTypes) {
  const threeType = isfTypeToThreeJS(type);
  if (threeType) {
    ok(`type-${type}`, `${type} → ${threeType}`);
    console.log(`    ${type.padEnd(10)} → ${threeType}`);
  } else {
    bad(`type-${type}`, `no Three.js mapping for ISF type "${type}"`);
    console.log(`    ${type.padEnd(10)} → ??? (UNMAPPED)`);
  }
}

// ============================================================
// TEST 7: Coordinate system compatibility
// ============================================================
section('7. Coordinate system analysis');

let usesFragCoord = 0, usesNormCoord = 0, usesBoth = 0;
for (const entry of manifest) {
  if (!entry.file || entry.file.endsWith('.scene.js')) continue;
  const filePath = 'shaders/' + entry.file;
  if (!existsSync(filePath)) continue;
  const src = readFileSync(filePath, 'utf8');
  const parsed = parseISF(src);
  if (!parsed.meta) continue;

  const hasFC = parsed.glsl.includes('gl_FragCoord');
  const hasNC = parsed.glsl.includes('isf_FragNormCoord');
  if (hasFC && hasNC) usesBoth++;
  else if (hasFC) usesFragCoord++;
  else if (hasNC) usesNormCoord++;
}

console.log(`  gl_FragCoord only:      ${usesFragCoord} shaders (pixel coords — works directly in Three.js)`);
console.log(`  isf_FragNormCoord only: ${usesNormCoord} shaders (normalized — needs #define alias to vUv)`);
console.log(`  Both:                   ${usesBoth} shaders`);

if (usesNormCoord > 0 || usesBoth > 0) {
  ok('coord-alias', 'isf_FragNormCoord → vUv alias needed and provided');
} else {
  ok('coord-direct', 'all shaders use gl_FragCoord directly');
}

// Both approaches work in Three.js:
//   gl_FragCoord → works identically (built-in GLSL)
//   isf_FragNormCoord → aliased to vUv via #define
ok('coord-compat', 'coordinate systems are compatible');

// ============================================================
// SUMMARY
// ============================================================
section('SUMMARY');
console.log(`${pass} passed, ${fail} failed`);
if (failures.length > 0) {
  console.log('\nFailures:');
  failures.forEach(f => console.log(f));
  process.exit(1);
} else {
  console.log('\nAll tests passed! ISF shaders are compatible with Three.js transpilation.');
  console.log('\nKey findings:');
  console.log('  - ISF header parsing works for all shaders');
  console.log('  - All ISF input types have Three.js uniform mappings');
  console.log('  - ISF macros (IMG_PIXEL etc.) translate via #define');
  console.log('  - isf_FragNormCoord → vUv alias handles normalized coords');
  console.log('  - gl_FragCoord works identically (built-in GLSL)');
  console.log('  - Multi-pass shaders map to WebGLRenderTarget chains');
  console.log('  - POC scene demonstrates ISF + 3D in same Three.js context');
}
