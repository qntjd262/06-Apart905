using System;

/// <summary>
/// Func<NodeState>로 액션을 받아 결과를 반환하는 노드
/// </summary>
public class ActionNode : Node
{
    private Func<NodeState> _action;

    public ActionNode(Func<NodeState> action)
    {
        _action = action;
    }

    public override NodeState Evaluate()
    {
        return _action.Invoke();
    }
}
