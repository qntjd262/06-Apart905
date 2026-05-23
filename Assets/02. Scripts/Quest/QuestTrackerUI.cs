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
    private Color subQuestColor = Color.blue;

    private Quest targetQuest;
    private bool effectPlayed = false;

    void Awake()
    {
        // 생성되자마자 UIManager에게 자신을 등록
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterTracker(this);
        }
    }

    private void OnEnable()
    {
        UpdateProgress(); // 다시 켜질 때 최신 데이터로 갱신
    }
    public void Setup(Quest quest)
    {
        targetQuest = quest;
        effectPlayed = false;

        // 1. 제목 색상 설정 (메인/일반)
        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;

        // 2. 취소선 초기화
        strikeLine.rectTransform.localScale = new Vector3(0, 1, 1);

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

        lineRect.localScale = new Vector3(0, 1, 1); // 시작은 0
        lineRect.DOScaleX(1f, 0.5f).SetEase(Ease.OutQuad);

        progressText.DOColor(Color.gray, 0.5f);

        // 여기서 만약 "다음 목표"가 있다면 텍스트를 교체하는 로직을 추가할 수 있습니다.
        // 현재 담당자 코드는 단일 목표 구조이므로, 완료 연출까지만 표시합니다.
    }
}