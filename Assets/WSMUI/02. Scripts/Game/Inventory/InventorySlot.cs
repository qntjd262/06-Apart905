using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;

    public bool IsEmpty => item == null || amount <= 0;

    public void AddItem(ItemData newItem, int count)
    {
        item = newItem;
        amount += count;
    }

    public void RemoveItem(int count)
    {
        amount -= count;
        if (amount <= 0)
        {
            Clear();
        }
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}