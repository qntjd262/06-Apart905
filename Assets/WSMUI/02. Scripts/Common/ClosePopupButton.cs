using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClosePopupButton : MonoBehaviour
{
    [Header("Close target panel")]
    public GameObject targetPanel;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(CloseAndNotify);
    }

    private void CloseAndNotify()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }
}
