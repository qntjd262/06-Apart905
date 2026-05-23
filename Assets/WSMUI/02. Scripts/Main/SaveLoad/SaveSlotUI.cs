using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button slotButton;
    [SerializeField] private Image outlineImage;

    private int slotIndex;
    private bool hasSaveData; // [추가] 해당 슬롯에 데이터가 있는지 여부
    private Action<int, bool> onSelectAction; // [수정] Action이 데이터 유무도 전달하게 변경

    public void Initialize(int index, string displayName, string dateString, bool hasData, Action<int, bool> onSelect)
    {
        slotIndex = index;
        hasSaveData = hasData;
        onSelectAction = onSelect;

        slotNameText.text = displayName;
        dateText.text = dateString;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotSelect);
    }

    private void OnSlotSelect()
    {
        onSelectAction?.Invoke(slotIndex, hasSaveData);
    }

    public void SetSelected(bool isSelected)
    {
        if (outlineImage != null)
        {
            outlineImage.enabled = isSelected;
        }
    }
}