using Common.UserInput;
using FluentAssertions;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CommonTest.UserInput;

[TestFixture]
internal class ContextTest
{
    [Test]
    public void ShouldRegisterKeys()
    {
        // Arrange
        Context subject = new Context();
        List<Keys> keys = new List<Keys> { Keys.A, Keys.B };

        // Act
        subject.RegisterKeys(keys);

        // Assert
        subject.UsedKeys.Contains(Keys.A).Should().BeTrue();
        subject.UsedKeys.Contains(Keys.B).Should().BeTrue();
        subject.UsedKeys.Contains(Keys.C).Should().BeFalse();
    }

    [Test]
    public void ShouldChangeKeyState()
    {
        // Arrange
        Context subject = new Context();
        List<Keys> keys = new List<Keys> { Keys.A, Keys.B };

        // Act
        subject.RegisterKeys(keys);
        foreach (var callback in subject.KeyDownCallbacks[Keys.A])
        {
            callback();
        }

        // Assert
        subject.HeldKeys[Keys.A].Should().BeTrue();
        subject.HeldKeys[Keys.B].Should().BeFalse();
    }

    [Test]
    public void ShouldRegisterKeyCallback()
    {
        // Arrange
        Context subject = new Context();
        List<Keys> keys = new List<Keys> { Keys.A };
        int callbackCallCount = 0;
        Action callback = () => callbackCallCount++;

        // Act
        subject.RegisterKeys(keys);
        subject.RegisterKeyDownCallback(Keys.A, callback);
        foreach (var c in subject.KeyDownCallbacks[Keys.A])
        {
            c();
        }

        // Assert
        callbackCallCount.Should().Be(1);
    }

    [Test]
    public void ShouldRegisterMouseButtons()
    {
        // Arrange
        Context subject = new Context();
        List<MouseButton> mouseButtons = new List<MouseButton> { MouseButton.Left, MouseButton.Right };

        // Act
        subject.RegisterMouseButtons(mouseButtons);

        // Assert
        subject.UsedMouseButtons.Contains(MouseButton.Left).Should().BeTrue();
        subject.UsedMouseButtons.Contains(MouseButton.Right).Should().BeTrue();
        subject.UsedMouseButtons.Contains(MouseButton.Middle).Should().BeFalse();
    }

    [Test]
    public void ShouldChangeMouseButtonState()
    {
        // Arrange
        Context subject = new Context();
        List<MouseButton> mouseButtons = new List<MouseButton> { MouseButton.Left, MouseButton.Right };

        // Act
        subject.RegisterMouseButtons(mouseButtons);
        foreach (var callback in subject.ButtonDownCallbacks[MouseButton.Left])
        {
            callback();
        }

        // Assert
        subject.HeldButtons[MouseButton.Left].Should().BeTrue();
        subject.HeldButtons[MouseButton.Right].Should().BeFalse();
    }

    [Test]
    public void ShouldRegisterMouseButtonCallbacks()
    {
        // Arrange
        Context subject = new Context();
        List<MouseButton> mouseButtons = new List<MouseButton> { MouseButton.Left };
        int callbackCallCount = 0;
        Action callback = () => callbackCallCount++;

        // Act
        subject.RegisterMouseButtons(mouseButtons);
        subject.RegisterMouseButtonDownCallback(MouseButton.Left, callback);
        foreach (var c in subject.ButtonDownCallbacks[MouseButton.Left])
        {
            c();
        }

        // Assert
        callbackCallCount.Should().Be(1);
    }
}
