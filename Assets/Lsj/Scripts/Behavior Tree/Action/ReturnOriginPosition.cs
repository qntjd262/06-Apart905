using UnityEngine;
using UnityEngine.AI;

public class ReturnOriginPosition : Node
{
    private GameObject _self;
    private Vector3 _originPos;
    private NavMeshAgent _navMeshAgent;

    public ReturnOriginPosition(GameObject self, Vector3 originPos, NavMeshAgent navMeshAgent)
    {
        _self = self;
        _originPos = originPos;
        _navMeshAgent = navMeshAgent;
    }

    public override NodeState Evaluate()
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
