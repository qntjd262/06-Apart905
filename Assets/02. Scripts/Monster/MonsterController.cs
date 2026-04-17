using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider))]
public class MonsterController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Blackboard _blackBoard;
    public MonsterStatSO monsterStatSO;

    [Header("몬스터 체력")]
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

    #region 적 발소리 체크
    /// <summary>
    /// 플레이어의 발소리가 난 위치를 전달받아 처리
    /// </summary>
    /// <param name="soundOrigin"></param>
    public void CanHearPlayerSound(Vector3 soundOrigin)
    {
        if (_blackBoard.MonsterState != Blackboard.State.Idle &&
            _blackBoard.MonsterState != Blackboard.State.Patrol)
        {
            return;
        }

        var soundDirection = (soundOrigin - transform.position).normalized;

        // TODO : 플레이어의 Noise 범위 내에 있는지 확인
        // 만약, 범위 내에 있다면 true, 없다면 false
        _blackBoard.CanHearPlayer = true;
        _blackBoard.SoundDirection = soundDirection;
    }
    #endregion

    #region 피격 및 사망
    public void TakeDamage(float damage, GameObject player)
    {
        if (currentHealth < 0)
            return;

        currentHealth -= damage;
       
        _blackBoard.Player = player;
        _blackBoard.MonsterState = Blackboard.State.Attacked;

        Debug.Log($"{damage} 입음, 남은 체력 {currentHealth}");
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    // 사망
    public void Death()
    {
        Debug.Log(_blackBoard.MonsterState);
        _blackBoard.MonsterState = Blackboard.State.Death;
        StartCoroutine(DeathAnim());
    }

    // 사망 시 처리
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
    #endregion
}
