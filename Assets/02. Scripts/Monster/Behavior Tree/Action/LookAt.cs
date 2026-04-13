using System;
using UnityEngine;

public class LookAt : ActionNode
{
    private Func<Vector3> _getDirection;
    private Vector3 _soundDirection;

    public LookAt(Func<Vector3> getDirection, Blackboard blackboard)
    {
        _getDirection = getDirection;
        _blackboard = blackboard;
    }

    public override void OnStart()
    {
        // getDir로 방향을 받아옴
        _soundDirection = _getDirection();
        Debug.Log(_soundDirection);
        // NavMeshAgent 회전 잠금
        _blackboard.NavMeshAgent.updateRotation = false; 
    }

    public override NodeState OnUpdate()
    {
        var selfTransform = _blackboard.Self.transform;

        var lookRotation = Quaternion.LookRotation(_soundDirection);
        selfTransform.rotation = Quaternion.Slerp(selfTransform.rotation, lookRotation, 1 * Time.deltaTime);

        if (Vector3.Angle(selfTransform.forward, _soundDirection) < 3f)
        {
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public override void OnStop()
    {
        base.OnStop();
        _blackboard.NavMeshAgent.updateRotation = true;
        _blackboard.CanHearPlayer = false;
        _blackboard.SoundDirection = Vector3.zero;
    }
}
