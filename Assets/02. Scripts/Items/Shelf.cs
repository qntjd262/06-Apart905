using System.Collections.Generic;
using UnityEngine;
using System;

public class Shelf : MonoBehaviour, IInteractable
{
    [Header("선반 고유 번호")]
    [SerializeField] private string shelfID;

    [Header ("고정 스폰 아이템")]
    [SerializeField] private List<ItemData> fixedItems = new List<ItemData>();

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
    foreach (var slot in shelfSlots)
    {
        slot.item = null;
    }

    int currentSlotIndex = 0;

    Debug.Log($"=== [Shelf] 파밍 생성 시작 ===");

    // 고정 아이템 배치
    if (fixedItems != null && fixedItems.Count > 0)
    {
        Debug.Log($"[Shelf] 고정 아이템 개수: {fixedItems.Count}개 감지됨.");
        foreach (ItemData item in fixedItems)
        {
            if (currentSlotIndex < shelfSlots.Length)
            {
                if (item != null)
                {
                    string searchName = item.itemName; 

                    ItemData officialItem = GlobalItemCountManager.Instance.GetItemByName(searchName);

                    if (officialItem != null)
                    {
                        shelfSlots[currentSlotIndex].AddItem(officialItem, 1);
                        Debug.Log($"[Shelf] 고정 아이템 생성 성공: {officialItem.name} (슬롯 {currentSlotIndex})");
                        currentSlotIndex++;
                    }
                    else
                    {
                        Debug.LogError($"[Shelf] 고정 아이템 생성 실패! 매니저에서 '{searchName}'을(를) 찾지 못함.");
                    }
                }
            }
        }
    }
    
    // 랜덤 아이템 배치
    int ranItemCount = UnityEngine.Random.Range(minItems, maxItems + 1);
    Debug.Log($"[Shelf] 랜덤 아이템 생성 시도 개수: {ranItemCount}개");

    for (int i = 0; i < ranItemCount; i++)
    {
        if (currentSlotIndex >= shelfSlots.Length) break;

        ItemData newItem = GlobalItemCountManager.Instance.DrawItem();
        if (newItem != null)
        {
            shelfSlots[currentSlotIndex].AddItem(newItem, 1);
            Debug.Log($"[Shelf] 랜덤 아이템 생성 성공: {newItem.name} (슬롯 {currentSlotIndex})");
            currentSlotIndex++;
        }
        else
        {
            Debug.LogWarning("[Shelf] 매니저의 ItemDeck이 비어있어 랜덤 아이템을 뽑지 못했습니다.");
        }
    }

    Debug.Log($"=== [Shelf] 파밍 생성 종료 (총 배치된 아이템: {currentSlotIndex}개) ===");
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
                shelfSlots[i].Clear();
                Debug.Log($"선반 슬롯 {i}에서 {itemToRemove.Name}이 제거됨");
                break;
            }
        }
    }

    public string GetInteractText()
    {
        return "조사하기";
    }

    public Constants.InteractType GetInteractType()
    {
        return Constants.InteractType.Search;
    }
}
