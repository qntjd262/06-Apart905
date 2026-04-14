using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    private SlotUI[] uiSlots;

    void Start()
    {
        uiSlots = slotsParent.GetComponentsInChildren<SlotUI>();

        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = false;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnBagUpdated += RefreshUI;
            RefreshUI();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.inventoryPanel = this.gameObject;
        }

        gameObject.SetActive(true);  //false로 바꿔야됨
    }

    private void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < InventoryManager.Instance.BagSlots.Length)
            {
                uiSlots[i].UpdateSlot(InventoryManager.Instance.BagSlots[i]);
            }
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnBagUpdated -= RefreshUI;
        }
    }


}
