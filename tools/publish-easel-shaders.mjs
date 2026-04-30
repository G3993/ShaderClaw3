#!/usr/bin/env node
import crypto from "node:crypto";
import fs from "node:fs/promises";
import path from "node:path";

const DEFAULT_PROJECT = "etherea-aa67d";
const DEFAULT_DATABASE = "(default)";
const DEFAULT_COLLECTION = "easel_shaders";
const DATASTORE_SCOPE = "https://www.googleapis.com/auth/datastore";
const TOKEN_URL = "https://oauth2.googleapis.com/token";

function usage() {
  console.log(`Publish ShaderClaw shaders to Easel's Firestore shader library.

Usage:
  node tools/publish-easel-shaders.mjs [options]

Options:
  --source-dir <path>     Shader directory (default: shaders)
  --shader <file>         Publish one shader file instead of the full set
  --project <id>          Firebase project (default: ${DEFAULT_PROJECT})
  --database <id>         Firestore database (default: ${DEFAULT_DATABASE})
  --collection <name>     Firestore collection (default: ${DEFAULT_COLLECTION})
  --credentials <path>    Firebase service account JSON
  --status <status>       Published status value (default: published)
  --prune                 Delete Firestore docs not in the full source set
  --dry-run               Print what would publish without writing
  --help                  Show this help

Credentials default to GOOGLE_APPLICATION_CREDENTIALS.`);
}

function parseArgs(argv) {
  const out = {};
  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i];
    if (!arg.startsWith("--")) {
      throw new Error(`Unexpected argument: ${arg}`);
    }
    const eq = arg.indexOf("=");
    const key = arg.slice(2, eq > 0 ? eq : undefined);
    if (["dry-run", "prune", "help"].includes(key)) {
      out[key] = true;
      continue;
    }
    if (eq > 0) {
      out[key] = arg.slice(eq + 1);
      continue;
    }
    const value = argv[i + 1];
    if (!value || value.startsWith("--")) {
      throw new Error(`Missing value for --${key}`);
    }
    out[key] = value;
    i += 1;
  }
  return out;
}

function slug(value) {
  const withoutExt = value.trim().toLowerCase().replace(/\.(fs|frag|glsl)$/i, "");
  const dashed = withoutExt.replace(/[^a-z0-9_-]+/g, "-").replace(/-+/g, "-").replace(/^-|-$/g, "");
  return dashed || "shader";
}

function parseIsfMetadata(source) {
  const start = source.indexOf("/*");
  const end = source.indexOf("*/", start + 2);
  if (start < 0 || end < 0) return {};
  let block = source.slice(start + 2, end).trim();
  const brace = block.indexOf("{");
  if (brace >= 0) block = block.slice(brace);
  try {
    return JSON.parse(block);
  } catch {
    return {};
  }
}

async function loadManifest(sourceDir) {
  const manifestPath = path.join(sourceDir, "manifest.json");
  try {
    const raw = await fs.readFile(manifestPath, "utf8");
    const parsed = JSON.parse(raw);
    const byFile = new Map();
    if (Array.isArray(parsed)) {
      for (const item of parsed) {
        if (item && typeof item === "object" && item.file) {
          byFile.set(item.file, item);
        }
      }
    }
    return byFile;
  } catch (err) {
    if (err && err.code === "ENOENT") return new Map();
    throw err;
  }
}

function isShaderFile(fileName) {
  return /\.(fs|frag|glsl)$/i.test(fileName);
}

async function exists(filePath) {
  try {
    await fs.access(filePath);
    return true;
  } catch {
    return false;
  }
}

async function shaderFiles(sourceDir, manifest, oneShader) {
  const byName = new Map();
  if (manifest.size > 0) {
    for (const [fileName, item] of manifest.entries()) {
      if (item.type === "scene") continue;
      const filePath = path.join(sourceDir, item.folder || "", fileName);
      if (isShaderFile(fileName) && await exists(filePath)) {
        byName.set(path.basename(filePath), filePath);
      }
    }
  }

  const entries = await fs.readdir(sourceDir, { withFileTypes: true });
  for (const entry of entries) {
    if (entry.isFile() && isShaderFile(entry.name)) {
      byName.set(entry.name, path.join(sourceDir, entry.name));
    }
  }

  if (oneShader) {
    const key = path.basename(oneShader);
    const filePath = byName.get(key) || path.resolve(sourceDir, oneShader);
    if (!isShaderFile(filePath) || !await exists(filePath)) {
      throw new Error(`Shader not found: ${oneShader}`);
    }
    return [filePath];
  }

  return [...byName.values()].sort((a, b) => path.basename(a).localeCompare(path.basename(b)));
}

