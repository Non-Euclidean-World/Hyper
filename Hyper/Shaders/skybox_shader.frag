#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform samplerCube skybox;

void main()
{
	vec3 flippedTexCoords = TexCoords;
    flippedTexCoords.y = -flippedTexCoords.y;
    FragColor = texture(skybox, flippedTexCoords);
}