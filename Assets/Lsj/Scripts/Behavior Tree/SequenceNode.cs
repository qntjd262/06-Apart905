using System.Collections.Generic;

/// <summary>
/// 자식 노드 중 하나라도 실패하면 Failure 반환, 모두 성공하면 Success를 반환하는 노드
/// </summary>
public class SequenceNode : Node
{
    private List<Node> _children = new List<Node>();

    public SequenceNode(params Node[] children)
    {
        _children.AddRange(children);
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
