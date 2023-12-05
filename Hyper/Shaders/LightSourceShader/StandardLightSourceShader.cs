namespace Hyper.Shaders.LightSourceShader;


internal class StandardLightSourceShader : AbstractLightSourceShader
{
    private StandardLightSourceShader() : base()
    { }

    public static StandardLightSourceShader Create()
    {
        return new StandardLightSourceShader();
    }
}
