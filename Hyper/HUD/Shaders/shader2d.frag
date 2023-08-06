#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform vec4 color;
uniform sampler2D texture0;
uniform bool useTexture;

void main()
{             
    if (useTexture)
    {
        FragColor = texture(texture0, TexCoord);
    }
    else
    {
        FragColor = color; 
    }
}
