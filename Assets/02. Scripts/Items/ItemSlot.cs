using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData currentItemData; // 현재 이 슬롯에 들어있는 데이터
    public Image iconImage;          // 슬롯에 보일 아이콘 이미지 UI

    public void UpdateSlot(ItemData newData)
    {
        currentItemData = newData;
        if (currentItemData != null)
        {
            iconImage.sprite = currentItemData.icon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItemData != null)
        {
            TooltipManager.Instance.ShowTooltip(currentItemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}