using UnityEngine;

public class IsSeeingPlayer : Node
{
    private GameObject _self;
    private float _detectRadius;
    private LayerMask _playerLayer;
    private Blackboard _blackboard;

    public IsSeeingPlayer(float detectRadius, LayerMask playerLayer, Blackboard blackboard)
    {
        _detectRadius = detectRadius;
        _playerLayer = playerLayer;
        _blackboard = blackboard;
    }

    public override NodeState Evaluate()
    {
        if (_self == null)
            _self = _blackboard.Self;

        var overlapSphere = Physics.OverlapSphere(_self.transform.position, _detectRadius, _playerLayer);
        if (overlapSphere.Length > 0)
        {
            var player = overlapSphere[0].gameObject;

            // 각도, 방향, 거리 계산
            Vector3 direction = player.transform.position - _self.transform.position;
            float distance = direction.magnitude;
            float angle = Vector3.Angle(_self.transform.forward, direction);

            // 시야각이 150도 이내이고, 사이에 장애물이 없으면 발견 판정
            if (angle <= 150f &&
                Physics.Raycast(_self.transform.position, direction, out RaycastHit hit, distance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    _blackboard.CanAttackPlayer = distance <= 2f;
                    _blackboard.Player = player; // 플레이어 정보를 블랙보드에 저장
                    return NodeState.Success;
                }
            }
        }

        // 못찾았을 시 플레이어 관련 정보 비우기 및 Failure 반환
        _blackboard.Player = null;
        _blackboard.CanAttackPlayer = false;
        return NodeState.Failure;
    }
}
