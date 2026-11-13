// Name: AVA Dissolve
// Author: ShaderClaw
// License: MIT
// The quiet one: the outgoing picture drifts forward and softens out of focus while the
// incoming one settles back into focus underneath. No edges, no shapes, one eased move.
uniform float push; // = 0.05
uniform float blur; // = 0.018

vec3 avSoft(sampler2D tex, vec2 uv, float r) {
  if (r < 0.0005) return texture2D(tex, uv).rgb;
  vec3 c = texture2D(tex, uv).rgb * 2.0;
  for (int i = 0; i < 12; i++) {
    float a = float(i) * 2.39996;
    float d = r * sqrt((float(i) + 0.5) / 12.0);
    c += texture2D(tex, uv + vec2(cos(a) / ratio, sin(a)) * d).rgb;
  }
  return c / 14.0;
}

vec4 transition(vec2 uv) {
  float p = progress * progress * (3.0 - 2.0 * progress);          // ease in-out
  vec2 c = uv - 0.5;
  vec2 uvA = 0.5 + c * (1.0 - push * p);                           // outgoing drifts toward you
  vec2 uvB = 0.5 + c * (1.0 + push * 0.6 * (1.0 - p));             // incoming settles into place
  vec3 A = avSoft(from, uvA, blur * p);
  vec3 B = avSoft(to, uvB, blur * (1.0 - p) * 0.8);
  float m = smoothstep(0.12, 0.88, p);
  return vec4(mix(A, B, m), 1.0);
}
