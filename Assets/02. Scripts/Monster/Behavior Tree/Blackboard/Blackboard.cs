using UnityEngine;
using UnityEngine.AI;

public class Blackboard
{
    // 적(플레이어) 관련
    public GameObject Player { get; set; }
    public float Distance { get; set; }

    // 기본 데이터
    public GameObject Self { get; set; }
    public Animator Animator { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public MonsterStatSO MonsterStat { get; set; }
    public Transform Center { get; set; }

    // 공격, 피격
    public bool IsAttacking { get; set; }
    public bool IsAttacked { get; set; }

}
