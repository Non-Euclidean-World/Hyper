using BepuPhysics;
using Common;
using Common.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Physics;

/// <summary>
/// Represents a renderable body with a mesh and pose in a 3D environment.
/// </summary>
public class Body : IDisposable
{
    /// <summary>
    /// The mesh associated with the body.
    /// </summary>
    public Mesh Mesh { get; set; }

    /// <summary>
    /// The rigid pose of the body in the 3D space.
    /// </summary>
    public RigidPose RigidPose { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Body"/> class with the provided mesh.
    /// </summary>
    /// <param name="mesh">The mesh associated with the body.</param>
    public Body(Mesh mesh)
    {
        Mesh = mesh;
    }

    /// <summary>
    /// Renders the body using the provided shader, scale, curve, and camera position.
    /// </summary>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="scale">The scale factor applied to the body.</param>
    /// <param name="curve">The curvature of the scene.</param>
    /// <param name="cameraPosition">The position of the camera in the 3D space.</param>
    public virtual void RenderFullDescription(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        var translation = Matrix4.CreateTranslation(
            GeomPorting.CreateTranslationTarget(Conversions.ToOpenTKVector(RigidPose.Position), cameraPosition, curve, scale));
        var scaleMatrix = Matrix4.CreateScale(scale);
        var rotation = Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(RigidPose.Orientation));

        shader.SetMatrix4("model", scaleMatrix * rotation * translation);
        shader.SetMatrix4("normalRotation", rotation);

        GL.BindVertexArray(Mesh.VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.Vertices.Length);
    }


    public void Dispose()
    {
        Mesh.Dispose();
    }
}