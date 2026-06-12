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
