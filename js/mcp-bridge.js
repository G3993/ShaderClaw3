// ============================================================
// ShaderClaw — NDI State and Helpers (Worker-based capture)
// ============================================================

let ndiReceiveEntry = null;
let ndiReceiveCanvas = null;
let ndiReceiveCtx = null;
let ndiSendingActive = false;
// Auto-start NDI only on localhost (Vercel has no server)
let _ndiAutoStartOnConnect = (typeof location !== 'undefined') && (location.hostname === 'localhost' || location.hostname === '127.0.0.1' || /^192\.168\.|^10\./.test(location.hostname));
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
  const compressed = view.getUint8(1); // 0x01 = JPEG
  const width = view.getUint32(2, true);
  const height = view.getUint32(6, true);

  if (compressed === 0x01) {
    // JPEG compressed — decode via createImageBitmap
    const jpegBlob = new Blob([new Uint8Array(data, 10)], { type: 'image/jpeg' });
    createImageBitmap(jpegBlob, { premultiplyAlpha: 'none' }).then(bitmap => {
      if (!ndiReceiveEntry) return;

      // Resize canvas if needed
      if (!ndiReceiveCanvas || ndiReceiveCanvas.width !== width || ndiReceiveCanvas.height !== height) {
        ndiReceiveCanvas = document.createElement('canvas');
        ndiReceiveCanvas.width = width;
        ndiReceiveCanvas.height = height;
        ndiReceiveCtx = ndiReceiveCanvas.getContext('2d');
        if (ndiReceiveEntry) ndiReceiveEntry.element = ndiReceiveCanvas;
      }

      // Draw bitmap to canvas (needed for GL upload)
      ndiReceiveCtx.drawImage(bitmap, 0, 0);
      bitmap.close();

      // Upload to GL texture (flip Y for correct orientation)
      if (ndiReceiveEntry.glTexture && glRef) {
        glRef.pixelStorei(glRef.UNPACK_FLIP_Y_WEBGL, true);
        glRef.bindTexture(glRef.TEXTURE_2D, ndiReceiveEntry.glTexture);
        if (ndiReceiveEntry._texW === width && ndiReceiveEntry._texH === height) {
          glRef.texSubImage2D(glRef.TEXTURE_2D, 0, 0, 0, glRef.RGBA, glRef.UNSIGNED_BYTE, ndiReceiveCanvas);
        } else {
          glRef.texImage2D(glRef.TEXTURE_2D, 0, glRef.RGBA, glRef.RGBA, glRef.UNSIGNED_BYTE, ndiReceiveCanvas);
          ndiReceiveEntry._texW = width;
          ndiReceiveEntry._texH = height;
        }
        glRef.pixelStorei(glRef.UNPACK_FLIP_Y_WEBGL, false);
      }

      // Three.js texture
      if (ndiReceiveEntry.threeTexture) {
        ndiReceiveEntry.threeTexture.image = ndiReceiveCanvas;
        ndiReceiveEntry.threeTexture.needsUpdate = true;
      }
    }).catch(() => {});
    return;
  }

  // Fallback: raw RGBA (uncompressed — legacy path)
  const pixels = new Uint8Array(data, 10);
  if (!ndiReceiveCanvas || ndiReceiveCanvas.width !== width || ndiReceiveCanvas.height !== height) {
    ndiReceiveCanvas = document.createElement('canvas');
    ndiReceiveCanvas.width = width;
    ndiReceiveCanvas.height = height;
    ndiReceiveCtx = ndiReceiveCanvas.getContext('2d');
    if (ndiReceiveEntry) ndiReceiveEntry.element = ndiReceiveCanvas;
  }

  if (ndiReceiveEntry.glTexture && glRef) {
    glRef.bindTexture(glRef.TEXTURE_2D, ndiReceiveEntry.glTexture);
    if (ndiReceiveEntry._texW === width && ndiReceiveEntry._texH === height) {
      glRef.texSubImage2D(glRef.TEXTURE_2D, 0, 0, 0, width, height, glRef.RGBA, glRef.UNSIGNED_BYTE, pixels);
    } else {
      glRef.texImage2D(glRef.TEXTURE_2D, 0, glRef.RGBA, width, height, 0, glRef.RGBA, glRef.UNSIGNED_BYTE, pixels);
      ndiReceiveEntry._texW = width;
      ndiReceiveEntry._texH = height;
    }
  }

  if (ndiReceiveEntry.threeTexture) {
    const imageData = new ImageData(new Uint8ClampedArray(pixels.buffer, pixels.byteOffset, pixels.byteLength), width, height);
    ndiReceiveCtx.putImageData(imageData, 0, 0);
    ndiReceiveEntry.threeTexture.image = ndiReceiveCanvas;
    ndiReceiveEntry.threeTexture.needsUpdate = true;
  }
}

