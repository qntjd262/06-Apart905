using Unity.Behavior;
using UnityEngine;

public class Test : MonoBehaviour
{
    private BehaviorGraphAgent _behaviorGraphAgent;

    private void Awake()
    {
        _behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
    }
}
