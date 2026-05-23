using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DOTweenFlicker : MonoBehaviour
{
    public Image brightImage;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;

    [Header("밝기 유지 설정")]
    public float brightStayDuration = 1.5f; // 밝은 상태가 유지될 시간
    [Range(0, 1)] public float brightThreshold = 0.8f; // 이 값보다 알파가 높으면 '밝음'으로 간주

    void Start()
    {
        if (brightImage != null)
        {
            StartFlicker();
        }
    }

    void StartFlicker()
    {
        // 1. 랜덤하게 투명도와 변화 시간을 결정
        float randomAlpha = Random.Range(minAlpha, maxAlpha);
        float duration = Random.Range(0.01f, 0.15f);

        // 2. 투명도 조절
        brightImage.DOFade(randomAlpha, duration)
            .OnComplete(() => {
                float delay = 0f;

                // [수정 포인트] 
                // 지금 투명도가 설정한 임계값(Threshold)보다 높다면(즉, 불이 밝게 들어왔다면)
                // 지정한 시간만큼 더 머무르게 설정
                if (randomAlpha >= brightThreshold)
                {
                    delay = brightStayDuration + Random.Range(-0.2f, 0.5f); // 약간의 랜덤성을 더해 자연스럽게
                }
                else
                {
                    // 불이 어두울 때는 가끔만 쉬어감 (지직거리는 느낌 유지)
                    delay = (Random.value > 0.9f) ? Random.Range(0.1f, 0.3f) : 0f;
                }

                DOVirtual.DelayedCall(delay, StartFlicker);
            });
    }

    void OnDestroy()
    {
        // 시퀀스나 개별 트윈 모두 정리
        if (brightImage != null)
            brightImage.DOKill();
    }
}