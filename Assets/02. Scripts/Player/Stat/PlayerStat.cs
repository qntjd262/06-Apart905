using Unity.VisualScripting;
using UnityEngine;
using System;
using Unity.AppUI.UI;

public class PlayerStat : MonoBehaviour
{
    [Header("기본 스탯(데이터 SO추가 시 변경 필요)")]
    [SerializeField] private float maxHp;
    [SerializeField] private float def;
    [SerializeField] private float attackPower;
    [SerializeField] private float maxStamina;

    [SerializeField] private float infectionIncreaseRate;
    [SerializeField] private float ThirstDecreaseRate;
    [SerializeField] private float HungerDecreaseRate;

    public float AttackPower
    {
        get { return attackPower; }
    }

    [Header("현재 생존 상태")]
    public StatCondition hp;
    public StatCondition stamina;
    public StatCondition hunger;
    public StatCondition thirst;
    public StatCondition infection;

    [Header("감염 상태 관리")]
    public int currentInfectionStage = 0;

    [Header("상태 제어")]
    public bool isInteracting = false;

    public event Action<bool> OnInfectionStateBool;
    public event Action<float, float> OnHpChanged;
    public event Action OnPlayerDeath;

    void Awake()
    {
        // 인벤토리 매니저의 Player 변수에 자기 자신(this)을 할당
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Player = this;
        }
    }

    void Start() // 데이터 매니저 참조는 Start가 안전합니다.
    {
        InitializeStats();
    }

    private void InitializeStats()
    {

        if (CharacterDataManager.Instance != null && CharacterDataManager.Instance.selectedCharacterSO != null)
        {
            CharacterStatSO myData = CharacterDataManager.Instance.selectedCharacterSO;

            maxHp = myData.Hp;
            def = myData.Def;
            attackPower = myData.AttackPower;
            maxStamina = myData.MaxStamina;
            infectionIncreaseRate = myData.InfectionIncreaseRate;
            ThirstDecreaseRate = myData.ThirstDecreaseRate;
            HungerDecreaseRate = myData.HungerDecreaseRate;

            Debug.Log($"가져온 캐릭터 데이터 : {myData.Name}");
        }
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
        PlayerGamemanager.OnGameStatChangeTime += DecreaseSurvivalStat;
    }

    void OnDisable()
    {
        PlayerGamemanager.OnGameStatChangeTime -= DecreaseSurvivalStat;
    }


    //몬스터에게 피격당할 시 호출되는 함수
    public void TakeDamage(float damage)
    {
        hp.DecreaseStat(damage);
        OnHpChanged?.Invoke(hp.currentValue, hp.maxValue);

        AddInfection();

        if (hp.currentValue <= 0)
        {
            Die();
        }
    }

    private void AddInfection()
    {
        infection.AddStat(infectionIncreaseRate);
        UpdateInfectionStage();
    }

    private void UpdateInfectionStage()
    {
        int targetStage = 0;

        if (infection.currentValue >= 80f) targetStage = 3;
        else if (infection.currentValue >= 50f) targetStage = 2;
        else if (infection.currentValue >= 30f) targetStage = 1;

        if (currentInfectionStage == 0 && targetStage > 0)
        {
            OnInfectionStateBool?.Invoke(true);
        }
        else if (currentInfectionStage == 1 && targetStage == 0)
        {
            OnInfectionStateBool?.Invoke(false);
        }
        currentInfectionStage = targetStage;
    }

    public void Die()
    {
        Debug.Log("플레이어 사망");

        this.enabled = false;
      
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenGameOverUI();
        }
        Time.timeScale = 0f;
        OnPlayerDeath?.Invoke();

    }

    //배고픔, 갈증 감소 함수
    private void DecreaseSurvivalStat()
    {
        hunger.DecreaseStat(HungerDecreaseRate);
        thirst.DecreaseStat(ThirstDecreaseRate);

        if (hunger.currentValue <= 0 || thirst.currentValue <= 0 || infection.currentValue >= 100f)
        {
            Debug.Log("허기 또는 갈증이 0 이하 체력깍임");
            hp.DecreaseStat(5f);
            //OnHpChanged?.Invoke(hp.currentValue, hp.maxValue);
            Debug.Log($"현재 체력 : {hp.currentValue}");
        }
    }
    public void ApplyEatableEffect(EatableType type, float value)
    {
        Debug.Log($"아이템 효과 발동 타입 : {type}, 수치 : {value}");

        StatCondition targetStat = null;

        switch (type)
        {
            case EatableType.Hunger: targetStat = hunger; break;
            case EatableType.Thirst: targetStat = thirst; break;
            case EatableType.Health: targetStat = hp; break;
            case EatableType.Stamina: targetStat = stamina; break;
            case EatableType.Infection: targetStat = infection; break;
        }
        if (targetStat != null)
        {
            //로그찍기 위함
            float before = targetStat.currentValue;

            if (type == EatableType.Infection) //감염도인 경우 수치 빼기
            {
                targetStat.currentValue -= value;
            }
            else //나머지는 수치 더하기
            {
                targetStat.currentValue += value;
            }

            //스탯이 0 ~ 최대값 범위를 벗어나지 않게 고정
            targetStat.currentValue = Mathf.Clamp(targetStat.currentValue, 0, targetStat.maxValue);
            Debug.Log($"{type} 변경 전 : {before} ->  변경 후 : {targetStat.currentValue}");

            if (type == EatableType.Health)
            {
                OnHpChanged?.Invoke(hp.currentValue, hp.maxValue);
            }
        }
    }
}
