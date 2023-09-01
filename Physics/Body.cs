using BepuPhysics;
using Common;
using Common.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Physics;

public class Body : IDisposable
{
    public Mesh Mesh { get; set; }
    
    public RigidPose RigidPose { get; set; }

    public Body(Mesh mesh)
    {
        Mesh = mesh;
    }
    
    public virtual void RenderFullDescription(Shader shader, float scale, Vector3 cameraPosition)
    {
        var translation = Matrix4.CreateTranslation((Conversions.ToOpenTKVector(RigidPose.Position) - cameraPosition) * scale);
        var scaleMatrix = Matrix4.CreateScale(scale);
        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(RigidPose.Orientation));

        shader.SetMatrix4("model", scaleMatrix * rotation * translation);

        GL.BindVertexArray(Mesh.VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.NumberOfVertices);
    }


    public void Dispose()
    {
        Mesh.Dispose();
    }
}