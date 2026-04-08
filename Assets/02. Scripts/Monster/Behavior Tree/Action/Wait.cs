using UnityEngine;

public class Wait : ActionNode
{
    private float _lastTimer;
    private float _duration;

    public Wait(float duration, Blackboard blackboard)
    {
        _duration = duration;
        _blackboard = blackboard;
    }

    public override NodeState OnUpdate()
    {
        if (isFirstRun)
        {
            _lastTimer = Time.time;
            _blackboard.Animator.OnIdle();
            isFirstRun = false;
        }

        if (Time.time - _lastTimer >= _duration)
        {
            Debug.Log("Wait Сп");
            isFirstRun = true;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}
