#version 330 core

const int MAX_BONES = 50; //max bones allowed in a skeleton
const int MAX_WEIGHTS = 3; //max number of joints that can affect a vertex

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec3 in_normal;
layout(location = 2) in vec2 in_textureCoords;
layout(location = 3) in ivec3 in_boneIndices;
layout(location = 4) in vec3 in_weights;

out vec4 FragPos;
out vec2 Texture;
out vec4 Normal;

uniform float curv;
uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat4 boneTransforms[MAX_BONES];

vec4 port(vec4 ePoint) {
	vec3 p = ePoint.xyz;
	float d = length(p);
	if(d < 0.0001 || curv == 0) return ePoint;
	if(curv > 0) return vec4(p / d * sin(d), cos(d));
	return vec4(p / d * sinh(d), cosh(d));
}

mat4 TranslateMatrix(vec4 to)
{
	if (abs(curv) < 0.001)
	{
		return transpose(mat4(
		1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		to.x, to.y, to.z, 1));
	}
	else
	{
		float denom = 1 + to.w;
		return transpose(mat4(
		1 - curv * to.x * to.x / denom, -curv * to.x * to.y / denom, -curv * to.x * to.z / denom, -curv * to.x,
		-curv * to.y * to.x / denom, 1 - curv * to.y * to.y / denom, -curv * to.y * to.z / denom, -curv * to.y,
		-curv * to.z * to.x / denom, -curv * to.z * to.y / denom, 1 - curv * to.z * to.z / denom, -curv * to.z,
		to.x, to.y, to.z, to.w));
	}
}

void main(void){
	vec4 totalLocalPos = vec4(0.0);
	vec4 totalNormal = vec4(0.0);
	
	for(int i = 0; i < MAX_WEIGHTS; i++){
		mat4 boneTransform = boneTransforms[in_boneIndices[i]];
		vec4 posePosition = boneTransform * vec4(in_position, 1.0);
		totalLocalPos += posePosition * in_weights[i];
		
		vec4 worldNormal = boneTransform * vec4(in_normal, 0.0);
		totalNormal += worldNormal * in_weights[i];
	}

	gl_Position = port(totalLocalPos * model) * view * projection;
	FragPos = port(totalLocalPos * model);
	Normal = totalNormal * TranslateMatrix(port(totalLocalPos * model));
	Texture = in_textureCoords;
}