// ============================================================
// ShaderClaw 3 — Toolbar Strip Controller
// Manages vertical toolbar + floating panel visibility
// ============================================================

(function initToolbarStrip() {
  const sourcesPanel = document.querySelector('.sc3-sources');
  const propertiesPanel = document.querySelector('.sc3-properties');
  const toolbarBtns = document.querySelectorAll('.sc3-tool[data-panel]');
  const headerGalleryBtn = document.getElementById('gallery-btn');
  const headerCodeBtn = document.getElementById('code-btn');

  // Track which sources sub-section is active
  let activeSourceSection = null; // 'gallery' | 'code' | 'assets'

  // Show properties panel by default (has all layer controls)
  if (propertiesPanel) {
    propertiesPanel.classList.add('visible');
  }

  // --- Sources panel sub-section toggling ---
  function showSourceSection(sectionId) {
    if (!sourcesPanel) return;
    const sections = {
      gallery: document.getElementById('gallery-tab-content'),
      code: document.getElementById('editor-section'),
      assets: document.getElementById('inputs-section'),
    };

    // If clicking the same section, toggle off
    if (activeSourceSection === sectionId) {
      sourcesPanel.classList.remove('visible');
      activeSourceSection = null;
      updateToolbarState();
      return;
    }

    // Show the panel and the requested section
    sourcesPanel.classList.add('visible');
    activeSourceSection = sectionId;

    for (const [key, el] of Object.entries(sections)) {
      if (!el) continue;
      if (key === sectionId) {
        el.style.display = '';
        el.classList.remove('collapsed');
        const body = el.querySelector('.sc3-section-body');
        if (body) body.style.display = '';
      } else {
        el.style.display = 'none';
      }
    }

    updateToolbarState();
  }

  function showAllSourceSections() {
    if (!sourcesPanel) return;
    const sections = ['gallery-tab-content', 'editor-section', 'inputs-section'];
    sections.forEach(id => {
      const el = document.getElementById(id);
      if (el) {
        el.style.display = '';
        el.classList.remove('collapsed');
      }
    });
  }

  // --- Toolbar button state ---
  function updateToolbarState() {
    toolbarBtns.forEach(btn => {
      const panel = btn.dataset.panel;
      if (panel === 'properties') {
        btn.classList.toggle('active', propertiesPanel && propertiesPanel.classList.contains('visible'));
      } else if (panel === 'gallery' || panel === 'code' || panel === 'assets') {
        btn.classList.toggle('active', activeSourceSection === panel);
      }
    });

    // Header buttons
    if (headerGalleryBtn) headerGalleryBtn.classList.toggle('active', activeSourceSection === 'gallery');
    if (headerCodeBtn) headerCodeBtn.classList.toggle('active', activeSourceSection === 'code');
  }

  // --- Toolbar strip click handlers ---
  toolbarBtns.forEach(btn => {
    btn.addEventListener('click', () => {
      const panel = btn.dataset.panel;

      if (panel === 'properties') {
        if (propertiesPanel) {
          propertiesPanel.classList.toggle('visible');
        }
      } else if (panel === 'gallery' || panel === 'code' || panel === 'assets') {
        showSourceSection(panel);
      }

      updateToolbarState();
    });
  });

  // --- Header button handlers ---
  if (headerGalleryBtn) {
    headerGalleryBtn.addEventListener('click', () => showSourceSection('gallery'));
  }
  if (headerCodeBtn) {
    headerCodeBtn.addEventListener('click', () => showSourceSection('code'));
  }

  // --- Keyboard shortcuts for panel toggling ---
  document.addEventListener('keydown', (e) => {
    // Ctrl+G = Gallery, Ctrl+E = Code Editor (don't override if in input/editor)
    if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.tagName === 'SELECT') return;
    if (e.target.closest('.CodeMirror')) return;

    if (e.key === 'g' && !e.ctrlKey && !e.metaKey) {
      showSourceSection('gallery');
    }
    if (e.key === 'e' && !e.ctrlKey && !e.metaKey) {
      showSourceSection('code');
    }
    if (e.key === 'p' && !e.ctrlKey && !e.metaKey) {
      if (propertiesPanel) propertiesPanel.classList.toggle('visible');
      updateToolbarState();
    }
    // Escape closes all panels
    if (e.key === 'Escape') {
      if (activeSourceSection) {
        sourcesPanel.classList.remove('visible');
        activeSourceSection = null;
      }
      if (exportHub) exportHub.classList.remove('show');
      updateToolbarState();
    }
  });

  // Initial state
  updateToolbarState();

  // Expose for other modules
  window._toolbarStrip = {
    showSourceSection,
    showAllSourceSections,
    updateToolbarState,
  };
})();
