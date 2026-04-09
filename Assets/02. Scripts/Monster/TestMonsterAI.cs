using UnityEngine;
using UnityEngine.AI;

public class TestMonsterAI : MonoBehaviour
{
    public Blackboard blackboard; // 블랙보드 

    private Node _rootNode;
    private Vector3 _originPos;
    private MonsterController monsterStatData; // 몬스터 스탯 데이터

    [Header("탐색 설정")]
    [SerializeField] private float      _detectRadius = 10f;
    [SerializeField] private LayerMask  _playerLayer;
    [SerializeField] private float      _detectAngle = 60f;
    [SerializeField] private Transform _center;

    [Header("Chase 경로 탐색 주기 및 추가 이동 시간")]
    [SerializeField] private float  _chaseInterval = 0.3f;
    [SerializeField] private float  _chaseDuration = 3f;

    [Header("공격 범위 및 공격 후 대기 시간")]
    [SerializeField] private Vector3 _halfExtents;
    [SerializeField] private float _waitTime;

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
        blackboard.Center = _center;
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


                new SequenceNode
                (
                    new IsSeeingPlayer(_detectRadius, _detectAngle, _playerLayer, blackboard), // 플레이어를 보고 있는지 확인

                    new SelectorNode // 플레이어가 보인다면
                    (
                        new SequenceNode
                        (
                            new IsInAttackRange(blackboard), // 공격 가능 범위 내인지 확인
                            new MemorySequenceNode // 범위 내라면 공격
                            (
                                new AttackPlayer(_halfExtents, blackboard),
                                new Wait(_waitTime, blackboard)
                            )
                        ),

                        new ChasePlayer(_chaseInterval, blackboard) // 범위 바깥이라면 추적
                    )
                ),

                new MemorySequenceNode
                (
                    new ConditionNode(() => blackboard.HasLostTarget),
                    new MoveToLastPoint(_chaseDuration, blackboard)
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
        Debug.Log(blackboard.MonsterState);
        _rootNode.Evaluate(); // 매 프레임마다 Behavior Tree 평가
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectRadius); // 탐지 범위를 시각적으로 표시
        Gizmos.DrawRay(_center.position, Quaternion.Euler(0, -_detectAngle, 0) * transform.forward * _detectRadius);
        Gizmos.DrawRay(_center.position, Quaternion.Euler(0, _detectAngle, 0) * transform.forward * _detectRadius);


        if (!Application.isPlaying) return;

        // 공격 범위 표시
        if (blackboard.MonsterState == Blackboard.State.Attack)
        {
            var center = _center.position + transform.forward;
            Gizmos.DrawWireCube(center, _halfExtents * 2f);
        }

        if (blackboard.Player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_center.position, blackboard.Player.transform.position); // 탐지된 플레이어와의 선을 시각적으로 표시
        }

        if (blackboard.NavMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_center.position, blackboard.NavMeshAgent.destination);
        }
    }
}
