using UnityEngine;

public class Headlight : MonoBehaviour
{
    public Light spotLight;
    private bool isOn = false;

    public bool hasHeadlight { get; private set; } = false;

    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Header("Battery Settings")]
    [Tooltip("최대 배터리 양")]
    [SerializeField] private float maxBattery = 100f;
    [Tooltip("현재 배터리 양")]
    private float currentBattery;
    [Tooltip("초당 배터리 소모량")]
    [SerializeField] private float consumptionRate = 0.33f; 

    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBattery;

    void Awake()
    {
        currentBattery = maxBattery; 

        if (spotLight == null)
        {
            Light[] allLights = transform.root.GetComponentsInChildren<Light>(true);

            foreach (Light light in allLights)
            {
                if (light.gameObject.name == "Spotlight")
                {
                    spotLight = light;
                    break;
                }
            }
        }

        if (spotLight == null)
        {
            Debug.Log("Spotlight 컴포넌트를 찾을수가 없음");
        }
        
        ForceTurnOff();
    }

    void Start()
    {
        isOn = false;
        if (spotLight != null)
        {
            spotLight.gameObject.SetActive(true); 
            spotLight.enabled = false;           
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.LogWarning($"=== [Headlight] 키보드 F 입력 확인됨! 현재 배터리: {currentBattery}/{maxBattery} ===");
        }

        if (hasHeadlight && Input.GetKeyDown(toggleKey))
        {
            if (currentBattery > 0)
            {
                ToggleFlashlight();
            }
            else
            {
                Debug.LogWarning("배터리가 없어 헤드라이트를 켤 수 없습니다!");
            }
        }

        if (isOn)
        {
            ConsumeBattery();
        }
    }

    private void ConsumeBattery()
    {
        currentBattery -= consumptionRate * Time.deltaTime;
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        if (currentBattery <= 0f)
        {
            Debug.Log("헤드라이트 배터리가 방전되었습니다.");
            ForceTurnOff();
        }
    }
    public void UseBatteryItem(float amount = 100f)
    {
        if (!hasHeadlight)
        {
            Debug.LogWarning("헤드라이트가 없어 배터리를 사용할 수 없습니다.");
            return;
        }

        currentBattery += amount;
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);
        Debug.Log($"배터리를 사용했습니다! 현재 배터리: {currentBattery}/{maxBattery}");
    }

    public void AcquireHeadlight()
    {
        hasHeadlight = true;
        currentBattery = maxBattery;
        Debug.Log("헤드라이트를 획득했습니다! 이제 단축키로 라이트를 켤 수 있습니다.");
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;

        if (spotLight != null)
        {
            spotLight.enabled = isOn;
            Debug.Log($"불빛 상태 변경됨 -> 라이트 컴포넌트 활성화: {isOn}");
        }
        else
        {
            Debug.LogError("Spot Light 오브젝트가 인스펙터에 연결되지 않았습니다!");
        }
    }

    public void ForceTurnOff()
    {
        isOn = false;

        if (spotLight != null)
        {
            spotLight.enabled = false;
        }
    }
}
