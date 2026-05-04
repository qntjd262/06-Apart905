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
                SyncQuestAndUI(itemToAdd.itemName);


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
        string itemName = item.itemName;


        if (item is EatableItemData eatItem)
        {
            if (eatItem.eatableType_1 != EatableType.None)
                player.ApplyEatableEffect(eatItem.eatableType_1, eatItem.value_1);
            if (eatItem.eatableType_2 != EatableType.None)
                player.ApplyEatableEffect(eatItem.eatableType_2, eatItem.value_2);




            targetSlot.item = null;
            //targetSlot.amount = 0;


            SyncQuestAndUI(itemName);


            if (isQuickSlot) OnQuickSlotUpdated?.Invoke();
            else OnBagUpdated?.Invoke();
        }
 else if (item.itemType == ItemType.Equipable)
        {
            // 이미 퀵슬롯에 있는 아이템을 다시 '장착'할 필요는 없으므로 가방에 있을 때만 실행
            if (!isQuickSlot)
            {
                EquipToQuickSlot(index);
            }
            else
            {
                Debug.Log("이미 퀵슬롯에 장착된 아이템입니다.");
            }
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


        SyncQuestAndUI(itemName);


        OnBagUpdated?.Invoke();
        OnQuickSlotUpdated?.Invoke();
    }

    private void EquipToQuickSlot(int bagIndex)
    {
        // 1. 퀵슬롯에서 빈 공간(item이 null인 곳) 찾기
        int emptyIndex = -1;
        for (int i = 0; i < QuickSlots.Length; i++)
        {
            if (QuickSlots[i].item == null)
            {
                emptyIndex = i;
                break;
            }
        }


        // 2. 빈 공간을 찾았다면 아이템 이동
        if (emptyIndex != -1)
        {
            // 가방에 있는 아이템을 퀵슬롯으로 복사
            QuickSlots[emptyIndex].item = BagSlots[bagIndex].item;
            // 원래 가방에 있던 자리는 비우기
            BagSlots[bagIndex].item = null;


            // 3. UI 갱신을 위해 액션 호출
            OnBagUpdated?.Invoke();
            OnQuickSlotUpdated?.Invoke();


            Debug.Log($"{QuickSlots[emptyIndex].item.Name}이(가) 퀵슬롯 {emptyIndex}번에 장착되었습니다.");
        }
        else
        {
            // 빈 공간이 없는 경우
            Debug.Log("퀵슬롯이 가득 차서 장착할 수 없습니다.");
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


    public void DiscardItem(int index, bool isQuickSlot)
    {
        InventorySlot targetSlot = isQuickSlot ? QuickSlots[index] : BagSlots[index];


        if (targetSlot.item != null)
        {
            Debug.Log($"{targetSlot.item.Name}을(를) 버렸습니다.");
            targetSlot.item = null; // 아이템 삭제


            // UI 갱신 알림
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


