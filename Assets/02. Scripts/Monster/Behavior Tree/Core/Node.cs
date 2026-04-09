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

    public bool isFirstStart = true; // 노드의 첫 시작 구분

    public virtual void OnStart() { }

    public abstract NodeState OnUpdate();

    // 노드가 정지 시 호출
    public virtual void OnStop() => isFirstStart = true;

    public NodeState Evaluate()
    {
        if (isFirstStart)
        {
            OnStart();
            isFirstStart = false;
        }

        state = OnUpdate();

        if (state == NodeState.Failure || state == NodeState.Success)
        {
            OnStop();
        }

        return state;        
    }

}
