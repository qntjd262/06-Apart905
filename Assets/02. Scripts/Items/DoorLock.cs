using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [Header ("잠금 상태 설정")]
    public bool isLocked = true;
     //열쇠의 이름과 일치하는 변수명이 들어가야함
    public int doorNum;

   


    public bool TryUnlock()
    {
        if (!isLocked) return true;

        string requiredKeyName = FindKeyNameByDoorNum(doorNum);

        if(string.IsNullOrEmpty(requiredKeyName)) return false;
        

        if(InventoryManager.Instance.GetItemCount(requiredKeyName) > 0)
        {
            InventoryManager.Instance.RemoveItem(requiredKeyName, 1);
            isLocked = false;

            Debug.Log("문이 열렸습니다");
            return true;
        }
        else
        {
            Debug.Log($"{doorNum}호 열쇠가 필요합니다.");
            return false;
        }
    }

    private string FindKeyNameByDoorNum(int targetNum)
    {
        foreach(ItemData item in ItemDataManager.Instance.allItemList)
        {
            if(item is KeyItemData keyData && keyData.targetDoorNum == targetNum)
            {
                return keyData.Name;
            }
        }

        return null;
    }
}
