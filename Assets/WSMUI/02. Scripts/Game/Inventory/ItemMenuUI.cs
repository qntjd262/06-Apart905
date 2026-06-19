using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemMenuUI : MonoBehaviour
{
    public static ItemMenuUI Instance { get; private set; }

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button discardButton;

    private float _openTime;
    private int _targetSlotIndex;
    private bool _isTargetQuickSlot;
    private bool _initialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        Instance = this;

        if (menuPanel == null)
        {
            menuPanel = gameObject;
        }

        _initialized = true;
    }

    public void ShowMenu(SlotUI slot, InventorySlot slotData)
    {
        if (!_initialized)
        {
            Initialize();
        }

        if (slot == null || slotData == null || slotData.item == null)
        {
            return;
        }

        if (menuPanel == null || actionButtonText == null || actionButton == null || discardButton == null)
        {
            Debug.LogError("ItemMenuUI has missing UI references.", this);
            return;
        }

        InventoryUI inventoryUI = slot.GetComponentInParent<InventoryUI>();
        RectTransform boundaryRect = inventoryUI != null ? inventoryUI.transform as RectTransform : null;

        Canvas targetCanvas = slot.GetComponentInParent<Canvas>();
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (targetCanvas == null && uiManager != null)
        {
            targetCanvas = uiManager.CurrentCanvas;
        }
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
        }

        if (boundaryRect == null && targetCanvas != null)
        {
            boundaryRect = targetCanvas.transform as RectTransform;
        }

        if (boundaryRect != null)
        {
            transform.SetParent(boundaryRect, false);
        }

        menuPanel.SetActive(true);
        transform.SetAsLastSibling();

        _targetSlotIndex = slot.SlotIndex;
        _isTargetQuickSlot = slot.IsQuickSlot;

        if (slotData.item.itemType == ItemType.Eatable)
        {
            actionButtonText.text = "사용하기";
        }
        else if (slotData.item.itemType == ItemType.Equipable)
        {
            actionButtonText.text = "장착하기";
        }

        MoveToMousePosition(targetCanvas, boundaryRect);
        FixMenuPosition(boundaryRect);

        _openTime = Time.unscaledTime;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionClicked);

        discardButton.onClick.RemoveAllListeners();
        discardButton.onClick.AddListener(OnDiscardClicked);
    }

    private void MoveToMousePosition(Canvas targetCanvas, RectTransform boundaryRect)
    {
        RectTransform menuRect = menuPanel.GetComponent<RectTransform>();

        if (menuRect != null && boundaryRect != null)
        {
            Camera eventCamera = targetCanvas == null || targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(boundaryRect, Input.mousePosition, eventCamera, out Vector3 worldPoint))
            {
                menuRect.position = worldPoint;
                return;
            }
        }

        menuPanel.transform.position = Input.mousePosition;
    }

    private void FixMenuPosition(RectTransform boundaryRect)
    {
        RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
        if (menuRect == null || boundaryRect == null) return;

        Canvas.ForceUpdateCanvases();

        Vector3[] menuCorners = new Vector3[4];
        Vector3[] boundaryCorners = new Vector3[4];
        menuRect.GetWorldCorners(menuCorners);
        boundaryRect.GetWorldCorners(boundaryCorners);

        Vector3 offset = Vector3.zero;
        if (menuCorners[2].x > boundaryCorners[2].x) offset.x = boundaryCorners[2].x - menuCorners[2].x;
        if (menuCorners[0].x < boundaryCorners[0].x) offset.x = boundaryCorners[0].x - menuCorners[0].x;
        if (menuCorners[2].y > boundaryCorners[2].y) offset.y = boundaryCorners[2].y - menuCorners[2].y;
        if (menuCorners[0].y < boundaryCorners[0].y) offset.y = boundaryCorners[0].y - menuCorners[0].y;

        menuRect.position += offset;
    }

    private void OnActionClicked()
    {
        PlayerStat player = InventoryManager.Instance.Player;
        InventoryManager.Instance.UseItem(_targetSlotIndex, _isTargetQuickSlot, player);
        CloseMenu();
    }

    private void OnDiscardClicked()
    {
        InventoryManager.Instance.DiscardItem(_targetSlotIndex, _isTargetQuickSlot);
        CloseMenu();
    }

    public void CloseMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (menuPanel == null || !menuPanel.activeSelf) return;
        if (Time.unscaledTime - _openTime < 0.1f) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RectTransform panelRect = menuPanel.GetComponent<RectTransform>();
            bool isMouseInsideMenu = RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition);

            if (!isMouseInsideMenu)
            {
                CloseMenu();
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
