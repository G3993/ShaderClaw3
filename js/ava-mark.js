// The AVA mark, drawn the way AVA OS draws it in its header: the floating figures traced by
// an oscilloscope beam. The logo's own 12 s animation loops with a slow turn, and whatever is
// driving the visuals (sound, or your finger) ripples along the outlines.
// Data: ava-floaters.bin = 180 frames (15 per second) x 3 figures x 64 points, x/y as int8.
(function () {
  var host = document.querySelector('.tb-brand');
  if (!host) return;
  var FRAMES = 180, LOOPS = 3, PTS = 64, FPS = 15;
  var c = document.createElement('canvas');
  c.className = 'ava-mark'; c.setAttribute('role', 'img'); c.setAttribute('aria-label', 'AVA');
  var still = host.querySelector('img');
  fetch('ava-floaters.bin').then(function (r) { return r.ok ? r.arrayBuffer() : Promise.reject(); }).then(function (buf) {
    var d = new Int8Array(buf);
    if (d.length < FRAMES * LOOPS * PTS * 2) return;
    if (still) still.replaceWith(c); else host.insertBefore(c, host.firstChild);
    var x = c.getContext('2d'), calm = matchMedia('(prefers-reduced-motion: reduce)').matches;
    function lvl(name) { try { return Math.max(0, Math.min(1, +window.eval(name) || 0)); } catch (e) { return 0; } }
    function draw(ms) {
      requestAnimationFrame(draw);
      if (!c.offsetParent) return;
      var dpr = Math.min(window.devicePixelRatio || 1, 3), size = c.clientWidth || 34, w = Math.round(size * dpr);
      if (c.width !== w) { c.width = c.height = w; }
      var t = calm ? 0 : ms / 1000;
      var touch = (window._sc3Touch && window._sc3Touch.energy) || 0;
      var level = Math.max(lvl('audioLevel'), touch * 0.8), bass = Math.max(lvl('audioBass'), touch);
      var ft = (t * FPS) % FRAMES, f0 = Math.floor(ft), f1 = (f0 + 1) % FRAMES, mix = ft - f0;
      var spin = t * (Math.PI * 2 / 90), cs = Math.cos(spin), sn = Math.sin(spin);
      var scale = 0.94 + 0.06 * level, r = w * 0.47, cx = w / 2, cy = w / 2;
      x.clearRect(0, 0, w, w);
      x.lineJoin = 'round';
      for (var li = 0; li < LOOPS; li++) {
        var a = (f0 * LOOPS + li) * PTS * 2, b = (f1 * LOOPS + li) * PTS * 2;
        var px = new Float32Array(PTS), py = new Float32Array(PTS), i;
        for (i = 0; i < PTS; i++) {
          px[i] = (d[a + i * 2] * (1 - mix) + d[b + i * 2] * mix) / 127;
          py[i] = (d[a + i * 2 + 1] * (1 - mix) + d[b + i * 2 + 1] * mix) / 127;
        }
        x.beginPath();
        for (i = 0; i < PTS; i++) {
          var ip = (i - 1 + PTS) % PTS, inx = (i + 1) % PTS;
          var tx = px[inx] - px[ip], ty = py[inx] - py[ip], tl = Math.sqrt(tx * tx + ty * ty) + 1e-6;
          var u = i * Math.PI * 2 / PTS;
          var wav = (0.004 + 0.035 * bass) * Math.sin(u * 7 - t * 3 + li) + (0.002 + 0.02 * level) * Math.sin(u * 13 + t * 4.5 + li * 0.7);
          var X = (px[i] - ty / tl * wav) * scale, Y = (py[i] + tx / tl * wav) * scale;
          var sx = cx + (X * cs - Y * sn) * r, sy = cy - (X * sn + Y * cs) * r;
          if (i) x.lineTo(sx, sy); else x.moveTo(sx, sy);
        }
        x.closePath();
        x.strokeStyle = 'rgba(255,255,255,0.14)'; x.lineWidth = 2.4 * dpr; x.stroke();   // the beam's glow
        x.strokeStyle = 'rgba(255,255,255,0.92)'; x.lineWidth = 0.85 * dpr; x.stroke();   // the beam
      }
    }
    requestAnimationFrame(draw);
  }).catch(function () {});
})();
