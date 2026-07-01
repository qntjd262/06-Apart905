using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public NPCdata npcData;

    [Header("Quest Settings")]
    [Tooltip("이 NPC에게서 '수락'할 수 있는 퀘스트 목록")]
    public List<Quest> startQuests = new List<Quest>(); 
    
    [Tooltip("이 NPC에게서 '완료'할 수 있는 퀘스트 목록")]
    public List<Quest> completeQuests = new List<Quest>();

    private Renderer myRenderer;
    private MaterialPropertyBlock PropertyBlock;

    void Awake()
    {
        myRenderer = GetComponentInChildren<Renderer>();
        PropertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        UpdateOutlineColor();
    }

    public void UpdateOutlineColor()
    {
        if (myRenderer == null) return;

        myRenderer.GetPropertyBlock(PropertyBlock);
        Color targetColor = Color.red;
        string status = "퀘스트 없음";

        if (completeQuests != null && completeQuests.Count > 0)
        {
            foreach (var q in completeQuests)
            {
                if (q == null || q.isCompleted) continue;

                bool isActive = QuestManager.Instance.activeQuests.Exists(activeQ => activeQ.questName == q.questName);
                if (isActive)
                {
                    if (q.IsAllObjectivesComplete())
                    {
                        targetColor = Color.green;
                        status = $"[{q.questName}] 목표달성 (완료 가능)";
                        SetOutline(targetColor, status);
                        return; 
                    }
                    else
                    {
                        targetColor = Color.red;
                        status = $"[{q.questName}] 목표치 부족 (진행중)";
                    }
                }
            }
        }

        if (startQuests != null && startQuests.Count > 0)
        {
            foreach (var q in startQuests)
            {
                if (q == null || q.isCompleted) continue;

                bool isActive = QuestManager.Instance.activeQuests.Exists(activeQ => activeQ.questName == q.questName);
                
                if (!QuestManager.Instance.IsQuestAvailable(q) && !isActive)
                {
                    if (status == "퀘스트 없음") status = $"[{q.questName}] 선행 미완료";
                }
                else if (isActive)
                {
                    if (status == "퀘스트 없음") status = $"[{q.questName}] 이미 진행 중";
                }
                else
                {
                    // 받을 수 있는 퀘스트 발견!
                    targetColor = Color.green;
                    status = $"[{q.questName}] 수락 전(보유중)";
                    SetOutline(targetColor, status);
                    return; 
                }
            }
        }

        SetOutline(targetColor, status);
    }

    private void SetOutline(Color color, string status)
    {
        Debug.Log($"{gameObject.name}의 퀘스트 상태: {status} / 설정 색상: {color}");
        PropertyBlock.SetColor("_Outline_color", color);
        myRenderer.SetPropertyBlock(PropertyBlock);
    }

    public string GetInteractText()
    {
        return "대화하기";
    }

    public Constants.InteractType GetInteractType()
    {
        return Constants.InteractType.Talk;
    }

    public void Interact(PlayerStat player)
    {
        if (player.isInteracting) return;

        Quest activeInteractionQuest = null;
        bool isCompleteFlow = false;

        if (completeQuests != null && completeQuests.Count > 0)
        {
            foreach (var q in completeQuests)
            {
                if (q != null && !q.isCompleted && QuestManager.Instance.activeQuests.Exists(activeQ => activeQ.questName == q.questName))
                {
                    activeInteractionQuest = q;
                    isCompleteFlow = true;
                    break; 
                }
            }
        }

        if (activeInteractionQuest == null && startQuests != null && startQuests.Count > 0)
        {
            foreach (var q in startQuests)
            {
                if (q != null && !q.isCompleted && QuestManager.Instance.IsQuestAvailable(q))
                {
                    activeInteractionQuest = q;
                    isCompleteFlow = false;
                    break; 
                }
            }
        }

        if (activeInteractionQuest == null)
        {
            if (npcData != null && NPCUIHandler.Instance != null)
                NPCUIHandler.Instance.ShowNPC(npcData);
            return;
        }

        if (npcData != null && NPCUIHandler.Instance != null)
        {
            NPCUIHandler.Instance.ShowNPC(npcData);
        }

        SyncQuestObjectives(activeInteractionQuest);

        string[] currentDialogues;
        bool isAlreadyActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == activeInteractionQuest.questName);

        if (!isAlreadyActive)
        {
            currentDialogues = activeInteractionQuest.beforeAcceptDialogues;
        }
        else if (activeInteractionQuest.IsAllObjectivesComplete())
        {
            currentDialogues = activeInteractionQuest.completeDialogues;
        }
        else
        {
            currentDialogues = activeInteractionQuest.duringAcceptDialogues;
        }

        player.isInteracting = true;

        DialogueManager.Instance.StartDialogue(npcData, currentDialogues, () => {
            
            SyncQuestObjectives(activeInteractionQuest);
            bool stillActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == activeInteractionQuest.questName);

            if (!stillActive && !activeInteractionQuest.isCompleted && !isCompleteFlow)
            {
                QuestManager.Instance.AcceptQuest(activeInteractionQuest, player);
                
                if (activeInteractionQuest.type == QuestType.Interact && npcData != null)
                {
                    foreach (var obj in activeInteractionQuest.objectives)
                    {
                        if (obj.targetID == npcData.NpcID)
                        {
                            activeInteractionQuest.ForceSyncProgress(obj.targetID, obj.goalAmount);
                        }
                    }
                }
            }
            else
            {
                if (activeInteractionQuest.IsAllObjectivesComplete() && !activeInteractionQuest.isCompleted && isCompleteFlow)
                {
                    CompleteQuest(activeInteractionQuest);
                }
                
                player.isInteracting = false;
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.EndDialogue();
                }
            }
            UpdateOutlineColor();
        });
    }

    private void SyncQuestObjectives(Quest quest)
    {
        if (quest == null) return;

        if (quest.type == QuestType.ItemCollection)
        {
            for (int i = 0; i < quest.objectives.Count; i++)
            {
                var obj = quest.objectives[i];
                int currentBagCount = InventoryManager.Instance.GetItemCount(obj.targetID);
                quest.ForceSyncProgress(obj.targetID, currentBagCount);
            }
        }
        else if (quest.type == QuestType.Interact && QuestManager.Instance.activeQuests.Exists(q => q.questName == quest.questName))
        {
            if (npcData != null)
            {
                for (int i = 0; i < quest.objectives.Count; i++)
                {
                    var obj = quest.objectives[i];
                    if (obj.targetID == npcData.NpcID)
                    {
                        quest.ForceSyncProgress(obj.targetID, obj.goalAmount);
                    }
                }
            }
        }
    }

    void CompleteQuest(Quest targetQuest)
    {
        targetQuest.isCompleted = true;

        if (QuestManager.Instance != null && !QuestManager.Instance.completedQuestNames.Contains(targetQuest.questName))
        {
            QuestManager.Instance.completedQuestNames.Add(targetQuest.questName);
        }

        if (targetQuest.type == QuestType.ItemCollection)
        {
            foreach (var obj in targetQuest.objectives)
            {
                InventoryManager.Instance.RemoveItem(obj.targetID, obj.goalAmount);
            }
        }

        if (targetQuest.rewardItemID != null)
        {
            InventoryManager.Instance.AddItem(targetQuest.rewardItemID);
            Debug.Log($"{targetQuest.rewardItemID.itemName}를 획득하였습니다!");
        }

        QuestManager.Instance.activeQuests.RemoveAll(q => q.questName == targetQuest.questName);

        if (QuestManager.Instance.trackingQuest != null && QuestManager.Instance.trackingQuest.questName == targetQuest.questName)
        {
            QuestManager.Instance.trackingQuest = null;

            if (QuestManager.Instance.activeQuests.Count > 0)
            {
                Quest nextQuest = QuestManager.Instance.activeQuests[0];
                QuestManager.Instance.SetTrackingQuest(nextQuest);
            }
            else
            {
                QuestTrackerUI tracker = FindFirstObjectByType<QuestTrackerUI>(FindObjectsInactive.Include);
                if (tracker != null) tracker.HideTracker();
            }
        }

        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPC npc in allNPCs)
        {
            npc.UpdateOutlineColor();
        }

        QuestUI ui = FindAnyObjectByType<QuestUI>(FindObjectsInactive.Include);
        if (ui != null) ui.RefreshQuestList();

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CheckAndAutoAcceptQuests();
        }
    }

    void OnEnable()
    {
        InventoryManager.OnInventoryChanged += RefreshStatus;
    }

    void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= RefreshStatus;
    }

    void RefreshStatus()
    {
        if (startQuests != null)
        {
            foreach (var q in startQuests) SyncQuestObjectives(q);
        }
        if (completeQuests != null)
        {
            foreach (var q in completeQuests) SyncQuestObjectives(q);
        }
        UpdateOutlineColor();
    }
}
