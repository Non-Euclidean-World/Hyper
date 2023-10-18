namespace Common;
public class ObjectResource
{
    public readonly int[] Vaos;

    public readonly Assimp.Scene Model;

    protected ObjectResource(string modelPath)
    {
        Model = ObjectLoader.GetModel(modelPath);
        Vaos = ObjectLoader.GetVaos(Model);
    }
}
