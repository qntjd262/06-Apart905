using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public bool IsEmpty => item == null;

    public void AddItem(ItemData newItem, int count)
    {
        item = newItem;
    }
    public void RemoveItem(int count)
    {

    }
    public void Clear()
    {
        item = null;
    }
}