using Common.UserInput;
using Hyper.Controllers;
using Hyper.HUD.InventoryRendering;
using Hyper.PlayerData.InventorySystem.Items;
using Hyper.PlayerData.InventorySystem.Items.Tools;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.PlayerData.InventorySystem;

public class Inventory : IInputSubscriber
{
    private static Inventory? _instance;
    public static Inventory Instance { get => _instance ??= new Inventory(); }
    
    public const int Columns = 10;
    
    public const int Rows = 4;
    
    public int SelectedItemIndex = 0;
    
    public bool IsOpen = false;
    
    public readonly (Item? Item, int Count)[,] Items;
    
    public (Item? Item, int Count)[] Hotbar => Enumerable.Range(0, Columns).Select(i => Items[i, 0]).ToArray();
    
    public Item? SelectedItem => Items[0, SelectedItemIndex].Item;

    public (Item? Item, int Count) InHandItem;
    
    private Inventory()
    {
        Items = new (Item? Item, int Count)[Columns, Rows];
        RegisterCallbacks();
        
        for (int i = 0; i < 12; i++)
        {
            AddItem(new Sword());
        }
        for (int i = 0; i < 3; i++)
        {
            AddItem(new Hammer());
        }
        AddItem(new Rock(), 23);
        AddItem(new Rock(), 7);
    }

    public void UseItem() => SelectedItem?.Use();
    
    public void AddItem(Item item, int count = 1)
    {
        int x, y;
        if (item.IsStackable)
        {
            bool hasItem = TryGetFirstSlotWithItem(item, out x, out y);
            if (hasItem)
            {
                Items[x, y].Count++;
                return;
            }
        }
        
        bool isEmptySlot = TryGetFirstEmptySlot(out x, out y);
        if (isEmptySlot) Items[x, y] = (item, count);
    }

    private bool TryGetFirstEmptySlot(out int x, out int y)
    {
        for (int j = 0; j < Rows; j++)
        {
            for (int i = 0; i < Columns; i++)
            {
                if (Items[i, j].Item is null)
                {
                    (x, y) = (i, j);
                    return true;
                }
            }
        }

        (x, y) = (-1, -1);
        return false;
    }
    
    private bool TryGetFirstSlotWithItem(Item item, out int x, out int y)
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (Items[i, j].Item == item)
                {
                    (x, y) = (i, j);
                    return true;
                }
            }
        }

        (x, y) = (-1, -1);
        return false;
    }

    private void DropItem()
    {
        InHandItem.Item = null;
        InHandItem.Count = 0;
    }

    private (Item?, int) RemoveItem(int x, int y)
    {
        var item = Items[x, y];
        Items[x, y] = (null, 0);
        
        return item;
    }
    
    private void SwapWithHand(int x, int y)
    {
        var temp = InHandItem;
        InHandItem = RemoveItem(x, y);
        Items[x, y] = temp;
    }

    private bool TryGetPosition(out int x, out int y)
    {
        var mousePosition = HudController.GetMousePosition();

        if (TryGetHotbarPosition(mousePosition, out x, out y)) return true;
        if (TryGetInventoryPosition(mousePosition, out x, out y)) return true;
        return false;
    }

    private bool TryGetHotbarPosition(Vector2 mousePosition, out int x, out int y)
    {
        float cellSize = 2 * InventoryRenderer.HotbarSizeY;

        float gridTopLeftX = InventoryRenderer.HotbarPosition.X - (cellSize * Columns / 2);
        float gridTopLeftY = InventoryRenderer.HotbarPosition.Y - (cellSize / 2);

        x = (int)((mousePosition.X - gridTopLeftX) / cellSize);
        y = (int)((mousePosition.Y - gridTopLeftY) / cellSize);
        
        return x is >= 0 and < Columns && y == 0;
    }

    private bool TryGetInventoryPosition(Vector2 mousePosition, out int x, out int y)
    {
        float cellSize = 2 * InventoryRenderer.HotbarSizeY;

        float gridTopLeftX = InventoryRenderer.InventoryPosition.X - (cellSize * Columns / 2);
        float gridTopLeftY = InventoryRenderer.InventoryPosition.Y - (cellSize * (Rows - 1) / 2);

        x = (int)((mousePosition.X - gridTopLeftX) / cellSize);
        y = Rows - (int)((mousePosition.Y - gridTopLeftY) / cellSize) - 1;
        
        return x is >= 0 and < Columns && y is > 0 and < Rows;
    }

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;

        context.RegisterKeyDownCallback(Keys.D0, () => SelectedItemIndex = 9);
        context.RegisterKeyDownCallback(Keys.D1, () => SelectedItemIndex = 0);
        context.RegisterKeyDownCallback(Keys.D2, () => SelectedItemIndex = 1);
        context.RegisterKeyDownCallback(Keys.D3, () => SelectedItemIndex = 2);
        context.RegisterKeyDownCallback(Keys.D4, () => SelectedItemIndex = 3);
        context.RegisterKeyDownCallback(Keys.D5, () => SelectedItemIndex = 4);
        context.RegisterKeyDownCallback(Keys.D6, () => SelectedItemIndex = 5);
        context.RegisterKeyDownCallback(Keys.D7, () => SelectedItemIndex = 6);
        context.RegisterKeyDownCallback(Keys.D8, () => SelectedItemIndex = 7);
        context.RegisterKeyDownCallback(Keys.D9, () => SelectedItemIndex = 8);
        
        context.RegisterKeyDownCallback(Keys.E, () =>
        {
            IsOpen = !IsOpen;
            Window.Instance.CursorState = IsOpen ? CursorState.Normal : CursorState.Grabbed;
            DropItem();
        });
        
        context.RegisterMouseButtonDownCallback(MouseButton.Left, () =>
        {
            if (!IsOpen) return;

            var isMouseOnInventory = TryGetPosition(out int x, out int y);
            if (isMouseOnInventory) SwapWithHand(x, y);
        });
    }
}