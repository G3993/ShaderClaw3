// Audio Analysis Pipeline
// Extracted from ShaderClaw monolith

import { state, emit } from './state.js';

// Audio state is stored in state.audio but managed here
let audioCtx = null;
let audioAnalyser = null;
let audioDataArray = null;
let audioFFTGLTexture = null;
let audioFFTThreeTexture = null;
let audioLevel = 0, audioBass = 0, audioMid = 0, audioHigh = 0;
let activeAudioEntry = null;
let _cachedBarEl = null;
let _cachedBarId = null;

// --- Audio Feature Bus state (native Easel parity, see AudioFeatures.h) ---
// Smoothed with fast attack / slow release so motion defaults feel smooth
// (house default: release smoothing 0.85).
const _bus = {
  floats: {}, vec2: {}, vec3: {}, vec4: {},
};
const _sm = {}; // smoothed band values
let _prevBins = null;
let _prevLevel = 0, _prevEnergyVel = 0;
let _fluxAvg = 0.001, _lastBeatT = 0, _beatInterval = 0.5, _beatPulse = 0;
let _bandTime = { bass: 0, mid: 0, high: 0 };
let _lastNow = 0;

function _smooth(key, v, attack, release) {
  const prev = _sm[key] != null ? _sm[key] : v;
  const a = v > prev ? (attack != null ? attack : 0.5) : (release != null ? release : 0.85);
  return (_sm[key] = prev * a + v * (1 - a));
}

function _bandAvg(bins, a, b) {
  let s = 0;
  for (let i = a; i < b; i++) s += bins[i];
  return s / ((b - a) * 255);
}

/**
 * Initialize audio context (call on user gesture)
 */
export function initAudioContext() {
  if (audioCtx) return audioCtx;
  audioCtx = new (window.AudioContext || window.webkitAudioContext)();
  audioAnalyser = audioCtx.createAnalyser();
  audioAnalyser.fftSize = 256;
  audioDataArray = new Uint8Array(audioAnalyser.frequencyBinCount); // 128 bins
  // Sync to state
  state.audio.ctx = audioCtx;
  state.audio.analyser = audioAnalyser;
  state.audio.dataArray = audioDataArray;
  return audioCtx;
}

/**
 * Connect a media element to the audio analyser
 */
export function connectAudioSource(mediaElement) {
  if (!audioCtx) initAudioContext();
  // Disconnect previous
  if (activeAudioEntry && activeAudioEntry._sourceNode) {
    try { activeAudioEntry._sourceNode.disconnect(); } catch(e) {}
  }
  const source = audioCtx.createMediaElementSource(mediaElement);
  source.connect(audioAnalyser);
  audioAnalyser.connect(audioCtx.destination);
  activeAudioEntry = { element: mediaElement, _sourceNode: source };
  state.audio.activeEntry = activeAudioEntry;
  return source;
}

/**
 * Set active audio entry directly (for mic, etc.)
 */
export function setActiveAudioEntry(entry) {
  activeAudioEntry = entry;
  state.audio.activeEntry = entry;
}

/**
 * Per-frame audio analysis — call from render loop
 * Updates FFT texture and level/bass/mid/high values
 * @param {WebGLRenderingContext} gl
 */
