#version 330 core

const int MAX_JOINTS = 50;//max joints allowed in a skeleton
const int MAX_WEIGHTS = 3;//max number of joints that can affect a vertex

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec3 in_normal;
layout(location = 2) in vec2 in_textureCoords;
layout(location = 3) in ivec3 in_jointIndices;
layout(location = 4) in vec3 in_weights;

out vec2 pass_textureCoords;
out vec3 pass_normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat4 jointTransforms[MAX_JOINTS];

bool areMatricesEqual(mat4 matr1, mat4 matr2) {
	for(int i = 0; i < 4; i++) {
		for(int j = 0; j < 4; j++) {
			if(matr1[i][j] != matr2[i][j]) {
				return false;
			}
		}
	}
	return true;
}


void main(void){
	
	vec4 totalLocalPos = vec4(0.0);
	vec4 totalNormal = vec4(0.0);
	
//	vec3 in_weights2 = vec3(in_weights[0], in_weights[1], 0.3);
//	vec3 in_weights2 = vec3(in_weights[0], 0.3, in_weights[2]);
//	vec3 in_weights2 = vec3(0.3, in_weights[1], in_weights[2]);
	vec3 in_weights2 = vec3(in_weights[0], in_weights[1], in_weights[2]);
	
//	ivec3 in_jointIndices2 = ivec3(0, 1, 14);
//	ivec3 in_jointIndices2 = ivec3(12, 0 ,4);
//		ivec3 in_jointIndices2 = ivec3(12, 20 ,20);
	ivec3 in_jointIndices2 = ivec3(in_jointIndices[0], in_jointIndices[1], in_jointIndices[2]);
	
	for(int i=0;i<MAX_WEIGHTS;i++){
		mat4 jointTransform = jointTransforms[in_jointIndices2[i]];
//		mat4 jointTransform = jointTransforms[10];
		vec4 posePosition = jointTransform * vec4(in_position, 1.0);
//		vec4 posePosition = vec4(in_position, 1.0) * jointTransform;
		totalLocalPos += posePosition * in_weights2[i];
//		totalLocalPos += posePosition * 0.3;
		
//		vec4 worldNormal = jointTransform * vec4(in_normal, 0.0);
//		totalNormal += worldNormal * in_weights[i];
	}

//	if (areMatricesEqual(jointTransforms[0], jointTransforms[1])) {
//		totalLocalPos = vec4(in_position, 1.0);
//		totalNormal = vec4(in_normal, 0.0);
//	}
//	if (in_jointIndices[1] > 15 && in_jointIndices[1] < 0) {
//		totalLocalPos = vec4(in_position, 1.0);
//		totalNormal = vec4(in_normal, 0.0);
//	}
//	if (in_jointIndices[2] > 15 && in_jointIndices[2] < 0) {
//		totalLocalPos = vec4(in_position, 1.0);
//		totalNormal = vec4(in_normal, 0.0);
//	}
	
//	if (totalLocalPos.w == 0 && totalLocalPos.x == 0 && totalLocalPos.y == 0 && totalLocalPos.z == 0) {
//		totalLocalPos = vec4(in_position, 1.0);
//	}
	gl_Position = totalLocalPos * model * view * projection;
//	gl_Position =  projection * view * model * totalLocalPos;
	pass_normal = totalNormal.xyz;
	pass_textureCoords = in_textureCoords;

}