function hasAudio(metadata) {
  const inputs = Array.isArray(metadata.INPUTS) ? metadata.INPUTS : [];
  return inputs.some((input) => input && (input.TYPE === "audio" || input.TYPE === "audioFFT"));
}

async function buildDoc(sourceDir, filePath, manifestItem, status) {
  const fragment = await fs.readFile(filePath, "utf8");
  const metadata = parseIsfMetadata(fragment);
  const file = path.basename(filePath);
  const id = slug(path.basename(file, path.extname(file)));
  const categories = manifestItem.categories || metadata.CATEGORIES || [];
  const description = manifestItem.description || metadata.DESCRIPTION || "";
  const title = manifestItem.title || metadata.DESCRIPTION || path.basename(file, path.extname(file));

  let vertex = "";
  const vertexPath = filePath.replace(/\.(fs|frag|glsl)$/i, ".vs");
  if (await exists(vertexPath)) {
    vertex = await fs.readFile(vertexPath, "utf8");
  }

  return {
    id,
    doc: {
      id,
      title,
      description,
      file,
      type: manifestItem.type || "generator",
      categories,
      status,
      enabled: true,
      hidden: Boolean(manifestItem.hidden),
      updatedAt: new Date().toISOString(),
      fragment,
      vertex,
      metadata,
      library_entry: {
        description,
        credit: metadata.CREDIT || "",
        categories,
        inputs: Array.isArray(metadata.INPUTS) ? metadata.INPUTS : [],
        has_audio: hasAudio(metadata),
        shader_type: manifestItem.type || "generator",
        source: "shaderclaw",
      },
      source: "shaderclaw",
      sourceId: manifestItem.id == null ? "" : String(manifestItem.id),
      sourcePath: path.relative(sourceDir, filePath).split(path.sep).join("/"),
    },
  };
}

function firestoreValue(value) {
  if (value === null || value === undefined) return { nullValue: null };
  if (typeof value === "string") return { stringValue: value };
  if (typeof value === "boolean") return { booleanValue: value };
  if (typeof value === "number") {
    if (Number.isInteger(value)) return { integerValue: String(value) };
    return { doubleValue: value };
  }
  if (Array.isArray(value)) {
    return { arrayValue: { values: value.map(firestoreValue) } };
  }
  if (typeof value === "object") {
    const fields = {};
    for (const [key, child] of Object.entries(value)) {
      fields[key] = firestoreValue(child);
    }
    return { mapValue: { fields } };
  }
  return { stringValue: String(value) };
}

function firestoreFields(doc) {
  const fields = {};
  for (const [key, value] of Object.entries(doc)) {
    fields[key] = firestoreValue(value);
  }
  return fields;
}

function base64UrlJson(value) {
  return Buffer.from(JSON.stringify(value)).toString("base64url");
}

async function accessToken(credentialsPath) {
  const raw = await fs.readFile(credentialsPath, "utf8");
  const serviceAccount = JSON.parse(raw);
  if (!serviceAccount.client_email || !serviceAccount.private_key) {
    throw new Error("Credentials must be a Firebase/Google service account JSON file");
  }

  const now = Math.floor(Date.now() / 1000);
  const aud = serviceAccount.token_uri || TOKEN_URL;
  const header = base64UrlJson({ alg: "RS256", typ: "JWT" });
  const claim = base64UrlJson({
    iss: serviceAccount.client_email,
    scope: DATASTORE_SCOPE,
    aud,
    iat: now,
    exp: now + 3600,
  });
  const signer = crypto.createSign("RSA-SHA256");
  signer.update(`${header}.${claim}`);
  const signature = signer.sign(serviceAccount.private_key).toString("base64url");
  const assertion = `${header}.${claim}.${signature}`;

  const response = await fetch(aud, {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded" },
    body: new URLSearchParams({
      grant_type: "urn:ietf:params:oauth:grant-type:jwt-bearer",
      assertion,
    }),
  });
  const text = await response.text();
  if (!response.ok) {
    throw new Error(`OAuth token request failed (${response.status}): ${text}`);
  }
  const parsed = JSON.parse(text);
  return parsed.access_token;
}

