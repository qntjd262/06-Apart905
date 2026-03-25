using UnityEngine;
using UnityEngine.AI;

public class ChaseDetectedEnemy : Node
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _detectedPlayer;
    private Blackboard _blackboard;
    private float _chaseInterval = 0.5f;
    private float _chaseTime = 0f;

    public ChaseDetectedEnemy(NavMeshAgent navMeshAgent, float chaseInterval, Blackboard blackboard)
    {
        _navMeshAgent = navMeshAgent;
        _blackboard = blackboard;
        _chaseInterval = chaseInterval;
    }

    public override NodeState Evaluate()
    {
        // 탐지된 적이 없다면 Failure 반환
        if (_detectedPlayer == null)
        {
            _detectedPlayer = _blackboard.Player;
            if (_detectedPlayer == null)
                return NodeState.Failure;
        }

        // _chaseInterval마다 플레이어 쪽으로 경로 탐색 및 이동 명령
        _chaseTime += Time.deltaTime;
        if (_chaseTime >= _chaseInterval)
        {
            // 플레이어 쪽으로 이동하게 명령
            _navMeshAgent.SetDestination(_detectedPlayer.transform.position);
            _chaseTime = 0f;
        }

        // 플레이어한테 이동할 때 거리가 정지거리 이하면 정지
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.ResetPath(); // 경로 초기화하여 멈춤
            _chaseTime = _chaseInterval; // 다음 추적이 가능하게 타이머 초기화
            return NodeState.Success;
        }

        // 거리가 아직 멀다면 Running 반환
        return NodeState.Running;
    }
}
