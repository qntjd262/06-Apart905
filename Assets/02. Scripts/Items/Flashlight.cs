using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light spotLight;

    private bool isOn = false;

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

    public void ToggleFlashlight()
    {
        isOn = !isOn;
        
        if(spotLight != null)
        {
            spotLight.enabled = isOn;
        }

        Debug.Log(isOn ? "손전등 켜짐" : "손전등 꺼짐");
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
