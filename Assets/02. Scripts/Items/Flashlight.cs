using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("라이트 컴포넌트")]
    public Light spotLight; 

    [Header("배터리 세팅")]
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float currentBattery = 100f;
    [SerializeField] private float consumptionRate = 2f; // 초당 차감될 배터리 양

    private bool isOn = false;

    public float CurrentBattery => currentBattery;

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
                Debug.Log("배터리가 방전되어 손전등이 꺼졌습니다.");
            }
        }
    }

    public void ToggleFlashlight()
    {
        if (spotLight == null) return;

        if (!isOn && currentBattery <= 0f)
        {
            Debug.Log("배터리가 없어서 손전등을 켤 수 없습니다.");
            return;
        }

        isOn = !isOn;
        spotLight.enabled = isOn;

        Debug.Log(isOn ? $"손전등 켜짐 (남은 배터리: {currentBattery:F1}%)" : "손전등 꺼짐");
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
}