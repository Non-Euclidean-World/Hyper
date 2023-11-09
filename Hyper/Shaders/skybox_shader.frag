#version 330 core
out vec4 FragColor;

struct Data
{
	vec4 worldspacePos;
	vec3 texCoords;
};

in Data vertexData;

uniform samplerCube skybox;

struct Phase
{
	vec4 skytopColor;
	vec4 horizonColor;
	vec4 sunglareColor;
	float starsVisibility;
};

uniform Phase prevPhase;
uniform Phase nextPhase;
uniform float phaseT;
uniform vec3 sunVector;

void main()
{
	vec3 flippedTexCoords = vertexData.texCoords;
    flippedTexCoords.y = -flippedTexCoords.y;

	vec3 pointOnSphere = normalize(vertexData.worldspacePos.xyz);
	float h = pointOnSphere.y;
	vec4 skytopColor = mix(prevPhase.skytopColor, nextPhase.skytopColor, phaseT);
	vec4 horizonColor = mix(prevPhase.horizonColor, nextPhase.horizonColor, phaseT);
	vec4 sunglareColor = mix(prevPhase.sunglareColor, nextPhase.sunglareColor, phaseT);
	float starsVisibility = mix(prevPhase.starsVisibility, nextPhase.starsVisibility, phaseT);

	float sunDot = dot(normalize(sunVector), pointOnSphere) / 2 + 0.5;
	float sunGradient = sunDot > 0.993 ? 1 : sunDot;

	vec4 skyColor = mix(horizonColor, skytopColor, h);
	FragColor = mix(mix(skyColor, sunglareColor, pow(sunGradient, 20)), texture(skybox, flippedTexCoords), starsVisibility);
}