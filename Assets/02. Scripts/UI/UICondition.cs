using UnityEngine;
using UnityEngine.UI;

public class ConditionUI : MonoBehaviour
{
    public PlayerStat playerStat; 

    [Header("UI Images")]
    public Image hpBarImage;
    public Image hungerBarImage;
    public Image thirstBarImage;
    public Image infectBarImage;
    public Image staminaImage;

    void Update()
    {
        if (playerStat == null) return;

        // 각 스탯의 비율을 계산하여 fillAmount에 적용
        if (hpBarImage) hpBarImage.fillAmount = GetPercentage(playerStat.hp);
        if (hungerBarImage) hungerBarImage.fillAmount = GetPercentage(playerStat.hunger);
        if (thirstBarImage) thirstBarImage.fillAmount = GetPercentage(playerStat.thirst);
        if (infectBarImage) infectBarImage.fillAmount = GetPercentage(playerStat.infection);
        if (staminaImage) staminaImage.fillAmount = GetPercentage(playerStat.stamina);
    }

    private float GetPercentage(StatCondition stat)
    {
        // 0으로 나누기 방지 및 0~1 사이 값 반환
        return (stat.maxValue > 0) ? Mathf.Clamp01(stat.currentValue / stat.maxValue) : 0;
    }
}
