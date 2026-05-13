using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Transform slotsParent;
    private SlotUI[] uiSlots;
    [SerializeField] private GameObject questSelectorObject;

    [Header("Character Info 연동")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI characterNameText;

    [SerializeField] private SurvivalGauge inventorySurvivalGauge;
    private PlayerStat _playerStat;

    private void OnEnable()
    {
        // 1. 캐릭터 선택 창에서 고른 데이터 연동 (초상화 & 이름)
        if (CharacterDataManager.Instance != null && CharacterDataManager.Instance.selectedCharacterSO != null)
        {
            CharacterStatSO selectedData = CharacterDataManager.Instance.selectedCharacterSO;

            if (portraitImage != null) portraitImage.sprite = selectedData.characterSprite;
            if (characterNameText != null) characterNameText.text = selectedData.Name;
        }
        else
        {
            Debug.LogWarning("InventoryUI: CharacterDataManager에서 선택된 캐릭터 데이터를 찾을 수 없습니다.");
        }

        // 2. 실시간 스탯 연동을 위한 PlayerStat 캐싱
        if (_playerStat == null)
        {
            _playerStat = FindFirstObjectByType<PlayerStat>();
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
        // 3. 인벤토리가 열려 있는 동안 HUD처럼 실시간으로 게이지 갱신
        if (_playerStat == null || inventorySurvivalGauge == null) return;

        inventorySurvivalGauge.UpdateStamina(_playerStat.stamina.currentValue, _playerStat.stamina.maxValue, false);
        inventorySurvivalGauge.UpdateHunger(_playerStat.hunger.currentValue, _playerStat.hunger.maxValue);
        inventorySurvivalGauge.UpdateThirst(_playerStat.thirst.currentValue, _playerStat.thirst.maxValue);
        inventorySurvivalGauge.UpdateSanity(_playerStat.infection.currentValue, _playerStat.infection.maxValue);
    }
    public void SetQuestSelectorActive(bool isActive)
    {
        if (questSelectorObject != null)
        {
            questSelectorObject.SetActive(isActive);
        }
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