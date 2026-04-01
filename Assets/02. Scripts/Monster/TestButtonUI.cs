using UnityEngine;

public class TestButtonUI : MonoBehaviour
{
    public TestMonsterAI monsterAI;

    public void SetAttacked()
    {
        var blackboard = monsterAI.blackboard;
        Debug.Log(monsterAI);
        Debug.Log(monsterAI.blackboard.IsAttacked);
        blackboard.IsAttacked = true;
    }
}
