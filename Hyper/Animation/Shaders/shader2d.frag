#version 330 core

out vec4 FragColor;

#define MAX_LIGHTS 10

uniform vec3 lightColor[MAX_LIGHTS];
uniform vec4 lightPos[MAX_LIGHTS];
uniform vec4 viewPos;
uniform int numLights;
uniform sampler2D texture0;

in vec4 Normal;
in vec4 FragPos;
in vec2 Texture;

void main(void){
	float ambientStrength = 0.1;
	vec3 ambient = ambientStrength * vec3(1.0);
	vec3 diffuse = vec3(0.0);
	vec3 specular = vec3(0.0);

	vec4 norm = normalize(Normal);
	vec4 viewDir = normalize(viewPos - FragPos);
	
	for (int i = 0; i < numLights; ++i) {
		vec4 lightDir = normalize(lightPos[i] - FragPos);
		diffuse += max(dot(norm, lightDir), 0.0) * lightColor[i];

		float specularStrength = 0.5;
		vec4 reflectDir = 2 * dot(lightDir, norm) * norm - lightDir;
		float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
		specular += specularStrength * spec * lightColor[i];
	}
	
	vec3 diffuseColour = (ambient + diffuse + specular) * texture(texture0, Texture).xyz;		
	FragColor = vec4(diffuseColour, 1);
}