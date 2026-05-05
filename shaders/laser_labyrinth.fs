/*{
  "CATEGORIES": ["Generator", "Radiant", "3D"],
  "DESCRIPTION": "Laser Labyrinth — 6 volumetric light cones sweeping through layered fog from above. Nightclub laser aesthetic. HDR linear output.",
  "INPUTS": [
    {"NAME":"sweepSpeed","TYPE":"float","DEFAULT":0.5,"MIN":0,"MAX":2,"LABEL":"Sweep Speed"},
    {"NAME":"beamIntensity","TYPE":"float","DEFAULT":1.0,"MIN":0,"MAX":2,"LABEL":"Beam Intensity"},
    {"NAME":"audioReact","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"baseColor","LABEL":"Tint","TYPE":"color","DEFAULT":[1.0,1.0,1.0,1.0]},
    {"NAME":"inputTex","LABEL":"Texture","TYPE":"image"}
  ]
}*/

precision highp float;

#define PI 3.14159265359

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash3(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }

float noise3d(vec3 p) {
  vec3 i = floor(p); vec3 f = fract(p);
  f = f * f * (3.0 - 2.0 * f);
  float n000 = hash3(i); float n100 = hash3(i + vec3(1,0,0));
  float n010 = hash3(i + vec3(0,1,0)); float n110 = hash3(i + vec3(1,1,0));
  float n001 = hash3(i + vec3(0,0,1)); float n101 = hash3(i + vec3(1,0,1));
  float n011 = hash3(i + vec3(0,1,1)); float n111 = hash3(i + vec3(1,1,1));
  return mix(mix(mix(n000,n100,f.x),mix(n010,n110,f.x),f.y),
             mix(mix(n001,n101,f.x),mix(n011,n111,f.x),f.y),f.z);
}

float fbm(vec3 p) {
  float v = 0.0; float a = 0.5; vec3 shift = vec3(100.0);
  for (int i = 0; i < 4; i++) { v += a * noise3d(p); p = p * 2.0 + shift; a *= 0.5; }
  return v;
}

vec3 coneColor(int idx, float hueShift) {
  vec3 col;
  if (idx == 0) col = vec3(1.0, 0.0, 0.5);
  else if (idx == 1) col = vec3(0.5, 0.05, 1.0);
  else if (idx == 2) col = vec3(0.1, 0.35, 1.0);
  else if (idx == 3) col = vec3(0.85, 0.0, 0.85);
  else if (idx == 4) col = vec3(0.15, 0.2, 1.0);
  else col = vec3(0.0, 0.8, 0.95);
  float angle = hueShift;
  float cosA = cos(angle); float sinA = sin(angle);
  float lum = dot(col, vec3(0.299, 0.587, 0.114));
  vec3 grey = vec3(lum); vec3 diff = col - grey;
  vec3 axis1 = normalize(vec3(1.0, -1.0, 0.0));
  vec3 axis2 = normalize(vec3(0.5, 0.5, -1.0));
  float d1 = dot(diff, axis1); float d2 = dot(diff, axis2);
  // No clamp — allow HDR colors; host applies ACES
  return grey + axis1 * (d1 * cosA - d2 * sinA) + axis2 * (d1 * sinA + d2 * cosA);
}

