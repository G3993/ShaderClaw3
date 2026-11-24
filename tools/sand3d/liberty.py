# Statue of Liberty, written in sand3d. Units: mm (about 1:100 — 930 mm from ground to torch).
import math, sys
import sand3d as s3d

def star_fort(r_out=210, r_in=150, points=11, h=40):
    pts = []
    for i in range(points * 2):
        a = math.pi * i / points + math.pi / 2
        r = r_out if i % 2 == 0 else r_in
        pts.append((r * math.cos(a), r * math.sin(a)))
    return s3d.extrude("fort", s3d.poly(pts), h)

def pedestal(z0):
    plinth = s3d.at(s3d.box("plinth", 190, 190, 60), z=z0)
    shaft = s3d.at(s3d.extrude("shaft", s3d.rect(150, 150, chamfer=8), 230, taper=0.12), z=z0 + 60)
    cap = s3d.at(s3d.box("cap", 150, 150, 26), z=z0 + 290)
    s3d.chamfer(cap, 6, edges="top")
    return s3d.join(plinth, shaft, cap)

def figure(z0):
    # robe: superellipse stations from the hem to the shoulders, drapery as displaced relief
    robe = s3d.loft("robe",
        s3d.station(z0 + 0,   118, 104, n=2.4),
        s3d.station(z0 + 40,  112, 96,  n=2.2),
        s3d.station(z0 + 150, 96,  78,  n=2.2),
        s3d.station(z0 + 230, 92,  66,  n=2.2, y=-2),
        s3d.station(z0 + 280, 100, 60,  n=2.6, y=-4),
        s3d.station(z0 + 300, 70,  48,  n=2.2, y=-4))
    s3d.displace(robe, 10, 34, noise="ridged", octaves=2, seed=7, direction="normal", fade=(0, 300))
    neck = s3d.at(s3d.cylinder("neck", 28, 26), y=-4, z=z0 + 296)
    head = s3d.at(s3d.sphere("head", 44), y=-4, z=z0 + 312)
    band = s3d.at(s3d.torus("band", 50, 9), y=-4, z=z0 + 336)
    rays = []
    for i in range(7):                                   # the seven rays, fanned over the front
        a = -78 + i * 26
        ray = s3d.cone("ray", 9, 46)
        rays.append(s3d.at(ray, x=25 * math.sin(math.radians(a)), y=-4 - 25 * math.cos(math.radians(a)), z=z0 + 338,
                           rx=-62 * math.cos(math.radians(a)), ry=-62 * math.sin(math.radians(a)) * -1))
    # right arm raised with the torch (statue's right = our -X when she faces -Y)
    arm = s3d.wire("arm", [(-34, -4, z0 + 282), (-44, -6, z0 + 330), (-52, -8, z0 + 390), (-56, -8, z0 + 430)], 30, fillet=20)
    torch_cup = s3d.at(s3d.cone("cup", 18, 22, d2=30), x=-56, y=-8, z=z0 + 430)
    flame = s3d.at(s3d.loft("flame", s3d.station(0, 26, 26), s3d.station(16, 30, 30), s3d.station(40, 4, 4)), x=-56, y=-8, z=z0 + 452)
    # left arm bent, holding the tablet against the body
    larm = s3d.wire("larm", [(34, -6, z0 + 276), (46, -14, z0 + 236), (36, -34, z0 + 214)], 26, fillet=14)
    tablet = s3d.at(s3d.box("tablet", 52, 12, 74), x=40, y=-40, z=z0 + 188, rx=8, ry=-14)
    body = s3d.join(robe, neck, head, band, rays, arm, torch_cup, larm, tablet)
    s3d.material(body, "copper", color="#5fa58d", roughness=0.55, metallic=0.4)
    s3d.material(flame, "light", color="#ffc24a", emission=8)
    return body, flame

def liberty():
    fort = star_fort()
    ped = pedestal(40)
    s3d.material(fort, "concrete", color="#8a857c"); s3d.material(ped, "concrete", color="#a39d92")
    statue, flame = figure(0)
    fig = s3d.group("figure", statue, flame)
    s3d.scale(fig, 1.45)                                  # the real figure is about as tall as its pedestal
    s3d.at(fig, z=40 + 316)
    return s3d.group("liberty", fort, ped, fig)

if __name__ == "__main__":
    out = sys.argv[1]
    with s3d.facets(48):
        root = liberty()
    s3d.export(root, out + ".glb")
    s3d.export(root, out + ".obj")
    print("exported", out)
