using System.Collections.Generic;
using UnityEngine;

public enum QuestType { ItemCollection, ZombieHunt, Interact}

[CreateAssetMenu(fileName = "New Quest", menuName = "QuestSystem/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public Quest prevQuest;

    [System.Serializable]
    public struct QuestObjective
    {
    public string targetID; //필요 아이템의 이름 | type이 Interact일 시 상호작용 npc ID
    public int goalAmount;     //필요 갯수
    public int currentAmount;  //현재 갯수
    }

    [TextArea(2, 5)]
    public string questDiscrip; //퀘스트 설명
    public string questGoal; //목표 설명
    public QuestType type;
    public bool isMainQuest;//UI용 추가

    [Header("Quest Objectives")]
    public List<QuestObjective> objectives = new List<QuestObjective>();

    [Header("Quest Options")]
    public bool isAutoAccept;
    public bool isAutoComplete;

    [Header("Reward")]
    public ItemData rewardItemID; //보상 아이템의 이름

    [Header("Dialogues")]
    [TextArea] public string[] beforeAcceptDialogues;
    [TextArea] public string[] duringAcceptDialogues;
    [TextArea] public string[] completeDialogues;

    public bool isCompleted;

    public bool IsAllObjectivesComplete()
        {
            foreach (var obj in objectives)
            {
                if (obj.currentAmount < obj.goalAmount)
                    return false;
            }
            return true;
        }

    public void UpdateProgress(QuestType eventType, string id, int amount)
    {
        if (isCompleted) return;

        if (this.type == eventType)
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                if (objectives[i].targetID == id)
                {
                    var obj = objectives[i];
                    obj.currentAmount += amount;
                    if (obj.currentAmount >= obj.goalAmount) obj.currentAmount = obj.goalAmount;
                    
                    objectives[i] = obj;
                    Debug.Log($"{questName} - {id} 진행도: {obj.currentAmount}/{obj.goalAmount}");
                }
            }

            if (IsAllObjectivesComplete())
            {
                Debug.Log($"{questName} 모든 목표 달성!");
                if (isAutoComplete)
                {
                    QuestManager.Instance.CompleteQuestInstantly(this);
                }
            }
        }
    }

    // 인벤토리 수량 동기화용 함수
    public void ForceSyncProgress(string id, int amount)
    {
        if (isCompleted) return;

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].targetID == id)
            {
                var obj = objectives[i];
                obj.currentAmount = amount;
                if (obj.currentAmount >= obj.goalAmount) obj.currentAmount = obj.goalAmount;
                
                objectives[i] = obj;
            }
        }

        if (IsAllObjectivesComplete() && isAutoComplete && QuestManager.Instance.activeQuests.Contains(this))
        {
            QuestManager.Instance.CompleteQuestInstantly(this);
        }
    }
}
