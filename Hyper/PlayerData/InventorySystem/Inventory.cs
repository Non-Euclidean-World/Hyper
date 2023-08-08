using Hyper.PlayerData.InventorySystem.Items;
using Hyper.UserInput;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.PlayerData.InventorySystem;

public class Inventory : IInputSubscriber
{
    private static Inventory? _instance;
    public static Inventory Instance { get => _instance ??= new Inventory(); }
    
    public const int ItemColumns = 10;
    
    public const int ItemRows = 4;
    
    public int SelectedItemIndex = 0;
    
    public bool IsOpen = false;
    
    public readonly (Item? Item, int Count)[,] Items;
    
    public (Item? Item, int Count)[] Hotbar => Enumerable.Range(0, ItemColumns).Select(i => Items[i, 0]).ToArray();
    
    public Item? SelectedItem => Items[0, SelectedItemIndex].Item;
    
    private Inventory()
    {
        Items = new (Item? Item, int Count)[ItemColumns, ItemRows];
        RegisterCallbacks();
        
        for (int i = 0; i < 12; i++)
        {
            AddItem(new Sword());
        }
    }

    public void UseItem() => SelectedItem?.Use();
    
    public void AddItem(Item item)
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
        if (isEmptySlot) Items[x, y] = (item, 1);
    }

    private bool TryGetFirstEmptySlot(out int x, out int y)
    {
        for (int j = 0; j < ItemRows; j++)
        {
            for (int i = 0; i < ItemColumns; i++)
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
        for (int i = 0; i < ItemColumns; i++)
        {
            for (int j = 0; j < ItemRows; j++)
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
        
        context.RegisterKeyDownCallback(Keys.E, () => IsOpen = !IsOpen);
    }
}