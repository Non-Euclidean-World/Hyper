#version 330 core
layout (location = 0) in vec3 aPos;

struct Data
{
	vec4 worldspacePos;
	vec3 texCoords;
};

out Data vertexData;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main()
{
	vertexData.texCoords = aPos;
	vertexData.worldspacePos = vec4(aPos, 1.0) * model;
	vec4 pos = vertexData.worldspacePos * view * projection;
	gl_Position = pos.xyww;
}