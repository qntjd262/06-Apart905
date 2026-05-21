using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public QuestUI questUI;
    public List<Quest> activeQuests = new List<Quest>();

    #region UI파트 추가
    public Quest trackingQuest; // 현재 메인 트래커에 표시될 퀘스트

    // 특정 퀘스트를 추적 대상으로 설정하는 함수
    public void SetTrackingQuest(Quest quest)
    {
        trackingQuest = quest;

        // 메인 트래커 갱신
        if (UIManager.Instance != null && UIManager.Instance.MainTracker != null && quest != null)
        {
            UIManager.Instance.MainTracker.Setup(quest);
        }
    }
    #endregion
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AcceptQuest(Quest newQuest)
    {
        // 1. 중복 체크 방식을 '주소'가 아닌 '이름' 비교로 변경 (매우 중요)
        if (activeQuests.Exists(q => q.questName == newQuest.questName))
        {
            Debug.Log($"[{newQuest.questName}]은 이미 진행 중인 퀘스트입니다.");
            return;
        }

        if (newQuest.type == QuestType.ItemCollection)
        {
            if (InventoryManager.Instance != null)
            {
                int alreadyHaveCount = InventoryManager.Instance.GetItemCount(newQuest.targetID);
                newQuest.currentAmount = alreadyHaveCount;
            }
        }

        // 2. 리스트에 추가
        activeQuests.Add(newQuest);
        Debug.Log($"{newQuest.questName} 퀘스트를 수락했습니다.");

        // 3. UI 새로고침 강제 실행 (이 코드가 있어야 Content에 자식이 생깁니다!)
        QuestUI ui = FindObjectOfType<QuestUI>(true);
        if (ui != null)
        {
            ui.RefreshQuestList();
        }
        else
        {
            Debug.LogWarning("씬에서 QuestUI를 찾을 수 없습니다. UI가 생성되지 않습니다.");
        }
        if (trackingQuest == null) SetTrackingQuest(newQuest);
    }

    public void NotifyEvent(QuestType type, string id, int amount)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.type == type)
            {
                quest.UpdateProgress(id, amount);
            }
        }
        //UI용 추가
        QuestTrackerUI[] trackers = FindObjectsOfType<QuestTrackerUI>();
        foreach (var tracker in trackers)
        {
            tracker.UpdateProgress();
        }

        if (questUI != null && questUI.gameObject.activeSelf)
        {
            questUI.RefreshQuestList();
        }
    }

    public void OnPickUp()
    {

    }

    public void OnKill()
    {

    }
}
