using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadController : BasePopupUI
{
    [Header("모드 설정")]
    public Constants.ESaveLoadType currentMode;

    [Header("UI 연결")]
    [SerializeField] private Transform slotContentParent;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private int maxSlots = 5;

    [Header("저장 연출 UI")]
    [SerializeField] private GameObject saveNoticePanel;
    [SerializeField] private float saveDisplayDuration = 1.2f;

    [Header("버튼 연결")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button returnButton;

    private int selectedSlotIndex = -1;
    private SaveSlotUI[] spawnedSlots;
    private bool isInitialized = false;

    private void Awake()
    {
        popupPanel = this.gameObject;
        returnButton.onClick.AddListener(OnReturnClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
    }

    public void Open(Constants.ESaveLoadType mode, bool canReturn = true)
    {
        currentMode = mode;

        this.gameObject.SetActive(true);

        // [핵심 추가] 게임오버 등에서 취소를 못 하게 막아야 한다면 버튼을 완전히 끈다.
        // if (returnButton != null)
        // {
        //     returnButton.gameObject.SetActive(canReturn);
        // }

        if (!isInitialized || spawnedSlots == null || spawnedSlots.Length == 0 || spawnedSlots[0] == null)
        {
            foreach (Transform child in slotContentParent) Destroy(child.gameObject);
            InitializeSlots();
        }

        UpdateSlotData();

        ShowPanel();

        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }

    private void OnEnable()
    {
        selectedSlotIndex = -1;
        if (saveNoticePanel != null) saveNoticePanel.SetActive(false);

        bool isSaveMode = (currentMode == Constants.ESaveLoadType.Save);
        saveButton.gameObject.SetActive(isSaveMode);
        loadButton.gameObject.SetActive(!isSaveMode);

        saveButton.interactable = false;
        loadButton.interactable = false;

        string windowTitle = isSaveMode ? "저장하기" : "불러오기";
        if (UIManager.Instance != null) UIManager.Instance.OpenPopupWithEffects(windowTitle);
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null) UIManager.Instance.ClosePopupWithEffects();
    }

    private void InitializeSlots()
    {
        spawnedSlots = new SaveSlotUI[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(saveSlotPrefab, slotContentParent);
            spawnedSlots[i] = slotObj.GetComponent<SaveSlotUI>();
        }
        isInitialized = true;
    }

    private void UpdateSlotData()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            string slotDisplayName = (i == 0) ? "자동 저장" : $"슬롯 {i}";

            bool tempHasData = false;
            string tempDate = "빈 슬롯";

            // PlayerPrefs에 해당 슬롯 번호의 키가 있는지 진짜로 확인합니다.
            if (PlayerPrefs.HasKey($"SaveSlot_{i}"))
            {
                tempHasData = true;
                tempDate = PlayerPrefs.GetString($"SaveSlot_{i}_Date", "시간 정보 없음");
            }

            spawnedSlots[i].Initialize(i, slotDisplayName, tempDate, tempHasData, OnSlotSelected);
            spawnedSlots[i].SetSelected(false);
        }
    }

    private void OnSlotSelected(int index, bool hasData)
    {
        if (currentMode == Constants.ESaveLoadType.Save && index == 0)
        {
            ResetSelection();
            return;
        }

        if (currentMode == Constants.ESaveLoadType.Load && !hasData)
        {
            ResetSelection();
            return;
        }

        selectedSlotIndex = index;
        for (int i = 0; i < spawnedSlots.Length; i++)
            spawnedSlots[i].SetSelected(i == index);

        if (currentMode == Constants.ESaveLoadType.Save) saveButton.interactable = true;
        else loadButton.interactable = true;
    }

    private void ResetSelection()
    {
        saveButton.interactable = false;
        loadButton.interactable = false;
        for (int i = 0; i < spawnedSlots.Length; i++) spawnedSlots[i].SetSelected(false);
        selectedSlotIndex = -1;
    }

    private void OnSaveButtonClick()
    {
        if (selectedSlotIndex == -1) return;
        StartCoroutine(SaveProcessCoroutine());
    }

    private IEnumerator SaveProcessCoroutine()
    {
        if (saveNoticePanel != null) saveNoticePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(saveDisplayDuration);

        // [기존 코드] 시간 UI 기록
        PlayerPrefs.SetInt($"SaveSlot_{selectedSlotIndex}", 1);
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        PlayerPrefs.SetString($"SaveSlot_{selectedSlotIndex}_Date", currentDate);
        PlayerPrefs.Save();

        // --- [추가] 실제 게임 데이터 JSON 저장 실행 ---
        SaveManager.Instance.SaveGame(selectedSlotIndex);
        // ------------------------------------------

        UpdateSlotData();
        if (saveNoticePanel != null) saveNoticePanel.SetActive(false);
        HidePanel();
    }

    private void OnLoadButtonClick()
    {
        if (selectedSlotIndex == -1) return;
        StartCoroutine(LoadProcessCoroutine());
    }

    private IEnumerator LoadProcessCoroutine()
    {
        // [수정 2] 씬이 넘어가기 전에 일시정지 상태로 굳어버린 유니티의 물리 시간축을 강제로 정상화(1) 시킨다.
        Time.timeScale = 1f;

        // [수정 2] 현재 켜져 있는 일시정지 메뉴 패널을 찾아서 수동으로 꺼버린다. (UI 잔재 박멸)
        if (UIManager.Instance != null && UIManager.Instance.pauseMenuPanel != null)
        {
            UIManager.Instance.pauseMenuPanel.SetActive(false);
            UIManager.Instance.UnregisterUI(UIManager.Instance.pauseMenuPanel);
        }

        if (UIManager.Instance != null && UIManager.Instance.gameOverPanel != null)
        {
            UIManager.Instance.gameOverPanel.SetActive(false);
            UIManager.Instance.UnregisterUI(UIManager.Instance.gameOverPanel);
        }

        Constants.ESceneType targetScene = Constants.ESceneType.PrototypeGame;

        HidePanel();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(targetScene, true);
        }

        yield return null;
    }

    private void OnReturnClick()
    {
        System.Action closeAction = onCloseAction;
        onCloseAction = null;
        HidePanel();
        closeAction?.Invoke();
    }
}