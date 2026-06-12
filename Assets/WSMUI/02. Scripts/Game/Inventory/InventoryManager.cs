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

    public static System.Action OnInventoryChanged;

    public PlayerStat Player { get; set; }

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
        for (int i = 0; i < bagSize; i++) BagSlots[i] = new InventorySlot();

        QuickSlots = new InventorySlot[quickSlotSize];
        for (int i = 0; i < quickSlotSize; i++) QuickSlots[i] = new InventorySlot();
    }

    public bool AddItem(ItemData itemToAdd)
    {
        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item == null)
            {
                BagSlots[i].item = itemToAdd;

                if (QuestManager.Instance != null)
                    QuestManager.Instance.NotifyEvent(QuestType.ItemCollection, itemToAdd.itemName, 1);

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
                int count = GetItemCount(npc.myQuest.targetID);
                npc.myQuest.ForceSyncProgress(count);
                npc.UpdateOutlineColor();
            }
        }
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;
        // [수정] 아이템이 퀵슬롯으로 완전히 넘어갔으므로, 퀵슬롯의 아이템 개수도 합산해야 합니다.
        foreach (var slot in BagSlots)
        {
            if (slot.item != null && slot.item.itemName == itemName) count++;
        }
        foreach (var slot in QuickSlots)
        {
            if (slot.item != null && slot.item.itemName == itemName) count++;
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
                player.ApplyEatableEffect(eatItem.eatableType_1, eatItem.value_1);
            if (eatItem.eatableType_2 != EatableType.None)
                player.ApplyEatableEffect(eatItem.eatableType_2, eatItem.value_2);
            targetSlot.item = null;

            SyncQuestAndUI(item.itemName);

            if (isQuickSlot) OnQuickSlotUpdated?.Invoke();
            else OnBagUpdated?.Invoke();
        }
        else if (item.itemType == ItemType.Equipable)
        {
            // 우클릭으로 퀵슬롯에 장착할 때 처리
            if (!isQuickSlot) EquipToQuickSlot(index);
        }
    }

    public void RemoveItem(string itemName, int countToRemove)
    {
        int removedCount = 0;

        // 1. 가방에서 먼저 삭제
        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item != null && BagSlots[i].item.itemName == itemName)
            {
                BagSlots[i].item = null;
                removedCount++;
                if (removedCount >= countToRemove) break;
            }
        }

        // 2. 가방에서 다 못 지웠다면 퀵슬롯도 검사하여 삭제
        if (removedCount < countToRemove)
        {
            for (int i = 0; i < quickSlotSize; i++)
            {
                if (QuickSlots[i].item != null && QuickSlots[i].item.itemName == itemName)
                {
                    QuickSlots[i].item = null;
                    removedCount++;
                    if (removedCount >= countToRemove) break;
                }
            }
        }

        SyncQuestAndUI(itemName);
        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
    }

    private void EquipToQuickSlot(int bagIndex)
    {
        int emptyIndex = -1;
        for (int i = 0; i < QuickSlots.Length; i++)
        {
            if (QuickSlots[i].item == null)
            {
                emptyIndex = i;
                break;
            }
        }

        if (emptyIndex != -1)
        {
            // [수정] 참조만 하는게 아니라 퀵슬롯으로 완전히 '이동'시키고 가방은 비웁니다.
            QuickSlots[emptyIndex].item = BagSlots[bagIndex].item;
            BagSlots[bagIndex].item = null; 
            
            OnBagUpdated?.Invoke();
            OnQuickSlotUpdated?.Invoke();
        }
        else
        {
            Debug.Log("퀵슬롯이 가득 차서 장착할 수 없습니다.");
        }
    }

    // 드래그 앤 드롭으로 장착할 때 호출
    public void AssignToQuickSlot(int bagIndex, int quickIndex)
    {
        if (bagIndex < 0 || bagIndex >= bagSize) return;
        if (quickIndex < 0 || quickIndex >= quickSlotSize) return;

        // 가방에 있는 아이템이 퀵슬롯에 들어갈 수 없는 타입인지 체크 (가방 슬롯이 비어있으면 스왑을 위해 통과)
        if (!BagSlots[bagIndex].IsEmpty && BagSlots[bagIndex].item.itemType != ItemType.Equipable)
        {
            Debug.Log("이 아이템은 퀵슬롯에 장착할 수 없습니다.");
            return;
        }

        // [수정] 퀵슬롯과 가방 슬롯의 아이템을 서로 '스왑'합니다. (기존 퀵슬롯 아이템은 가방으로 돌아감)
        SwapSlots(BagSlots[bagIndex], QuickSlots[quickIndex]);
        
        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
    }

    // 더블클릭 등으로 퀵슬롯에서 장착 해제할 때
    public void ClearQuickSlot(int quickIndex)
    {
        if (QuickSlots[quickIndex].IsEmpty) return;

        // [수정] 아이템을 삭제(null)하는게 아니라 가방의 빈 슬롯을 찾아 다시 넣어줍니다.
        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].IsEmpty)
            {
                BagSlots[i].item = QuickSlots[quickIndex].item;
                QuickSlots[quickIndex].item = null;
                
                OnBagUpdated?.Invoke();
                OnQuickSlotUpdated?.Invoke();
                return;
            }
        }
        Debug.Log("가방이 가득 차서 퀵슬롯에서 해제할 수 없습니다.");
    }

    public void SwapItemWithinQuickSlot(int index1, int index2)
    {
        if (index1 < 0 || index1 >= quickSlotSize || index2 < 0 || index2 >= quickSlotSize) return;
        SwapSlots(QuickSlots[index1], QuickSlots[index2]);
        OnQuickSlotUpdated?.Invoke();
    }

    public void SwapItemWithinBag(int index1, int index2)
    {
        if (index1 < 0 || index1 >= bagSize || index2 < 0 || index2 >= bagSize) return;
        SwapSlots(BagSlots[index1], BagSlots[index2]);
        OnBagUpdated?.Invoke();
    }

    public void SwapItemBetweenStorageAndBag(int storageIndex, int bagIndex)
    {
        if (CurrentStorageSlots == null) return;
        if (storageIndex < 0 || storageIndex >= CurrentStorageSlots.Length) return;
        if (bagIndex < 0 || bagIndex >= bagSize) return;

        SwapSlots(CurrentStorageSlots[storageIndex], BagSlots[bagIndex]);
        OnStorageUpdated?.Invoke();
        OnBagUpdated?.Invoke();

        if(BagSlots[bagIndex].item != null)
        {
            SyncQuestAndUI(BagSlots[bagIndex].item.itemName);
        }
        UpdateAllNPCOutlines();
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

   public void DiscardItem(int index, bool isQuickSlot)
    {
        InventorySlot targetSlot = isQuickSlot ? QuickSlots[index] : BagSlots[index];

        if (targetSlot.item != null)
        {
            string itemName = targetSlot.item.itemName;

            targetSlot.item = null;

            SyncQuestAndUI(itemName);

            if (isQuickSlot) OnQuickSlotUpdated?.Invoke();
            else OnBagUpdated?.Invoke();
        }
    }

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
            int currentCount = GetItemCount(itemName);
            var targetQuest = QuestManager.Instance.activeQuests.Find(q => q.targetID == itemName);
            if (targetQuest != null) targetQuest.ForceSyncProgress(currentCount);

            QuestUI ui = FindObjectOfType<QuestUI>(true);
            if (ui != null) ui.RefreshQuestList();
        }
    }

    public void MoveItemStorageToBag(int storageIndex)
    {
        if (CurrentStorageSlots == null || CurrentStorageSlots[storageIndex].IsEmpty) return;

        for (int i = 0; i < BagSlots.Length; i++)
        {
            if (BagSlots[i].IsEmpty)
            {
                string movedItemName = CurrentStorageSlots[storageIndex].item.itemName;

                SwapSlots(CurrentStorageSlots[storageIndex], BagSlots[i]);
                OnBagUpdated?.Invoke();
                OnStorageUpdated?.Invoke();

                SyncQuestAndUI(movedItemName);
                UpdateAllNPCOutlines();
                return;
            }
        }
        Debug.Log("가방에 빈 공간이 없습니다.");
    }

    public void MoveItemBagToStorage(int bagIndex)
    {
        if (CurrentStorageSlots == null || BagSlots[bagIndex].IsEmpty) return;

        for (int i = 0; i < CurrentStorageSlots.Length; i++)
        {
            if (CurrentStorageSlots[i].IsEmpty)
            {
                string removedItemName = BagSlots[bagIndex].item.itemName;

                SwapSlots(BagSlots[bagIndex], CurrentStorageSlots[i]);
                OnBagUpdated?.Invoke();
                OnStorageUpdated?.Invoke();

                SyncQuestAndUI(removedItemName);
                UpdateAllNPCOutlines();
                return;
            }
        }
        Debug.Log("창고에 빈 공간이 없습니다.");
    }

    public void MoveAllFromStorageToBag()
    {
        if (CurrentStorageSlots == null) return;
        for (int i = 0; i < CurrentStorageSlots.Length; i++)
        {
            if (!CurrentStorageSlots[i].IsEmpty)
            {
                MoveItemStorageToBag(i);
            }
        }
    }

    public void ResetInventory()
    {
        InitializeInventory();
        CurrentStorageSlots = null;

        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
        OnStorageUpdated?.Invoke();
    }
}