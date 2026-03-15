/*{
  "CATEGORIES": ["Generator", "Simulation"],
  "DESCRIPTION": "Red Carpet — Verlet cloth simulation with velvet shading, wind, and audio-reactive ripples",
  "INPUTS": [
    { "NAME": "clothColor", "LABEL": "Cloth Color", "TYPE": "color", "DEFAULT": [0.7, 0.04, 0.06, 1.0] },
    { "NAME": "highlightColor", "LABEL": "Highlight", "TYPE": "color", "DEFAULT": [1.0, 0.35, 0.3, 1.0] },
    { "NAME": "shadowColor", "LABEL": "Shadow", "TYPE": "color", "DEFAULT": [0.15, 0.0, 0.02, 1.0] },
    { "NAME": "gravity", "LABEL": "Gravity", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "windStrength", "LABEL": "Wind", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
    { "NAME": "stiffness", "LABEL": "Stiffness", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "velvet", "LABEL": "Velvet Sheen", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "specular", "LABEL": "Specular", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "noiseAmount", "LABEL": "Film Grain", "TYPE": "float", "MIN": 0.0, "MAX": 0.1, "DEFAULT": 0.015 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "clothState", "PERSISTENT": true, "WIDTH": 40, "HEIGHT": 80 },
    {}
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
// RED CARPET — Verlet cloth simulation (40x40 grid, 3D positions)
// Based on iq/eiffie's cloth sim approach
// ═══════════════════════════════════════════════════════════════════════

const float PI = 3.14159265;
const float GRID = 40.0;
const int GRIDI = 40;

// Rest length for direct neighbors (horizontal/vertical)
const float REST_LEN = 1.0 / GRID;
// Rest length for diagonal neighbors
const float DIAG_LEN = 1.41421356 / GRID;

float hash1(vec2 p) {
    float n = dot(p, vec2(127.1, 311.7));
    return fract(sin(n) * 43758.5453);
}

// Read particle position from state buffer
// Bottom 40 rows = current position (xyz)
// Top 40 rows = previous position (xyz)
vec3 getPos(vec2 id) {
    return texture2D(clothState, (id + 0.5) / vec2(GRID, GRID * 2.0)).rgb;
}
vec3 getPrev(vec2 id) {
    return texture2D(clothState, (id + vec2(0.5, GRID + 0.5)) / vec2(GRID, GRID * 2.0)).rgb;
}

// Distance constraint: push/pull particle toward rest length
vec3 constraint(vec3 p, vec2 neighborId, float restLen) {
    vec3 q = getPos(neighborId);
    vec3 delta = q - p;
    float dist = length(delta);
    if (dist < 0.0001) return p;
    float correction = (dist - restLen) / dist;
    // Stiffness controls how much of the correction to apply
    p += 0.5 * correction * delta * mix(0.3, 0.8, stiffness);
    return p;
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 0: Cloth simulation state (40x80 persistent buffer)
// ═══════════════════════════════════════════════════════════════════════

vec4 passClothSim() {
    vec2 texel = floor(gl_FragCoord.xy);
    vec2 id = texel;
    bool isPrev = false;

    // Top half stores previous positions
    if (id.y >= GRID) {
        id.y -= GRID;
        isPrev = true;
    }

    // Out of bounds
    if (id.x >= GRID || id.y >= GRID) return vec4(0.0);

    vec3 pos = getPos(id);
    vec3 prev = getPrev(id);

    // First frame: lay cloth flat, slightly draped
    if (FRAMEINDEX < 1) {
        float x = id.x / GRID;
        float y = id.y / GRID;
        // Cloth hangs from top edge (y=1), drapes down
        float sag = 0.05 * sin(x * PI) * (1.0 - y);
        pos = vec3(x, y, sag + hash1(id) * 0.001);
        prev = pos - vec3(0.0, 0.0, hash1(id.yx) * 0.001);

        if (isPrev) return vec4(prev, 1.0);
        return vec4(pos, 1.0);
    }

    // Store previous before modifying
    vec3 oldPos = pos;

    // --- Verlet integration ---
    vec3 vel = pos - prev;
    vel *= 0.995; // damping

    // Gravity: pulls cloth downward (negative Y)
    float grav = gravity * 0.00015;
    vel.y -= grav;

    // Wind: periodic lateral force in Z
    float windT = TIME * 1.5;
    float wx = id.x / GRID;
    float wy = id.y / GRID;
    float windForce = windStrength * 0.0003;
    vel.z += windForce * sin(wy * 4.0 + windT) * cos(wx * 3.0 + windT * 0.7);
    vel.z += windForce * 0.5 * sin(wy * 7.0 + windT * 2.3 + wx * 2.0);
    // Audio kicks add burst ripples
    float audioForce = audioBass * audioReact * 0.0004;
    vel.z += audioForce * sin(wx * 5.0 + TIME * 3.0) * sin(wy * 4.0 + TIME * 2.0);
    vel.z += audioHigh * audioReact * 0.0001 * sin(wx * 12.0 + TIME * 8.0);

    // Integrate
    pos += vel;

    // --- Distance constraints ---
    // Direct neighbors (4-connected)
    if (id.x > 0.5)        pos = constraint(pos, id + vec2(-1.0, 0.0), REST_LEN);
    if (id.x < GRID - 1.5) pos = constraint(pos, id + vec2( 1.0, 0.0), REST_LEN);
    if (id.y > 0.5)        pos = constraint(pos, id + vec2( 0.0,-1.0), REST_LEN);
    if (id.y < GRID - 1.5) pos = constraint(pos, id + vec2( 0.0, 1.0), REST_LEN);

    // Diagonal neighbors (8-connected, for shear resistance)
    if (id.x > 0.5 && id.y > 0.5)               pos = constraint(pos, id + vec2(-1.0,-1.0), DIAG_LEN);
    if (id.x > 0.5 && id.y < GRID - 1.5)        pos = constraint(pos, id + vec2(-1.0, 1.0), DIAG_LEN);
    if (id.x < GRID - 1.5 && id.y > 0.5)        pos = constraint(pos, id + vec2( 1.0,-1.0), DIAG_LEN);
    if (id.x < GRID - 1.5 && id.y < GRID - 1.5) pos = constraint(pos, id + vec2( 1.0, 1.0), DIAG_LEN);

    // Pin top edge (y = GRID-1): cloth hangs from the top
    if (id.y > GRID - 1.5) {
        pos.x = id.x / GRID;
        pos.y = 1.0;
        pos.z = 0.0;
    }

    // Floor collision
    if (pos.y < 0.0) { pos.y = 0.0; vel.y = 0.0; }

    // Bounds
    pos.x = clamp(pos.x, -0.2, 1.2);
    pos.y = clamp(pos.y, 0.0, 1.1);
    pos.z = clamp(pos.z, -0.3, 0.3);

    if (isPrev) return vec4(oldPos, 1.0);
    return vec4(pos, 1.0);
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 1: Render cloth surface
// ═══════════════════════════════════════════════════════════════════════

vec3 getClothPos(vec2 id) {
    return texture2D(clothState, (id + 0.5) / vec2(GRID, GRID * 2.0)).rgb;
}

// Compute normal from neighboring particles
vec3 clothNormal(vec2 id) {
    vec3 c = getClothPos(id);
    vec3 r = getClothPos(id + vec2(1.0, 0.0));
    vec3 u = getClothPos(id + vec2(0.0, 1.0));
    vec3 l = getClothPos(id + vec2(-1.0, 0.0));
    vec3 d = getClothPos(id + vec2(0.0, -1.0));

    vec3 dx = (id.x < GRID - 1.5) ? r - c : c - l;
    vec3 dy = (id.y < GRID - 1.5) ? u - c : c - d;

    return normalize(cross(dx, dy));
}

vec4 passRender() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Map screen UV to cloth grid coordinate
    // Cloth spans roughly x=[0,1] y=[0,1] in sim space
    // We view it head-on, filling the screen
    vec2 clothUV = uv;
    vec2 gridPos = clothUV * (GRID - 1.0);
    vec2 id = floor(gridPos);
    vec2 f = fract(gridPos);

    // Clamp to grid bounds
    if (id.x < 0.0 || id.x >= GRID - 1.0 || id.y < 0.0 || id.y >= GRID - 1.0) {
        if (transparentBg) return vec4(0.0);
        return vec4(shadowColor.rgb * 0.5, 1.0);
    }

    // Bilinear interpolation of 3D positions for smooth surface
    vec3 p00 = getClothPos(id);
    vec3 p10 = getClothPos(id + vec2(1.0, 0.0));
    vec3 p01 = getClothPos(id + vec2(0.0, 1.0));
    vec3 p11 = getClothPos(id + vec2(1.0, 1.0));

    vec3 pos = mix(mix(p00, p10, f.x), mix(p01, p11, f.x), f.y);

    // Interpolated normal
    vec3 n00 = clothNormal(id);
    vec3 n10 = clothNormal(id + vec2(1.0, 0.0));
    vec3 n01 = clothNormal(id + vec2(0.0, 1.0));
    vec3 n11 = clothNormal(id + vec2(1.0, 1.0));
    vec3 N = normalize(mix(mix(n00, n10, f.x), mix(n01, n11, f.x), f.y));

    // Ensure normal faces viewer (flip if needed)
    if (N.z < 0.0) N = -N;

    // ── Lighting ─────────────────────────────────────────────────
    vec3 lightDir = normalize(vec3(0.3, 0.5, 0.9));
    vec3 fillDir = normalize(vec3(-0.4, 0.3, 0.7));
    vec3 viewDir = vec3(0.0, 0.0, 1.0);

    float NdotL = max(dot(N, lightDir), 0.0);
    float NdotF = max(dot(N, fillDir), 0.0);

    // Blinn-Phong specular
    vec3 halfDir = normalize(lightDir + viewDir);
    float NdotH = max(dot(N, halfDir), 0.0);
    float spec = pow(NdotH, 50.0) * specular;

    vec3 halfFill = normalize(fillDir + viewDir);
    float specFill = pow(max(dot(N, halfFill), 0.0), 35.0) * specular * 0.3;

    // Velvet rim: light at grazing angles
    float NdotV = max(dot(N, viewDir), 0.0);
    float velvetRim = pow(1.0 - NdotV, 3.0) * velvet;

    // ── Fold shading from Z displacement ─────────────────────────
    float depth = pos.z; // displacement from flat
    float foldShade = smoothstep(-0.1, 0.1, depth);

    // Diffuse
    vec3 diffuse = mix(shadowColor.rgb, clothColor.rgb, 0.4 + 0.6 * foldShade);
    diffuse *= 0.25 + 0.75 * NdotL;
    diffuse += clothColor.rgb * NdotF * 0.12;

    // Highlights
    vec3 highlights = highlightColor.rgb * spec;
    highlights += highlightColor.rgb * specFill;

    // Velvet glow
    vec3 velvetGlow = highlightColor.rgb * velvetRim * 0.4;

    // Fabric weave texture
    float weaveX = sin(clothUV.x * RENDERSIZE.x * 0.5) * 0.5 + 0.5;
    float weaveY = sin(clothUV.y * RENDERSIZE.y * 0.5) * 0.5 + 0.5;
    float weave = mix(1.0, 0.96 + 0.04 * weaveX * weaveY, velvet);

    // Compose
    vec3 col = diffuse * weave + highlights + velvetGlow;

    // Audio shimmer on specular
    col += highlightColor.rgb * audioHigh * audioReact * 0.12 * spec * 3.0;

    // Ambient occlusion in folds
    float ao = smoothstep(-0.08, 0.05, depth);
    col *= 0.7 + 0.3 * ao;

    // Film grain
    float grain = (hash1(uv * RENDERSIZE.xy + fract(TIME * 43.1) * 1000.0) - 0.5) * noiseAmount;
    col += grain;

    col = clamp(col, 0.0, 1.0);

    if (transparentBg) {
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        return vec4(col, lum);
    }
    return vec4(col, 1.0);
}

// ═══════════════════════════════════════════════════════════════════════
// Main — route by PASSINDEX
// ═══════════════════════════════════════════════════════════════════════

void main() {
    if (PASSINDEX == 0) {
        gl_FragColor = passClothSim();
    } else {
        gl_FragColor = passRender();
    }
}
