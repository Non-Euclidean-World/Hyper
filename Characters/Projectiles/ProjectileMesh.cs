using BepuPhysics;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;
using Physics;

namespace Character.Projectiles;
/// <summary>
/// Represents a projectile mesh.
/// </summary>
public class ProjectileMesh : IDisposable
{
    private readonly Body _body;
    
    /// <summary>
    /// Size of the projectile.
    /// </summary>
    public Vector3 Size { get; private set; }

    private static readonly Vector3 Color = new(1, 0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectileMesh"/> class.
    /// </summary>
    /// <param name="size">Size of the mesh.</param>
    public ProjectileMesh(Vector3 size)
    {
        Size = size;
        _body = new Body(BoxMesh.Create(size, Color));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectileMesh"/> class.
    /// </summary>
    /// <param name="sizeX">Width of the mesh.</param>
    /// <param name="sizeY">Height of the mesh.</param>
    /// <param name="sizeZ">Length of the mesh.</param>
    public ProjectileMesh(float sizeX, float sizeY, float sizeZ)
        : this(new Vector3(sizeX, sizeY, sizeZ)) { }

    /// <summary>
    /// Updated the pose of the mesh.
    /// </summary>
    /// <param name="bodyPose"></param>
    public void Update(RigidPose bodyPose)
    {
        _body.RigidPose = bodyPose;
    }

    /// <summary>
    /// Renders the mesh.
    /// </summary>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="scale">The scale of the scene.</param>
    /// <param name="curve">The curvature of the scene.</param>
    /// <param name="cameraPosition">The position of the camera in the scene.</param>
    public void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        _body.RenderFullDescription(shader, scale, curve, cameraPosition);
    }

    /// <summary>
    /// Disposes the mesh.
    /// </summary>
    public void Dispose()
    {
        _body.Dispose();
    }
}
