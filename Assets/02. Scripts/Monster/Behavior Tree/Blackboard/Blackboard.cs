using UnityEngine;
using UnityEngine.AI;

public class Blackboard
{
    public GameObject Player { get; set; }
    public float Distance { get; set; }
    public GameObject Self { get; set; }
    public Animator Animator { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public bool IsAttacking { get; set; }
}
