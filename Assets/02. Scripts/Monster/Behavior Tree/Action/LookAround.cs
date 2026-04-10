using UnityEngine;

public class LookAround : ActionNode
{
    // 경과 시간 및 지속시간
    private float _elapsedTime;
    private float _duration;

    private float _currentAngle;
    private float _maxRotAngle;
    private float _sin;


    public LookAround(float duration, float maxRotAngle, Blackboard blackboard)
    {
        _duration = duration;
        _maxRotAngle = maxRotAngle;
        _blackboard = blackboard;
    }

    public override void OnStart()
    {
        _blackboard.MonsterState = Blackboard.State.Idle;
        _blackboard.Animator.OnIdle();

        _elapsedTime = Time.time; // 경과시간 초기화
        _blackboard.NavMeshAgent.updateRotation = false; // NavMesh로 인한 회전 정지
        _currentAngle = _blackboard.Self.transform.eulerAngles.y; // 행동 시작시에 y축 rotation 값
    }

    public override NodeState OnUpdate()
    {
        float timer = Time.time - _elapsedTime;

        if (timer < _duration)
        {
            float rotate = _currentAngle + Mathf.Sin(timer) * _maxRotAngle;

            _blackboard.Self.transform.rotation = Quaternion.Euler(0f, rotate, 0f);

            return NodeState.Running;
        }

        return NodeState.Success;
    }

    public override void OnStop()
    {
        base.OnStop();
        _blackboard.NavMeshAgent.updateRotation = true;
        _blackboard.Self.transform.rotation = Quaternion.Euler(0f, _currentAngle, 0f);
    }
}
