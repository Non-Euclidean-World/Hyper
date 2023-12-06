using Common.UserInput;
using FluentAssertions;
using Hyper.PlayerData.InventorySystem;
using Hyper.PlayerData.InventorySystem.Items;
using Hyper.PlayerData.InventorySystem.Items.Pickaxes;

namespace HyperTest.PlayerData.InventorySystem;

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
        var count1 = 10;
        var count2 = 5;

        // Act
        inventory.AddItem(item, count1);
        inventory.InHandItem = (item, count2);
        inventory.SwapWithHand(0, 0);

        // Assert
        inventory.Items[0, 0].Item!.Id.Should().Be(item.Id);
        inventory.Items[0, 0].Count.Should().Be(count1 + count2);
    }

    [Test]
    public void ShouldRemoveItem()
    {
        // Arrange
        var context = new Context();
        var inventory = new Inventory(context);
        var count0 = 1;
        var count1 = 0;
        inventory.AddItem(new Lamp(), count0);

        // Act
        inventory.Items[0, 0].Count.Should().Be(1);

        var result0 = inventory.TryRemoveItem("lamp");
        result0.Should().BeTrue();
        inventory.Items[0, 0].Count.Should().Be(count1);

        var result1 = inventory.TryRemoveItem("lamp");
        result1.Should().BeFalse();
        inventory.Items[0, 0].Count.Should().Be(count1);
    }
}