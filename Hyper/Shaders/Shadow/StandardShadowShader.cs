namespace Hyper.Shaders.Shadow;

public class StandardShadowShader : AbstractShadowShader
{
    private StandardShadowShader(float globalScale) : base(globalScale)
    {
    }

    public static StandardShadowShader Create(float globalScale)
    {
        return new StandardShadowShader(globalScale);
    }
}