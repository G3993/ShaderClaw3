# Statue of Liberty figure (heel to torch), written in sand3d. mm; 700 mm tall (~1:66). Faces -Y.
# Proportions from the statue: head ~5.3 m of 46 m, tablet 7.2 x 4.1 m, torch arm 12.8 m.
import math, sys
import numpy as np
import sand3d as s3d

def pleats(x, y, z):
    th = np.arctan2(y, x)
    sway = np.sin(z * 0.012) * 0.6
    v = np.sin(th * 15.0 + sway) * 0.75 + np.sin(th * 7.0 - 1.3 + sway * 0.5) * 0.25
    return np.clip(v, -1.0, 1.0)

def robe():
    r = s3d.loft("robe",
        s3d.station(0,   172, 150, n=2.3),
        s3d.station(25,  166, 146, n=2.3),
        s3d.station(120, 150, 126, n=2.2),
        s3d.station(220, 138, 108, n=2.2, y=2),
        s3d.station(300, 140, 100, n=2.3, y=2),
        s3d.station(365, 150, 96,  n=2.6, y=4),
        s3d.station(398, 142, 86,  n=2.6, y=4),
        s3d.station(418, 84,  64,  n=2.2, y=4))
    s3d.displace(r, 9, 22, noise=pleats, direction="normal", fade=(40, 400))
    # the mantle: a heavy band from the left shoulder across the chest to the right hip
    m = s3d.at(s3d.sphere("mantle", 60), x=8, y=-48, z=318, ry=-38, scale=(0.5, 0.42, 2.1))
    return s3d.join(r, m)

def head_and_crown():
    neck = s3d.at(s3d.cylinder("neck", 42, 40), y=4, z=394)
    head = s3d.at(s3d.sphere("head", 72), y=2, z=424)                      # centre ~460
    bun = s3d.at(s3d.sphere("bun", 46), y=30, z=432)
    band = s3d.at(s3d.torus("diadem", 80, 13), y=2, z=470)
    rays = []
    for i in range(7):                                                  # seven rays fanned like sunbeams
        az = -78 + i * 26
        el = 22 + 10 * math.cos(math.radians(az))                       # the front rays stand up a little more
        c = s3d.cone("ray", 17, 84)
        px = 40 * math.sin(math.radians(az)); py = 2 - 40 * math.cos(math.radians(az))
        rays.append(s3d.at(c, x=px, y=py, z=468, rx=90 - el, rz=az))
    return s3d.join(neck, head, bun, band, rays)

def torch_arm():
    upper = s3d.wire("upper", [(-58, 0, 396), (-78, -6, 448), (-88, -10, 492)], 36, fillet=20)
    fore = s3d.wire("fore", [(-88, -10, 492), (-86, -16, 548), (-82, -22, 600)], 28, fillet=16)
    sleeve = s3d.at(s3d.sphere("sleeve", 62), x=-80, y=-4, z=402, scale=(0.55, 0.5, 1.2))   # the sleeve hanging off the arm
    handle = s3d.at(s3d.cylinder("handle", 17, 34), x=-82, y=-22, z=592)
    cup = s3d.at(s3d.cone("cup", 20, 22, d2=44), x=-82, y=-22, z=622)
    rim = s3d.at(s3d.torus("rim", 46, 7), x=-82, y=-22, z=642)
    return s3d.join(upper, fore, sleeve, handle, cup, rim)

def flame():
    f = s3d.loft("flame", s3d.station(0, 30, 30), s3d.station(16, 40, 40), s3d.station(36, 30, 30), s3d.station(56, 2, 2))
    return s3d.at(f, x=-82, y=-22, z=644)

def tablet_arm():
    arm = s3d.wire("larm", [(58, 0, 392), (82, -14, 330), (62, -48, 298)], 32, fillet=18)
    tab = s3d.at(s3d.box("tablet", 64, 15, 110), x=78, y=-40, z=262, rx=12, ry=-16, rz=14)
    return s3d.join(arm, tab)

def figure():
    body = s3d.join(robe(), head_and_crown(), torch_arm(), tablet_arm())
    s3d.material(body, "copper", color="#5fa58d")
    fl = flame(); s3d.material(fl, "light", color="#ffc24a", emission=6)
    return s3d.group("liberty_figure", body, fl)

if __name__ == "__main__":
    with s3d.facets(64):
        root = figure()
    s3d.export(root, sys.argv[1] + ".glb"); s3d.export(root, sys.argv[1] + ".obj")
    print("exported")
