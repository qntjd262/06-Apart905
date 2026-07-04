using UnityEngine;
using TMPro;
using System.Collections;

public enum NotificationType
{
    ItemGet,
    QuestComplete
}

public class NotificationItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText; 
    [SerializeField] private CanvasGroup canvasGroup; 

    [SerializeField] private float duration = 2.0f;     // 화면에 머무는 시간
    [SerializeField] private float fadeSpeed = 1.0f;    

    public void Setup(string itemName, int count, NotificationType type = NotificationType.ItemGet)
    {
        if (itemText != null)
        {
            if(type == NotificationType.ItemGet)
            {
                itemText.text = $"{itemName}을(를) 획득하였습니다.";
            }
            else if(type == NotificationType.QuestComplete)
            {
                itemText.text = $"<color=#00FF00>[퀘스트 완료]</color> {itemName}";
            }
        }
        else
        {
            Debug.LogError("[알림 에러] itemText 컴포넌트가 인스펙터에서 연결되지 않았습니다!");
        }
        
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;

        StartCoroutine(NotificationRoutine());
    }

    private IEnumerator NotificationRoutine()
    {
        yield return new WaitForSeconds(duration);
        
        while (canvasGroup != null && canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
