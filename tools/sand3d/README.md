# Sand3D → shader

`liberty.py` models the Statue of Liberty in [Sand3D](https://github.com/Sandflow-ai/sand3d) (Apache-2.0).
`bake.py` slices the exported mesh into a signed-distance atlas that `shaders/ny_liberty.fs` raymarches.

    uv venv -p 3.13 .venv && uv pip install -p .venv sand3d trimesh scipy pillow rtree
    .venv/bin/python liberty.py out/liberty          # writes out/liberty.glb and .obj
    python bake.py out/liberty.obj ../../shaders/ny_liberty_sdf.png

Atlas: 192 slices up Y (0–1.08 m), each 80×80 over x,z ∈ [−0.22, 0.22] m, 16 tiles per row (1280×960);
value 0.5 = surface, ±0.5 = ±6 cm.
