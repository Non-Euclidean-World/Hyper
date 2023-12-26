#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec3 aColor;

uniform mat4 model;
uniform mat4 normalRotation;
uniform mat4 view;
uniform mat4 projection;
uniform float curv;
uniform int sphere; // 0 for upper, 1 for lower
uniform vec3 lowerSphereCenter;
uniform int characterSphere;

out vec4 Normal;
out vec4 FragPos; // in world space coordinates
out vec3 Color;

vec3 flipXZ(vec3 v);
vec3 flipY(vec3);

vec4 port(vec4 ePoint)
{
    vec3 p = ePoint.xyz;
    float d = length(p);
    if(d < 0.0001 || curv == 0)
        return ePoint;
    if(curv > 0)
    {
        if(characterSphere == 0)
        {
            if(sphere == 0)
            {
                d = length(p);
                return vec4(p / d * sin(d), cos(d));
            }
            if(sphere == 1)
            {
                p = p - lowerSphereCenter;
                d = length(p);
                return vec4(flipXZ(p) / d * sin(d), -cos(d));
            }
        }
        else
        {
            if(sphere == 0)
            {
                d = length(p);   
                return vec4(flipY(p) / d * sin(d), cos(d));
            }
            if(sphere == 1)
            {
                p = p - lowerSphereCenter;
                d = length(p);
                return vec4(flipY(flipXZ(p)) / d * sin(d), -cos(d));
            }
        }
    }

    return vec4(p / d * sinh(d), cosh(d));
}

vec3 flipXZ(vec3 v) {
    return vec3(-v.x, v.y, -v.z);
}

vec3 flipY(vec3 v) {
    return vec3(v.x, -v.y, v.z);
}

mat4 TranslateMatrix(vec4 to)
{
	if (abs(curv) < 0.001)
    {
        return transpose(mat4(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        to.x, to.y, to.z, 1));
    }
    else
    {
        if(curv > 0 && to.w < 0) { // this is just to remove the black spot caused by division by zero
            to.w *= -1;
        }
        float denom = 1 + to.w;
        return transpose(mat4(
            1 - curv * to.x * to.x / denom, -curv * to.x * to.y / denom, -curv * to.x * to.z / denom, -curv * to.x,
            -curv * to.y * to.x / denom, 1 - curv * to.y * to.y / denom, -curv * to.y * to.z / denom, -curv * to.y,
            -curv * to.z * to.x / denom, -curv * to.z * to.y / denom, 1 - curv * to.z * to.z / denom, -curv * to.z,
            to.x, to.y, to.z, to.w));
    }
}

void main(void)
{
    vec4 eucPos = vec4(aPosition, 1);

    gl_Position = port(eucPos * model) * view * projection;
    FragPos = port(eucPos * model);
    Normal = vec4(aNormal, 0) * normalRotation * TranslateMatrix(port(eucPos * model));
    Color = aColor;
}