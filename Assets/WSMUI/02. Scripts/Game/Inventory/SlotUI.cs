using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image icon;

    [Header("Highlight UI")]
    [SerializeField] private GameObject highlightImageObject;
    private static SlotUI currentlySelectedSlot;

    public int SlotIndex { get; set; }
    public bool IsQuickSlot { get; set; }
    public bool IsStorageSlot { get; set; }
    public bool IsInGameQuickSlot { get; set; }

    private static SlotUI pickedSlot;
    public static SlotUI PickedSlot => pickedSlot;
    private static Image cursorIcon;

    public void UpdateSlot(InventorySlot slotData)
    {
        if (slotData == null || slotData.IsEmpty)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }
        else
        {
            bool isPicked = (pickedSlot == this);
            icon.sprite = slotData.item.icon;
            icon.color = new Color(1, 1, 1, isPicked ? 0.3f : 1f);
        }
    }

    public void SelectSlot()
    {
        if (currentlySelectedSlot != null && currentlySelectedSlot != this)
        {
            if (currentlySelectedSlot.highlightImageObject != null)
                currentlySelectedSlot.highlightImageObject.SetActive(false);
        }

        currentlySelectedSlot = this;
        if (highlightImageObject != null) highlightImageObject.SetActive(true);

        // 1. 보관함 슬롯인 경우 -> StorageUI로 정보 전달
        if (IsStorageSlot)
        {
            StorageUI storageUI = GetComponentInParent<StorageUI>();
            if (storageUI != null && InventoryManager.Instance.CurrentStorageSlots != null)
            {
                InventorySlot slotData = InventoryManager.Instance.CurrentStorageSlots[SlotIndex];
                storageUI.ShowItemInfo(slotData);
            }
        }
        // 2. 인벤토리/퀵슬롯인 경우 (인게임 HUD용 퀵슬롯 제외) -> InventoryUI로 정보 전달
        else if (!IsInGameQuickSlot)
        {
            InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
            if (inventoryUI != null)
            {
                // 퀵슬롯인지 가방인지에 따라 참조할 배열이 다름
                InventorySlot slotData = IsQuickSlot
                    ? InventoryManager.Instance.QuickSlots[SlotIndex]
                    : InventoryManager.Instance.BagSlots[SlotIndex];

                inventoryUI.ShowItemInfo(slotData);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInGameQuickSlot) return;

        // 우클릭 (메뉴 호출)
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SelectSlot(); // [수정 1] 우클릭할 때도 외곽선 표시 적용
            HandleRightClick();
            return;
        }

        // 좌클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (IsStorageSlot) InventoryManager.Instance.MoveAllFromStorageToBag();
                return;
            }

            // 단일 클릭
            if (eventData.clickCount == 1)
            {
                SelectSlot(); // [수정 1] 공용 함수 호출로 깔끔하게 정리
            }
            // 더블 클릭
            else if (eventData.clickCount == 2)
            {
                if (icon.sprite == null) return;

                if (IsStorageSlot) InventoryManager.Instance.MoveItemStorageToBag(SlotIndex);
                else if (!IsQuickSlot && InventoryManager.Instance.CurrentStorageSlots != null) InventoryManager.Instance.MoveItemBagToStorage(SlotIndex);
                else if (IsQuickSlot) InventoryManager.Instance.ClearQuickSlot(SlotIndex);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (icon.sprite == null || IsInGameQuickSlot) return;

        pickedSlot = this;

        if (cursorIcon == null)
        {
            GameObject iconObj = new GameObject("CursorIcon");
            iconObj.transform.SetParent(GetComponentInParent<Canvas>().transform);
            cursorIcon = iconObj.AddComponent<Image>();
            cursorIcon.raycastTarget = false;
            cursorIcon.rectTransform.sizeDelta = new Vector2(50, 50);
        }

        cursorIcon.sprite = icon.sprite;
        cursorIcon.gameObject.SetActive(true);
        icon.color = new Color(1, 1, 1, 0.3f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pickedSlot == this && cursorIcon != null) cursorIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (pickedSlot == this) CancelPick();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (pickedSlot != null && pickedSlot != this)
        {
            ExecuteSwap(pickedSlot, this);
            pickedSlot.CancelPick();
        }
    }

    public void CancelPick()
    {
        if (pickedSlot == null) return;
        pickedSlot = null;
        if (cursorIcon != null) cursorIcon.gameObject.SetActive(false);

        InventoryManager.Instance.OnBagUpdated?.Invoke();
        InventoryManager.Instance.OnStorageUpdated?.Invoke();
        InventoryManager.Instance.OnQuickSlotUpdated?.Invoke();
    }

    private void OnDisable()
    {
        if (pickedSlot == this) CancelPick();

        // [수정 2] 창이 꺼질 때 외곽선 선택 상태를 완전히 리셋한다.
        if (currentlySelectedSlot == this)
        {
            if (highlightImageObject != null) highlightImageObject.SetActive(false);
            currentlySelectedSlot = null;
        }
    }

    private void ExecuteSwap(SlotUI from, SlotUI to)
    {
        if (from == to) return;

        if (from.IsQuickSlot || to.IsQuickSlot)
        {
            if (from.IsStorageSlot || to.IsStorageSlot) return;

            if (!from.IsQuickSlot && to.IsQuickSlot) InventoryManager.Instance.AssignToQuickSlot(from.SlotIndex, to.SlotIndex);
            else if (from.IsQuickSlot && !to.IsQuickSlot) InventoryManager.Instance.ClearQuickSlot(from.SlotIndex);
            else InventoryManager.Instance.SwapItemWithinQuickSlot(from.SlotIndex, to.SlotIndex);
        }
        else if (from.IsStorageSlot || to.IsStorageSlot)
        {
            if (from.IsStorageSlot && !to.IsStorageSlot) InventoryManager.Instance.SwapItemBetweenStorageAndBag(from.SlotIndex, to.SlotIndex);
            else if (!from.IsStorageSlot && to.IsStorageSlot) InventoryManager.Instance.SwapItemBetweenStorageAndBag(to.SlotIndex, from.SlotIndex);
        }
        else
        {
            InventoryManager.Instance.SwapItemWithinBag(from.SlotIndex, to.SlotIndex);
        }
    }

    private void HandleRightClick()
    {
        if (IsStorageSlot || IsQuickSlot) return;
        if (SlotIndex < 0 || SlotIndex >= InventoryManager.Instance.BagSlots.Length) return;

        InventorySlot slotData = InventoryManager.Instance.BagSlots[SlotIndex];
        if (slotData != null && slotData.item != null)
        {
            if (slotData.item.itemType != ItemType.Eatable && slotData.item.itemType != ItemType.Equipable)
            {
                Debug.Log("이 아이템은 메뉴에서 조작할 수 없습니다.");
                return;
            }

            ItemMenuUI itemMenuUI = ItemMenuUI.Instance;
            if (itemMenuUI == null)
            {
                itemMenuUI = FindFirstObjectByType<ItemMenuUI>(FindObjectsInactive.Include);
            }

            if (itemMenuUI == null)
            {
                Debug.LogError("ItemMenuUI를 찾을 수 없습니다. InventoryPanel 프리팹에 ItemMenuUI가 있는지 확인하세요.", this);
                return;
            }

            itemMenuUI.ShowMenu(this, slotData);
        }
    }
}
