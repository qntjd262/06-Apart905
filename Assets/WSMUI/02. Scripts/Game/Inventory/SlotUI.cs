using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    // 프로퍼티를 이용한 접근 제어
    public int SlotIndex { get; set; }
    public bool IsQuickSlot { get; set; }

    private static SlotUI draggingSlot;

    public void UpdateSlot(InventorySlot slotData)
    {
        if (slotData.IsEmpty)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
            amountText.text = "";
        }
        else
        {
            icon.sprite = slotData.item.icon;
            icon.color = new Color(1, 1, 1, 1);
            amountText.text = slotData.amount > 1 ? slotData.amount.ToString() : "";
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

        if (!draggingSlot.IsQuickSlot && this.IsQuickSlot)
        {
            InventoryManager.Instance.SwapItemBetweenBagAndQuickSlot(draggingSlot.SlotIndex, this.SlotIndex);
        }
        else if (draggingSlot.IsQuickSlot && !this.IsQuickSlot)
        {
            InventoryManager.Instance.SwapItemBetweenBagAndQuickSlot(this.SlotIndex, draggingSlot.SlotIndex);
        }
        else if (!draggingSlot.IsQuickSlot && !this.IsQuickSlot)
        {
            InventoryManager.Instance.SwapItemWithinBag(draggingSlot.SlotIndex, this.SlotIndex);
        }
        else if (draggingSlot.IsQuickSlot && this.IsQuickSlot)
        {
            InventoryManager.Instance.SwapItemWithinQuickSlot(draggingSlot.SlotIndex, this.SlotIndex);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (icon.sprite != null)
            {
                PlayerStat player = GameObject.FindWithTag("Player").GetComponent<PlayerStat>();
                InventoryManager.Instance.UseItem(SlotIndex, IsQuickSlot, player);
            }
        }
    }

    private void UseItem()
    {
        PlayerStat player = GameObject.FindWithTag("Player").GetComponent<PlayerStat>();

        if (player != null)
        {
            InventoryManager.Instance.UseItem(SlotIndex, IsQuickSlot, player);
            Debug.Log($"슬롯 {SlotIndex} (퀵슬롯 여부: {IsQuickSlot}) 아이템 사용 시도");
        }
    }
}