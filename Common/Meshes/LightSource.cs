using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;

public class LightSource : Mesh
{
    public Vector3 Color { get; set; }
    
    public readonly CubeMap CubeMap;

    public const float NearPlane = 1;
    
    public const float FarPlane = 100;

    public LightSource(Vertex[] vertices, Vector3 position, Vector3 color) : base(vertices, position)
    {
        Color = color;
        CubeMap = new CubeMap();
    }

    public override void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        var modelLs = Matrix4.CreateTranslation(
            GeomPorting.CreateTranslationTarget(Position, cameraPosition, curve, scale));
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelLs);
        shader.SetVector3("color", Color);

        GL.BindVertexArray(VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public Matrix4[] GetShadowTransforms()
    {
        Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), (float)CubeMap.Width / CubeMap.Height, NearPlane, FarPlane);
        Matrix4[] shadowTransforms = { // Not sure if order is correct
            shadowProj * Matrix4.LookAt(Position, Position + Vector3.UnitX, -Vector3.UnitY),
            shadowProj * Matrix4.LookAt(Position, Position - Vector3.UnitX, -Vector3.UnitY),
            shadowProj * Matrix4.LookAt(Position, Position + Vector3.UnitY, Vector3.UnitZ),
            shadowProj * Matrix4.LookAt(Position, Position - Vector3.UnitY, -Vector3.UnitZ),
            shadowProj * Matrix4.LookAt(Position, Position + Vector3.UnitZ, -Vector3.UnitY),
            shadowProj * Matrix4.LookAt(Position, Position - Vector3.UnitZ, -Vector3.UnitY)
        };

        return shadowTransforms;
    }
}
