using System.Collections.Generic;
using UnityEngine;
using System;

public class Shelf : MonoBehaviour, IInteractable
{
    [Header ("선반 고유 번호")]
    [SerializeField] private string shelfID;

    [Header("선반 파밍 설정")]
    [SerializeField] private int minItems = 2;
    [SerializeField] private int maxItems = 4;

    [SerializeField] private int shelfStorageSize = 15;

    [Header ("현재 선반에 보관된 아이템들")]
    [SerializeField] private InventorySlot[] shelfSlots;

    private bool isFarming = false;

    void Awake()
    {
        shelfSlots = new InventorySlot[shelfStorageSize];
        for (int i = 0; i < shelfStorageSize; i++)
        {
            shelfSlots[i] = new InventorySlot();
        }
    }

    //TODO : 각각 선반 고유 ID 생성을 통해 선반의 파밍 상태 확인(게임 끄고 켜도 유지)
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(shelfID))
        {
            shelfID = Guid.NewGuid().ToString();
        }
    }

    public void Interact(PlayerStat player)
    {
        
        if (!isFarming)
        {
            Debug.Log("상호작용 시도");
            GenerateRandomItem();
            isFarming = true;
        }

        InventoryManager.Instance.OpenStorage(shelfSlots);
        //UIManager.Instance.OpenStorageUI();
        //TODO : 선반 UI 열기

    }

    //선반에 최소, 최대 중 랜덤하게 아이템 생성하는 메서드
    private void GenerateRandomItem()
    {
        int itemCount = UnityEngine.Random.Range(minItems, maxItems + 1);
        int currentCount = 0;

        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if(currentCount >= itemCount) break;

            ItemData newItem = GlobalItemCountManager.Instance.DrawItem();
            if (newItem != null)
            {
                shelfSlots[i].item = newItem;
                currentCount++;
            }
        }
    }

    //선반에서 아이템 획득 시 호출되는 메서드
    public void RemoveItem(ItemData itemToRemove)
    {
        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if(shelfSlots[i].item == itemToRemove)
            {
                shelfSlots[i].item = null;
                Debug.Log($"선반에서 {itemToRemove.Name}제거");
                break;
            }
        }
    }
}
