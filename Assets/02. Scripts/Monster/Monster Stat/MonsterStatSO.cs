using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStat", menuName = "Scriptable Objects/MonsterStatData")]
public class MonsterStatSO : ScriptableObject
{
    public float maxHP; // 체력
    public float damage; // 공격력
}
