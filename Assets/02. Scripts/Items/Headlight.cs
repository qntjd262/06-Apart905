using UnityEngine;

public class Headlight : MonoBehaviour
{
    [Header("라이트 컴포넌트")]
    public Light spotLight; 
    
    [Header("획득 시스템")]
    [SerializeField] private bool hasHeadlight = false;

    [Header("배터리 세팅")]
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float currentBattery = 100f;
    [SerializeField] private float consumptionRate = 2f; // 초당 차감될 배터리 양

    private bool isOn = false;

    public float CurrentBattery => currentBattery;
    public bool HasHeadlight => hasHeadlight;

    void Awake()
    {
        if (spotLight == null)
        {
            Light[] allLights = GetComponentsInChildren<Light>(true);
            foreach (Light light in allLights)
            {
                if (light.type == LightType.Spot)
                {
                    spotLight = light;
                    break;
                }
            }
        }
        
        currentBattery = maxBattery; // 시작할 때 배터리 완충
        ForceTurnOff();
    }

    void Update()
    {
        if (isOn)
        {
            ConsumeBattery();
        }
    }

    private void ConsumeBattery()
    {
        if (currentBattery > 0f)
        {
            currentBattery -= consumptionRate * Time.deltaTime;
            
            // 배터리가 바닥나면 자동으로 끔
            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                ForceTurnOff();
                Debug.Log("배터리가 방전되어 헤드라이트가 꺼졌습니다.");
            }
        }
    }

    public void ToggleFlashlight()
    {
        if(!hasHeadlight)
        {
            Debug.Log("헤드라이트를 획득하지 않아 사용할 수 없습니다.");
            return;
        }

        if(spotLight == null) return;

        if(!isOn && CurrentBattery <= 0f)
        {
            Debug.Log("배터리가 부족하여 사용할 수 없습니다.");
            return;
        }

        isOn = !isOn;
        spotLight.enabled = isOn;

        Debug.Log(isOn ? $"헤드라이트 켜짐 (남은 배터리: {currentBattery:F1}%)" : "헤드라이트 꺼짐");
    }

    public void ForceTurnOff()
    {
        isOn = false;
        if (spotLight != null)
        {
            spotLight.enabled = false;
        }
    }

    public void RechargeBattery()
    {
        currentBattery = maxBattery;
        Debug.Log("배터리가 100% 충전되었습니다!");
    }

    public void AcquireHeadlight()
    {
        hasHeadlight = true;
        Debug.Log("헤드라이트를 획득했습니다! 이제 사용할 수 있습니다.");
    }
}