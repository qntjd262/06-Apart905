
public class IsInAttackRange : Node
{
    private Blackboard _blackboard;

    public IsInAttackRange(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public override NodeState Evaluate()
    {
        // 공격이 가능하다면 Success 반환
        if (_blackboard.CanAttackPlayer) 
            return NodeState.Success;

        return NodeState.Failure;
    }
}
