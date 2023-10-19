namespace Common.ResourceClasses;
public class ObjectResource
{
    public readonly int[] Vaos;

    public readonly Assimp.Scene Model;

    private static readonly Assimp.AssimpContext Importer = new();

    protected ObjectResource(string modelPath)
    {
        Model = ModelLoader.GetModel(modelPath, Importer);
        Vaos = ModelLoader.GetVaos(Model);
    }
}
