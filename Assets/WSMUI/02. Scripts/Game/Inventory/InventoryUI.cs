using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Transform slotsParent;
    private SlotUI[] uiSlots;

    [Header("Position Settings")]
    [SerializeField] private Vector2 centerPosition = Vector2.zero; // 중앙 위치
    [SerializeField] private Vector2 rightPosition = new Vector2(670f, 0f);

    void Awake()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }
    public void SetPanelPosition(bool isStorageOpen)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        Vector2 targetPos = isStorageOpen ? rightPosition : centerPosition;

        // 즉각 반영 (테스트용)
        rectTransform.anchoredPosition = targetPos;
    }
    void Start()
    {
        uiSlots = slotsParent.GetComponentsInChildren<SlotUI>();

        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = false;

            uiSlots[i].IsStorageSlot = false;
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