using AScene = Assimp.Scene;
namespace Hyper.Animation;

public class Model
{
    private readonly int _vao;
    private readonly Texture _texture;
    private readonly AScene _model;
    
    public Model()
    {
        var path = "Animation/Resources/model.dae";
        _model = ModelLoader.GetModel(path);
        _vao = ModelLoader.GetVao(_model);
        _texture = Texture.LoadFromFile("Animation/Resources/diffuse.png");
    }
}