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
        if(targetQuest == null) return;

        if(targetQuest.type == QuestType.ItemCollection)
        {
            questPrograssText.text = $"{targetQuest.targetID} ({targetQuest.currentAmount}/{targetQuest.goalAmount})";
            
        }
    }


}
