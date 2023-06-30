#version 330

out vec4 outputColor;

in vec2 uvs;

uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, uvs);
}