export function updateAudioUniforms(gl) {
  if (!audioAnalyser || !activeAudioEntry) {
    audioLevel = audioBass = audioMid = audioHigh = 0;
    _updateFeatureBus(null);
    _syncState();
    return;
  }
  audioAnalyser.getByteFrequencyData(audioDataArray);
  const len = audioDataArray.length; // 128 bins

  // RMS level
  let sum = 0;
  for (let i = 0; i < len; i++) sum += audioDataArray[i] * audioDataArray[i];
  audioLevel = Math.sqrt(sum / len) / 255.0;

  // Bass (0-15), Mid (16-80), High (81-127)
  let bassSum = 0, midSum = 0, highSum = 0;
  for (let i = 0; i < 16; i++) bassSum += audioDataArray[i];
  for (let i = 16; i < 81; i++) midSum += audioDataArray[i];
  for (let i = 81; i < len; i++) highSum += audioDataArray[i];
  audioBass = bassSum / (16 * 255);
  audioMid = midSum / (65 * 255);
  audioHigh = highSum / (47 * 255);

  _updateFeatureBus(audioDataArray);

  // Upload FFT to GL texture (256x1 LUMINANCE)
  if (!audioFFTGLTexture) {
    audioFFTGLTexture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, audioFFTGLTexture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    state.audio.fftGLTexture = audioFFTGLTexture;
  }
  gl.bindTexture(gl.TEXTURE_2D, audioFFTGLTexture);
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.LUMINANCE, len, 1, 0, gl.LUMINANCE, gl.UNSIGNED_BYTE, audioDataArray);

  // Update THREE DataTexture (if THREE is available)
  if (window.THREE) {
    if (!audioFFTThreeTexture) {
      audioFFTThreeTexture = new THREE.DataTexture(audioDataArray, len, 1, THREE.LuminanceFormat);
      audioFFTThreeTexture.needsUpdate = true;
      state.audio.fftThreeTexture = audioFFTThreeTexture;
    } else {
      audioFFTThreeTexture.needsUpdate = true;
    }
  }

  _syncState();

  // Update audio bar UI (cached DOM ref to avoid querySelector per frame)
  if (activeAudioEntry && activeAudioEntry.id) {
    if (_cachedBarId !== activeAudioEntry.id) {
      _cachedBarEl = document.querySelector('.audio-bar-fill[data-audio-id="' + activeAudioEntry.id + '"]');
      _cachedBarId = activeAudioEntry.id;
    }
    if (_cachedBarEl) _cachedBarEl.style.width = (audioLevel * 100) + '%';
  }
}

/**
 * Compute the Audio Feature Bus (tier-1 approximation of the native
 * AudioFeatures engine) and publish it on window._audioBus + state.audio.bus.
 * bins === null means silence — decay everything toward rest.
 */
