using UnityEngine;

public enum NodeState
{
    Success,
    Failure,
    Running
}

public abstract class Node
{
    protected NodeState _nodeState;
    public NodeState NodeState => _nodeState;
    
    /// <summary>
    /// 노드의 상태를 반환하는 메서드. Success, Failure, Running 중 하나를 반환
    /// </summary>
    /// <returns></returns>
    public abstract NodeState Evaluate();
}
