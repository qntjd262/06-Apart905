using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider))]
public class MonsterController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Blackboard _blackboard;
    private Coroutine _visionSensor;
    public MonsterStatSO monsterStatSO;

    [Header("몬스터 체력")]
    private float monsterHealth;
    [SerializeField] private float currentHealth;

    [Header("적 탐지 센서")]
    //[SerializeField] private float detectRadius = 10f;
    [SerializeField] private float detectAngle = 60f;

    private void Awake()
    {
        monsterHealth = monsterStatSO.maxHP;
    }

    private void Start()
    {
        currentHealth = monsterHealth;
        _blackboard = GetComponent<TestMonsterAI>().blackboard;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    #region 적 보이는지 체크
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _visionSensor = StartCoroutine(VisionSensor(other.gameObject));
        }
    }

    IEnumerator VisionSensor(GameObject player)
    {
        while (true)
        {
            // 각도, 방향, 거리 계산
            Vector3 direction = player.transform.position - _blackboard.Center.position + new Vector3(0, 1.5f, 0);
            float distance = direction.magnitude;
            float angle = Vector3.Angle(transform.forward, direction);
            Debug.Log(angle);
            // 시야각이 일정 각도 이내이고, 사이에 장애물이 없으면 발견 판정
            if (angle <= detectAngle &&
                Physics.Raycast(_blackboard.Center.position, direction, out RaycastHit hit, distance + 0.5f))
            {
                // hit한 오브젝트의 태그가 Player라면
                if (hit.collider.CompareTag("Player"))
                {
                    _blackboard.Player = player; // 플레이어 정보를 블랙보드에 저장
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_visionSensor != null)
            {
                StopCoroutine(_visionSensor);
                _visionSensor = null;
            }
            _blackboard.Player = null;
        }
    }

    #endregion

    #region 적 발소리 체크
    /// <summary>
    /// 플레이어의 발소리가 난 위치를 전달받아 처리
    /// </summary>
    /// <param name="soundOrigin"></param>
    public void CanHearPlayerSound(Vector3 soundOrigin)
    {
        if (_blackboard.MonsterState != Blackboard.State.Idle &&
            _blackboard.MonsterState != Blackboard.State.Patrol)
        {
            return;
        }

        var soundDirection = (soundOrigin - transform.position).normalized;

        // TODO : 플레이어의 Noise 범위 내에 있는지 확인
        // 만약, 범위 내에 있다면 true, 없다면 false
        _blackboard.CanHearPlayer = true;
        _blackboard.SoundDirection = soundDirection;
    }
    #endregion

    #region 피격 및 사망
    public void TakeDamage(float damage, GameObject player)
    {
        if (currentHealth < 0)
            return;

        currentHealth -= damage;
       
        _blackboard.Player = player;
        _blackboard.MonsterState = Blackboard.State.Attacked;

        Debug.Log($"{damage} 입음, 남은 체력 {currentHealth}");
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    // 사망
    public void Death()
    {
        Debug.Log(_blackboard.MonsterState);
        _blackboard.MonsterState = Blackboard.State.Death;
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
