using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    private GameObject spawnedMonster;              // 스폰한 몬스터를 저장
    private MonsterController monsterController;
    private NavMeshAgent navMeshAgent;
    [field:SerializeField]
    public int CurrFloor { get; set; }              // 현재 스포너가 위치한 층

    [Header("몬스터 이름")]
    [SerializeField] private string[] monsterNames;

    [Header("랜덤스폰 여부")]
    [SerializeField] private bool isRandomSpawn = true;

    [Header("고정스폰 시 스폰할 몬스터 종류")]
    [SerializeField] private MonsterType selectedMonster;

    private void Start()
    {
        StartCoroutine(Spawn());
        string parentName = transform.parent.name;
        CurrFloor = int.Parse(parentName.Replace("F", ""));
    }

    private void OnEnable()
    {
        if (spawnedMonster != null)
        {
            spawnedMonster.SetActive(true);
            navMeshAgent.isStopped = false;
        }
    }

    private void OnDisable()
    {
        if (spawnedMonster != null)
        {
            navMeshAgent.isStopped = true;
            spawnedMonster.SetActive(false);
        }
    }

    #region 스폰 타이밍
    // 스크립트 실행 순서 문제 해결을 위해 한프레임 늦게 실행
    IEnumerator Spawn()
    {
        yield return null;
        SpawnRandomMonster();
    }

    public void SpawnRandomMonster()
    {
        MonsterType targetMonster;
        // 랜덤 스폰일 때와 아닐 때
        if (isRandomSpawn)
        {
            int random = Random.Range(0, MonsterSpawnManager.Instance.Monsters.Length);
            targetMonster = (MonsterType)random;
        }
        else
        {
            targetMonster = selectedMonster;
        }

        GameObject monster = MonsterSpawnManager.Instance.GetPoolObject(targetMonster);

        // 몬스터 설정 초기화
        MonsterReset(monster);

        monsterController.OnDied += ResetSpawner;   // 스폰한 몬스터 사망시 실행할 액션 등록
        monsterController.Spawner = this;           // 스폰한 몬스터의 스포너로 자신을 지정
    }
    #endregion

    private void MonsterReset(GameObject monster)
    {
        // 몬스터 설정 초기화
        monsterController = monster.GetComponent<MonsterController>();
        if (monsterController == null)
        {
            Debug.Log("MonsterController 없음");
        }
        monster.GetComponent<MonsterController>().ResetMonster();

        // NavMesh 초기화
        navMeshAgent = monster.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position);
            navMeshAgent.ResetPath();
            monster.transform.rotation = transform.rotation;
        }

        spawnedMonster = monster;
    }

    private void ResetSpawner()
    {
        // 사망 액션 등록 해지
        if (monsterController != null)
        {
            monsterController.OnDied -= ResetSpawner;
        }

        spawnedMonster = null;
        monsterController = null;
        navMeshAgent = null;
    }
}
