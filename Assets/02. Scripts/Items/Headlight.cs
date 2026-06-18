using UnityEngine;

public class Headlight : MonoBehaviour
{
    public Light spotLight;
    private bool isOn = false;

    public bool hasHeadlight { get; private set; } = false;

    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    void Awake()
    {
        if(spotLight == null)
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

        if(spotLight == null)
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
            spotLight.gameObject.SetActive(false); 
        }
    }

void Update()
{
    if (Input.GetKeyDown(KeyCode.F))
    {
        Debug.LogWarning($"=== [Headlight] 키보드 F 입력 확인됨! 현재 hasHeadlight 상태: {hasHeadlight} ===");
    }

    // 기존 작동 로직
    if (hasHeadlight && Input.GetKeyDown(toggleKey))
    {
        ToggleFlashlight();
    }
}

    public void AcquireHeadlight()
    {
        hasHeadlight = true;
        Debug.Log("헤드라이트를 획득했습니다! 이제 단축키로 라이트를 켤 수 있습니다.");
    }

private void ToggleFlashlight()
{
    isOn = !isOn;

    if (spotLight != null)
    {
        spotLight.gameObject.SetActive(isOn);
        
        Debug.Log($"불빛 상태 변경됨 -> 오브젝트 활성화: {isOn}");
    }
    else
    {
        Debug.LogError("Spot Light 오브젝트가 인스펙터에 연결되지 않았습니다!");
    }
}

    public void ForceTurnOff()
    {
        isOn = false;

        if(spotLight != null)
        {
            spotLight.enabled = false;
        }
    }
}
