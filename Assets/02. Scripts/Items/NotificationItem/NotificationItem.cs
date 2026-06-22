using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText; 
    [SerializeField] private CanvasGroup canvasGroup; 

    [SerializeField] private float duration = 2.0f;     // 화면에 머무는 시간
    [SerializeField] private float fadeSpeed = 1.0f;    // ★ 1.0으로 낮춰서 스르륵 사라지는 게 눈에 보이도록 수정

    public void Setup(string itemName, int count)
    {
        if (itemText != null)
        {
            itemText.text = $"{itemName}을(를) 획득하였습니다.";
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
