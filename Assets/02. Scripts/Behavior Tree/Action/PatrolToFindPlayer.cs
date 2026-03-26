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
 
    public PatrolToFindPlayer(float patrolRadius, Blackboard blackboard)
    {
        _blackboard = blackboard;
        _patrolRadius = patrolRadius;
    }

    public override NodeState Evaluate()
    {
        if (_navMeshAgent == null)
            _navMeshAgent = _blackboard.NavMeshAgent;

        SetRandomDestination();

        // TODO : 목적지까지 갔는지 확인되면 Success 리턴
        if (_navMeshAgent.pathPending) { }

        // patrol이 일정 시간 이상 지속될 시 강제 Success처리
        // 너무 오랫동안 patrol이 지속되거나 하는 등의 사태 방지를 위함
        _patrolTimer += Time.deltaTime;
        if (_patrolTimer >= _patrolDuration)
        { 
            _patrolTimer = 0f;
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public void SetRandomDestination()
    {
        Vector3 randomDir = Random.insideUnitCircle * _patrolRadius;

        randomDir += _blackboard.Self.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, _patrolRadius, NavMesh.AllAreas))
        {
            _navMeshAgent.SetDestination(hit.position);
        }
    }
}
