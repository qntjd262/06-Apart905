
using System;
using UnityEngine;
using UnityEngine.UI;


public class DamageUIController : MonoBehaviour
{
    [Header("연결 설정")]
    public PlayerStat playerStat;
    public Image bloodOverlay;

    [Header("효과 강도 설정")]
    [Range(0,1)] public float alphaLevel1 = 0.3f;
    [Range(0,1)] public float alphaLevel2 = 0.6f;
    [Range(0,1)] public float alphaLevel3 = 1.0f;

    [Header("깜빡임 속도 설정")]
    public float blinkSpeedLevel1 = 2f;
    public float blinkSpeedLevel2 = 4f;
    public float blinkSpeedLevel3 = 8f;

    private float currentTargetAlpha = 0f;
    private float currentBlinkSpeed = 0f;
    private bool isDanger = false;

    void Start()
    {
        if(playerStat != null)
        {
            playerStat.OnHpChanged += UpdateDamagUI;
        }
        SetAlpha(0);        
    }

    void OnDestroy()
    {
        if(playerStat != null)
        {
            playerStat.OnHpChanged -= UpdateDamagUI;
        }
    }

    void Update()
    {
        if (isDanger)
        {
            float alpha = Mathf.PingPong(Time.time * currentBlinkSpeed, currentTargetAlpha);
            SetAlpha(alpha);
        }
    }

    private void UpdateDamagUI(float currentHp, float maxHp)
    {
        float hpPercent = currentHp / maxHp;

        if(hpPercent <= 0.2f)
        {
            isDanger = true;
            currentTargetAlpha = alphaLevel3;
            currentBlinkSpeed = blinkSpeedLevel3;
        }
        else if(hpPercent <= 0.5f)
        {
            isDanger = true;
            currentTargetAlpha = alphaLevel2;
            currentBlinkSpeed = blinkSpeedLevel2;
        }
        else if(hpPercent <= 0.8f)
        {
            isDanger = true;
            currentTargetAlpha = alphaLevel1;
            currentBlinkSpeed = blinkSpeedLevel3;
        }
        else
        {
            isDanger = false;
            SetAlpha(0);
        }
    }

    private void SetAlpha(float alpha)
    {
        if(bloodOverlay != null)
        {
            Color c = bloodOverlay.color;
            c.a = alpha;
            bloodOverlay.color = c;
        }
    }
}
