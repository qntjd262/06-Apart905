using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer: ActionNode
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    // 추적 시간
    private float _chaseInterval = 0.3f;
    private float _chaseTime = 0.3f;

    public ChasePlayer(float chaseInterval, Blackboard blackboard)
    {
        _blackboard = blackboard;
        _chaseInterval = chaseInterval;
        _animator = _blackboard.Animator;
    }

    public override NodeState Evaluate()
    {
        if (_navMeshAgent == null)
            _navMeshAgent = _blackboard.NavMeshAgent;

        // 매번 적이 있는지 확인 후, 없으면 Failure
        var player = _blackboard.Player;
        if (player == null)
        {
            _navMeshAgent.ResetPath();
            return NodeState.Failure;
        }

        // _chaseInterval마다 플레이어 쪽으로 경로 탐색 및 이동 명령
        _chaseTime += Time.deltaTime;
        if (_chaseTime >= _chaseInterval)
        {
            _navMeshAgent.SetDestination(player.transform.position); // 플레이어 쪽으로 이동하게 명령
            _chaseTime = 0f;
        }

        // 플레이어한테 이동할 때 거리가 정지거리 이하면 정지
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            Debug.Log("Chase 끝");
            _navMeshAgent.ResetPath(); // 경로 초기화하여 멈춤
            _chaseTime = _chaseInterval; // 다음 추적이 가능하게 타이머 초기화
            return NodeState.Success;
        }

        // 거리가 아직 멀다면 Running 반환
        return NodeState.Running;
    }
}
