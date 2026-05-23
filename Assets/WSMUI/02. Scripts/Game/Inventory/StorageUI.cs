using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    private SlotUI[] uiSlots;

    [Header("Item Info UI (Optional)")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private Image itemIconImage;

    private void Start()
    {
        uiSlots = slotsParent.GetComponentsInChildren<SlotUI>();

        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = false;
            uiSlots[i].IsStorageSlot = true;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnStorageUpdated += RefreshUI;
            RefreshUI();
        }

        // 처음에 정보창 꺼두기
        if (infoPanel != null) infoPanel.SetActive(false);
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

    // 슬롯에서 클릭했을 때 호출할 함수
    public void ShowItemInfo(InventorySlot slotData)
    {
        if (slotData == null || slotData.IsEmpty)
        {
            if (infoPanel != null) infoPanel.SetActive(false);
            return;
        }

        if (infoPanel != null) infoPanel.SetActive(true);
        if (itemNameText != null) itemNameText.text = slotData.item.Name;
        if (itemDescText != null) itemDescText.text = slotData.item.description;
        if (itemIconImage != null) itemIconImage.sprite = slotData.item.icon;
    }
    private void OnDisable()
    {
        // 보관함 창이 꺼질 때 정보창도 함께 꺼서 잔여 데이터 노출 방지
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnStorageUpdated -= RefreshUI;
    }
}