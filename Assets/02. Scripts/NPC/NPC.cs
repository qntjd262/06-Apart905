using UnityEditor.PackageManager;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public NPCdata npcData;

    public Quest myQuest;
    private Renderer myRenderer;
    private MaterialPropertyBlock PropertyBlock;

    void Awake()
    {
        myRenderer = GetComponentInChildren<Renderer>();
        PropertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        UpdateOutlineColor();
    }

    public void UpdateOutlineColor()
    {
        if(myRenderer == null || myQuest == null) return;

        myRenderer.GetPropertyBlock(PropertyBlock);
        Color targetColor = Color.red;
        string status = "";

        //퀘스트 완료 시
        if(myQuest.isCompleted)
        {
            targetColor = Color.red;
            status = "완료";
        }
        else
        {
            bool isActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

            if(isActive)
            {
                //목표 아이템을 다 모았나?
                if(myQuest.currentAmount >= myQuest.goalAmount)
                {
                    targetColor = Color.green;
                    status = "목표달성";
                }
                else
                {
                    targetColor = Color.red;
                    status = "목표치 부족";
                }
            }
            else
            {
                //퀘스트가 수락 가능한 상태인가?
                targetColor = Color.green;
                status = "수락 전(보유중)";
            }
        }
        Debug.Log($"{gameObject.name}의 퀘스트 상태: {status} / 설정 색상: {targetColor}");

        PropertyBlock.SetColor("_Outline_color", targetColor);
        myRenderer.SetPropertyBlock(PropertyBlock);
    }

    public void Interact(PlayerStat player)
    {
        if(myQuest != null && myQuest.isCompleted)
        {
            Debug.Log($"{gameObject.name}: 이미 완료된 퀘스트입니다. ");
            return;
        }

        if(npcData != null && NPCUIHandler.Instance != null)
        {
            NPCUIHandler.Instance.ShowNPC(npcData);
        }

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
            //player.isInteracting = false; 

            int finalCheckCount = InventoryManager.Instance.GetItemCount(myQuest.targetID);
            myQuest.ForceSyncProgress(finalCheckCount);
            
            // 다시 한번 현재 리스트에 있는지 확인
            bool stillActive = QuestManager.Instance.activeQuests.Exists(q => q.questName == myQuest.questName);

            if (!stillActive && !myQuest.isCompleted)
            {
                // 퀘스트 수락 (이 안에서 UI 새로고침이 호출되어야 합니다)
                QuestManager.Instance.AcceptQuest(myQuest, player);
            }
            else
            {
                if(myQuest.currentAmount >= myQuest.goalAmount && !myQuest.isCompleted)
                {
                    // 퀘스트 완료 처리
                    CompleteQuest();         
                }
                player.isInteracting = false; 
                if(DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.EndDialogue();
                }
            }
                UpdateOutlineColor();
        });
    }

    void CompleteQuest()
    {
        myQuest.isCompleted = true;

        //퀘스트 완료 시 목표 아이템 제거
        if (myQuest.type == QuestType.ItemCollection)
        {
            InventoryManager.Instance.RemoveItem(myQuest.targetID, myQuest.goalAmount);
        }

        //보상 아이템 지급
        if (myQuest.rewardItemID != null)
            {
                bool isSuccess = InventoryManager.Instance.AddItem(myQuest.rewardItemID);
                Debug.Log($"{myQuest.rewardItemID.itemName}를 획득하였습니다!");
            }

        QuestManager.Instance.activeQuests.RemoveAll(q => q.questName == myQuest.questName);

        QuestUI ui = FindAnyObjectByType<QuestUI>(FindObjectsInactive.Include);
        if (ui != null) ui.RefreshQuestList();
    }

    void OnEnable()
    {
        InventoryManager.OnInventoryChanged += RefreshStatus;
    }

    void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= RefreshStatus;
    }

    void RefreshStatus()
    {
        if(myQuest != null)
        {
            int count = InventoryManager.Instance.GetItemCount(myQuest.targetID);
            myQuest.ForceSyncProgress(count);
            UpdateOutlineColor();
        }
    }
}