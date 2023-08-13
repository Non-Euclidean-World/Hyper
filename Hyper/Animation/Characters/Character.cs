using BepuPhysics;
using OpenTK.Mathematics;

namespace Hyper.Animation.Characters;

internal abstract class Character
{
    public RigidPose RigidPose { get; set; }

    public float Scale { get; set; }

    public Model Model;

    protected Character(float scale, Model model)
    {
        Scale = scale;
        Model = model;
    }

    public void RenderFullDescription(Shader shader, float globalScale, Vector3 cameraPosition, Vector3 localTranslation)
     => Model.RenderFromPose(RigidPose, shader, globalScale, cameraPosition, Scale, localTranslation);
}