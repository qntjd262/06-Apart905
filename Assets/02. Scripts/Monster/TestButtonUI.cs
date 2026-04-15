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
    }

    public void SetPlayerSound()
    {
        var blackboard = monsterAI.blackboard;
        blackboard.Self.GetComponent<MonsterController>().CanHearPlayerSound(player.transform.position);
    }
}
