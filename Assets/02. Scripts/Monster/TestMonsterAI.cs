using UnityEngine;
using UnityEngine.AI;

public class TestMonsterAI : MonoBehaviour
{
    public Blackboard blackboard; // 블랙보드 

    private Node _rootNode;
    private Vector3 _originPos;
    private MonsterStat monsterStatData; // 몬스터 스탯 데이터

    [Header("탐색 설정")]
    [SerializeField] private float      _detectRadius = 10f;
    [SerializeField] private LayerMask  _playerLayer;
    [SerializeField] private float      _detectAngle = 60f;
    [SerializeField] private Transform _center;

    [Header("Chase 경로 탐색 주기")]
    [SerializeField] private float  _chaseInterval = 0.3f;

    [Header("공격 범위")]
    [SerializeField] private Vector3 _halfExtents;

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
        blackboard.MonsterStat = GetComponent<MonsterStat>();
        blackboard.Self = gameObject; // 몬스터 자신 저장
        blackboard.NavMeshAgent = GetComponent<NavMeshAgent>(); // NavMeshAgnet 가져오기
        blackboard.Animator = GetComponent<Animator>(); // Animation 가져오기
        blackboard.Center = _center;
    }

    // SelectorNode -> 실패하면 다음 노드로
    // SequenceNode -> 성공하면 다음 노드로
    private void Start()
    {
        // 여러 곳에서 호출되는 노드 객체 미리 생성
        var AttackPlayer = new AttackPlayer(_halfExtents, blackboard);

        // Behavior Tree 생성
        var rootNode = new SelectorNode
            (
                //// 공격을 받았다면
                new SequenceNode
                (
                    new ConditionNode(() => blackboard.IsAttacked),
                    new Attacked(blackboard) // 피격 노드
                ),

                // 공격 중이었다면 끝날 때까지 진행
                new SequenceNode
                (
                    new ConditionNode(() => blackboard.IsAttacking),
                    AttackPlayer
                ),

                new SequenceNode
                (
                    new IsSeeingPlayer(_detectRadius, _detectAngle, _playerLayer, blackboard), // 플레이어를 보고 있는지 확인

                    new SelectorNode
                    (
                        new SequenceNode
                        (
                            new IsInAttackRange(blackboard), // 공격 가능 범위 내인지 확인
                            new CooldownNode(3f),
                            AttackPlayer
                        ),

                        new ChasePlayer(_chaseInterval, blackboard) // 탐지된 플레이어를 쫓기
                    )
                ),

                // 복귀 및 정찰
                new SequenceNode
                (
                    //new ReturnOriginPosition(_originPos, _blackboard), // 제자리로 복귀
                    new Idle(minIdleTime, maxIdleTime),
                    new PatrolToFindPlayer(patrolRadius, blackboard)
                )
            );

        _rootNode = rootNode;
    }

    private void Update()
    {
        _rootNode.Evaluate(); // 매 프레임마다 Behavior Tree 평가
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectRadius); // 탐지 범위를 시각적으로 표시
        Gizmos.DrawRay(_center.position, Quaternion.Euler(0, -_detectAngle, 0) * transform.forward * _detectRadius);
        Gizmos.DrawRay(_center.position, Quaternion.Euler(0, _detectAngle, 0) * transform.forward * _detectRadius);

        // 공격 범위 표시
        var center = _center.position + transform.forward;
        Gizmos.DrawWireCube(center, _halfExtents * 2f);

        if (!Application.isPlaying) return;

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
