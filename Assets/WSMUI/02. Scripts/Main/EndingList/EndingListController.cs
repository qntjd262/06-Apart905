using UnityEngine;
using UnityEngine.UI;

public class EndingListController : BasePopupUI
{
    [Header("UI 연결")]
    [SerializeField] private Transform slotContentParent;
    [SerializeField] private int maxSlots = 5;

    [SerializeField] private Button returnButton;

    public bool ReturnToPauseMenuOnClose { get; set; }

    private void Awake()
    {
        returnButton.onClick.AddListener(OnReturnClick);
    }

    private void OnEnable()
    {
        ShowPanel();
    }

    private void OnDisable()
    {
        HidePanel();
    }

    private void OnSlotSelected(int index)
    {

    }

    private void OnReturnClick()
    {
        HidePanel();
    }
}
