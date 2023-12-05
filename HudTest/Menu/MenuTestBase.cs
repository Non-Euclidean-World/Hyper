using Hud.Shaders;
using Hud.Widgets;
using Moq;
using OpenTK.Mathematics;

namespace HudTest.Menu;

public abstract class MenuTestBase
{
    protected Mock<Widget> WidgetMock;
    protected static readonly Vector2 Size = new(100, 100);
    protected Context Context;
    protected Vector2 ContextPosition = Vector2.Zero;
    protected Vector2 ContextSize = Vector2.One;
    
    [SetUp]
    public void Setup()
    {
        WidgetMock = new Mock<Widget>();
        WidgetMock.Setup(w => w.GetSize()).Returns(Size);
        WidgetMock.Setup(w => w.Render(It.IsAny<Context>()));
        
        var shaderMock = new Mock<HudShader>();
        Context = new Context(shaderMock.Object, ContextPosition, ContextSize);
    }
    
    [TearDown]
    public void Cleanup()
    {
        WidgetMock.Reset();
    }
}