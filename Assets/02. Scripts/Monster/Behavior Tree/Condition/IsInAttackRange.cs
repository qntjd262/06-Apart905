using UnityEngine;

public class IsInAttackRange : Node
{
    private Blackboard _blackboard;
    private GameObject _self;
    private GameObject _player;

    public IsInAttackRange(Blackboard blackboard)
    {
        _blackboard = blackboard;
        _self = _blackboard.Self;
    }

    public override NodeState OnUpdate()
    {
        _player = _blackboard.Player;

        if (Vector3.Distance(_self.transform.position, _player.transform.position) <= _blackboard.NavMeshAgent.stoppingDistance)
        {
            return NodeState.Success;
        }
        else
        {
            return NodeState.Failure;
        }
    }
}
