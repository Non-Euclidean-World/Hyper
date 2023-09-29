using Common.UserInput;
using FluentAssertions;
using Player.InventorySystem;
using Player.InventorySystem.Items;
using Player.InventorySystem.Items.Tools;

namespace PlayerTest;

[TestFixture]
public class InventoryTests
{
    [Test]
    public void AddItemAddsItemToFirstEmptySlot()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var item = new Sword();

        // Act
        inventory.AddItem(item);

        // Assert
        inventory.Items[0, 0].Item!.ID.Should().Be(item.ID);
    }

    [Test]
    public void SwapWithHandShouldSwapDifferentItems()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var item = new Sword();
        var item2 = new Hammer();

        // Act
        inventory.AddItem(item);
        inventory.InHandItem = (item2, 1);
        inventory.SwapWithHand(0, 0);

        // Assert
        inventory.Items[0, 0].Item!.ID.Should().Be(item2.ID);
        inventory.InHandItem.Item!.ID.Should().Be(item.ID);
    }

    [Test]
    public void SwapWithHandShouldStackItems()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var item = new Rock();
        int count1 = 10;
        int count2 = 5;

        // Act
        inventory.AddItem(item, count1);
        inventory.InHandItem = (item, count2);
        inventory.SwapWithHand(0, 0);

        // Assert
        inventory.Items[0, 0].Item!.ID.Should().Be(item.ID);
        inventory.Items[0, 0].Count.Should().Be(count1 + count2);
    }
}