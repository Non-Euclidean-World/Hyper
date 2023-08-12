using BepuPhysics;
using OpenTK.Mathematics;

namespace Hyper.Animation.Characters;

internal abstract class Character
{
    public Vector3 Position { get; set; }

    public Matrix4 Rotation { get; set; }

    public RigidPose RigidPose { get; set; }

    public float Scale { get; set; }

    protected Model Model = null!;

    protected Character(Vector3 position, float scale = 1f)
    {
        Position = position;
        Rotation = Matrix4.Identity;
        Scale = scale;
    }

    public void Render(Shader shader, float worldScale, Vector3 cameraPosition) =>
        Model.Render(shader, worldScale * Scale, cameraPosition, Position, Rotation);

    public void RenderFullDescription(Shader shader, float scale, Vector3 cameraPosition)
     => Model.RenderFromPose(RigidPose, shader, scale * Scale, cameraPosition);
}