using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light spotLight;

    private bool isOn = false;

    void Awake()
    {
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
