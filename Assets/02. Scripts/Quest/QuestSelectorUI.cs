using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSelectorUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI goalText;
    public Button leftArrow;
    public Button rightArrow;

    private int currentIndex = 0;

    void OnEnable()
    {
        // 인벤토리 열릴 때 현재 추적 중인 퀘스트의 인덱스를 먼저 찾음
        UpdateIndexToTrackingQuest();
        RefreshDisplay();
    }

    void Start()
    {
        leftArrow.onClick.AddListener(() => ChangeQuest(-1));
        rightArrow.onClick.AddListener(() => ChangeQuest(1));
    }

    private void ChangeQuest(int direction)
    {
        var quests = QuestManager.Instance.activeQuests;
        if (quests.Count == 0) return;

        // 인덱스 순환 로직
        currentIndex += direction;
        if (currentIndex < 0) currentIndex = quests.Count - 1;
        else if (currentIndex >= quests.Count) currentIndex = 0;

        RefreshDisplay();

        // [핵심] 여기서 바꾼 퀘스트를 바로 메인 트래커 추적 대상으로 설정
        QuestManager.Instance.SetTrackingQuest(quests[currentIndex]);
    }

    public void RefreshDisplay()
    {
        // 1. UI 컴포넌트 자체가 인스펙터에서 할당 안 됐을 경우 방어
        if (titleText == null || goalText == null)
        {
            Debug.LogWarning("QuestSelectorUI: UI Text 컴포넌트가 할당되지 않았습니다.");
            return;
        }

        // 2. QuestManager 인스턴스 확인
        if (QuestManager.Instance == null) return;

        var quests = QuestManager.Instance.activeQuests;

        // 3. 리스트가 비어있을 때 처리
        if (quests == null || quests.Count == 0)
        {
            titleText.text = "진행 중인 퀘스트 없음";
            goalText.text = "";
            return;
        }

        // 4. 인덱스 범위 안전성 확인
        if (currentIndex < 0 || currentIndex >= quests.Count) currentIndex = 0;

        Quest q = quests[currentIndex];

        // 5. 퀘스트 데이터 자체가 null인지 확인
        if (q != null)
        {
            titleText.text = q.questName;
            titleText.color = q.isMainQuest ? new Color(0.8f, 0.4f, 0f) : Color.green;
            goalText.text = $"{q.targetID} ({q.currentAmount}/{q.goalAmount})";
        }
    }

    private void UpdateIndexToTrackingQuest()
    {
        // 1. QuestManager 인스턴스 자체가 없는 경우 체크
        if (QuestManager.Instance == null) return;

        var quests = QuestManager.Instance.activeQuests;

        // 2. 리스트가 초기화되지 않았거나 비어있는 경우 체크
        if (quests == null || quests.Count == 0) return;

        if (QuestManager.Instance.trackingQuest != null)
        {
            currentIndex = quests.FindIndex(q => q.questName == QuestManager.Instance.trackingQuest.questName);
            if (currentIndex == -1) currentIndex = 0;
        }
    }
}