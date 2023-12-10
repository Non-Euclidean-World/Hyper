namespace Common.ResourceClasses;

/// <summary>
/// Represents a resource containing information about a 3D model, including its vertices, textures, and scene.
/// This is an abstract class.
/// </summary>
public abstract class ModelResource
{
    /// <summary>
    /// Vertex Array Objects (VAOs) containing model data.
    /// </summary>
    public readonly int[] Vaos;

    /// <summary>
    /// Texture associated with the model.
    /// </summary>
    public readonly Texture? Texture;

    /// <summary>
    /// Scene representing the model.
    /// </summary>
    public readonly Assimp.Scene Model;

    private static readonly Assimp.AssimpContext Importer = new();

    /// <summary>
    /// Creates a textured model.
    /// </summary>
    /// <param name="modelPath">The path to the model file.</param>
    /// <param name="texturePath">The path to the texture file.</param>
    /// <param name="isAnimated">Determines if the model contains animations.</param>
    protected ModelResource(string modelPath, string texturePath, bool isAnimated)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model, texture: true, bones: isAnimated);
        Texture = Texture.LoadFromFile(texturePath);
    }

    /// <summary>
    /// Creates an untextured model.
    /// </summary>
    /// <param name="modelPath">The path to the model file.</param>
    /// <param name="isAnimated">Determines if the model contains animations.</param>
    protected ModelResource(string modelPath, bool isAnimated = false)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model, texture: false, bones: isAnimated);
    }
}