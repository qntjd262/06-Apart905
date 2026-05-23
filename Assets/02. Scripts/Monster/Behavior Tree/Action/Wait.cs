using UnityEngine;

public class Wait : ActionNode
{
    private float _startTimer;
    private float _duration;

    public Wait(float duration, Blackboard blackboard)
    {
        _duration = duration;
        _blackboard = blackboard;
    }

    public override void OnStart()
    {
        _startTimer = Time.time;
    }

    public override NodeState OnUpdate()
    {
        if (Time.time - _startTimer >= _duration)
        {
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public override void OnStop()
    {
        base.OnStop();
        _startTimer = 0f;
    }
}
