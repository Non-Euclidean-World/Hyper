using OpenTK.Mathematics;

namespace Hyper.Animation.Characters;

internal abstract class Character
{
    public Vector3 Position { get; set; }
    
    protected Model Model;

    public void Render(Shader shader, float scale, Vector3 cameraPosition) => 
        Model.Render(shader, scale, cameraPosition, Position);
}