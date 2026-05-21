using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SurvivalGauge : MonoBehaviour
{
    [Header("Circular Gauges (Ring & Center)")]
    [SerializeField] private Image hungerRing;
    [SerializeField] private Image thirstRing;
    [SerializeField] private Image sanityGauge;

    [Header("Stamina Slider")]
    [SerializeField] private Slider staminaSlider;

    [Header("Text Displays (Optional)")]
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI thirstText;
    [SerializeField] private TextMeshProUGUI sanityText;

    [Header("Tween Settings")]
    [SerializeField] private float duration = 0.3f;

    [Header("Gauge Shape Settings")]
    [Tooltip("체크하면 반원 게이지(최대 50%)로 작동, 해제하면 일직선(최대 100%)으로 작동")]
    [SerializeField] private bool isHalfCircleGauge = false;

    private Tween sanityColorTween;

    // [핵심] 이전 타겟 값을 기억하기 위한 변수
    private float lastHunger = -1f;
    private float lastThirst = -1f;
    private float lastSanity = -1f;
    private float lastStamina = -1f;

    public void UpdateHunger(float current, float max, bool isSmooth = true)
    {
        if (hungerRing == null && hungerText == null) return;

        float targetRatio = max > 0 ? current / max : 0f;

        // 반원 체크 여부에 따라 0.5 또는 1.0을 곱한다
        float fillMultiplier = isHalfCircleGauge ? 0.5f : 1.0f;

        if (Mathf.Abs(lastHunger - targetRatio) > 0.001f || !isSmooth)
        {
            lastHunger = targetRatio;
            UpdateFillAmount(hungerRing, targetRatio * fillMultiplier, isSmooth);
        }

        if (hungerText != null)
            hungerText.text = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
    }

    public void UpdateThirst(float current, float max, bool isSmooth = true)
    {
        if (thirstRing == null && thirstText == null) return;

        float targetRatio = max > 0 ? current / max : 0f;

        float fillMultiplier = isHalfCircleGauge ? 0.5f : 1.0f;

        if (Mathf.Abs(lastThirst - targetRatio) > 0.001f || !isSmooth)
        {
            lastThirst = targetRatio;
            UpdateFillAmount(thirstRing, targetRatio * fillMultiplier, isSmooth);
        }

        if (thirstText != null)
            thirstText.text = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
    }
    public void UpdateSanity(float current, float max, bool isSmooth = true)
    {
        if (sanityGauge == null && sanityText == null) return;

        float targetRatio = max > 0 ? current / max : 0f;

        if (Mathf.Abs(lastSanity - targetRatio) > 0.001f || !isSmooth)
        {
            lastSanity = targetRatio;
            UpdateFillAmount(sanityGauge, targetRatio, isSmooth);

            if (sanityGauge != null)
            {
                if (targetRatio > 0.8f)
                {
                    if (sanityColorTween == null || !sanityColorTween.IsActive())
                        sanityColorTween = sanityGauge.DOColor(Color.red, 0.2f).SetLoops(-1, LoopType.Yoyo);
                }
                else
                {
                    if (sanityColorTween != null)
                    {
                        sanityColorTween.Kill();
                        sanityColorTween = null;
                        sanityGauge.color = Color.magenta;
                    }
                }
            }
        }

        if (sanityText != null)
            sanityText.text = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
    }

    public void UpdateStamina(float current, float max, bool isSmooth = true)
    {
        if (staminaSlider == null) return;

        float targetValue = max > 0 ? current / max : 0f;

        if (Mathf.Abs(lastStamina - targetValue) > 0.001f || !isSmooth)
        {
            lastStamina = targetValue;
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
    }

    private void UpdateFillAmount(Image img, float target, bool isSmooth)
    {
        if (img == null) return;

        img.DOKill();

        if (isSmooth)
        {
            img.DOFillAmount(target, duration).SetEase(Ease.OutQuad);
        }
        else
        {
            img.fillAmount = target;
        }
    }
}