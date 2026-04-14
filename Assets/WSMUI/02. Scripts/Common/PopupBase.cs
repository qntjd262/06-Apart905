using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    private void OnEnable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OpenPopupWithEffects(GetTitle());
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ClosePopupWithEffects();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    protected virtual string GetTitle() => string.Empty;
}
