using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ItemMenuUI : Singleton<ItemMenuUI>
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button discardButton;

    private int _targetSlotIndex;
    private bool _isTargetQuickSlot;

    protected override void Awake()
    {
        base.Awake();
        menuPanel.SetActive(false);
    }

    public void ShowMenu(SlotUI slot, InventorySlot slotData)
    {
        Canvas mainCanvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        if (mainCanvas != null)
        {
            transform.SetParent(mainCanvas.transform, false);
        }
        transform.SetAsLastSibling();

        _targetSlotIndex = slot.SlotIndex;
        _isTargetQuickSlot = slot.IsQuickSlot;

        // 버튼 텍스트 설정
        if (slotData.item.itemType == ItemType.Eatable)
            actionButtonText.text = "사용하기";
        else if (slotData.item.itemType == ItemType.Equipable)
            actionButtonText.text = "장착하기";

        // 메뉴 위치를 마우스 위치로 이동
        menuPanel.transform.position = Input.mousePosition;

        // 2. 화면 밖으로 나가는 현상 방지 (Clamp)
        FixMenuPosition();
        menuPanel.SetActive(true);

        // 버튼 이벤트 초기화 및 등록
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => OnActionClicked());

        discardButton.onClick.RemoveAllListeners();
        discardButton.onClick.AddListener(() => OnDiscardClicked());
    }

    private void FixMenuPosition()
    {
        // RectTransform을 사용하여 메뉴가 화면을 벗어나지 않게 조정
        RectTransform rect = menuPanel.GetComponent<RectTransform>();

        // 화면 크기 가져오기
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

        // 현재 패널의 월드 위치를 기반으로 캔버스 내 좌표 계산
        Vector3 pos = menuPanel.transform.position;

        // 메뉴의 크기만큼 여유 공간 계산 (Pivot이 좌상단(0, 1)일 때 기준)
        float menuWidth = rect.rect.width;
        float menuHeight = rect.rect.height;

        // 오른쪽 화면 밖으로 나가는 경우
        if (pos.x + menuWidth > screenBounds.x)
            pos.x -= menuWidth;

        // 아래쪽 화면 밖으로 나가는 경우
        if (pos.y - menuHeight < 0)
            pos.y += menuHeight;

        menuPanel.transform.position = pos;
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

    public void CloseMenu() => menuPanel.SetActive(false);

    private void Update()
    {
        if (!menuPanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RectTransform panelRect = menuPanel.GetComponent<RectTransform>();

            // 현재 마우스 위치가 메뉴 패널(panelRect) 영역 안에 포함되어 있는지 검사
            bool isMouseInsideMenu = RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition);

            // 마우스가 메뉴 바깥을 클릭했을 때만 창을 닫음
            if (!isMouseInsideMenu)
            {
                CloseMenu();
            }
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    protected override void OnSceneUnloaded(Scene scene)
    {

    }
}