// ============================================================
// ShaderClaw 3 — Data Sources Module
// Live data feeds that drive shader parameters as signals
// Weather, RSS, JSON APIs, WebSocket, CSV, Clock, Expressions
// ============================================================

(function() {
  // Registry of active data sources
  const _sources = [];
  let _nextId = 1;

  // All data source values (read by binding system in state.js)
  // Keys are like 'data_weather_temp', 'data_rss_count', etc.
  const values = {};

  // --- Source Types ---

  const SOURCE_TYPES = {
    clock: {
      label: 'Clock',
      icon: '\u23F0',
      fields: [
        { key: 'hours',   fn: () => new Date().getHours() / 23 },
        { key: 'minutes', fn: () => new Date().getMinutes() / 59 },
        { key: 'seconds', fn: () => new Date().getSeconds() / 59 },
        { key: 'day',     fn: () => new Date().getDate() / 31 },
        { key: 'month',   fn: () => (new Date().getMonth()) / 11 },
        { key: 'dow',     fn: () => new Date().getDay() / 6 },
      ],
      create() {
        return { type: 'clock', interval: null };
      },
      start(src) {
        const update = () => {
          for (const f of SOURCE_TYPES.clock.fields) {
            values[`data_${src.id}_${f.key}`] = f.fn();
          }
          src._signals = SOURCE_TYPES.clock.fields.map(f => ({
            name: `Clock ${f.key}`, key: `data_${src.id}_${f.key}`
          }));
        };
        update();
        src.interval = setInterval(update, 1000);
      },
      stop(src) {
        if (src.interval) clearInterval(src.interval);
      },
      getSignals(src) {
        return SOURCE_TYPES.clock.fields.map(f => ({
          name: `Clock ${f.key}`, key: `data_${src.id}_${f.key}`
        }));
      }
    },

    weather: {
      label: 'Weather',
      icon: '\u2600',
      fields: ['temp', 'humidity', 'wind', 'clouds', 'pressure'],
      create(config) {
        return {
          type: 'weather',
          city: config.city || 'London',
          interval: null,
          raw: {},
        };
      },
      async start(src) {
        const fetchWeather = async () => {
          try {
            // Open-Meteo (no API key needed)
            const geo = await fetch(`https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(src.city)}&count=1`);
            const geoData = await geo.json();
            if (!geoData.results || !geoData.results.length) return;
            const { latitude, longitude } = geoData.results[0];
            const resp = await fetch(`https://api.open-meteo.com/v1/forecast?latitude=${latitude}&longitude=${longitude}&current_weather=true&hourly=relativehumidity_2m,surface_pressure,cloudcover`);
            const data = await resp.json();
            const cw = data.current_weather || {};
            const hour = new Date().getHours();
            src.raw = {
              temp: cw.temperature || 0,
              wind: cw.windspeed || 0,
              humidity: data.hourly?.relativehumidity_2m?.[hour] || 50,
              pressure: data.hourly?.surface_pressure?.[hour] || 1013,
              clouds: data.hourly?.cloudcover?.[hour] || 0,
            };
            // Normalize to 0-1 ranges
            values[`data_${src.id}_temp`] = Math.max(0, Math.min(1, (src.raw.temp + 20) / 60)); // -20 to 40C
            values[`data_${src.id}_humidity`] = src.raw.humidity / 100;
            values[`data_${src.id}_wind`] = Math.min(1, src.raw.wind / 50); // 0-50 km/h
            values[`data_${src.id}_clouds`] = src.raw.clouds / 100;
            values[`data_${src.id}_pressure`] = Math.max(0, Math.min(1, (src.raw.pressure - 960) / 80)); // 960-1040 hPa
          } catch (e) {
            console.warn('Weather fetch failed:', e);
          }
        };
        await fetchWeather();
        src.interval = setInterval(fetchWeather, 5 * 60 * 1000); // Every 5 min
      },
      stop(src) {
        if (src.interval) clearInterval(src.interval);
      },
      getSignals(src) {
        return SOURCE_TYPES.weather.fields.map(f => ({
          name: `Weather ${f}`, key: `data_${src.id}_${f}`
        }));
      }
    },

    json_api: {
      label: 'JSON API',
      icon: '{ }',
      create(config) {
        return {
          type: 'json_api',
          url: config.url || '',
          field: config.field || '',
          pollMs: (config.pollSeconds || 30) * 1000,
          normalize: config.normalize || { min: 0, max: 1 },
          interval: null,
        };
      },
      async start(src) {
        const fetchData = async () => {
          try {
            const resp = await fetch(src.url);
            const data = await resp.json();
            // Navigate to field path (e.g. "data.price" or "results[0].value")
            let val = data;
            for (const part of src.field.split('.')) {
              const m = part.match(/^(\w+)\[(\d+)\]$/);
              if (m) { val = val[m[1]][parseInt(m[2])]; }
              else { val = val[part]; }
              if (val == null) break;
            }
            const num = parseFloat(val);
            if (!isNaN(num)) {
              const range = src.normalize.max - src.normalize.min || 1;
              values[`data_${src.id}_value`] = Math.max(0, Math.min(1, (num - src.normalize.min) / range));
            }
          } catch (e) {
            console.warn('JSON API fetch failed:', e);
          }
        };
        await fetchData();
        if (src.pollMs > 0) src.interval = setInterval(fetchData, src.pollMs);
      },
      stop(src) {
        if (src.interval) clearInterval(src.interval);
      },
      getSignals(src) {
        return [{ name: `API ${src.field || 'value'}`, key: `data_${src.id}_value` }];
      }
    },

    websocket: {
      label: 'WebSocket',
      icon: '\u21C4',
      create(config) {
        return {
          type: 'websocket',
          url: config.url || '',
          field: config.field || '',
          ws: null,
        };
      },
      start(src) {
        try {
          src.ws = new WebSocket(src.url);
          src.ws.onmessage = (evt) => {
            try {
              const data = JSON.parse(evt.data);
              let val = data;
              if (src.field) {
                for (const part of src.field.split('.')) val = val?.[part];
              }
              const num = parseFloat(val);
              if (!isNaN(num)) {
                values[`data_${src.id}_value`] = Math.max(0, Math.min(1, num));
              }
            } catch (_) {}
          };
          src.ws.onerror = () => console.warn('WebSocket error:', src.url);
        } catch (e) {
          console.warn('WebSocket connect failed:', e);
        }
      },
      stop(src) {
        if (src.ws) { src.ws.close(); src.ws = null; }
      },
      getSignals(src) {
        return [{ name: `WS ${src.field || 'value'}`, key: `data_${src.id}_value` }];
      }
    },

    csv: {
      label: 'CSV File',
      icon: '\uD83D\uDCC4',
      create(config) {
        return {
          type: 'csv',
          data: config.data || [],
          colIndex: config.colIndex || 0,
          fps: config.fps || 30,
          loop: true,
        };
      },
      start(src) {
        // CSV data is already loaded into _dataManager in state.js
        if (src.data && src.data.length && typeof _dataManager !== 'undefined') {
          _dataManager.loadJSON(`src_${src.id}`, src.data);
        }
      },
      stop(src) {
        if (typeof _dataManager !== 'undefined') {
          delete _dataManager.csvData[`src_${src.id}`];
        }
      },
      getSignals(src) {
        return [{ name: 'CSV Value', key: `csv_src_${src.id}` }];
      }
    },

    expression: {
      label: 'Expression',
      icon: 'f(x)',
      create(config) {
        return {
          type: 'expression',
          expr: config.expr || 'Math.sin(t * 2)',
        };
      },
      start(src) {
        if (typeof _dataManager !== 'undefined') {
          const err = _dataManager.setExpression(`src_${src.id}`, src.expr);
          if (err) console.warn('Expression error:', err);
        }
      },
      stop(src) {
        if (typeof _dataManager !== 'undefined') {
          delete _dataManager.expressions[`src_${src.id}`];
        }
      },
      getSignals(src) {
        return [{ name: `Expr`, key: `expr_src_${src.id}` }];
      }
    },

    rss: {
      label: 'RSS Feed',
      icon: '\uD83D\uDCE1',
      create(config) {
        return {
          type: 'rss',
          url: config.url || '',
          interval: null,
          items: [],
          currentIndex: 0,
        };
      },
      async start(src) {
        const fetchRSS = async () => {
          try {
            // Use a CORS proxy for RSS feeds
            const proxyUrl = `https://api.rss2json.com/v1/api.json?rss_url=${encodeURIComponent(src.url)}`;
            const resp = await fetch(proxyUrl);
            const data = await resp.json();
            src.items = data.items || [];
            // Expose item count as a signal (normalized 0-1 based on max 100 items)
            values[`data_${src.id}_count`] = Math.min(1, src.items.length / 100);
            // Cycle through items every 10 seconds — expose index as signal
            values[`data_${src.id}_index`] = src.items.length ? src.currentIndex / src.items.length : 0;
            // Expose current item title as text (for text layer binding)
            if (src.items.length) {
              window._dataTexts = window._dataTexts || {};
              window._dataTexts[`data_${src.id}_title`] = src.items[src.currentIndex % src.items.length].title || '';
            }
          } catch (e) {
            console.warn('RSS fetch failed:', e);
          }
        };
        await fetchRSS();
        src.interval = setInterval(async () => {
          src.currentIndex = (src.currentIndex + 1) % Math.max(1, src.items.length);
          values[`data_${src.id}_index`] = src.items.length ? src.currentIndex / src.items.length : 0;
          if (src.items.length) {
            window._dataTexts = window._dataTexts || {};
            window._dataTexts[`data_${src.id}_title`] = src.items[src.currentIndex % src.items.length].title || '';
          }
        }, 10000);
        // Re-fetch every 5 min
        src._refetchInterval = setInterval(fetchRSS, 5 * 60 * 1000);
      },
      stop(src) {
        if (src.interval) clearInterval(src.interval);
        if (src._refetchInterval) clearInterval(src._refetchInterval);
      },
      getSignals(src) {
        return [
          { name: 'RSS Count', key: `data_${src.id}_count` },
          { name: 'RSS Index', key: `data_${src.id}_index` },
        ];
      }
    },
    image_feed: {
      label: 'Image Feed',
      icon: '\uD83D\uDDBC',
      create(config) {
        return {
          type: 'image_feed',
          feedName: config.feedName || 'default',
          pollMs: (config.pollSeconds || 5) * 1000,
          interval: null,
          images: [],
          textures: [],       // WebGL textures
          loadedPaths: new Set(),
          maxTextures: 16,
        };
      },
      async start(src) {
        const poll = async () => {
          try {
            const resp = await fetch(`/api/feeds/${src.feedName}`);
            const data = await resp.json();
            src.images = data.images || [];
            values[`data_${src.id}_count`] = Math.min(1, src.images.length / src.maxTextures);
            values[`data_${src.id}_total`] = src.images.length;
            // Load new images as textures
            for (let i = 0; i < Math.min(src.images.length, src.maxTextures); i++) {
              const path = src.images[src.images.length - 1 - i]; // newest first
              if (!src.loadedPaths.has(path)) {
                src.loadedPaths.add(path);
                const img = new Image();
                img.crossOrigin = 'anonymous';
                img.onload = () => {
                  src.textures[i] = { element: img, path, loaded: true };
                  values[`data_${src.id}_latest`] = i / src.maxTextures;
                  if (window._bus) window._bus.emit('feed:image', { sourceId: src.id, index: i, path, element: img });
                };
                img.src = path;
              }
            }
          } catch (e) {
            console.warn('Image feed poll failed:', e);
          }
        };
        await poll();
        src.interval = setInterval(poll, src.pollMs);
      },
      stop(src) {
        if (src.interval) clearInterval(src.interval);
      },
      getSignals(src) {
        return [
          { name: `Feed Count`, key: `data_${src.id}_count` },
          { name: `Feed Total`, key: `data_${src.id}_total` },
          { name: `Feed Latest`, key: `data_${src.id}_latest` },
        ];
      }
    },

    csv_timeseries: {
      label: 'CSV Time Series',
      icon: '\uD83D\uDCC8',
      create(config) {
        return {
          type: 'csv_timeseries',
          url: config.url || '',
          columns: [],      // parsed column names
          rows: [],          // parsed numeric rows
          playhead: 0,       // current row index (float for interpolation)
          speed: config.speed || 1.0,     // rows per second
          loop: config.loop !== false,
          playing: true,
          interval: null,
        };
      },
      async start(src) {
        // Load and parse CSV
        try {
          const resp = await fetch(src.url);
          const text = await resp.text();
          const lines = text.trim().split('\n');
          if (lines.length < 2) return;
          src.columns = lines[0].split(',').map(s => s.trim().replace(/^"|"$/g, ''));
          src.rows = [];
          for (let i = 1; i < lines.length; i++) {
            const cells = lines[i].split(',').map(s => s.trim().replace(/^"|"$/g, ''));
            const row = cells.map(c => {
              const n = parseFloat(c);
              return isNaN(n) ? 0 : n;
            });
            src.rows.push(row);
          }
          // Find min/max per column for normalization
          src._min = []; src._max = [];
          for (let c = 0; c < src.columns.length; c++) {
            let min = Infinity, max = -Infinity;
            for (const row of src.rows) {
              if (row[c] < min) min = row[c];
              if (row[c] > max) max = row[c];
            }
            src._min.push(min);
            src._max.push(max);
          }
          console.log(`CSV loaded: ${src.rows.length} rows, ${src.columns.length} columns: ${src.columns.join(', ')}`);
        } catch (e) {
          console.warn('CSV load failed:', e);
          return;
        }

        // Playback loop at 60fps
        const fps = 60;
        src.interval = setInterval(() => {
          if (!src.playing || src.rows.length === 0) return;
          src.playhead += src.speed / fps;
          if (src.playhead >= src.rows.length) {
            src.playhead = src.loop ? 0 : src.rows.length - 1;
          }

          // Interpolate between rows
          const idx = Math.floor(src.playhead);
          const frac = src.playhead - idx;
          const row0 = src.rows[idx];
          const row1 = src.rows[Math.min(idx + 1, src.rows.length - 1)];

          // Write normalized values for each column
          for (let c = 0; c < src.columns.length; c++) {
            const v = row0[c] * (1 - frac) + row1[c] * frac;
            const range = src._max[c] - src._min[c] || 1;
            values[`data_${src.id}_${src.columns[c]}`] = (v - src._min[c]) / range;
            values[`data_${src.id}_${src.columns[c]}_raw`] = v;
          }
          // Progress through dataset (0-1)
          values[`data_${src.id}_progress`] = src.playhead / src.rows.length;
          values[`data_${src.id}_year`] = values[`data_${src.id}_${src.columns[0]}_raw`] || 0;
        }, 1000 / fps);
      },
      stop(src) {
        if (src.interval) clearInterval(src.interval);
      },
      getSignals(src) {
        const signals = [
          { name: 'Progress', key: `data_${src.id}_progress` },
        ];
        for (const col of src.columns) {
          signals.push({ name: `CSV ${col}`, key: `data_${src.id}_${col}` });
        }
        return signals;
      }
    },
  };

  // --- Public API ---

  function addSource(type, config = {}) {
    const typeDef = SOURCE_TYPES[type];
    if (!typeDef) { console.warn('Unknown data source type:', type); return null; }
    const src = typeDef.create(config);
    src.id = _nextId++;
    src.label = config.label || `${typeDef.label} ${src.id}`;
    _sources.push(src);
    typeDef.start(src);
    if (window._bus) window._bus.emit('data:added', { source: src });
    return src;
  }

  function removeSource(id) {
    const idx = _sources.findIndex(s => s.id === id);
    if (idx < 0) return;
    const src = _sources[idx];
    const typeDef = SOURCE_TYPES[src.type];
    if (typeDef) typeDef.stop(src);
    // Clean up values
    const prefix = `data_${src.id}_`;
    for (const key of Object.keys(values)) {
      if (key.startsWith(prefix)) delete values[key];
    }
    _sources.splice(idx, 1);
    if (window._bus) window._bus.emit('data:removed', { id });
  }

  function getSignals() {
    const signals = [];
    for (const src of _sources) {
      const typeDef = SOURCE_TYPES[src.type];
      if (typeDef) {
        signals.push(...typeDef.getSignals(src));
      }
    }
    return signals;
  }

  function getSources() {
    return _sources.map(s => ({ ...s }));
  }

  function getValues() {
    return values;
  }

  // Expose globally
  window._dataSources = {
    SOURCE_TYPES,
    addSource,
    removeSource,
    getSignals,
    getSources,
    getValues,
    values,
  };
})();
