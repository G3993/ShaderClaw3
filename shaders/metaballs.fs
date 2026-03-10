/*{
  "DESCRIPTION": "Mouse-controlled rotating metaballs with dynamic Blinn-Phong lighting",
  "CREDIT": "shellderr '23, ISF adaptation",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "ballCount", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 8.0, "LABEL": "Ball Count" },
    { "NAME": "ballSpeed", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 4.0, "LABEL": "Speed" },
    { "NAME": "smoothK", "TYPE": "float", "DEFAULT": 0.68, "MIN": 0.1, "MAX": 1.5, "LABEL": "Blend" },
    { "NAME": "keyColor", "TYPE": "color", "DEFAULT": [0.4, 0.6, 1.0, 1.0], "LABEL": "Key Light" },
    { "NAME": "rimColor", "TYPE": "color", "DEFAULT": [0.8, 0.3, 0.6, 1.0], "LABEL": "Rim Light" },
    { "NAME": "bgColor", "TYPE": "color", "DEFAULT": [0.4, 0.6, 1.0, 1.0], "LABEL": "Background" }
  ]
}*/

#define MAX_STEPS 26
#define EPSILON 0.016
#define MAX_BALLS 8

// Global ball positions (avoids array function params — GLSL ES 1.0 compat)
vec3 gBalls[MAX_BALLS];
int gBallCount;

mat3 rotation_matrix(vec2 angles) {
    float cx = cos(angles.x), sx = sin(angles.x);
    float cy = cos(angles.y), sy = sin(angles.y);
    return mat3(
        cy, sy*sx, sy*cx,
        0.0, cx, -sx,
        -sy, cy*sx, cy*cx
    );
}

vec3 metaball_position(float id) {
    float t = id * 88.0 + TIME * ballSpeed;
    return 0.7 * vec3(
        sin(t*1.2) * cos(t*0.82),
        cos(6.0 + t*0.9) * sin(9.0 + t*1.15),
        sin(12.0 + t*0.7) * cos(22.0 + t*1.33)
    );
}

float smooth_min(float a, float b, float k) {
    float h = clamp(0.5 + 0.5*(b-a)/k, 0.0, 1.0);
    return mix(b, a, h) - k*h*(1.0-h);
}

float scene_distance(vec3 p) {
    float dist = 1e6;
    for(int i = 0; i < MAX_BALLS; i++) {
        if(i >= gBallCount) break;
        dist = smooth_min(dist, length(p - 1.2*gBalls[i]), smoothK);
    }
    return dist - 0.24;
}

vec3 calculate_normal(vec3 p) {
    vec2 k = vec2(1.0, -1.0);
    return normalize(
        k.xyy * scene_distance(p + k.xyy*0.0001) +
        k.yyx * scene_distance(p + k.yyx*0.0001) +
        k.yxy * scene_distance(p + k.yxy*0.0001) +
        k.xxx * scene_distance(p + k.xxx*0.0001)
    );
}

vec3 lighting(vec3 p) {
    vec3 n = calculate_normal(p);

    vec3 key_dir = normalize(vec3(0.4, 0.7, -0.3));
    float key = pow(clamp(dot(n, key_dir), 0.0, 1.0), 2.2);

    vec3 fill_dir = normalize(vec3(-0.2, 0.5, 0.1));
    float fill = pow(clamp(dot(n, fill_dir), 0.0, 1.0), 2.0);

    vec3 view_dir = normalize(vec3(0.0, 0.0, -1.0));
    float rim = pow(1.0 - clamp(dot(n, view_dir), 0.0, 1.0), 3.0);

    return key * keyColor.rgb +
           fill * vec3(0.7, 0.7, 0.9) +
           rim * rimColor.rgb;
}

vec3 trace_rays(vec3 origin, vec3 dir) {
    vec3 p = origin;
    for(int i = 0; i < MAX_STEPS; i++) {
        float dist = scene_distance(p);
        if(dist < EPSILON) {
            return lighting(p);
        }
        p += dist * dir;
    }
    return bgColor.rgb * (0.9 - length(p.xy)*0.4);
}

void main() {
    // Mouse rotation — mousePos is already normalized 0-1
    vec2 mouse_uv = mousePos * 2.0 - 1.0;
    mat3 rot = rotation_matrix(mouse_uv * 3.1416);

    vec2 uv = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;

    // Setup metaballs
    gBallCount = int(ballCount + 0.5);
    for(int i = 0; i < MAX_BALLS; i++) {
        gBalls[i] = metaball_position(float(i + 1));
    }

    vec3 ray_origin = vec3(0.0, 0.0, -2.2);
    vec3 ray_dir = rot * normalize(vec3(uv * 0.7, 1.0));

    vec3 color = trace_rays(ray_origin, ray_dir);
    gl_FragColor = vec4(1.25 * color, 1.0);
}
