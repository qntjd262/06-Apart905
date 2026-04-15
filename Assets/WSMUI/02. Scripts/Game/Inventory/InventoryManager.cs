using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private int bagSize = 20;
    [SerializeField] private int quickSlotSize = 5;

    public InventorySlot[] BagSlots { get; private set; }
    public InventorySlot[] QuickSlots { get; private set; }
    public InventorySlot[] CurrentStorageSlots { get; private set; }

    public Action OnBagUpdated;
    public Action OnQuickSlotUpdated;
    public Action OnStorageUpdated;

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
            BagSlots[i] = new InventorySlot();

        QuickSlots = new InventorySlot[quickSlotSize];
        for (int i = 0; i < quickSlotSize; i++)
            QuickSlots[i] = new InventorySlot();
    }

    public bool AddItem(ItemData itemToAdd)
    {
        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item == null)
            {
                BagSlots[i].item = itemToAdd;

                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.NotifyEvent(QuestType.ItemCollection, itemToAdd.Name, 1);
                }

                OnBagUpdated?.Invoke();
                return true;
            }
        }
        Debug.Log("가방이 가득 찼습니다.");
        return false;
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;
        // 가방 확인
        foreach (var slot in BagSlots)
        {
            if (slot.item != null && slot.item.Name == itemName)
            {
                count++; // amount 참조 대신 카운트만 증가
            }
        }
        // 퀵슬롯 확인
        foreach (var slot in QuickSlots)
        {
            if (slot.item != null && slot.item.Name == itemName)
            {
                count++;
            }
        }
        return count;
    }

    public void UseItem(int index, bool isQuickSlot, PlayerStat player)
    {
        InventorySlot targetSlot = isQuickSlot ? QuickSlots[index] : BagSlots[index];

        if (targetSlot.item == null || targetSlot.IsEmpty) return;

        ItemData item = targetSlot.item;

        if (item is EatableItemData eatItem)
        {
            if (eatItem.eatableType_1 != EatableType.None)
                ApplyEffect(player, eatItem.eatableType_1, eatItem.value_1);
            if (eatItem.eatableType_2 != EatableType.None)
                ApplyEffect(player, eatItem.eatableType_2, eatItem.value_2);

            targetSlot.item = null;

            if (isQuickSlot) OnQuickSlotUpdated?.Invoke();
            else OnBagUpdated?.Invoke();
        }
    }

    public void RemoveItem(string itemName, int countToRemove)
    {
        int removedCount = 0;

        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item != null && BagSlots[i].item.Name == itemName)
            {
                BagSlots[i].item = null;
                removedCount++;

                if (removedCount >= countToRemove) break;
            }
        }

        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
    }

    private void ApplyEffect(PlayerStat player, EatableType type, float value)
    {
        StatCondition targetStat = null;

        switch (type)
        {
            case EatableType.Hunger: targetStat = player.hunger; break;
            case EatableType.Thirst: targetStat = player.thirst; break;
            case EatableType.Health: targetStat = player.hp; break;
            case EatableType.Stamina: targetStat = player.stamina; break;
            case EatableType.Infection: targetStat = player.infection; break;
        }

        if (targetStat != null)
        {
            // 감염도(Infection)인 경우에만 수치를 뺌
            if (type == EatableType.Infection)
            {
                targetStat.currentValue -= value;
            }
            else // 나머지는 수치를 더합니다 (회복 효과)
            {
                targetStat.currentValue += value;
            }

            // 스탯이 0 ~ 최대값 범위를 벗어나지 않게 고정
            targetStat.currentValue = Mathf.Clamp(targetStat.currentValue, 0, targetStat.maxValue);
        }
    }

    public void SwapItemBetweenBagAndQuickSlot(int bagIndex, int quickIndex)
    {
        if (bagIndex < 0 || bagIndex >= bagSize) return;
        if (quickIndex < 0 || quickIndex >= quickSlotSize) return;

        InventorySlot bSlot = BagSlots[bagIndex];

        if (!bSlot.IsEmpty && bSlot.item.itemType != ItemType.Equipable)
        {
            Debug.Log("이 아이템은 퀵슬롯에 장착할 수 없습니다.");
            return;
        }

        SwapSlots(BagSlots[bagIndex], QuickSlots[quickIndex]);
        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
    }

    public void SwapItemWithinBag(int index1, int index2)
    {
        if (index1 < 0 || index1 >= bagSize || index2 < 0 || index2 >= bagSize) return;
        SwapSlots(BagSlots[index1], BagSlots[index2]);
        OnBagUpdated?.Invoke();
    }

    public void SwapItemWithinQuickSlot(int index1, int index2)
    {
        if (index1 < 0 || index1 >= quickSlotSize || index2 < 0 || index2 >= quickSlotSize) return;
        SwapSlots(QuickSlots[index1], QuickSlots[index2]);
        OnQuickSlotUpdated?.Invoke();
    }

    public void SwapItemBetweenStorageAndBag(int storageIndex, int bagIndex)
    {
        if (CurrentStorageSlots == null) return;
        if (storageIndex < 0 || storageIndex >= CurrentStorageSlots.Length) return;
        if (bagIndex < 0 || bagIndex >= bagSize) return;

        SwapSlots(CurrentStorageSlots[storageIndex], BagSlots[bagIndex]);
        OnStorageUpdated?.Invoke();
        OnBagUpdated?.Invoke();
    }

    public void OpenStorage(InventorySlot[] storageSlots)
    {
        CurrentStorageSlots = storageSlots;
        OnStorageUpdated?.Invoke();
    }

    public void CloseStorage()
    {
        CurrentStorageSlots = null;
        OnStorageUpdated?.Invoke();
    }

    private void SwapSlots(InventorySlot slot1, InventorySlot slot2)
    {
        ItemData tempItem = slot1.item;

        slot1.item = slot2.item;

        slot2.item = tempItem;
    }
}