using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SurvivalGauge gauge;
    private PlayerStat playerStat;

    private bool isInitialized = false;

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

        // 초기값으로 게이지 세팅
        if(gauge != null)
        {
            gauge.UpdateStamina(playerStat.stamina.currentValue, playerStat.stamina.maxValue, false);
            gauge.UpdateHunger(playerStat.hunger.currentValue, playerStat.hunger.maxValue);
            gauge.UpdateThirst(playerStat.thirst.currentValue, playerStat.thirst.maxValue);
            gauge.UpdateSanity(playerStat.infection.currentValue, playerStat.infection.maxValue);
        }

        isInitialized = true;
        Debug.Log("플레이어 스탯 ui 연동 완료");
    }

    void Update()
    {
        if (!isInitialized || playerStat == null || gauge == null) return;

        gauge.UpdateStamina(playerStat.stamina.currentValue, playerStat.stamina.maxValue);
        gauge.UpdateHunger(playerStat.hunger.currentValue, playerStat.hunger.maxValue);
        gauge.UpdateThirst(playerStat.thirst.currentValue, playerStat.thirst.maxValue);
        gauge.UpdateSanity(playerStat.infection.currentValue, playerStat.infection.maxValue);
    }
}