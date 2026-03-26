using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("기본 스탯(데이터 SO추가 시 변경 필요)")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float def = 5f;
    [SerializeField] private float attackPower = 3f;
    [SerializeField] private float maxStamina = 20f;

    [SerializeField] private float infectionIncreaseRate = 1f;
    [SerializeField] private float ThirstDecreaseRate = 1.5f;
    [SerializeField] private float HungerDecreaseRate = 1f;

    public float AttackPower
    {
        get { return attackPower;}
    }

    [Header("현재 생존 상태")]
    public StatCondition hp;
    public StatCondition stamina;
    public StatCondition hunger;
    public StatCondition thirst;
    public StatCondition infection;

    void Awake()
    {
        //TODO : 캐릭터 선택 시 데이터매니저에 저장 -> 데이터매니저에 저장된 characterSO를 통해 스탯 초기화
        hp = new StatCondition(maxHp);
        stamina = new StatCondition(maxStamina);

        hunger = new StatCondition(100f);
        thirst = new StatCondition(100f);
        infection = new StatCondition(100f);
        infection.currentValue = 0f;
    }

    void OnEnable()
    {
        PlayerGamemanager.OnGameStatChangeTime += DecreasSurvivalStat;
    }

    //TODO : 좀비에게 피격 당할 시 증가
    public void AddInfection(float value)
    {
        infection.AddStat(value * infectionIncreaseRate);

        if(infection.currentValue >= 100f)
            Debug.Log("사망");
    }

    //배고픔, 갈증 감소 함수
    private void DecreasSurvivalStat()
    {
        hunger.DecreaseStat(HungerDecreaseRate);
        thirst.DecreaseStat(ThirstDecreaseRate);

        if(hunger.currentValue <= 0 || thirst.currentValue <= 0)
        {
            Debug.Log("허기 또는 갈증이 0 이하 체력깍임");
            hp.DecreaseStat(5f);
            Debug.Log($"현재 체력 : {hp.currentValue}");
        }
    }
}
