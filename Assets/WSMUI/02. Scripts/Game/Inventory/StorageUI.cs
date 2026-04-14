using UnityEngine;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    private SlotUI[] uiSlots;

    private void Start()
    {
        uiSlots = slotsParent.GetComponentsInChildren<SlotUI>();

        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = false;
            uiSlots[i].IsStorageSlot = true; // SlotUI에 추가 필요
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnStorageUpdated += RefreshUI;
        }
    }

    private void RefreshUI()
    {
        if (InventoryManager.Instance.CurrentStorageSlots == null) return;

        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < InventoryManager.Instance.CurrentStorageSlots.Length)
                uiSlots[i].UpdateSlot(InventoryManager.Instance.CurrentStorageSlots[i]);
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnStorageUpdated -= RefreshUI;
    }
}