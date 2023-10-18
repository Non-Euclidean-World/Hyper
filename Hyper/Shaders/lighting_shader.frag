#version 330 core

out vec4 FragColor;

#define MAX_LIGHTS 10

uniform samplerCube depthMap;
uniform float far_plane;

uniform vec3 lightColor[MAX_LIGHTS];
uniform vec4 lightPos[MAX_LIGHTS];
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

float ShadowCalculation(vec3 fragPos)
{
    vec3 fragToLight = fragPos - lightPos[0].xyz;
    float closestDepth = texture(depthMap, fragToLight).r;
    closestDepth *= far_plane;
    float currentDepth = length(fragToLight);
    float bias = 0.05;
    float shadow = currentDepth -  bias > closestDepth ? 1.0 : 0.0;

    return shadow;
}

void main()
{
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * vec3(1.0);
    vec3 diffuse = vec3(0.0);
    vec3 specular = vec3(0.0);

    vec4 norm = normalize(Normal);
    vec4 viewDir = normalize(direction(FragPos, viewPos));

    for (int i = 0; i < numLights; ++i)
    {
        vec4 lightDir = normalize(direction(FragPos, lightPos[i]));
        diffuse += max(dotProduct(norm, lightDir), 0.0) * lightColor[i];
        
        float specularStrength = 0.5;
        vec4 reflectDir = 2 * dotProduct(lightDir, norm) * norm - lightDir;
        float spec = pow(max(dotProduct(viewDir, reflectDir), 0.0), 32);
        specular += specularStrength * spec * lightColor[i];
    }

    float shadow = ShadowCalculation(FragPos.xyz);
    vec3 result = (ambient + diffuse + specular) * (1 - shadow) * Color;
    FragColor = vec4(result, 1.0);
}
