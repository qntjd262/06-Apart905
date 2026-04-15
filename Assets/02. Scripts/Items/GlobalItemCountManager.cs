using System.Collections.Generic;
using UnityEngine;

public class GlobalItemCountManager : MonoBehaviour
{
    public static GlobalItemCountManager Instance;

    private List<ItemData> ItemDeck = new List<ItemData>();

    void Awake()
    {
        Instance = this;
    }

    public void InitializeItemDeck(List<ItemData> allItems)
    {
        ItemDeck.Clear();
        foreach(var item in allItems)
        {
            for(int i = 0; i< item.SpawnCount; i++)
            {
                ItemDeck.Add(item);
            }
        }
    }

    public ItemData DrawItem()
    {
        if(ItemDeck.Count == 0) return null;

        int idx = UnityEngine.Random.Range(0,ItemDeck.Count);
        ItemData item = ItemDeck[idx];
        ItemDeck.RemoveAt(idx);
        return item;
    }
}
