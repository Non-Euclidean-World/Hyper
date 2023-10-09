using Common;

namespace Character;

public abstract class ModelResources
{
    public readonly int[] Vaos;

    public readonly Texture Texture;

    public readonly Assimp.Scene Model;
    
    protected ModelResources(string modelPath, string texturePath)
    {
        Model = ModelLoader.GetModel(modelPath);
        Vaos = ModelLoader.GetVaos(Model);
        Texture = Texture.LoadFromFile(texturePath);
    }
}