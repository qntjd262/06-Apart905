using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SurvivalGauge gauge;
    private PlayerStat playerStat;

    private bool isInitialized = false;
    private float initTime;

    [SerializeField] private TextMeshProUGUI timeText;

    void Awake()
    {
        gauge = GetComponent<SurvivalGauge>();
    }

    private void OnEnable()
    {
        TimeFlow.OnTimeChanged += UpdateTimeText;

        // [추가] 켜지는 시점에도 즉시 한 번 갱신 (구독 전에 이미 흐른 시간 반영)
        if (TimeFlow.Instance != null)
        {
            UpdateTimeText(TimeFlow.Instance.Days, TimeFlow.Instance.Hours, TimeFlow.Instance.MinutesInt);
        }
    }

    private void OnDisable()
    {
        TimeFlow.OnTimeChanged -= UpdateTimeText;
    }

    private void UpdateTimeText(int days, int hours, int minutes)
    {
        timeText.text = $"{days}일 {hours:00}:{minutes:00}";
    }

    public void InitHUD()
    {
        playerStat = FindFirstObjectByType<PlayerStat>();
        if (playerStat == null)
        {
            Debug.LogWarning("HUD: PlayerStat을 찾을 수 없습니다.");
            return;
        }

        UpdateGauges(false);

        isInitialized = true;
        initTime = Time.time;
    }

    void Update()
    {
        if (!isInitialized || playerStat == null || gauge == null) return;

        bool useSmooth = (Time.time - initTime) > 0.5f;

        UpdateGauges(useSmooth);
    }

    private void UpdateGauges(bool isSmooth)
    {
        gauge.UpdateStamina(playerStat.stamina.currentValue, playerStat.stamina.maxValue, isSmooth);
        gauge.UpdateHunger(playerStat.hunger.currentValue, playerStat.hunger.maxValue, isSmooth);
        gauge.UpdateThirst(playerStat.thirst.currentValue, playerStat.thirst.maxValue, isSmooth);
        gauge.UpdateSanity(playerStat.infection.currentValue, playerStat.infection.maxValue, isSmooth);
    }
}
