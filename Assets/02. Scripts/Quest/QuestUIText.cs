using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class QuestUIText : MonoBehaviour
{
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questPrograssText;
    private Quest targetQuest;

    public void Setup(Quest quest)
    {
        targetQuest = quest;
        questNameText.text = quest.questName;
        UpdatePrograssUI();
    }

    public void UpdatePrograssUI()
    {
        if (targetQuest == null) return;

        if (targetQuest.type == QuestType.ItemCollection)
        {
            string progressDisplay = "";

            for (int i = 0; i < targetQuest.objectives.Count; i++)
            {
                var obj = targetQuest.objectives[i];
                
                progressDisplay += $"{obj.targetID} ({obj.currentAmount}/{obj.goalAmount})";

                if (i < targetQuest.objectives.Count - 1)
                {
                    progressDisplay += "\n";
                }
            }

            questPrograssText.text = progressDisplay;
        }
        else if (targetQuest.type == QuestType.Interact)
        {
            if (targetQuest.isCompleted)
                questPrograssText.text = "완료됨";
            else if (targetQuest.IsAllObjectivesComplete())
                questPrograssText.text = "NPC에게 보고하기";
            else
                questPrograssText.text = targetQuest.questGoal;
        }
    }
}
