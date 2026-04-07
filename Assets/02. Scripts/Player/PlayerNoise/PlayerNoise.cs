using System.Collections;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    [SerializeField] private GameObject ripplePrefab;
    [SerializeField] private Transform groundPoint;


    private float currentRadius = 0f;
    private Coroutine pulseCoroutine;
    private float noiseDuration = 1f;
    private float noiseInterval = 1f;

    void OnEnable()
    {
        pulseCoroutine = StartCoroutine(RipplePulseRoutine());
    }

    void OnDisable()
    {
        if(pulseCoroutine != null) StopCoroutine(pulseCoroutine);
    }

    private IEnumerator RipplePulseRoutine()
    {
        while (true)
        {
            if(currentRadius > 0f)
            {
                SpawnRipple(currentRadius);
            }
            yield return new WaitForSeconds(noiseInterval);
        }
    }

    public void SetNoiseRadius(float radius)
    {
        currentRadius = radius;
    }

    //플레이어 한번의 행동 소음 생성
    public void TriggerOneShotNoise(float radius)
    {
        SpawnRipple(radius);
    }
    private void SpawnRipple(float radius)
    {
        if(ripplePrefab == null || groundPoint == null) return;
          
        GameObject ripple = Instantiate(ripplePrefab, groundPoint.position, Quaternion.Euler(90,0,0));
        ripple.GetComponent<NoiseVisual>().PlayRipple(radius, noiseDuration, groundPoint);
    }
}
