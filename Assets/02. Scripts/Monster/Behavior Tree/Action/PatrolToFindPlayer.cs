using UnityEngine;
using UnityEngine.AI;

public class PatrolToFindPlayer : ActionNode
{
    private NavMeshAgent _navMeshAgent;

    // Patrol 범위
    private float _patrolRadius;

    // Patrol 시간
    private float _patrolTimer = 0f;
    private float _patrolDuration = 5f;
 
    /// <summary>
    /// 주변 랜덤한 장소로 이동하며 Patrol하는 함수
    /// </summary>
    /// <param name="patrolRadius">Patrol 범위</param>
    /// <param name="blackboard">데이터 저장용 블랙보드</param>
    public PatrolToFindPlayer(float patrolRadius, Blackboard blackboard)
    {
        _blackboard = blackboard;
        _patrolRadius = patrolRadius;
        _navMeshAgent = _blackboard.NavMeshAgent;
    }

    public override NodeState OnUpdate()
    {
        if(isFirstRun)
        {
            _blackboard.Animator.OnWalk();
            isFirstRun = false;
        }

        // 길을 찾고있거나 이미 찾은 것이 아니라면 
        if (!_navMeshAgent.pathPending && !_navMeshAgent.hasPath)
        {
            // 랜덤한 목적지로 이동
            SetRandomDestination();
        }

        // 시간 업데이트
        _patrolTimer += Time.deltaTime;

        // 목적지까지 도착했는가 확인하는 변수
        bool isArrived = !_navMeshAgent.pathPending
            && _navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance;
        // patrol이 일정 시간 이상 지속될 시 강제로 성공처리 하기위한 변수
        bool isTimeOver = _patrolTimer >= _patrolDuration; 

        if (isArrived || isTimeOver)
        {
            _navMeshAgent.ResetPath();
            _patrolTimer = 0f;
            isFirstRun = true;
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public override void OnStop()
    {
        Debug.Log("Patrol이 다른 running노드가 생겨 정지");
        _navMeshAgent.ResetPath();
        _patrolTimer = 0f;
        isFirstRun = true;
    }

    public void SetRandomDestination()
    {
        Vector2 randomDir2d = Random.insideUnitCircle * _patrolRadius;
        Vector3 randomDir = new Vector3(randomDir2d.x, 0, randomDir2d.y);

        randomDir += _blackboard.Self.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, _patrolRadius, NavMesh.AllAreas))
        {
            _navMeshAgent.SetDestination(hit.position);
        }
    }
}
