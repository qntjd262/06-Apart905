using UnityEngine;
using UnityEngine.AI;

public class Blackboard
{
    public enum State
    {
        Idle, Patrol, Attacking, Attacked
    }

    // 적(플레이어) 관련
    public GameObject Player { get; set; }
    public float Distance { get; set; }

    // 기본 데이터
    public GameObject Self { get; set; }
    public MonsterAnimator Animator { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public MonsterStat MonsterStat { get; set; }
    public Transform Center { get; set; }

    // 공격, 피격
    public State MonsterState { get; set; } = State.Idle;
}
