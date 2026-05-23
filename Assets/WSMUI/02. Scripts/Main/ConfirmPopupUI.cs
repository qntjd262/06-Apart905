using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopupUI : BasePopupUI
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("버튼 연결")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action _onConfirmAction;

    private void Awake()
    {
        popupPanel = this.gameObject;
        
        confirmButton.onClick.AddListener(OnConfirmClick);
        cancelButton.onClick.AddListener(OnCancelClick);
    }

    public void Show(string content, Action onConfirm)
    {
        if (contentText != null) contentText.text = content;
        
        _onConfirmAction = onConfirm;

        ShowPanel();
    }

    private void OnConfirmClick()
    {
        _onConfirmAction?.Invoke(); 
        HidePanel(); 
    }

    private void OnCancelClick()
    {
        HidePanel(); 
    }
}