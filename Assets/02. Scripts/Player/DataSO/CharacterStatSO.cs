using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatSO", menuName = "Scriptable Objects/CharacterStatSO")]
public class CharacterStatSO : ScriptableObject
{
    [Header("기본 정보")]
    public string ID;
    public string Name;

    [Header("플레이어 기본 스탯")]
    public float Hp;
    public float Def;
    public float AttackPower;
    public float MaxStamina;
    public float MaxHunger;
    public float MaxThirst;
    public float MaxInfection;

    [Header("플레이어 스탯 증감율")]

    public float InfectionIncreaseRate;
    public float ThirstDecreaseRate;
    public float HungerDecreaseRate;
}
