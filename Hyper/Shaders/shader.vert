#version 330 core

layout(location = 0) in vec3 aPosition;

layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float curv;
uniform float anti;

vec4 port(vec3 ePoint) {
    vec3 p = ePoint;
    float d = length(p);
    if(d < 0.0001 || curv == 0) return vec4(p, 1);
    if(curv > 0) return vec4(p / d * sin(d), cos(d));
    if(curv < 0) return vec4(p / d * sinh(d), cosh(d));
}

void main(void)
{
    texCoord = aTexCoord;

    gl_Position = anti * port(aPosition) * model * view * projection;
}
