using UnityEngine;

public class Death : ActionNode
{
    public Death(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public override void OnStart()
    {
        Debug.Log("사망 애니메이션 실행");
        // 시작 시, 사망 애니메이션 실행
        _blackboard.Animator.OnDeath();
    }

    public override NodeState OnUpdate()
    {
        // 계속 Running를 반환해, 다른 노드로 안넘어가게
        return NodeState.Running;
    }
}