async function firestoreRequest(token, url, options = {}) {
  const response = await fetch(url, {
    ...options,
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
      ...(options.headers || {}),
    },
  });
  const text = await response.text();
  if (!response.ok) {
    throw new Error(`Firestore request failed (${response.status}): ${text}`);
  }
  return text ? JSON.parse(text) : {};
}

function databaseRoot(project, database) {
  return `projects/${project}/databases/${database}`;
}

function documentsBase(project, database) {
  return `https://firestore.googleapis.com/v1/${databaseRoot(project, database)}/documents`;
}

function documentName(project, database, collection, id) {
  return `${databaseRoot(project, database)}/documents/${collection}/${id}`;
}

async function batchWrite(token, project, database, writes) {
  const chunkSize = 20;
  let completed = 0;
  for (let i = 0; i < writes.length; i += chunkSize) {
    const chunk = writes.slice(i, i + chunkSize);
    const url = `${documentsBase(project, database)}:batchWrite`;
    const result = await firestoreRequest(token, url, {
      method: "POST",
      body: JSON.stringify({ writes: chunk }),
    });
    const statuses = result.status || [];
    const failed = statuses.find((status) => status && status.code);
    if (failed) {
      throw new Error(`Firestore batch write failed: ${JSON.stringify(failed)}`);
    }
    completed += chunk.length;
  }
  return completed;
}

async function listDocumentIds(token, project, database, collection) {
  const ids = new Set();
  let pageToken = "";
  do {
    const url = new URL(`${documentsBase(project, database)}/${collection}`);
    url.searchParams.set("pageSize", "500");
    if (pageToken) url.searchParams.set("pageToken", pageToken);
    const result = await firestoreRequest(token, url.toString(), { method: "GET" });
    for (const doc of result.documents || []) {
      const name = doc.name || "";
      ids.add(name.slice(name.lastIndexOf("/") + 1));
    }
    pageToken = result.nextPageToken || "";
  } while (pageToken);
  return ids;
}

async function main() {
  const args = parseArgs(process.argv.slice(2));
  if (args.help) {
    usage();
    return;
  }
  if (args.prune && args.shader) {
    throw new Error("--prune is only allowed when publishing the full shader set");
  }

  const sourceDir = path.resolve(args["source-dir"] || "shaders");
  const project = args.project || DEFAULT_PROJECT;
  const database = args.database || DEFAULT_DATABASE;
  const collection = args.collection || DEFAULT_COLLECTION;
  const status = args.status || "published";
  const credentialsPath = args.credentials || process.env.GOOGLE_APPLICATION_CREDENTIALS;

  const manifest = await loadManifest(sourceDir);
  const files = await shaderFiles(sourceDir, manifest, args.shader);
  const docs = [];
  for (const filePath of files) {
    docs.push(await buildDoc(sourceDir, filePath, manifest.get(path.basename(filePath)) || {}, status));
  }

  console.log(`Prepared ${docs.length} shader${docs.length === 1 ? "" : "s"} for ${project}/${collection}`);
  for (const { id, doc } of docs.slice(0, 10)) {
    console.log(`  ${id}: ${doc.title} (${doc.file})`);
  }
  if (docs.length > 10) console.log(`  ... ${docs.length - 10} more`);

  if (args["dry-run"]) return;
  if (!credentialsPath) {
    throw new Error("Set GOOGLE_APPLICATION_CREDENTIALS or pass --credentials");
  }

  const token = await accessToken(path.resolve(credentialsPath));
  const writes = docs.map(({ id, doc }) => ({
    update: {
      name: documentName(project, database, collection, id),
      fields: firestoreFields(doc),
    },
  }));
  const written = await batchWrite(token, project, database, writes);
  console.log(`Wrote ${written} shader documents to ${collection}`);

  if (args.prune) {
    const keep = new Set(docs.map(({ id }) => id));
    const existing = await listDocumentIds(token, project, database, collection);
    const deletes = [...existing]
      .filter((id) => !keep.has(id))
      .map((id) => ({ delete: documentName(project, database, collection, id) }));
    const pruned = deletes.length ? await batchWrite(token, project, database, deletes) : 0;
    console.log(`Pruned ${pruned} stale shader documents from ${collection}`);
  }
}

main().catch((err) => {
  console.error(err.message || err);
  process.exitCode = 1;
});
