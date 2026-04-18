using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public Quest myQuest;

    public void Interact(PlayerStat player)
    {
        if (player.isInteracting || myQuest == null) return;

        // 현재 인벤토리 수량을 퀘스트 데이터에 동기화
        int currentBagCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
        myQuest.ForceSyncProgress(currentBagCount); 

        string[] currentDialogues;

        // [중요] Contains 대신 Exists를 사용하여 현재 퀘스트가 리스트에 있는지 확인
        bool isAlreadyActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

        // 상태에 따른 대사 결정
        if (!isAlreadyActive && !myQuest.isCompleted)
        {
            // 아직 수락하지 않은 상태
            currentDialogues = myQuest.beforeAcceptDialogues;
        }
        else if (myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
        {
            // 목표 달성, 완료 대기 상태
            currentDialogues = myQuest.completeDialogues;
        }
        else
        {
            // 이미 수락했으나 아직 진행 중인 상태
            currentDialogues = myQuest.duringAcceptDialogues;
        }

        player.isInteracting = true; 
        
        DialogueManager.Instance.StartDialogue(currentDialogues, () => {
            player.isInteracting = false; 

            int finalCheckCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
            myQuest.ForceSyncProgress(finalCheckCount);
            
            // 다시 한번 현재 리스트에 있는지 확인
            bool stillActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

            if (!stillActive && !myQuest.isCompleted)
            {
                // 퀘스트 수락 (이 안에서 UI 새로고침이 호출되어야 합니다)
                QuestManager.Instance.AcceptQuest(myQuest);
            }
            else if (myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
            {
                // 퀘스트 완료 처리
                CompleteQuest();
            }
        });
    }

    void CompleteQuest()
    {
        myQuest.isCompleted = true;
        if (myQuest.type == QuestType.ItemCollection)
        {
            InventoryManager.Instance.RemoveItem(myQuest.targetID, myQuest.goalAmount);
        }

        // [수정] 주소값이 아닌 '이름' 기반으로 리스트에서 제거
        QuestManager.Instance.activeQuests.RemoveAll(q => q.questName == myQuest.questName);

        // [추가] 퀘스트가 완료되어 사라졌으므로 UI를 새로고침합니다.
        QuestUI ui = FindObjectOfType<QuestUI>(true);
        if (ui != null) ui.RefreshQuestList();
    }
}