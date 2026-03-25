using UnityEngine;

public class IsSeeingPlayer : Node
{
    private Transform _self;
    private float _detectRadius;
    private LayerMask _playerLayer;
    private Blackboard _blackboard;

    public IsSeeingPlayer(Transform self, float detectRadius, LayerMask playerLayer, Blackboard blackboard)
    {
        _self = self;
        _detectRadius = detectRadius;
        _playerLayer = playerLayer;
        _blackboard = blackboard;
    }

    public override NodeState Evaluate()
    {
        var overlapSphere = Physics.OverlapSphere(_self.position, _detectRadius, _playerLayer);
        if (overlapSphere.Length > 0)
        {
            var player = overlapSphere[0].gameObject;

            // 각도, 방향, 거리 계산
            Vector3 direction = player.transform.position - _self.position;
            float distance = direction.magnitude;
            float angle = Vector3.Angle(_self.forward, direction);

            // 시야각이 150도 이내이고, 사이에 장애물이 없으면 발견 판정
            if (angle <= 150f &&
                Physics.Raycast(_self.position, direction, out RaycastHit hit, distance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    _blackboard.Player = player; // 플레이어 정보를 블랙보드에 저장
                    Debug.Log(_blackboard.Player);
                    return NodeState.Success;
                }
            }
        }

        return NodeState.Failure;
    }
}
