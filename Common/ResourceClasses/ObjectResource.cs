namespace Common.ResourceClasses;
public class ObjectResource
{
    public readonly int[] Vaos;

    public readonly Assimp.Scene Model;

    protected static readonly Assimp.AssimpContext Importer = new();

    // TODO this absolutely sucks
    protected ObjectResource(string modelPath, bool texture = false)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model, texture);
    }
}
