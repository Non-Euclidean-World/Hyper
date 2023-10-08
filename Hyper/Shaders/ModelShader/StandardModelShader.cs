namespace Hyper.Shaders.ModelShader;
internal class StandardModelShader : AbstractModelShader
{
    private StandardModelShader(float globalScale)
        : base(globalScale)
    { }

    public static StandardModelShader Create(float globalScale)
    {
        return new StandardModelShader(globalScale);
    }
}
