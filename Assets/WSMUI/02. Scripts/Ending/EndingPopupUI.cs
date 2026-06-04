using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EndingPopupUI : BasePopupUI 
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    [Header("Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f);
    private EndingData currentEnding;

    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAccept);
        cancelButton.onClick.AddListener(OnCancel);
        
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void ShowEndingPopup(EndingData data)
    {
        currentEnding = data;

        string coloredEndingName = $"<color=#{ColorUtility.ToHtmlStringRGB(mainQuestColor)}>{data.endingName}</color>";
        noticeText.text = $"{coloredEndingName} 엔딩\n진행하시겠습니까?";

        ShowPanel();

        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void OnAccept()
    {
        acceptButton.interactable = false;
        GameManager.Instance.selectedEnding = currentEnding;
        
        popupPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            HidePanel(); 
            
            UIManager.Instance.FadeOut(1.5f, () =>
            {
                SceneManager.LoadScene("PrototypeEnding");
            });
        });
    }

    private void OnCancel()
    {
        popupPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            HidePanel();
        });
    }
}