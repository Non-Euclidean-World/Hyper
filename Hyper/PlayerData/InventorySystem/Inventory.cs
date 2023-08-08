using System.ComponentModel;
using Hyper.PlayerData.InventorySystem.Items;
using Hyper.PlayerData.InventorySystem.Items.Tools;

namespace Hyper.PlayerData.InventorySystem;

public class Inventory
{
    private static Inventory? _instance;
    public static Inventory Instance { get => _instance ??= new Inventory(); }
    
    private const int ItemColumns = 10;
    
    private const int ItemRows = 5;
    
    private int _selectedItemIndex = 0;
    
    public bool isOpen = false;
    
    public readonly (Item? Item, int Count)[,] Items;
    public (Item? Item, int Count)[] Hotbar => Enumerable.Range(0, ItemColumns).Select(i => Items[i, 0]).ToArray();
    public Item? SelectedItem => Items[0, _selectedItemIndex].Item;
    
    private Inventory()
    {
        Items = new (Item? Item, int Count)[ItemColumns, ItemRows];
        AddItem(new Hammer());
        AddItem(new Sword());
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
        for (int i = 0; i < ItemColumns; i++)
        {
            for (int j = 0; j < ItemRows; j++)
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
}