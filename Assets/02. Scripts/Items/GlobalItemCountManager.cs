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

    //아이템 데이터 매니저에서 각각의 아이템 스폰 카운트만큼 아이템 덱에 추가
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

    //아이템 덱에서 랜덤으로 아이템 하나 뽑아서 반환, 뽑힌 아이템은 제거
    public ItemData DrawItem()
    {
        if(ItemDeck.Count == 0) return null;

        int idx = UnityEngine.Random.Range(0,ItemDeck.Count);
        ItemData item = ItemDeck[idx];
        ItemDeck.RemoveAt(idx);
        return item;
    }
}
