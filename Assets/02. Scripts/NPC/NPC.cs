using UnityEditor.PackageManager;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public NPCdata npcData;

    public Quest myQuest;
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
        if(myRenderer == null || myQuest == null) return;

        myRenderer.GetPropertyBlock(PropertyBlock);
        Color targetColor = Color.red;
        string status = "";

        if(myQuest.isCompleted)
        {
            targetColor = Color.red;
            status = "완료";
        }
        else
        {
            if(!QuestManager.Instance.IsQuestAvailable(myQuest) && !QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName))
            {
                targetColor = Color.red;
                status = "선행 퀘스트 미완료(잠김)";
            }
            else
            {
                bool isActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

                if(isActive)
                {
                    if(myQuest.currentAmount >= myQuest.goalAmount)
                    {
                        targetColor = Color.green;
                        status = "목표달성";
                    }
                    else
                    {
                        targetColor = Color.red;
                        status = "목표치 부족";
                    }
                }
                else
                {
                    targetColor = Color.green;
                    status = "수락 전(보유중)";
                }
            }
        }
        Debug.Log($"{gameObject.name}의 퀘스트 상태: {status} / 설정 색상: {targetColor}");

        PropertyBlock.SetColor("_Outline_color", targetColor);
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
        if(myQuest != null && myQuest.isCompleted)
        {
            Debug.Log($"{gameObject.name}: 이미 완료된 퀘스트입니다. ");
            return;
        }

        if (myQuest != null && !QuestManager.Instance.IsQuestAvailable(myQuest) && !QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName))
        {
            Debug.Log($"{gameObject.name}: 선행 퀘스트를 완료하지 않아 퀘스트를 줄 수 없습니다.");
            return; 
        }

        if(npcData != null && NPCUIHandler.Instance != null)
        {
            NPCUIHandler.Instance.ShowNPC(npcData);
        }

        if (player.isInteracting || myQuest == null) return;

        if (myQuest.type == QuestType.ItemCollection)
        {
            int currentBagCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
            myQuest.ForceSyncProgress(currentBagCount);
        }
        else if (myQuest.type == QuestType.Interact && QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName))
        {
            if (npcData != null && myQuest.targetID == npcData.NpcID)
            {
                myQuest.ForceSyncProgress(myQuest.goalAmount);
            }
        }

        string[] currentDialogues;
        bool isAlreadyActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

        if (!isAlreadyActive && !myQuest.isCompleted)
        {
            currentDialogues = myQuest.beforeAcceptDialogues;
        }
        else if (myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
        {
            currentDialogues = myQuest.completeDialogues;
        }
        else
        {
            currentDialogues = myQuest.duringAcceptDialogues;
        }

        player.isInteracting = true; 
        
        DialogueManager.Instance.StartDialogue(npcData, currentDialogues, () => {

            if (myQuest.type == QuestType.ItemCollection)
            {
                int finalCheckCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
                myQuest.ForceSyncProgress(finalCheckCount);
            }

            bool stillActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

            if (!stillActive && !myQuest.isCompleted)
            {
                QuestManager.Instance.AcceptQuest(myQuest, player);
                
                if (myQuest.type == QuestType.Interact && npcData != null && myQuest.targetID == npcData.NpcID)
                {
                    myQuest.ForceSyncProgress(myQuest.goalAmount);
                }
            }
            else
            {
                if(myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
                {
                    CompleteQuest();         
                }
                player.isInteracting = false; 
                if(DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.EndDialogue();
                }
            }
            UpdateOutlineColor();
        });
    }

    void CompleteQuest()
    {
        myQuest.isCompleted = true;

        if(QuestManager.Instance != null && !QuestManager.Instance.completedQuestNames.Contains(myQuest.questName))
        {
            QuestManager.Instance.completedQuestNames.Add(myQuest.questName);
        }

        if (myQuest.type == QuestType.ItemCollection)
        {
            InventoryManager.Instance.RemoveItem(myQuest.targetID, myQuest.goalAmount);
        }

        if (myQuest.rewardItemID != null)
        {
            bool isSuccess = InventoryManager.Instance.AddItem(myQuest.rewardItemID);
            Debug.Log($"{myQuest.rewardItemID.itemName}를 획득하였습니다!");
        }

        QuestManager.Instance.activeQuests.RemoveAll(q => q.questName == myQuest.questName);

        if (QuestManager.Instance.trackingQuest != null && QuestManager.Instance.trackingQuest.questName == myQuest.questName)
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
                if (tracker != null)
                {
                    tracker.HideTracker();
                }
            }
        }

        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NPC npc in allNPCs)
        {
            npc.UpdateOutlineColor();
        }

        QuestUI ui = FindAnyObjectByType<QuestUI>(FindObjectsInactive.Include);
        if (ui != null) ui.RefreshQuestList();
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
        if(myQuest != null && myQuest.type == QuestType.ItemCollection)
        {
            int count = InventoryManager.Instance.GetItemCount(myQuest.targetID);
            myQuest.ForceSyncProgress(count);
            UpdateOutlineColor();
        }
    }
}
