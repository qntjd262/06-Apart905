using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPWarningUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Image warningOverlay;

    [Header("Settings")]
    [SerializeField] private Color damageColor = new Color(0.4f, 0.0f, 0.0f, 0f); 

    private Tween blinkTween;

    private void Start()
    {
        warningOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);

        if (playerStat != null)
        {
            playerStat.OnHpChanged += UpdateWarningEffect;
        }
    }

    private void UpdateWarningEffect(float currentHp, float maxHp)
    {
        float hpPercent = currentHp / maxHp;

        blinkTween?.Kill();
        warningOverlay.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f);

        if (hpPercent > 0.99f) return;

        float targetAlpha = 0f;
        float blinkDuration = 0f;
        float overlayScale = 1f;
        
        // 반복 횟수 제어 변수
        // 2 = 왕복 1회 (한 번 깜빡이고 사라짐), -1 = 무한 반복
        int loopCount = 2; 

        if (hpPercent <= 0.20f) // 심하게 닳았을 때 (20% 이하)
        {
            targetAlpha = 0.8f;     
            blinkDuration = 0.4f;   // 계속 깜빡일 때는 속도를 살짝 늦춰서 눈 피로도 감소
            overlayScale = 1.0f;    
            loopCount = -1;         // 계속 깜빡임 (무한 루프)
        }
        else if (hpPercent <= 0.50f)
        {
            targetAlpha = 0.6f;
            blinkDuration = 0.4f;   
            overlayScale = 1.1f;    
            loopCount = -1;         
        }
        else if (hpPercent <= 0.80f)
        {
            targetAlpha = 0.4f;
            blinkDuration = 0.2f;
            overlayScale = 1.2f;    
            loopCount = 2;
        }
        else // 99% 이하
        {
            targetAlpha = 0.2f;
            blinkDuration = 0.2f;
            overlayScale = 1.3f;    
            loopCount = 2;
        }

        warningOverlay.rectTransform.localScale = Vector3.one * overlayScale;

        blinkTween = warningOverlay.DOFade(targetAlpha, blinkDuration)
            .SetLoops(loopCount, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        if (playerStat != null)
        {
            playerStat.OnHpChanged -= UpdateWarningEffect;
        }
        blinkTween?.Kill();
    }
}