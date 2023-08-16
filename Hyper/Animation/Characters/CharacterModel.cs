using BepuPhysics;
using Hyper.TypingUtils;
using OpenTK.Mathematics;

namespace Hyper.Animation.Characters;

internal abstract class CharacterModel
{
    public RigidPose RigidPose { get; set; }

    public float Scale { get; set; }

    public Model Model;

    protected CharacterModel(float scale, Model model)
    {
        Scale = scale;
        Model = model;
    }

    public void Render(Shader shader, float globalScale, Vector3 cameraPosition, Vector3 localTranslation)
     => Model.Render(RigidPose, shader, globalScale, cameraPosition, Scale, localTranslation);

    // in general this can depend on the properties of the character e.g. size etc
    public Vector3 GetThirdPersonCameraOffset(Camera camera)
        => camera.Up * 1f - camera.Front * 5f;

    public System.Numerics.Vector3 GetCharacterRay(Vector3 viewDirection, float length)
        => RigidPose.Position + Conversions.ToNumericsVector(viewDirection) * length;
}