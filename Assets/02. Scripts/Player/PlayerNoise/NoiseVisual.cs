using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class NoiseVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private IObjectPool<NoiseVisual> managedPool;
    

    public void PlayRipple(float targetRadius, float duration, Transform target,IObjectPool<NoiseVisual> pool)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        managedPool = pool;

        transform.SetParent(target, false);
        transform.localPosition = Vector3.up * 0.05f;
        transform.localRotation = Quaternion.Euler(90,0,0);

        transform.DOKill();
        spriteRenderer.DOKill();
        transform.localScale = Vector3.zero;

        Color c = spriteRenderer.color;
        c.a = 0.5f;
        spriteRenderer.color = c;

        float targetScale = targetRadius * 2f;
        transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad);

        spriteRenderer.DOFade(0f, duration).SetEase(Ease.OutQuad).OnComplete(() => {transform.SetParent(null); managedPool.Release(this);});
    }

    
}
