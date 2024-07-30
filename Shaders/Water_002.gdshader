shader_type spatial;

uniform vec3 albedo : source_color = vec3(0, 0.1, 0.4);
uniform float metallic : hint_range(0.0, 1.0) = 0;
uniform float roughness : hint_range(0.0, 1.0) = 0.02;

// Bump textures for wave shapes
uniform sampler2D texture_normal;
uniform sampler2D texture_normal2;

// Wave movement
uniform vec2 wave_direction  = vec2(2.0, 1.0);
uniform vec2 wave_direction2 = vec2(0.0, 2.0);
uniform float time_scale : hint_range(0.0, 0.2) = 0.01;


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
    
    vec2 time  = (TIME * wave_direction)  * time_scale;
    vec2 time2 = (TIME * wave_direction2) * time_scale;
    
    vec3 normal1 = texture(texture_normal,  UV + time).rgb;
    vec3 normal2 = texture(texture_normal2, UV + time2).rgb;
    vec3 normal_blend = mix(normal1, normal2, 0.5);
    
    vec3 normal_blend2 = mix(texture(texture_normal,  UV + time).rgb, texture(texture_normal2, UV + time2).rgb, 0.5);
    
	// Called for every pixel the material is visible on.
    ALBEDO = albedo;
    METALLIC = metallic;
    ROUGHNESS = roughness;
    NORMAL_MAP = normal_blend2;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
