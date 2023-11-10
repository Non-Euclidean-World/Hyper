﻿#version 330 core

out vec4 FragColor;

struct PointLight 
{
    vec4 position;
    vec3 color;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    // attenuation parameters
    float constant;
    float linear;
    float quadratic;
};

struct DirectionalLight
{
    vec4 direction;

    float ambient;
    float diffuse;
    float specular;
};

#define MAX_LIGHTS 10

uniform PointLight lightCasters[MAX_LIGHTS];
uniform bool hasSun;
struct EnvironmentInfo
{
    vec3 prevPhaseSunLightColor;
    vec3 nextPhaseSunLightColor;
    float phaseT;
    float prevPhaseNightAmbient;
    float nextPhaseNightAmbient;
};
uniform DirectionalLight sunLight;
uniform EnvironmentInfo envInfo;

uniform vec4 viewPos;
uniform int numLights;

uniform float curv;

in vec4 Normal;
in vec4 FragPos;
in vec3 Color;

float dotProduct(vec4 u, vec4 v)
{
    return dot(u, v) - ((curv < 0) ? 2 * u.w * v.w : 0);
}

vec4 direction(vec4 from, vec4 to)
{
    if (curv > 0)
    {
        float cosd = dotProduct(from, to);
        float sind = sqrt(1 - cosd * cosd);
        return (to - from * cosd) / sind;
    }
    if (curv < 0)
    {
        float coshd = -dotProduct(from, to);
        float sinhd = sqrt(coshd * coshd - 1);
        return (to - from * coshd) / sinhd;
    }
    return normalize(to - from);
}

vec3 CalcPointLight(PointLight light, vec4 normal, vec4 fragPos, vec4 viewDir);
vec3 CalcDirLight(DirectionalLight light, vec4 normal, vec4 viewDir);

void main()
{
    vec4 norm = normalize(Normal);
    vec4 viewDir = normalize(direction(FragPos, viewPos));

    vec3 result = vec3(0);
    for (int i = 0; i < numLights; ++i)
    {
        result += CalcPointLight(lightCasters[i], norm, FragPos, viewDir); 
    }

    if(hasSun) // skip in spherical geometry
    {
        result += CalcDirLight(sunLight, norm, viewDir);
        result += mix(envInfo.prevPhaseNightAmbient, envInfo.nextPhaseNightAmbient, envInfo.phaseT) * vec3(1);
    }

    FragColor = vec4(result * Color, 1.0);
}

vec3 CalcDirLight(DirectionalLight light, vec4 normal, vec4 viewDir)
{
    vec3 sunLightColor = mix(envInfo.prevPhaseSunLightColor, envInfo.nextPhaseSunLightColor, envInfo.phaseT);
    vec3 ambient = light.ambient * sunLightColor;

    vec4 lightDir = normalize(light.direction);
    float diff = max(dotProduct(normal, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * sunLightColor;

    vec4 reflectDir = 2 * dotProduct(lightDir, normal) * normal - lightDir;
    float spec = pow(max(dotProduct(viewDir, reflectDir), 0.0), 32);
    vec3 specular = light.specular * spec * sunLightColor;

    return ambient + diffuse + specular;
}

vec3 CalcPointLight(PointLight light, vec4 normal, vec4 fragPos, vec4 viewDir)
{
    vec3 currAmbient = light.ambient * light.color;

    vec4 lightDir = normalize(direction(FragPos, light.position));
    float diff = max(dotProduct(normal, lightDir), 0.0);
    vec3 currDiffuse = light.diffuse * diff * light.color;
        
    vec4 reflectDir = 2 * dotProduct(lightDir, normal) * normal - lightDir;
    float spec = pow(max(dotProduct(viewDir, reflectDir), 0.0), 32);
    vec3 currSpecular = light.specular * spec * light.color;

    float dist = sqrt(dotProduct(light.position - FragPos, light.position - FragPos));
    float attenuation = 1.0 / (light.constant + light.linear * dist + light.quadratic * (dist * dist));

    currAmbient *= attenuation;
    currDiffuse *= attenuation;
    currSpecular *= attenuation;

    return (currAmbient + currDiffuse + currSpecular) * light.color;
}
