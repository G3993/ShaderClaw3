# Publishing Shaders To Easel

ShaderClaw is the prototyping space. Easel owns the runtime shader library in
Firestore, so ready ShaderClaw shaders can be pushed to Easel without requiring
Easel clients to read this repository.

## Publish The Full Library

Use a Firebase service account that can write Firestore documents:

```sh
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/firebase-service-account.json
npm run publish:easel-shaders -- --prune
```

The default target is:

```text
project: etherea-aa67d
collection: easel_shaders
```

`--prune` removes Firestore shader documents that no longer exist in the
ShaderClaw source set. Use it for normal full-library publishes.

## Publish One Shader

```sh
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/firebase-service-account.json
npm run publish:easel-shaders -- --shader vaporwave_floral_shoppe.fs
```

Single-shader publishes update or create only that document. They intentionally
do not allow `--prune`.

## Dry Run

```sh
npm run publish:easel-shaders -- --dry-run
```

Useful options:

```text
--source-dir shaders
--project etherea-aa67d
--database "(default)"
--collection easel_shaders
--credentials /path/to/firebase-service-account.json
--status published
```

