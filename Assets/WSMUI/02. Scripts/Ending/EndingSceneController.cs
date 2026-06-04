using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
// SceneManager 사용 안 함

public class EndingSceneController : MonoBehaviour
{
    [Header("UI 연결 (인스펙터에서 드래그)")]
    [SerializeField] private Image illustrationImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("설정")]
    [SerializeField, Tooltip("엔딩 화면을 자동으로 유지할 시간(초)")] 
    private float displayDuration = 5.0f;
    
    [SerializeField, Tooltip("돌아갈 메인 타이틀 씬 열거형")] 
    private Constants.ESceneType titleSceneType; 

    private bool isTransitioning = false; 

    void Start()
    {
        EndingData data = GameManager.Instance.selectedEnding;

        if (data != null)
        {
            descriptionText.text = data.endingDescription;
            
            if (data.endingIllustration != null)
            {
                illustrationImage.sprite = data.endingIllustration;
            }
        }
        else
        {
            Debug.LogError("표시할 엔딩 데이터가 없습니다!");
        }

        // 엔딩 씬에서는 커서를 무조건 풀어준다. (UIManager가 OnSceneLoaded에서 해주지만 명시적으로 한 번 더 방어)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UIManager.Instance.FadeIn(2.0f);
        StartCoroutine(WaitAndReturnToMain());
    }

    void Update()
    {
        if (!isTransitioning && Input.anyKeyDown)
        {
            isTransitioning = true;
            StopAllCoroutines();
            
            UIManager.Instance.LoadScene(titleSceneType, true); 
        }
    }

    private IEnumerator WaitAndReturnToMain()
    {
        yield return new WaitForSeconds(displayDuration);
        
        if (!isTransitioning)
        {
            isTransitioning = true;
            UIManager.Instance.LoadScene(titleSceneType, true); 
        }
    }
}