# Glyph atlas for ny_city_skin: 8x8 tiles of 64 px. R = glyph mask, G = colour hint (MTA line colours for bullets).
from PIL import Image, ImageDraw, ImageFont, ImageFilter
import json; EMAP = json.load(open('emojimap.json'))
T = 64; EM = 'noto-emoji-0-700-normal.woff'
COND = '/usr/share/fonts/truetype/dejavu/DejaVuSansCondensed-Bold.ttf'
emoji = list("🗽🚕🍕🍎🥯🌭☕🏙🌉🚇🐀🕊🎷🗞💵🚦🏀⚾🌃🚒🎭🎤🚲🏢🔥🎆🚶✋")
# subway bullets: (letter, colour hint) 1 blue 2 red 3 green 4 purple 5 yellow 6 orange 7 grey 8 lime
bullets = [("A",1),("C",1),("E",1),("1",2),("2",2),("3",2),("4",3),("5",3),("6",3),("7",4),("N",5),("Q",5),("B",6),("F",6),("L",7),("G",8)]
words = ["NY","NYC","BK","BX","QNS","SI","212","718","$1","DELI","TAXI","WALK","EXIT","24H"]
signs = ["★","♥","→","↑","✕","ONEWAY"]
assert len(emoji)+len(bullets)+len(words)+len(signs) == 64, (len(emoji),len(bullets),len(words),len(signs))
R = Image.new('L', (T*8, T*8), 0); G = Image.new('L', (T*8, T*8), 0)
def fit_text(d, box, s, path, maxsz=60):
    x0, y0, x1, y1 = box
    for sz in range(maxsz, 6, -1):
        f = ImageFont.truetype(path, sz); b = d.textbbox((0, 0), s, font=f)
        if b[2]-b[0] <= x1-x0 and b[3]-b[1] <= y1-y0: break
    w, h = b[2]-b[0], b[3]-b[1]
    d.text(((x0+x1-w)/2 - b[0], (y0+y1-h)/2 - b[1]), s, font=f, fill=255)
def tile(i):
    im = Image.new('L', (T, T), 0); return im, ImageDraw.Draw(im)
tiles = []
for ch in emoji:
    im, d = tile(0)
    big = Image.new('L', (T*2, T*2), 0); bd = ImageDraw.Draw(big)
    fit_text(bd, (6, 6, 2*T-6, 2*T-6), ch, '../fontdl/package/files/' + EMAP[ch], 120)
    tiles.append((big.filter(ImageFilter.MaxFilter(5)).resize((T, T), Image.LANCZOS), 0))
for ch, c in bullets:
    big = Image.new('L', (T*2, T*2), 0); bd = ImageDraw.Draw(big)
    bd.ellipse((8, 8, 2*T-8, 2*T-8), fill=255)
    m = Image.new('L', (T*2, T*2), 0); fit_text(ImageDraw.Draw(m), (36, 30, 2*T-36, 2*T-30), ch, COND, 90)
    big.paste(0, (0, 0), m)
    tiles.append((big.resize((T, T), Image.LANCZOS), c))
for w in words:
    big = Image.new('L', (T*2, T*2), 0); fit_text(ImageDraw.Draw(big), (8, 24, 2*T-8, 2*T-24), w, COND, 110)
    tiles.append((big.resize((T, T), Image.LANCZOS), 0))
for s in signs:
    big = Image.new('L', (T*2, T*2), 0); bd = ImageDraw.Draw(big)
    if s == "ONEWAY":      # the black-and-white ONE WAY arrow sign, white plate, arrow knocked out
        bd.rectangle((4, 34, 2*T-4, 2*T-34), fill=255)
        bd.polygon([(14, 64), (40, 44), (40, 56), (112, 56), (112, 72), (40, 72), (40, 84)], fill=0)
    else:
        fit_text(bd, (10, 10, 2*T-10, 2*T-10), s, '/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf', 120)
    tiles.append((big.resize((T, T), Image.LANCZOS), 0))
for i, (im, c) in enumerate(tiles):
    x, y = (i % 8) * T, (i // 8) * T
    R.paste(im, (x, y)); G.paste(c * 20, (x, y, x+T, y+T))
Image.merge('RGB', (R, G, Image.new('L', R.size, 0))).save('ny_city_skin_glyphs.png')
R.save('preview.png'); print('ok')
