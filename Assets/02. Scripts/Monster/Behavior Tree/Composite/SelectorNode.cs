using UnityEngine;

/// <summary>
/// 실패하면 다음 노드, 하나라도 성공하면 전체 성공 처리하는 노드
/// </summary>
public class SelectorNode : CompositeNode
{
    public SelectorNode(params Node[] children) : base(children) { }

    public override NodeState OnUpdate()
    { 
        foreach (Node child in _children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success: // 하나라도 성공하면 성공 반환
                    _currentChild = null;
                    return NodeState.Success;

                case NodeState.Running: // 실행 중이면 상태 유지
                    if (_currentChild != null && _currentChild != child)
                    {
                        _currentChild.OnStop();
                    }
                    _currentChild = child;
                    return NodeState.Running;

                case NodeState.Failure: // 실패하면 다음 노드 평가
                    continue;
            }
        }

        return NodeState.Failure; // 모든 자식이 실패하면 실패 반환
    }
}