function _updateFeatureBus(bins) {
  const now = (typeof performance !== 'undefined' ? performance.now() : Date.now()) / 1000;
  const dt = Math.min(0.1, Math.max(0.001, now - (_lastNow || now - 0.016)));
  _lastNow = now;
  const f = _bus.floats;

  let sub = 0, lowMid = 0, highMid = 0, treble = 0, brightness = 0, flux = 0, bassFlux = 0, midFlux = 0, highFlux = 0;
  if (bins) {
    const len = bins.length; // 128 bins, ~172 Hz each
    sub = _bandAvg(bins, 0, 2);
    lowMid = _bandAvg(bins, 2, 12);
    highMid = _bandAvg(bins, 12, 41);
    treble = _bandAvg(bins, 41, len);
    // spectral centroid → brightness
    let wsum = 0, msum = 0;
    for (let i = 0; i < len; i++) { wsum += bins[i] * i; msum += bins[i]; }
    brightness = msum > 1 ? (wsum / msum) / len : 0;
    // positive spectral flux (broadband + per-band)
    if (_prevBins) {
      for (let i = 0; i < len; i++) {
        const d = (bins[i] - _prevBins[i]) / 255;
        if (d > 0) {
          flux += d;
          if (i < 16) bassFlux += d; else if (i < 81) midFlux += d; else highFlux += d;
        }
      }
      flux /= len; bassFlux /= 16; midFlux /= 65; highFlux /= 47;
    }
    if (!_prevBins) _prevBins = new Uint8Array(len);
    _prevBins.set(bins);
  } else {
    _prevBins = null;
  }

  // Smoothed bands (fast attack, smooth release per house default)
  f.audioSub = _smooth('sub', sub);
  f.audioLowMid = _smooth('lowMid', lowMid);
  f.audioHighMid = _smooth('highMid', highMid);
  f.audioTreble = _smooth('treble', treble);
  f.audioBrightness = _smooth('bright', brightness);
  f.audioFlux = _smooth('flux', Math.min(1, flux * 6), 0.3, 0.8);
  f.audioOnset = Math.min(1, flux * 10);
  f.audioPunch = _smooth('punch', Math.min(1, bassFlux * 8), 0.2, 0.75);

  // Beat detection: adaptive threshold on bass flux
  _fluxAvg = _fluxAvg * 0.98 + bassFlux * 0.02;
  let beat = 0;
  if (bassFlux > Math.max(0.02, _fluxAvg * 2.2) && (now - _lastBeatT) > 0.25) {
    beat = 1;
    if (_lastBeatT > 0) {
      const iv = now - _lastBeatT;
      if (iv < 2.0) _beatInterval = _beatInterval * 0.7 + iv * 0.3;
    }
    _lastBeatT = now;
    _beatPulse = 1;
  }
  _beatPulse *= Math.exp(-dt * 6.0);
  f.audioBeat = beat;
  f.audioBeatPulse = _beatPulse;
  f.audioBeatPhase = _beatInterval > 0 ? Math.min(1, (now - _lastBeatT) / _beatInterval) : 0;
  f.audioBarPhase = _beatInterval > 0 ? ((now - _lastBeatT) / (_beatInterval * 4)) % 1 : 0;
  f.audioBPM = _beatInterval > 0 ? 60 / _beatInterval : 0;
  f.audioTempo01 = Math.max(0, Math.min(1, (f.audioBPM - 60) / 120));

  // Energy / structure
  const energy = audioLevel;
  f.audioEnergy = _smooth('energy', energy);
  const vel = (energy - _prevLevel) / dt;
  f.audioEnergyVel = _smooth('energyVel', Math.max(-1, Math.min(1, vel)), 0.4, 0.8);
  f.audioEnergyAcc = Math.max(-1, Math.min(1, (f.audioEnergyVel - _prevEnergyVel) / dt));
  _prevEnergyVel = f.audioEnergyVel;
  _prevLevel = energy;
  f.audioBuildup = _smooth('buildup', Math.max(0, f.audioEnergyVel), 0.3, 0.97);
  f.audioBuildupRate = f.audioEnergyVel > 0 ? f.audioEnergyVel : 0;
  f.audioDrop = f.audioEnergy < 0.08 && _sm['energyPrevHigh'] > 0.4 ? 1 : 0;
  _sm['energyPrevHigh'] = Math.max(f.audioEnergy, (_sm['energyPrevHigh'] || 0) * 0.995);

  // Spectral character (cheap approximations)
  f.audioSpread = _smooth('spread', Math.min(1, Math.abs(treble - sub) + Math.abs(highMid - lowMid)));
  f.audioRolloff = f.audioBrightness;
  f.audioFlatness = _smooth('flat', 1 - Math.abs(audioBass - audioHigh));
  f.audioTexture = f.audioFlatness;
  f.audioOnsetRate = _smooth('onsetRate', f.audioOnset, 0.3, 0.95);
  f.audioTilt = _smooth('tilt', audioBass - audioHigh);
  f.audioZCR = f.audioBrightness;

  // Affect / mood (heuristic but musical)
  f.audioValence = _smooth('valence', 0.5 + 0.5 * (f.audioBrightness - 0.35) + 0.2 * (f.audioBeatPulse - 0.3));
  f.audioArousal = _smooth('arousal', Math.min(1, f.audioEnergy * 1.4 + f.audioOnsetRate * 0.4));
  f.audioTension = _smooth('tension', Math.min(1, f.audioBuildup * 1.5));
  f.audioWarmth = _smooth('warmth', Math.min(1, audioBass * 1.2 * (1 - f.audioBrightness * 0.5)));
  f.audioSoftness = _smooth('softness', Math.max(0, 1 - f.audioOnsetRate * 2));
  f.audioRoughness = _smooth('rough', Math.min(1, f.audioFlux * 1.5));
  f.audioCharm = f.audioValence * f.audioSoftness;
  f.audioNovelty = f.audioOnset;
  f.audioSectionPhase = f.audioBarPhase;
  f.audioSectionAge = Math.min(1, (now - _lastBeatT) / 16);
  f.audioLayers = _smooth('layers', Math.min(1, (audioBass > 0.1 ? 0.34 : 0) + (audioMid > 0.1 ? 0.33 : 0) + (audioHigh > 0.1 ? 0.33 : 0)));
  f.audioDensity = f.audioLayers;
  f.audioDominantPitch = f.audioBrightness;
  f.audioMajorMinor = f.audioValence;
  f.audioHCDF = f.audioFlux;

  // Legacy advanced names (kept for older shaders)
  f.audioBassHit = f.audioPunch;
  f.audioMidHit = _smooth('midHit', Math.min(1, midFlux * 8), 0.2, 0.75);
  f.audioHighHit = _smooth('highHit', Math.min(1, highFlux * 8), 0.2, 0.75);
  _bandTime.bass += audioBass * dt; _bandTime.mid += audioMid * dt; _bandTime.high += audioHigh * dt;
  f.audioBassTime = _bandTime.bass;
  f.audioMidTime = _bandTime.mid;
  f.audioHighTime = _bandTime.high;

  // Palette anchors (linear-ish RGB, warmth/brightness driven)
  const warm = f.audioWarmth, bright = f.audioBrightness, sat = Math.min(1, 0.4 + f.audioEnergy);
  f.audioPalTemp = warm;
  f.audioPalSat = sat;
  _bus.vec3.audioPalShadow = [0.02 + warm * 0.05, 0.02, 0.05 + (1 - warm) * 0.06];
  _bus.vec3.audioPalMid = [0.2 + warm * 0.5, 0.15 + bright * 0.3, 0.5 - warm * 0.25 + bright * 0.2];
  _bus.vec3.audioPalHigh = [0.6 + warm * 0.4, 0.55 + bright * 0.4, 0.75 + (1 - warm) * 0.25];
  _bus.vec3.audioPalAccent = [0.9, 0.25 + bright * 0.55, 0.35 + (1 - warm) * 0.5];
  _bus.vec2.audioMood = [f.audioValence, f.audioArousal];
  _bus.vec2.audioFlow = [f.audioTilt, f.audioEnergyVel];
  _bus.vec4.audioPresence = [audioBass, audioMid, audioHigh, f.audioEnergy];

  if (typeof window !== 'undefined') window._audioBus = _bus;
  state.audio.bus = _bus;
}

