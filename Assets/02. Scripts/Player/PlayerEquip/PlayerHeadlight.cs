using UnityEngine;

public class PlayerHeadlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject headlightObject;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.E;

    // 헤드라이트 아이템을 획득했는지 여부
    public bool HasHeadlight { get; private set; } = false; 
    
    private bool isLightOn = false;

    void Start()
    {
        if (headlightObject != null)
        {
            headlightObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!HasHeadlight) return;

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleHeadlight();
        }
    }

    public void AcquireHeadlight()
    {
        HasHeadlight = true;
        Debug.Log("헤드라이트를 획득했습니다! 이제 E키로 켜고 끌 수 있습니다.");
    }

    private void ToggleHeadlight()
    {
        if (headlightObject == null) return;

        isLightOn = !isLightOn;
        headlightObject.SetActive(isLightOn);

        // 켜고 끌때 사운드 추가
    }
}
