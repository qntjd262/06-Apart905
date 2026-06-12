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
        popupPanel = this.gameObject;
        
        returnButton.onClick.AddListener(OnReturnClick);
    }

    private void OnEnable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenPopupWithEffects("엔딩 리스트");
        }
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePopupWithEffects();
        }
    }

    private void OnSlotSelected(int index)
    {

    }

    private void OnReturnClick()
    {
        HidePanel();
    }
}