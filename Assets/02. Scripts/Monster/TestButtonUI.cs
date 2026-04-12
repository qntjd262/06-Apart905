using UnityEngine;

public class TestButtonUI : MonoBehaviour
{
    public TestMonsterAI monsterAI;
    public GameObject player;
    public MonsterController monsterStat;

    public void SetAttacked()
    {
        var blackboard = monsterAI.blackboard;
        monsterStat.TakeDamage(monsterStat.monsterStatSO.damage, player);
        //blackboard.IsAttacked = true;
        //blackboard.Player = player;
    }
}
