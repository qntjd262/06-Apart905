using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public bool IsEmpty => item == null;

    public Action OnSlotChanged;

    public void AddItem(ItemData newItem, int count)
    {
        item = newItem;

        OnSlotChanged.Invoke();
    }
    public void RemoveItem(int count)
    {

    }
    public void Clear()
    {
        item = null;

        OnSlotChanged.Invoke();
    }
}