using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum NodeState
{
    Success,
    Failure,
    Running
}

public abstract class Node
{
    protected NodeState state; // 노드의 상태

    public Node parent;

    /// <summary>
    /// 노드의 상태를 반환하는 메서드. Success, Failure, Running 중 하나를 반환
    /// </summary>
    /// <returns></returns>
    public abstract NodeState OnUpdate();

    // 노드가 정지 시 호출
    public virtual void OnStop() { }

    public NodeState Evaluate()
    {
        state = OnUpdate();

        //if (state == NodeState.Failure || state == NodeState.Success)
        //{
        //    OnStop();
        //}

        return state;        
    }

}
