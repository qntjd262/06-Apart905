using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private string[] monsterNames;

    // 랜덤 스폰 여부 및 고정 스폰 시 스폰할 몬스터 종류
    [SerializeField] private bool isRandomSpawn = true;
    [SerializeField] private MonsterType selectedMonster;

    // 스폰한 몬스터를 저장
    private GameObject spawnedMonster;

    private MonsterController monsterController;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private void OnEnable()
    {
        if (spawnedMonster != null)
        {
            navMeshAgent.isStopped = false;
            spawnedMonster.SetActive(true);
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
        spawnedMonster = monster;

        // 스폰한 몬스터 사망시 실행할 액션 등록
        monsterController.OnDied += ResetSpawner;
    }

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
