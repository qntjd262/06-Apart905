using UnityEngine;
using UnityEngine.AI;

public class Blackboard
{
    public enum State
    {
        Idle, Patrol, Chase, Attack, Attacked, Death
    }

    // 적(플레이어) 관련
    public GameObject Player { get; set; }
    // 적(플레이어) 발소리 여부 및 위치
    public bool CanHearPlayer { get; set; }
    public Vector3 SoundDirection{ get; set; }

    // 기본 데이터
    public GameObject Self { get; set; }
    public MonsterAnimator Animator { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public MonsterController MonsterStat { get; set; }
    public Transform Center { get; set; }

    // 공격, 피격
    private State _monsterState = State.Idle;
    public State MonsterState
    {
        get => _monsterState;
        set
        {
            if (_monsterState == State.Death) return;
            _monsterState = value;
        }
    }

    // 추적 중 플레이어가 안보이게 되었을 때
    public bool HasLostTarget { get; set; } = false;
    public Vector3 LastPoint { get; set; }
}
