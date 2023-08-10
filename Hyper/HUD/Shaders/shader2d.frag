#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform vec4 color;
uniform sampler2D texture0;
uniform bool useTexture;
uniform vec4 spriteRect; // x - left, y - bottom, z - width, w - height

void main()
{             
    if (useTexture)
    {
        vec2 adjustedCoords = TexCoord * spriteRect.zw + spriteRect.xy;
        FragColor = texture(texture0, adjustedCoords) * color;
    }
    else
    {
        FragColor = color; 
    }
}
