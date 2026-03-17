// ============================================================
// ShaderClaw 3 — Tab Bar Controller
// Manages tabbed panel (Canvas / Assets / Gallery)
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
      const hiddenTile = document.getElementById(tileMap[type]);
      if (hiddenTile) hiddenTile.click();
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

  // Tab click handlers
  tabs.forEach(tab => {
    tab.addEventListener('click', () => activateTab(tab.dataset.tab));
  });

  // Show properties panel by default
  if (propertiesPanel) {
    propertiesPanel.classList.add('visible');
  }

  // --- Keyboard shortcuts ---
  document.addEventListener('keydown', (e) => {
    if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.tagName === 'SELECT') return;
    if (e.target.closest('.CodeMirror')) return;

    if (e.key === 'g' && !e.ctrlKey && !e.metaKey) {
      activateTab('gallery');
    }
    if (e.key === 'e' && !e.ctrlKey && !e.metaKey) {
      activateTab('assets'); // code is inside assets now
    }
    if (e.key === 'p' && !e.ctrlKey && !e.metaKey) {
      activateTab('canvas');
    }
    if (e.key === 'Escape') {
      if (propertiesPanel) propertiesPanel.classList.toggle('visible');
    }
  });

  // --- Expose for other modules (backward compat) ---
  function showSourceSection(sectionId) {
    const map = { gallery: 'gallery', code: 'assets', assets: 'assets' };
    const tabName = map[sectionId] || 'canvas';
    if (propertiesPanel) propertiesPanel.classList.add('visible');
    activateTab(tabName);
  }

  function showAllSourceSections() {
    activateTab('canvas');
  }

  function updateToolbarState() {}

  window._toolbarStrip = {
    showSourceSection,
    showAllSourceSections,
    updateToolbarState,
  };
})();
