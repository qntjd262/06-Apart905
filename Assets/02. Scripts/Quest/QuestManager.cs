using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> activeQuests = new List<Quest>();

    void Awake() => Instance = this;

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
}
