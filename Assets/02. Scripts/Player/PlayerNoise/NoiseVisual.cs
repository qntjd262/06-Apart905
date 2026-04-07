using DG.Tweening;
using UnityEngine;

public class NoiseVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    

    public void PlayRipple(float targetRadius, float duration, Transform target)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.SetParent(target, false);

        transform.localPosition = Vector3.up * 0.05f;

        transform.localScale = Vector3.zero;
        Color c = spriteRenderer.color;
        c.a = 0.5f;
        spriteRenderer.color = c;

        float targetScale = targetRadius * 2f;
        transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad);

        spriteRenderer.DOFade(0f, duration).SetEase(Ease.OutQuad).OnComplete(() => Destroy(gameObject));
    }

    
}
