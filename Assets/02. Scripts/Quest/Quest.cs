using UnityEngine;

public enum QuestType { ItemCollection, ZombieHunt }

[CreateAssetMenu(fileName = "New Quest", menuName = "QuestSystem/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public QuestType type;
    public string targetID; //필요 아이템의 이름
    public int goalAmount; //필요 갯수
    public int currentAmount;
    public bool isMainQuest;//UI용 추가

    [Header("Reward")]
    public ItemData rewardItemID; //보상 아이템의 이름

    [Header("Dialogues")]
    [TextArea] public string[] beforeAcceptDialogues;
    [TextArea] public string[] duringAcceptDialogues;
    [TextArea] public string[] completeDialogues;

    public bool isCompleted;

    public void UpdateProgress(string id, int amount)
    {
        if(isCompleted) return;

        if(targetID == id)
        {
            currentAmount += amount;
            if(currentAmount >= goalAmount)
            {
                currentAmount = goalAmount;
                Debug.Log($"{questName} 목표 달성");
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
    }
}
}
