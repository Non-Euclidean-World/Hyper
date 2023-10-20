using Common.UserInput;
using FluentAssertions;
using Hyper.PlayerData.InventorySystem;
using Hyper.PlayerData.InventorySystem.Items;
using Hyper.PlayerData.InventorySystem.Items.Pickaxes;

namespace HyperTest.PlayerTests;

[TestFixture]
public class InventoryTests
{
    [Test]
    public void AddItemAddsItemToFirstEmptySlot()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var item = new IronPickaxe();

        // Act
        inventory.AddItem(item);

        // Assert
        inventory.Items[0, 0].Item!.Id.Should().Be(item.Id);
    }

    [Test]
    public void SwapWithHandShouldSwapDifferentItems()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var item = new IronPickaxe();
        var item2 = new Bullet();

        // Act
        inventory.AddItem(item);
        inventory.InHandItem = (item2, 1);
        inventory.SwapWithHand(0, 0);

        // Assert
        inventory.Items[0, 0].Item!.Id.Should().Be(item2.Id);
        inventory.InHandItem.Item!.Id.Should().Be(item.Id);
    }

    [Test]
    public void SwapWithHandShouldStackItems()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var item = new Bullet();
        int count1 = 10;
        int count2 = 5;

        // Act
        inventory.AddItem(item, count1);
        inventory.InHandItem = (item, count2);
        inventory.SwapWithHand(0, 0);

        // Assert
        inventory.Items[0, 0].Item!.Id.Should().Be(item.Id);
        inventory.Items[0, 0].Count.Should().Be(count1 + count2);
    }
}