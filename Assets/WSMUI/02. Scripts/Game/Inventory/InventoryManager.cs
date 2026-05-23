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
                    QuestManager.Instance.NotifyEvent(QuestType.ItemCollection, itemToAdd.Name, 1);

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
        foreach (var slot in BagSlots)
        {
            if (slot.item != null && slot.item.Name == itemName) count++;
        }
        // 퀵슬롯은 이제 가방의 참조일 뿐이므로 중복 카운트 방지를 위해 가방만 체크한다.
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

            SyncQuestAndUI(item.Name);

            if (isQuickSlot) OnQuickSlotUpdated?.Invoke();
            else OnBagUpdated?.Invoke();
        }
        else if (item.itemType == ItemType.Equipable)
        {
            if (!isQuickSlot) EquipToQuickSlot(index);
            else Debug.Log("이미 퀵슬롯에 장착된 아이템입니다.");
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

        // 가방에서 삭제되었으니 퀵슬롯에 걸려있던 링크도 끊어준다.
        for (int i = 0; i < quickSlotSize; i++)
        {
            if (QuickSlots[i].item != null && QuickSlots[i].item.Name == itemName)
            {
                QuickSlots[i].item = null;
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
            // [수정됨] 퀵슬롯으로 복사만 하고 원본 가방 아이템은 지우지 않음 (참조 링크)
            QuickSlots[emptyIndex].item = BagSlots[bagIndex].item;
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

        if (!BagSlots[bagIndex].IsEmpty && BagSlots[bagIndex].item.itemType != ItemType.Equipable)
        {
            Debug.Log("이 아이템은 퀵슬롯에 장착할 수 없습니다.");
            return;
        }

        QuickSlots[quickIndex].item = BagSlots[bagIndex].item; // 가방에 놔두고 링크만
        OnQuickSlotUpdated?.Invoke();
    }

    // 퀵슬롯에서 바깥으로 빼버릴 때 장착 해제
    public void ClearQuickSlot(int quickIndex)
    {
        QuickSlots[quickIndex].item = null;
        OnQuickSlotUpdated?.Invoke();
    }

    // 기존 스왑은 퀵슬롯끼리 위치 바꿀 때만 사용
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
            string itemName = targetSlot.item.Name;

            if (isQuickSlot)
            {
                // 퀵슬롯에서 버리면 링크만 해제 (가방 아이템은 유지)
                targetSlot.item = null;
            }
            else
            {
                // 가방에서 버리면 완전 삭제
                RemoveItem(itemName, 1);
            }

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
                SwapSlots(CurrentStorageSlots[storageIndex], BagSlots[i]);
                OnBagUpdated?.Invoke();
                OnStorageUpdated?.Invoke();
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
                SwapSlots(BagSlots[bagIndex], CurrentStorageSlots[i]);
                OnBagUpdated?.Invoke();
                OnStorageUpdated?.Invoke();
                return;
            }
        }
        Debug.Log("창고에 빈 공간이 없습니다.");
    }

    public void MoveAllFromStorageToBag()
    {
        if (CurrentStorageSlots == null) return;

        // 보관함의 모든 슬롯을 검사
        for (int i = 0; i < CurrentStorageSlots.Length; i++)
        {
            // 슬롯이 비어있지 않다면 이동 함수 호출
            if (!CurrentStorageSlots[i].IsEmpty)
            {
                // MoveItemStorageToBag 내부에서 가방 공간 체크 및 UI 업데이트가 이뤄짐
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