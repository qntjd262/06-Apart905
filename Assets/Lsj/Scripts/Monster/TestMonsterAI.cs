using UnityEngine;
using UnityEngine.AI;

public class TestMonsterAI : MonoBehaviour
{
    // 블랙보드 
    public Blackboard _blackboard;

    private Node _rootNode;
    private GameObject _detectedPlayer;
    private Vector3 _originPos;
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    [Header("AI 설정")]
    [SerializeField] private float      _detectRadius = 5f;
    [SerializeField] private LayerMask  _playerLayer;

    [Header("Chase 경로 탐색 주기")]
    [SerializeField] private float _chaseInterval = 0.3f;
    private float _chaseTime = 0.2f;

    private void Awake()
    {
        _originPos = transform.position; // 초기 위치 저장
        _navMeshAgent = GetComponent<NavMeshAgent>(); // NavMeshAgnet 가져오기
        _animator = GetComponent<Animator>();
        _blackboard = new Blackboard(); // 블랙보드 초기화
    }

    private void Start()
    {
        // Behavior Tree 생성
        var rootNode = new SelectorNode // 실패하면 다음 노드로
            (
                new SequenceNode // 성공하면 다음 노드로
                (
                    new IsSeeingPlayer(transform, _detectRadius, _playerLayer, _blackboard), // 플레이어를 보고 있는지 확인

                    new SelectorNode
                    (
                        new ChaseDetectedEnemy(_navMeshAgent, _chaseInterval, _blackboard), // 탐지된 플레이어를 쫓기
                        new ActionNode(AttackPlayer)
                    )
                ),

                new ReturnOriginPosition(gameObject, _originPos, _navMeshAgent) // 원래 위치로 복귀
            );

        _rootNode = rootNode;
    }

    private void Update()
    {
        _rootNode.Evaluate(); // 매 프레임마다 Behavior Tree 평가
    }

    private NodeState IsPlayerSeeing()
    {
        var overlapSphere = Physics.OverlapSphere(transform.position, _detectRadius, _playerLayer);
        if (overlapSphere.Length > 0)
        {
            Debug.Log(overlapSphere[0].gameObject.name);
            var player = overlapSphere[0].gameObject;

            // 각도, 방향, 거리 계산
            float angle         = Vector3.Angle(transform.forward, overlapSphere[0].transform.position);
            Vector3 direction   = player.transform.position - transform.position;
            float distance      = direction.magnitude;

            // 시야각이 150도 이내이고, 사이에 장애물이 없으면 발견 판정
            if (angle <= 150f &&
                Physics.Raycast(transform.position, direction, out RaycastHit hit, distance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    _detectedPlayer = hit.collider.gameObject;
                    return NodeState.Success;
                }
            }
        }

        return NodeState.Failure;
    }

    // 범위 내에 적을 탐색
    private NodeState CheckDetectedEnemy()
    {
        // 본 몬스터를 중심으로 한 구체를 생성
        var overlapSphere = Physics.OverlapSphere(transform.position, _detectRadius, _playerLayer); 

        // 구체 안에 플레이어가 탐지되면 _detectedPlayer에 저장 및 Success 반환 
        if (overlapSphere != null && overlapSphere.Length > 0) 
        {
            _detectedPlayer = overlapSphere[0].gameObject;
            return NodeState.Success;
        }

        // 적이 없다면 failure 반환
        return NodeState.Failure; 
    }

    private NodeState ChaseDetectedEnemy()
    {
        // 탐지된 적이 없다면 Failure 반환
        if (_detectedPlayer == null) return NodeState.Failure;

        // _chaseInterval마다 플레이어 쪽으로 경로 탐색 및 이동 명령
        _chaseTime += Time.deltaTime;
        if (_chaseTime >= _chaseInterval)
        {
            _navMeshAgent.SetDestination(_detectedPlayer.transform.position);  // 플레이어 쪽으로 이동하게 명령
            transform.rotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized); // 이동 방향으로 회전
            _chaseTime = 0f;
        }

        // 플레이어한테 이동할 때 거리가 정지거리 이하면 정지
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.ResetPath(); // 경로 초기화하여 멈춤
            _chaseTime = _chaseInterval; // 다음 추적이 가능하게 타이머 초기화
            return NodeState.Success;
        }

        if (Vector3.Distance(transform.position, _detectedPlayer.transform.position) > _detectRadius)
        {
            _detectedPlayer = null; // 플레이어가 탐지 범위를 벗어나면 탐지된 플레이어 초기화
            return NodeState.Failure; // 플레이어를 잃었으므로 Failure 반환
        }

        // 거리가 아직 멀다면 Running 반환
        return NodeState.Running;
    }

    // 플레이어에게 공격
    private NodeState AttackPlayer()
    {
        if (_detectedPlayer == null) return NodeState.Failure;

        Debug.Log("공격");

        return NodeState.Success;
    }

    // 원래 위치로 복귀
    private NodeState ReturnOriginPosition()
    {
        // 원래 위치와의 거리가 0.5 이하면 Success 반환
        if (Vector3.Distance(transform.position ,_originPos) < 0.5f) 
        {
            return NodeState.Success;
        }

        _navMeshAgent.SetDestination(_originPos); // NavMeshAgent를 사용하여 원래 위치로 이동

        // 아직 원래 위치로 돌아가지 못했다면 Running 반환
        return NodeState.Running; 
    }

    // 플레이어를 찾아 순찰
    private NodeState PatrolToFindPlayer()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _detectRadius;
        randomDirection += transform.position;

        return NodeState.Running;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectRadius); // 탐지 범위를 시각적으로 표시

        if (_detectedPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _detectedPlayer.transform.position); // 탐지된 플레이어와의 선을 시각적으로 표시
        }
    }
}
