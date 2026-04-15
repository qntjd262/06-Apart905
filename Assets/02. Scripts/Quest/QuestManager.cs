using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> activeQuests = new List<Quest>();

    void Awake() => Instance = this;

    public void AcceptQuest(Quest newQuest)
    {
        if(activeQuests.Contains(newQuest))
        {
            Debug.Log("이미 진행 중인 퀘스트입니다.");
            return;
        }

        if(newQuest.type == QuestType.ItemCollection)
        {
            int alreadyHaveCount = InventoryManager.Instance.GetItemCount(newQuest.targetID);
            newQuest.currentAmount = alreadyHaveCount;
        }

        activeQuests.Add(newQuest);
        Debug.Log($"{newQuest.questName} 퀘스트를 수락했습니다. ");
    }

    public void NotifyEvent(QuestType type, string id, int amount)
    {
        foreach(var quest in activeQuests)
        {
            if(quest.type == type)
            {
                quest.UpdateProgress(id, amount);
            }
        }
    }

    public void OnPickUp()
    {
        
    }

    public void OnKill()
    {
        
    }
}
