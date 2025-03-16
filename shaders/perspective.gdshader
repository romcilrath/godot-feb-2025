// Hey this is Hei! This shader "fakes" a 3D-camera perspective on CanvasItems.
// License: MIT

shader_type canvas_item;

// Camera FOV
uniform float fov : hint_range(1, 179) = 90;
uniform bool cull_back = true;
uniform float rotation_y : hint_range(-180, 180) = 0.0;
uniform float rotation_x : hint_range(-180, 180) = 0.0;

// At 0, the image retains its size when unrotated.
// At 1, the image is resized so that it can do a full
// rotation without clipping inside its rect.
uniform float inset : hint_range(0, 1) = 0.0;

varying flat vec2 o;
varying vec3 p;
varying vec4 MODULATE;

void vertex(){
	float sin_b = sin(rotation_y / 180.0 * PI); 
	float cos_b = cos(rotation_y / 180.0 * PI); 
	float sin_c = sin(rotation_x / 180.0 * PI); 
	float cos_c = cos(rotation_x / 180.0 * PI); 
	
	mat3 inv_rot_mat;
	inv_rot_mat[0][0] = cos_b;
	inv_rot_mat[0][1] = 0.0;
	inv_rot_mat[0][2] = -sin_b;
	
	inv_rot_mat[1][0] = sin_b * sin_c;
	inv_rot_mat[1][1] = cos_c;
	inv_rot_mat[1][2] = cos_b * sin_c;
	
	inv_rot_mat[2][0] = sin_b * cos_c;
	inv_rot_mat[2][1] = -sin_c;
	inv_rot_mat[2][2] = cos_b * cos_c;
	
	
	float t = tan(fov / 360.0 * PI);
	p = inv_rot_mat * vec3((UV - 0.5), 0.5 / t);
	float v = (0.5 / t) + 0.5;
	p.xy *= v * inv_rot_mat[2].z;
	o = v * inv_rot_mat[2].xy;

	VERTEX += (UV - 0.5) / TEXTURE_PIXEL_SIZE * t * (1.0 - inset);
    MODULATE = COLOR;
}

void fragment(){
    if (cull_back && p.z <= 0.0) discard;
    vec2 uv = (p.xy / p.z).xy - o;
    vec4 texture_color = texture(TEXTURE, uv + 0.5);
    
    if (AT_LIGHT_PASS) {
        COLOR = texture_color;
    } else {
        COLOR = texture_color * MODULATE; // Use MODULATE for tinting
    }
    COLOR.a *= step(max(abs(uv.x), abs(uv.y)), 0.5);
}

void light() {
    vec4 texture_color = texture(TEXTURE, UV);
    LIGHT = vec4(texture_color.rgb * LIGHT_COLOR.rgb * LIGHT_ENERGY, LIGHT_COLOR.a);
}
