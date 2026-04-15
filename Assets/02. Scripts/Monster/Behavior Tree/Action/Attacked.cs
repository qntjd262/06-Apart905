using UnityEngine;
using UnityEngine.AI;

public class Attacked : ActionNode
{
    // 피격 시간
    private float _hitTimer;
    private float _hitDuration = 3f;

    // 진행 상태 변수
    private bool _isRunning;
    
    private NavMeshAgent _navMeshAgent;

    // 피격 후 돌아볼 시 사용
    private Transform playerTransform;
    private Transform selfTransform;

    public Attacked(Blackboard blackboard)
    {
        _blackboard = blackboard;
        _navMeshAgent = _blackboard.NavMeshAgent;
    }

    public override void OnStart()
    {
        _hitTimer = Time.time;
        _blackboard.MonsterState = Blackboard.State.Attacked;

        // 이동 정지
        _navMeshAgent.isStopped = true;

        // 돌아보는데 필요한 정보
        playerTransform = _blackboard.Player.transform;
        selfTransform = _blackboard.Self.transform;
        _blackboard.Player = null;
        _isRunning = true;

        // 맞은 방향 계산
        Vector3 dirToPlayer = (playerTransform.position - selfTransform.position).normalized;
        float dot = Vector3.Dot(selfTransform.forward, dirToPlayer);

        // angle이 0보다 크면 앞에서, 작으면 뒤에서 맞는 애니메이션 출력
        _blackboard.Animator.OnAttacked(dot > 0);
    }

    public override NodeState OnUpdate()
    {
        // 피격 시간이 아직 다 끝나지 않았을 때
        if (Time.time - _hitTimer < _hitDuration)
        {
            return NodeState.Running;
        }

        // 플레이어 방향을 바라봄
        if (_isRunning)
        {
            _navMeshAgent.updateRotation = false; // 네브메쉬로 인한 회전 정지

            Quaternion dirToPlayer = Quaternion.LookRotation(playerTransform.position - selfTransform.position);
            _blackboard.Self.transform.rotation = Quaternion.Slerp(selfTransform.rotation, dirToPlayer, 1.5f * Time.deltaTime);

            // 아직 다 돌아보지 않았다면 계속 회전
            if (Quaternion.Angle(_blackboard.Self.transform.rotation, dirToPlayer) > 30f)
            {
                return NodeState.Running;
            }
        }

        OnStop();

        return NodeState.Success;
    }

    public override void OnStop()
    {
        base.OnStop();
        _navMeshAgent.updateRotation = true;    // 네브메쉬로 인한 회전 시작
        _navMeshAgent.ResetPath();              // 기존 경로 초기화
        _navMeshAgent.isStopped = false;        // 네브메쉬 이동 시작
        _blackboard.MonsterState = Blackboard.State.Idle;  // Idle 상태로  변경
        _isRunning = false;
        _hitTimer = 0f;
    }
}
