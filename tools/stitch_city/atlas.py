# Atlas for ny_stitch_city: 8x12 tiles of 64 px (512x768).
#   R = glyph mask, G = colour hint * 20 (subway line), B = Manhattan map over the whole sheet (1 land, 0.5 Central Park).
import json, math
from PIL import Image, ImageDraw, ImageFont, ImageFilter
EMAP = json.load(open('emojimap.json')); FD = '../fontdl/package/files/'
T = 64; COND = '/usr/share/fonts/truetype/dejavu/DejaVuSansCondensed-Bold.ttf'
BOLD = '/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf'; REG = '/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf'
emoji = list("🗽🚕🍕🍎🥯🌭☕🏙🌉🚇🐀🕊🎷🗞💵🚦🏀⚾🌃🚒🎭🎤🚲🏢🔥🎸🚶✋")
bullets = [("A",1),("C",1),("E",1),("1",2),("2",2),("3",2),("4",3),("5",3),("6",3),("7",4),("N",5),("Q",5),("B",6),("F",6),("L",7),("G",8)]
words = ["NY","NYC","BK","BX","QNS","SI","212","718","$1","DELI","TAXI","WALK","EXIT","24H"]
signs = ["★","♥","→","↑","✕","ONEWAY"]
emoji2 = list("🚓🚑🎨🧢👟🥨🥟🍜🍔🛍🌳🎹🎺🎶💃🕺☔⚡🎡🍩")
words2 = ["SOHO","DUMBO","NOHO","UWS","LES","HEY!","BODEGA","OK!"]
signs2 = ["♛","☮","♪","☺"]
def fit_text(d, box, s, path, maxsz=60):
    x0, y0, x1, y1 = box
    for sz in range(maxsz, 6, -1):
        f = ImageFont.truetype(path, sz); b = d.textbbox((0, 0), s, font=f)
        if b[2]-b[0] <= x1-x0 and b[3]-b[1] <= y1-y0: break
    w, h = b[2]-b[0], b[3]-b[1]
    d.text(((x0+x1-w)/2 - b[0], (y0+y1-h)/2 - b[1]), s, font=f, fill=255)
def big(): im = Image.new('L', (2*T, 2*T), 0); return im, ImageDraw.Draw(im)
def down(im): return im.resize((T, T), Image.LANCZOS)
tiles = []
def add_emoji(lst):
    for ch in lst:
        im, d = big(); fit_text(d, (6, 6, 2*T-6, 2*T-6), ch, FD + EMAP[ch], 120)
        tiles.append((down(im.filter(ImageFilter.MaxFilter(5))), 0))
def add_words(lst):
    for w in lst:
        im, d = big(); fit_text(d, (6, 24, 2*T-6, 2*T-24), w, COND, 110); tiles.append((down(im), 0))
def add_signs(lst, font):
    for s in lst:
        im, d = big()
        if s == "ONEWAY":
            d.rectangle((4, 34, 2*T-4, 2*T-34), fill=255)
            d.polygon([(14, 64), (40, 44), (40, 56), (112, 56), (112, 72), (40, 72), (40, 84)], fill=0)
        else: fit_text(d, (10, 10, 2*T-10, 2*T-10), s, font, 120)
        tiles.append((down(im), 0))
add_emoji(emoji)
for ch, c in bullets:
    im, d = big(); d.ellipse((8, 8, 2*T-8, 2*T-8), fill=255)
    m = Image.new('L', (2*T, 2*T), 0); fit_text(ImageDraw.Draw(m), (36, 30, 2*T-36, 2*T-30), ch, COND, 90)
    im.paste(0, (0, 0), m); tiles.append((down(im), c))
add_words(words); add_signs(signs, BOLD)
add_emoji(emoji2); add_words(words2); add_signs(signs2, REG)
assert len(tiles) == 96, len(tiles)
W, H = 8*T, 12*T
R = Image.new('L', (W, H), 0); G = Image.new('L', (W, H), 0)
for i, (im, c) in enumerate(tiles):
    x, y = (i % 8) * T, (i // 8) * T
    R.paste(im, (x, y)); G.paste(c * 20, (x, y, x+T, y+T))
# Manhattan, hand-traced in island coords (x across, y = 0 at the Battery .. 1 at Inwood), turned 29 degrees like the grid
west = [(0.00,0.00),(-0.035,0.02),(-0.06,0.05),(-0.075,0.10),(-0.085,0.16),(-0.09,0.24),(-0.095,0.33),(-0.10,0.42),(-0.10,0.50),
        (-0.095,0.58),(-0.09,0.66),(-0.085,0.74),(-0.075,0.82),(-0.065,0.88),(-0.055,0.93),(-0.04,0.97),(-0.02,1.0)]
east = [(0.01,0.995),(0.03,0.97),(0.035,0.93),(0.045,0.89),(0.06,0.85),(0.07,0.81),(0.085,0.77),(0.095,0.72),(0.10,0.67),(0.095,0.62),
        (0.09,0.56),(0.085,0.50),(0.09,0.44),(0.10,0.38),(0.115,0.32),(0.13,0.26),(0.135,0.21),(0.125,0.17),(0.105,0.13),(0.085,0.09),
        (0.06,0.05),(0.035,0.02)]
park = [(-0.035,0.555),(0.03,0.555),(0.03,0.755),(-0.035,0.755)]
a = math.radians(29)
def rot(p): x, y = p; return (x*math.cos(a) + y*math.sin(a), -x*math.sin(a) + y*math.cos(a))
isl = [rot(p) for p in west + east]; pk = [rot(p) for p in park]
xs = [p[0] for p in isl]; ys = [p[1] for p in isl]
s = min((W-24)/(max(xs)-min(xs)), (H-24)/(max(ys)-min(ys)))
cx, cy = (max(xs)+min(xs))/2, (max(ys)+min(ys))/2
def px(p): return (W/2 + (p[0]-cx)*s, H/2 - (p[1]-cy)*s)
Bm = Image.new('L', (W*4, H*4), 0); bd = ImageDraw.Draw(Bm)
bd.polygon([(x*4, y*4) for x, y in map(px, isl)], fill=255)
bd.polygon([(x*4, y*4) for x, y in map(px, pk)], fill=128)
B = Bm.resize((W, H), Image.BOX)
Image.merge('RGB', (R, G, B)).save('ny_stitch_city_atlas.png')
Image.merge('RGB', (R, G, B)).save('preview.png'); print('ok', s)
