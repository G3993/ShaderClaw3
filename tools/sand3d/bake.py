import trimesh, numpy as np, sys
from scipy import ndimage
from PIL import Image
m = trimesh.load(sys.argv[1], force='mesh')
NX = NZ = 80; NY = 192
X0, X1, Y0, Y1 = -0.22, 0.22, 0.0, 1.08
xs = X0 + (np.arange(NX) + 0.5) * (X1 - X0) / NX
ys = Y0 + (np.arange(NY) + 0.5) * (Y1 - Y0) / NY
zs = X0 + (np.arange(NZ) + 0.5) * (X1 - X0) / NZ
from PIL import ImageDraw
inside = np.zeros((NX, NY, NZ), bool)
SS = 4                                                    # supersample each slice, then average
for k, y in enumerate(ys):
    sec = m.section(plane_origin=[0, y, 0], plane_normal=[0, 1, 0])
    if sec is None: continue
    W, H = NX * SS, NZ * SS
    img = Image.new("L", (W, H), 0); dr = ImageDraw.Draw(img)
    # even-odd fill: XOR every closed loop of the cross-section
    acc = np.zeros((H, W), np.uint8)
    for loop in sec.discrete:
        if len(loop) < 3: continue
        px = (loop[:, 0] - X0) / (X1 - X0) * W; pz = (loop[:, 2] - X0) / (X1 - X0) * H
        li = Image.new("L", (W, H), 0); ImageDraw.Draw(li).polygon(list(zip(px, pz)), fill=1)
        acc ^= np.asarray(li, np.uint8)
    occ = acc.reshape(NZ, SS, NX, SS).mean(axis=(1, 3)) > 0.5    # [z, x]
    inside[:, k, :] = occ.T
h = np.array([(X1 - X0) / NX, (Y1 - Y0) / NY, (X1 - X0) / NZ])
d = ndimage.distance_transform_edt(~inside, sampling=h) - ndimage.distance_transform_edt(inside, sampling=h)
d = d + np.where(inside, 0.5, -0.5) * h.mean()          # voxel-centre bias: put the zero between cells
d = ndimage.gaussian_filter(d, 0.6)                     # soften the voxel stair-steps
R = 0.06                                                # ±6 cm stored; the shader steps by the box outside that
v = np.clip(0.5 + d / (2 * R), 0, 1)
TX = 16; TY = (NY + TX - 1) // TX
atlas = np.zeros((TY * NZ, TX * NX), np.uint8)
for k in range(NY):
    tx, ty = k % TX, k // TX
    sl = (v[:, k, :] * 255 + 0.5).astype(np.uint8)        # [x, z]
    atlas[ty * NZ:(ty + 1) * NZ, tx * NX:(tx + 1) * NX] = sl.T   # rows = z, cols = x
Image.fromarray(atlas, 'L').convert('RGB').save(sys.argv[2])
print('inside voxels', inside.sum(), 'atlas', atlas.shape, 'min d', d.min(), 'max d', d.max())
