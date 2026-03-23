using System.Collections.Generic;

public class SequenceNode : Node
{
    private List<Node> _children = new List<Node>();

    public SequenceNode(List<Node> children)
    {
        _children = children;
    }

    public override NodeState Evaluate()
    {
        foreach (Node child in _children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success: // 성공하면 다음 노드로
                    continue;

                case NodeState.Running: // 실행 중이면 상태 유지
                    return NodeState.Running;

                case NodeState.Failure: // 실패하면 전체 실패 처리
                    return NodeState.Failure;
            }
        }

        return NodeState.Success; // 모든 자식이 성공하면 성공 반환
    }
}
