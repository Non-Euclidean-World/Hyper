namespace Common.ResourceClasses;
public class TexturedObjectResource : ObjectResource
{
    public Texture Texture;

    protected TexturedObjectResource(string modelPath, string texturePath)
        : base(modelPath, texture: true)
    {
        Texture = Texture.LoadFromFile(texturePath);
    }
}
