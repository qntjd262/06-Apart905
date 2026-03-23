using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DoPatrol", story: "[Self] finds [Player]", category: "Action", id: "a0637795d9135f59b8e54ef48238dbac")]
public partial class DoPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    protected override Status OnStart()
    {
        Debug.Log("Patrol Start");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Running;
    }

    protected override void OnEnd()
    {
        Debug.Log("Patrol End");
    }
}

