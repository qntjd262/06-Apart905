using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadController : MonoBehaviour
{
    [Header("모드 설정")]
    public Constants.ESaveLoadType currentMode;

    [Header("UI 연결")]
    [SerializeField] private Transform slotContentParent;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private int maxSlots = 5;

    [Header("버튼 연결 (각각 따로 할당)")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button closeButton;

    private int selectedSlotIndex = -1;
    private SaveSlotUI[] spawnedSlots;

    public bool ReturnToPauseMenuOnClose { get; set; }

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
    }

    private void OnEnable()
    {
        selectedSlotIndex = -1;

        bool isSaveMode = (currentMode == Constants.ESaveLoadType.Save);
        saveButton.gameObject.SetActive(isSaveMode);
        loadButton.gameObject.SetActive(!isSaveMode);

        saveButton.interactable = false;
        loadButton.interactable = false;

        // 핵심: 현재 모드에 따라 제목 문자열을 결정한 뒤 UIManager로 넘긴다
        string windowTitle = isSaveMode ? "SAVE FILES" : "LOAD FILES";
        UIManager.Instance.OpenPopupWithEffects(windowTitle);

        RefreshSlots();
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePopupWithEffects();

            if (ReturnToPauseMenuOnClose)
            {
                ReturnToPauseMenuOnClose = false;
                UIManager.Instance.ShowPauseMenuWithoutChangingTimeScale();
            }
        }
    }
    private void RefreshSlots()
    {
        foreach (Transform child in slotContentParent)
        {
            Destroy(child.gameObject);
        }

        spawnedSlots = new SaveSlotUI[maxSlots];

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(saveSlotPrefab, slotContentParent);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();

            string slotDisplayName = (i == 0) ? "자동 저장" : $"슬롯 {i}";
            string tempDate = "2026-04-07 23:18";

            slotUI.Initialize(i, slotDisplayName, tempDate, OnSlotSelected);
            spawnedSlots[i] = slotUI;
        }
    }

    private void OnSlotSelected(int index)
    {
        if (currentMode == Constants.ESaveLoadType.Save && index == 0)
        {
            Debug.Log("자동 저장 슬롯에는 직접 저장할 수 없습니다.");
            saveButton.interactable = false;

            for (int i = 0; i < spawnedSlots.Length; i++) spawnedSlots[i].SetSelected(false);
            selectedSlotIndex = -1;
            return;
        }

        selectedSlotIndex = index;

        for (int i = 0; i < spawnedSlots.Length; i++)
        {
            spawnedSlots[i].SetSelected(i == index);
        }

        // 모드에 맞춰 현재 화면에 켜져 있는 버튼의 상호작용(클릭)을 활성화한다.
        if (currentMode == Constants.ESaveLoadType.Save) saveButton.interactable = true;
        else loadButton.interactable = true;
    }

    private void OnSaveButtonClick()
    {
        if (selectedSlotIndex == -1) return;
        Debug.Log($"UI 통제: [{selectedSlotIndex}]번 슬롯에 저장 지시 전달.");
        gameObject.SetActive(false);
    }

    private void OnLoadButtonClick()
    {
        if (selectedSlotIndex == -1) return;
        Debug.Log($"UI 통제: [{selectedSlotIndex}]번 슬롯 불러오기 지시 전달.");
        gameObject.SetActive(false);
    }

    private void OnCloseClick()
    {
        gameObject.SetActive(false);
    }
}
