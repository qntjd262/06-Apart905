using UnityEngine;
using System;

public abstract class BasePopupUI : MonoBehaviour
{
    [Header("Base UI Components")]
    [SerializeField] protected GameObject popupPanel;

    // 창이 닫힐 때 실행될 함수를 담아두는 델리게이트
    public Action onCloseAction; 

    protected virtual void ShowPanel()
    {
        if (popupPanel == null) popupPanel = this.gameObject; 

        popupPanel.SetActive(true);
        UIManager.Instance.RegisterUI(popupPanel);
    }

    protected virtual void HidePanel()
    {
        if (popupPanel == null) popupPanel = this.gameObject;

        UIManager.Instance.UnregisterUI(popupPanel);
        popupPanel.SetActive(false);

        // 창이 꺼지면서, 예약된 작업이 있다면 실행하고 비운다.
        onCloseAction?.Invoke();
        onCloseAction = null;
    }
}