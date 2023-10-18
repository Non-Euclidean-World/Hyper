using Common;

namespace Character;

public abstract class ModelResources
{
    public readonly int[] Vaos;

    public readonly Texture Texture;

    public readonly Assimp.Scene Model;

    private static readonly Assimp.AssimpContext Importer = new();

    protected ModelResources(string modelPath, string texturePath)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model, texture: true, bones: true);
        Texture = Texture.LoadFromFile(texturePath);
    }
}