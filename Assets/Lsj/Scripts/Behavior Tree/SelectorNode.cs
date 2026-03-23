using System.Collections.Generic;

public class SelectorNode : Node
{
    private List<Node> _children = new List<Node>();

    public SelectorNode(List<Node> children)
    {
        _children = children;
    }

    public override NodeState Evaluate()
    { 
        foreach (Node child in _children
)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success: // 하나라도 성공하면 성공 반환
                    return NodeState.Success;

                case NodeState.Running: // 실행 중이면 상태 유지
                    return NodeState.Running;

                case NodeState.Failure: // 실패하면 다음 노드 평가
                    continue;
            }
        }

        return NodeState.Failure; // 모든 자식이 실패하면 실패 반환
    }
}
