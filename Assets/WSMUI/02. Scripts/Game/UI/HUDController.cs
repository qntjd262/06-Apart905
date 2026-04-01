using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SurvivalGauge gauge;

    void Awake()
    {
        // Start가 아닌 Awake에서 컴포넌트를 캐싱해야 안전하다.
        gauge = GetComponent<SurvivalGauge>();
    }

    void Start()
    {
        InitHUD();
    }

    public void InitHUD()
    {
        if (GameManager.Instance == null || GameManager.Instance.SelectedCharacterData == null)
        {
            Debug.LogWarning("HUD: 초기화할 캐릭터 데이터가 존재하지 않습니다.");
            return;
        }

        var data = GameManager.Instance.SelectedCharacterData;

        // 플레이어 연동 시 주석 해제 필수
        // gauge.UpdateStamina(data.maxStamina, data.maxStamina, false);
        // gauge.UpdateHunger(data.maxHunger, data.maxHunger);
        // gauge.UpdateThirst(data.maxThirst, data.maxThirst);
        // gauge.UpdateSanity(0, data.maxSanity); 
    }
}