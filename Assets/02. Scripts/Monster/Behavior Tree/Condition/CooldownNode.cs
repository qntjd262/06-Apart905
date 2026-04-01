using UnityEngine;

public class CooldownNode: Node
{
    private float _lastTimer = 0f;
    private float _cooldown;

    public CooldownNode(float cooldown)
    {
        _cooldown = cooldown;
    }

    public override NodeState Evaluate()
    {
        // 현재 시간 - 지난 시작 시간 < 쿨타임이면 쿨타임이 안지났으므로 faiure
        if (Time.time - _lastTimer < _cooldown)
        {
            return NodeState.Failure;
        }

        _lastTimer = Time.time;
        return NodeState.Success;
        
    }
}