function createNdiWorker() {
  const code = `
    let offscreen = null;
    let ctx = null;
    let pixelBuf = null;
    let headerBuf = new ArrayBuffer(9);
    let headerView = new DataView(headerBuf);
    headerView.setUint8(0, 0x02);

    self.onmessage = (e) => {
      const { bitmap, width, height, ndiWidth, ndiHeight } = e.data;
      // NDI output resolution — downscale for performance if canvas is huge
      const outW = ndiWidth || width;
      const outH = ndiHeight || height;

      if (!offscreen || offscreen.width !== outW || offscreen.height !== outH) {
        offscreen = new OffscreenCanvas(outW, outH);
        ctx = offscreen.getContext('2d', { alpha: false, willReadFrequently: true });
        pixelBuf = null;
      }
      ctx.drawImage(bitmap, 0, 0, outW, outH);
      bitmap.close();

      const imgData = ctx.getImageData(0, 0, outW, outH);
      const pixelBytes = imgData.data.byteLength;

      // Build message: [header 9 bytes][pixels]
      // Reuse pixel buffer to reduce allocation
      if (!pixelBuf || pixelBuf.byteLength !== 9 + pixelBytes) {
        pixelBuf = new ArrayBuffer(9 + pixelBytes);
      }
      const view = new DataView(pixelBuf, 0, 9);
      view.setUint8(0, 0x02);
      view.setUint32(1, outW, true);
      view.setUint32(5, outH, true);
      new Uint8Array(pixelBuf, 9).set(imgData.data);
      self.postMessage(pixelBuf, [pixelBuf]);
      pixelBuf = null; // transferred
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
  const workers = [createNdiWorker(), createNdiWorker()]; // double-buffer
  _ndiSendWorkerPair = workers;
  ndiSendWorker = workers[0]; // ref for cleanup
  let workerIdx = 0;
  const inflight = [false, false];
  workers.forEach((w, i) => {
    w.onmessage = (e) => {
      inflight[i] = false;
      if (!ndiSendingActive) return;
      if (!ws || ws.readyState !== WebSocket.OPEN) return;
      // Allow up to 2 frames in WS buffer (adapts to large canvases like 8000x1800)
      const frameBytes = e.data.byteLength || e.data.length || 0;
      if (ws.bufferedAmount > Math.max(frameBytes * 2, 8 * 1024 * 1024)) return;
      ws.send(e.data);
      ndiSendFrameCount++;
    };
  });
  let lastCapture = 0;
  function captureLoop(timestamp) {
    if (!ndiSendingActive) return;
    ndiSendAnimId = requestAnimationFrame(captureLoop);
    // Target 30fps — skip frame if worker is still busy (natural backpressure)
    if (timestamp - lastCapture < 33) return;
    if (inflight[workerIdx]) return; // worker still busy — skip this frame
    lastCapture = timestamp;
    inflight[workerIdx] = true;
    const idx = workerIdx;
    workerIdx ^= 1; // alternate workers

    // NDI output resolution: cap at 1920x1080 for throughput
    // Canvas renders at full res (8000x1800), NDI sends scaled down
    const cw = canvasEl.width, ch = canvasEl.height;
    const maxNdiPixels = 1920 * 1080; // ~2M pixels max for smooth NDI
    const pixelCount = cw * ch;
    let ndiW = cw, ndiH = ch;
    if (pixelCount > maxNdiPixels) {
      const scale = Math.sqrt(maxNdiPixels / pixelCount);
      ndiW = Math.round(cw * scale);
      ndiH = Math.round(ch * scale);
      // Keep even dimensions (required by some NDI receivers)
      ndiW = ndiW & ~1;
      ndiH = ndiH & ~1;
    }

    createImageBitmap(canvasEl)
      .then(bitmap => {
        if (!ndiSendingActive) { bitmap.close(); inflight[idx] = false; return; }
        workers[idx].postMessage({ bitmap, width: cw, height: ch, ndiWidth: ndiW, ndiHeight: ndiH }, [bitmap]);
      })
      .catch(() => { inflight[idx] = false; });
  }
  ndiSendAnimId = requestAnimationFrame(captureLoop);
}

// Pause capture loop without changing UI (for WS disconnect/reconnect)
let _ndiSendWorkerPair = null;
function pauseNdiSend() {
  if (ndiSendAnimId) { cancelAnimationFrame(ndiSendAnimId); ndiSendAnimId = null; }
  if (_ndiSendWorkerPair) {
    _ndiSendWorkerPair.forEach(w => { try { w.terminate(); } catch(e) {} });
    _ndiSendWorkerPair = null;
  }
  if (ndiSendWorker) { try { ndiSendWorker.terminate(); } catch(e) {} ndiSendWorker = null; }
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
