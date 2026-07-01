using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// [추가] CanvasGroup이 무조건 있도록 강제
[RequireComponent(typeof(CanvasGroup))]
public class QuestNotifyUI : BasePopupUI
{
    public static QuestNotifyUI Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private Button acceptButton;

    [Header("Color Settings")]
    [SerializeField] private Color mainQuestColor = new Color(0.8f, 0.4f, 0f);
    [SerializeField] private Color subQuestColor = Color.green;

    private Quest currentQuest;

    // [추가] CanvasGroup 캐싱
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();

        acceptButton.onClick.AddListener(OnAcceptButtonClicked);

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public void ShowNotice(Quest quest)
    {
        currentQuest = quest;

        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;

        storyText.text = quest.questDiscrip;
        goalText.text = $"□ {quest.questGoal}";

        ShowPanel();

        // [수정] 튕기는 스케일(OutBack) 제거, 무게감 있게 스르륵 나타나는 연출 적용
        popupPanel.transform.DOKill();
        canvasGroup.DOKill();

        popupPanel.transform.localScale = Vector3.one * 1.05f;
        canvasGroup.alpha = 0f;

        popupPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
        canvasGroup.DOFade(1f, 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    private void OnAcceptButtonClicked()
    {
        acceptButton.interactable = false;

        // [수정] 0.15초 만에 순식간에 사라지며 게임으로 빠른 복귀
        popupPanel.transform.DOScale(0.9f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true);
        canvasGroup.DOFade(0f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
        {
            acceptButton.interactable = true;
            HidePanel();
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.ConfirmAcceptQuest();
            }
        });
    }
}