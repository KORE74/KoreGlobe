shader_type spatial;

void fragment() {
    vec3 wirecolor = vec3(1.0, 1.0, 1.0); // White wireframe color
    float width    = 0.02; // Wireframe width

    vec3 d       = fwidth(VERTEX);
    vec3 a3      = smoothstep(vec3(0.0), d * width, VERTEX);
    float factor = min(min(a3.x, a3.y), a3.z);
    ALBEDO       = mix(wirecolor, vec3(0.0), factor);
}
