using UnityEngine;

public class MemorySequenceNode : CompositeNode
{
    private int _currentIndex;

    public MemorySequenceNode(params Node[] children) : base(children) { }

    public override NodeState OnUpdate()
    {
        for (int i = _currentIndex; i < _children.Count; i++)
        {
            Node child = _children[i];

            switch (child.Evaluate())
            {
                case NodeState.Success: // 성공하면 다음 노드로
                    continue;

                case NodeState.Running: // 실행 중이면 상태 유지
                    if (_currentChild != null && _currentChild != child)
                    {
                        _currentChild.OnStop();
                    }
                    _currentChild = child;
                    _currentIndex = i;
                    return NodeState.Running;

                case NodeState.Failure: // 실패하면 전체 실패 처리
                    OnStop();
                    return NodeState.Failure;
            }
        }

        OnStop();
        return NodeState.Success; // 모든 자식이 성공하면 성공 반환
    }

    public override void OnStop()
    {
        base.OnStop();
        _currentIndex = 0;
    }
}