void main() {
  vec2 fragUV = gl_FragCoord.xy / RENDERSIZE.xy;
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 uv = fragUV;
  uv.x = (uv.x - 0.5) * aspect;

  float t = TIME * sweepSpeed;
  // Audio as modulator (not gate): alive in silence, boosted by audio
  float audioMod = 0.5 + 0.5 * audioLevel * audioReact;
  float intensity = beamIntensity * audioMod;

  vec3 fogCoord = vec3(fragUV * 3.0, t * 0.08);
  fogCoord.y -= t * 0.03; fogCoord.x += t * 0.015;
  float fogDensity = fbm(fogCoord);
  vec3 fogCoord2 = vec3(fragUV * 6.0 + 50.0, t * 0.12);
  fogCoord2.y -= t * 0.05;
  float fog = fogDensity * 0.5 + fbm(fogCoord2) * 0.5;
  fog = fog * fog * 1.5;

  vec3 col = vec3(0.0);
  float hueShift = sin(t * 0.07) * 0.2;
  float globalBeat = pow(abs(sin(t * PI / 1.5)), 8.0) * 0.4 + audioBass * audioReact * 0.6;

  for (int i = 0; i < 3; i++) {
    float fi = float(i);
    float originX = (fi - 1.0) * 0.4 * aspect + sin(t * 0.07 + fi * 2.5) * 0.1 * aspect;
    vec2 origin = vec2(originX, 1.05);
    float sweepAmp = 0.4 + fi * 0.1;
    float theta = sin(t * (0.3 + fi * 0.11) * 0.7 + fi * 1.9) * sweepAmp;
    vec2 dir = vec2(sin(theta), -cos(theta));
    vec2 toPixel = uv - origin;
    float along = dot(toPixel, dir);
    float perp = abs(toPixel.x * dir.y - toPixel.y * dir.x);
    float coneWidth = (0.13 + fi * 0.015) * max(along, 0.0) + 0.012;
    float inCone = exp(-perp * perp / (coneWidth * coneWidth * 0.55));
    inCone *= smoothstep(0.0, 0.08, along) * exp(-along * along * 0.15);
    float volumetric = inCone * (0.25 + fog * 0.75);
    // HDR beam cores — peak at ~1.5–2.5 so bloom catches them
    col += coneColor(i, hueShift + fi * 0.15) * volumetric * 1.5 * intensity * (1.0 + globalBeat);
  }

  for (int i = 0; i < 3; i++) {
    float fi = float(i);
    float originX = (fi - 1.0) * 0.5 * aspect + 0.15 * aspect + sin(t * 0.1 + fi * 3.1 + 1.0) * 0.08 * aspect;
    vec2 origin = vec2(originX, 1.02);
    float theta = sin(t * (0.4 + fi * 0.13) + fi * 2.3 + 0.7) * (0.5 + fi * 0.08);
    vec2 dir = vec2(sin(theta), -cos(theta));
    vec2 toPixel = uv - origin;
    float along = dot(toPixel, dir);
    float perp = abs(toPixel.x * dir.y - toPixel.y * dir.x);
    float coneWidth = (0.11 + fi * 0.012) * max(along, 0.0) + 0.01;
    float inCone = exp(-perp * perp / (coneWidth * coneWidth * 0.4));
    inCone += exp(-perp * perp / (coneWidth * coneWidth * 0.04)) * 0.4;
    inCone *= smoothstep(0.0, 0.06, along) * exp(-along * along * 0.1);
    float volumetric = inCone * (0.2 + fog * 0.8);
    col += coneColor(i + 3, hueShift + fi * 0.15 + 0.5) * volumetric * 2.5 * intensity * (1.0 + globalBeat);
  }

  float brightness = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(col, vec3(brightness * 1.3), smoothstep(0.4, 1.2, brightness) * 0.5);

  float groundHaze = smoothstep(0.2, 0.0, fragUV.y);
  col += col * groundHaze * 0.3;
  col += vec3(0.06, 0.03, 0.1) * groundHaze * fbm(vec3(fragUV.x * 4.0, fragUV.y * 2.0, t * 0.05 + 10.0)) * intensity;

  // Film grain — keep in linear; host ACES handles clipping
  col += hash(gl_FragCoord.xy + fract(TIME) * 100.0) * 0.03 - 0.015;

  // Soft vignette — no clamp (HDR output)
  vec2 vigUV = fragUV - 0.5;
  col *= max(0.0, 1.0 - dot(vigUV, vigUV) * 0.7);

  // Texture as source — shader VFX processes the input content
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 texSample = texture2D(inputTex, texUV);
    if (texSample.a > 0.01) {
        // Blend: texture is the source, shader effect modulates it
        float effectStrength = max(col.r, max(col.g, col.b));
        col = mix(texSample.rgb, col, 0.5) * (0.5 + effectStrength * 0.5);
        col *= baseColor.rgb;
    } else {
        col *= baseColor.rgb;
    }

  // Surprise: every ~15s the labyrinth solves itself — for ~0.5s a
  // single straight horizontal beam cuts through everything, the
  // shortest path revealed.
  {
      vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
      float _ph = fract(TIME / 15.0);
      float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.16, 0.08, _ph);
      float _y  = 0.5 + 0.20 * sin(floor(TIME / 15.0));
      float _beam = exp(-pow((_suv.y - _y) * 320.0, 2.0));
      col += vec3(1.0, 0.6, 0.2) * _beam * _f * 1.5;
  }

  gl_FragColor = vec4(col, 1.0);
}
