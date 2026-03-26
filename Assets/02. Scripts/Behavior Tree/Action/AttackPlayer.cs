using UnityEngine;

public class AttackPlayer : ActionNode
{

    public AttackPlayer(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public override NodeState Evaluate()
    {
        

        return NodeState.Failure;
    }
}
