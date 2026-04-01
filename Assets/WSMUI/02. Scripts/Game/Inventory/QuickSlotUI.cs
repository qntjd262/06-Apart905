using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private Transform quickSlotParent;
    private SlotUI[] uiSlots;

    void Start()
    {
        uiSlots = quickSlotParent.GetComponentsInChildren<SlotUI>();
        
        // 추가: 슬롯 번호 자동 할당 (퀵슬롯)
        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = true;
        }
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated += RefreshQuickSlot;
            RefreshQuickSlot();
        }
    }

    private void RefreshQuickSlot()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < InventoryManager.Instance.QuickSlots.Length)
            {
                uiSlots[i].UpdateSlot(InventoryManager.Instance.QuickSlots[i]);
            }
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated -= RefreshQuickSlot;
        }
    }
}