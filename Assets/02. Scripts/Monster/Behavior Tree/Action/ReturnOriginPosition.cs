using UnityEngine;
using UnityEngine.AI;

public class ReturnOriginPosition : ActionNode
{
    private Animator _animator;

    private GameObject _self;
    private Vector3 _originPos;
    private NavMeshAgent _navMeshAgent;

    public ReturnOriginPosition(Vector3 originPos, Blackboard blackboard)
    {
        _originPos = originPos;
        _blackboard = blackboard;
        _self = _blackboard.Self;
        _navMeshAgent = _blackboard.NavMeshAgent;
    }

    public override NodeState OnUpdate()
    {
        // 원래 위치와의 거리가 0.5 이하면 Success 반환
        if (Vector3.Distance(_self.transform.position, _originPos) < 0.5f)
        {
            return NodeState.Success;
        }

        _navMeshAgent.SetDestination(_originPos); // NavMeshAgent를 사용하여 원래 위치로 이동

        // 아직 원래 위치로 돌아가지 못했다면 Running 반환
        return NodeState.Running;
    }

}
