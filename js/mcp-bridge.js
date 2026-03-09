// ============================================================
// ShaderClaw — NDI State and Helpers (Worker-based capture)
// ============================================================

let ndiReceiveEntry = null;
let ndiReceiveCanvas = null;
let ndiReceiveCtx = null;
let ndiSendingActive = false;
let _ndiAutoStartOnConnect = true; // auto-start NDI send on first WS connect
let ndiSendAnimId = null;
let ndiSendFrameCount = 0;
let ndiSendWorker = null;
let ndiWsMsgId = 0;
const ndiPending = new Map();
const FRAME_TYPE_NDI_VIDEO = 0x01;
const FRAME_TYPE_CANVAS = 0x02;

function ndiRequest(ws, action, params = {}) {
  return new Promise((resolve, reject) => {
    if (!ws || ws.readyState !== WebSocket.OPEN) {
      return reject(new Error('WebSocket not connected'));
    }
    const id = --ndiWsMsgId;
    const timer = setTimeout(() => {
      ndiPending.delete(id);
      reject(new Error('NDI request timeout'));
    }, 5000);
    ndiPending.set(id, { resolve, reject, timer });
    ws.send(JSON.stringify({ id, action, params }));
  });
}

function handleNdiVideoFrame(data, glRef) {
  if (!ndiReceiveEntry) return;
  const view = new DataView(data);
  const width = view.getUint32(1, true);
  const height = view.getUint32(5, true);
  const pixels = new Uint8ClampedArray(data, 9);

  if (!ndiReceiveCanvas || ndiReceiveCanvas.width !== width || ndiReceiveCanvas.height !== height) {
    ndiReceiveCanvas = document.createElement('canvas');
    ndiReceiveCanvas.width = width;
    ndiReceiveCanvas.height = height;
    ndiReceiveCtx = ndiReceiveCanvas.getContext('2d');
  }

  const imageData = new ImageData(pixels, width, height);
  ndiReceiveCtx.putImageData(imageData, 0, 0);

  // Upload to GL texture — use UNPACK_FLIP_Y so NDI frames orient correctly
  if (ndiReceiveEntry.glTexture && glRef) {
    glRef.pixelStorei(glRef.UNPACK_FLIP_Y_WEBGL, true);
    glRef.bindTexture(glRef.TEXTURE_2D, ndiReceiveEntry.glTexture);
    glRef.texImage2D(glRef.TEXTURE_2D, 0, glRef.RGBA, glRef.RGBA, glRef.UNSIGNED_BYTE, ndiReceiveCanvas);
    glRef.pixelStorei(glRef.UNPACK_FLIP_Y_WEBGL, false);
  }

  if (ndiReceiveEntry.threeTexture) {
    ndiReceiveEntry.threeTexture.image = ndiReceiveCanvas;
    ndiReceiveEntry.threeTexture.needsUpdate = true;
  }
}

function createNdiWorker() {
  const code = `
    let offscreen = null;
    let ctx = null;
    self.onmessage = (e) => {
      const { bitmap, width, height } = e.data;
      if (!offscreen || offscreen.width !== width || offscreen.height !== height) {
        offscreen = new OffscreenCanvas(width, height);
        ctx = offscreen.getContext('2d', { alpha: false, willReadFrequently: true });
      }
      ctx.drawImage(bitmap, 0, 0, width, height);
      const imageData = ctx.getImageData(0, 0, width, height);
      bitmap.close();
      const msg = new Uint8Array(9 + imageData.data.length);
      const view = new DataView(msg.buffer, 0, 9);
      view.setUint8(0, 0x02);
      view.setUint32(1, width, true);
      view.setUint32(5, height, true);
      msg.set(imageData.data, 9);
      self.postMessage(msg.buffer, [msg.buffer]);
    };
  `;
  const blob = new Blob([code], { type: 'application/javascript' });
  return new Worker(URL.createObjectURL(blob));
}

function startNdiSend(ws, canvasEl) {
  if (ndiSendingActive) return;
  ndiSendingActive = true;
  document.getElementById('ndi-indicator')?.classList.add('active');
  // Update sidebar button + status dot
  const sendBtn = document.getElementById('ndi-send-btn');
  const statusDot = document.getElementById('ndi-status');
  if (sendBtn) sendBtn.textContent = 'Stop';
  if (statusDot) statusDot.classList.add('active');
  ndiSendFrameCount = 0;
  const NDI_W = 960, NDI_H = 540;
  const worker = createNdiWorker();
  ndiSendWorker = worker;
  worker.onmessage = (e) => {
    if (!ndiSendingActive) return;
    if (!ws || ws.readyState !== WebSocket.OPEN) return;
    if (ws.bufferedAmount > 4 * 1024 * 1024) return;
    ws.send(e.data);
    ndiSendFrameCount++;
  };
  let lastCapture = 0;
  let pending = false;
  let pendingStart = 0;
  function captureLoop(timestamp) {
    if (!ndiSendingActive) return;
    ndiSendAnimId = requestAnimationFrame(captureLoop);
    if (timestamp - lastCapture < 33) return;
    lastCapture = timestamp;
    // Safety: if pending stuck for >500ms (e.g. during shader compile), force reset
    if (pending && (timestamp - pendingStart > 500)) {
      pending = false;
    }
    if (pending) return;
    pending = true;
    pendingStart = timestamp;
    createImageBitmap(canvasEl, { resizeWidth: NDI_W, resizeHeight: NDI_H })
      .then(bitmap => {
        pending = false;
        if (!ndiSendingActive) { bitmap.close(); return; }
        worker.postMessage({ bitmap, width: NDI_W, height: NDI_H }, [bitmap]);
      })
      .catch(() => { pending = false; });
  }
  ndiSendAnimId = requestAnimationFrame(captureLoop);
}

// Pause capture loop without changing UI (for WS disconnect/reconnect)
function pauseNdiSend() {
  if (ndiSendAnimId) { cancelAnimationFrame(ndiSendAnimId); ndiSendAnimId = null; }
  if (ndiSendWorker) { ndiSendWorker.terminate(); ndiSendWorker = null; }
}

// Full stop — user-initiated, resets UI
function stopNdiSend() {
  ndiSendingActive = false;
  document.getElementById('ndi-indicator')?.classList.remove('active');
  // Update sidebar button + status dot
  const sendBtn = document.getElementById('ndi-send-btn');
  const statusDot = document.getElementById('ndi-status');
  if (sendBtn) sendBtn.textContent = 'Send';
  if (statusDot) statusDot.classList.remove('active');
  pauseNdiSend();
}
