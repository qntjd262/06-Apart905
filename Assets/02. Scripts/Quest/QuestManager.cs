using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public QuestUI questUI;
    public List<Quest> activeQuests = new List<Quest>();
    public List<string> completedQuestNames = new List<string>();

    private Quest pendingQuest;
    private PlayerStat pendingPlayer;
    public Quest trackingQuest; // 현재 메인 트래커에 표시될 퀘스트

    [Header("모든 퀘스트 데이터베이스")]
    public List<Quest> allQuests = new List<Quest>();

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

    private void Start()
    {
        CheckAndAutoAcceptQuests();
    }

    public bool IsQuestAvailable(Quest quest)
    {
        if (quest == null) return false;
        if (completedQuestNames.Contains(quest.questName)) return false;
        if (activeQuests.Exists(q => q.questName == quest.questName)) return false;

        if (pendingQuest != null && pendingQuest.questName == quest.questName) return false;

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

        if (UIManager.Instance != null && UIManager.Instance.MainTracker != null && quest != null)
        {
            UIManager.Instance.MainTracker.Setup(quest);
        }
    }

    public void CheckAndAutoAcceptQuests()
    {
        PlayerStat player = FindFirstObjectByType<PlayerStat>(); 

        foreach (var quest in allQuests)
        {
            if (!quest.isCompleted && 
                !activeQuests.Exists(q => q.questName == quest.questName) && 
                quest.isAutoAccept && 
                IsQuestAvailable(quest))
            {
                AcceptQuest(quest, player);
                Debug.Log($"[자동 수락 감지] '{quest.questName}' 퀘스트 팝업을 요청합니다.");
                
                break; 
            }
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
        pendingPlayer = player; // 자동 수락일 경우 null일 수 있음

        // 자동 수락이든 NPC 수락이든 팝업창 띄움
        if (QuestNotifyUI.Instance != null)
        {
            QuestNotifyUI.Instance.ShowNotice(newQuest);
            
            if (newQuest.isAutoAccept && pendingPlayer != null)
            {
                pendingPlayer.isInteracting = true;
            }
        }
        else
        {
            Debug.LogError("씬에 QuestNotifyUI 오브젝트를 찾을수 없습니다");
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

        Quest acceptedQuest = pendingQuest; 
        activeQuests.Add(acceptedQuest);
        Debug.Log($"{acceptedQuest.questName} 퀘스트를 수락하였습니다.");

        pendingQuest = null;

        // 대화 중이거나 자동 수락으로 묶여있던 플레이어 상태 해제
        if (pendingPlayer != null)
        {
            pendingPlayer.isInteracting = false;
            pendingPlayer = null;
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.EndDialogue();
        }

        UpdateAllNPCOutlines();
        RefreshQuestUIs();

        if (acceptedQuest.currentAmount >= acceptedQuest.goalAmount && acceptedQuest.isAutoComplete)
        {
            CompleteQuestInstantly(acceptedQuest);
        }
        else
        {
            CheckAndAutoAcceptQuests();
        }
    }

    public void CancelQuest()
    {
        Debug.Log("퀘스트 수락을 거절했습니다.");
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
        
        CheckAndAutoAcceptQuests();
    }

    public void NotifyEvent(QuestType type, string id, int amount)
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            if (activeQuests[i].type == type)
            {
                activeQuests[i].UpdateProgress(type, id, amount);
            }
        }

        if (UIManager.Instance != null && UIManager.Instance.MainTracker != null)
        {
            UIManager.Instance.MainTracker.UpdateProgress();
        }

        RefreshQuestUIs();
    }

    public void CompleteQuestInstantly(Quest quest)
    {
        if (quest.isCompleted) return;

        quest.isCompleted = true;

        if (!completedQuestNames.Contains(quest.questName))
        {
            completedQuestNames.Add(quest.questName);
        }

        if (quest.type == QuestType.ItemCollection && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItem(quest.targetID, quest.goalAmount);
        }

        if (quest.rewardItemID != null && InventoryManager.Instance != null)
        {
            bool isSuccess = InventoryManager.Instance.AddItem(quest.rewardItemID);
            Debug.Log($"[즉시 완료 보상] {quest.rewardItemID.itemName}를 획득하였습니다!");
        }

        activeQuests.RemoveAll(q => q.questName == quest.questName);

        if (trackingQuest != null && trackingQuest.questName == quest.questName)
        {
            trackingQuest = null;

            if (activeQuests.Count > 0)
            {
                SetTrackingQuest(activeQuests[0]);
            }
            else
            {
                // 트래커 UI 숨기기 등
            }
        }

        UpdateAllNPCOutlines();
        RefreshQuestUIs();

        Debug.Log($"[즉시 완료] {quest.questName} 퀘스트가 조건을 만족하여 즉시 완료되었습니다.");

        CheckAndAutoAcceptQuests();
    }

    private void UpdateAllNPCOutlines()
    {
        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPC npc in allNPCs)
        {
            npc.UpdateOutlineColor();
        }
    }

    private void RefreshQuestUIs()
    {
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

    public void OnPickUp(string itemName, int amount)
    {
        NotifyEvent(QuestType.ItemCollection, itemName, amount);
    }

    public void OnKill(string zombieID, int amount)
    {
        NotifyEvent(QuestType.ZombieHunt, zombieID, amount);
    }
}