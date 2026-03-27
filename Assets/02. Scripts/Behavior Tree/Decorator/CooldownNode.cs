using UnityEngine;

public class CooldownNode: DecoratorNode
{
    private float _cooldownTimer;
    private float _cooldownDuration;

    public CooldownNode(Node child, float cooldownDuration) : base(child)
    {
        _cooldownDuration = cooldownDuration;
        _cooldownTimer = _cooldownDuration;
    }

    public override NodeState Evaluate()
    {
        _cooldownTimer += Time.deltaTime;
        if (_cooldownTimer < _cooldownDuration)
        {
            return NodeState.Failure;
        }

        var result = child.Evaluate();
        if (result == NodeState.Success || result == NodeState.Running)
        {
            _cooldownTimer = 0f;
        }

        return result;
    
    }
}
