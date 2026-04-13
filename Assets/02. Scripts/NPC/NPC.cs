using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public Quest myQuest;
    private int dialogueIndex = 0;

    public void Interact(PlayerStat player)
    {
        if(myQuest == null) return;

        //퀘스트 받기 전
        if(!QuestManager.Instance.activeQuests.Contains(myQuest) && !myQuest.isCompleted)
        {
            ShowDialogue(myQuest.beforeAcceptDialogues);
            QuestManager.Instance.AcceptQuest(myQuest);
        }
        //퀘스트 진행중
        else if(QuestManager.Instance.activeQuests.Contains(myQuest) && !myQuest.isCompleted)
        {
            if(myQuest.currentAmount >= myQuest.goalAmount)
            {
                ShowDialogue(myQuest.completeDialogues);
                CompleteQuest();
            }
            else
            {
                ShowDialogue(myQuest.duringAcceptDialogues);
            }
        }
    }

    void ShowDialogue(string[] dialogues)
    {
        foreach(string line in dialogues)
        {
            Debug.Log($"{gameObject.name} : {line}");
        }
    }

    void CompleteQuest()
    {
        myQuest.isCompleted = true;
        QuestManager.Instance.activeQuests.Remove(myQuest);
        Debug.Log("보상 지급 로직 실행");
    }
}
