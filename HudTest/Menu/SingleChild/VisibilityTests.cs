using Hud.Widgets;
using Hud.Widgets.SingleChild;
using Moq;

namespace HudTest.Menu.SingleChild;

[TestFixture]
public class VisibilityTests : MenuTestBase
{
    [Test]
    public void Visibility_ShouldRenderChild_WhenVisible()
    {
        // Arrange
        var visibility = new Visibility(WidgetMock.Object);
        
        // Act
        visibility.Render(Context);
        
        // Assert
        WidgetMock.Verify(w => w.Render(It.IsAny<Context>()), Times.Once);
    }
}