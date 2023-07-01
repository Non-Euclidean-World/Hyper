#version 330 core

out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec4 lightPos;
uniform vec4 viewPos;

uniform float curv;

in vec4 Normal;
in vec4 FragPos;

float dotProduct(vec4 u, vec4 v)
{
	return dot(u, v) - ((curv < 0) ? 2 * u.w * v.w : 0);
}

vec4 direction(vec4 from, vec4 to)
{
	if (curv > 0) {
		float cosd = dotProduct(from, to);
		float sind = sqrt(1 - cosd * cosd);
		return (to - from * cosd) / sind;
	}
	if (curv < 0) {
		float coshd = -dotProduct(from, to);
		float sinhd = sqrt(coshd * coshd - 1);
		return (to - from * coshd) / sinhd;
	}
	return normalize(to - from);
}

void main()
{
	float ambientStrength = 0.1;
	vec3 ambient = ambientStrength * lightColor;

	vec4 norm = normalize(Normal);
	vec4 lightDir = normalize(direction(FragPos, lightPos));
	vec3 diffuse = max(dotProduct(norm, lightDir), 0.0) * lightColor;
	
	float specularStrength = 0.5;
	vec4 viewDir = normalize(direction(FragPos, viewPos));
	vec4 reflectDir = 2 * dotProduct(lightDir, norm) * norm - lightDir;
	float spec = pow(max(dotProduct(viewDir, reflectDir), 0.0), 32);
	vec3 specular = specularStrength * spec * lightColor;

	vec3 result = (ambient + diffuse + specular) * objectColor;
	FragColor = vec4(result, 1.0);
}

