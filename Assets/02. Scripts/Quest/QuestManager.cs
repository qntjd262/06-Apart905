using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public QuestUI questUI;
    public List<Quest> activeQuests = new List<Quest>();

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
        if(activeQuests.Exists(q => q.questName == newQuest.questName))
        {
            Debug.Log($"[{newQuest.questName}]은 이미 진행 중인 퀘스트입니다.");
            return;
        }

        if(newQuest.type == QuestType.ItemCollection)
        {
            if (InventoryManager.Instance != null)
            {
                int alreadyHaveCount = InventoryManager.Instance.GetItemCount(newQuest.targetID);
                newQuest.currentAmount = alreadyHaveCount;
            }
        }

        activeQuests.Add(newQuest);
        Debug.Log($"{newQuest.questName} 퀘스트를 수락했습니다.");

        QuestUI ui = FindObjectOfType<QuestUI>(true);
        if (ui != null)
        {
            ui.RefreshQuestList();
        }
        else
        {
            Debug.LogWarning("씬에서 QuestUI를 찾을 수 없습니다.");
        }
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

        if(questUI != null && questUI.gameObject.activeSelf)
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
