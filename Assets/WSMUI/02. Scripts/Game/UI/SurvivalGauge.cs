using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SurvivalGauge : MonoBehaviour
{
    [Header("Circular Gauges (Ring & Center)")]
    [SerializeField] private Image hungerRing;
    [SerializeField] private Image thirstRing;
    [SerializeField] private Image sanityGauge;

    [Header("Stamina Slider")]
    [SerializeField] private Slider staminaSlider;

    [Header("Tween Settings")]
    [SerializeField] private float duration = 0.3f;

    // 색상 깜빡임 트윈을 추적하기 위한 변수
    private Tween sanityColorTween;

    public void UpdateHunger(float current, float max)
    {
        float ratio = max > 0 ? current / max : 0f;
        UpdateFillAmount(hungerRing, ratio * 0.5f);
    }

    public void UpdateThirst(float current, float max)
    {
        float ratio = max > 0 ? current / max : 0f;
        UpdateFillAmount(thirstRing, ratio * 0.5f);
    }

    public void UpdateSanity(float current, float max)
    {
        float ratio = max > 0 ? current / max : 0f;
        UpdateFillAmount(sanityGauge, ratio);

        if (ratio > 0.8f)
        {
            // 이미 실행 중인 색상 트윈이 없다면 새로 실행
            if (sanityColorTween == null || !sanityColorTween.IsActive())
            {
                sanityColorTween = sanityGauge.DOColor(Color.red, 0.2f).SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            // 수치가 내려가면 색상 트윈만 선택적으로 종료
            if (sanityColorTween != null)
            {
                sanityColorTween.Kill();
                sanityColorTween = null;
                sanityGauge.color = Color.magenta;
            }
        }
    }

    public void UpdateStamina(float current, float max, bool isSmooth = true)
    {
        if (staminaSlider == null) return;

        float targetValue = max > 0 ? current / max : 0f;;
        
        // 슬라이더의 Value를 제어하는 트윈만 종료
        staminaSlider.DOKill();

        if (isSmooth)
        {
            staminaSlider.DOValue(targetValue, duration).SetEase(Ease.OutQuad);
        }
        else
        {
            staminaSlider.value = targetValue;
        }
    }

    private void UpdateFillAmount(Image img, float target)
    {
        if (img == null) return;
        
        // 이미지의 모든 트윈을 끄는 대신, FillAmount 트윈만 덮어씌우도록 처리
        img.DOFillAmount(target, duration).SetEase(Ease.OutQuad);
    }
}