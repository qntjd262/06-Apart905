using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyBindList : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bindText;
    [SerializeField] private Button bindButton;

    public EKeyAction TargetAction { get; private set; }
    public KeyCode TempKey { get; private set; }

    public void Initialize(EKeyAction action, KeyCode currentKey, Action<KeyBindList> onBindButtonClicked)
    {
        TargetAction = action;

        titleText.text = GetActionName(action);

        UpdateUI(currentKey);

        bindButton.onClick.RemoveAllListeners();
        bindButton.onClick.AddListener(() => onBindButtonClicked(this));
    }

    private string GetActionName(EKeyAction action)
    {
        switch (action)
        {
            case EKeyAction.MoveUp: return "앞으로 이동";
            case EKeyAction.MoveDown: return "뒤로 이동";
            case EKeyAction.MoveLeft: return "왼쪽 이동";
            case EKeyAction.MoveRight: return "오른쪽 이동";
            case EKeyAction.Sprint: return "달리기";
            case EKeyAction.Crouch: return "앉기";
            case EKeyAction.Interact: return "상호작용";
            case EKeyAction.Attack: return "공격";
            case EKeyAction.Flashlight: return "손전등";
            case EKeyAction.Inventory: return "인벤토리";
            case EKeyAction.QuickSlot1: return "퀵슬롯 1";
            case EKeyAction.QuickSlot2: return "퀵슬롯 2";
            case EKeyAction.QuickSlot3: return "퀵슬롯 3";
            case EKeyAction.QuickSlot4: return "퀵슬롯 4";
            case EKeyAction.QuickSlot5: return "퀵슬롯 5";
            default: return action.ToString();
        }
    }

    public void UpdateUI(KeyCode key)
    {
        TempKey = key;
        if (key == KeyCode.None)
        {
            bindText.text = "비어있음";
            bindText.color = Color.gray; 
        }
        else
        {
            bindText.text = key.ToString();
            bindText.color = Color.white;
        }
    }

    public void SetWaitingState()
    {
        bindText.text = "<입력 대기>";
        bindText.color = Color.red;
    }
}