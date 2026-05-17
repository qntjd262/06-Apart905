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
        if (returnButton != null)
        {
            returnButton.gameObject.SetActive(canReturn);
        }

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

            // [해결 핵심] 하드코딩을 제거하고, 각 슬롯 인덱스(i)에 맞는 세이브 데이터를 독립적으로 조회한다.
            // 아래는 실제 세이브 매니저(SaveManager)를 연동할 때 사용할 구조다.

            bool tempHasData = false;
            string tempDate = "빈 슬롯";

            // 예시: PlayerPrefs나 ES3, 혹은 일반 JSON 파일이 해당 인덱스에 존재하는지 체크
            // (지금은 세이브 매니저가 없으므로 파일이 없다고 가정하거나 아래처럼 테스트용 분기를 태운다)
            if (PlayerPrefs.HasKey($"SaveSlot_{i}"))
            {
                tempHasData = true;
                tempDate = PlayerPrefs.GetString($"SaveSlot_{i}_Date", "시간 정보 없음");
            }
            else
            {
                // 테스트용: 세이브 매니저 만들기 전까지 슬롯별로 다르게 보고 싶다면 
                // 인덱스별로 다른 값을 들고 있게 처리한다.
                if (i == 0) { tempHasData = true; tempDate = "2026-05-17 21:00 (자동)"; }
                else if (i == 1) { tempHasData = true; tempDate = "2026-05-17 21:30 (슬롯1)"; }
                else { tempHasData = false; tempDate = "빈 슬롯"; }
            }

            // 이제 각 슬롯UI 인스턴스는 철저하게 분리된 독립 데이터를 주입받는다.
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
        HidePanel();
    }
}