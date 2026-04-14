using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SurvivalGauge gauge;
    private PlayerStat playerStat;

    void Awake()
    {
        gauge = GetComponent<SurvivalGauge>();
    }

    public void InitHUD()
    {
        if (GameManager.Instance == null || GameManager.Instance.SelectedCharacterData == null)
        {
            Debug.LogWarning("HUD: 초기화할 캐릭터 데이터가 존재하지 않습니다.");
            return;
        }

        playerStat = FindFirstObjectByType<PlayerStat>();
        if (playerStat == null)
        {
            Debug.LogWarning("HUD: PlayerStat을 찾을 수 없습니다.");
            return;
        }

        // 초기값으로 게이지 세팅
        gauge.UpdateStamina(playerStat.stamina.currentValue, playerStat.stamina.maxValue, false);
        gauge.UpdateHunger(playerStat.hunger.currentValue, playerStat.hunger.maxValue);
        gauge.UpdateThirst(playerStat.thirst.currentValue, playerStat.thirst.maxValue);
        gauge.UpdateSanity(playerStat.infection.currentValue, playerStat.infection.maxValue);
    }

    void Update()
    {
        if (playerStat == null || gauge == null) return;

        gauge.UpdateStamina(playerStat.stamina.currentValue, playerStat.stamina.maxValue);
        gauge.UpdateHunger(playerStat.hunger.currentValue, playerStat.hunger.maxValue);
        gauge.UpdateThirst(playerStat.thirst.currentValue, playerStat.thirst.maxValue);
        gauge.UpdateSanity(playerStat.infection.currentValue, playerStat.infection.maxValue);
    }
}