
using UnityEngine;
using UnityEngine.UI;

public class DamageUIController : MonoBehaviour
{
    [Header("연결 설정")]
    public PlayerStat playerStat;
    public Image bloodOverlay;

    [Header("효과 강도 설정")]
    [Range(0,1)] public float alphaLevel1 = 0.3f;
    [Range(0,1)] public float alphaLevel2 = 0.3f;
    [Range(0,1)] public float alphaLevel3 = 0.3f;

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
