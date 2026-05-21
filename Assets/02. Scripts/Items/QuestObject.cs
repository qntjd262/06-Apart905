using UnityEngine;

public class QuestObject : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    public void Interact(PlayerStat player) // 매개변수 타입 변경
    {
        if (itemData == null) return;

        if (InventoryManager.Instance.AddItem(itemData))
        {
            Debug.Log($"{itemData.Name}을(를) 가방에 넣었습니다.");
            if (itemData.itemType == ItemType.Quest)
            {
                if (itemData is QuestItemData questItemData)
                {
                    //TODO : 퀘스트 아이템을 획득했을 때 퀘스트 매니저에 전달 및 진행상황 업데이트
                }
                else
                {
                    Debug.Log($"{itemData.Name}의 타입은 quest타입이지만 questitemdata의 설정이 되어있지 않음 ");
                }
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }

    public string GetInteractText()
    {
        return "획득";
    }

    public Constants.InteractType GetInteractType()
    {
        return Constants.InteractType.Pickup;
    }
}
