/*{
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "DESCRIPTION": "Octagonal sound temple — eight frequency bands circle you in synchronised pulses. Designed for 5-channel installations where light orbits the listener like a Turrell apse.",
  "INPUTS": [
    {"NAME":"sectors","TYPE":"float","MIN":4.0,"MAX":12.0,"DEFAULT":8.0},
    {"NAME":"pulseSpeed","TYPE":"float","MIN":0.0,"MAX":3.0,"DEFAULT":0.6},
    {"NAME":"pulseWidth","TYPE":"float","MIN":0.01,"MAX":0.2,"DEFAULT":0.06},
    {"NAME":"seamSoftness","TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.008},
    {"NAME":"coreSize","TYPE":"float","MIN":0.0,"MAX":0.4,"DEFAULT":0.12},
    {"NAME":"texMix","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.3},
    {"NAME":"paletteShift","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.0},
    {"NAME":"trail","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.4},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

#define TAU 6.28318530718
#define PI  3.14159265358

// Per-sector colour wheel — picks a hue evenly distributed around the
// octagon, then offset by paletteShift so the user can rotate the
// colour temple without rotating the geometry.
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 sectorColor(float sec, float total, float shift) {
    float h = sec / total + shift;
    return hsv2rgb(vec3(fract(h), 0.78, 1.0));
}

void main() {
    // Centred, aspect-corrected polar coords. Use the smaller dimension
    // so the temple stays circular regardless of viewport ratio.
    vec2  p     = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy)
                / min(RENDERSIZE.x, RENDERSIZE.y) * 2.0;
    float r     = length(p);
    float th    = atan(p.y, p.x);

    float secs  = max(4.0, sectors);
    float sec   = floor((th + PI) / TAU * secs);
    float secF  = sec / secs;

    // Per-sector FFT bin. Distribute bins across 0..0.85 of the FFT
    // texture so we skip the very-top bins which are mostly noise.
    float bin   = (sec + 0.5) / secs * 0.85;
    float amp   = texture(audioFFT, vec2(bin, 0.5)).r;

    // Travelling pulse — fract advances over time, offset per sector
    // so consecutive sectors see the pulse at staggered phases. This
    // produces a "wave going around the room" feel.
    float pulsePhase = fract(TIME * pulseSpeed - secF);
    float pulse      = smoothstep(pulseWidth, 0.0, abs(r - pulsePhase));

    // Trail — second softer pulse behind the leading edge so each band
    // reads as comet-like rather than a single stripe.
    float trailPhase = fract(TIME * pulseSpeed - secF - 0.12);
    float trailPulse = smoothstep(pulseWidth * 2.0, 0.0,
                                  abs(r - trailPhase)) * trail;

    // Sector seam — anti-aliased line on the octant boundary so the
    // temple geometry reads as a real shape, not a smooth gradient.
    float seamFrac = fract((th + PI) / TAU * secs);
    float seam     = 1.0 - smoothstep(0.0, seamSoftness * secs * 0.5,
                                      min(seamFrac, 1.0 - seamFrac));

    // Compose
    vec3 hue   = sectorColor(sec, secs, paletteShift);
    vec3 col   = hue * (pulse + trailPulse) * (0.45 + amp * 1.85);

    // Subtle base wash so dark sectors aren't pure black.
    col += hue * 0.04;

    // Centre core — bass-driven breathing sphere
    float core = smoothstep(coreSize, coreSize * 0.55, r);
    col += vec3(1.0, 0.96, 0.92) * core * (0.4 + audioBass * 1.6);

    // Inter-sector seam — neutral light so it reads as architecture.
    col += vec3(0.95) * seam * 0.18;

    // Optional input texture mapped radially around the temple — like
    // a print on the curved interior wall. Wraps angle to U, radius
    // to V so a square input becomes a 360° panorama.
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec2 texUV = vec2(fract(th / TAU + 0.5), clamp(r, 0.0, 1.0));
        vec3 t = texture(inputTex, texUV).rgb;
        col = mix(col, t * (0.5 + amp), texMix);
    }

    // Soft outer falloff so the chamber dissolves at the corners
    // instead of clipping against a hard rectangle.
    col *= smoothstep(1.25, 0.9, r);

    gl_FragColor = vec4(col, 1.0);
}
