using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public Quest myQuest;
    private int dialogueIndex = 0;

    public void Interact(PlayerStat player)
    {
        if (player.isInteracting || myQuest == null) return;

        //현재 인벤토리의 사과 개수를 퀘스트 데이터에 동기화
        int currentBagCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
        myQuest.ForceSyncProgress(currentBagCount); 
        // --------------------------------------------------------------------------

        string[] currentDialogues;

        // 상태에 따른 대사 결정 (이미 수치가 동기화되었으므로 정확한 대사가 나옵니다)
        if (!QuestManager.Instance.activeQuests.Contains(myQuest) && !myQuest.isCompleted)
            currentDialogues = myQuest.beforeAcceptDialogues;
        else if (myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
            currentDialogues = myQuest.completeDialogues;
        else
            currentDialogues = myQuest.duringAcceptDialogues;

        player.isInteracting = true; 
        
        DialogueManager.Instance.StartDialogue(currentDialogues, () => {
            player.isInteracting = false; 

            //대화가 끝나는 시점에 다시 한번 인벤토리 체크
            int finalCheckCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
            myQuest.ForceSyncProgress(finalCheckCount);
            // --------------------------------------------------------------------------
            
            if (!QuestManager.Instance.activeQuests.Contains(myQuest) && !myQuest.isCompleted)
            {
                QuestManager.Instance.AcceptQuest(myQuest);
            }
            else if (myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
            {
                CompleteQuest();
            }
        });
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
        if (myQuest.type == QuestType.ItemCollection)
        {
            InventoryManager.Instance.RemoveItem(myQuest.targetID, myQuest.goalAmount);
        }
        QuestManager.Instance.activeQuests.Remove(myQuest);
    }
}
