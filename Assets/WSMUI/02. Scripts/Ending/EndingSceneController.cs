using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingSceneController : MonoBehaviour
{
    [Header("UI 연결 (인스펙터에서 드래그)")]
    [SerializeField] private Image illustrationImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    void Start()
    {
        // 1. 전역 매니저에 저장된 엔딩 데이터가 있는지 확인
        // (이전 씬의 팝업에서 수락 버튼을 눌렀을 때 이미 여기에 저장됨)
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

        // 3. 연출 (UIManager를 이용한 부드러운 시작)
        UIManager.Instance.FadeIn(2.0f);
    }
}