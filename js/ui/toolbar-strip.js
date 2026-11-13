// ============================================================
// ShaderClaw 3 — Top Bar Controller
// One row of grouped controls; each item opens its own dropdown panel
// ============================================================

(function initToolbarStrip() {
  const propertiesPanel = document.querySelector('.sc3-properties');
  const tabs = document.querySelectorAll('.sc3-tab');
  const tabContents = document.querySelectorAll('.sc3-tab-content');

  // --- Move source panel sections into tab containers ---

  // Gallery → Gallery tab
  const gallerySection = document.getElementById('gallery-tab-content');
  const galleryTarget = document.querySelector('.sc3-tab-content[data-tab-content="gallery"]');
  if (gallerySection && galleryTarget) {
    gallerySection.style.display = '';
    gallerySection.classList.remove('collapsed');
    const body = gallerySection.querySelector('.sc3-section-body');
    if (body) body.style.display = '';
    galleryTarget.appendChild(gallerySection);
  }

  // Assets section → Assets tab
  const assetsSection = document.getElementById('inputs-section');
  const assetsTarget = document.querySelector('.sc3-tab-content[data-tab-content="assets"]');
  if (assetsSection && assetsTarget) {
    assetsSection.style.display = '';
    assetsSection.classList.remove('collapsed');
    assetsTarget.appendChild(assetsSection);
  }

  // Code Editor → inside Assets tab (appended after the assets section)
  const codeSection = document.getElementById('editor-section');
  if (codeSection && assetsTarget) {
    codeSection.style.display = '';
    codeSection.classList.remove('collapsed');
    const body = codeSection.querySelector('.sc3-section-body');
    if (body) body.style.display = '';
    assetsTarget.appendChild(codeSection);
  }

  // --- Wire up new content tile grid clicks ---
  const contentTiles = document.querySelectorAll('.sc3-content-tile[data-type]');
  contentTiles.forEach(tile => {
    tile.addEventListener('click', () => {
      const type = tile.dataset.type;
      // Map to existing hidden tile buttons for compat
      const tileMap = {
        image: 'tile-image',
        video: 'tile-video',
        model: 'tile-model',
        sound: 'tile-sound',
        mic: 'tile-mic',
        webcam: 'tile-webcam',
        ndi: 'tile-ndi',
        code: 'tile-code'
      };
      if (type === 'overlay') {
        const oi = document.getElementById('image-file-input');
        if (oi) oi.click();
      } else if (type === 'file') {
        const fi = document.getElementById('data-file-input') || document.getElementById('code-file-input');
        if (fi) fi.click();
      } else {
        const hiddenTile = document.getElementById(tileMap[type]);
        if (hiddenTile) hiddenTile.click();
      }
    });
  });

  // --- Tab switching ---
  function activateTab(tabName) {
    tabs.forEach(t => t.classList.toggle('active', t.dataset.tab === tabName));
    tabContents.forEach(tc => tc.classList.toggle('active', tc.dataset.tabContent === tabName));

    // CodeMirror needs a refresh when code section becomes visible
    if (tabName === 'assets') {
      const cm = document.querySelector('.CodeMirror');
      if (cm && cm.CodeMirror) {
        setTimeout(() => cm.CodeMirror.refresh(), 50);
      }
    }
  }

  // ============================================================
  // Top bar — every control lives in one row, bunched into groups.
  // An item opens ONE dropdown panel under itself holding just that
  // group's controls. Click the item again (or Esc) to close.
  // Canvas presses never close the panel: you tweak while you play.
  // ============================================================
  const canvasTab = document.querySelector('.sc3-tab-content[data-tab-content="canvas"]');
  const items = document.querySelectorAll('.tb-item[data-open]');
  let openKey = null;
  let lastKey = 'shader-layer';

  function soloSection(sectionKey) {
    if (!canvasTab) return;
    canvasTab.classList.toggle('solo', !!sectionKey);
    Array.from(canvasTab.children).forEach(el => {
      const match = !!sectionKey && (
        sectionKey === 'canvas-size'
          ? el.classList.contains('sc3-canvas-size-row')
          : !!el.querySelector('[data-section="' + sectionKey + '"]'));
      el.classList.toggle('solo-target', match);
      if (match) el.classList.remove('collapsed');
    });
    canvasTab.scrollTop = 0;
  }

  function anchorPanel(btn) {
    if (!propertiesPanel || !btn) return;
    const gap = 12;
    const group = btn.closest('.tb-group');
    const r = group.getBoundingClientRect();
    const w = propertiesPanel.offsetWidth;
    const want = group.classList.contains('tb-viewer') ? r.left + r.width / 2 - w / 2 : r.left;
    const left = Math.max(gap, Math.min(want, window.innerWidth - w - gap));
    propertiesPanel.style.left = left + 'px';
  }

  function closePanel() {
    openKey = null;
    if (propertiesPanel) propertiesPanel.classList.add('panel-closed');
    document.body.classList.remove('panel-open');
    items.forEach(b => b.classList.remove('active'));
  }

  // Two bar items can own the same panel (viewer title + Gallery) — use the visible one
  function itemFor(key) {
    const all = document.querySelectorAll('.tb-item[data-open="' + key + '"]');
    return Array.from(all).find(b => b.offsetParent !== null) || all[0] || null;
  }

  function openPanel(key) {
    const btn = itemFor(key);
    openKey = lastKey = key;
    items.forEach(b => b.classList.toggle('active', b.dataset.open === key));
    if (propertiesPanel) {
      propertiesPanel.classList.add('visible');
      propertiesPanel.dataset.panel = key;
    }
    if (key.startsWith('tab:')) {
      soloSection(null);
      activateTab(key.slice(4));
    } else {
      activateTab('canvas');
      soloSection(key);
    }
    anchorPanel(btn);
    if (propertiesPanel) propertiesPanel.classList.remove('panel-closed');
    document.body.classList.add('panel-open');
    if (btn && btn.scrollIntoView) btn.scrollIntoView({ block: 'nearest', inline: 'nearest' });
  }

  function togglePanel(key) {
    if (openKey === key) closePanel(); else openPanel(key);
  }

  items.forEach(btn => btn.addEventListener('click', () => togglePanel(btn.dataset.open)));

  // Legacy tab buttons (hidden) — other modules still .click() them
  tabs.forEach(tab => tab.addEventListener('click', () => {
    const key = tab.dataset.tab === 'canvas' ? 'shader-layer' : 'tab:' + tab.dataset.tab;
    if (openKey !== key) openPanel(key);
  }));

  window.addEventListener('resize', () => {
    if (openKey) anchorPanel(itemFor(openKey));
  });

  // Export drops from the same corner — one dropdown at a time
  const exportBtn = document.getElementById('export-hub-btn');
  if (exportBtn) exportBtn.addEventListener('click', closePanel);

  // --- Shader name in the bar: the first item names what is playing ---
  const shaderNameEl = document.getElementById('tb-shader-name');
  const viewNameEl = document.getElementById('tb-view-name');
  const viewCountEl = document.getElementById('tb-view-count');
  const shaderSel = () => document.querySelector('.layer-shader-select[data-layer="shader"]');
  let pool = null;            // files safe to show with no media inputs
  let lastFile = null;

  fetch('shaders/manifest.json').then(r => r.json()).then(m => {
    pool = new Set(m.filter(i =>
      !i.hidden && /\.fs$/.test(i.file || '') &&
      (i.folder || 'shaders') === 'shaders' &&
      !(i.categories || []).includes('Text') &&
      (i.type === 'generator' || (i.categories || []).includes('Generator'))
    ).map(i => i.file));
  }).catch(() => {});

  // Shaders that don't hold up for a visitor (won't compile, blocked as too heavy, or
  // black on a phone) are left out of the walk. shaders/viewer-skip.json is the list the
  // phone sweep produced; anything that fails live is added for the rest of the visit.
  const skip = new Set();
  try { JSON.parse(sessionStorage.getItem('sc3-skip') || '[]').forEach(f => skip.add(f)); } catch (e) {}
  fetch('shaders/viewer-skip.json').then(r => r.ok ? r.json() : []).then(list => {
    (list || []).forEach(f => skip.add(f));
  }).catch(() => {});

  // The shaders a visitor steps through, in gallery (alphabetical) order
  function playlist() {
    const sel = shaderSel();
    if (!sel) return [];
    return Array.from(sel.options).filter(o => o.value && !o.disabled && !skip.has(o.value) &&
      (o.value === '__fluid__' || !pool || pool.has(o.value)));
  }

  let stepToken = 0;
  function stepShader(dir, hops) {
    const sel = shaderSel();
    const list = playlist();
    if (!sel || !list.length) return;
    // from a shader outside the walk, continue from where it sits alphabetically
    const all = Array.from(sel.options);
    let i = list.findIndex(o => o.value === sel.value);
    if (i < 0) {
      const at = all.findIndex(o => o.value === sel.value);
      const after = list.findIndex(o => all.indexOf(o) > at);
      i = dir > 0 ? (after < 0 ? list.length : after) - 1 : (after < 0 ? list.length : after);
    }
    const next = list[(i + dir + list.length) % list.length];
    sel.value = next.value;
    sel.dispatchEvent(new Event('change', { bubbles: true }));
    syncShaderName();
    // If it fails to load, drop it and keep walking — a visitor never lands on an error
    const token = ++stepToken;
    const bar = document.getElementById('error-bar');
    let checks = 0;
    const watch = setInterval(() => {
      if (token !== stepToken || ++checks > 25) return clearInterval(watch);
      if (!(bar && bar.classList.contains('show') && bar.textContent.trim())) return;
      clearInterval(watch);
      skip.add(next.value);
      try { sessionStorage.setItem('sc3-skip', JSON.stringify(Array.from(skip))); } catch (e) {}
      bar.classList.remove('show');
      if ((hops || 0) < 12) stepShader(dir, (hops || 0) + 1);
    }, 100);
  }

  function syncShaderName() {
    const sel = shaderSel();
    const opt = sel && sel.selectedOptions && sel.selectedOptions[0];
    const name = (opt && opt.textContent.replace(/^[^A-Za-z0-9]+/, '').trim()) || '';
    if (shaderNameEl && shaderNameEl.textContent !== (name || 'Shader')) shaderNameEl.textContent = name || 'Shader';
    if (viewNameEl && name && viewNameEl.textContent !== name) viewNameEl.textContent = name;
    if (viewCountEl && opt) {
      const list = playlist();
      const i = list.indexOf(opt);
      const txt = i >= 0 ? (i + 1) + ' / ' + list.length : '';
      if (viewCountEl.textContent !== txt) viewCountEl.textContent = txt;
    }
    // Once the visitor picks a shader, the address bar links straight to it
    const file = opt && opt.value;
    if (file && file !== lastFile) {
      if (lastFile !== null) {
        try {
          const u = new URL(location.href);
          u.searchParams.set('s', file.replace(/\.fs$/, ''));
          history.replaceState(null, '', u);
        } catch (e) {}
      }
      lastFile = file;
    }
  }
  setInterval(syncShaderName, 500);

  const prevBtn = document.getElementById('view-prev');
  const nextBtn = document.getElementById('view-next');
  if (prevBtn) prevBtn.addEventListener('click', () => stepShader(-1));
  if (nextBtn) nextBtn.addEventListener('click', () => stepShader(1));

  // --- Viewer / editor: visitors get the shader and three buttons; Edit brings the full bar ---
  const viewToggle = document.getElementById('view-toggle');
  function setViewing(on) {
    closePanel();
    document.body.classList.toggle('viewing', on);
    document.body.classList.remove('idle');
    if (viewToggle) {
      viewToggle.classList.toggle('active', !on);
      viewToggle.title = on ? 'Edit controls' : 'Back to viewing';
    }
    try { localStorage.setItem('sc3-edit', on ? '0' : '1'); } catch (e) {}
  }
  if (viewToggle) {
    viewToggle.classList.toggle('active', !document.body.classList.contains('viewing'));
    viewToggle.addEventListener('click', () => setViewing(!document.body.classList.contains('viewing')));
  }

  // --- Camera, one tap from the viewer: webcam + hand tracking (your hand becomes the pointer) ---
  const viewCam = document.getElementById('view-cam');
  const camSwitch = document.getElementById('cam-webcam-btn');
  if (viewCam && camSwitch) {
    const syncCam = () => viewCam.classList.toggle('active', camSwitch.classList.contains('active'));
    viewCam.addEventListener('click', () => {
      viewCam.classList.add('busy');
      camSwitch.click();
      setTimeout(() => viewCam.classList.remove('busy'), 2500);
    });
    new MutationObserver(syncCam).observe(camSwitch, { attributes: true, attributeFilter: ['class'] });
    syncCam();
  }

  // In the viewer the bar steps aside after a few still seconds; any movement brings it back
  let idleTimer = null;
  function wake() {
    document.body.classList.remove('idle');
    clearTimeout(idleTimer);
    idleTimer = setTimeout(() => {
      const bar = document.getElementById('topbar');
      if (!document.body.classList.contains('viewing') || openKey) return;
      if (bar && bar.matches(':hover')) return wake();
      document.body.classList.add('idle');
    }, 3500);
  }
  ['pointermove', 'pointerdown', 'keydown', 'touchstart'].forEach(t =>
    window.addEventListener(t, wake, { passive: true, capture: true }));
  wake();

  // --- Canvas size chip: shows the current shape, opens the size picker ---
  const sizeSel = document.getElementById('canvas-size-select');
  const aspectEl = document.getElementById('tb-aspect-label');
  function syncAspect() {
    if (!sizeSel || !aspectEl) return;
    const m = /^(\d+)x(\d+)$/.exec(sizeSel.value);
    let label = 'Size';
    if (m) {
      const w = +m[1], h = +m[2];
      const known = { '1.78': '16:9', '0.56': '9:16', '1.00': '1:1' };
      label = known[(w / h).toFixed(2)] || (w / h > 3 ? 'Wide' : w + '×' + h);
    }
    aspectEl.textContent = label;
  }
  if (sizeSel) sizeSel.addEventListener('change', syncAspect);
  syncAspect();

  // Once the first shader is on screen: honour ?s=<shader>, set the phone
  // editor to vertical 9:16, then fade the canvas in.
  const _isMobileUI = window.innerWidth <= 900 || /Mobi|Android|iPhone/i.test(navigator.userAgent);
  function reveal() { document.body.classList.remove('booting'); }
  // A visitor sees the shader alone: the demo text layer starts switched off
  // (Edit > Text > eye turns it back on).
  const _viewAtBoot = document.body.classList.contains('viewing');
  const _bootAt = Date.now();
  let _textTouched = false;
  const textEye = document.querySelector('.layer-vis[data-layer="text"]');
  if (textEye) textEye.addEventListener('click', (e) => { if (e.isTrusted) _textTouched = true; });
  function quietText() {
    const hide = () => {
      const T = window.getLayer && window.getLayer('text');
      if (T && T.visible && textEye && !_textTouched) textEye.click();
    };
    hide();
    const guard = setInterval(hide, 300);
    setTimeout(() => clearInterval(guard), 4000);
  }
  const readyPoll = setInterval(() => {
    const L = window.getLayer && window.getLayer('shader');
    if (!(L && (L.program || L._fluidActive))) return;
    const T = window.getLayer('text');
    if (_viewAtBoot && !(T && T.program) && Date.now() - _bootAt < 4000) return;
    clearInterval(readyPoll);
    if (_viewAtBoot) quietText();
    let wanted = null;
    try { wanted = new URL(location.href).searchParams.get('s'); } catch (e) {}
    const sel = shaderSel();
    const opt = wanted && sel && Array.from(sel.options).find(o => o.value === wanted + '.fs' || o.value === wanted);
    if (opt && sel.value !== opt.value) {
      sel.value = opt.value;
      sel.dispatchEvent(new Event('change', { bubbles: true }));
      setTimeout(reveal, 900);
    } else {
      reveal();
    }
    // Viewer fills the phone edge to edge; the editor frames a vertical canvas
    if (_isMobileUI && sizeSel && !document.body.classList.contains('viewing') &&
        window.innerHeight > window.innerWidth) {
      sizeSel.value = '1080x1920';
      sizeSel.dispatchEvent(new Event('change', { bubbles: true }));
    }
  }, 200);
  setTimeout(() => { clearInterval(readyPoll); reveal(); }, 8000);

  // --- Fullscreen fallback: iOS Safari has no element fullscreen, so hide
  // the chrome instead. Registered before app.js, so it can pre-empt it. ---
  const fsBtn = document.getElementById('fs-btn');
  const previewEl = document.getElementById('preview');
  if (fsBtn) fsBtn.addEventListener('click', (e) => {
    if (previewEl && typeof previewEl.requestFullscreen === 'function') return;
    e.stopImmediatePropagation();
    closePanel();
    document.body.classList.add('sc3-fs');
  });
  const fsExit = document.getElementById('fs-exit');
  if (fsExit) fsExit.addEventListener('click', () => document.body.classList.remove('sc3-fs'));

  // --- Keyboard shortcuts ---
  document.addEventListener('keydown', (e) => {
    if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.tagName === 'SELECT') return;
    if (e.target.closest && e.target.closest('.CodeMirror')) return;
    if (e.ctrlKey || e.metaKey || e.altKey) return;

    if (e.key === 'g') togglePanel('tab:gallery');
    if (e.key === 'e') togglePanel('tab:assets'); // code lives inside assets
    if (e.key === 'p' && !document.body.classList.contains('viewing')) togglePanel('shader-layer');
    if (e.key === 'ArrowRight') stepShader(1);
    if (e.key === 'ArrowLeft') stepShader(-1);
    if (e.key === 'Escape') {
      if (document.body.classList.contains('sc3-fs')) document.body.classList.remove('sc3-fs');
      else if (openKey) closePanel();
      else if (!document.body.classList.contains('viewing')) openPanel(lastKey);
    }
  });

  // --- Expose for other modules (backward compat) ---
  function showSourceSection(sectionId) {
    const map = { gallery: 'tab:gallery', code: 'tab:assets', assets: 'tab:assets' };
    openPanel(map[sectionId] || 'shader-layer');
  }

  function showAllSourceSections() {
    openPanel('shader-layer');
  }

  function updateToolbarState() {}

  window._toolbarStrip = {
    showSourceSection,
    showAllSourceSections,
    updateToolbarState,
    openPanel,
    closePanel,
  };
})();
