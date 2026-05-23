using UnityEngine;
using TMPro;
using System.Text;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipWindow;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText; // 아이템 분류 표시용
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemEffectText; // 소모품 효과 표시용

    private void Awake()
    {
        Instance = this;
        tooltipWindow.SetActive(false);
    }

    public void ShowTooltip(ItemData data)
    {
        itemNameText.text = data.itemName;
        itemDescriptionText.text = data.description;
        itemTypeText.text = GetTypeText(data.itemType); // 타입을 한글로 변환

        if (data.itemType == ItemType.Eatable && data is EatableItemData eatableData)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var effect in data.eatables)
            {
                sb.AppendLine($"{GetStatName(effect.type)}: +{effect.value}");
            }
            itemEffectText.text = sb.ToString();
            itemEffectText.gameObject.SetActive(true);
        }
        else
        {
            itemEffectText.gameObject.SetActive(false);
        }

        tooltipWindow.SetActive(true);
    }

    private string GetTypeText(ItemType type)
    {
        return type switch {
            ItemType.Equipable => "<color=#FFCC00>[장착 아이템]</color>",
            ItemType.Eatable => "<color=#00FF00>[소모 아이템]</color>",
            ItemType.Useable => "<color=#00FFFF>[특수 아이템]</color>",
            _ => ""
        };
    }

    private string GetStatName(EatableType type)
    {
        return type switch {
            EatableType.Hunger => "허기",
            EatableType.Thirst => "갈증",
            EatableType.Health => "체력",
            EatableType.Stamina => "스태미나",
            EatableType.Infection => "감염도",
            _ => ""
        };
    }

    public void HideTooltip() => tooltipWindow.SetActive(false);
}