function _syncState() {
  state.audio.level = audioLevel;
  state.audio.bass = audioBass;
  state.audio.mid = audioMid;
  state.audio.high = audioHigh;
  state.audio.fftGLTexture = audioFFTGLTexture;
}

/**
 * Get current audio levels (for external queries)
 */
export function getAudioLevels() {
  return {
    level: audioLevel,
    bass: audioBass,
    mid: audioMid,
    high: audioHigh,
    hasAudio: !!activeAudioEntry
  };
}

/**
 * Get audio state for passing to renderer
 */
export function getAudioState() {
  return {
    level: audioLevel,
    bass: audioBass,
    mid: audioMid,
    high: audioHigh,
    fftGLTexture: audioFFTGLTexture,
    fftThreeTexture: audioFFTThreeTexture,
  };
}

/**
 * Cleanup
 */
export function disposeAudio() {
  if (activeAudioEntry && activeAudioEntry._sourceNode) {
    try { activeAudioEntry._sourceNode.disconnect(); } catch(e) {}
  }
  activeAudioEntry = null;
  if (audioCtx) {
    try { audioCtx.close(); } catch(e) {}
    audioCtx = null;
  }
  audioAnalyser = null;
  audioDataArray = null;
  audioFFTGLTexture = null;
  audioFFTThreeTexture = null;
}
