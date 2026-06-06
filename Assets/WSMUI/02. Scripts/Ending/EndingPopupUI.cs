using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

// [추가] CanvasGroup이 무조건 있도록 강제
[RequireComponent(typeof(CanvasGroup))]
public class EndingPopupUI : BasePopupUI
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    [Header("Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f);
    private EndingData currentEnding;

    // [추가] CanvasGroup 캐싱
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

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

        // [수정] 튕기는 스케일 대신 살짝 큰 상태에서 정상 크기로 돌아오며 페이드 인
        popupPanel.transform.DOKill();
        canvasGroup.DOKill();

        popupPanel.transform.localScale = Vector3.one * 1.05f;
        canvasGroup.alpha = 0f;

        popupPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
        canvasGroup.DOFade(1f, 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    private void OnAccept()
    {
        acceptButton.interactable = false;
        GameManager.Instance.selectedEnding = currentEnding;

        popupPanel.transform.DOScale(0.9f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true);
        canvasGroup.DOFade(0f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
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
        popupPanel.transform.DOScale(0.9f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true);
        canvasGroup.DOFade(0f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
        {
            HidePanel();
        });
    }
}