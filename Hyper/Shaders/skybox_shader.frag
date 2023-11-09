#version 330 core
out vec4 FragColor;

struct Data
{
	vec4 worldspacePos;
	vec3 texCoords;
};

in Data vertexData;

uniform samplerCube skybox;

//const vec4 skytop = vec4(0.0f, 0.0f, 1.0f, 1.0f);
//const vec4 skyhorizon = vec4(0.3294f, 0.92157f, 1.0f, 1.0f);
//uniform float starsVisibility;

struct Phase
{
	vec4 skytopColor;
	vec4 horizonColor;
	float starsVisibility;
};

uniform Phase prevPhase;
uniform Phase nextPhase;
uniform float phaseT;

void main()
{
	vec3 flippedTexCoords = vertexData.texCoords;
    flippedTexCoords.y = -flippedTexCoords.y;

	vec3 pointOnSphere = normalize(vertexData.worldspacePos.xyz);
	float h = pointOnSphere.y;
	vec4 skytopColor = mix(prevPhase.skytopColor, nextPhase.skytopColor, phaseT);
	vec4 horizonColor = mix(prevPhase.horizonColor, nextPhase.horizonColor, phaseT);
	float starsVisibility = mix(prevPhase.starsVisibility, nextPhase.starsVisibility, phaseT);
	FragColor = mix(mix(horizonColor, skytopColor, h), texture(skybox, flippedTexCoords), starsVisibility);
}