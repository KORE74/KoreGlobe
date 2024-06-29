shader_type spatial;
render_mode unshaded, wireframe, cull_disabled;

uniform vec4 albedo : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float outline_width : hint_range(0.0, 10.0) = 0.01; // Adjusted default value

void vertex() {
    // Transform vertex position into clip space
    vec4 clip_position = PROJECTION_MATRIX * (MODELVIEW_MATRIX * vec4(VERTEX, 1.0));

    // Calculate screen space vertex position
    vec4 screen_position = clip_position / clip_position.w;

    // Calculate screen space normal
    vec3 screen_normal = normalize(mat3(MODELVIEW_MATRIX) * NORMAL);

    // Calculate offset based on screen space normal
    vec2 offset = outline_width * screen_normal.xy / SCREEN_PIXEL_SIZE;

    // Apply offset to screen space vertex position
    screen_position.xy += offset;

    // Transform screen space position back to clip space
    clip_position = screen_position * clip_position.w;

    // Assign modified clip space position back to POSITION
    POSITION = clip_position;
}

void fragment() {
    // Set fragment color to specified albedo color
    ALBEDO = albedo.rgb;
}
