using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI interactText;

    void Awake()
    {
        UIManager.Instance.RegisterInteractionUI(this);
    }
}