using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 1. BasePopupUI 상속
public class SaveLoadController : BasePopupUI
{
    [Header("모드 설정")]
    public Constants.ESaveLoadType currentMode;

    [Header("UI 연결")]
    [SerializeField] private Transform slotContentParent;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private int maxSlots = 5;

    [Header("버튼 연결")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button returnButton;

    private int selectedSlotIndex = -1;
    private SaveSlotUI[] spawnedSlots;

    private void Awake()
    {
        popupPanel = this.gameObject; // 부모 변수 연결
        returnButton.onClick.AddListener(OnReturnClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
    }

    // [핵심] 외부에서 패널을 열 때 반드시 이 함수를 사용
    public void Open(Constants.ESaveLoadType mode)
    {
        currentMode = mode;
        ShowPanel(); // UIManager 스택 등록 및 SetActive(true) 자동 실행
    }

    private void OnEnable()
    {
        selectedSlotIndex = -1;

        bool isSaveMode = (currentMode == Constants.ESaveLoadType.Save);
        saveButton.gameObject.SetActive(isSaveMode);
        loadButton.gameObject.SetActive(!isSaveMode);

        saveButton.interactable = false;
        loadButton.interactable = false;

        string windowTitle = isSaveMode ? "저장하기" : "불러오기";
        if (UIManager.Instance != null) UIManager.Instance.OpenPopupWithEffects(windowTitle);

        RefreshSlots();
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null) UIManager.Instance.ClosePopupWithEffects();
    }

    private void RefreshSlots()
    {
        foreach (Transform child in slotContentParent) Destroy(child.gameObject);
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
            saveButton.interactable = false;
            for (int i = 0; i < spawnedSlots.Length; i++) spawnedSlots[i].SetSelected(false);
            selectedSlotIndex = -1;
            return;
        }

        selectedSlotIndex = index;
        for (int i = 0; i < spawnedSlots.Length; i++) spawnedSlots[i].SetSelected(i == index);

        if (currentMode == Constants.ESaveLoadType.Save) saveButton.interactable = true;
        else loadButton.interactable = true;
    }

    private void OnSaveButtonClick()
    {
        if (selectedSlotIndex == -1) return;
        Debug.Log($"UI 통제: [{selectedSlotIndex}]번 슬롯에 저장 지시 전달.");
        HidePanel();
    }

    private void OnLoadButtonClick()
    {
        if (selectedSlotIndex == -1) return;
        Debug.Log($"UI 통제: [{selectedSlotIndex}]번 슬롯 불러오기 지시 전달.");
        HidePanel();
    }

    private void OnReturnClick()
    {
        HidePanel(); 
    }
}