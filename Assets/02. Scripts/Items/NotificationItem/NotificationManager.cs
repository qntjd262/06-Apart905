using UnityEngine;
using UnityEngine.SceneManagement;

public class NotificationManager : Singleton<NotificationManager>
{
    [Header("UI References")]
    [SerializeField] private Transform container;      
    [SerializeField] private GameObject itemPrefab;    

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
    protected override void OnSceneUnloaded(Scene scene) { }

    public void ShowNotification(string itemName, int count = 1)
    {
        if (container == null || itemPrefab == null) return;

        GameObject popUp = Instantiate(itemPrefab, container);
        popUp.transform.SetAsLastSibling();

        NotificationItem notificationScript = popUp.GetComponent<NotificationItem>();
        
        if (notificationScript != null)
        {
            notificationScript.Setup(itemName, count); 
        }
        else
        {
            Debug.LogError("생성된 프리팹에서 'NotificationItem' 스크립트를 찾을 수 없습니다! 프리팹을 확인하세요.");
        }
    }
}
