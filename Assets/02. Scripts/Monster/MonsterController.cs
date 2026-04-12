using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController
    : MonoBehaviour
{
    public MonsterStatSO monsterStatSO;
    private Blackboard _blackBoard;
    private NavMeshAgent _navMeshAgent;

    [Header("���� ü��")]
    private float monsterHealth;
    [SerializeField] private float currentHealth;

    private void Awake()
    {
        monsterHealth = monsterStatSO.maxHP;
    }

    private void Start()
    {
        currentHealth = monsterHealth;
        _blackBoard = GetComponent<TestMonsterAI>().blackboard;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(float damage, GameObject player)
    {
        if (currentHealth < 0)
            return;

        currentHealth -= damage;
       
        Debug.Log($"{damage} ���� , ���� ü�� {currentHealth}");
        if (currentHealth <= 0)
        {
            Death();
        }

        _blackBoard.Player = player;
        _blackBoard.MonsterState = Blackboard.State.Attacked;
    }

    public void Death()
    {
        Debug.Log(_blackBoard.MonsterState);
        _blackBoard.MonsterState = Blackboard.State.Death;
        StartCoroutine(DeathAnim());
    }

    IEnumerator DeathAnim()
    {
        float deathAnimTime = 0f;
        float duration = 2.75f;

        while (deathAnimTime <= duration)
        {
            deathAnimTime += Time.deltaTime;
            yield return null;
        }

        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;

        GetComponent<TestMonsterAI>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }
}
