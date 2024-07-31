shader_type spatial; // Specifies the shader type as spatial, used for 3D objects in Godot.

// Uniform variables for setting colors and properties of the shader.
uniform vec4 albedo : source_color = vec4(0.0, 0.0, 0.0, 1.0); // Base color of the mesh.
uniform vec4 wire_color : source_color = vec4(1.0, 1.0, 1.0, 1.0); // Color of the wireframe.
uniform float wire_width : hint_range(0.0, 10.0) = 0.5; // Width of the wireframe lines.
uniform float wire_smoothness : hint_range(0.0, 0.1) = 0.01; // Smoothness factor for the wireframe lines.

varying vec3 barys; // Varying variable to hold barycentric coordinates.

void vertex() {
    // Calculate the barycentric coordinates based on the vertex index in the triangle.
    int index = VERTEX_ID % 3; // Get the index of the vertex within the current triangle.

    switch (index) { // Switch based on the vertex index modulo 3 (0, 1, or 2).
        case 0: // First vertex of the triangle.
            barys = vec3(1.0, 0.0, 0.0); // Set barycentric coordinates (1.0, 0.0, 0.0) for vertex 0.
            break;
        case 1: // Second vertex of the triangle.
            barys = vec3(0.0, 1.0, 0.0); // Set barycentric coordinates (0.0, 1.0, 0.0) for vertex 1.
            break;
        case 2: // Third vertex of the triangle.
            barys = vec3(0.0, 0.0, 1.0); // Set barycentric coordinates (0.0, 0.0, 1.0) for vertex 2.
            break;
    }
}

void fragment() {
    vec3 deltas = fwidth(barys); // Calculate the derivative of barycentric coordinates.

    // Smoothstep function to create smooth transitions for the wireframe.
    vec3 barys_s = smoothstep(deltas * wire_width - wire_smoothness,
                              deltas * wire_width + wire_smoothness,
                              barys);

    // Determine the minimum value of the smoothed barycentric coordinates.
    float wires = min(barys_s.x, min(barys_s.y, barys_s.z));

    // Mix between wireframe color and albedo color based on the smoothed barycentric coordinates.
    ALBEDO.rgb = mix(wire_color.rgb, albedo.rgb, wires);
}
