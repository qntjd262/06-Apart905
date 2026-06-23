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

    // [핵심 추가] 현재 정보창에 띄워둔 슬롯 데이터를 추적하기 위한 캐싱 변수
    private InventorySlot currentDisplayedSlot;

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

        // [핵심 추가] 인벤토리가 갱신될 때, 현재 띄워둔 슬롯이 방금 비워졌는지 검사한다.
        if (currentDisplayedSlot != null && currentDisplayedSlot.IsEmpty)
        {
            if (infoPanel != null) infoPanel.SetActive(false);
            currentDisplayedSlot = null; // 초기화
        }
    }

    // 슬롯에서 클릭했을 때 호출할 함수
    public void ShowItemInfo(InventorySlot slotData)
    {
        if (slotData == null || slotData.IsEmpty || slotData.item == null)
        {
            if (infoPanel != null) infoPanel.SetActive(false);
            return;
        }

        if (infoPanel != null) infoPanel.SetActive(true);
        
        string displayName = !string.IsNullOrEmpty(slotData.item.Name) ? slotData.item.Name : 
                            (!string.IsNullOrEmpty(slotData.item.itemName) ? slotData.item.itemName : slotData.item.name);

        string displayDesc = !string.IsNullOrEmpty(slotData.item.description) ? slotData.item.description : "";

        // UI에 안전하게 대입
        if (itemNameText != null) itemNameText.text = displayName;
        if (itemDescText != null) itemDescText.text = displayDesc;
        if (itemIconImage != null) itemIconImage.sprite = slotData.item.icon;
        
        Debug.Log($"선택한 아이템: {displayName}, 설명 텍스트: {displayDesc}", slotData.item);
    }

    private void OnDisable()
    {
        // 보관함 창이 꺼질 때 정보창도 함께 꺼서 잔여 데이터 노출 방지
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
        currentDisplayedSlot = null; // 꺼질 때 캐싱 데이터도 같이 비워준다.
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnStorageUpdated -= RefreshUI;
    }
}