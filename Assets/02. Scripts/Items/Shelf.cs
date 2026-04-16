using System.Collections.Generic;
using UnityEngine;
using System;

public class Shelf : MonoBehaviour, IInteractable
{
    [Header("선반 고유 번호")]
    [SerializeField] private string shelfID;

    [Header("선반 파밍 설정")]
    [SerializeField] private int minItems = 2;
    [SerializeField] private int maxItems = 4;
    [SerializeField] private int storageSize = 20;

    [Header("현재 선반에 보관된 아이템들")]
    private InventorySlot[] shelfSlots;

    private bool isFarming = false;

    private void Awake()//추가 부분
    {
        // 슬롯 배열 초기화
        shelfSlots = new InventorySlot[storageSize];
        for (int i = 0; i < storageSize; i++)
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

        //TODO : 선반 UI 열기 추가 부분
        UIManager.Instance.ToggleStorage(shelfSlots);
    }

    private void GenerateRandomItem()
    {
        int itemCount = UnityEngine.Random.Range(minItems, maxItems + 1);
        // shelfInventory.Clear();

        // for (int i = 0; i < itemCount; i++)
        // {
        //     //newitem에 아이템카운트매니저에서 랜덤으로 아이템 하나 뽑아서 넣기
        //     //TODO : 아이템 드로우 시 선반에 아이템 그려주기
        //     ItemData newItem = GlobalItemCountManager.Instance.DrawItem();
        //     if (newItem != null)
        //     {
        //         shelfInventory.Add(newItem);
        //     }
        // }
        //수정 부분
        foreach (var slot in shelfSlots)
        {
            slot.item = null;
        }

        for (int i = 0; i < itemCount; i++)
        {
            // GlobalItemCountManager에서 아이템 드로우
            ItemData newItem = GlobalItemCountManager.Instance.DrawItem();
            if (newItem != null)
            {
                // 생성된 개수만큼 앞에서부터 슬롯에 할당
                shelfSlots[i].item = newItem;
            }
        }
    }

    //선반에서 아이템 획득 시 호출되는 메서드
    public void RemoveItem(ItemData itemToRemove)
    {
        // if (shelfInventory.Contains(itemToRemove))
        // {
        //     shelfInventory.Remove(itemToRemove);
        //     Debug.Log($"선반에서 {itemToRemove.Name}이 제거 됨");
        // }
        //수정 부분
        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if (shelfSlots[i].item == itemToRemove)
            {
                shelfSlots[i].item = null;
                Debug.Log($"선반 슬롯 {i}에서 {itemToRemove.Name}이 제거됨");
                break;
            }
        }
    }
}
