namespace Hyper.Shaders.LightSourceShader;
public class StandardLightSourceShader : AbstractLightSourceShader
{
    private StandardLightSourceShader(float globalScale) : base(globalScale)
    { }

    public static StandardLightSourceShader Create(float globalScale)
    {
        return new StandardLightSourceShader(globalScale);
    }
}
