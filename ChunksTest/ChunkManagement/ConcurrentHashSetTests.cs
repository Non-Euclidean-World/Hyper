using Chunks.ChunkManagement;
using FluentAssertions;

namespace ChunksTest.ChunkManagement;

public class ConcurrentHashSetTests
{
    [Test]
    public void Add_ShouldAddItemAndReturnTrue_WhenNewItem()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        int item = 1;

        // Act
        bool result = set.Add(item);

        // Assert
        result.Should().BeTrue();
        set.Contains(item).Should().BeTrue();
    }

    [Test]
    public void Add_ShouldReturnFalse_WhenAddingDuplicateItem()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        int item = 1;
        set.Add(item);

        // Act
        bool result = set.Add(item);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Remove_ShouldRemoveItemAndReturnTrue_WhenItemExists()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        int item = 1;
        set.Add(item);

        // Act
        bool result = set.Remove(item);

        // Assert
        result.Should().BeTrue();
        set.Contains(item).Should().BeFalse();
    }

    [Test]
    public void Remove_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        int item = 1;

        // Act
        bool result = set.Remove(item);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Contains_ShouldReturnTrue_WhenItemExists()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        int item = 1;
        set.Add(item);

        // Act
        bool result = set.Contains(item);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Contains_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        int item = 1;

        // Act
        bool result = set.Contains(item);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var set = new ConcurrentHashSet<int>();
        set.Add(1);
        set.Add(2);

        // Act
        set.Clear();

        // Assert
        set.Contains(1).Should().BeFalse();
        set.Contains(2).Should().BeFalse();
    }
}