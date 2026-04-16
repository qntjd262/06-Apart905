using UnityEngine;
using UnityEngine.UI;

public class LayoutInitializer : MonoBehaviour
{
    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}