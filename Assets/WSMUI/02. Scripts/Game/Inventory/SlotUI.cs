using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;

    // 프로퍼티를 이용한 접근 제어
    public int SlotIndex { get; set; }
    public bool IsQuickSlot { get; set; }

    private static SlotUI draggingSlot;

    public bool IsStorageSlot { get; set; }


    public void UpdateSlot(InventorySlot slotData)
    {
        if (slotData.IsEmpty)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }
        else
        {
            icon.sprite = slotData.item.icon;
            icon.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (icon.sprite == null) return;
        draggingSlot = this;
        icon.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingSlot != this) return;
        draggingSlot = null;
        if (icon.sprite != null) icon.color = new Color(1, 1, 1, 1f);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggingSlot == null || draggingSlot == this) return;

        // 1. 퀵슬롯 관련 스왑
        if (draggingSlot.IsQuickSlot || this.IsQuickSlot)
        {
            if (draggingSlot.IsStorageSlot || this.IsStorageSlot)
            {
                Debug.Log("창고와 퀵슬롯 간의 직접 이동은 불가능합니다.");
                return;
            }

            // 가방 <-> 퀵슬롯 혹은 퀵슬롯 간 스왑
            if (!draggingSlot.IsQuickSlot && this.IsQuickSlot)
                InventoryManager.Instance.SwapItemBetweenBagAndQuickSlot(draggingSlot.SlotIndex, this.SlotIndex);
            else if (draggingSlot.IsQuickSlot && !this.IsQuickSlot)
                InventoryManager.Instance.SwapItemBetweenBagAndQuickSlot(this.SlotIndex, draggingSlot.SlotIndex);
            else if (draggingSlot.IsQuickSlot && this.IsQuickSlot)
                InventoryManager.Instance.SwapItemWithinQuickSlot(draggingSlot.SlotIndex, this.SlotIndex);
        }
        // 2. 창고 관련 스왑
        else if (draggingSlot.IsStorageSlot || this.IsStorageSlot)
        {
            if (draggingSlot.IsStorageSlot && !this.IsStorageSlot)
                InventoryManager.Instance.SwapItemBetweenStorageAndBag(draggingSlot.SlotIndex, this.SlotIndex);
            else if (!draggingSlot.IsStorageSlot && this.IsStorageSlot)
                InventoryManager.Instance.SwapItemBetweenStorageAndBag(this.SlotIndex, draggingSlot.SlotIndex);
        }
        // 3. 일반 가방 내 스왑
        else
        {
            InventoryManager.Instance.SwapItemWithinBag(draggingSlot.SlotIndex, this.SlotIndex);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (IsStorageSlot || icon.sprite == null || IsQuickSlot)
            {
                // 만약 퀵슬롯에서 우클릭했을 때 '해제' 기능을 넣고 싶다면 여기에 작성
                if (IsQuickSlot)
                {
                    Debug.Log("퀵슬롯 아이템은 가방에서 관리하거나 단축키를 이용하세요.");
                }
                return;
            }
            InventorySlot slotData = InventoryManager.Instance.BagSlots[SlotIndex];

            if (slotData != null && slotData.item != null)
            {
                if (slotData.item.itemType == ItemType.Useable)
                {
                    Debug.Log("퀘스트 아이템은 조작할 수 없습니다.");
                    return;
                }
                ItemMenuUI.Instance.ShowMenu(this, slotData);
            }
        }
    }

}