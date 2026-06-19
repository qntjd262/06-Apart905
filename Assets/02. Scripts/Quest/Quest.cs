using UnityEngine;

public enum QuestType { ItemCollection, ZombieHunt, Interact}

[CreateAssetMenu(fileName = "New Quest", menuName = "QuestSystem/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;

    public Quest prevQuest;

    [TextArea(2, 5)]
    public string questDiscrip; //퀘스트 설명
    
    public string questGoal; //목표 설명
    public QuestType type;
    public string targetID; //필요 아이템의 이름 | type이 Interact일 시 상호작용 npc ID
    public int goalAmount; //필요 갯수
    public int currentAmount;
    public bool isMainQuest;//UI용 추가

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

    public void UpdateProgress(QuestType eventType, string id, int amount)
    {
        if(isCompleted) return;

        if(this.type == eventType && targetID == id)
        {
            currentAmount += amount;
            if(currentAmount >= goalAmount)
            {
                currentAmount = goalAmount;
                Debug.Log($"{questName} 목표 달성");

                if(isAutoComplete)
                {
                    QuestManager.Instance.CompleteQuestInstantly(this);
                }
            }
        }
    }

    public void ForceSyncProgress(int amount)
{
    if (isCompleted) return;
    
    currentAmount = amount;
    if (currentAmount >= goalAmount)
    {
        currentAmount = goalAmount;

        if(isAutoComplete && QuestManager.Instance.activeQuests.Exists(q => q.questName == this.questName))
            {
                QuestManager.Instance.CompleteQuestInstantly(this);
            }
    }
}
}
