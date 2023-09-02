using BepuPhysics;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;
using Physics;

namespace Character.Projectiles;
public class ProjectileMesh : IDisposable
{
    public Body Body { get; private set; }
    public Vector3 Size { get; private set; }

    public ProjectileMesh(Vector3 size)
    {
        Size = size;
        Body = new Body(BoxMesh.Create(size));
    }

    public ProjectileMesh(float sizeX, float sizeY, float sizeZ)
        : this(new Vector3(sizeX, sizeY, sizeZ)) { }

    public void Update(RigidPose bodyPose)
    {
        Body.RigidPose = bodyPose;
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        Body.RenderFullDescription(shader, scale, cameraPosition);
    }

    public void Dispose()
    {
        Body.Dispose();
    }
}
