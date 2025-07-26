shader_type spatial;
render_mode specular_schlick_ggx, ambient_light_disabled;

uniform vec4 Color : source_color;
uniform float Roughness = 0.2;

void fragment() {
	ALBEDO = Color.rgb;
	ALPHA = Color.a;
	ROUGHNESS = Roughness;
}