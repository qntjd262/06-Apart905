using System;
using UnityEngine;

public class LookAt : ActionNode
{
    private Func<Vector3> _getDirection;
    private Vector3 _soundDirection;
    private float _rotateSpeed;

    public LookAt(Func<Vector3> getDirection, float rotateSpeed, Blackboard blackboard)
    {
        _getDirection = getDirection;
        _rotateSpeed = rotateSpeed;
        _blackboard = blackboard;
    }

    public override void OnStart()
    {
        // getDir로 방향을 받아옴
        _soundDirection = _getDirection();
        // NavMeshAgent 회전 잠금
        _blackboard.NavMeshAgent.updateRotation = false;
    }

    public override NodeState OnUpdate()
    {
        var selfTransform = _blackboard.Self.transform;

        var lookRotation = Quaternion.LookRotation(_soundDirection);
        selfTransform.rotation = Quaternion.Slerp(selfTransform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);

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
