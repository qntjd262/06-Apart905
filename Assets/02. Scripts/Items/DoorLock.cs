using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [Header ("잠금 상태 설정")]
    public bool isLocked = true;
     //열쇠의 이름과 일치하는 변수명이 들어가야함
    public string requiredKeyName;

   


    public bool TryUnlock()
    {
        if (!isLocked)
        {
            return true;
        }

        if(InventoryManager.Instance.GetItemCount(requiredKeyName) > 0)
        {
            InventoryManager.Instance.RemoveItem(requiredKeyName, 1);
            isLocked = false;

            Debug.Log("문이 열렸습니다");
            return true;
        }
        else
        {
            Debug.Log("해당 열쇠가 없습니다");
            return false;
        }
    }
}
