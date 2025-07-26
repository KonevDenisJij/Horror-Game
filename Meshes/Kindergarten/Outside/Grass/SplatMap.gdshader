shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;
uniform sampler2D splatmap_texture;
uniform sampler2D grass_texture : source_color;
uniform sampler2D sand_texture : source_color;
uniform sampler2D gravel_texture : source_color;
uniform sampler2D normal_grass_texture  : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D normal_sand_texture  : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D normal_gravel_texture  : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D AORM_grass_texture : hint_roughness_g, filter_linear_mipmap, repeat_enable;
uniform sampler2D AORM_sand_texture : hint_roughness_g, filter_linear_mipmap, repeat_enable;
uniform sampler2D AORM_gravel_texture : hint_roughness_g, filter_linear_mipmap, repeat_enable;
uniform float texture_scale_X = 10.0;
uniform float texture_scale_Y = 10.0;
uniform float metallic : hint_range(0.0, 1.0, 0.01);

void fragment() {
    // Sample each texture
    vec2 uv_scaled = UV * vec2(texture_scale_X, texture_scale_Y);
	
    vec3 grass_tex = texture(grass_texture, uv_scaled).rgb;
    vec3 sand_tex = texture(sand_texture, uv_scaled).rgb;
    vec3 gravel_tex = texture(gravel_texture, uv_scaled).rgb;
	
    vec3 grass_normal = texture(normal_grass_texture, uv_scaled).rgb;
    vec3 sand_normal = texture(normal_sand_texture, uv_scaled).rgb;
    vec3 gravel_normal = texture(normal_gravel_texture, uv_scaled).rgb;
	
    float grass_ao = texture(AORM_grass_texture, uv_scaled).r;
    float sand_ao = texture(AORM_sand_texture, uv_scaled).r;
    float gravel_ao = texture(AORM_gravel_texture, uv_scaled).r;
	
    float grass_r = texture(AORM_grass_texture, uv_scaled).g;
    float sand_r = texture(AORM_sand_texture, uv_scaled).g;
    float gravel_r = texture(AORM_gravel_texture, uv_scaled).g;
	
    float grass_m = texture(AORM_grass_texture, uv_scaled).b;
    float sand_m = texture(AORM_sand_texture, uv_scaled).b;
    float gravel_m = texture(AORM_gravel_texture, uv_scaled).b;
	
    vec3 splatmap_text = texture(splatmap_texture, UV).rgb;

    // Mix textures based on vertex colors
    vec3 final_color = vec3(0.0);
    final_color += sand_tex * splatmap_text.r; // Sand for red
    final_color += grass_tex * splatmap_text.g; // Grass for green
    final_color += gravel_tex * splatmap_text.b; // Gravel for blue
	
    vec3 final_normal = vec3(0.0);
    final_normal += sand_normal * splatmap_text.r; // Sand for red
    final_normal += grass_normal * splatmap_text.g; // Grass for green
    final_normal += gravel_normal * splatmap_text.b; // Gravel for blue
	
    float final_ao = 0.0f;
    final_ao += sand_ao * splatmap_text.r; // Sand for red
    final_ao += grass_ao * splatmap_text.g; // Grass for green
    final_ao += gravel_ao * splatmap_text.b; // Gravel for blue
	
    float final_r = 0.0f;
    final_r += sand_r * splatmap_text.r; // Sand for red
    final_r += grass_r * splatmap_text.g; // Grass for green
    final_r += gravel_r * splatmap_text.b; // Gravel for blue
	
    float final_m = 0.0f;
    final_m += sand_m * splatmap_text.r; // Sand for red
    final_m += grass_m * splatmap_text.g; // Grass for green
    final_m += gravel_m * splatmap_text.b; // Gravel for blue
	
    // Output the final color
    ALBEDO = final_color;
	NORMAL_MAP = final_normal;
	AO = final_ao;
	ROUGHNESS = final_r;
	METALLIC = final_m * metallic;
}