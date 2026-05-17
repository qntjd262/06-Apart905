using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private int bagSize = 20;
    [SerializeField] private int quickSlotSize = 5;

    // 외부에서는 읽기만 가능하도록 프로퍼티로 캡슐화
    public InventorySlot[] BagSlots { get; private set; }
    public InventorySlot[] QuickSlots { get; private set; }

    public Action OnBagUpdated;
    public Action OnQuickSlotUpdated;

    public InventorySlot[] CurrentStorageSlots { get; private set; }
    public Action OnStorageUpdated;

    public static System.Action OnInventoryChanged;

    protected override void Awake()
    {
        base.Awake();
        InitializeInventory();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
    protected override void OnSceneUnloaded(Scene scene) { }

    private void InitializeInventory()
    {
        BagSlots = new InventorySlot[bagSize];
        for (int i = 0; i < bagSize; i++)
        {
            BagSlots[i] = new InventorySlot();
        }

        QuickSlots = new InventorySlot[quickSlotSize];
        for (int i = 0; i < quickSlotSize; i++)
        {
            QuickSlots[i] = new InventorySlot();
        }
    }

    // public bool AddItem(ItemData itemToAdd, int amount)
    // {
        // // 1. 겹칠 수 있는 아이템인지 먼저 확인
        // if (itemToAdd.maxStack > 1)
        // {
        //     for (int i = 0; i < bagSize; i++)
        //     {
        //         if (!BagSlots[i].IsEmpty && BagSlots[i].item == itemToAdd && BagSlots[i].amount < itemToAdd.maxStack)
        //         {
        //             // 해당 슬롯에 얼마나 더 넣을 수 있는지 계산
        //             int spaceLeft = itemToAdd.maxStack - BagSlots[i].amount;
        //             int amountToAdd = Mathf.Min(spaceLeft, amount);
                    
        //             BagSlots[i].amount += amountToAdd;
        //             amount -= amountToAdd; // 남은 개수 차감

        //             // 다 넣었다면 갱신 후 종료
        //             if (amount <= 0)
        //             {
        //                 OnBagUpdated?.Invoke();
        //                 return true;
        //             }
        //         }
        //     }
        // }

        // // 2. 겹치지 못했거나 남은 개수가 있다면 빈 슬롯 탐색
        // if (amount > 0)
        // {
        //     for (int i = 0; i < bagSize; i++)
        //     {
        //         if (BagSlots[i].IsEmpty)
        //         {
        //             BagSlots[i].item = itemToAdd;
        //             BagSlots[i].amount = amount;
        //             OnBagUpdated?.Invoke();
        //             return true;
        //         }
        //     }
        // }

        // Debug.Log("가방이 꽉 차서 아이템을 획득할 수 없습니다.");
        // return false;
    // }

    public bool AddItem(ItemData itemToAdd)
    {
        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item == null)
            {
                BagSlots[i].item = itemToAdd;
                BagSlots[i].amount = 1;

                SyncQuestAndUI(itemToAdd.itemName);

                UpdateAllNPCOutlines();

                OnBagUpdated?.Invoke();
                return true;
            }
        }
        Debug.Log("가방이 가득 찼습니다.");
        return false;
    }

    private void UpdateAllNPCOutlines()
    {
        NPC[] allNPCs = FindObjectsOfType<NPC>();
        foreach (NPC npc in allNPCs)
        {
            if (npc.myQuest != null)
            {
                // 인벤토리 수량을 NPC의 퀘스트 데이터에 다시 맞추고 색상 변경
                int count = GetItemCount(npc.myQuest.targetID);
                npc.myQuest.ForceSyncProgress(count);
                npc.UpdateOutlineColor();
            }
        }
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach(var slot in BagSlots)
        {
            if(slot.item != null && slot.item.itemName == itemName)
            {
                count += slot.amount;
            }
        }
        //퀵슬롯용
        foreach(var slot in QuickSlots)
        {
            if(slot.item != null && slot.item.itemName == itemName)
            {
                count += slot.amount;
            }
        }
        return count;
    }

    public void UseItem(int index, bool isQuickSlot, PlayerStat player)
    {
        InventorySlot targetSlot = isQuickSlot ? QuickSlots[index] : BagSlots[index];

        if (targetSlot.item == null || targetSlot.IsEmpty) return;

        ItemData item = targetSlot.item;
        string itemName = item.itemName;

        if (item.type == ItemType.Eatable)
        {
            foreach (var effect in item.eatables)
            {
                ApplyEffect(player, effect);
            }

            targetSlot.item = null; 
            targetSlot.amount = 0;

            SyncQuestAndUI(itemName);

            if (isQuickSlot) OnQuickSlotUpdated?.Invoke();
            else OnBagUpdated?.Invoke();
        }
    }

    public void RemoveItem(string itemName, int amount)
    {
        int remainingToRemove = amount;

        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item != null && BagSlots[i].item.itemName == itemName)
            {
                if (BagSlots[i].amount > remainingToRemove)
                {
                    BagSlots[i].amount -= remainingToRemove;
                    remainingToRemove = 0;
                }
                else
                {
                    remainingToRemove -= BagSlots[i].amount;
                    BagSlots[i].item = null;
                    BagSlots[i].amount = 0;
                }
            }
            if (remainingToRemove <= 0) break;
        }

        SyncQuestAndUI(itemName);

        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke(); // 퀵슬롯 적용
    }

    private void ApplyEffect(PlayerStat player, ItemDataEatable effect)
    {
        StatCondition targetStat = null;

        switch (effect.type)
        {
            case EatableType.Hunger: 
                targetStat = player.hunger; 
                break;
            case EatableType.Thirst: 
                targetStat = player.thirst; 
                break;
            case EatableType.Health: 
                targetStat = player.hp; 
                break;
            case EatableType.Stamina: 
                targetStat = player.stamina; 
                break;
            case EatableType.Infection: 
                targetStat = player.infection; 
                break;
        }

    if (targetStat != null)
    {
        // 감염도(Infection)인 경우에만 수치를 뺌
        if (effect.type == EatableType.Infection)
        {
            targetStat.currentValue -= effect.value;
        }
        else // 나머지는 수치를 더합니다 (회복 효과)
        {
            targetStat.currentValue += effect.value;
        }

        // 스탯이 0 ~ 최대값 범위를 벗어나지 않게 고정
        targetStat.currentValue = Mathf.Clamp(targetStat.currentValue, 0, targetStat.maxValue);
    }
}

    public void SwapItemBetweenBagAndQuickSlot(int bagIndex, int quickIndex)
    {

        // if (bagIndex < 0 || bagIndex >= bagSize) return;
        // if (quickIndex < 0 || quickIndex >= quickSlotSize) return;


        // InventorySlot bSlot = BagSlots[bagIndex];


        // if (!bSlot.IsEmpty && bSlot.item.itemType == ItemData.ItemType.Resource)
        // {
        //     Debug.Log("이 아이템은 퀵슬롯에 장착할 수 없습니다.");
        //     return;
        // }
        // SwapSlots(BagSlots[bagIndex], QuickSlots[quickIndex]);
        // OnBagUpdated?.Invoke();
        // OnQuickSlotUpdated?.Invoke();
    }

    // 가방 내부 스왑 기능 추가
    public void SwapItemWithinBag(int index1, int index2)
    {
        if (index1 < 0 || index1 >= bagSize || index2 < 0 || index2 >= bagSize) return;
        SwapSlots(BagSlots[index1], BagSlots[index2]);
        OnBagUpdated?.Invoke();
    }

    // 퀵슬롯 내부 스왑 기능 추가
    public void SwapItemWithinQuickSlot(int index1, int index2)
    {
        if (index1 < 0 || index1 >= quickSlotSize || index2 < 0 || index2 >= quickSlotSize) return;
        SwapSlots(QuickSlots[index1], QuickSlots[index2]);
        OnQuickSlotUpdated?.Invoke();
    }

    // 중복되는 스왑 처리 로직을 내부 함수로 분리하여 최적화
    // private void SwapSlots(InventorySlot slot1, InventorySlot slot2)
    // {
    //      ItemData tempItem = slot1.item;
    //      int tempAmount = slot1.amount;

    //      slot1.item = slot2.item;
    //      slot1.amount = slot2.amount;

    //      slot2.item = tempItem;
    //      slot2.amount = tempAmount;
    // }

    private void SwapSlots(InventorySlot slot1, InventorySlot slot2)
    {
        ItemData tempItem = slot1.item;
        slot1.item = slot2.item;
        slot2.item = tempItem;
    }

    private void SyncQuestAndUI(string itemName)
    {
        if (QuestManager.Instance != null)
    {
        //현재 남은 개수 확인
        int currentCount = GetItemCount(itemName);
        
        //당 아이템 관련 퀘스트 찾아서 데이터 동기화
        var targetQuest = QuestManager.Instance.activeQuests.Find(q => q.targetID == itemName);
        if (targetQuest != null)
        {
            targetQuest.ForceSyncProgress(currentCount);
        }

        //퀘스트 UI 새로고침
        QuestUI ui = FindObjectOfType<QuestUI>(true);
        if (ui != null) ui.RefreshQuestList();
    }
    }
}