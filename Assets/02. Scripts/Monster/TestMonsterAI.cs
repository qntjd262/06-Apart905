using UnityEngine;
using UnityEngine.AI;

public class TestMonsterAI : MonoBehaviour
{
    // 블랙보드 
    public Blackboard _blackboard;

    private Node _rootNode;
    private Vector3 _originPos;

    [Header("AI 설정")]
    [SerializeField] private float      _detectRadius = 5f;
    [SerializeField] private LayerMask  _playerLayer;

    [Header("Chase 경로 탐색 주기")]
    [SerializeField] private float _chaseInterval = 0.3f;

    [Header("Patrol 범위 및 시간")]
    [SerializeField] float minPatrolTime;
    [SerializeField] float maxPatrolTime;
    [SerializeField] float patrolRadius;
    
    private void Awake()
    {
        _blackboard = new Blackboard(); // 블랙보드 초기화

        _originPos = transform.position; // 초기 위치 저장
        _blackboard.Self = gameObject; // 몬스터 자신 저장
        _blackboard.NavMeshAgent = GetComponent<NavMeshAgent>(); // NavMeshAgnet 가져오기
        _blackboard.Animator = GetComponent<Animator>(); // Animation 가져오기
    }

    // SelectorNode -> 실패하면 다음 노드로
    // SequenceNode -> 성공하면 다음 노드로
    private void Start()
    {
        // 여러 곳에서 호출되는 노드 객체 미리 생성
        var AttackPlayer = new AttackPlayer(_blackboard);

        // Behavior Tree 생성
        var rootNode = new SelectorNode
            (
                // 공격 중이었다면 끝날 때까지 진행
                new SequenceNode
                (
                    new ConditionNode(() => _blackboard.IsAttacking),
                    AttackPlayer
                ),

                new SequenceNode
                (
                    new IsSeeingPlayer(_detectRadius, _playerLayer, _blackboard), // 플레이어를 보고 있는지 확인

                    new SelectorNode
                    (
                        new SequenceNode
                        (
                            new IsInAttackRange(_blackboard), // 공격 가능 범위 내인지 확인
                            new CooldownNode(3f),
                            AttackPlayer
                        ),

                        new ChasePlayer(_chaseInterval, _blackboard) // 탐지된 플레이어를 쫓기
                    )
                ),

                // 복귀 및 정찰
                new SequenceNode
                (
                    //new ReturnOriginPosition(_originPos, _blackboard), // 제자리로 복귀
                    new Idle(2f, 5f),
                    new PatrolToFindPlayer(10f, _blackboard)
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

        if (!Application.isPlaying) return;

        if (_blackboard.Player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _blackboard.Player.transform.position); // 탐지된 플레이어와의 선을 시각적으로 표시
        }

        if (_blackboard.NavMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _blackboard.NavMeshAgent.destination);
        }

        var center = _blackboard.Self.transform.position + (_blackboard.Self.transform.forward * 1f);
        Gizmos.DrawWireCube(center, new Vector3(0.5f, 1f, 0.5f));
    }
}
