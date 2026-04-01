using System.Collections.Generic;

/// <summary>
/// 성공하면 다음 노드로, 실패하면 전체 실패 처리하는 노드
/// </summary>
public class SequenceNode : CompositeNode
{
    public SequenceNode(params Node[] children) : base(children) { }

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
