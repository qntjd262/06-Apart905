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

        QuestManager.Instance.SetTrackingQuest(quests[currentIndex]);
    }

    public void RefreshDisplay()
    {
        if (titleText == null || goalText == null)
        {
            Debug.LogWarning("QuestSelectorUI: UI Text 컴포넌트가 할당되지 않았습니다.");
            return;
        }

        if (QuestManager.Instance == null) return;

        var quests = QuestManager.Instance.activeQuests;

        if (quests == null || quests.Count == 0)
        {
            titleText.text = "진행 중인 퀘스트 없음";
            goalText.text = "";
            return;
        }

        if (currentIndex < 0 || currentIndex >= quests.Count) currentIndex = 0;

        Quest q = quests[currentIndex];

        if (q != null)
        {
            titleText.text = q.questName;
            titleText.color = q.isMainQuest ? new Color(0.8f, 0.4f, 0f) : Color.green;
            
            if (q.type == QuestType.ItemCollection)
            {
                string progressDisplay = "";
                for (int i = 0; i < q.objectives.Count; i++)
                {
                    var obj = q.objectives[i];
                    progressDisplay += $"{obj.targetID} ({obj.currentAmount}/{obj.goalAmount})";

                    if (i < q.objectives.Count - 1)
                    {
                        progressDisplay += "\n";
                    }
                }
                goalText.text = progressDisplay;
            }
            else
            {
                goalText.text = q.questGoal;
            }
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