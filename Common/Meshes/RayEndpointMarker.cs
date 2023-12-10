using Common.ResourceClasses;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;

/// <summary>
/// Represents a marker indicating the endpoint of a ray in a 3D environment.
/// </summary>
public class RayEndpointMarker
{
    private readonly Vector3 _color;

    private readonly SphereResource _sphereResource = SphereResource.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="RayEndpointMarker"/> class with the specified color.
    /// </summary>
    /// <param name="color">The color of the marker.</param>
    public RayEndpointMarker(Vector3 color)
    {
        _color = color;
    }

    /// <summary>
    /// Renders the ray endpoint marker using a shader and specific parameters.
    /// </summary>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="scale">The scale factor applied to the marker.</param>
    /// <param name="curve">The curvature applied to the marker.</param>
    /// <param name="rayPosition">The position of the ray endpoint in 3D space.</param>
    /// <param name="cameraPosition">The position of the camera in 3D space.</param>
    /// <param name="size">The size of the marker.</param>
    public void Render(Shader shader, float scale, float curve, Vector3 rayPosition, Vector3 cameraPosition, float size)
    {
        var modelLs = Matrix4.CreateTranslation(GeomPorting.CreateTranslationTarget(rayPosition, cameraPosition, curve, scale));
        var scaleLs = Matrix4.CreateScale(scale);
        var sizeLs = Matrix4.CreateScale(size);
        shader.SetMatrix4("model", sizeLs * scaleLs * modelLs);
        shader.SetVector3("color", _color);

        GL.BindVertexArray(_sphereResource.Vaos[0]);
        GL.DrawElements(PrimitiveType.Triangles, _sphereResource.Model.Meshes[0].FaceCount * 3,
            DrawElementsType.UnsignedInt, 0);
    }
}
