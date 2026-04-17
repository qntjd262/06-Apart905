using UnityEngine;
using UnityEngine.AI;

public class TestMonsterAI : MonoBehaviour
{
    public Blackboard blackboard; // 블랙보드 

    private Node _rootNode;
    private Vector3 _originPos;

    [Header("탐색 설정")]
    [SerializeField] private float      detectRadius = 10f;
    [SerializeField] private LayerMask  playerLayer;
    [SerializeField] private float      detectAngle = 60f;
    [SerializeField] private Transform  center;

    [Header("Chase 경로 탐색 주기")]
    [SerializeField] private float  _chaseInterval = 0.3f;
    [Header("Chase 후 추가 이동 시간")]
    [SerializeField] private float  _chaseDuration = 3f;

    [Header("공격 범위, 공격 후 대기 시간, 공격 범위 offset")]
    [SerializeField] private Vector3    _halfExtents;
    [SerializeField] private float      _waitTime;
    [SerializeField] private Vector3    offset;

    [Header("Patrol 범위 및 시간")]
    [SerializeField] float minPatrolTime;
    [SerializeField] float maxPatrolTime;
    [SerializeField] float patrolRadius;

    [Header("Idle 시간")]
    [SerializeField] float minIdleTime;
    [SerializeField] float maxIdleTime;

    private void Awake()
    {
        blackboard = new Blackboard(); // 블랙보드 초기화

        _originPos = transform.position; // 초기 위치 저장
        blackboard.MonsterStat = GetComponent<MonsterController>();
        blackboard.Self = gameObject; // 몬스터 자신 저장
        blackboard.NavMeshAgent = GetComponent<NavMeshAgent>(); // NavMeshAgnet 가져오기
        blackboard.Animator = GetComponent<MonsterAnimator>(); // Animator 가져오기
        blackboard.Center = center;
    }

    // SelectorNode -> 실패하면 다음 노드로
    // SequenceNode -> 성공하면 다음 노드로
    private void Start()
    {
        // Behavior Tree 생성
        var rootNode = new SelectorNode
            (
                // 몬스터가 죽었다면
                new SequenceNode
                (
                    new ConditionNode(() => blackboard.MonsterState == Blackboard.State.Death),
                    new Death(blackboard) // 사망
                ),

                // 공격을 받았다면
                new MemorySequenceNode
                (
                    new ConditionNode(() => blackboard.MonsterState == Blackboard.State.Attacked),
                    new Attacked(blackboard) // 피격 노드
                ),

                // 플레이어가 보인다면
                new SequenceNode
                (
                    new IsSeeingPlayer(detectRadius, detectAngle, playerLayer, blackboard), // 플레이어를 보고 있는지 확인

                    new SelectorNode // 플레이어가 보인다면
                    (
                        new SequenceNode
                        (
                            new IsInAttackRange(blackboard), // 공격 가능 범위 내인지 확인
                            new MemorySequenceNode // 범위 내라면 공격
                            (
                                new AttackPlayer(_halfExtents, offset, blackboard),
                                new Wait(_waitTime, blackboard)
                            )
                        ),

                        new ChasePlayer(_chaseInterval, blackboard) // 범위 바깥이라면 추적
                    )
                ),

                // 적의 발소리가 들린다면 && 추적 중이 아니라면 
                new SequenceNode
                (
                    new ConditionNode(() => blackboard.MonsterState != Blackboard.State.Chase &&
                                            blackboard.CanHearPlayer),
                    new LookAt(() => blackboard.SoundDirection, 1f, blackboard)
                ),

                // 추적 중 플레이어가 시야에서 사라진다면
                new MemorySequenceNode
                (
                    new ConditionNode(() => blackboard.HasLostTarget),
                    new MoveToLastPoint(_chaseDuration, blackboard),
                    new LookAround(4f, 40f, blackboard)
                ),

                // 정찰 및 대기
                new MemorySequenceNode
                (
                    new Idle(minIdleTime, maxIdleTime, blackboard),
                    new PatrolToFindPlayer(patrolRadius, blackboard)
                )
            );

        _rootNode = rootNode;
    }

    private void Update()
    {
        //Debug.Log(blackboard.MonsterState);
        //Debug.Log(blackboard.HasLostTarget);
        _rootNode.Evaluate(); // 매 프레임마다 Behavior Tree 평가
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRadius); // 탐지 범위를 시각적으로 표시
        Gizmos.DrawRay(this.center.position, Quaternion.Euler(0, -detectAngle, 0) * transform.forward * detectRadius);
        Gizmos.DrawRay(this.center.position, Quaternion.Euler(0, detectAngle, 0) * transform.forward * detectRadius);

        var center = this.center.position + transform.forward + offset;
        Gizmos.DrawWireCube(center, _halfExtents * 2f);

        if (!Application.isPlaying) return;

        if (blackboard.Player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.center.position, blackboard.Player.transform.position + new Vector3(0, 0.5f, 0)); // 탐지된 플레이어와의 선을 시각적으로 표시
        }

        if (blackboard.NavMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.center.position, blackboard.NavMeshAgent.destination);
        }
    }
}
