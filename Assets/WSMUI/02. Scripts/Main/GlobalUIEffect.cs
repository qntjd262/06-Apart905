using UnityEngine;
using TMPro;

public class GlobalUIEffectRegister : MonoBehaviour
{
    [SerializeField] private GameObject dim;
    [SerializeField] private GameObject bars;
    [SerializeField] private TextMeshProUGUI title;

    private void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterGlobalEffects(dim, bars, title);
        }
    }
}
