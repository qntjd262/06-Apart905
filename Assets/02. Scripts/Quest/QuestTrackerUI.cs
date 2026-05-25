using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public Image strikeLine;

    [Header("Color Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f); // 어두운 주황색
    private Color subQuestColor = Color.green;

    private Quest targetQuest;
    private bool effectPlayed = false;

    void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterTracker(this);
        }
    }

    private void OnEnable()
    {
        if (targetQuest == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        UpdateProgress();
    }

    public void Setup(Quest quest)
    {
        progressText.DOKill();
        strikeLine.rectTransform.DOKill();

        targetQuest = quest;

        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }

        effectPlayed = false;

        // 1. 제목 색상 설정 (메인/일반)
        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;

        // 2. 취소선 초기화
        strikeLine.rectTransform.localScale = new Vector3(0, 1, 1);
        progressText.color = Color.white;
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        if (targetQuest == null) return;

        progressText.text = $"ㆍ {targetQuest.targetID} ({targetQuest.currentAmount}/{targetQuest.goalAmount})";

        if (targetQuest.currentAmount >= targetQuest.goalAmount && !effectPlayed)
        {
            PlayCompleteEffect();
        }
    }

    private void PlayCompleteEffect()
    {
        effectPlayed = true;

        float textWidth = progressText.preferredWidth;

        RectTransform lineRect = strikeLine.rectTransform;
        lineRect.sizeDelta = new Vector2(textWidth, lineRect.sizeDelta.y);

        lineRect.localScale = new Vector3(0, 1, 1);
        lineRect.DOScaleX(0.6f, 0.5f).SetEase(Ease.OutQuad);

        progressText.DOColor(Color.gray, 0.5f);
    }

    public void HideTracker()
    {
        progressText.DOKill();
        strikeLine.rectTransform.DOKill();

        targetQuest = null;
        effectPlayed = false;
        this.gameObject.SetActive(false);
    }
}