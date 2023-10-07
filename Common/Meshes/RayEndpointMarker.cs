using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;
// TODO right now this is an exact copy of LightSource class
// Do something more visually pleasing at some point
public class RayEndpointMarker : Mesh
{
    public Vector3 Color { get; set; }

    public RayEndpointMarker(Vertex[] vertices, Vector3 position, Vector3 color) : base(vertices, position)
    {
        Color = color;
    }

    public override void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        var modelLs = Matrix4.CreateTranslation(GeomPorting.CreateTranslationTarget(Position, cameraPosition, curve, scale));
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelLs);
        shader.SetVector3("color", Color);

        GL.BindVertexArray(VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }
}
