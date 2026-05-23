using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SurvivalGauge gauge;
    private PlayerStat playerStat;

    private bool isInitialized = false;
    private float initTime;

    void Awake()
    {
        gauge = GetComponent<SurvivalGauge>();
    }

   public void InitHUD()
    {
        playerStat = FindFirstObjectByType<PlayerStat>();
        if (playerStat == null)
        {
            Debug.LogWarning("HUD: PlayerStat을 찾을 수 없습니다.");
            return;
        }

        UpdateGauges(false); // 즉시 적용

        isInitialized = true;
        initTime = Time.time; // 시간 기록
    }

    void Update()
    {
        if (!isInitialized || playerStat == null || gauge == null) return;

        // 핵심 방어: 시작하고 0.5초 동안은 스탯이 뒤늦게 100으로 차올라도 무조건 애니메이션 없이 스냅(false)시킨다.
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
