using UnityEngine;
using UnityEngine.AI;

public class Attacked : ActionNode
{
    // 피격 시간
    private float _hitTimer;
    private float _hitDuration = 2f;

    // 진행 상태 변수
    private bool _isRunning;
    
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    public Attacked(Blackboard blackboard)
    {
        _blackboard = blackboard;
        _animator = _blackboard.Animator;
        _navMeshAgent = _blackboard.NavMeshAgent;
    }

    public override NodeState Evaluate()
    {
        // 처음 시작 시 초기화
        if (!_isRunning)
        {
            Debug.Log("공격 받음");
            _isRunning = true;
            _hitTimer = Time.time;
            _blackboard.IsAttacking = false;

            // 이동 정지
            _navMeshAgent.isStopped = true;

            // TODO : 피격 애니메이션 실행
        }

        // 피격 시간이 아직 다 끝나지 않았을 때
        if (Time.time - _hitTimer < _hitDuration)
        {
            return NodeState.Running;
        }

        // 정지 해제
        _navMeshAgent.isStopped = false;

        // 피격 및 진행, 시간 변수 초기화
        _isRunning = false;
        _hitTimer = 0f;
        _blackboard.IsAttacked = false;

        return NodeState.Success;
    }
}
