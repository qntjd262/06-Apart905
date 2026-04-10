using System;
using UnityEngine;

public class ConditionNode : Node
{
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override NodeState OnUpdate()
    {
        return _condition() ? NodeState.Success : NodeState.Failure;
    }
}
