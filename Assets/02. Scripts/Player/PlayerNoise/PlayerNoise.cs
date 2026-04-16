using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerNoise : MonoBehaviour
{
    [SerializeField] private NoiseVisual ripplePrefab;
    [SerializeField] private Transform groundPoint;

    [SerializeField] private LayerMask monsterLayer;

    private IObjectPool<NoiseVisual> ripplePool;


    private float currentRadius = 0f;
    private Coroutine pulseCoroutine;
    private float noiseDuration = 1f;
    private float noiseInterval = 1f;

    void Awake()
    {
        ripplePool = new ObjectPool<NoiseVisual>(
            createFunc : CreateRipple,
            actionOnGet : OnGetRipple,
            actionOnRelease : OnReleaseRipple,
            actionOnDestroy : OnDestroyRipple,
            collectionCheck : false,
            defaultCapacity : 5,
            maxSize : 10
        );
    }

    void OnEnable()
    {
        pulseCoroutine = StartCoroutine(RipplePulseRoutine());
    }

    void OnDisable()
    {
        if(pulseCoroutine != null) StopCoroutine(pulseCoroutine);
    }

    private NoiseVisual CreateRipple()
    {
        return Instantiate(ripplePrefab, groundPoint.position, Quaternion.Euler(90,0,0));
    }
    private void OnGetRipple(NoiseVisual ripple){ripple.gameObject.SetActive(true);}
    private void OnReleaseRipple(NoiseVisual ripple){ripple.gameObject.SetActive(false);}
    private void OnDestroyRipple(NoiseVisual ripple){Destroy(ripple.gameObject);}

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
          
        NoiseVisual ripple = ripplePool.Get();
        ripple.PlayRipple(radius,noiseDuration,groundPoint,ripplePool);

        AlertMonsterInRadius(radius);
    }

    //몬스터 감지 로직
    private void AlertMonsterInRadius(float radius)
    {
        Collider[] hitMonsters = Physics.OverlapSphere(groundPoint.position, radius, monsterLayer);

        foreach (Collider hit in hitMonsters)
        {
            MonsterController monster = hit.GetComponent<MonsterController>();
            if(monster != null)
            {
                monster.CanHearPlayerSound(groundPoint.position);
                Debug.Log($"{monster.gameObject.name}소리 들음");
            }
        }
    }
}
