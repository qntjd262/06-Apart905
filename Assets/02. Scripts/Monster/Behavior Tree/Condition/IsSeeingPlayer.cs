using UnityEngine;

public class IsSeeingPlayer : Node
{
    private GameObject _self;
    private float _detectRadius;
    private float _detectAngle;
    private LayerMask _playerLayer;
    private Blackboard _blackboard;

    public IsSeeingPlayer(float detectRadius, float detectAngle, LayerMask playerLayer, Blackboard blackboard)
    {
        _detectRadius = detectRadius;
        _detectAngle = detectAngle;
        _playerLayer = playerLayer;
        _blackboard = blackboard;
        _self = _blackboard.Self;
    }

    public override NodeState OnUpdate()
    {
        var overlapSphere = Physics.OverlapSphere(_blackboard.Self.transform.position, _detectRadius, _playerLayer);
        if (overlapSphere.Length > 0)
        {
            var player = overlapSphere[0].gameObject;

            // 각도, 방향, 거리 계산
            Vector3 direction = player.transform.position - _blackboard.Center.position + new Vector3(0, 1f, 0);
            float distance = direction.magnitude;
            float angle = Vector3.Angle(_self.transform.forward, direction);
            Debug.Log(angle);
            // 시야각이 일정 각도 이내이고, 사이에 장애물이 없으면 발견 판정
            if (angle <= _detectAngle &&
                Physics.Raycast(_blackboard.Center.position, direction, out RaycastHit hit, distance + 0.5f))
            {
                // hit한 오브젝트의 태그가 Player라면
                if (hit.collider.CompareTag("Player"))
                {
                    _blackboard.Player = player; // 플레이어 정보를 블랙보드에 저장
                    return NodeState.Success;
                }
            }
        }

        // 못찾았을 시 플레이어 관련 정보 비우기 및 Failure 반환
        _blackboard.Player = null;
        return NodeState.Failure;
    }
}
