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

    // [루팅 및 맵에서 획득] 필드 아이템 추가 함수
    public bool AddItem(ItemData itemToAdd)
    {
        if (itemToAdd == null) return false;

        if (itemToAdd.itemName == "Headlight" || itemToAdd.itemName == "헤드라이트")
        {
            PlayerEquip playerEquip = FindObjectOfType<PlayerEquip>();
            if (playerEquip != null)
            {
                playerEquip.TryAcquireHeadlight(itemToAdd);
            }

            if (NotificationManager.Instance != null)
                NotificationManager.Instance.ShowNotification(itemToAdd.itemName, 1);

            return true; 
        }

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

                if (NotificationManager.Instance != null)
                    NotificationManager.Instance.ShowNotification(itemToAdd.itemName, 1);

                return true;
            }
        }
        Debug.Log("가방이 가득 찼습니다.");
        return false;
    }

    private void UpdateAllNPCOutlines()
        {
            NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            for (int i = 0; i < allNPCs.Length; i++)
            {
                NPC npc = allNPCs[i];
                
                if (npc != null && npc.myQuest != null)
                {
                    if (npc.myQuest.type == QuestType.ItemCollection)
                    {
                        for (int j = 0; j < npc.myQuest.objectives.Count; j++)
                        {
                            var obj = npc.myQuest.objectives[j];
                            int count = GetItemCount(obj.targetID);   
                            npc.myQuest.ForceSyncProgress(obj.targetID, count);
                        }
                    }
                    npc.UpdateOutlineColor();
                }
            }
        }

    public int GetItemCount(string itemName)
    {
        int count = 0;
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
            if (!isQuickSlot) EquipToQuickSlot(index);
        }
    }

    public void RemoveItem(string itemName, int countToRemove)
    {
        int removedCount = 0;

        for (int i = 0; i < bagSize; i++)
        {
            if (BagSlots[i].item != null && BagSlots[i].item.itemName == itemName)
            {
                BagSlots[i].item = null;
                removedCount++;
                if (removedCount >= countToRemove) break;
            }
        }

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

    public void AssignToQuickSlot(int bagIndex, int quickIndex)
    {
        if (bagIndex < 0 || bagIndex >= bagSize) return;
        if (quickIndex < 0 || quickIndex >= quickSlotSize) return;

        if (!BagSlots[bagIndex].IsEmpty && BagSlots[bagIndex].item.itemType != ItemType.Equipable)
        {
            Debug.Log("이 아이템은 퀵슬롯에 장착할 수 없습니다.");
            return;
        }

        SwapSlots(BagSlots[bagIndex], QuickSlots[quickIndex]);
        
        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
    }

    public void ClearQuickSlot(int quickIndex)
    {
        if (QuickSlots[quickIndex].IsEmpty) return;

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

        ItemData movingItem = CurrentStorageSlots[storageIndex].item;
        if (movingItem != null && (movingItem.itemName == "Headlight" || movingItem.itemName == "헤드라이트"))
        {
            PlayerEquip playerEquip = FindObjectOfType<PlayerEquip>();
            if (playerEquip != null)
            {
                playerEquip.TryAcquireHeadlight(movingItem);
            }
            
            if (NotificationManager.Instance != null)
                NotificationManager.Instance.ShowNotification(movingItem.itemName, 1);

            CurrentStorageSlots[storageIndex].item = null; 
            OnStorageUpdated?.Invoke();
            return;
        }

        SwapSlots(CurrentStorageSlots[storageIndex], BagSlots[bagIndex]);
        
        OnStorageUpdated?.Invoke();
        OnBagUpdated?.Invoke();

        if (BagSlots[bagIndex].item != null)
        {
            ItemData finalItem = BagSlots[bagIndex].item;

            string finalItemName = !string.IsNullOrEmpty(finalItem.itemName) ? finalItem.itemName : 
                              (!string.IsNullOrEmpty(finalItem.Name) ? finalItem.Name : finalItem.name);

            SyncQuestAndUI(finalItemName);

            if (NotificationManager.Instance != null && !string.IsNullOrEmpty(finalItemName))
            {
                NotificationManager.Instance.ShowNotification(finalItemName, 1);
            }
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
                
                var activeQuests = QuestManager.Instance.activeQuests;
                for (int i = activeQuests.Count - 1; i >= 0; i--)
                {
                    if (i >= activeQuests.Count) continue; 
                    
                    Quest quest = activeQuests[i];
                    if (quest != null && quest.type == QuestType.ItemCollection)
                    {
                        quest.ForceSyncProgress(itemName, currentCount);
                    }
                }

                QuestUI ui = FindFirstObjectByType<QuestUI>(FindObjectsInactive.Include);
                if (ui != null) ui.RefreshQuestList();
            }
        }

    public void MoveItemStorageToBag(int storageIndex)
    {
        if (CurrentStorageSlots == null || CurrentStorageSlots[storageIndex].IsEmpty) return;

        ItemData targetItem = CurrentStorageSlots[storageIndex].item;

        if (targetItem.itemName == "Headlight" || targetItem.itemName == "헤드라이트")
        {
            PlayerEquip playerEquip = FindObjectOfType<PlayerEquip>();
            if (playerEquip != null)
            {
                playerEquip.TryAcquireHeadlight(targetItem);
            }

            if (NotificationManager.Instance != null)
                NotificationManager.Instance.ShowNotification(targetItem.itemName, 1);

            CurrentStorageSlots[storageIndex].item = null;
            OnStorageUpdated?.Invoke();
            return;
        }

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

                if (NotificationManager.Instance != null)
                    NotificationManager.Instance.ShowNotification(movedItemName, 1);

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
