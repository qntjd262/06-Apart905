using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public QuestUI questUI;
    public List<Quest> activeQuests = new List<Quest>();

    public List<string> completedQuestNames = new List<string>();

    private Quest pendingQuest;

    private PlayerStat pendingPlayer;

    public Quest trackingQuest; // 현재 메인 트래커에 표시될 퀘스트

    public bool IsQuestAvailable(Quest quest)
    {
        if (quest == null) return false;

        if (completedQuestNames.Contains(quest.questName)) return false;

        if (activeQuests.Exists(q => q.questName == quest.questName)) return false;

        if (quest.prevQuest != null)
        {
            if (!completedQuestNames.Contains(quest.prevQuest.questName))
            {
                return false;
            }
        }
        return true;
    }

    public void SetTrackingQuest(Quest quest)
    {
        trackingQuest = quest;

        // 메인 트래커 갱신
        if (UIManager.Instance != null && UIManager.Instance.MainTracker != null && quest != null)
        {
            UIManager.Instance.MainTracker.Setup(quest);
        }
    }

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

    public void AcceptQuest(Quest newQuest, PlayerStat player)
    {
        if (activeQuests.Exists(q => q.questName == newQuest.questName))
        {
            Debug.Log($"[{newQuest.questName}]은 이미 진행 중인 퀘스트입니다.");
            if (player != null) player.isInteracting = false;

            if (DialogueManager.Instance != null) DialogueManager.Instance.EndDialogue();
            return;
        }

        pendingQuest = newQuest;
        pendingPlayer = player;

        if (QuestNotifyUI.Instance != null)
        {
            QuestNotifyUI.Instance.ShowNotice(newQuest);
        }
        else
        {
            Debug.LogError("씬에 오브젝트를 찾을수 없습니다");
            if (pendingPlayer != null) pendingPlayer.isInteracting = false;
            if (DialogueManager.Instance != null) DialogueManager.Instance.EndDialogue();
        }
        if (trackingQuest == null) SetTrackingQuest(newQuest);
    }

    public void ConfirmAcceptQuest()
    {
        if (pendingQuest == null) return;

        if (pendingQuest.type == QuestType.ItemCollection)
        {
            if (InventoryManager.Instance != null)
            {
                int alreadyHaveCount = InventoryManager.Instance.GetItemCount(pendingQuest.targetID);
                pendingQuest.currentAmount = alreadyHaveCount;
            }
        }

        activeQuests.Add(pendingQuest);
        Debug.Log($"{pendingQuest.questName} 퀘스트를 수락하였습니다.");

        pendingQuest = null;

        if (pendingPlayer != null)
        {
            pendingPlayer.isInteracting = false;
            pendingPlayer = null;
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.EndDialogue();
        }

        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPC npc in allNPCs)
        {
            npc.UpdateOutlineColor();
        }

        if (questUI != null) questUI.RefreshQuestList();
        else
        {
            QuestUI ui = FindAnyObjectByType<QuestUI>(FindObjectsInactive.Include);
            if (ui != null) ui.RefreshQuestList();
        }
    }

    public void CancelQuest()
    {
        Debug.Log("퀘스트 수락을 거절했습니다.");
        pendingQuest = null; //대기 중인 퀘스트 취소

        if (pendingPlayer != null)
        {
            pendingPlayer.isInteracting = false;
            pendingPlayer = null;

        }
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.EndDialogue();
        }
    }

    // public void NotifyEvent(QuestType type, string id, int amount)
    // {
    //     foreach (var quest in activeQuests)
    //     {
    //         if (quest.type == type)
    //         {
    //             quest.UpdateProgress(id, amount);
    //         }
    //     }
    //     //UI용 추가
    //     QuestTrackerUI[] trackers = FindObjectsOfType<QuestTrackerUI>();
    //     foreach (var tracker in trackers)
    //     {
    //         tracker.UpdateProgress();
    //     }

    //     if (questUI != null && questUI.gameObject.activeSelf)
    //     {
    //         questUI.RefreshQuestList();
    //     }
    // }

    public void NotifyEvent(QuestType type, string id, int amount)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.type == type)
            {
                quest.UpdateProgress(id, amount);
            }
        }

        if (UIManager.Instance != null && UIManager.Instance.MainTracker != null)
        {
            UIManager.Instance.MainTracker.UpdateProgress();
        }

        if (questUI != null && questUI.gameObject.activeSelf)
        {
            questUI.RefreshQuestList();
        }
        else
        {
            QuestUI ui = FindFirstObjectByType<QuestUI>(FindObjectsInactive.Include);
            if (ui != null) ui.RefreshQuestList();
        }
    }

    public void OnPickUp()
    {

    }

    public void OnKill()
    {

    }
}
