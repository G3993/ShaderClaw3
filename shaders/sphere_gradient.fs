/*{
  "DESCRIPTION": "Blinn-Phong shaded sphere with two orbiting light sources. Adapted from Shadertoy by cmalessa.",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "diffuseColor", "TYPE": "color", "DEFAULT": [0.9, 0.0, 0.7, 1.0], "LABEL": "Diffuse" },
    { "NAME": "specColor", "TYPE": "color", "DEFAULT": [0.0, 1.0, 1.0, 1.0], "LABEL": "Specular" },
    { "NAME": "smoothness", "TYPE": "float", "DEFAULT": 11.0, "MIN": 1.0, "MAX": 64.0, "LABEL": "Smoothness" },
    { "NAME": "lightIntensity", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 2.0, "LABEL": "Light Intensity" },
    { "NAME": "orbitSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0, "LABEL": "Orbit Speed" }
  ]
}*/

#define PI 3.14159265358979

void main() {
    vec2 center = RENDERSIZE.xy * 0.5;
    float r = RENDERSIZE.y / 3.0;
    vec2 pos = gl_FragCoord.xy - center;

    float t = TIME * orbitSpeed;

    // Two orbiting light directions
    vec3 l0 = normalize(vec3(sin(t), sin(t), cos(t)));
    vec3 l1 = normalize(vec3(-sin(t), cos(t), sin(t)));

    // Check if pixel is inside sphere
    float d2 = pos.x * pos.x + pos.y * pos.y;
    if (d2 > r * r) {
        gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    float z = sqrt(r * r - d2);
    vec3 n = normalize(vec3(pos.x, pos.y, z));

    vec3 Kd = diffuseColor.rgb / PI;
    vec3 Ks = specColor.rgb * ((smoothness + 8.0) / (8.0 * PI));

    vec3 Lo = vec3(0.0);

    // Light 0
    vec3 h0 = normalize(l0 + n);
    float cosTi0 = max(dot(n, l0), 0.0);
    float cosTh0 = max(dot(n, h0), 0.0);
    Lo += (Kd + Ks * pow(cosTh0, smoothness)) * lightIntensity * cosTi0;

    // Light 1
    vec3 h1 = normalize(l1 + n);
    float cosTi1 = max(dot(n, l1), 0.0);
    float cosTh1 = max(dot(n, h1), 0.0);
    Lo += (Kd + Ks * pow(cosTh1, smoothness)) * lightIntensity * cosTi1;

    gl_FragColor = vec4(Lo, 1.0);
}
