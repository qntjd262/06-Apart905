using UnityEngine;


public class Idle : ActionNode
{
    private Animator _animator;

    private float _idleTimer;
    private float _idleDuration = 0f;
    private float _minIdleTime;
    private float _maxIdleTime;

    /// <summary>
    /// 일정 시간동안 가만히 대기하는 함수
    /// </summary>
    /// <param name="minIdleTime">대기 최소 시간</param>
    /// <param name="maxIdleTime">대기 최대 시간</param>
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
            Debug.Log("Idle Success");
            _idleTimer = 0f;
            _idleDuration = 0f;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}
