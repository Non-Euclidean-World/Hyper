namespace Common.ResourceClasses;

public abstract class ModelResource
{
    public readonly int[] Vaos;

    public readonly Texture? Texture;

    public readonly Assimp.Scene Model;

    private static readonly Assimp.AssimpContext Importer = new();

    /// <summary>
    /// Creates a textured model.
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="texturePath"></param>
    /// <param name="isAnimated"></param>
    protected ModelResource(string modelPath, string texturePath, bool isAnimated)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model, texture: true, bones: isAnimated);
        Texture = Texture.LoadFromFile(texturePath);
    }

    /// <summary>
    /// Creates an untextured model.
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="isAnimated"></param>
    protected ModelResource(string modelPath, bool isAnimated = false)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model, texture: false, bones: isAnimated);
    }
}