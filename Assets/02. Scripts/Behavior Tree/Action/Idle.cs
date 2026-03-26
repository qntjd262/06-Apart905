using UnityEngine;

public class Idle : ActionNode
{
    private float _idleTimer;
    private float _idleDuration = 0f;
    private float _minIdleTime;
    private float _maxIdleTime;

    public Idle(float minIdleTime, float maxIdleTime)
    {
        _minIdleTime = minIdleTime;
        _maxIdleTime = maxIdleTime;
    }

    public override NodeState Evaluate()
    {
        if (_idleDuration == 0f)
            _idleDuration = Random.Range(_minIdleTime, _maxIdleTime);

        _idleTimer += Time.deltaTime;

        if (_idleTimer >= _idleDuration)
        {
            _idleTimer = 0f;
            _idleDuration = 0f;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}
