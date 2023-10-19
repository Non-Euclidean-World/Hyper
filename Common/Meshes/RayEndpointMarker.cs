using Common.ResourceClasses;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;

public class RayEndpointMarker
{
    private readonly Vector3 _color;

    private readonly SphereResource _sphereResource = SphereResource.Instance;

    public RayEndpointMarker(Vector3 color)
    {
        _color = color;
    }

    public void Render(Shader shader, float scale, float curve, Vector3 rayPosition, Vector3 cameraPosition, float size)
    {
        var modelLs = Matrix4.CreateTranslation(GeomPorting.CreateTranslationTarget(rayPosition, cameraPosition, curve, scale));
        var scaleLs = Matrix4.CreateScale(scale);
        var sizeLs = Matrix4.CreateScale(size);
        shader.SetMatrix4("model", sizeLs * scaleLs * modelLs);
        shader.SetVector3("color", _color);

        GL.BindVertexArray(_sphereResource.Vaos[0]);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _sphereResource.Model.Meshes[0].FaceCount * 3);
    }
}
