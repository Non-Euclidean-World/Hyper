#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;

out vec3 OurColor;
out vec2 TexCoord;

uniform mat4 model;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(aPos, 0.0, 1.0) * model * projection;
    OurColor = aColor;
    TexCoord = aTexCoord;
}