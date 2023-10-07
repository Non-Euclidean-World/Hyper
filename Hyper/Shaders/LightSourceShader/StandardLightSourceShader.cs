namespace Hyper.Shaders.LightSourceShader;


internal class StandardLightSourceShader : AbstractLightSourceShader
{
    private StandardLightSourceShader(float globalScale) : base(globalScale)
    { }

    public static StandardLightSourceShader Create(float globalScale)
    {
        return new StandardLightSourceShader(globalScale);
    }
}
