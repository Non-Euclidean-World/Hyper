using BepuPhysics;
using OpenTK.Mathematics;

namespace Hyper.Meshes;
internal class ProjectileMesh : IDisposable
{
    public Mesh Body { get; private set; }
    public float Size { get; private set; }

    public ProjectileMesh(float size)
    {
        Size = size;
        Body = BoxMesh.Create(new Vector3(size, size, size));
    }

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
