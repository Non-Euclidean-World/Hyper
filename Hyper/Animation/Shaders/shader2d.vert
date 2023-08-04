#version 330 core

const int MAX_JOINTS = 50;//max joints allowed in a skeleton
const int MAX_WEIGHTS = 3;//max number of joints that can affect a vertex

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec3 in_normal;
layout(location = 2) in vec2 in_textureCoords;
layout(location = 3) in ivec3 in_jointIndices;
layout(location = 4) in vec3 in_weights;

out vec4 FragPos;
out vec2 Texture;
out vec4 Normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat4 jointTransforms[MAX_JOINTS];

void main(void){
	vec4 totalLocalPos = vec4(0.0);
	vec4 totalNormal = vec4(0.0);
	
	for(int i=0;i<MAX_WEIGHTS;i++){
		mat4 jointTransform = jointTransforms[in_jointIndices[i]];
		vec4 posePosition = jointTransform * vec4(in_position, 1.0);
		totalLocalPos += posePosition * in_weights[i];
		
		vec4 worldNormal = jointTransform * vec4(in_normal, 0.0);
		totalNormal += worldNormal * in_weights[i];
	}
	
	gl_Position = totalLocalPos * model * view * projection;
	FragPos = totalLocalPos * model;
	Normal = totalNormal * model;
	Texture = in_textureCoords;
}