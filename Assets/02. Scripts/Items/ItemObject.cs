using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.itemName}/n{data.description}"; //띄울 정보(이름, 설명)
        return str;
    }

    public void OnInteract()
    {
        
    }
}
