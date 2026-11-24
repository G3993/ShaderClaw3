# Sand3D → shader

`liberty.py` models the Statue of Liberty's figure (heel to torch, 700 mm) in [Sand3D](https://github.com/Sandflow-ai/sand3d) (Apache-2.0).
`bake.py` slices the exported mesh into a signed-distance atlas that `shaders/ny_liberty.fs` raymarches.

    uv venv -p 3.13 .venv && uv pip install -p .venv sand3d trimesh scipy pillow rtree
    .venv/bin/python liberty.py out/liberty          # writes out/liberty.glb and .obj
    python bake.py out/liberty.obj ../../shaders/ny_liberty_sdf.png

Atlas: 256 slices up Y (0–0.72 m), each 96×96 over x,z ∈ [−0.13, 0.13] m, 16 tiles per row (1536×1536);
value 0.5 = surface, ±0.5 = ±4 cm. The shader stands her on top of an analytic pedestal and eleven-point
star fort (pedestal top at y = 0.356 m), so only the figure is baked.
