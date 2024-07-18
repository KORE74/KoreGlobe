shader_type spatial;

render_mode unshaded, depth_draw_always, cull_back;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    ALBEDO = COLOR.rgb;
    ALPHA = 1.0;
}
