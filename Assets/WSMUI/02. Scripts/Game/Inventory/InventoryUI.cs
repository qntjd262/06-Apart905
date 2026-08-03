using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    private SlotUI[] uiSlots;
    [SerializeField] private GameObject questSelectorObject;

    [Header("Character Info 연동")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI characterNameText;

    [Header("Item Info UI (Optional)")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private Image itemIconImage;

    [SerializeField] private SurvivalGauge inventorySurvivalGauge;

    private CanvasGroup _canvasGroup;

    private PlayerStat _playerStat;
    private float _openTime;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        UIManager.Instance.RegisterInventoryPanel(this.gameObject);
    }

    private void OnEnable()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
        // 1. [핵심] 1프레임 찌꺼기를 가리기 위한 마스킹 연출
        if (_canvasGroup != null)
        {
            _canvasGroup.DOKill(); // 기존 연출 강제 종료
            _canvasGroup.alpha = 0f; // 켜지자마자 강제 투명화 (여기서 1프레임 찌꺼기가 가려짐)
            _canvasGroup.DOFade(1f, 0.15f).SetUpdate(true); // 0.15초 동안 빠르게 나타남
        }

        // 묵직하게 다가오는 느낌을 주기 위한 미세한 스케일 연출 (0.95 -> 1.0)
        transform.DOKill();
        transform.localScale = Vector3.one * 0.95f;
        transform.DOScale(1f, 0.15f).SetEase(Ease.OutCubic).SetUpdate(true);

        // 2. 캐릭터 데이터 연동
        if (CharacterDataManager.Instance != null && CharacterDataManager.Instance.selectedCharacterSO != null)
        {
            CharacterStatSO selectedData = CharacterDataManager.Instance.selectedCharacterSO;

            if (portraitImage != null) portraitImage.sprite = selectedData.characterSprite;
            if (characterNameText != null) characterNameText.text = selectedData.Name;
        }

        // 3. 스탯 스냅 (CanvasGroup이 투명한 상태에서 게이지가 맞춰지므로 유저 눈에는 보이지 않음)
        if (_playerStat == null)
        {
            _playerStat = FindFirstObjectByType<PlayerStat>();
        }
        _openTime = Time.unscaledTime;

        if (_playerStat != null && inventorySurvivalGauge != null)
        {
            inventorySurvivalGauge.UpdateStamina(_playerStat.stamina.currentValue, _playerStat.stamina.maxValue, false);
            inventorySurvivalGauge.UpdateHunger(_playerStat.hunger.currentValue, _playerStat.hunger.maxValue, false);
            inventorySurvivalGauge.UpdateThirst(_playerStat.thirst.currentValue, _playerStat.thirst.maxValue, false);
            inventorySurvivalGauge.UpdateSanity(_playerStat.infection.currentValue, _playerStat.infection.maxValue, false);
        }
    }

    void Start()
    {
        uiSlots = slotsParent.GetComponentsInChildren<SlotUI>();

        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = false;
            uiSlots[i].IsStorageSlot = false;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnBagUpdated += RefreshUI;
            RefreshUI();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.inventoryPanel = this.gameObject;
        }
    }

    private void Update()
    {
        if (_playerStat == null || inventorySurvivalGauge == null) return;

        bool useSmooth = (Time.unscaledTime - _openTime) > 0.2f;

        inventorySurvivalGauge.UpdateStamina(_playerStat.stamina.currentValue, _playerStat.stamina.maxValue, useSmooth);
        inventorySurvivalGauge.UpdateHunger(_playerStat.hunger.currentValue, _playerStat.hunger.maxValue, useSmooth);
        inventorySurvivalGauge.UpdateThirst(_playerStat.thirst.currentValue, _playerStat.thirst.maxValue, useSmooth);
        inventorySurvivalGauge.UpdateSanity(_playerStat.infection.currentValue, _playerStat.infection.maxValue, useSmooth);

        if (infoPanel != null && infoPanel.activeSelf)
        {
            if (UIManager.Instance.storagePanel != null && UIManager.Instance.storagePanel.activeSelf)
            {
                infoPanel.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        if (infoPanel != null) infoPanel.SetActive(false);

        if (ItemMenuUI.Instance != null) ItemMenuUI.Instance.CloseMenu();
    }

    public void SetQuestSelectorActive(bool isActive)
    {
        if (questSelectorObject != null)
        {
            questSelectorObject.SetActive(isActive);
        }
    }

    public void ShowItemInfo(InventorySlot slotData)
    {
        if (slotData == null || slotData.IsEmpty || slotData.item == null)
        {
            if (infoPanel != null) infoPanel.SetActive(false);
            return;
        }

        if (infoPanel != null) infoPanel.SetActive(true);
        
        string displayName = !string.IsNullOrEmpty(slotData.item.Name) ? slotData.item.Name : 
                            (!string.IsNullOrEmpty(slotData.item.itemName) ? slotData.item.itemName : slotData.item.name);

        // ★ 설명글도 순간적인 타이밍 누락을 방지하기 위해 빈값 검사를 칩니다.
        string displayDesc = !string.IsNullOrEmpty(slotData.item.description) ? slotData.item.description : "";

        // UI에 안전하게 대입
        if (itemNameText != null) itemNameText.text = displayName;
        if (itemDescText != null) itemDescText.text = displayDesc;
        if (itemIconImage != null) itemIconImage.sprite = slotData.item.icon;
        
        Debug.Log($"선택한 아이템: {displayName}, 설명 텍스트: {displayDesc}", slotData.item);
    }

    private void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < InventoryManager.Instance.BagSlots.Length)
            {
                uiSlots[i].UpdateSlot(InventoryManager.Instance.BagSlots[i]);
            }
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnBagUpdated -= RefreshUI;
        }
    }
}
