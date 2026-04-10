using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button slotButton;
    [SerializeField] private Image outlineImage; // 선택 시 보여줄 테두리 이미지

    private int slotIndex;
    private Action<int> onSelectAction;

    // 담당자가 나중에 데이터를 던져줄 때 받을 통로
    public void Initialize(int index, string displayName, string dateString, Action<int> onSelect)
    {
        slotIndex = index;
        onSelectAction = onSelect;

        slotNameText.text = displayName;
        dateText.text = dateString;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotSelect);
        
        SetSelected(false); // 초기 상태는 선택 해제
    }

    private void OnSlotSelect()
    {
        onSelectAction?.Invoke(slotIndex);
    }

    public void SetSelected(bool isSelected)
    {
        if (outlineImage != null)
        {
            outlineImage.enabled = isSelected;
        }
    }
}