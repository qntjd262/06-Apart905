using UnityEngine;
using UnityEngine.UI;

public class HeadlightUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("배터리 잔량을 표시할 Slider")]
    public Slider batterySlider;
    
    public GameObject uiPanel;

    private Headlight playerHeadlight;

    void Start()
    {
        playerHeadlight = FindFirstObjectByType<Headlight>();

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (playerHeadlight == null) return;

        if (playerHeadlight.hasHeadlight)
        {
            if (uiPanel != null && !uiPanel.activeSelf)
            {
                uiPanel.SetActive(true);
            }

            if (batterySlider != null)
            {
                batterySlider.value = playerHeadlight.CurrentBattery;
            }
        }
        else
        {
            if (uiPanel != null && uiPanel.activeSelf)
            {
                uiPanel.SetActive(false);
            }
        }
    }
}