shader_type spatial;

// Uniform variables for setting colors and properties of the shader.
uniform vec4 albedo : source_color = vec4(0.0, 0.0, 0.8, 1.0); // Base color of the mesh (red by default).
uniform vec4 wire_color : source_color = vec4(1.0); // Color of the wireframe (white by default).
uniform float wire_width : hint_range(0.0, 40.0) = 1.0; // Width of the wireframe lines.
uniform float wire_smoothness : hint_range(0.0, 0.1) = 0.0; // Smoothness factor for the wireframe lines.

varying vec3 barys; // Varying variable to hold barycentric coordinates.

void vertex() {
    // Calculate the barycentric coordinates based on the vertex index in the triangle.
    int index = VERTEX_ID % 3; // Get the index of the vertex within the current triangle.

    switch (index) {
        case 0:
            barys = vec3(1.0, 0.0, 0.0); // Set barycentric coordinates (1.0, 0.0, 0.0) for vertex 0.
            break;
        case 1:
            barys = vec3(0.0, 1.0, 0.0); // Set barycentric coordinates (0.0, 1.0, 0.0) for vertex 1.
            break;
        case 2:
            barys = vec3(0.0, 0.0, 1.0); // Set barycentric coordinates (0.0, 0.0, 1.0) for vertex 2.
            break;
    }
}

// fragment: works on each pixel of the mesh.

void fragment() {
    vec3 deltas = fwidth(barys); // Calculate the derivative of barycentric coordinates.

    // Smoothstep function to create smooth transitions for the wireframe.
    vec3 barys_s = smoothstep(deltas * wire_width - wire_smoothness, deltas * wire_width + wire_smoothness, barys);

    // Determine the minimum value of the smoothed barycentric coordinates.
    float wires = min(barys_s.x, min(barys_s.y, barys_s.z));

    // Mix between wireframe color and albedo color based on the smoothed barycentric coordinates.
    ALBEDO.rgb = mix(wire_color.rgb, albedo.rgb, wires);
}
