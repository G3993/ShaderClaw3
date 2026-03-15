/*{
  "DESCRIPTION": "Champions League Match — two competing energy fields driven by live match data. Bind possession, shots, goals to parameters via data signals.",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "momentum", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Momentum" },
    { "NAME": "homePossession", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Home Possession" },
    { "NAME": "awayPossession", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Away Possession" },
    { "NAME": "homeShots", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Home Shots" },
    { "NAME": "awayShots", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Away Shots" },
    { "NAME": "homeGoals", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Home Goals" },
    { "NAME": "awayGoals", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Away Goals" },
    { "NAME": "goalFlashHome", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Goal Flash H" },
    { "NAME": "goalFlashAway", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Goal Flash A" },
    { "NAME": "matchMinute", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Match Minute" },
    { "NAME": "intensity", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.5, "LABEL": "Intensity" },
    { "NAME": "homeColor", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0], "LABEL": "Home Color" },
    { "NAME": "awayColor", "TYPE": "color", "DEFAULT": [0.2, 0.5, 1.0, 1.0], "LABEL": "Away Color" }
  ]
}*/

#define TAU 6.2831853

float hash(vec2 p) {
  return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float noise(vec2 p) {
  vec2 i = floor(p);
  vec2 f = fract(p);
  f = f * f * (3.0 - 2.0 * f);
  float a = hash(i);
  float b = hash(i + vec2(1, 0));
  float c = hash(i + vec2(0, 1));
  float d = hash(i + vec2(1, 1));
  return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
  float v = 0.0;
  float a = 0.5;
  for (int i = 0; i < 5; i++) {
    v += a * noise(p);
    p *= 2.1;
    a *= 0.5;
  }
  return v;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE;
  vec2 centered = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME;

  vec3 hc = homeColor.rgb;
  vec3 ac = awayColor.rgb;

  // === TERRITORY BOUNDARY ===
  // Momentum shifts the dividing line (0.5 = center, >0.5 = home pushes right)
  float boundary = momentum;
  // Add turbulent noise to boundary edge
  float turbulence = fbm(vec2(centered.y * 3.0 + t * 0.3, t * 0.2)) * 0.15;
  turbulence += fbm(vec2(centered.y * 7.0 - t * 0.5, t * 0.15)) * 0.08;
  float edgeDist = uv.x - boundary - turbulence;

  // Soft blend zone at boundary
  float blendWidth = 0.06 + abs(homePossession - awayPossession) * 0.1;
  float homeMask = smoothstep(blendWidth, -blendWidth, edgeDist);
  float awayMask = 1.0 - homeMask;

  // === ENERGY FIELDS ===
  // Each side has flowing energy proportional to possession + shots
  float homeEnergy = fbm(centered * 3.0 + vec2(t * 0.4, t * 0.2)) * homePossession;
  homeEnergy += fbm(centered * 6.0 + vec2(-t * 0.6, t * 0.3)) * homeShots * 0.5;

  float awayEnergy = fbm(centered * 3.0 + vec2(-t * 0.4, -t * 0.2)) * awayPossession;
  awayEnergy += fbm(centered * 6.0 + vec2(t * 0.6, -t * 0.3)) * awayShots * 0.5;

  // === SHOT PULSES ===
  // Radial pulses from each side based on shot count
  float homePulse = 0.0;
  float awayPulse = 0.0;
  // Home shots pulse from left
  for (int i = 0; i < 5; i++) {
    float fi = float(i);
    float phase = fract(t * 0.3 + fi * 0.2) * homeShots;
    float radius = phase * 1.5;
    float d = length(centered - vec2(-0.4 + fi * 0.05, sin(fi * 2.3 + t) * 0.2));
    homePulse += exp(-pow(d - radius, 2.0) / 0.01) * (1.0 - phase) * homeShots;
  }
  // Away shots pulse from right
  for (int i = 0; i < 5; i++) {
    float fi = float(i);
    float phase = fract(t * 0.3 + fi * 0.2 + 0.5) * awayShots;
    float radius = phase * 1.5;
    float d = length(centered - vec2(0.4 - fi * 0.05, cos(fi * 1.7 + t) * 0.2));
    awayPulse += exp(-pow(d - radius, 2.0) / 0.01) * (1.0 - phase) * awayShots;
  }

  // === GOAL EXPLOSIONS ===
  float homeFlash = 0.0;
  float awayFlash = 0.0;
  if (goalFlashHome > 0.01) {
    float d = length(centered - vec2(-0.25, 0.0));
    homeFlash = exp(-d * d / (0.1 + goalFlashHome * 0.3)) * goalFlashHome * 3.0;
    // Expanding ring
    float ring = exp(-pow(d - goalFlashHome * 0.8, 2.0) / 0.005) * goalFlashHome * 2.0;
    homeFlash += ring;
  }
  if (goalFlashAway > 0.01) {
    float d = length(centered - vec2(0.25, 0.0));
    awayFlash = exp(-d * d / (0.1 + goalFlashAway * 0.3)) * goalFlashAway * 3.0;
    float ring = exp(-pow(d - goalFlashAway * 0.8, 2.0) / 0.005) * goalFlashAway * 2.0;
    awayFlash += ring;
  }

  // === BOUNDARY CLASH SPARKS ===
  // Where the two fields meet, sparks fly
  float clashZone = exp(-edgeDist * edgeDist / (blendWidth * blendWidth * 0.5));
  float sparks = 0.0;
  for (int i = 0; i < 8; i++) {
    float fi = float(i);
    vec2 sp = vec2(boundary + turbulence, (fi + 0.5) / 8.0);
    float d = length(uv - sp);
    float flicker = step(0.97, hash(vec2(fi, floor(t * 12.0 + fi))));
    sparks += smoothstep(0.015, 0.0, d) * flicker;
  }
  float clashIntensity = clashZone * (homeEnergy + awayEnergy) * 2.0;

  // === MATCH PROGRESS BAR ===
  // Thin line at bottom showing match progress
  float barY = smoothstep(0.008, 0.0, abs(uv.y - 0.015));
  float barProgress = step(uv.x, matchMinute);
  float progressBar = barY * 0.3;

  // === COMPOSITE ===
  vec3 col = vec3(0);

  // Base energy fields
  col += hc * homeEnergy * homeMask * intensity;
  col += ac * awayEnergy * awayMask * intensity;

  // Shot pulses
  col += hc * homePulse * 0.4 * intensity;
  col += ac * awayPulse * 0.4 * intensity;

  // Boundary clash — white sparks + mixed energy
  vec3 clashColor = mix(hc, ac, 0.5) + vec3(0.3);
  col += clashColor * clashIntensity * 0.3 * intensity;
  col += vec3(1) * sparks * 0.8;

  // Goal explosions
  col += hc * homeFlash + vec3(1) * homeFlash * 0.5;
  col += ac * awayFlash + vec3(1) * awayFlash * 0.5;

  // Goals scored: persistent glow orbs
  float goalGlowH = homeGoals * 0.3;
  float goalGlowA = awayGoals * 0.3;
  for (int i = 0; i < 4; i++) {
    float fi = float(i);
    float active = step(fi + 0.5, homeGoals * 8.0);
    vec2 gp = vec2(-0.35 + fi * 0.06, 0.35 + sin(t + fi) * 0.02);
    float d = length(centered - gp);
    col += hc * smoothstep(0.02, 0.0, d) * active * 0.8;
    col += hc * exp(-d * d / 0.002) * active * 0.3;
  }
  for (int i = 0; i < 4; i++) {
    float fi = float(i);
    float active = step(fi + 0.5, awayGoals * 8.0);
    vec2 gp = vec2(0.35 - fi * 0.06, 0.35 + cos(t + fi) * 0.02);
    float d = length(centered - gp);
    col += ac * smoothstep(0.02, 0.0, d) * active * 0.8;
    col += ac * exp(-d * d / 0.002) * active * 0.3;
  }

  // Progress bar
  vec3 barColor = mix(hc, ac, uv.x);
  col += barColor * progressBar * barProgress;
  col += vec3(0.15) * barY * (1.0 - barProgress);

  // Subtle vignette
  col *= 1.0 - dot((uv - 0.5) * 0.8, (uv - 0.5) * 0.8);

  // Background — very dark with slight team tint
  vec3 bg = mix(hc * 0.03, ac * 0.03, uv.x);
  col = max(col, bg);

  col = clamp(col, 0.0, 1.0);
  gl_FragColor = vec4(col, 1.0);
}
