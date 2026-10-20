#!/usr/bin/env python3
"""Low-star queue: shaders Lu rated 1-2 stars in Easel's Gallery that do not
yet have a variation. Each one is a job: make a NEW variation shader
(<name>_v2.fs, new manifest id, same spirit, better), never overwrite the
original. Usage: low_star_queue.py [max_stars=2]"""
import json, os, sys
HOME = os.path.expanduser('~')
ratings = json.load(open(os.path.join(HOME, '.easel/shader_ratings.json')))
manifest = json.load(open(os.path.join(HOME, 'ShaderClaw3/shaders/manifest.json')))
files = {e['file'] for e in manifest}
titles = {e['file']: e['title'] for e in manifest}
usage = {}
try:
    u = json.load(open(os.path.join(HOME, '.easel/shader_usage.json')))
    usage = u if isinstance(u, dict) else {}
except Exception:
    pass
maxs = int(sys.argv[1]) if len(sys.argv) > 1 else 2
rows = []
for f, stars in ratings.items():
    if not isinstance(stars, (int, float)) or stars > maxs or stars <= 0:
        continue
    if f not in files:
        continue                       # deleted from the library
    stem = f[:-3]
    if any(x.startswith(stem + '_v') for x in files):
        continue                       # already has a variation
    rows.append((stars, -usage.get(f, usage.get(stem, 0)) if isinstance(usage.get(f, usage.get(stem, 0)), (int, float)) else 0, f))
rows.sort()
print(f'{len(rows)} low-star shaders without a variation (<= {maxs} stars):')
for stars, _, f in rows:
    print(f'  {int(stars)}*  {titles.get(f, f):28s} {f}')
