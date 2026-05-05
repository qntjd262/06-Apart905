using UnityEngine;
using UnityEngine.AI;

public class MoveToLastPoint : ActionNode
{
    private NavMeshAgent _navMeshAgent;

    private float _elapsedTime;
    private float _duration;

    public MoveToLastPoint(float duration, Blackboard blackboard)
    {
        _duration = duration;
        _blackboard = blackboard;
        _navMeshAgent = _blackboard.NavMeshAgent;
    }

    public override void OnStart()
    {
        _elapsedTime = Time.time;
        _navMeshAgent.SetDestination(_blackboard.LastPoint);
    }
    
    public override NodeState OnUpdate()
    {
        Debug.Log("Chase 이후 추적 중");

        // 추가 탐색 후 일정 시간이 지났다면 성공 처리
        if (Time.time - _elapsedTime > _duration)
        {
            return NodeState.Success;
        }

        // 길 찾는 중이거나
        // 일정거리까지 가까워지지 않았다면 계속 진행
        if (_navMeshAgent.pathPending ||
            _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
        {
            return NodeState.Running;
        }

        return NodeState.Success;
    }

    public override void OnStop()
    {
        base.OnStop();
        _navMeshAgent.ResetPath();
        _blackboard.HasLostTarget = false;
        _blackboard.LastPoint = Vector3.zero;
    }
}
