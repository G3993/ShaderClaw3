// ShaderClaw self-test. Open the site with ?selftest (optionally ?selftest&from=120 or
// ?selftest&only=eggs.fs,perlin.fs) on any device, phones included. It loads every shader
// in the picker on THIS device, checks it compiles and draws, performs the touch gestures
// (drag, pinch, double tap, triple tap) and checks the shader's controls answered.
// The report lists every shader that failed and can be copied out.
(function () {
  const q = new URLSearchParams(location.search);
  if (!q.has('selftest')) return;
  const sleep = (ms) => new Promise(r => setTimeout(r, ms));
  const box = document.createElement('div');
  box.id = 'selftest-box';
  box.style.cssText = 'position:fixed;left:8px;right:8px;bottom:8px;max-height:46vh;overflow:auto;z-index:99999;' +
    'background:rgba(0,0,0,.82);color:#fff;font:12px/1.45 ui-monospace,Menlo,monospace;padding:10px 12px;border-radius:12px;' +
    'white-space:pre-wrap;-webkit-user-select:text;user-select:text';
  const say = (t) => { box.textContent = t; };
  const sel = () => document.querySelector('.layer-shader-select[data-layer="shader"]');
  const canvas = () => document.getElementById('gl-canvas');
  const errBar = () => document.getElementById('error-bar');
  const pageErrs = [];
  window.addEventListener('error', (e) => pageErrs.push(String(e.message).slice(0, 140)));

  function frame() {
    const c = canvas(), t = document.createElement('canvas');
    t.width = 96; t.height = 192;
    const x = t.getContext('2d');
    try { x.drawImage(c, 0, 0, 96, 192); } catch (e) { return null; }
    return x.getImageData(0, 0, 96, 192).data;
  }
  const brightest = (d) => { let m = 0; for (let i = 0; i < d.length; i += 4) m = Math.max(m, d[i] + d[i + 1] + d[i + 2]); return m; };
  const differs = (a, b) => JSON.stringify(a) !== JSON.stringify(b);
  const G = (kind, id, x, y) => window._sc3Finger(kind, id, x, y);
  async function drag() {
    G('down', 't1', 0.3, 0.3);
    for (let i = 1; i <= 10; i++) { G('move', 't1', 0.3 + i * 0.035, 0.3 + i * 0.03); await sleep(24); }
    G('up', 't1'); await sleep(60);
  }
  async function pinch(dir) {
    const a = dir > 0 ? 0.06 : 0.3, s = dir > 0 ? 0.024 : -0.024;
    G('down', 't1', 0.5 - a, 0.5); G('down', 't2', 0.5 + a, 0.5); await sleep(40);
    for (let i = 1; i <= 10; i++) { G('move', 't1', 0.5 - a - i * s, 0.5); G('move', 't2', 0.5 + a + i * s, 0.5); await sleep(24); }
    G('up', 't1'); G('up', 't2'); await sleep(60);
  }
  async function taps(n) {
    for (let i = 0; i < n; i++) { G('down', 't1', 0.5, 0.5); await sleep(35); G('up', 't1'); await sleep(70); }
    await sleep(420);
  }

  async function run() {
    while (!sel() || !window._sc3Pick || !window._sc3Finger || sel().options.length < 5) await sleep(300);
    await sleep(3500);
    document.body.appendChild(box);
    let files = Array.from(sel().options).filter(o => o.value && !o.disabled && o.value !== '__fluid__').map(o => o.value);
    if (q.get('only')) files = q.get('only').split(',');
    const from = parseInt(q.get('from') || '0', 10) || 0;
    const res = [];
    const gl = canvas().getContext('webgl');
    for (let n = from; n < files.length; n++) {
      const f = files[n];
      say('Self-test ' + (n + 1) + ' / ' + files.length + '\n' + f + '\nfailed so far: ' + res.filter(r => r.fail.length).length);
      const eb = errBar(); if (eb) { eb.textContent = ''; eb.classList.remove('show'); }
      const e0 = pageErrs.length;
      sel().value = f; sel().dispatchEvent(new Event('change', { bubbles: true }));
      await sleep(2200);
      const r = { f, fail: [] };
      if (gl && gl.isContextLost()) {
        r.fail.push('graphics context lost'); res.push(r);
        say('Graphics context lost on ' + f + '. Reloading to continue...');
        try { sessionStorage.setItem('sc3-selftest', JSON.stringify(res)); } catch (e) {}
        await sleep(1500);
        q.set('from', String(n + 1)); location.search = '?' + q.toString();
        return;
      }
      const et = eb && eb.classList.contains('show') ? eb.textContent.trim() : '';
      if (et) r.fail.push('error: ' + et.replace(/\s+/g, ' ').slice(0, 150));
      if (pageErrs.length > e0) r.fail.push('script: ' + pageErrs[e0]);
      let d = frame();
      for (let w = 0; w < 6 && d && brightest(d) < 24; w++) { await sleep(1000); d = frame(); }   // slow phones take a while to compile
      if (d && brightest(d) < 24) r.fail.push('black screen');
      const pick = window._sc3Pick();
      if (!et && pick) {
        r.map = [pick.pt || pick.x, pick.y, pick.pinch, pick.e, pick.look].map(v => v || '-').join(' / ');
        const v0 = window._sc3Vals();
        await drag();
        const v1 = window._sc3Vals();
        const dragKey = pick.pt || pick.x;
        if (!pick.native && !dragKey) r.fail.push('no control mapped to drag');
        else if (dragKey && !differs(v0[dragKey], v1[dragKey])) r.fail.push('drag did not move ' + dragKey);
        if (pick.y && !differs(v0[pick.y], v1[pick.y])) r.fail.push('drag did not move ' + pick.y);
        if (pick.pinch) {
          await pinch(1); let v2 = window._sc3Vals();
          if (!differs(v1[pick.pinch], v2[pick.pinch])) { await pinch(-1); const v3 = window._sc3Vals(); if (!differs(v2[pick.pinch], v3[pick.pinch])) r.fail.push('pinch did not move ' + pick.pinch); }
        } else if (!pick.native) r.fail.push('no control mapped to pinch');
        if (pick.look) { const a = window._sc3Vals(); await taps(2); if (!differs(a[pick.look], window._sc3Vals()[pick.look])) r.fail.push('double tap did not change ' + pick.look); }
        await taps(3);
        const v9 = window._sc3Vals();
        for (const k of [pick.pt, pick.x, pick.y, pick.pinch, pick.look]) if (k && differs(v0[k], v9[k])) { r.fail.push('triple tap did not reset ' + k); break; }
      } else if (!et && !pick) r.fail.push('no touch map');
      res.push(r);
    }
    let prev = [];
    try { prev = JSON.parse(sessionStorage.getItem('sc3-selftest') || '[]'); sessionStorage.removeItem('sc3-selftest'); } catch (e) {}
    const all = prev.concat(res), bad = all.filter(r => r.fail.length);
    const lines = ['ShaderClaw self-test: ' + (all.length - bad.length) + ' of ' + all.length + ' passed, ' + bad.length + ' failed',
      navigator.userAgent, canvas().width + 'x' + canvas().height, ''];
    bad.forEach(r => lines.push(r.f + ': ' + r.fail.join('; ')));
    window._sc3SelfTest = { all, bad, text: lines.join('\n') };
    say(lines.join('\n'));
    const btn = document.createElement('button');
    btn.textContent = 'Copy report';
    btn.style.cssText = 'display:block;margin-top:10px;padding:8px 14px;border-radius:999px;border:0;background:#fff;color:#000;font:600 13px -apple-system,sans-serif';
    btn.onclick = () => { (navigator.clipboard ? navigator.clipboard.writeText(lines.join('\n')) : Promise.reject()).then(() => { btn.textContent = 'Copied'; }).catch(() => { btn.textContent = 'Select the text above to copy'; }); };
    box.appendChild(btn);
    if (q.get('report')) fetch(q.get('report'), { method: 'POST', mode: 'no-cors', body: JSON.stringify(window._sc3SelfTest) }).catch(() => {});
  }
  run();
})();
