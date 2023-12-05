using OpenTK.Mathematics;

namespace Hud.Shaders;

public interface IHudShader
{
    void SetProjection(Matrix4 projection);

    void SetUp(float aspectRatio);

    void SetColor(Vector4 color);

    void UseTexture(bool useTexture);

    void SetModel(Matrix4 model);
    
    void SetSpriteRect(Vector4 rect);
}