// Simulate the full buildFragmentShader pipeline and output the compiled GLSL
import { readFileSync } from 'fs';

const file = process.argv[2] || 'shaders/laser.fs';
const source = readFileSync(file, 'utf8');

// parseISF
const match = source.match(/\/\*\s*(\{[\s\S]*?\})\s*\*\//);
if (!match) { console.log('NO ISF BLOCK'); process.exit(1); }
const meta = JSON.parse(match[1]);
const glsl = source.slice(source.indexOf(match[0]) + match[0].length).trim();
const inputs = meta.INPUTS || [];

function isfInputToUniform(input) {
  const t = input.TYPE;
  if (t === 'float') return `uniform float ${input.NAME};`;
  if (t === 'color') return `uniform vec4 ${input.NAME};`;
  if (t === 'bool') return `uniform bool ${input.NAME};`;
  if (t === 'point2D') return `uniform vec2 ${input.NAME};`;
  if (t === 'image') return `uniform sampler2D ${input.NAME};`;
  if (t === 'long') return `uniform float ${input.NAME};`;
  if (t === 'text') {
    const maxLen = input.MAX_LENGTH || 12;
    const lines = [];
    for (let i = 0; i < maxLen; i++) lines.push(`uniform float ${input.NAME}_${i};`);
    lines.push(`uniform float ${input.NAME}_len;`);
    return lines.join('\n');
  }
  return `// unknown type: ${t} ${input.NAME}`;
}

const uniformLines = inputs.map(isfInputToUniform);

const headerParts = [
  'precision highp float;',
  'uniform float TIME;',
  'uniform vec2 RENDERSIZE;',
  'uniform int PASSINDEX;',
  'uniform int FRAMEINDEX;',
  'varying vec2 isf_FragNormCoord;',
  '#define IMG_NORM_PIXEL(img, coord) texture2D(img, coord)',
  '#define IMG_PIXEL(img, coord) texture2D(img, coord / RENDERSIZE)',
  '#define IMG_THIS_PIXEL(img) texture2D(img, isf_FragNormCoord)',
  '#define IMG_THIS_NORM_PIXEL(img) texture2D(img, isf_FragNormCoord)',
  'uniform vec2 mousePos;',
  'uniform vec2 mouseDelta;',
  'uniform float mouseDown;',
  'uniform float pinchHold;',
  'uniform sampler2D audioFFT;',
  'uniform float audioLevel;',
  'uniform float audioBass;',
  'uniform float audioMid;',
  'uniform float audioHigh;',
  'uniform sampler2D varFontTex;',
  'uniform sampler2D fontAtlasTex;',
  'uniform float useFontAtlas;',
  'uniform float _voiceGlitch;',
  'uniform sampler2D mpHandLandmarks;',
  'uniform sampler2D mpFaceLandmarks;',
  'uniform sampler2D mpPoseLandmarks;',
  'uniform sampler2D mpSegMask;',
  'uniform float mpHandCount;',
  'uniform vec3 mpHandPos;',
  'uniform vec3 mpHandPos2;',
  'uniform float _transparentBg;',
  ...uniformLines,
  ''
];

const header = headerParts.join('\n');
const cleaned = glsl
  .replace(/#version\s+\d+.*/g, '')
  .replace(/#ifdef\s+GL_ES\s*\r?\nprecision\s+\w+\s+float\s*;\s*\r?\n#endif\s*\r?\n?/g, '');

const hasTransparentBg = inputs.some(i => i.NAME === 'transparentBg');
let body = header + cleaned;
const mainRe = /void\s+main\s*\(\s*(void)?\s*\)/;
const mainMatch = mainRe.test(body);

console.log('File:', file);
console.log('Inputs:', inputs.map(i => i.NAME + ':' + i.TYPE).join(', '));
console.log('main() regex matches:', mainMatch);
console.log('Has transparentBg input:', hasTransparentBg);
console.log('Wrapper applied:', mainMatch && !hasTransparentBg);

if (mainMatch && !hasTransparentBg) {
  body = body.replace(mainRe, 'void _shaderMain()');
  body += `
void main() {
    _shaderMain();
    vec2 _muv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 _md = _muv - mousePos;
    _md.x *= RENDERSIZE.x / RENDERSIZE.y;
    float _mr = length(_md);
    float _mspot = exp(-_mr * _mr * 6.0) * 0.25;
    gl_FragColor.rgb += gl_FragColor.rgb * _mspot;
    if (_transparentBg > 0.5) {
        float _lum = dot(gl_FragColor.rgb, vec3(0.299, 0.587, 0.114));
        gl_FragColor.a = smoothstep(0.02, 0.15, _lum);
    }
}
`;
}

// Check for obvious GLSL issues
const lines = body.split('\n');
console.log('Total lines:', lines.length);

// Check for duplicate precision
const precisions = lines.filter(l => l.match(/^\s*precision\s/));
console.log('Precision statements:', precisions.length);
precisions.forEach(p => console.log('  ' + p.trim()));

// Check for duplicate uniform declarations
const uniforms = {};
lines.forEach((l, i) => {
  const m = l.match(/^\s*uniform\s+\w+\s+(\w+)\s*;/);
  if (m) {
    if (uniforms[m[1]]) {
      console.log('DUPLICATE UNIFORM: ' + m[1] + ' at lines ' + uniforms[m[1]] + ' and ' + (i+1));
    }
    uniforms[m[1]] = i + 1;
  }
});

// Check for multiple main functions
const mains = lines.filter(l => l.match(/void\s+(main|_shaderMain)\s*\(/));
console.log('Main functions:', mains.length);
mains.forEach(m => console.log('  ' + m.trim()));

// Output the compiled GLSL
console.log('\n=== COMPILED GLSL ===');
console.log(body);
