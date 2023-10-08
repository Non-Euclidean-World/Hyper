namespace Hyper.Shaders.ObjectShader;
internal class StandardObjectShader : AbstractObjectShader
{
    private StandardObjectShader(float globalScale) : base(globalScale)
    {
    }

    public static StandardObjectShader Create(float globalScale)
    {
        return new StandardObjectShader(globalScale);
    }
}
