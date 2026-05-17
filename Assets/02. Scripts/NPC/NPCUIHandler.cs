using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCUIHandler : MonoBehaviour
{
    public static NPCUIHandler Instance;

    [Header("UI References")]
    public Image imageUI;
    public TextMeshProUGUI nametagUI;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowNPC(NPCdata data)
    {
        nametagUI.text = data.NpcName;
        imageUI.sprite = data.NpcImage;
        
    }
}
